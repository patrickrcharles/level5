#if UNITY_INCLUDE_TESTS
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// Shared scaffolding for PlayMode fixtures that drive real scenes end to end: silencing scene log
/// noise unrelated to what is under test, unloading every scene after a test, and reflecting into
/// private production fields. Extracted because <c>BasketballVisibilityTests</c> and
/// <c>PlayerMovementPhysicsTests</c> each hand-rolled an identical copy of all three.
/// </summary>
internal static class RealScenePlayModeTestSupport
{
    internal const BindingFlags PrivateInstanceFlags =
        BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

    /// <summary>
    /// These fixtures drive real scenes end to end, and those scenes log errors of their own that
    /// have nothing to do with what is under test. Without this the runner turns any stray
    /// Debug.LogError into a failure for whichever test happened to be running.
    /// </summary>
    internal static void IgnoreSceneLogNoise()
    {
        LogAssert.ignoreFailingMessages = true;
    }

    /// <summary>
    /// Restores <see cref="Time.timeScale"/> and unloads every scene the test loaded, so the next
    /// test starts clean regardless of which scene(s) this one left active. Creates and activates a
    /// blank scene first since Unity does not allow unloading the last loaded scene.
    /// </summary>
    internal static IEnumerator UnloadAllLoadedScenes(string blankSceneName)
    {
        Time.timeScale = 1f;
        Scene blank = SceneManager.CreateScene(blankSceneName);
        SceneManager.SetActiveScene(blank);
        for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene != blank && scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }
        }

        yield return null;
    }

    /// <summary>Looks up a private/internal instance or static field by name via reflection.</summary>
    internal static FieldInfo GetFieldInfo(object target, string name)
    {
        return target?.GetType().GetField(name, PrivateInstanceFlags);
    }

    /// <summary>Reads a private/internal instance or static field by name via reflection.</summary>
    internal static object GetField(object target, string name)
    {
        return GetFieldInfo(target, name)?.GetValue(target);
    }

    /// <summary>Reads a private/internal instance or static field by name via reflection, typed.</summary>
    internal static T GetField<T>(object target, string name) where T : class
    {
        return GetField(target, name) as T;
    }
}
#endif
