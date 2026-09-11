#if UNITY_INCLUDE_TESTS
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

/// <summary>
/// AUD-112/AUD-103: no test previously loaded a menu scene at all. These smoke-test the screens whose
/// migration to serialized <c>*UiObjects</c>/<c>MenuFooterUiObjects</c> references this issue completed
/// and that have no heavy runtime dependency (loaded player-select data, an active match session, a
/// live database wait) that would make an isolated scene load flaky: the scene loads, its manager stays
/// enabled (proving <c>ValidateMenuUi</c> did not disable it for a missing reference), and the
/// EventSystem has a non-null selection.
///
/// Progression, Start and Pause are not covered here - they pull in LoadedData/DBHelper/
/// GameLevelManager/MatchSession, which are not initialized in an isolated scene load and would make
/// the test flaky or slow rather than a meaningful smoke check.
///
/// AUD-012 Phase 2c Slice 46: OptionsManager/CreditsManager/StatsManager/AccountManager live in
/// Assets/Scripts folders with no asmdef and compile into Assembly-CSharp, which this assembly cannot
/// reference. <see cref="FindManagerInScene"/> resolves each manager by exact runtime type name scoped
/// to the loaded scene instead, so this fixture compiles into <c>Level5.PlayModeTests</c> with no
/// Assembly-CSharp dependency. The compiler-backed proof that each concrete manager type actually
/// implements its UI contract stays in EditMode: <c>Level5SceneContractTests.
/// EveryMenuManagerHasItsRequiredUiObjectReferencesWired</c> drives <c>Level5ProjectValidator.
/// CollectMenuUiObjectContractErrors</c>, which references all four manager types directly and invokes
/// their real <c>ValidateMenuUi</c>.
/// </summary>
public class Level5MenuScreenPlayModeTests
{
    private string loadedSceneName;
    private Scene loadedScene;

    [UnityTearDown]
    public IEnumerator TearDown()
    {
        if (!string.IsNullOrEmpty(loadedSceneName))
        {
            Scene scene = SceneManager.GetSceneByName(loadedSceneName);
            if (scene.IsValid() && scene.isLoaded)
            {
                yield return SceneManager.UnloadSceneAsync(scene);
            }

            loadedSceneName = null;
            loadedScene = default;
        }
    }

    /// <summary>
    /// Loads <paramref name="sceneName"/> additively and resolves the loaded <see cref="Scene"/> once,
    /// into <see cref="loadedScene"/>, so callers don't each repeat their own
    /// <see cref="SceneManager.GetSceneByName"/> lookup.
    /// </summary>
    private IEnumerator LoadMenuScene(string sceneName)
    {
        loadedSceneName = sceneName;
        yield return SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        loadedScene = SceneManager.GetSceneByName(sceneName);
        // let Start()/EnsureSelected run before asserting on their results
        yield return null;
        yield return null;
    }

    /// <summary>
    /// Resolves a manager by exact runtime type name scoped to <paramref name="scene"/>, so the lookup
    /// cannot accidentally match a same-named component left over in another loaded scene. Walking each
    /// root's <see cref="GameObject.GetComponentsInChildren{T}(bool)"/> with <c>includeInactive: false</c>
    /// inspects only GameObjects active in the hierarchy while still returning a disabled MonoBehaviour
    /// on an otherwise active GameObject, so the caller's own <c>manager.enabled</c> assertion can fail
    /// correctly instead of this helper silently reporting it as "not found".
    /// </summary>
    private static MonoBehaviour FindManagerInScene(Scene scene, string typeName)
    {
        Assert.That(scene.IsValid() && scene.isLoaded, Is.True, $"scene must be loaded before resolving '{typeName}'.");

        foreach (GameObject root in scene.GetRootGameObjects())
        {
            foreach (MonoBehaviour behaviour in root.GetComponentsInChildren<MonoBehaviour>(false))
            {
                if (behaviour != null && behaviour.gameObject.scene == scene && behaviour.GetType().Name == typeName)
                {
                    return behaviour;
                }
            }
        }

        return null;
    }

    [UnityTest]
    public IEnumerator OptionsScreenLoadsAndKeepsItsManagerEnabled()
    {
        yield return LoadMenuScene(Constants.SCENE_NAME_level_00_options);

        MonoBehaviour manager = FindManagerInScene(loadedScene, "OptionsManager");
        Assert.That(manager, Is.Not.Null, "OptionsManager was not found in the loaded scene.");
        Assert.That(manager.enabled, Is.True, "OptionsManager disabled itself - a required UI reference is missing.");
        Assert.That(EventSystem.current, Is.Not.Null);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator CreditsScreenLoadsAndKeepsItsManagerEnabled()
    {
        yield return LoadMenuScene(Constants.SCENE_NAME_level_00_credits);

        MonoBehaviour manager = FindManagerInScene(loadedScene, "CreditsManager");
        Assert.That(manager, Is.Not.Null, "CreditsManager was not found in the loaded scene.");
        Assert.That(manager.enabled, Is.True, "CreditsManager disabled itself - a required UI reference is missing.");
        Assert.That(EventSystem.current, Is.Not.Null);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator StatsScreenLoadsAndKeepsItsManagerEnabled()
    {
        yield return LoadMenuScene(Constants.SCENE_NAME_level_00_stats);

        MonoBehaviour manager = FindManagerInScene(loadedScene, "StatsManager");
        Assert.That(manager, Is.Not.Null, "StatsManager was not found in the loaded scene.");
        Assert.That(manager.enabled, Is.True, "StatsManager disabled itself - a required UI reference is missing.");
        Assert.That(EventSystem.current, Is.Not.Null);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.Null);
    }

    [UnityTest]
    public IEnumerator AccountHubScreenLoadsAndKeepsItsManagerEnabled()
    {
        yield return LoadMenuScene(Constants.SCENE_NAME_level_00_account);

        MonoBehaviour manager = FindManagerInScene(loadedScene, "AccountManager");
        Assert.That(manager, Is.Not.Null, "AccountManager was not found in the loaded scene.");
        Assert.That(manager.enabled, Is.True, "AccountManager disabled itself - a required UI reference is missing.");
        Assert.That(EventSystem.current, Is.Not.Null);
        Assert.That(EventSystem.current.currentSelectedGameObject, Is.Not.Null);
    }
}
#endif
