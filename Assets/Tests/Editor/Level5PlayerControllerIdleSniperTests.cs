using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 33: <see cref="PlayerController.checkIdleTimeForSniper"/> now resolves the
/// idle-sniper runtime through a bound <c>Func&lt;IPlayerIdleSniperRuntime&gt;</c>
/// (<see cref="PlayerController.BindIdleSniperRuntimeReader"/>) instead of reading
/// <c>SniperManager.instance</c> directly, but keeps its own ownership of sniper-enabled policy, idle
/// timing, the 150-second threshold, random-delay generation, the lock transition and coroutine
/// execution unchanged. These tests protect the distinctions most likely to regress across that
/// swap - missing vs. locked vs. eligible-unlocked runtime - against a fake
/// <see cref="IPlayerIdleSniperRuntime"/>, without depending on 150 seconds of real elapsed test time
/// (<see cref="SetIdleEligible"/> moves <c>idleStartTime</c> into the past instead) and without
/// exercising the real projectile flow.
/// </summary>
public class Level5PlayerControllerIdleSniperTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();

    [SetUp]
    public void SetUp()
    {
        ActiveMatch.Clear();
    }

    [TearDown]
    public void TearDown()
    {
        ActiveMatch.Clear();

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

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"{target.GetType().Name} must declare a field named '{fieldName}'");
        field.SetValue(target, value);
    }

    private static object GetPrivateField(object target, string fieldName)
    {
        FieldInfo field = target.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"{target.GetType().Name} must declare a field named '{fieldName}'");
        return field.GetValue(target);
    }

    private static void InvokeCheckIdleTimeForSniper(PlayerController controller)
    {
        MethodInfo method = typeof(PlayerController).GetMethod("checkIdleTimeForSniper", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(method, "PlayerController must declare checkIdleTimeForSniper()");
        method.Invoke(controller, null);
    }

    /// <summary>
    /// Puts the controller on the idle-accumulating branch (no movement input, grounded) and moves
    /// <c>idleStartTime</c> far enough into the past that the very next
    /// <c>checkIdleTimeForSniper()</c> call computes <c>idleTime &gt; 150</c> from
    /// <c>Time.time - idleStartTime</c> - the same expression the production code uses - rather than
    /// this test waiting on real time or poking <c>idleTime</c> directly (which that expression would
    /// overwrite on the very next call anyway).
    /// </summary>
    private static void SetIdleEligible(PlayerController controller)
    {
        SetPrivateField(controller, "movementHorizontal", 0f);
        SetPrivateField(controller, "movementVertical", 0f);
        controller.Grounded = true;
        SetPrivateField(controller, "idleStartTime", Time.time - 200f);
    }

    private static void EnableSniperViaActiveMatch()
    {
        GameModeDefinition mode = TestDefinitions.Mode(GameModeId.TotalPoints);
        LevelDefinition level = TestDefinitions.Level(1);
        PlayerRoster roster = TestDefinitions.SoloRoster();

        MatchConfiguration configuration = new MatchConfiguration(
            mode,
            level,
            roster,
            MatchModifiers.Default,
            new ResolvedMatchRules(sniper: SniperMode.Bullet),
            CheerleaderSelection.None,
            "idle sniper test");

        ActiveMatch.Begin(configuration);
    }

    // ==================== missing runtime ====================

    [Test]
    public void SniperEnabled_UnboundReader_ResetsIdleTiming()
    {
        EnableSniperViaActiveMatch();
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        SetIdleEligible(controller);

        InvokeCheckIdleTimeForSniper(controller);

        Assert.That((float)GetPrivateField(controller, "idleTime"), Is.EqualTo(0f));
        Assert.That((float)GetPrivateField(controller, "idleStartTime"), Is.EqualTo(Time.time).Within(0.0001f));
    }

    [Test]
    public void SniperEnabled_ReaderReturningNull_ResetsIdleTiming()
    {
        EnableSniperViaActiveMatch();
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        controller.BindIdleSniperRuntimeReader(() => null);
        SetIdleEligible(controller);

        InvokeCheckIdleTimeForSniper(controller);

        Assert.That((float)GetPrivateField(controller, "idleTime"), Is.EqualTo(0f));
        Assert.That((float)GetPrivateField(controller, "idleStartTime"), Is.EqualTo(Time.time).Within(0.0001f));
    }

    [Test]
    public void SniperDisabled_BoundRuntime_StillResetsIdleTimingAndRequestsNoRoutine()
    {
        // ActiveMatch.Clear() in SetUp means MatchRuntime.Rules.SniperEnabled is false here.
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeIdleSniperRuntime runtime = new FakeIdleSniperRuntime();
        controller.BindIdleSniperRuntimeReader(() => runtime);
        SetIdleEligible(controller);

        InvokeCheckIdleTimeForSniper(controller);

        Assert.That((float)GetPrivateField(controller, "idleTime"), Is.EqualTo(0f));
        Assert.That(runtime.RequestedRoutineCount, Is.EqualTo(0));
    }

    // ==================== locked runtime ====================

    [Test]
    public void LockedRuntime_DoesNotTriggerInstantKill()
    {
        EnableSniperViaActiveMatch();
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeIdleSniperRuntime runtime = new FakeIdleSniperRuntime { Locked = true };
        controller.BindIdleSniperRuntimeReader(() => runtime);
        SetIdleEligible(controller);

        InvokeCheckIdleTimeForSniper(controller);

        Assert.That(runtime.RequestedRoutineCount, Is.EqualTo(0));
        Assert.That(runtime.Locked, Is.True);
    }

    [Test]
    public void LockedRuntime_DoesNotFalselyResetAccumulatedIdleTime()
    {
        EnableSniperViaActiveMatch();
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeIdleSniperRuntime runtime = new FakeIdleSniperRuntime { Locked = true };
        controller.BindIdleSniperRuntimeReader(() => runtime);
        SetIdleEligible(controller);

        InvokeCheckIdleTimeForSniper(controller);

        Assert.That(
            (float)GetPrivateField(controller, "idleTime"),
            Is.GreaterThan(150f),
            "a locked runtime must not reset idle timing merely because it is locked");
    }

    // ==================== eligible unlocked runtime ====================

    [Test]
    public void EligibleUnlockedRuntime_LocksTheRuntimeBeforeRequestingTheRoutine()
    {
        EnableSniperViaActiveMatch();
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeIdleSniperRuntime runtime = new FakeIdleSniperRuntime();
        runtime.OnRoutineRequested = () => Assert.IsTrue(
            runtime.Locked,
            "the runtime must already be locked at the moment the instant-kill routine is requested");
        controller.BindIdleSniperRuntimeReader(() => runtime);
        SetIdleEligible(controller);

        Assert.DoesNotThrow(() => InvokeCheckIdleTimeForSniper(controller));

        Assert.That(runtime.Locked, Is.True);
        Assert.That(runtime.RequestedRoutineCount, Is.EqualTo(1));
    }

    [Test]
    public void EligibleUnlockedRuntime_ResetsIdleTimingAndRequestsExactlyOneRoutine()
    {
        EnableSniperViaActiveMatch();
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        FakeIdleSniperRuntime runtime = new FakeIdleSniperRuntime();
        controller.BindIdleSniperRuntimeReader(() => runtime);
        SetIdleEligible(controller);

        InvokeCheckIdleTimeForSniper(controller);

        Assert.That(runtime.RequestedRoutineCount, Is.EqualTo(1));
        Assert.That((float)GetPrivateField(controller, "idleTime"), Is.EqualTo(0f));
        Assert.That((float)GetPrivateField(controller, "idleStartTime"), Is.EqualTo(Time.time).Within(0.0001f));
    }

    // ==================== test double ====================

    private sealed class FakeIdleSniperRuntime : IPlayerIdleSniperRuntime
    {
        public bool Locked { get; set; }
        public int RequestedRoutineCount { get; private set; }
        public Action OnRoutineRequested;

        // A plain (non-iterator) method so the request is counted the instant PlayerController calls
        // it - before StartCoroutine ever runs the returned enumerator - matching what "exactly one
        // routine is requested" means here without executing the real projectile flow.
        public IEnumerator GetInstantKillRoutine(float shootDelay)
        {
            RequestedRoutineCount++;
            OnRoutineRequested?.Invoke();
            return EmptyRoutine();
        }

        private static IEnumerator EmptyRoutine()
        {
            yield break;
        }
    }
}
