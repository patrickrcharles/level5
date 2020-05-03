using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private bool paused;
    public static Pause instance;
    private Image fadeTexture;

    private Text loadSceneText;
    private Text loadStartScreenText;
    private Text quitGameText;

    [SerializeField]
    private Button loadSceneButton;
    [SerializeField]
    private Button loadStartScreenButton;
    [SerializeField]
    private Button quitGameButton;

    [SerializeField]
    private GameObject currentHighlightedButton;


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

        if (Time.timeScale == 1f)
        {
            setBackgroundFade(false);
            setPauseScreen(false);
        }

        currentHighlightedButton = EventSystem.current.firstSelectedGameObject;
    }

    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {
        //if (GameLevelManager.instance.GameOver)
        //{
        //    currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
        //    //Debug.Log("current : "+ currentHighlightedButton);
        //}
        //pause ESC, submit, cancel
        if (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Cancel")
            || InputManager.GetKeyDown(KeyCode.Escape)
            && !GameLevelManager.instance.GameOver)
        {
            paused = TogglePause();
        }

        // ===== pause checks to catch bugs ==============
        if (Time.timeScale == 0 && !paused)
        {
            Debug.Log("timescale = 0");
            TogglePause();
        }
        if (Time.timeScale == 1 && paused)
        {
            Debug.Log("timescale = 0");
            TogglePause();
        }
        //===============================================

        // if paused, show pause menu
        if (paused)
        {
            // if no button is selected (null). this happens with mouse clicks on the screen.
            // reset to default button
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject; // + "_description";

            if (currentHighlightedButton == null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
                currentHighlightedButton = EventSystem.current.currentSelectedGameObject; // + "_description";
            }

            // ================== pause menu options ==============================================================

            // reload scene
            if (currentHighlightedButton.name.Equals(loadSceneButton.name)
                && (InputManager.GetKeyDown(KeyCode.Return) || InputManager.GetKeyDown(KeyCode.Space)))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                if (paused)
                {
                    TogglePause();
                }
            }

            //load start screen
            if (currentHighlightedButton.name.Equals(loadStartScreenButton.name)
            && (InputManager.GetKeyDown(KeyCode.Return) || InputManager.GetKeyDown(KeyCode.Space)))
            {
                SceneManager.LoadScene("level_00_start");
            }
            // quit
            if (currentHighlightedButton.name.Equals(quitGameButton.name)
            && (InputManager.GetKeyDown(KeyCode.Return) || InputManager.GetKeyDown(KeyCode.Space)))
            {
                Debug.Log("trail3");
                Quit();
            }
        }
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
    //    Debug.Log("right : player select");
    //    if (currentHighlightedButton.Equals(playerSelectButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnRight().gameObject);
    //    }
    //    Debug.Log("right : level select");
    //    if (currentHighlightedButton.Equals(levelSelectButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnRight().gameObject);
    //    }
    //}

    //// left arrow navigation on player options
    //if (InputManager.GetKeyDown(KeyCode.LeftArrow))
    //{
    //    Debug.Log("left : player select");
    //    if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnLeft().gameObject);
    //    }
    //    Debug.Log("left : level select");
    //    if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnLeft().gameObject);
    //    }
    //}

    //// up/down arrow on player options
    //if ((InputManager.GetKeyDown(KeyCode.W) || InputManager.GetKeyDown(KeyCode.UpArrow)))
    //{
    ////    Debug.Log("up : player option");
    ////    if (currentHighlightedButton.Equals(loadSceneButton))
    ////    {

    ////    }

    ////    Debug.Log("up : level option");
    ////    if (currentHighlightedButton.Equals(loadStartScreenButton))
    ////    {

    ////    }

    ////    Debug.Log("up : level option");
    ////    if (currentHighlightedButton.Equals(quitGameButton))
    ////    {

    ////    }
    //}

    //if ((InputManager.GetKeyDown(KeyCode.S) || InputManager.GetKeyDown(KeyCode.DownArrow)))
    //{
    //    Debug.Log("down : player option");
    //    if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //    {
    //        changeSelectedPlayerDown();
    //        initializePlayerDisplay();
    //    }

    //    Debug.Log("down : level option");
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
        Debug.Log("togglepause");
        //Debug.Log("timescale : "+ Time.timeScale);
        if (Time.timeScale == 0f)
        {
            //gameManager.instance.backgroundFade.SetActive(false);
            paused = false;
            Time.timeScale = 1f;
            setBackgroundFade(false);
            setPauseScreen(false);
            //resumeAllAudio();

            Debug.Log("timescale : " + Time.timeScale);
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

            Debug.Log("timescale : " + Time.timeScale);
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
