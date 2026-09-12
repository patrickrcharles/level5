using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 37: <c>PlayerController</c>'s last <c>Assembly-CSharp</c> dependency,
/// <c>MatchRuntime</c>, is now reached only through a bound <see cref="IPlayerMatchRuntime"/> -
/// <c>GameLevelManager</c>'s explicit forwarding implementation over <c>MatchRuntime</c>, delivered by
/// <see cref="SpawnCoordinator.BindHumanMatchRuntime"/> from <c>GameLevelManager.Awake</c>'s spawn pass.
///
/// These tests cover what that inversion made load-bearing: the forwarding boundary genuinely stays
/// live rather than becoming a value snapshotted at composition time, the composition path reaches
/// every human and skips every CPU (mirroring the shape already established for
/// <see cref="Level5PlayerInputReaderCompositionTests"/>'s <c>BindHumanLegacyTouchMovement</c>
/// coverage), and <c>PlayerController</c>'s own <c>InitializeInput</c>/<c>Start</c> behave correctly
/// whether or not a provider ever arrives. <c>MatchRuntime</c>'s own legacy-fallback rules matrix is
/// not re-tested here - see <c>Level5MatchBridgeParityTests</c> for that.
/// </summary>
public class Level5PlayerMatchRuntimeCompositionTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();
    private GameOptionsSnapshot gameOptionsSnapshot;

    [SetUp]
    public void SetUp()
    {
        gameOptionsSnapshot = GameOptionsSnapshot.Capture();
        ActiveMatch.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        ActiveMatch.Clear();
        gameOptionsSnapshot.Restore();

        foreach (GameObject go in spawned)
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        spawned.Clear();
    }

    private GameObject Spawn(string name, bool active = true)
    {
        GameObject go = new GameObject(name);
        go.SetActive(active);
        spawned.Add(go);
        return go;
    }

    /// <summary>
    /// Built inactive so Unity never calls <c>GameLevelManager.Awake()</c> - that method does
    /// scene-dependent setup (spawn-location lookups, singleton assignment, spawning) none of these
    /// tests want to trigger just to reach the explicit <see cref="IPlayerMatchRuntime"/> members below,
    /// which read nothing but the static <c>MatchRuntime</c> regardless of whether <c>Awake</c> ran.
    /// </summary>
    private GameLevelManager SpawnUnstartedGameLevelManager()
    {
        return Spawn("game-level-manager", active: false).AddComponent<GameLevelManager>();
    }

    // ==================== contract mapping ====================

    [Test]
    public void GameLevelManagerImplementsIPlayerMatchRuntime()
    {
        GameLevelManager manager = SpawnUnstartedGameLevelManager();

        Assert.That(manager, Is.InstanceOf<IPlayerMatchRuntime>());
    }

    // ==================== live forwarding proof ====================

    [Test]
    public void RulesForwardsLiveToMatchRuntimeRatherThanASnapshot()
    {
        IPlayerMatchRuntime runtime = SpawnUnstartedGameLevelManager();

        GameOptions.enemiesEnabled = false;
        bool before = runtime.Rules.EnemiesEnabled;

        GameOptions.enemiesEnabled = true;
        bool after = runtime.Rules.EnemiesEnabled;

        Assert.That(before, Is.False);
        Assert.That(
            after,
            Is.True,
            "IPlayerMatchRuntime.Rules must read MatchRuntime.Rules live at the point of use, not a "
            + "value captured when the boundary was first resolved - this fails if the implementation "
            + "is ever changed to answer from a field instead of the static MatchRuntime.Rules.");
    }

    [Test]
    public void CustomCameraForwardsToMatchRuntime()
    {
        IPlayerMatchRuntime runtime = SpawnUnstartedGameLevelManager();

        Assert.That(runtime.CustomCamera, Is.EqualTo(MatchRuntime.CustomCamera));
    }

    [Test]
    public void LocalInputSlotForForwardsTheSameAnswerAsMatchRuntime()
    {
        IPlayerMatchRuntime runtime = SpawnUnstartedGameLevelManager();

        Assert.That(runtime.LocalInputSlotFor(0), Is.EqualTo(MatchRuntime.LocalInputSlotFor(0)));
    }

    // ==================== composition: SpawnCoordinator.BindHumanMatchRuntime ====================

    [Test]
    public void BindHumanMatchRuntime_HumanParticipant_ReceivesTheExactProviderInstance()
    {
        FakeMatchRuntime fake = new FakeMatchRuntime();
        PlayerRegistry registry = new PlayerRegistry();
        PlayerIdentifier human = RegisterHumanParticipant(registry, pid: 0);

        MakeCoordinator(registry).BindHumanMatchRuntime(fake);

        Assert.That(GetPrivateField(human.playerController, "matchRuntime"), Is.SameAs(fake));
    }

    [Test]
    public void BindHumanMatchRuntime_MultipleHumans_AllReceiveTheSameInstance()
    {
        FakeMatchRuntime fake = new FakeMatchRuntime();
        PlayerRegistry registry = new PlayerRegistry();
        PlayerIdentifier first = RegisterHumanParticipant(registry, pid: 0);
        PlayerIdentifier second = RegisterHumanParticipant(registry, pid: 1);

        MakeCoordinator(registry).BindHumanMatchRuntime(fake);

        Assert.That(GetPrivateField(first.playerController, "matchRuntime"), Is.SameAs(fake));
        Assert.That(GetPrivateField(second.playerController, "matchRuntime"), Is.SameAs(fake));
    }

    [Test]
    public void BindHumanMatchRuntime_CpuParticipant_IsUnaffected()
    {
        PlayerRegistry registry = new PlayerRegistry();
        RegisterHumanParticipant(registry, pid: 0);
        PlayerIdentifier cpu = RegisterCpuParticipant(registry, pid: 1);

        Assert.DoesNotThrow(() => MakeCoordinator(registry).BindHumanMatchRuntime(new FakeMatchRuntime()));

        Assert.IsNull(cpu.playerController, "a CPU participant has no PlayerController to bind match runtime to");
    }

    [Test]
    public void BindHumanMatchRuntime_HumanMissingPlayerController_LogsAndContinues()
    {
        // Same fail-closed shape as the other BindHuman* passes: one broken participant must not abort
        // binding for the humans registered after it.
        PlayerRegistry registry = new PlayerRegistry();
        GameObject actorGo = Spawn("human-actor-no-controller");
        actorGo.AddComponent<CharacterProfile>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, false);
        identifier.player = actorGo;
        registry.Add(identifier);
        PlayerIdentifier intact = RegisterHumanParticipant(registry, pid: 1);
        FakeMatchRuntime fake = new FakeMatchRuntime();

        LogAssert.Expect(LogType.Error, new Regex("no PlayerController"));
        Assert.DoesNotThrow(() => MakeCoordinator(registry).BindHumanMatchRuntime(fake));

        Assert.That(GetPrivateField(intact.playerController, "matchRuntime"), Is.SameAs(fake));
    }

    // ==================== PlayerController input behavior ====================

    [Test]
    public void InitializeInput_HumanWithBoundProvider_PassesTheParticipantIdToLocalInputSlotForUnchanged()
    {
        // LocalInputSlotAnswer is deliberately invalid (-1): InitializeInput must still consult the
        // bound provider (proving the pass-through) and then take the pre-existing "invalid slot" exit,
        // rather than this test needing the real PlayerControlsProvider acquisition path.
        GameObject actorGo = Spawn("human-actor");
        PlayerController controller = actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(2, false);
        FakeMatchRuntime fake = new FakeMatchRuntime { LocalInputSlotAnswer = -1 };
        controller.BindMatchRuntime(fake);

        LogAssert.Expect(LogType.Error, new Regex("could not resolve a human input slot"));
        InvokePrivate(controller, "InitializeInput");

        Assert.That(
            fake.LastRequestedPlayerId,
            Is.EqualTo(2),
            "the participant id must reach LocalInputSlotFor unchanged");
        Assert.That(controller.enabled, Is.False, "the pre-existing invalid-slot guard must still fire");
    }

    [Test]
    public void InitializeInput_UnboundHumanController_FailsClosedWithoutRestoringAStaticFallback()
    {
        GameObject actorGo = Spawn("human-actor");
        PlayerController controller = actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, false);

        LogAssert.Expect(LogType.Error, new Regex("no bound IPlayerMatchRuntime"));
        InvokePrivate(controller, "InitializeInput");

        Assert.That(controller.enabled, Is.False);
        Assert.That(GetPrivateField(controller, "matchRuntimeRequiredButMissing"), Is.EqualTo(true));
    }

    [Test]
    public void Start_UnboundHumanController_ReturnsBeforeDereferencingTheMissingProvider()
    {
        // CharacterProfile is deliberately NOT added to this actor: if Start() ever regressed to
        // continue past the missing-provider guard, the very next line it would reach
        // (movementSpeed = characterProfile.Speed) throws a NullReferenceException instead of this
        // assertion failing quietly - the guard's absence would be impossible to miss.
        GameObject actorGo = Spawn("human-actor");
        PlayerController controller = actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, false);

        LogAssert.Expect(LogType.Error, new Regex("no bound IPlayerMatchRuntime"));
        Assert.DoesNotThrow(
            () => InvokePrivate(controller, "Start"),
            "Start() must stop before its later MatchRuntime reads once InitializeInput reports a "
            + "missing provider, not throw a NullReferenceException");

        Assert.That(controller.enabled, Is.False);
    }

    [Test]
    public void InitializeInput_CpuParticipant_FailsOnTheCpuGuardWithoutRequiringAProvider()
    {
        // The CPU guard must run before the match-runtime requirement, so a CPU PlayerController still
        // fails for being CPU rather than for a missing provider it was never going to receive.
        GameObject actorGo = Spawn("cpu-actor");
        PlayerController controller = actorGo.AddComponent<PlayerController>();
        PlayerIdentifier identifier = actorGo.AddComponent<PlayerIdentifier>();
        identifier.setIds(0, true);

        LogAssert.Expect(LogType.Error, new Regex("cannot own input for a CPU player"));
        InvokePrivate(controller, "InitializeInput");

        Assert.That(controller.enabled, Is.False);
        Assert.That(
            GetPrivateField(controller, "matchRuntimeRequiredButMissing"),
            Is.EqualTo(false),
            "the CPU guard must exit before the match-runtime guard ever runs");
    }

    // ==================== helpers ====================

    private PlayerIdentifier RegisterHumanParticipant(PlayerRegistry registry, int pid)
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

    private PlayerIdentifier RegisterCpuParticipant(PlayerRegistry registry, int pid)
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

    private static SpawnCoordinator MakeCoordinator(PlayerRegistry registry)
    {
        return new SpawnCoordinator(
            new SpawnCoordinator.SpawnLocations(),
            registry,
            new ResolvedMatchRules(combatMode: CombatMode.Standard, enemiesEnabled: false, hardcore: false, enemiesOnly: false),
            new PlayerRoster(new PlayerSlot[0]),
            GameModeId.None);
    }

    private static object GetPrivateField(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"{target.GetType().Name} must declare a field named '{fieldName}'");
        return field.GetValue(target);
    }

    private static void InvokePrivate(object target, string methodName)
    {
        MethodInfo method = target.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, $"{target.GetType().Name} must declare {methodName}()");
        method.Invoke(target, null);
    }

    // ==================== test double ====================

    private sealed class FakeMatchRuntime : IPlayerMatchRuntime
    {
        public ResolvedMatchRules Rules { get; set; } =
            new ResolvedMatchRules(combatMode: CombatMode.None, enemiesEnabled: false, hardcore: false, enemiesOnly: false);

        public bool CustomCamera { get; set; }

        public bool LevelHasSevenPointers { get; set; }

        public int LocalInputSlotAnswer { get; set; } = -1;

        public int LastRequestedPlayerId { get; private set; } = int.MinValue;

        public int LocalInputSlotFor(int playerId)
        {
            LastRequestedPlayerId = playerId;
            return LocalInputSlotAnswer;
        }
    }
}
