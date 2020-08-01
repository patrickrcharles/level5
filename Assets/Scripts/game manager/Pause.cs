﻿
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    // main flag
    private bool paused;

    //fade texture to obscure background
    private Image fadeTexture;

    // ui text
    private Text loadSceneText;
    private Text loadStartScreenText;
    private Text quitGameText;

    //ui buttons
    private Button loadSceneButton;
    private Button loadStartScreenButton;
    private Button quitGameButton;
    private AudioSource[] allAudioSources;
    private GameObject currentHighlightedButton;

    GameObject controlsObject;

    public static Pause instance;

    void Awake()
    {
        instance = this;
        fadeTexture = GameObject.Find("fade_texture").GetComponent<Image>();

        loadSceneText = GameObject.Find("load_scene").GetComponent<Text>();
        loadStartScreenText = GameObject.Find("load_start").GetComponent<Text>();
        quitGameText = GameObject.Find("quit_game").GetComponent<Text>();

        loadSceneButton = GameObject.Find("load_scene").GetComponent<Button>();
        loadStartScreenButton = GameObject.Find("load_start").GetComponent<Button>();
        quitGameButton = GameObject.Find("quit_game").GetComponent<Button>();

        controlsObject = GameObject.Find("controls");

        if(controlsObject != null)
        {
            controlsObject.SetActive(false);
        }

       
        // if game active, disable pause
        if (Time.timeScale == 1f)
        {
            setBackgroundFade(false);
            setPauseScreen(false);
        }
        // init current button
        currentHighlightedButton = EventSystem.current.firstSelectedGameObject.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //pause ESC, submit, cancel
        if (GameLevelManager.Instance.Controls.Player.submit.triggered
            || GameLevelManager.Instance.Controls.Player.cancel.triggered
            //|| GameLevelManager.Instance.Controls.Player.esc.triggered
            && !GameLevelManager.Instance.GameOver)
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
            TogglePause();
        }
        //==========================================================

        //disable cotnrols display if game over
        if (GameRules.instance.GameOver)
        {
            controlsObject.SetActive(false);
        }

        // if paused, show pause menu
        if (paused)
        {
            // check for some button not selected
            //*this is a hack but it works patch for v3.0.1 : clicking mouse causing game to crash
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
            }
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject; // + "_description";
            currentHighlightedButton.GetComponent<Button>().Select();
            currentHighlightedButton.GetComponent<Button>().OnSelect(null);

            // ================== pause menu options ==============================================================

            // reload scene
            if (currentHighlightedButton.name.Equals(loadSceneButton.name)
                && (GameLevelManager.Instance.Controls.Player.submit.triggered
                    || GameLevelManager.Instance.Controls.Player.jump.triggered))
                    //|| InputManager.GetButtonDown("Fire1")))
            {

                // update all time stats
                if (DBConnector.instance != null && GameOptions.gameModeSelectedName.ToLower().Contains("free"))
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            //load start screen
            if (currentHighlightedButton.name.Equals(loadStartScreenButton.name)
                && (GameLevelManager.Instance.Controls.Player.submit.triggered
                    || GameLevelManager.Instance.Controls.Player.jump.triggered))
                //|| InputManager.GetButtonDown("Fire1")))
            {
                // update all time stats
                if (DBConnector.instance != null && GameOptions.gameModeSelectedName.ToLower().Contains("free") )
                {
                    updateFreePlayStats();
                }

                // start screen should be first scene in build
                SceneManager.LoadScene("level_00_start");
            }
            // quit
            if (currentHighlightedButton.name.Equals(quitGameButton.name)
                && (GameLevelManager.Instance.Controls.Player.submit.triggered
                    || GameLevelManager.Instance.Controls.Player.jump.triggered))
                //|| InputManager.GetButtonDown("Fire1"))
            {
                // update all time stats
                if (DBConnector.instance != null && GameOptions.gameModeSelectedName.ToLower().Contains("free"))
                {
                    updateFreePlayStats();
                }
                Quit();
            }
        }
    }

    private static void updateFreePlayStats()
    {
        //set time played to stopped
        GameRules.instance.setTimePlayed();

        // save free play stats
        DBConnector.instance.savePlayerGameStats(BasketBall.instance.BasketBallStats);
        // update all time stats
        DBConnector.instance.savePlayerAllTimeStats(BasketBall.instance.BasketBallStats);
        //DBConnector.instance.saveHitByCarGameStats(PlayerData.instance.HitByCars);
    }

    private void setPauseScreen(bool value)
    {
        loadSceneText.enabled = value;
        loadStartScreenText.enabled = value;
        quitGameText.enabled = value;

        loadSceneButton.enabled = value;
        loadStartScreenButton.enabled = value;
        quitGameButton.enabled = value;
        controlsObject.SetActive(value);
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
        get => paused;
    }

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

    private void Quit()
    {
        Application.Quit();
    }

}
