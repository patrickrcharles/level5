
using Assets.Scripts.database;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    // main flag
    [SerializeField]
    private bool paused;

    //fade texture to obscure background
    [SerializeField]
    private Image fadeTexture;

    // ui text
    [SerializeField]
    private Text loadSceneText;
    private Text loadStartScreenText;
    private Text cancelMenuText;
    private Text quitGameText;

    // pause options
    private Text toggleCameraText;
    private Text toggleUiStatsText;
    private Text toggleMaxStatsText;
    private Text toggleFpsText;

    const string toggleCameraName = "toggle_camera";
    const string toggleUiStatsName = "toggle_stats";
    const string toggleMaxStatsName = "toggle_max_stats";
    const string toggleFpsName = "toggle_fps";

    //ui buttons
    private Button loadSceneButton;
    private Button loadStartScreenButton;
    private Button cancelMenuButton;
    private Button quitGameButton;

    private AudioSource[] allAudioSources;
    private GameObject currentHighlightedButton;

    GameObject controlsObject;
    GameObject controlsMobileObject;
    GameObject controlsDesktopObject;

    private GameObject maxStatsObject;
    private GameObject toggleFpsObject;
    private GameObject toggleUiStatsObject;

    public static Pause instance;

    void Awake()
    {
        instance = this;

        fadeTexture = GameObject.Find("fade_texture").GetComponent<Image>();
        //text
        loadSceneText = GameObject.Find("load_scene").GetComponent<Text>();
        cancelMenuText = GameObject.Find("cancel_menu").GetComponent<Text>();
        loadStartScreenText = GameObject.Find("load_start").GetComponent<Text>();
        quitGameText = GameObject.Find("quit_game").GetComponent<Text>();
        //buttons
        loadSceneButton = GameObject.Find("load_scene").GetComponent<Button>();
        loadStartScreenButton = GameObject.Find("load_start").GetComponent<Button>();
        cancelMenuButton = GameObject.Find("cancel_menu").GetComponent<Button>();
        quitGameButton = GameObject.Find("quit_game").GetComponent<Button>();
        //controls
        controlsObject = GameObject.Find("controls").gameObject;
        controlsMobileObject = GameObject.Find("control_key_mobile").gameObject;
        controlsDesktopObject = GameObject.Find("control_key_desktop").gameObject;

        //toggleCameraText = GameObject.Find(toggleCameraName).GetComponent<Text>();
        toggleUiStatsText = GameObject.Find(toggleUiStatsName).GetComponent<Text>();
        toggleMaxStatsText = GameObject.Find(toggleMaxStatsName).GetComponent<Text>();

        toggleFpsText = GameObject.Find(toggleFpsName).GetComponent<Text>();

        if (controlsObject != null)
        {
            controlsObject.SetActive(false);

#if UNITY_ANDROID && !UNITY_EDITOR
            controlsDesktopObject.SetActive(false);
            controlsMobileObject.SetActive(true);
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
            controlsDesktopObject.SetActive(true);
            controlsMobileObject.SetActive(false);
            disableMobileOnlyPauseOptions();
#endif

        }

        EventSystem.current.firstSelectedGameObject = loadSceneButton.gameObject;
        // init current button
        currentHighlightedButton = EventSystem.current.firstSelectedGameObject.gameObject;
        //disable joystick if active
    }

    private void Start()
    {
        /*pause game
         *disable fade + footer
         *display press start message
         *wait for input 
         *(on imput) disable message + set time scale = 1
         */

        // if game active, disable pause
        if (Time.timeScale == 1f)
        {
            setBackgroundFade(false);
            setPauseScreen(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Update()" );
        //pause ESC, submit, cancel
        if (GameLevelManager.instance.Controls.Player.submit.triggered
            || GameLevelManager.instance.Controls.Player.cancel.triggered
            //|| GameLevelManager.Instance.Controls.Player.esc.triggered
            && !GameLevelManager.instance.GameOver)
        {
            paused = TogglePause();
        }

        // ===================== pause checks =======================
        // truth table, should be paused but isn't
        /* 0 0  time 0, !pause
         * 0 1  time 0, pause  - correct
         * 1 0  time 1, !pause - correct
         * 1 1  time 1, pause
         *
         * check for  0 0, 1 1
         */
        if ((Time.timeScale == 0 && !paused) || (Time.timeScale == 1 && paused))
        {
            //Debug.Log("TogglePause");
            TogglePause();
        }
        //==========================================================

        //disable cotnrols display if game over
        if (GameRules.instance.GameOver || GameLevelManager.instance.PlayerHealth.IsDead)
        {
            controlsObject.SetActive(false);
        }

        // if paused, show pause menu
        if (paused)
        {
            //EventSystem.current.firstSelectedGameObject = loadSceneButton.gameObject;

            // check for some button not selected
            //*this is a hack but it works patch for v3.0.1 : clicking mouse causing game to crash
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
            }
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject; // + "_description";
            currentHighlightedButton.GetComponent<Button>().OnSelect(null);
            currentHighlightedButton.GetComponent<Button>().Select();
            //Debug.Log("currentHighlightedButton : " + currentHighlightedButton);

            // ================== pause menu options ==============================================================

            // reload scene
            if (currentHighlightedButton.name.Equals(loadSceneButton.name)
                && GameLevelManager.instance.Controls.UINavigation.Submit.triggered)
            {
                reloadScene();
            }
            //load start screen
            if (currentHighlightedButton.name.Equals(loadStartScreenButton.name)
                && GameLevelManager.instance.Controls.UINavigation.Submit.triggered)
            {
                StartCoroutine(loadstartScreen());
            }
            // quit
            if (currentHighlightedButton.name.Equals(cancelMenuButton.name)
                && GameLevelManager.instance.Controls.UINavigation.Submit.triggered)
            {
                TogglePause();
            }
            // quit
            if (currentHighlightedButton.name.Equals(quitGameButton.name)
                && GameLevelManager.instance.Controls.UINavigation.Submit.triggered)
            {
                StartCoroutine(Quit());
            }
        }
    }

    private void disableMobileOnlyPauseOptions()
    {
        // mobile buttons
        maxStatsObject = GameObject.Find(toggleMaxStatsName).gameObject;
        toggleFpsObject = GameObject.Find(toggleFpsName).gameObject;
        toggleUiStatsObject = GameObject.Find(toggleUiStatsName).gameObject;

        maxStatsObject.SetActive(false);
        toggleFpsObject.SetActive(false);
        toggleUiStatsObject.SetActive(false);
    }

    public IEnumerator Quit()
    {
        // update all time stats
        if (DBConnector.instance != null &&
           (GameOptions.gameModeSelectedName.ToLower().Contains("free") || GameOptions.gameModeSelectedId == 99))
        {
            updateFreePlayStats();
        }
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);
        QuitApplication();
    }

    public IEnumerator loadstartScreen()
    {
        // update all time stats
        if (DBConnector.instance != null &&
           (GameOptions.gameModeSelectedName.ToLower().Contains("free") || GameOptions.gameModeSelectedId == 99))
        {
            updateFreePlayStats();
        }
        if (DBConnector.instance != null)
        {
            yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);
            // load screen should be first scene in build
            SceneManager.LoadScene(SceneNameConstants.SCENE_NAME_level_00_loading);
        }
        else
        {
            // load screen should be first scene in build
            SceneManager.LoadScene(SceneNameConstants.SCENE_NAME_level_00_loading);
        }
    }

    public void reloadScene()
    {
        // update all time stats
        if (DBConnector.instance != null
            && (GameOptions.gameModeSelectedName.ToLower().Contains("free") || GameOptions.gameModeSelectedId == 99))
        {
            updateFreePlayStats();
            //make sure new high scores (if any) are loaded
            PlayerData.instance.loadStatsFromDatabase();
        }
        // check if game still paused. on reload, game should be active
        if (paused)
        {
            TogglePause();
        }
        // load highscores before loading scene
        if (PlayerData.instance != null)
        {
            try
            {
                PlayerData.instance.loadStatsFromDatabase();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private static void updateFreePlayStats()
    {
        //set time played to stopped
        GameRules.instance.setTimePlayed();
        // save free play stats
        // convert basketball stats to high score model
        HighScoreModel dBHighScoreModel = new HighScoreModel();
        HighScoreModel dBHighScoreModelTemp = new HighScoreModel();
        dBHighScoreModelTemp = dBHighScoreModel.convertBasketBallStatsToModel(BasketBall.instance.GameStats);

        DBConnector.instance.savePlayerGameStats(dBHighScoreModelTemp);
        // update all time stats
        DBConnector.instance.savePlayerAllTimeStats(BasketBall.instance.GameStats);
        DBConnector.instance.savePlayerProfileProgression(BasketBall.instance.GameStats.ExperienceGained);
    }

    private void setPauseScreen(bool value)
    {
        //// if ui stats enables, trn off
        //if (BasketBall.instance.UiStatsEnabled && paused)
        //{
        //    BasketBall.instance.toggleUiStats();
        //}

        loadSceneText.enabled = value;
        loadStartScreenText.enabled = value;
        quitGameText.enabled = value;
        cancelMenuText.enabled = value;

        loadSceneButton.enabled = value;
        loadStartScreenButton.enabled = value;
        cancelMenuButton.enabled = value;
        quitGameButton.enabled = value;
        controlsObject.SetActive(value);

        //toggleCameraText.enabled = value;
        toggleFpsText.enabled = value;
        toggleMaxStatsText.enabled = value;
        toggleUiStatsText.enabled = value;
    }

    public bool TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            //gameManager.instance.backgroundFade.SetActive(false);
            paused = false;
            Time.timeScale = 1f;
            setBackgroundFade(false);
            setPauseScreen(false);
            resumeAllAudio();

            if (GameLevelManager.instance.Joystick != null)
            {
                GameLevelManager.instance.Joystick.enabled = true;
            }
            return false;
        }
        else
        {
            //gameManager.instance.backgroundFade.SetActive(true);
            paused = true;
            Time.timeScale = 0f;
            pauseAllAudio();
            setBackgroundFade(true);
            setPauseScreen(true);

            if (GameLevelManager.instance.Joystick != null)
            {
                GameLevelManager.instance.Joystick.enabled = false;
            }
            return true;
        }
    }

    public void setTimeScaleToActive()
    {
        Time.timeScale = 1f;
    }

    public void setBackgroundFade(bool value)
    {
        fadeTexture.enabled = value;
    }

    public bool Paused
    {
        get => paused; set => paused = value;
    }
    public Button LoadSceneButton { get => loadSceneButton; set => loadSceneButton = value; }
    public Button LoadStartScreenButton { get => loadStartScreenButton; set => loadStartScreenButton = value; }
    public Button CancelMenuButton { get => cancelMenuButton; set => cancelMenuButton = value; }
    public Button QuitGameButton { get => quitGameButton; set => quitGameButton = value; }

    public static string ToggleCameraName => toggleCameraName;

    public static string ToggleUiStatsName => toggleUiStatsName;

    public static string ToggleMaxStatsName => toggleMaxStatsName;

    public static string ToggleFpsName => toggleFpsName;

    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    void pauseAllAudio()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioS in allAudioSources)
        {
            //audioS.Stop();
            audioS.Pause();
        }
    }

    void resumeAllAudio()
    {
        allAudioSources = FindObjectsOfType<AudioSource>();

        foreach (AudioSource audioS in allAudioSources)
        {
            //audioS.Stop();
            audioS.UnPause();
        }
    }

    private void QuitApplication()
    {
        Application.Quit();
    }

}
