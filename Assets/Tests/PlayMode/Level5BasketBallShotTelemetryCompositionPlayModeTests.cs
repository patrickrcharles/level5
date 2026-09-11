#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-010 Phase 2b0: proves the real <c>SpawnCoordinator.GiveBall</c> -&gt;
/// <c>BasketBall.BindShotTelemetry</c> wiring holds in an actual scene load, the same gap
/// <c>Level5MoneyBallStateCompositionPlayModeTests</c>'s own header comment explains no EditMode test
/// can observe: every EditMode test for this binding (<c>Level5BasketBallShotTelemetryTests</c>)
/// either calls <c>BindShotTelemetry</c> directly or drives <c>SpawnCoordinator.GiveBall</c> against a
/// hand-built <c>PlayerRegistry</c>, never the real start-menu -&gt; gameplay-scene load this file
/// drives instead.
///
/// AUD-012 Phase 2c Slice 42: bootstrap now goes through <see cref="GameplayScenePlayModeHarness"/>
/// instead of this fixture's own <c>StartManager</c>/<c>Pause</c> handling, so it compiles into
/// <c>Level5.PlayModeTests</c> with no <c>Assembly-CSharp</c> reference. <c>AnaylticsManager</c>'s
/// identity is still asserted below, but by runtime delegate metadata (declaring-type/method name)
/// rather than <c>typeof</c>/<c>nameof</c>, since that identity check is test-only inspection, not a
/// production dependency. The compiler-backed exact-type proof stays in EditMode's
/// <c>Level5BasketBallShotTelemetryTests.PrimaryHumanBallReceivesAnaylticsManagerPlayerShootAsItsTelemetryCallback</c>.
/// </summary>
public class Level5BasketBallShotTelemetryCompositionPlayModeTests
{
    [SetUp]
    public void IgnoreSceneLogNoise()
    {
        RealScenePlayModeTestSupport.IgnoreSceneLogNoise();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return RealScenePlayModeTestSupport.UnloadAllLoadedScenes("shot-telemetry-composition-cleanup");
    }

    [UnityTest]
    public IEnumerator EveryRealSpawnedHumanBasketballIsBoundToAnaylticsManagerPlayerShoot()
    {
        yield return GameplayScenePlayModeHarness.EnterPlayableGameplayScene(_ => { });

        // The harness only guarantees a live PlayerController, which proves the player spawned but not
        // that every ball's own composition step (SpawnCoordinator.GiveBall's telemetry bind included)
        // has finished - kept as the fixture's original bounded settle rather than trusting the
        // harness's readiness signal for this.
        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }

        BasketBall[] balls = Object.FindObjectsByType<BasketBall>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        BasketBallAuto[] autoBalls = Object.FindObjectsByType<BasketBallAuto>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log("DIAG human balls (BasketBall): " + balls.Length + ", cpu balls (BasketBallAuto): " + autoBalls.Length);

        Assert.That(balls.Length, Is.GreaterThan(0), "no BasketBall was spawned - nothing for this test to verify");

        foreach (BasketBall ball in balls)
        {
            object bound = RealScenePlayModeTestSupport.GetField(ball, "shotTelemetryCallback");
            Assert.That(bound, Is.Not.Null,
                $"BasketBall '{ball.gameObject.name}' reached play with no bound shot-telemetry callback - SpawnCoordinator.GiveBall's composition step did not reach it");

            System.Delegate callback = (System.Delegate)bound;
            Assert.That(callback.Method.DeclaringType?.Name, Is.EqualTo("AnaylticsManager"),
                $"BasketBall '{ball.gameObject.name}' is bound to a shot-telemetry callback that is not AnaylticsManager.PlayerShoot");
            Assert.That(callback.Method.Name, Is.EqualTo("PlayerShoot"));
        }

        // AUD-010 Phase 2b0: BasketBallAuto declares no BindShotTelemetry/telemetry field at all, so
        // CPU shots gaining no telemetry behavior is a static-type fact, not scene-dependent - proven
        // once by the EditMode test BasketBallAutoDeclaresNoShotTelemetryBindingMethod. This scene
        // load spawns no CPU ball to assert against either way (see the diagnostic log above), so a
        // reflection check here would only restate that same static fact without exercising it live.
    }
}
#endif
