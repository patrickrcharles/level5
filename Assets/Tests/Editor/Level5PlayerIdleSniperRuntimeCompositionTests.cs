using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 33: <see cref="SpawnCoordinator.BindHumanIdleSniperRuntime"/> forwards the
/// scene's live idle-sniper runtime resolver to every registered human participant's
/// <see cref="PlayerController"/>, replacing that controller's former direct
/// <c>SniperManager.instance</c> read. Mirrors the shape
/// <see cref="Level5PlayerDamageReactionCameraCompositionTests"/> already established for
/// <c>BindHumanDamageReactionCamera</c>.
/// </summary>
public class Level5PlayerIdleSniperRuntimeCompositionTests
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
                UnityEngine.Object.DestroyImmediate(go);
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

    private static Func<IPlayerIdleSniperRuntime> GetBoundReader(PlayerController controller)
    {
        FieldInfo field = typeof(PlayerController).GetField("idleSniperRuntimeReader", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, "PlayerController must declare idleSniperRuntimeReader");
        return (Func<IPlayerIdleSniperRuntime>)field.GetValue(controller);
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
    public void BindHumanIdleSniperRuntime_HumanParticipant_ReceivesTheResolver()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        FakeIdleSniperRuntime runtime = new FakeIdleSniperRuntime();

        coordinator.BindHumanIdleSniperRuntime(() => runtime);

        Func<IPlayerIdleSniperRuntime> reader = GetBoundReader(human.playerController);
        Assert.That(reader, Is.Not.Null);
        Assert.That(reader.Invoke(), Is.SameAs(runtime));
    }

    [Test]
    public void BindHumanIdleSniperRuntime_CpuParticipant_IsUnaffected()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        RegisterHumanParticipant(pid: 0);
        PlayerIdentifier cpu = RegisterCpuParticipant(pid: 1);

        Assert.DoesNotThrow(() => coordinator.BindHumanIdleSniperRuntime(() => null));

        Assert.IsNull(cpu.playerController, "a CPU participant has no PlayerController to bind an idle sniper runtime to");
    }

    [Test]
    public void BindHumanIdleSniperRuntime_ResolverIsLiveNotSnapshotted()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        FakeIdleSniperRuntime first = new FakeIdleSniperRuntime();
        FakeIdleSniperRuntime second = new FakeIdleSniperRuntime();
        IPlayerIdleSniperRuntime current = first;

        coordinator.BindHumanIdleSniperRuntime(() => current);

        Func<IPlayerIdleSniperRuntime> reader = GetBoundReader(human.playerController);
        Assert.That(reader.Invoke(), Is.SameAs(first));

        current = second;

        Assert.That(
            reader.Invoke(),
            Is.SameAs(second),
            "the composed resolver must be invoked live on every check, not captured once at bind time");
    }

    [Test]
    public void BindHumanIdleSniperRuntime_UnboundResolver_ResolvesAsNoRuntime()
    {
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);

        Func<IPlayerIdleSniperRuntime> reader = GetBoundReader(human.playerController);

        Assert.That(reader, Is.Null, "no BindHumanIdleSniperRuntime call yet must leave the reader unbound - equivalent to no sniper runtime");
    }

    [Test]
    public void BindHumanIdleSniperRuntime_HumanMissingPlayerController_LogsAndContinues()
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
        Assert.DoesNotThrow(() => coordinator.BindHumanIdleSniperRuntime(() => null));
    }

    // ==================== test double ====================

    private sealed class FakeIdleSniperRuntime : IPlayerIdleSniperRuntime
    {
        public bool Locked { get; set; }

        public IEnumerator GetInstantKillRoutine(float shootDelay)
        {
            yield break;
        }
    }
}
