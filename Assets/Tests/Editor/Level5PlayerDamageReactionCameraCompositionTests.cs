using System.Collections.Generic;
using Level5.Core;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Text.RegularExpressions;

/// <summary>
/// AUD-012 Phase 2b Slice 32: <see cref="SpawnCoordinator.BindHumanDamageReactionCamera"/> forwards
/// the scene's live shrink-camera resolver to every registered human participant's
/// <see cref="PlayerController"/>, replacing <c>PlayerDamageReactions</c>' former direct
/// <c>CameraManager.instance.Cameras[0]</c> read. Mirrors the shape
/// <see cref="Level5PlayerControllerArenaContextTests"/> already established for
/// <c>BindHumanArenaContext</c>.
/// </summary>
public class Level5PlayerDamageReactionCameraCompositionTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();
    private PlayerRegistry registry;

    [SetUp]
    public void SetUp()
    {
        registry = new PlayerRegistry();
    }

    [TearDown]
    public void TearDown()
    {
        foreach (GameObject go in spawned)
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        spawned.Clear();
    }

    private GameObject Spawn(string name)
    {
        GameObject go = new GameObject(name);
        spawned.Add(go);
        return go;
    }

    private PlayerIdentifier RegisterHumanParticipant(int pid)
    {
        GameObject actorGo = Spawn($"human-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(pid, false);
        identifier.player = actorGo;
        identifier.setPlayer(actorGo);
        registry.Add(identifier);
        return identifier;
    }

    private PlayerIdentifier RegisterCpuParticipant(int pid)
    {
        GameObject actorGo = Spawn($"cpu-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<AutoPlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(pid, true);
        identifier.autoPlayer = actorGo;
        identifier.setAutoPlayer(actorGo);
        registry.Add(identifier);
        return identifier;
    }

    private SpawnCoordinator MakeCoordinator()
    {
        return new SpawnCoordinator(
            new SpawnCoordinator.SpawnLocations(),
            registry,
            new ResolvedMatchRules(combatMode: CombatMode.Standard, enemiesEnabled: false, hardcore: false, enemiesOnly: false),
            new PlayerRoster(new PlayerSlot[0]),
            GameModeId.None);
    }

    [Test]
    public void BindHumanDamageReactionCamera_HumanParticipant_ReceivesTheResolver()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        Camera camera = Spawn("camera").AddComponent<Camera>();

        coordinator.BindHumanDamageReactionCamera(() => camera);

        IPlayerDamageReactionHost host = human.playerController;
        Assert.That(host.GetShrinkCamera(), Is.SameAs(camera));
    }

    [Test]
    public void BindHumanDamageReactionCamera_CpuParticipant_IsUnaffected()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        RegisterHumanParticipant(pid: 0);
        PlayerIdentifier cpu = RegisterCpuParticipant(pid: 1);

        Assert.DoesNotThrow(() => coordinator.BindHumanDamageReactionCamera(() => null));

        Assert.IsNull(cpu.playerController, "a CPU participant has no PlayerController to bind a damage reaction camera to");
    }

    [Test]
    public void BindHumanDamageReactionCamera_ResolverIsLiveNotSnapshotted()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        Camera first = Spawn("camera-1").AddComponent<Camera>();
        Camera second = Spawn("camera-2").AddComponent<Camera>();
        Camera current = first;

        coordinator.BindHumanDamageReactionCamera(() => current);

        IPlayerDamageReactionHost host = human.playerController;
        Assert.That(host.GetShrinkCamera(), Is.SameAs(first));

        current = second;

        Assert.That(
            host.GetShrinkCamera(),
            Is.SameAs(second),
            "the composed resolver must be invoked live on every shrink, not captured once at bind time");
    }

    [Test]
    public void BindHumanDamageReactionCamera_UnboundOrNullReturningResolver_IsSafe()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);

        IPlayerDamageReactionHost unboundHost = human.playerController;
        Assert.That(unboundHost.GetShrinkCamera(), Is.Null, "no BindHumanDamageReactionCamera call yet must resolve to null, not throw");

        coordinator.BindHumanDamageReactionCamera(() => null);

        Assert.That(unboundHost.GetShrinkCamera(), Is.Null);
    }

    [Test]
    public void BindHumanDamageReactionCamera_HumanMissingPlayerController_LogsAndContinues()
    {
        // A broken player prefab (no PlayerController) must fail closed on that one participant
        // rather than throwing and aborting the whole binding pass.
        SpawnCoordinator coordinator = MakeCoordinator();
        GameObject actorGo = Spawn("human-actor-no-controller");
        actorGo.AddComponent<CharacterProfile>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, false);
        identifier.player = actorGo;
        registry.Add(identifier);

        LogAssert.Expect(LogType.Error, new Regex("no PlayerController"));
        Assert.DoesNotThrow(() => coordinator.BindHumanDamageReactionCamera(() => null));
    }
}
