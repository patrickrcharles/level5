#if UNITY_INCLUDE_TESTS
using System.Collections;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// AUD-010 Phase 1c: closes a coverage gap a code review flagged on the money-ball-state migration.
/// Every EditMode test for <c>GameRules.BindMoneyBallStateToBasketballs</c>
/// (<c>Level5BasketballMoneyBallStateTests</c>) invokes it directly via reflection against a
/// hand-built <c>PlayerRegistry</c> - proving the binding logic is correct in isolation, but
/// never proving the real <c>GameRules.Awake()</c> -&gt; <c>GameLevelManager.instance.Registry</c> chain
/// actually wires a live scene's real spawned balls. That chain depends on Unity's script execution
/// order (<c>GameLevelManager</c> at -8000, <c>GameRules</c> at default 0) actually holding in a real
/// scene load, which no EditMode test can observe - EditMode tests build components directly and
/// never let Unity itself decide Awake() order across objects.
///
/// This drives the real production flow - the same technique
/// <c>Level5ShotMarkerSessionCompositionPlayModeTests</c> already uses - through the start menu into a
/// real gameplay scene, so the balls under test are spawned by the actual <c>SpawnCoordinator</c> and
/// bound by the actual <c>GameRules.Awake()</c>, not a test double standing in for either.
///
/// AUD-012 Phase 2c Slice 45: bootstrap now goes through <see cref="GameplayScenePlayModeHarness"/>
/// instead of this fixture's own <c>StartManager</c>/<c>Pause</c> handling, so it compiles into
/// <c>Level5.PlayModeTests</c> with no <c>Assembly-CSharp</c> reference. The canonical runtime
/// <c>GameRules</c> instance is resolved through the same shared
/// <see cref="RealScenePlayModeTestSupport.ResolveRuntimeGameRulesInstance"/> operation
/// <c>Level5ShotMarkerSessionCompositionPlayModeTests</c> uses, independently of any ball's own
/// binding, so the provider-identity assertions below stay non-circular.
///
/// This fixture no longer asserts <c>GameLevelManager.instance != null</c> directly: production's own
/// ordering (<c>GameRules.Awake()</c> reads <c>GameLevelManager.instance != null
/// ? GameLevelManager.instance.Registry : null</c> and <c>BindMoneyBallStateToBasketballs</c> returns
/// without binding when given a null registry) means a missing/unpopulated
/// <c>GameLevelManager</c> would already surface as a null <c>moneyBallState</c> below - the per-ball
/// provider assertions already prove the relevant composition chain held. The compiler-backed
/// exact-type <c>GameRules</c> composition proof, including human/CPU/secondary-human composition and
/// participant-with-no-ball behavior, stays in EditMode's <c>Level5BasketballMoneyBallStateTests</c>.
/// </summary>
public class Level5MoneyBallStateCompositionPlayModeTests
{
    [SetUp]
    public void IgnoreSceneLogNoise()
    {
        RealScenePlayModeTestSupport.IgnoreSceneLogNoise();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return RealScenePlayModeTestSupport.UnloadAllLoadedScenes("moneyball-composition-cleanup");
    }

    [UnityTest]
    public IEnumerator EveryRealSpawnedBasketballIsBoundToTheRealGameRulesInstance()
    {
        yield return GameplayScenePlayModeHarness.EnterPlayableGameplayScene(_ => { });

        // The harness only guarantees a live PlayerController, which proves the player spawned but not
        // that GameRules.Awake()'s own money-ball-state composition step has finished - kept as the
        // fixture's original bounded settle rather than trusting the harness's readiness signal for
        // this.
        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }

        object gameRulesInstance = RealScenePlayModeTestSupport.ResolveRuntimeGameRulesInstance();

        BasketBall[] balls = Object.FindObjectsByType<BasketBall>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        BasketBallAuto[] autoBalls = Object.FindObjectsByType<BasketBallAuto>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        Debug.Log("DIAG scene: " + SceneManager.GetActiveScene().name
            + ", human balls (BasketBall): " + balls.Length
            + ", cpu balls (BasketBallAuto): " + autoBalls.Length);

        Assert.That(balls.Length, Is.GreaterThan(0), "no BasketBall was spawned - nothing for this test to verify");

        foreach (BasketBall ball in balls)
        {
            IMoneyBallState provider = RealScenePlayModeTestSupport.GetField<IMoneyBallState>(ball, "moneyBallState");
            Assert.That(provider, Is.Not.Null, $"BasketBall '{ball.gameObject.name}' reached play with no bound IMoneyBallState - GameRules.Awake()'s composition step did not reach it");
            Assert.That(
                ReferenceEquals(provider, gameRulesInstance),
                Is.True,
                $"BasketBall '{ball.gameObject.name}' is bound to a money-ball provider that is not the real live GameRules instance");

            // AUD-010 Phase 2b0: proves the real SpawnCoordinator.GiveBall -> BasketBall.BindMatchRules
            // wiring holds in an actual scene load - the same gap this file's own header comment
            // already explains no EditMode test can observe. A missing bind here would have
            // deactivated the whole GameObject in Start() (see BasketBall.Start()'s rules guard).
            Assert.That(
                ball.gameObject.activeSelf,
                Is.True,
                $"BasketBall '{ball.gameObject.name}' deactivated itself - it reached Start() with no bound match rules");
            Assert.That(
                RealScenePlayModeTestSupport.GetField<ResolvedMatchRules>(ball, "matchRules"),
                Is.Not.Null,
                $"BasketBall '{ball.gameObject.name}' reached play with no bound ResolvedMatchRules - SpawnCoordinator.GiveBall's composition step did not reach it");
        }

        foreach (BasketBallAuto ball in autoBalls)
        {
            IMoneyBallState provider = RealScenePlayModeTestSupport.GetField<IMoneyBallState>(ball, "moneyBallState");
            Assert.That(provider, Is.Not.Null, $"BasketBallAuto '{ball.gameObject.name}' reached play with no bound IMoneyBallState - GameRules.Awake()'s composition step did not reach it");
            Assert.That(
                ReferenceEquals(provider, gameRulesInstance),
                Is.True,
                $"BasketBallAuto '{ball.gameObject.name}' is bound to a money-ball provider that is not the real live GameRules instance");

            // AUD-010 Phase 2b0 code review: proves the real SpawnCoordinator.GiveBall -> BasketBallAuto.BindMatchRules
            // wiring holds in an actual scene load, the same gap this file's own header comment
            // already explains no EditMode test can observe. A missing bind here would have
            // deactivated the whole GameObject in Start() (see BasketBallAuto.Start()'s rules guard).
            Assert.That(
                ball.gameObject.activeSelf,
                Is.True,
                $"BasketBallAuto '{ball.gameObject.name}' deactivated itself - it reached Start() with no bound match rules");
            Assert.That(
                RealScenePlayModeTestSupport.GetField<ResolvedMatchRules>(ball, "matchRules"),
                Is.Not.Null,
                $"BasketBallAuto '{ball.gameObject.name}' reached play with no bound ResolvedMatchRules - SpawnCoordinator.GiveBall's composition step did not reach it");
        }
    }
}
#endif
