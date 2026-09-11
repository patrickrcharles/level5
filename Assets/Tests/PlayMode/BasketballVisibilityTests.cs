#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// "When the player has the ball, the one above his head needs to be hidden."
///
/// BasketBall.Update moves the ball to the owner's basketBall_position (local y 1, above the head)
/// and hides it with spriteRenderer.color alpha 0 when hasBasketball is true, so the position is by
/// design and the hide is what fails. Static reading could not tell which ball is actually visible:
/// SpawnCoordinator gives every participant a ball and spawns them all at the same
/// ball_spawn_location, so a second player's ball is also a candidate.
///
/// This reports, per ball in the scene: its owner, whether that owner has it, where it is relative
/// to the owner's hold point, and whether it is actually being drawn.
///
/// Kept as regression coverage: alpha alone silently stopped hiding it once the sprite moved to
/// a particle shader, and nothing would have caught that.
/// </summary>
public class BasketballVisibilityTests
{
    [SetUp]
    public void IgnoreSceneLogNoise()
    {
        RealScenePlayModeTestSupport.IgnoreSceneLogNoise();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return RealScenePlayModeTestSupport.UnloadAllLoadedScenes("basketball-diag-cleanup");
    }

    [UnityTest]
    public IEnumerator TheHeldBallIsNotDrawnAbovethePlayersHead()
    {
        // The harness only needs a PlayerController as its own readiness signal; this re-resolves a
        // fresh one below after settling, rather than reusing that reference, in case the gameplay
        // scene still swaps the player object out during the settle wait.
        yield return GameplayScenePlayModeHarness.EnterPlayableGameplayScene(_ => { });

        for (int i = 0; i < 10; i++)
        {
            yield return null;
        }

        Debug.Log("DIAG scene: " + SceneManager.GetActiveScene().name);

        // The player never picks the ball up on his own here, so force the held state - that is the
        // state the bug is reported in ("when player has the ball, the one above his head needs to
        // be hidden").
        PlayerController human = Object.FindAnyObjectByType<PlayerController>();
        if (human != null)
        {
            FieldInfo hasBall = RealScenePlayModeTestSupport.GetFieldInfo(human, "hasBasketball");
            if (hasBall != null)
            {
                hasBall.SetValue(human, true);
                Debug.Log("DIAG forced hasBasketball=true on " + human.gameObject.name);
            }
            else
            {
                Debug.Log("DIAG could not find hasBasketball field");
            }
        }

        // let BasketBall.Update run against the held state
        for (int i = 0; i < 5; i++)
        {
            yield return null;
        }


        BasketBall[] balls = Object.FindObjectsByType<BasketBall>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log("DIAG human balls (BasketBall): " + balls.Length);

        for (int i = 0; i < balls.Length; i++)
        {
            Report("BasketBall[" + i + "]", balls[i], balls[i].gameObject);
        }

        BasketBallAuto[] autoBalls = Object.FindObjectsByType<BasketBallAuto>(
            FindObjectsInactive.Include, FindObjectsSortMode.None);
        Debug.Log("DIAG cpu balls (BasketBallAuto): " + autoBalls.Length);
        for (int i = 0; i < autoBalls.Length; i++)
        {
            Report("BasketBallAuto[" + i + "]", autoBalls[i], autoBalls[i].gameObject);
        }

        // the reported bug: the ball sits on the hold point above the head and is still drawn
        BasketBall held = null;
        for (int i = 0; i < balls.Length; i++)
        {
            object controller = RealScenePlayModeTestSupport.GetField(balls[i], "actor");
            object hasBallValue = RealScenePlayModeTestSupport.GetField(controller, "hasBasketball");
            if (hasBallValue is bool b && b)
            {
                held = balls[i];
                break;
            }
        }

        Assert.That(held, Is.Not.Null, "no ball reported its owner as holding it");

        SpriteRenderer renderer = RealScenePlayModeTestSupport.GetField<SpriteRenderer>(held, "spriteRenderer");
        Assert.That(renderer, Is.Not.Null, "the held ball has no sprite renderer");
        Assert.That(
            renderer.enabled,
            Is.False,
            "the ball is still being drawn while the player holds it - tinting alpha is not enough, "
                + "the sprite uses a particle shader that ignores the renderer colour");
    }

    private static void Report(string label, object ballComponent, GameObject ballObject)
    {
        object controller = RealScenePlayModeTestSupport.GetField(ballComponent, "actor");
        object owner = RealScenePlayModeTestSupport.GetField(ballComponent, "player");
        GameObject ownerObject = owner as GameObject;
        object hold = RealScenePlayModeTestSupport.GetField(ballComponent, "basketBallPosition");
        GameObject holdObject = hold as GameObject;

        object hasBallValue = RealScenePlayModeTestSupport.GetField(controller, "hasBasketball");
        bool hasBall = hasBallValue is bool b && b;

        SpriteRenderer sr = RealScenePlayModeTestSupport.GetField<SpriteRenderer>(ballComponent, "spriteRenderer");

        Debug.Log(
            "DIAG " + label
            + " owner=" + (ownerObject == null ? "NULL" : ownerObject.name)
            + " hasBasketball=" + hasBall
            + " ballPos=" + ballObject.transform.position
            + " holdPos=" + (holdObject == null ? "NULL" : holdObject.transform.position.ToString())
            + " atHoldPoint=" + (holdObject != null
                && Vector3.Distance(ballObject.transform.position, holdObject.transform.position) < 0.2f)
            + " | renderer=" + (sr == null ? "NULL" : sr.gameObject.name)
            + " enabled=" + (sr == null ? "-" : sr.enabled.ToString())
            + " alpha=" + (sr == null ? "-" : sr.color.a.ToString("F2"))
            + " isVisible=" + (sr == null ? "-" : sr.isVisible.ToString()));
    }
}
#endif
