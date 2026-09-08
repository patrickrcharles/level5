using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 25: <see cref="PlayerHealth"/>'s regeneration gate now reads an explicitly
/// bound <see cref="ResolvedMatchRules"/> (<see cref="PlayerHealth.BindMatchRules"/>, called by
/// <see cref="SpawnCoordinator"/> from both the human and the CPU registration path) instead of
/// reading <c>MatchRuntime.Rules</c> in its own <c>Update()</c> - the change that let the component
/// move into <c>Level5.Player</c>.
///
/// The gate itself is unchanged and is asserted here as behaviour: regeneration starts only under a
/// rule set that enables it, and damage, death and clamping keep working with no rules bound at all.
/// Mirrors <see cref="Level5CallBallToPlayerMatchRulesTests"/>'s shape - the SpawnCoordinator_* tests
/// drive the real private RegisterHuman/RegisterCpu composition path rather than a stand-in.
/// </summary>
public class Level5PlayerHealthMatchRulesTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();
    private PlayerRegistry registry;
    private SpawnCoordinator coordinator;
    private MethodInfo registerHuman;
    private MethodInfo registerCpu;

    [SetUp]
    public void SetUp()
    {
        // PlayerHealth no longer reads MatchRuntime at all; clearing still keeps the rest of the
        // composition path (InitializeHumanProfile, PrepareCpuMatchContext) deterministic regardless
        // of what ran before this test.
        ActiveMatch.Clear();

        registry = new PlayerRegistry();

        // Enemies enabled, deliberately not the default: a participant that ends up with a fresh
        // default ResolvedMatchRules instead of this exact match's rules would not regenerate, and a
        // participant that bound nothing would log a composition error - both fail the tests below.
        coordinator = new SpawnCoordinator(
            new SpawnCoordinator.SpawnLocations(),
            registry,
            Rules(enemiesEnabled: true),
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

    private static ResolvedMatchRules Rules(
        bool enemiesEnabled = false,
        bool obstaclesEnabled = false,
        SniperMode sniper = SniperMode.None)
    {
        return new ResolvedMatchRules(
            enemiesEnabled: enemiesEnabled,
            obstaclesEnabled: obstaclesEnabled,
            sniper: sniper);
    }

    private static void Invoke(PlayerHealth health, string method)
    {
        MethodInfo info = typeof(PlayerHealth).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(info, $"PlayerHealth must declare {method}()");
        info.Invoke(health, null);
    }

    /// <summary>
    /// Whether <c>Update()</c> started the block regeneration coroutine. The coroutine's first
    /// segment (setting this flag) runs synchronously inside <c>StartCoroutine</c>, so this is the
    /// deterministic signal that the gate opened - no timing, and no widening of the component's
    /// contract just to observe it.
    /// </summary>
    private static bool StartedRegeneratingBlock(PlayerHealth health)
    {
        FieldInfo field = typeof(PlayerHealth).GetField("regenerateBlock", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, "PlayerHealth must declare a regenerateBlock field");
        return (bool)field.GetValue(health);
    }

    /// <summary>
    /// A component whose Awake has run, so health/block/special hold their authored maxima and the
    /// only thing left deciding whether regeneration begins is the match-rule gate.
    /// </summary>
    private PlayerHealth Awoken(string name)
    {
        PlayerHealth health = Spawn(name).AddComponent<PlayerHealth>();
        Invoke(health, "Awake");
        return health;
    }

    private PlayerHealth StartedWith(ResolvedMatchRules rules)
    {
        PlayerHealth health = Awoken("player-health");
        health.BindMatchRules(rules);
        Invoke(health, "Start");
        health.SpendBlock(5f);
        return health;
    }

    // ==================== regeneration gate ====================

    [Test]
    public void Update_WithNoEnablingRule_DoesNotStartRegeneration()
    {
        PlayerHealth health = StartedWith(Rules());

        Invoke(health, "Update");

        Assert.IsFalse(StartedRegeneratingBlock(health));
    }

    [TestCase(true, false, SniperMode.None, TestName = "Update_EnemiesEnabled_StartsRegeneration")]
    [TestCase(false, true, SniperMode.None, TestName = "Update_ObstaclesEnabled_StartsRegeneration")]
    [TestCase(false, false, SniperMode.Bullet, TestName = "Update_SniperBullet_StartsRegeneration")]
    [TestCase(false, false, SniperMode.Laser, TestName = "Update_SniperLaser_StartsRegeneration")]
    [TestCase(false, false, SniperMode.MachineGun, TestName = "Update_SniperMachineGun_StartsRegeneration")]
    public void Update_WithAnEnablingRule_StartsRegeneration(bool enemies, bool obstacles, SniperMode sniper)
    {
        PlayerHealth health = StartedWith(Rules(enemiesEnabled: enemies, obstaclesEnabled: obstacles, sniper: sniper));

        Invoke(health, "Update");

        Assert.IsTrue(StartedRegeneratingBlock(health));
    }

    // ==================== binding contract ====================

    [Test]
    public void BindMatchRules_FirstValidCall_IsWhatTheGateReads()
    {
        PlayerHealth health = Awoken("player-health");
        health.BindMatchRules(Rules(enemiesEnabled: true));

        Invoke(health, "Start");
        health.SpendBlock(5f);
        Invoke(health, "Update");

        Assert.IsTrue(StartedRegeneratingBlock(health));
    }

    [Test]
    public void BindMatchRules_Null_IsRejected()
    {
        PlayerHealth health = Awoken("player-health");

        LogAssert.Expect(LogType.Error, new Regex("null match rules"));
        health.BindMatchRules(null);

        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match rules"));
        Invoke(health, "Start");
    }

    [Test]
    public void BindMatchRules_SecondCall_KeepsTheOriginalRules()
    {
        PlayerHealth health = Awoken("player-health");
        health.BindMatchRules(Rules());

        // Including a null second call: it must report "already bound", not "remaining unbound".
        LogAssert.Expect(LogType.Error, new Regex("already has bound match rules"));
        health.BindMatchRules(null);
        LogAssert.Expect(LogType.Error, new Regex("already has bound match rules"));
        health.BindMatchRules(Rules(enemiesEnabled: true));

        Invoke(health, "Start");
        health.SpendBlock(5f);
        Invoke(health, "Update");

        Assert.IsFalse(
            StartedRegeneratingBlock(health),
            "the first, non-regenerating binding must still be the one in force");
    }

    // ==================== missing composition ====================

    [Test]
    public void Start_WithNoBoundRules_ReportsTheCompositionErrorOnce()
    {
        PlayerHealth health = Awoken("player-health");

        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match rules"));
        Invoke(health, "Start");

        // Update() must not repeat the report every frame, and must not disable anything - an
        // unexpected Debug.LogError from either call would fail this test on its own.
        health.SpendBlock(5f);
        Invoke(health, "Update");
        Invoke(health, "Update");

        Assert.IsFalse(StartedRegeneratingBlock(health));
        Assert.IsTrue(health.enabled, "missing rules must skip regeneration, not disable the component");
        Assert.IsTrue(health.gameObject.activeSelf);
    }

    [Test]
    public void UnboundHealth_StillTakesDamageClampsAndDies()
    {
        PlayerHealth health = Awoken("player-health");
        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match rules"));
        Invoke(health, "Start");

        bool died = false;
        health.OnDied += () => died = true;

        Assert.IsFalse(health.TakeDamage(40f));
        Assert.That(health.Health, Is.EqualTo(60f).Within(0.0001f));

        health.Heal(1000f);
        Assert.That(health.Health, Is.EqualTo(health.MaxHealth).Within(0.0001f), "healing past max must still clamp");

        Assert.IsTrue(health.TakeDamage(500f));
        Assert.IsTrue(health.IsDead);
        Assert.IsTrue(died);
        Assert.That(health.Health, Is.EqualTo(0f).Within(0.0001f));

        Invoke(health, "Update");
        Assert.IsTrue(health.IsDead, "an unbound component must keep its death latch");
    }

    // ==================== SpawnCoordinator composition ====================

    private GameObject SpawnHumanParticipantWithHealth(int pid)
    {
        GameObject actorGo = Spawn($"human-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<PlayerController>();
        actorGo.AddComponent<PlayerIdentifier>();
        AddHealthChild(actorGo);
        return actorGo;
    }

    private GameObject SpawnCpuParticipantWithHealth(int pid)
    {
        GameObject actorGo = Spawn($"cpu-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<AutoPlayerController>();
        actorGo.AddComponent<PlayerIdentifier>();
        AddHealthChild(actorGo);
        return actorGo;
    }

    /// <summary>
    /// On a child, not the root: every live consumer resolves this component with
    /// <c>GetComponentInChildren&lt;PlayerHealth&gt;()</c>, so composition must find it there too.
    /// </summary>
    private PlayerHealth AddHealthChild(GameObject participant)
    {
        GameObject child = new GameObject("health");
        child.transform.SetParent(participant.transform);
        PlayerHealth health = child.AddComponent<PlayerHealth>();
        Invoke(health, "Awake");
        return health;
    }

    /// <summary>
    /// The coordinator's rules enable enemies, so a correctly composed participant regenerates with
    /// no error logged. The two ways this can go wrong are both caught: never binding at all logs a
    /// composition error in Start() (an unexpected <c>Debug.LogError</c> fails an EditMode test), and
    /// binding some other, default rules object would leave the gate shut.
    /// </summary>
    private void AssertComposedWithThisMatchsRules(GameObject participant)
    {
        PlayerHealth health = participant.GetComponentInChildren<PlayerHealth>(true);
        Invoke(health, "Start");
        health.SpendBlock(5f);
        Invoke(health, "Update");
        Assert.IsTrue(StartedRegeneratingBlock(health));
    }

    [Test]
    public void SpawnCoordinator_RegisterHuman_BindsThisMatchsResolvedRulesToPlayerHealth()
    {
        GameObject actorGo = SpawnHumanParticipantWithHealth(pid: 0);

        registerHuman.Invoke(coordinator, new object[] { actorGo, 0, null });

        AssertComposedWithThisMatchsRules(actorGo);
    }

    [Test]
    public void SpawnCoordinator_RegisterCpu_BindsThisMatchsResolvedRulesToPlayerHealth()
    {
        GameObject actorGo = SpawnCpuParticipantWithHealth(pid: 1);

        registerCpu.Invoke(coordinator, new object[] { actorGo, 1 });

        AssertComposedWithThisMatchsRules(actorGo);
    }

    [Test]
    public void SpawnCoordinator_ParticipantWithoutPlayerHealth_IsSkippedSilently()
    {
        // Composition supplies rules to an existing component; it never adds one. Modelled on
        // Lockdown's defender registration route, which uses this same RegisterCpu path.
        GameObject defender = Spawn("cpu-defender");
        defender.AddComponent<CharacterProfile>();
        defender.AddComponent<AutoPlayerDefense>();
        defender.AddComponent<PlayerIdentifier>();

        Assert.DoesNotThrow(() => registerCpu.Invoke(coordinator, new object[] { defender, 1 }));
        Assert.IsNull(defender.GetComponentInChildren<PlayerHealth>(true));
    }
}
