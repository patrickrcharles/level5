using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 21: <see cref="PlayerController"/>'s basketball-rim vector and no-Terrain
/// drop-shadow ground height now arrive through explicit composition
/// (<see cref="PlayerController.BindArenaContext"/>, called by
/// <see cref="SpawnCoordinator.BindHumanArenaContext"/> from <c>GameLevelManager.Start()</c> once
/// <c>ArenaBootstrap</c> has resolved the final rim) instead of reading
/// <c>GameLevelManager.instance.BasketballRimVector</c>/<c>TerrainHeight</c> directly. Mirrors the
/// shape <see cref="Level5BasketballGroundHeightProviderTests"/> already established for
/// <c>BasketBall</c>'s identical no-Terrain fallback.
/// </summary>
public class Level5PlayerControllerArenaContextTests
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

    private static object GetPrivateField(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"{target.GetType().Name} must declare a field named '{fieldName}'");
        return field.GetValue(target);
    }

    private static float InvokeResolveDropShadowHeight(PlayerController controller)
    {
        MethodInfo resolve = typeof(PlayerController).GetMethod("ResolveDropShadowHeight", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(resolve, "PlayerController must declare ResolveDropShadowHeight()");
        return (float)resolve.Invoke(controller, null);
    }

    // ==================== BindArenaContext ====================

    [Test]
    public void BindArenaContext_StoresRimVectorAndProvider()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeGroundHeightProvider provider = new FakeGroundHeightProvider { GroundHeight = 4f };
        Vector3 rim = new Vector3(1f, 2f, 3f);

        controller.BindArenaContext(rim, provider);

        Assert.AreEqual(rim, GetPrivateField(controller, "bballRimVector"));
        Assert.AreSame(provider, GetPrivateField(controller, "groundHeightProvider"));
    }

    // ==================== ResolveDropShadowHeight ====================

    [Test]
    public void ResolveDropShadowHeight_NoActiveTerrain_ReadsTheBoundProviderLiveNotSnapshotted()
    {
        // Proves the critical compatibility invariant: binding a provider reference must not snapshot
        // its value. GameLevelManager's own terrainHeight changes after spawn time (its Start() sets
        // it from the primary participant's actual Y) - PlayerController.Update() must observe that
        // later value, not whatever GroundHeight returned at bind time.
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeGroundHeightProvider provider = new FakeGroundHeightProvider { GroundHeight = 10f };
        controller.BindArenaContext(Vector3.zero, provider);

        float beforeChange = InvokeResolveDropShadowHeight(controller);
        Assert.That(beforeChange, Is.EqualTo(10.02f).Within(0.0001f));

        provider.GroundHeight = 250f;
        float afterChange = InvokeResolveDropShadowHeight(controller);

        Assert.That(afterChange, Is.EqualTo(250.02f).Within(0.0001f),
            "PlayerController must read IGroundHeightProvider.GroundHeight live at the point of use, "
            + "not a value captured when the provider was bound");
    }

    [Test]
    public void ResolveDropShadowHeight_NoBoundProvider_LogsAndDoesNotThrow()
    {
        // A composition-timing gap (BindArenaContext never called) must surface as an explicit,
        // narrow composition error - not a NullReferenceException, and not a silent fallback to
        // another global.
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();

        LogAssert.Expect(LogType.Error, new Regex("no bound ground-height provider"));
        Assert.DoesNotThrow(() => InvokeResolveDropShadowHeight(controller));
    }

    [Test]
    public void ResolveDropShadowHeight_NoBoundProvider_LogsOnlyOnceAcrossRepeatedCalls()
    {
        // An airborne player with no active Terrain re-enters this branch every Update() frame for the
        // whole arc - a missing provider must not flood the console once per frame.
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();

        LogAssert.Expect(LogType.Error, new Regex("no bound ground-height provider"));
        InvokeResolveDropShadowHeight(controller);

        // No further LogAssert.Expect: a second unexpected Debug.LogError fails an EditMode test by
        // default, so a regression that re-logs every call fails here.
        InvokeResolveDropShadowHeight(controller);
        InvokeResolveDropShadowHeight(controller);
    }

    // ==================== SpawnCoordinator.BindHumanArenaContext ====================

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
    public void BindHumanArenaContext_HumanParticipant_ReceivesRimVectorAndProvider()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier human = RegisterHumanParticipant(pid: 0);
        FakeGroundHeightProvider provider = new FakeGroundHeightProvider { GroundHeight = 7f };
        Vector3 rim = new Vector3(5f, 0f, -5f);

        coordinator.BindHumanArenaContext(rim, provider);

        Assert.AreEqual(rim, GetPrivateField(human.playerController, "bballRimVector"));
        Assert.AreSame(provider, GetPrivateField(human.playerController, "groundHeightProvider"));
    }

    [Test]
    public void BindHumanArenaContext_MultipleHumans_AllReceiveTheSameContext()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        PlayerIdentifier first = RegisterHumanParticipant(pid: 0);
        PlayerIdentifier second = RegisterHumanParticipant(pid: 1);
        FakeGroundHeightProvider provider = new FakeGroundHeightProvider { GroundHeight = 2f };
        Vector3 rim = new Vector3(1f, 1f, 1f);

        coordinator.BindHumanArenaContext(rim, provider);

        Assert.AreEqual(rim, GetPrivateField(first.playerController, "bballRimVector"));
        Assert.AreEqual(rim, GetPrivateField(second.playerController, "bballRimVector"));
        Assert.AreSame(provider, GetPrivateField(first.playerController, "groundHeightProvider"));
        Assert.AreSame(provider, GetPrivateField(second.playerController, "groundHeightProvider"));
    }

    [Test]
    public void BindHumanArenaContext_CpuParticipant_IsUnaffected()
    {
        SpawnCoordinator coordinator = MakeCoordinator();
        RegisterHumanParticipant(pid: 0);
        PlayerIdentifier cpu = RegisterCpuParticipant(pid: 1);
        FakeGroundHeightProvider provider = new FakeGroundHeightProvider { GroundHeight = 2f };

        Assert.DoesNotThrow(() => coordinator.BindHumanArenaContext(Vector3.one, provider));

        Assert.IsNull(cpu.playerController, "a CPU participant has no PlayerController to bind arena context to");
    }

    [Test]
    public void BindHumanArenaContext_HumanMissingPlayerController_LogsAndContinues()
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
        FakeGroundHeightProvider provider = new FakeGroundHeightProvider { GroundHeight = 2f };

        LogAssert.Expect(LogType.Error, new Regex("no PlayerController"));
        Assert.DoesNotThrow(() => coordinator.BindHumanArenaContext(Vector3.one, provider));
    }

    // ==================== test double ====================

    private sealed class FakeGroundHeightProvider : IGroundHeightProvider
    {
        public float GroundHeight { get; set; }
    }
}
