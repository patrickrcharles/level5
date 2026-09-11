#if UNITY_INCLUDE_TESTS
using System.Collections;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-010 Phase 1c: closes the same coverage gap <c>Level5MoneyBallStateCompositionPlayModeTests</c>
/// closes for the money-ball-state migration, for the shot-marker-session migration. Every EditMode
/// test for <c>GameRules.BindShotMarkerSessionToMarkers</c> (<c>Level5BasketballShotMarkerSessionTests</c>)
/// invokes it directly via reflection against hand-built markers - proving the binding logic is
/// correct in isolation, but never proving the real <c>GameRules.Awake()</c> -&gt; scene-authored
/// <c>"shot_marker"</c> tag scan actually wires a live scene's real markers before their own
/// <c>Start()</c> runs. That ordering depends on Unity's guarantee that every object's Awake() runs
/// before any object's Start() - Unity itself deciding Awake()/Start() order across objects is exactly
/// what an EditMode test cannot observe.
///
/// This drives the real production flow - the same technique
/// <c>Level5MoneyBallStateCompositionPlayModeTests</c> already uses - through the start menu into a
/// real gameplay scene, so the markers under test are the actual scene-authored objects and bound by
/// the actual <c>GameRules.Awake()</c>, not a test double standing in for either.
///
/// Markers a game mode does not need are deactivated later by <c>GameRules.Start()</c>'s
/// <c>SetPositionMarkers()</c> - after this binding pass already ran in <c>Awake()</c> - so inactive
/// markers are included in the scan rather than excluded: deactivation afterward does not unbind them.
///
/// AUD-012 Phase 2c Slice 43: bootstrap now goes through <see cref="GameplayScenePlayModeHarness"/>
/// instead of this fixture's own <c>StartManager</c>/<c>Pause</c> handling, so it compiles into
/// <c>Level5.PlayModeTests</c> with no <c>Assembly-CSharp</c> reference. The canonical runtime
/// <c>GameRules</c> instance is resolved independently of any marker's own binding (see
/// <c>RealScenePlayModeTestSupport.ResolveRuntimeGameRulesInstance</c>), so the session-identity
/// assertion below stays non-circular; the compiler-backed exact-type <c>GameRules</c> composition
/// proof stays in EditMode's <c>Level5BasketballShotMarkerSessionTests</c>.
///
/// AUD-012 Phase 2c Slice 45: the resolver itself moved into
/// <see cref="RealScenePlayModeTestSupport"/> once <c>Level5MoneyBallStateCompositionPlayModeTests</c>
/// became a second real consumer of the identical operation.
/// </summary>
public class Level5ShotMarkerSessionCompositionPlayModeTests
{
    [SetUp]
    public void IgnoreSceneLogNoise()
    {
        RealScenePlayModeTestSupport.IgnoreSceneLogNoise();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return RealScenePlayModeTestSupport.UnloadAllLoadedScenes("shot-marker-composition-cleanup");
    }

    [UnityTest]
    public IEnumerator EveryRealSceneMarkerIsBoundToTheRealGameRulesInstanceBeforeGameplay()
    {
        yield return GameplayScenePlayModeHarness.EnterPlayableGameplayScene(_ => { });

        // The harness only guarantees a live PlayerController, which proves the player spawned but not
        // that GameRules.Awake()'s own marker-binding composition step has finished - kept as the
        // fixture's original bounded settle rather than trusting the harness's readiness signal for
        // this.
        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }

        object gameRulesInstance = RealScenePlayModeTestSupport.ResolveRuntimeGameRulesInstance();

        BasketBallShotMarker[] markers = Object.FindObjectsByType<BasketBallShotMarker>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);

        Assert.That(markers.Length, Is.GreaterThan(0), "no BasketBallShotMarker was found - nothing for this test to verify");

        foreach (BasketBallShotMarker marker in markers)
        {
            IShotMarkerSession session = RealScenePlayModeTestSupport.GetField<IShotMarkerSession>(marker, "markerSession");
            Assert.That(session, Is.Not.Null, $"BasketBallShotMarker '{marker.gameObject.name}' reached play with no bound IShotMarkerSession - GameRules.Awake()'s composition step did not reach it");
            Assert.That(
                ReferenceEquals(session, gameRulesInstance),
                Is.True,
                $"BasketBallShotMarker '{marker.gameObject.name}' is bound to a shot-marker session that is not the real live GameRules instance");

            // AUD-010 Phase 2b0: the same composition step also binds this match's resolved
            // ResolvedMatchRules - proves the real GameRules.Awake() -> BindShotMarkerSessionToMarkers()
            // pass reaches this dependency too, not just the pre-existing session. Checked against
            // GameRules' own resolved rules field, the same reference-identity rigor as the session
            // assertion above, so a future change that gave each marker its own separately-resolved
            // rules instance (instead of sharing GameRules' one) would fail this too.
            ResolvedMatchRules boundRules = RealScenePlayModeTestSupport.GetField<ResolvedMatchRules>(marker, "matchRules");
            Assert.That(boundRules, Is.Not.Null, $"BasketBallShotMarker '{marker.gameObject.name}' reached play with no bound match rules - GameRules.Awake()'s composition step did not reach it");
            ResolvedMatchRules gameRulesOwnRules = RealScenePlayModeTestSupport.GetField<ResolvedMatchRules>(gameRulesInstance, "resolvedRules");
            Assert.That(
                ReferenceEquals(boundRules, gameRulesOwnRules),
                Is.True,
                $"BasketBallShotMarker '{marker.gameObject.name}' is bound to match rules that are not GameRules' own resolved rules reference");
        }
    }
}
#endif
