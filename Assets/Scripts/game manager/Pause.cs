using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using TeamUtility.IO;
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

    private GameObject currentHighlightedButton;

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
        if (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Cancel")
            || InputManager.GetKeyDown(KeyCode.Escape)
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
                && (InputManager.GetKeyDown(KeyCode.Return)
                    || InputManager.GetKeyDown(KeyCode.Space)))
                    //|| InputManager.GetButtonDown("Fire1")))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                // check if game still paused. on reload, game should be active
                if (paused)
                {
                    TogglePause();
                }
                // update all time stats
                if (DBConnector.instance != null && GameOptions.gameModeSelectedName.ToLower().Contains("free"))
                {
                    updateFreePlayStats();
                }
            }

            //load start screen
            if (currentHighlightedButton.name.Equals(loadStartScreenButton.name)
                && (InputManager.GetKeyDown(KeyCode.Return)
                || InputManager.GetKeyDown(KeyCode.Space)))
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
                && (InputManager.GetKeyDown(KeyCode.Return)
                || InputManager.GetKeyDown(KeyCode.Space)))
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
    }


    //// up arrow navigation
    //if (InputManager.GetKeyDown(KeyCode.UpArrow)
    //    && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
    //    && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
    //    && !currentHighlightedButton.Equals(modeSelectOptionButtonName))
    //{
    //    EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //        .GetComponent<Button>().FindSelectableOnUp().gameObject);
    //}

    //// down arrow navigation
    //if (InputManager.GetKeyDown(KeyCode.DownArrow)
    //    && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
    //    && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
    //    && !currentHighlightedButton.Equals(modeSelectOptionButtonName))
    //{
    //    EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //        .GetComponent<Button>().FindSelectableOnDown().gameObject);
    //}


    //// right arrow on player select
    //if (InputManager.GetKeyDown(KeyCode.RightArrow))
    //{
    //    //Debug.Log("right : player select");
    //    if (currentHighlightedButton.Equals(playerSelectButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnRight().gameObject);
    //    }
    //    //Debug.Log("right : level select");
    //    if (currentHighlightedButton.Equals(levelSelectButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnRight().gameObject);
    //    }
    //}

    //// left arrow navigation on player options
    //if (InputManager.GetKeyDown(KeyCode.LeftArrow))
    //{
    //    //Debug.Log("left : player select");
    //    if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnLeft().gameObject);
    //    }
    //    //Debug.Log("left : level select");
    //    if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnLeft().gameObject);
    //    }
    //}

    //// up/down arrow on player options
    //if ((InputManager.GetKeyDown(KeyCode.W) || InputManager.GetKeyDown(KeyCode.UpArrow)))
    //{
    ////    //Debug.Log("up : player option");
    ////    if (currentHighlightedButton.Equals(loadSceneButton))
    ////    {

    ////    }

    ////    //Debug.Log("up : level option");
    ////    if (currentHighlightedButton.Equals(loadStartScreenButton))
    ////    {

    ////    }

    ////    //Debug.Log("up : level option");
    ////    if (currentHighlightedButton.Equals(quitGameButton))
    ////    {

    ////    }
    //}

    //if ((InputManager.GetKeyDown(KeyCode.S) || InputManager.GetKeyDown(KeyCode.DownArrow)))
    //{
    //    //Debug.Log("down : player option");
    //    if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //    {
    //        changeSelectedPlayerDown();
    //        initializePlayerDisplay();
    //    }

    //    //Debug.Log("down : level option");
    //    if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
    //    {
    //        changeSelectedLevelDown();
    //        initializeLevelDisplay();
    //    }

    //    if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
    //    {
    //        changeSelectedModeDown();
    //        intializeModeDisplay();
    //    }
    //}
    private void setPauseScreen(bool value)
    {
        loadSceneText.enabled = value;
        loadStartScreenText.enabled = value;
        quitGameText.enabled = value;

        loadSceneButton.enabled = value;
        loadStartScreenButton.enabled = value;
        quitGameButton.enabled = value;
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
            //resumeAllAudio();

            return (false);
        }
        else
        {

            //gameManager.instance.backgroundFade.SetActive(true);
            paused = true;
            Time.timeScale = 0f;
            //pauseAllAudio();
            setBackgroundFade(true);
            setPauseScreen(true);

            return (true);
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

    private void Quit()
    {
        Application.Quit();
    }

}
