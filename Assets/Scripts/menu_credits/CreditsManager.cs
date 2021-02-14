
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class CreditsManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;

    //version text
    private Text versionText;

    //const object names
    private const string startButtonName = "press_start";
    private const string statsMenuButtonName = "stats_menu";
    private const string quitButtonName = "quit_game";
    private const string optionsMenuButtonName = "options_menu";
    private const string creditsMenuButtonName = "credits_menu";
    private const string updateMenuButtonName = "update_menu";
    private const string updatePointsAvailable = "update_points_available";

    // scene name
    private const string statsMenuSceneName = "level_00_stats";
    private const string startMenuSceneName = "level_00_start";
    private const string loadScreenSceneName = "level_00_loading";
    private const string progressionScreenSceneName = "level_00_progression";
    private const string creditsScreenSceneName = "level_00_credits";

    private const string playerSelectButtonName = "player_select";
    private const string playerSelectOptionButtonName = "player_selected_name";
    private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";
    private const string playerSelectUnlockObjectName = "player_selected_unlock";
    private const string playerSelectIsLockedObjectName = "player_selected_lock_texture";
    private const string playerSelectStatsCategoryName = "player_selected_stats_category";

    [SerializeField]
    public PlayerControls controls;

    [SerializeField]
    public static CreditsManager instance;

    bool buttonPressed = false;
    bool dataLoaded = false;

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
    }

    //private Text gameModeSelectText;
    void Awake()
    {
        instance = this;

        controls = new PlayerControls();
        // find all button / text / etc and assign to variables
        //getUiObjectReferences();
    }

    // Start is called before the first frame update
    void Start()
    {
        AnaylticsManager.MenuStartLoaded();
    }


    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
            }
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
        }

        // ================================== footer buttons =====================================================================
        // start button | start game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(startButtonName))
        {
            loadMenu(startMenuSceneName);
        }
        // quit button | quit game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(quitButtonName))
        {
            Application.Quit();
        }
        // stats menu button | load stats menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(statsMenuButtonName))
        {
            loadMenu(statsMenuSceneName);
        }

        // stats menu button | load stats menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(updateMenuButtonName))
        {
            loadMenu(progressionScreenSceneName);
        }
        // crediys menu button | load stats menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(creditsMenuButtonName))
        {
            loadMenu(creditsScreenSceneName);
        }


        // ================================== navigation =====================================================================
        
    }


    // ============================  footer options activate - load scene/stats/quit/etc ==============================
   
    public void loadMenu(string sceneName)
    {
        //Debug.Log("load scene : " + sceneName);

        SceneManager.LoadScene(sceneName);
    }

  

    // ============================  message display ==============================
    // used in this context to display if item is locked

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

    // ============================  navigation functions ==============================
    

    // ============================  public var references  ==============================
    // dont think some of these are used, keep an eye on this on refactor
    // button names
    public static string PlayerSelectOptionButtonName => playerSelectOptionButtonName;
    public static string CreditsMenuButtonName => creditsMenuButtonName;
    public static string StartButtonName => startButtonName;
    public static string StatsMenuButtonName => statsMenuButtonName;
    public static string QuitButtonName => quitButtonName;
    //scene names
    public static string StatsMenuSceneName => statsMenuSceneName;
    public static string ProgressionScreenSceneName => progressionScreenSceneName;
    public static string UpdateMenuButtonName => updateMenuButtonName;
    public static string CreditsScreenSceneName => creditsScreenSceneName;

}
