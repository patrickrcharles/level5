#if UNITY_INCLUDE_TESTS
using System;
using System.Collections;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// Start menu -> gameplay level -> unpause, end to end.
///
/// The bug this covers: every gameplay level opens on the start-on-pause screen, and the real
/// runtime <c>Pause</c> dismisses it with <c>Controls.Player.submit</c> read off the shared
/// PlayerControls instance - whose <c>Player</c> map nothing enabled. GameLevelManager enables only
/// <c>Other</c>, and real player input runs on the separate per-player instances from
/// <c>AcquireGameplayControls</c>. Outside sniper levels (the one <c>EnableGameplayMaps</c> caller)
/// both <c>Player.submit</c> and <c>Player.cancel</c> were permanently dead, so the level could
/// never be started and Escape could never toggle pause.
///
/// AUD-012 Phase 2c Slice 47: <c>Pause</c> lives in <c>Assembly-CSharp</c>, which this assembly
/// cannot reference at compile time, so it is resolved by exact runtime type name through
/// <see cref="RealScenePlayModeTestSupport.FindActiveBehaviourInScene"/> instead - the same pattern
/// Slice 46 used for the menu managers. <c>StartManager</c> is not referenced at all: reaching
/// gameplay through the real start menu is <see cref="GameplayScenePlayModeHarness"/>'s job. Unlike
/// every other fixture that uses the harness, this one needs production's actual initial pause state
/// to reach the test untouched, so it uses <c>EnterGameplayScenePreservingInitialPause</c> rather than
/// <c>EnterPlayableGameplayScene</c>.
/// </summary>
public class GameplayLevelUnpauseTests
{
    [SetUp]
    public void IgnoreSceneLogNoise()
    {
        RealScenePlayModeTestSupport.IgnoreSceneLogNoise();
    }

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        yield return RealScenePlayModeTestSupport.UnloadAllLoadedScenes("gameplay-unpause-test-cleanup");
    }

    [UnityTest]
    public IEnumerator GameplayLevelCanBeUnpaused()
    {
        Scene gameplayScene = default;
        yield return GameplayScenePlayModeHarness.EnterGameplayScenePreservingInitialPause(
            _ => gameplayScene = SceneManager.GetActiveScene());

        MonoBehaviour pause = RealScenePlayModeTestSupport.FindActiveBehaviourInScene(gameplayScene, "Pause");
        Assert.That(pause, Is.Not.Null, "No Pause in the gameplay level.");

        FieldInfo startOnPauseField = pause.GetType().GetField(
            "startOnPause", BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
        Assert.That(startOnPauseField, Is.Not.Null, "Pause no longer declares a 'startOnPause' field.");
        object startOnPauseValue = startOnPauseField.GetValue(pause);
        Assert.That(startOnPauseValue, Is.TypeOf<bool>(), "Pause.startOnPause is no longer a bool.");
        bool startOnPause = (bool)startOnPauseValue;

        bool playerMapEnabled = PlayerControlsProvider.Controls.Player.enabled;

        Debug.Log("DIAG gameplay scene     : " + gameplayScene.name);
        Debug.Log("DIAG startOnPause       : " + startOnPause);
        Debug.Log("DIAG Time.timeScale     : " + Time.timeScale);
        Debug.Log("DIAG Player map enabled : " + playerMapEnabled);

        Assert.That(
            playerMapEnabled,
            Is.True,
            "Controls.Player is disabled, so Pause can never see submit/cancel and the level "
                + "can never be unpaused.");

        // with the map live, the dismiss path is reachable; drive it directly to prove the rest
        if (startOnPause)
        {
            MethodInfo startGame = pause.GetType().GetMethod(
                "StartGame", BindingFlags.Instance | BindingFlags.Public, null, Type.EmptyTypes, null);
            Assert.That(startGame, Is.Not.Null, "Pause no longer declares a parameterless public StartGame().");

            startGame.Invoke(pause, null);
            yield return null;

            Debug.Log("DIAG after StartGame()  : timeScale=" + Time.timeScale);
            Assert.That(Time.timeScale, Is.EqualTo(1f), "StartGame did not resume the game.");
        }
    }
}
#endif
