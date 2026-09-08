using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 24: <see cref="CallBallToPlayer"/>'s call-enabled policy now reads an
/// explicitly bound <see cref="ResolvedMatchRules"/> (<see cref="CallBallToPlayer.BindMatchRules"/>,
/// called by <see cref="SpawnCoordinator"/> from both the human and the CPU registration path)
/// instead of reading <c>MatchRuntime.Rules</c> in its own <c>Start()</c> - the change that let the
/// component move into <c>Level5.Player</c>.
///
/// The policy itself is unchanged and is asserted here as behaviour rather than as a bound
/// reference: enabled by default, off for Hardcore + EnemiesOnly, back on for a point contest.
/// Mirrors <see cref="Level5ShotMeterOwnershipTests"/>'s shape - the SpawnCoordinator_* tests drive
/// the real private RegisterHuman/RegisterCpu composition path rather than a stand-in.
/// </summary>
public class Level5CallBallToPlayerMatchRulesTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();
    private PlayerRegistry registry;
    private SpawnCoordinator coordinator;
    private ResolvedMatchRules coordinatorRules;
    private MethodInfo registerHuman;
    private MethodInfo registerCpu;

    [SetUp]
    public void SetUp()
    {
        // CallBallToPlayer no longer reads MatchRuntime at all; clearing still keeps the rest of the
        // composition path (InitializeHumanProfile, PrepareCpuMatchContext) deterministic regardless
        // of what ran before this test.
        ActiveMatch.Clear();

        registry = new PlayerRegistry();

        // Hardcore + EnemiesOnly, deliberately not the default: a participant that ends up with a
        // fresh default ResolvedMatchRules instead of this exact match's rules would leave
        // CallEnabled true and fail the composition tests below.
        coordinatorRules = Rules(hardcore: true, enemiesOnly: true);
        coordinator = new SpawnCoordinator(
            new SpawnCoordinator.SpawnLocations(),
            registry,
            coordinatorRules,
            new PlayerRoster(new PlayerSlot[0]),
            GameModeId.None);

        registerHuman = typeof(SpawnCoordinator).GetMethod("RegisterHuman", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(registerHuman, "SpawnCoordinator.RegisterHuman must exist");
        registerCpu = typeof(SpawnCoordinator).GetMethod("RegisterCpu", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(registerCpu, "SpawnCoordinator.RegisterCpu must exist");
    }

    [TearDown]
    public void TearDown()
    {
        ActiveMatch.Clear();

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

    private static ResolvedMatchRules Rules(bool hardcore = false, bool enemiesOnly = false, ShotRule shotRule = ShotRule.Any)
    {
        return new ResolvedMatchRules(hardcore: hardcore, enemiesOnly: enemiesOnly, shotRule: shotRule);
    }

    /// <summary>
    /// <c>pullSpeed</c> is <c>internal</c>, and since Slice 24 that means internal to
    /// <c>Level5.Player</c> rather than to <c>Assembly-CSharp</c> - this fixture can no longer name it
    /// directly. Read by reflection rather than widening the component's contract for a test: nothing
    /// outside the component ever referenced it, and the migration deliberately changes no visibility.
    /// </summary>
    private static float PullSpeedOf(CallBallToPlayer callBall)
    {
        FieldInfo field = typeof(CallBallToPlayer).GetField("pullSpeed", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, "CallBallToPlayer must declare a pullSpeed field");
        return (float)field.GetValue(callBall);
    }

    private static void InvokeStart(CallBallToPlayer callBall)
    {
        MethodInfo start = typeof(CallBallToPlayer).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(start, "CallBallToPlayer must declare Start()");
        start.Invoke(callBall, null);
    }

    private CallBallToPlayer StartedWith(ResolvedMatchRules rules)
    {
        CallBallToPlayer callBall = Spawn("call-ball").AddComponent<CallBallToPlayer>();
        callBall.BindMatchRules(rules);
        InvokeStart(callBall);
        return callBall;
    }

    // ==================== call-enabled policy ====================

    [Test]
    public void Start_OrdinaryRules_LeavesCallingTheBallEnabled()
    {
        Assert.IsTrue(StartedWith(Rules()).CallEnabled);
    }

    [TestCase(true, false, TestName = "Start_HardcoreWithoutEnemiesOnly_LeavesCallingTheBallEnabled")]
    [TestCase(false, true, TestName = "Start_EnemiesOnlyWithoutHardcore_LeavesCallingTheBallEnabled")]
    public void Start_OnlyOneHalfOfTheGate_LeavesCallingTheBallEnabled(bool hardcore, bool enemiesOnly)
    {
        Assert.IsTrue(StartedWith(Rules(hardcore: hardcore, enemiesOnly: enemiesOnly)).CallEnabled);
    }

    [Test]
    public void Start_HardcoreAndEnemiesOnly_DisablesCallingTheBall()
    {
        Assert.IsFalse(StartedWith(Rules(hardcore: true, enemiesOnly: true)).CallEnabled);
    }

    [TestCase(ShotRule.ThreePoint)]
    [TestCase(ShotRule.FourPoint)]
    [TestCase(ShotRule.SevenPoint)]
    [TestCase(ShotRule.AllRanges)]
    public void Start_HardcoreAndEnemiesOnlyInAPointContest_ReEnablesCallingTheBall(ShotRule shotRule)
    {
        Assert.IsTrue(StartedWith(Rules(hardcore: true, enemiesOnly: true, shotRule: shotRule)).CallEnabled);
    }

    [Test]
    public void Start_AlwaysResetsLockedAndPullSpeed()
    {
        CallBallToPlayer callBall = Spawn("call-ball").AddComponent<CallBallToPlayer>();
        callBall.Locked = true;
        callBall.BindMatchRules(Rules());

        InvokeStart(callBall);

        Assert.IsFalse(callBall.Locked);
        Assert.That(PullSpeedOf(callBall), Is.EqualTo(2.3f).Within(0.0001f));
    }

    // ==================== binding contract ====================

    [Test]
    public void Start_WithNoBoundRules_FailsClosedAndReportsTheCompositionError()
    {
        // An improperly composed instance must not reach back into MatchRuntime, and must not disable
        // the whole player object - only calling the ball.
        CallBallToPlayer callBall = Spawn("call-ball").AddComponent<CallBallToPlayer>();

        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match rules"));
        InvokeStart(callBall);

        Assert.IsFalse(callBall.CallEnabled);
        Assert.IsTrue(callBall.enabled, "failing closed must disable calling the ball, not the component");
        Assert.IsTrue(callBall.gameObject.activeSelf);
    }

    [Test]
    public void BindMatchRules_Null_IsRejected()
    {
        CallBallToPlayer callBall = Spawn("call-ball").AddComponent<CallBallToPlayer>();

        LogAssert.Expect(LogType.Error, new Regex("null match rules"));
        callBall.BindMatchRules(null);

        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match rules"));
        InvokeStart(callBall);
    }

    [Test]
    public void BindMatchRules_SecondCall_KeepsTheOriginalRules()
    {
        CallBallToPlayer callBall = Spawn("call-ball").AddComponent<CallBallToPlayer>();
        callBall.BindMatchRules(Rules(hardcore: true, enemiesOnly: true));

        // Including a null second call: it must report "already bound", not "remaining unbound".
        LogAssert.Expect(LogType.Error, new Regex("already has bound match rules"));
        callBall.BindMatchRules(null);
        LogAssert.Expect(LogType.Error, new Regex("already has bound match rules"));
        callBall.BindMatchRules(Rules());

        InvokeStart(callBall);
        Assert.IsFalse(callBall.CallEnabled, "the first, Hardcore + EnemiesOnly binding must still be the one in force");
    }

    // ==================== SpawnCoordinator composition ====================

    private GameObject SpawnHumanParticipantWithCallBall(int pid)
    {
        GameObject actorGo = Spawn($"human-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<PlayerController>();
        actorGo.AddComponent<PlayerIdentifier>();
        actorGo.AddComponent<CallBallToPlayer>();
        return actorGo;
    }

    private GameObject SpawnCpuParticipantWithCallBall(int pid)
    {
        GameObject actorGo = Spawn($"cpu-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<AutoPlayerController>();
        actorGo.AddComponent<PlayerIdentifier>();
        actorGo.AddComponent<CallBallToPlayer>();
        return actorGo;
    }

    /// <summary>
    /// The coordinator's rules are Hardcore + EnemiesOnly, so a correctly composed participant's
    /// <c>Start()</c> disables calling the ball with no error logged. The two ways this can go wrong
    /// are both caught: never binding at all logs a composition error (an unexpected
    /// <c>Debug.LogError</c> fails an EditMode test), and binding some other, default rules object
    /// would leave <c>CallEnabled</c> true.
    /// </summary>
    private static void AssertComposedWithThisMatchsRules(GameObject participant)
    {
        CallBallToPlayer callBall = participant.GetComponent<CallBallToPlayer>();
        InvokeStart(callBall);
        Assert.IsFalse(callBall.CallEnabled);
    }

    [Test]
    public void SpawnCoordinator_RegisterHuman_BindsThisMatchsResolvedRulesToCallBallToPlayer()
    {
        GameObject actorGo = SpawnHumanParticipantWithCallBall(pid: 0);

        registerHuman.Invoke(coordinator, new object[] { actorGo, 0, null });

        AssertComposedWithThisMatchsRules(actorGo);
    }

    [Test]
    public void SpawnCoordinator_RegisterCpu_BindsThisMatchsResolvedRulesToCallBallToPlayer()
    {
        GameObject actorGo = SpawnCpuParticipantWithCallBall(pid: 1);

        registerCpu.Invoke(coordinator, new object[] { actorGo, 1 });

        AssertComposedWithThisMatchsRules(actorGo);
    }

    [Test]
    public void SpawnCoordinator_ParticipantWithoutCallBallToPlayer_IsSkippedSilently()
    {
        // Modelled on Lockdown's defender prefab (cpu_player_defense_oldreal), which carries
        // AutoPlayerDefense rather than AutoPlayerController and no CallBallToPlayer at all: that is
        // authored composition, not a defect, and must not log or throw. PlayerIdentifier.Actor is
        // null for such a participant, which the existing bind helpers already tolerate.
        GameObject defender = Spawn("cpu-defender");
        defender.AddComponent<CharacterProfile>();
        defender.AddComponent<AutoPlayerDefense>();
        defender.AddComponent<PlayerIdentifier>();

        Assert.DoesNotThrow(() => registerCpu.Invoke(coordinator, new object[] { defender, 1 }));
        Assert.IsNull(defender.GetComponent<CallBallToPlayer>());
    }
}
