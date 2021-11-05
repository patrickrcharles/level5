
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class StartManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;
    //list of all shooter profiles with player data
    [SerializeField]
    private List<CharacterProfile> playerSelectedData;
    // list off cheerleader profile data
    [SerializeField]
    private List<CheerleaderProfile> cheerleaderSelectedData;
    // list off level  data
    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;
    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;

    //private Text playerSelectUnlockText;

    [SerializeField]
    private Text cheerleaderSelectUnlockText;

    // option select buttons, this will be disabled with touch input
    Button levelSelectButton;
    Button trafficSelectButton;
    Button hardcoreSelectButton;
    Button enemySelectButton;
    Button sniperSelectButton;
    Button difficultySelectButton;
    Button obstacleSelectButton;
    Button playerSelectButton;
    Button CheerleaderSelectButton;
    Button modeSelectButton;

    //player selected display
    private Text playerSelectOptionText;
    private Image playerSelectOptionImage;
    private Text playerSelectOptionStatsText;
    private Text playerSelectCategoryStatsText;
    private Text playerProgressionCategoryText;
    private Text playerProgressionStatsText;
    [SerializeField]
    private Text playerProgressionUpdatePointsText;

    // level select display
    private Text levelSelectOptionText;

    //cheerleader selected display
    private Text cheerleaderSelectOptionText;
    private Image cheerleaderSelectOptionImage;

    //mode selected display
    private Text modeSelectOptionText;
    private Text ModeSelectOptionDescriptionText;

    //selectable option text
    [SerializeField]
    private Text trafficSelectOptionText;
    [SerializeField]
    private Text hardcoreSelectOptionText;
    [SerializeField]
    private Text enemySelectOptionText;
    [SerializeField]
    private Text sniperSelectOptionText;
    [SerializeField]
    private Text difficultySelectOptionText;
    [SerializeField]
    private Text difficultySelectOptionDescriptionText;
    [SerializeField]
    private Text obstacleSelectOptionText;

    //version text
    private Text versionText;
    private Text latestVersionText;

    [SerializeField]
    private Text userNameText;

    //const object names
    private const string startButtonName = "press_start";
    private const string statsMenuButtonName = "stats_menu";
    private const string quitButtonName = "quit_game";
    private const string optionsMenuButtonName = "options_menu";
    private const string creditsMenuButtonName = "credits_menu";
    private const string updateMenuButtonName = "update_menu";
    private const string accountMenuButtonName = "account_menu";
    private const string updatePointsAvailable = "update_points_available";

    private const string playerSelectButtonName = "player_select";
    private const string playerSelectOptionButtonName = "player_selected_name";
    private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";
    //private const string playerSelectUnlockObjectName = "player_selected_unlock";
    //private const string playerSelectIsLockedObjectName = "player_selected_lock_texture";
    private const string playerSelectStatsCategoryName = "player_selected_stats_category";

    private const string playerProgressionName = "player_progression";
    private const string playerProgressionStatsName = "player_progression_stats";

    //cheerleader objects
    private const string cheerleaderSelectButtonName = "cheerleader_select";
    private const string cheerleaderSelectOptionButtonName = "cheerleader_selected_name";
    private const string cheerleaderSelectImageObjectName = "cheerleader_selected_image";
    private const string cheerleaderSelectUnlockObjectName = "cheerleader_selected_unlock";
    private const string cheerleaderSelectIsLockedObjectName = "cheerleader_selected_lock_texture";

    //level objects
    private const string levelSelectButtonName = "level_select";
    private const string levelSelectOptionButtonName = "level_selected_name";

    //mode objects
    private const string modeSelectButtonName = "mode_select";
    private const string modeSelectOptionButtonName = "mode_selected_name";
    private const string modeSelectDescriptionObjectName = "mode_selected_description";

    //traffic objects
    private const string trafficSelectButtonName = "traffic_select";
    private const string trafficSelectOptionName = "traffic_select_option";

    //hardcore mode
    private const string hardcoreSelectButtonName = "hardcore_select";
    private const string hardcoreSelectOptionName = "hardcore_select_option";

    //hardcore mode
    private const string enemySelectButtonName = "enemy_select";
    private const string enemySelectOptionName = "enemy_select_option";

    //sniper
    private const string sniperSelectButtonName = "sniper_select";
    private const string sniperSelectOptionName = "sniper_select_option";
    //difficulty
    private const string difficultySelectButtonName = "difficulty_select";
    private const string difficultySelectOptionName = "difficulty_select_option";
    private const string difficultySelectDescriptionName = "difficulty_selected_description";
    //obstacle
    private const string obstacleSelectButtonName = "obstacle_select";
    private const string obstacleSelectOptionName = "obstacle_select_option";

    [SerializeField]
    private bool trafficEnabled;
    [SerializeField]
    private bool hardcoreEnabled;
    [SerializeField]
    private bool enemiesEnabled;
    [SerializeField]
    private bool obstaclesEnabled;
    [SerializeField]
    private int difficultySelected;
    [SerializeField]
    private bool sniperEnabled;
    private int playerSelectedIndex;
    private int levelSelectedIndex;
    private int modeSelectedIndex;
    private int cheerleaderSelectedIndex;

    [SerializeField]
    public PlayerControls controls;

    [SerializeField]
    public static StartManager instance;

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
        StartCoroutine(getLoadedData());

        controls = new PlayerControls();
        // find all button / text / etc and assign to variables
        getUiObjectReferences();

        //default index for player selected
        playerSelectedIndex = GameOptions.playerSelectedIndex;
        cheerleaderSelectedIndex = GameOptions.cheerleaderSelectedIndex;
        levelSelectedIndex = GameOptions.levelSelectedIndex;
        modeSelectedIndex = GameOptions.modeSelectedIndex;
        trafficEnabled = GameOptions.trafficEnabled;
        hardcoreEnabled = GameOptions.hardcoreModeEnabled;
        difficultySelected = GameOptions.difficultySelected;
        obstaclesEnabled = GameOptions.obstaclesEnabled;

        // update experience and levels
        // recommended here because experience will be gained after every game played
        StartCoroutine(UpdateLevelAndExperienceFromDatabase());

        // diable nav buttons if mobile
#if UNITY_ANDROID && !UNITY_EDITOR
            disableButtonsNotUsedForTouchInput();
#endif
        //StartCoroutine(InitializeDisplay());
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeDisplay());
        StartCoroutine(SetVersion());
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

        // if player highlighted, display player
        if ((!currentHighlightedButton.Equals(cheerleaderSelectButtonName) || !currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
            && dataLoaded)
        {
            try
            {
                initializePlayerDisplay();
            }
            catch
            {
                return;
            }
        }

        if (currentHighlightedButton.Equals(cheerleaderSelectButtonName) || currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
        {
            try
            {
                initializeCheerleaderDisplay();
            }
            catch
            {
                return;
            }
        }
        // ================================== footer buttons =====================================================================
        // start button | start game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(startButtonName))
        {
            loadGame();
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
            loadMenu(Constants.SCENE_NAME_level_00_stats);
        }

        // update menu button | load update menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(updateMenuButtonName))
        {
            GameOptions.playerSelectedIndex = playerSelectedIndex;
            loadMenu(Constants.SCENE_NAME_level_00_progression);
        }
        // options menu button | load options menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(optionsMenuButtonName))
        {
            loadMenu(Constants.SCENE_NAME_level_00_options);
        }
        // credits menu button | load credits menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(creditsMenuButtonName))
        {
            loadMenu(Constants.SCENE_NAME_level_00_credits);
        }

        // account menu button | load account menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(accountMenuButtonName))
        {
            loadMenu(Constants.SCENE_NAME_level_00_account);
        }

        // ================================== navigation =====================================================================
        // up, option select
        if (controls.UINavigation.Up.triggered && !buttonPressed
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
            && !currentHighlightedButton.Equals(trafficSelectOptionName)
            && !currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName)
            && !currentHighlightedButton.Equals(hardcoreSelectOptionName)
            && !currentHighlightedButton.Equals(enemySelectOptionName)
            && !currentHighlightedButton.Equals(difficultySelectOptionName)
            && !currentHighlightedButton.Equals(obstacleSelectOptionName)
            && !currentHighlightedButton.Equals(SniperSelectOptionName))
        {
            buttonPressed = true;
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnUp().gameObject);
            buttonPressed = false;
        }

        // down, option select
        if (controls.UINavigation.Down.triggered && !buttonPressed
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
            && !currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName)
            && !currentHighlightedButton.Equals(trafficSelectOptionName)
            && !currentHighlightedButton.Equals(hardcoreSelectOptionName)
            && !currentHighlightedButton.Equals(difficultySelectOptionName)
            && !currentHighlightedButton.Equals(enemySelectOptionName)
            && !currentHighlightedButton.Equals(obstacleSelectOptionName)
            && !currentHighlightedButton.Equals(SniperSelectOptionName))
        {
            buttonPressed = true;
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnDown().gameObject);
            buttonPressed = false;
        }

        // right, go to change options
        if (controls.UINavigation.Right.triggered
            && EventSystem.current.currentSelectedGameObject.GetComponent<Button>()
            .FindSelectableOnRight() != null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnRight().gameObject);
        }

        // left, return to option select
        if (controls.UINavigation.Left.triggered)
        {
            // check if button exists. if no selectable on left, throws null object exception
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnLeft() != null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
        }

        // ================================== change options =============================================================
        // up, change options
        if (controls.UINavigation.Up.triggered && !buttonPressed)
        {
            buttonPressed = true;
            try
            {
                if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
                {
                    changeSelectedPlayerUp();
                    initializePlayerDisplay();
                }
                if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
                {
                    changeSelectedLevelUp();
                    initializeLevelDisplay();
                }
                if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
                {
                    changeSelectedModeUp();
                    intializeModeDisplay();
                }
                if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
                {
                    changeSelectedCheerleaderUp();
                    initializeCheerleaderDisplay();
                }
                if (currentHighlightedButton.Equals(trafficSelectOptionName))
                {
                    // disabled for now. default : OFF
                    changeSelectedTrafficOption();
                    initializeTrafficOptionDisplay();
                }
                if (currentHighlightedButton.Equals(hardcoreSelectOptionName))
                {
                    changeSelectedHardcoreOption();
                    initializeHardcoreOptionDisplay();
                }
                if (currentHighlightedButton.Equals(enemySelectOptionName))
                {
                    changeSelectedEnemiesOption();
                    initializeEnemyOptionDisplay();
                }
                if (currentHighlightedButton.Equals(SniperSelectOptionName))
                {
                    changeSelectedSniperOption();
                    initializeSniperOptionDisplay();
                }
                if (currentHighlightedButton.Equals(difficultySelectOptionName))
                {
                    changeSelectedDifficultyOption();
                    initializeDifficultyOptionDisplay();
                }
                if (currentHighlightedButton.Equals(ObstacleSelectOptionName))
                {
                    changeSelectedObstacleOption();
                    initializeObstacleOptionDisplay();
                }
            }
            catch
            {
                return;
            }
            buttonPressed = false;
        }
        // down, change option
        if (controls.UINavigation.Down.triggered && !buttonPressed)
        {
            buttonPressed = true;
            try
            {
                if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
                {
                    changeSelectedPlayerDown();
                    initializePlayerDisplay();
                }
                if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
                {
                    changeSelectedLevelDown();
                    initializeLevelDisplay();
                }
                if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
                {
                    changeSelectedModeDown();
                    intializeModeDisplay();
                }
                if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
                {
                    changeSelectedCheerleaderDown();
                    initializeCheerleaderDisplay();
                }
                if (currentHighlightedButton.Equals(trafficSelectOptionName))
                {
                    changeSelectedTrafficOption();
                    initializeTrafficOptionDisplay();

                }
                if (currentHighlightedButton.Equals(hardcoreSelectOptionName))
                {
                    changeSelectedHardcoreOption();
                    initializeHardcoreOptionDisplay();
                }
                if (currentHighlightedButton.Equals(enemySelectOptionName))
                {
                    changeSelectedEnemiesOption();
                    initializeEnemyOptionDisplay();
                }
                if (currentHighlightedButton.Equals(SniperSelectOptionName))
                {
                    changeSelectedSniperOption();
                    initializeSniperOptionDisplay();
                }
                if (currentHighlightedButton.Equals(difficultySelectOptionName))
                {
                    changeSelectedDifficultyOption();
                    initializeDifficultyOptionDisplay();
                }
                if (currentHighlightedButton.Equals(ObstacleSelectOptionName))
                {
                    changeSelectedObstacleOption();
                    initializeObstacleOptionDisplay();
                }
            }
            catch
            {
                return;
            }
            buttonPressed = false;
        }
    }

    IEnumerator UpdateLevelAndExperienceFromDatabase()
    {
        yield return new WaitUntil(() => dataLoaded);

        foreach (CharacterProfile s in playerSelectedData)
        {
            s.Experience = DBHelper.instance.getIntValueFromTableByFieldAndCharId("CharacterProfile", "experience", s.PlayerId);
            s.Level = DBHelper.instance.getIntValueFromTableByFieldAndCharId("CharacterProfile", "level", s.PlayerId);
        }
    }

    IEnumerator getLoadedData()
    {
        if (LoadedData.instance != null)
        {
            yield return new WaitUntil(() => LoadedData.instance.PlayerSelectedData != null);

            playerSelectedData = LoadedData.instance.PlayerSelectedData;

            yield return new WaitUntil(() => LoadedData.instance.CheerleaderSelectedData != null);
            cheerleaderSelectedData = LoadedData.instance.CheerleaderSelectedData;

            yield return new WaitUntil(() => LoadedData.instance.LevelSelectedData != null);
            levelSelectedData = LoadedData.instance.LevelSelectedData;

            yield return new WaitUntil(() => LoadedData.instance.ModeSelectedData != null);
            modeSelectedData = LoadedData.instance.ModeSelectedData;

            if (playerSelectedData != null
                && cheerleaderSelectedData != null
                && levelSelectedData != null
                && modeSelectedData != null)
            {
                dataLoaded = true;
            }
        }
        else
        {
            SceneManager.LoadScene(Constants.SCENE_NAME_level_00_loading);
        }
    }

    IEnumerator InitializeDisplay()
    {
        yield return new WaitUntil(() => dataLoaded);
        //yield return new WaitForSeconds(0.05f);
        // display default data
        initializeCheerleaderDisplay();
        initializePlayerDisplay();
        initializeLevelDisplay();
        intializeModeDisplay();
        initializeTrafficOptionDisplay();
        initializeHardcoreOptionDisplay();
        initializeSniperOptionDisplay();
        initializeDifficultyOptionDisplay();
        initializeObstacleOptionDisplay();
        setInitialGameOptions();
    }

    private IEnumerator SetVersion()
    {
        if (APIHelper.BearerToken != null && !string.IsNullOrEmpty(GameOptions.userName))
        {
            userNameText.text = "username : " + GameOptions.userName + " connected";
        }
        if (APIHelper.BearerToken == null || string.IsNullOrEmpty(GameOptions.userName))
        {
            userNameText.text = "username : " + GameOptions.userName + " disconnected";
        }
        versionText.text = "current version: " + Application.version;
        yield return new WaitUntil(() => !APIHelper.ApiLocked);
        if (UtilityFunctions.IsConnectedToInternet())
        {
            latestVersionText.text = "latest version: " + APIHelper.GetLatestBuildVersion();
        }
        else
        {
            latestVersionText.text = "latest version: " + "No Internet";
        }
    }

    // ============================  get UI buttons / text references ==============================
    private void getUiObjectReferences()
    {
        // buttons to disable for touch input
        levelSelectButton = GameObject.Find(levelSelectButtonName).GetComponent<Button>();
        trafficSelectButton = GameObject.Find(trafficSelectButtonName).GetComponent<Button>();
        hardcoreSelectButton = GameObject.Find(hardcoreSelectButtonName).GetComponent<Button>();
        enemySelectButton = GameObject.Find(enemySelectButtonName).GetComponent<Button>();
        sniperSelectButton = GameObject.Find(sniperSelectButtonName).GetComponent<Button>();
        difficultySelectButton = GameObject.Find(difficultySelectButtonName).GetComponent<Button>();
        obstacleSelectButton = GameObject.Find(sniperSelectButtonName).GetComponent<Button>();
        playerSelectButton = GameObject.Find(obstacleSelectButtonName).GetComponent<Button>();
        CheerleaderSelectButton = GameObject.Find(cheerleaderSelectButtonName).GetComponent<Button>();
        modeSelectButton = GameObject.Find(modeSelectButtonName).GetComponent<Button>();

        // player object with lock texture and unlock text
        playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        playerSelectOptionStatsText = GameObject.Find(playerSelectStatsObjectName).GetComponent<Text>();
        playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();
        playerSelectCategoryStatsText = GameObject.Find(playerSelectStatsCategoryName).GetComponent<Text>();
        playerProgressionStatsText = GameObject.Find(playerProgressionStatsName).GetComponent<Text>();
        playerProgressionCategoryText = GameObject.Find(playerProgressionName).GetComponent<Text>();
        playerProgressionUpdatePointsText = GameObject.Find(updatePointsAvailable).GetComponent<Text>();

        // friend object with lock texture and unlock text
        cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
        cheerleaderSelectOptionImage = GameObject.Find(cheerleaderSelectImageObjectName).GetComponent<Image>();

        // options selection text
        trafficSelectOptionText = GameObject.Find(trafficSelectOptionName).GetComponent<Text>();
        hardcoreSelectOptionText = GameObject.Find(hardcoreSelectOptionName).GetComponent<Text>();
        enemySelectOptionText = GameObject.Find(enemySelectOptionName).GetComponent<Text>();
        sniperSelectOptionText = GameObject.Find(SniperSelectOptionName).GetComponent<Text>();
        difficultySelectOptionText = GameObject.Find(difficultySelectOptionName).GetComponent<Text>();
        obstacleSelectOptionText = GameObject.Find(obstacleSelectOptionName).GetComponent<Text>();

        //version
        versionText = GameObject.Find("version").GetComponent<Text>();
        latestVersionText = GameObject.Find("latestVersion").GetComponent<Text>();
        userNameText = GameObject.Find("username").GetComponent<Text>();
    }

    private void setInitialGameOptions()
    {
        GameOptions.characterObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeObjectName;
        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;

        GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;
        GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
    }

    public String getRandomWizardOfBoat()
    {

        Random random = new Random();
        int randNum = random.Next(1, 100);

        if (randNum > 50)
        {
            return "wob1";
        }
        else
        {
            return "wob2";
        }
    }

    public void disableButtonsNotUsedForTouchInput()
    {
        levelSelectButton.enabled = false;
        trafficSelectButton.enabled = false;
        playerSelectButton.enabled = false;
        CheerleaderSelectButton.enabled = false;
        modeSelectButton.enabled = false;
    }

    public void enableButtonsNotUsedForTouchInput()
    {
        Debug.Log("enable buttons");
        levelSelectButton.enabled = true;
        trafficSelectButton.enabled = true;
        playerSelectButton.enabled = true;
        CheerleaderSelectButton.enabled = true;
        modeSelectButton.enabled = true;
    }

    public void changeSelectedTrafficOption()
    {
        trafficEnabled = !trafficEnabled;
    }

    public void changeSelectedHardcoreOption()
    {
        hardcoreEnabled = !hardcoreEnabled;
    }

    public void changeSelectedEnemiesOption()
    {
        enemiesEnabled = !enemiesEnabled;
    }

    public void changeSelectedSniperOption()
    {
        sniperEnabled = !sniperEnabled;
    }

    public void changeSelectedObstacleOption()
    {
        obstaclesEnabled = !obstaclesEnabled;
    }

    public void changeSelectedDifficultyOption()
    {
        //Debug.Log("change difficulty");
        if (difficultySelected == 0)
        {
            difficultySelected = 1;
        }
        else
        {
            difficultySelected = 0;
        }
        //if (difficultySelected == 1)
        //{
        //    difficultySelected = 0;
        //}
        //Debug.Log("difficulty : "+ difficultySelected);
        //if (difficultySelected == 2)
        //{
        //    difficultySelected = 0;
        //}
    }

    // ============================  Initialize displays ==============================

    public void initializeTrafficOptionDisplay()
    {
        if (trafficEnabled)
        {
            trafficSelectOptionText.text = "ON";
        }
        if (!trafficEnabled)
        {
            trafficSelectOptionText.text = "OFF";
        }
    }

    public void initializeHardcoreOptionDisplay()
    {
        if (hardcoreEnabled)
        {
            hardcoreSelectOptionText.text = "ON";
        }
        if (!hardcoreEnabled)
        {
            hardcoreSelectOptionText.text = "OFF";
        }
    }
    public void initializeEnemyOptionDisplay()
    {
        if (enemiesEnabled)
        {
            enemySelectOptionText.text = "ON";
        }
        if (!enemiesEnabled)
        {
            enemySelectOptionText.text = "OFF";
        }
    }
    public void initializeSniperOptionDisplay()
    {
        if (sniperEnabled)
        {
            sniperSelectOptionText.text = "ON";
        }
        if (!sniperEnabled)
        {
            sniperSelectOptionText.text = "OFF";
        }
    }
    public void initializeDifficultyOptionDisplay()
    {
        difficultySelectOptionDescriptionText = GameObject.Find(difficultySelectDescriptionName).GetComponent<Text>();
        if (difficultySelected == 0)
        {
            difficultySelectOptionText.text = "easy";
            difficultySelectOptionDescriptionText.text = "max stats | 0.5x experience";
        }
        if (difficultySelected == 1)
        {
            difficultySelectOptionText.text = "normal";
            difficultySelectOptionDescriptionText.text = "basic stats | 1x experience";
        }
        //if (difficultySelected == 2)
        //{
        //    difficultySelectOptionText.text = "hardcore";
        //    difficultySelectOptionDescriptionText.text = "basic stats | 1.5x experience";
        //}
    }
    public void initializeObstacleOptionDisplay()
    {
        if (obstaclesEnabled)
        {
            obstacleSelectOptionText.text = "ON";
        }
        if (!obstaclesEnabled)
        {
            obstacleSelectOptionText.text = "OFF";
        }
    }

    public void initializeLevelDisplay()
    {
        levelSelectOptionText = GameObject.Find(levelSelectOptionButtonName).GetComponent<Text>();
        levelSelectOptionText.text = levelSelectedData[levelSelectedIndex].LevelDisplayName;
        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    }

    public void initializeCheerleaderDisplay()
    {
        try
        {
            cheerleaderSelectOptionImage.enabled = true;
            playerSelectOptionImage.enabled = false;
            playerSelectOptionStatsText.enabled = false;
            playerSelectCategoryStatsText.enabled = false;

            playerProgressionCategoryText.enabled = false;
            playerProgressionStatsText.enabled = false;

            cheerleaderSelectOptionText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
            cheerleaderSelectOptionImage.sprite = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderPortrait;


            cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
            cheerleaderSelectOptionText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
            GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    public void intializeModeDisplay()
    {
        modeSelectOptionText = GameObject.Find(modeSelectOptionButtonName).GetComponent<Text>();
        modeSelectOptionText.text = modeSelectedData[modeSelectedIndex].ModeDisplayName;

        ModeSelectOptionDescriptionText = GameObject.Find(modeSelectDescriptionObjectName).GetComponent<Text>();
        ModeSelectOptionDescriptionText.text = modeSelectedData[modeSelectedIndex].ModeDescription;
    }


    public void initializePlayerDisplay()
    {
        try
        {
            cheerleaderSelectOptionImage.enabled = false;

            playerSelectOptionImage.enabled = true;
            playerSelectOptionStatsText.enabled = true;
            playerSelectCategoryStatsText.enabled = true;

            playerProgressionCategoryText.enabled = true;
            playerProgressionStatsText.enabled = true;

            playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
            playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;

            playerSelectOptionStatsText.text = // playerSelectedData[playerSelectedIndex].Accuracy2Pt.ToString("F0") + "\n"
                playerSelectedData[playerSelectedIndex].Accuracy3Pt.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Accuracy4Pt.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Accuracy7Pt.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Release.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Range.ToString("F0") + " ft\n"
                + playerSelectedData[playerSelectedIndex].calculateSpeedToPercent().ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Luck.ToString("F0");

            playerSelectedData[playerSelectedIndex].Level =
                (playerSelectedData[playerSelectedIndex].Experience / 3000);
            int nextlvl = (((playerSelectedData[playerSelectedIndex].Level + 1) * 3000) - playerSelectedData[playerSelectedIndex].Experience);

            playerProgressionStatsText.text = playerSelectedData[playerSelectedIndex].Level.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Experience.ToString("F0") + "\n"
                + nextlvl.ToString("F0") + "\n";

            // player points avaiable for upgrade
            if (playerSelectedData[playerSelectedIndex].PointsAvailable > 0)
            {
                playerProgressionUpdatePointsText.text = "+" + playerSelectedData[playerSelectedIndex].PointsAvailable.ToString();
            }
            else
            {
                playerProgressionUpdatePointsText.text = "";
            }

            //Debug.Log("***************************************** 8");
            GameOptions.characterObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;

        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    // ============================  footer options activate - load scene/stats/quit/etc ==============================
    public void loadGame()
    {
        // tells character profile to load profile from LoadedData.instance
        GameOptions.gameModeHasBeenSelected = true;

        // update game options for game mode
        setGameOptions();

        // i create the string this way so that i can have a description of the level so i know what im opening
        string sceneName = GameOptions.levelSelected + "_" + levelSelectedData[levelSelectedIndex].LevelDescription;

        // if player not locked, cheerleader not locked, mode contains 'free', mode not aracde mode
        if (modeSelectedData[modeSelectedIndex].ModeDisplayName.ToLower().Contains("free")
            || !modeSelectedData[modeSelectedIndex].ModeDisplayName.ToLower().Contains("arcade"))
        {
            // load player progression info
            PlayerData.instance.CurrentExperience = playerSelectedData[playerSelectedIndex].Experience;
            PlayerData.instance.CurrentLevel = playerSelectedData[playerSelectedIndex].Level;
            PlayerData.instance.UpdatePointsAvailable = playerSelectedData[playerSelectedIndex].PointsAvailable;
            PlayerData.instance.UpdatePointsUsed = playerSelectedData[playerSelectedIndex].PointsUsed;
        }
        SceneManager.LoadScene(sceneName);
    }

    public void loadMenu(string sceneName)
    {
        //Debug.Log("load scene : " + sceneName);

        SceneManager.LoadScene(sceneName);
    }

    // ============================  set game options ==============================
    // this is necessary for setting Game Rules on game manager
    private void setGameOptions()
    {
        GameOptions.characterId = playerSelectedData[playerSelectedIndex].PlayerId;
        GameOptions.characterDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;

        // if Wizard of Boat selected, randomly choose which one to spawn
        if (playerSelectedData[playerSelectedIndex].PlayerDisplayName.ToLower().Contains("boat"))
        {
            GameOptions.characterObjectName = getRandomWizardOfBoat();
            //Debug.Log("GameOptions.characterObjectName : " + GameOptions.characterObjectName);
        }
        else
        {
            GameOptions.characterObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        }

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        GameOptions.levelId = levelSelectedData[levelSelectedIndex].LevelId;
        GameOptions.levelDisplayName = levelSelectedData[levelSelectedIndex].LevelDisplayName;
        GameOptions.levelRequiresTimeOfDay = levelSelectedData[levelSelectedIndex].LevelRequiresTimeOfDay;

        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeDisplayName;

        GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
        GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;

        GameOptions.gameModeRequiresShotMarkers3s = modeSelectedData[modeSelectedIndex].ModeRequiresShotMarkers3S;
        GameOptions.gameModeRequiresShotMarkers4s = modeSelectedData[modeSelectedIndex].ModeRequiresShotMarkers4S;

        GameOptions.gameModeThreePointContest = modeSelectedData[modeSelectedIndex].GameModeThreePointContest;
        GameOptions.gameModeFourPointContest = modeSelectedData[modeSelectedIndex].GameModeFourPointContest;
        GameOptions.gameModeAllPointContest = modeSelectedData[modeSelectedIndex].GameModeAllPointContest;

        // check if game mode requires timer that is not 120
        if (modeSelectedData[modeSelectedIndex].CustomTimer > 0)
        {
            GameOptions.customTimer = modeSelectedData[modeSelectedIndex].CustomTimer;
        }
        else
        {
            GameOptions.customTimer = 0;
        }

        GameOptions.gameModeRequiresMoneyBall = modeSelectedData[modeSelectedIndex].ModeRequiresMoneyBall;
        GameOptions.gameModeRequiresConsecutiveShot = modeSelectedData[modeSelectedIndex].ModeRequiresConsecutiveShots;

        GameOptions.cheerleaderDisplayName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
        GameOptions.cheerleaderId = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderId;
        GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;

        //GameOptions.trafficEnabled = trafficEnabled;

        GameOptions.applicationVersion = Application.version;
        GameOptions.operatingSystemVersion = SystemInfo.operatingSystem;

        // send current selected options to game options for next load on start manager
        GameOptions.playerSelectedIndex = playerSelectedIndex;
        GameOptions.cheerleaderSelectedIndex = cheerleaderSelectedIndex;
        GameOptions.levelSelectedIndex = levelSelectedIndex;
        GameOptions.modeSelectedIndex = modeSelectedIndex;
        GameOptions.trafficEnabled = trafficEnabled;
        GameOptions.enemiesEnabled = enemiesEnabled;
        GameOptions.hardcoreModeEnabled = hardcoreEnabled;
        GameOptions.sniperEnabled = sniperEnabled;

        GameOptions.arcadeModeEnabled = modeSelectedData[modeSelectedIndex].ArcadeModeActive;
        GameOptions.EnemiesOnlyEnabled = modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled;

        GameOptions.levelRequiresWeather = levelSelectedData[levelSelectedIndex].LevelHasWeather;
        GameOptions.difficultySelected = difficultySelected;
        GameOptions.obstaclesEnabled = obstaclesEnabled;

        GameOptions.battleRoyalEnabled = modeSelectedData[modeSelectedIndex].IsBattleRoyal;
        GameOptions.cageMatchEnabled = modeSelectedData[modeSelectedIndex].IsCageMatch;

        GameOptions.gameModeRequiresPlayerSurvive = modeSelectedData[modeSelectedIndex].GameModeRequiresPlayerSurvive;

        // if enemies only mode, enable enemies whether it was selected or not
        if (GameOptions.EnemiesOnlyEnabled || GameOptions.battleRoyalEnabled)
        {
            GameOptions.enemiesEnabled = true;
        }

        if (!levelSelectedData[levelSelectedIndex].LevelHasTraffic)
        {
            GameOptions.trafficEnabled = false;
        }

        GameOptions.customCamera = levelSelectedData[levelSelectedIndex].CustomCamera;
        //if (modeSelectedData[modeSelectedIndex].ModeId == 21)
        //{
        //    levelSelectedIndex = 15;
        //}

        // load hardcore mode highscores (for ui display) for game mode if hardcore mode enabled
        //Debug.Log("hardcore enabled : "+ GameOptions.hardcoreModeEnabled);
        PlayerData.instance.loadStatsFromDatabase();
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
    public void changeSelectedPlayerUp()
    {

        // if default index (first in list), go to end of list
        if (playerSelectedIndex == 0)
        {
            playerSelectedIndex = playerSelectedData.Count - 1;
        }
        else
        {
            // if not first index, decrement
            playerSelectedIndex--;
        }
        // check for fighting modes + if player is fighter
        if (!playerSelectedData[playerSelectedIndex].IsFighter
            && (modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled
            || enemiesEnabled))
        {
            //Debug.Log("player not fighter : " + playerSelectedData[playerSelectedIndex].PlayerObjectName);
            changeSelectedPlayerUp();
        }
        // check for shooting modes + if player is fighter
        // check for shooting modes + if player is fighter
        if (!playerSelectedData[playerSelectedIndex].IsShooter
            && !modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled
            && !enemiesEnabled)
        {
            //Debug.Log("player not shooter : " + playerSelectedData[playerSelectedIndex].PlayerObjectName);
            changeSelectedPlayerUp();
        }
        GameOptions.characterObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    }
    public void changeSelectedPlayerDown()
    {
        // if default index (first in list
        if (playerSelectedIndex == playerSelectedData.Count - 1)
        {
            playerSelectedIndex = 0;
        }
        else
        {
            //if not first index, increment
            playerSelectedIndex++;
        }
        // check for fighting modes + if player is fighter
        if (!playerSelectedData[playerSelectedIndex].IsFighter
            && (modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled
            || enemiesEnabled))
        {
            //Debug.Log("player not fighter : " + playerSelectedData[playerSelectedIndex].PlayerObjectName);
            changeSelectedPlayerDown();
        }
        // check for shooting modes + if player is fighter
        if (!playerSelectedData[playerSelectedIndex].IsShooter
            && !modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled
            && !enemiesEnabled)
        {
            //Debug.Log("player not shooter : " + playerSelectedData[playerSelectedIndex].PlayerObjectName);
            changeSelectedPlayerDown();
        }
        GameOptions.characterObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    }

    public void changeSelectedCheerleaderUp()
    {
        // if default index (first in list
        if (cheerleaderSelectedIndex == 0)
        {
            cheerleaderSelectedIndex = cheerleaderSelectedData.Count - 1;
        }
        else
        {
            //if not first index, increment
            cheerleaderSelectedIndex--;
        }
        GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
    }

    public void changeSelectedCheerleaderDown()
    {
        // if default index (first in list
        if (cheerleaderSelectedIndex == cheerleaderSelectedData.Count - 1)
        {
            cheerleaderSelectedIndex = 0;
        }
        else
        {
            //if not first index, increment
            cheerleaderSelectedIndex++;
        }
        GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
    }

    public void changeSelectedLevelUp()
    {
        // if default index (first in list), go to end of list
        if (levelSelectedIndex == 0)
        {
            levelSelectedIndex = levelSelectedData.Count - 1;
        }
        else
        {
            // if not first index, decrement
            levelSelectedIndex--;
        }
        if ((modeSelectedData[modeSelectedIndex].IsCageMatch && !levelSelectedData[levelSelectedIndex].IsCageMatchLevel))
        {
            changeSelectedLevelUp();
        }

        // if mode is shooting, level is not
        if ((!modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled && !levelSelectedData[levelSelectedIndex].IsShootingLevel)
            // mode has enemies, level isnt a fighting level
            || (modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled && !levelSelectedData[levelSelectedIndex].IsFightingLevel)
            // battle royal mode, level isnt battle royal level
            || (modeSelectedData[modeSelectedIndex].IsBattleRoyal && !levelSelectedData[levelSelectedIndex].IsBattleRoyalLevel)
            // not battle royal mode, level is battle royal
            || (!modeSelectedData[modeSelectedIndex].IsBattleRoyal && levelSelectedData[levelSelectedIndex].IsBattleRoyalLevel))
        //// mode is cage match, level is not cage match
        //|| (modeSelectedData[modeSelectedIndex].IsCageMatch && !levelSelectedData[levelSelectedIndex].IsCageMatchLevel)
        ////mode is not cage match, level is cage match
        //|| (!modeSelectedData[modeSelectedIndex].IsCageMatch && levelSelectedData[levelSelectedIndex].IsCageMatchLevel))
        {
            changeSelectedLevelUp();
        }

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        initializeLevelDisplay();
        intializeModeDisplay();
    }
    public void changeSelectedLevelDown()
    {
        // if default index (first in list
        if (levelSelectedIndex == levelSelectedData.Count - 1)
        {
            levelSelectedIndex = 0;
        }
        else
        {
            //if not first index, increment
            levelSelectedIndex++;
        }
        if ((modeSelectedData[modeSelectedIndex].IsCageMatch && !levelSelectedData[levelSelectedIndex].IsCageMatchLevel))
        {
            changeSelectedLevelDown();
        }
        // if mode is shooting, level is not
        if ((!modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled && !levelSelectedData[levelSelectedIndex].IsShootingLevel)
            // mode has enemies, level isnt a fighting level
            || (modeSelectedData[modeSelectedIndex].EnemiesOnlyEnabled && !levelSelectedData[levelSelectedIndex].IsFightingLevel)
            // battle royal mode, level isnt battle royal level
            || (modeSelectedData[modeSelectedIndex].IsBattleRoyal && !levelSelectedData[levelSelectedIndex].IsBattleRoyalLevel)
            // not battle royal mode, level is battle royal
            || (!modeSelectedData[modeSelectedIndex].IsBattleRoyal && levelSelectedData[levelSelectedIndex].IsBattleRoyalLevel))
        //// mode is cage match, level is not cage match
        //|| (modeSelectedData[modeSelectedIndex].IsCageMatch && !levelSelectedData[levelSelectedIndex].IsCageMatchLevel)
        ////mode is not cage match, level is cage match
        //|| (!modeSelectedData[modeSelectedIndex].IsCageMatch && levelSelectedData[levelSelectedIndex].IsCageMatchLevel))
        {
            changeSelectedLevelDown();
        }

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        initializeLevelDisplay();
        intializeModeDisplay();

    }
    public void changeSelectedModeUp()
    {
        // if default index (first in list), go to end of list
        if (modeSelectedIndex == 0)
        {
            modeSelectedIndex = modeSelectedData.Count - 1;
        }
        else
        {
            // if not first index, decrement
            modeSelectedIndex--;
        }
        // mode is not battle royal, level is not
        if (modeSelectedData[modeSelectedIndex].IsBattleRoyal
            && !levelSelectedData[levelSelectedIndex].IsBattleRoyalLevel)
        {
            changeSelectedLevelUp();
        }
        if ((modeSelectedData[modeSelectedIndex].IsCageMatch && !levelSelectedData[levelSelectedIndex].IsCageMatchLevel))
        {
            changeSelectedLevelUp();
        }
        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeDisplayName;
        intializeModeDisplay();
        initializeLevelDisplay();
    }

    public void changeSelectedModeDown()
    {
        // if default index (first in list
        if (modeSelectedIndex == modeSelectedData.Count - 1)
        {
            modeSelectedIndex = 0;
        }
        else
        {
            //if not first index, increment
            modeSelectedIndex++;
        }
        if (modeSelectedData[modeSelectedIndex].IsBattleRoyal
            && !levelSelectedData[levelSelectedIndex].IsBattleRoyalLevel)
        {
            changeSelectedLevelDown();
        }
        if ((modeSelectedData[modeSelectedIndex].IsCageMatch && !levelSelectedData[levelSelectedIndex].IsCageMatchLevel))
        {
            changeSelectedLevelUp();
        }
        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeDisplayName;
        intializeModeDisplay();
        initializeLevelDisplay();
    }

    // ============================  public var references  ==============================
    // dont think some of these are used, keep an eye on this on refactor
    // button names
    public static string PlayerSelectOptionButtonName => playerSelectOptionButtonName;
    public static string CheerleaderSelectOptionButtonName => cheerleaderSelectOptionButtonName;
    public static string LevelSelectOptionButtonName => levelSelectOptionButtonName;
    public static string ModeSelectOptionButtonName => modeSelectOptionButtonName;
    public static string CreditsMenuButtonName => creditsMenuButtonName;
    public static string TrafficSelectOptionName => trafficSelectOptionName;
    public static string StartButtonName => startButtonName;
    public static string StatsMenuButtonName => statsMenuButtonName;
    public static string QuitButtonName => quitButtonName;
    public static string UpdateMenuButtonName => updateMenuButtonName;
    public static string HardcoreSelectOptionName => hardcoreSelectOptionName;
    public static string AccountMenuButtonName => accountMenuButtonName;
    public static string CreditsMenuButtonName1 => creditsMenuButtonName;
    public static string SniperSelectOptionName => sniperSelectOptionName;
    public int PlayerSelectedIndex => playerSelectedIndex;
    public static string OptionsMenuButtonName => optionsMenuButtonName;
    public static string DifficultySelectButtonName => difficultySelectButtonName;
    public static string EnemySelectOptionName => enemySelectOptionName;
    public static string DifficultySelectOptionName => difficultySelectOptionName;
    public static string ObstacleSelectOptionName => obstacleSelectOptionName;
}
