
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

    private Text playerSelectUnlockText;

    [SerializeField]
    private Text cheerleaderSelectUnlockText;

    // option select buttons, this will be disabled with touch input
    Button levelSelectButton;
    Button trafficSelectButton;
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

    //traffic
    private Text trafficSelectOptionText;

    //const object names
    private const string startButtonName = "press_start";
    private const string statsMenuButtonName = "stats_menu";
    private const string quitButtonName = "quit_game";
    private const string optionsMenuButtonName = "options_menu";
    private const string updateMenuButtonName = "update_menu";

    private const string updatePointsAvailable = "update_points_available";

    // scene name
    private const string statsMenuSceneName = "level_00_stats";

    private const string playerSelectButtonName = "player_select";
    private const string playerSelectOptionButtonName = "player_selected_name";
    private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";
    private const string playerSelectUnlockObjectName = "player_selected_unlock";
    private const string playerSelectIsLockedObjectName = "player_selected_lock_texture";
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
    //private const string levelSelectImageObjectName = "level_selected_image"; // no other refences in solution

    //mode objects
    private const string modeSelectButtonName = "mode_select";
    private const string modeSelectOptionButtonName = "mode_selected_name";
    private const string modeSelectDescriptionObjectName = "mode_selected_description";

    //traffic objects
    private const string trafficSelectButtonName = "traffic_select";
    private const string trafficSelectOptionName = "traffic_select_option";

    private const string loadScreenSceneName = "level_00_loading";

    [SerializeField]
    private bool trafficEnabled;

    private int playerSelectedIndex;
    private int levelSelectedIndex;
    private int modeSelectedIndex;
    private int cheerleaderSelectedIndex;

    [SerializeField]
    GameObject playerSelectedIsLockedObject;

    [SerializeField]
    GameObject cheerleaderSelectedIsLockedObject;

    [SerializeField]
    public PlayerControls controls;

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
        // dont destroy on load / check for duplicate instance
        //destroyInstanceIfAlreadyExists();
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

        // update experience and levels
        // recommended here because experience will be gained after every game played
        StartCoroutine(UpdateLevelAndExperienceFromDatabase());
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(InitializeDisplay());
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
        if (currentHighlightedButton.Equals(playerSelectButtonName) || currentHighlightedButton.Equals(playerSelectOptionButtonName))
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
        // if cheerleader highlighted, display cheerleader
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
            //if (GameObject.Find("LoadedData") != null)
            //{
            //    Destroy(GameObject.Find("LoadedData").gameObject);
            //}
            loadScene();
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
            loadStatsMenu(statsMenuSceneName);
        }

        // ================================== navigation =====================================================================
        // up, option select
        if (controls.UINavigation.Up.triggered && !buttonPressed
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
            && !currentHighlightedButton.Equals(trafficSelectOptionName)
            && !currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
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
            && !currentHighlightedButton.Equals(trafficSelectOptionName))
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
            //Debug.Log(" change option up");
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
                    changeSelectedTrafficOption();
                    initializeTrafficOptionDisplay();
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
            //Debug.Log(" change option down");
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

        Debug.Log("updateLevelAndExperienceFromDatabase");
        foreach (CharacterProfile s in playerSelectedData)
        {
            s.Experience = DBHelper.instance.getIntValueFromTableByFieldAndCharId("CharacterProfile", "experience", s.PlayerId);
            s.Level = DBHelper.instance.getIntValueFromTableByFieldAndCharId("CharacterProfile", "level", s.PlayerId);
            //Debug.Log("level : " + s.Level + " experience : " + s.Experience);
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
            SceneManager.LoadScene(loadScreenSceneName);
        }
    }

    IEnumerator InitializeDisplay()
    {
        yield return new WaitUntil(() => dataLoaded) ;
        Debug.Log("start manager InitializeDisplay");

        // display default data
        initializeCheerleaderDisplay();
        initializePlayerDisplay();
        initializeLevelDisplay();
        intializeModeDisplay();
        initializeTrafficOptionDisplay();
        setInitialGameOptions();

    }
    // ============================  get UI buttons / text references ==============================
    private void getUiObjectReferences()
    {
        // buttons to disable for touch input
        levelSelectButton = GameObject.Find(levelSelectButtonName).GetComponent<Button>();
        trafficSelectButton = GameObject.Find(trafficSelectButtonName).GetComponent<Button>();
        playerSelectButton = GameObject.Find(playerSelectButtonName).GetComponent<Button>();
        CheerleaderSelectButton = GameObject.Find(cheerleaderSelectButtonName).GetComponent<Button>();
        modeSelectButton = GameObject.Find(modeSelectButtonName).GetComponent<Button>();

        // player object with lock texture and unlock text
        playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
        playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        playerSelectOptionStatsText = GameObject.Find(playerSelectStatsObjectName).GetComponent<Text>();
        playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();
        playerSelectUnlockText = GameObject.Find(playerSelectUnlockObjectName).GetComponent<Text>();
        playerSelectCategoryStatsText = GameObject.Find(playerSelectStatsCategoryName).GetComponent<Text>();
        playerProgressionStatsText = GameObject.Find(playerProgressionStatsName).GetComponent<Text>();
        playerProgressionCategoryText = GameObject.Find(playerProgressionName).GetComponent<Text>();
        playerProgressionUpdatePointsText = GameObject.Find(updatePointsAvailable).GetComponent<Text>();

        // friend object with lock texture and unlock text
        cheerleaderSelectedIsLockedObject = GameObject.Find(cheerleaderSelectIsLockedObjectName);
        cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
        cheerleaderSelectOptionImage = GameObject.Find(cheerleaderSelectImageObjectName).GetComponent<Image>();
        cheerleaderSelectUnlockText = GameObject.Find(cheerleaderSelectUnlockObjectName).GetComponent<Text>();

        // traffic option selection text
        trafficSelectOptionText = GameObject.Find(trafficSelectOptionName).GetComponent<Text>();
    }

    private void setInitialGameOptions()
    {
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeObjectName;
        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;

        GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;
        GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
    }

    public String getRandomWizardOfBoat()
    {

        Random random = new Random();
        int randNum = random.Next(1, 3);
        Debug.Log("*************************************** rand num value : " + randNum);

        if(randNum == 1)
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

    public void changeSelectedTrafficOption()
    {
        trafficEnabled = !trafficEnabled;
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
            playerSelectedIsLockedObject.SetActive(false);
            playerSelectCategoryStatsText.enabled = false;

            playerProgressionCategoryText.enabled = false;
            playerProgressionStatsText.enabled = false;

            playerSelectedIsLockedObject.SetActive(false);

            // check if players is locked

            //foreach (CheerleaderProfile cl in cheerleaderSelectedData)
            //{
            //    if (AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cl.CheerleaderId) != null)
            //    {
            //        Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cl.CheerleaderId);
            //        cl.IsLocked = tempAchieve.IsLocked;
            //        cl.UnlockCharacterText = tempAchieve.AchievementDescription;
            //    }
            //    // none selected
            //    if (cl.CheerleaderId == 0)
            //    {
            //        cl.IsLocked = false;
            //        cl.UnlockCharacterText = "";
            //    }
            //}

            if (cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
            {
                // get achievement progress for display
                Achievement tempAchieve = 
                    AchievementManager.instance.AchievementList
                    .Find(x => x.CheerleaderId == cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderId);

                // disable text and unlock text
                cheerleaderSelectedIsLockedObject.SetActive(true);
                cheerleaderSelectUnlockText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].UnlockCharacterText
                    + "\nprogress " + tempAchieve.ActivationValueProgressionInt
                    + " / " + tempAchieve.ActivationValueInt;
            }
            // if player is locked or free play mode selected
            if (!cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked
                || modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
            {
                //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
                cheerleaderSelectedIsLockedObject.SetActive(false);
                cheerleaderSelectUnlockText.text = "";
            }

            cheerleaderSelectOptionText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
            cheerleaderSelectOptionImage.sprite = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderPortrait;


            cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
            cheerleaderSelectOptionText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
            GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
        }
        catch
        {
            return;
        }
    }

    public void intializeModeDisplay()
    {
        modeSelectOptionText = GameObject.Find(modeSelectOptionButtonName).GetComponent<Text>();
        modeSelectOptionText.text = modeSelectedData[modeSelectedIndex].ModelDisplayName;

        ModeSelectOptionDescriptionText = GameObject.Find(modeSelectDescriptionObjectName).GetComponent<Text>();
        ModeSelectOptionDescriptionText.text = modeSelectedData[modeSelectedIndex].ModeDescription;
    }

    public void initializePlayerDisplay()
    {
        try
        {
            cheerleaderSelectOptionImage.enabled = false;
            cheerleaderSelectedIsLockedObject.SetActive(false);

            playerSelectOptionImage.enabled = true;
            playerSelectOptionStatsText.enabled = true;
            playerSelectedIsLockedObject.SetActive(true);
            playerSelectCategoryStatsText.enabled = true;

            playerProgressionCategoryText.enabled = true;
            playerProgressionStatsText.enabled = true;

            // check if players is locked
            //foreach (CharacterProfile sp in playerSelectedData)
            //{
            //    if (AchievementManager.instance.AchievementList.Find(x => x.PlayerId == sp.PlayerId) != null)
            //    {
            //        Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.PlayerId == sp.PlayerId);
            //        sp.IsLocked = tempAchieve.IsLocked;
            //        sp.UnlockCharacterText = tempAchieve.AchievementDescription;
            //        //Debug.Log(tempAchieve.achievementName + " islocked : " + tempAchieve.IsLocked);
            //    }
            //}

            if (playerSelectedData[playerSelectedIndex].IsLocked)
            {
                // get player achievement status
                Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.PlayerId == playerSelectedData[playerSelectedIndex].PlayerId);
                playerSelectedIsLockedObject.SetActive(true);
                
                // disable text and unlock text
                //cheerleaderSelectedIsLockedObject.SetActive(true);
                if (tempAchieve.IsProgressiveCount)
                {
                    //Debug.Log("========================================================== temp.progress : " + tempAchieve.ActivationValueProgressionInt);

                    playerSelectUnlockText.text = playerSelectedData[playerSelectedIndex].UnlockCharacterText
                        + "\nprogress " + tempAchieve.ActivationValueProgressionInt
                        + " / " + tempAchieve.ActivationValueInt;
                }
                else
                {
                    playerSelectUnlockText.text = playerSelectedData[playerSelectedIndex].UnlockCharacterText;
                }
            }

            // if player is locked or free play mode selected
            if (!playerSelectedData[playerSelectedIndex].IsLocked
                || modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
            {
                //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
                playerSelectedIsLockedObject.SetActive(false);
                playerSelectUnlockText.text = "";
            }

            playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
            playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;

            playerSelectOptionStatsText.text = // playerSelectedData[playerSelectedIndex].Accuracy2Pt.ToString("F0") + "\n"
                playerSelectedData[playerSelectedIndex].Accuracy3Pt.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Accuracy4Pt.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Accuracy7Pt.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].calculateSpeedToPercent().ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Range.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Release.ToString("F0") + "\n"
                //+ playerSelectedData[playerSelectedIndex].Range.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Luck.ToString("F0");

            //Debug.Log("=================================================================");
            playerSelectedData[playerSelectedIndex].Level = playerSelectedData[playerSelectedIndex].Experience / 3000;
            int nextlvl = ((playerSelectedData[playerSelectedIndex].Level + 1) * 3000) - playerSelectedData[playerSelectedIndex].Experience;
            //Debug.Log("experience : " + playerSelectedData[playerSelectedIndex].Experience);
            //Debug.Log("points available : " + playerSelectedData[playerSelectedIndex].PointsAvailable);
            //Debug.Log("level : " + playerSelectedData[playerSelectedIndex].Level);
            //Debug.Log("next level : " + nextlvl);

            playerProgressionStatsText.text = playerSelectedData[playerSelectedIndex].Level.ToString("F0") + "\n"
                + playerSelectedData[playerSelectedIndex].Experience.ToString("F0") + "\n"
                + nextlvl.ToString("F0") + "\n" ;

            // player points avaiable for upgrade
            if (playerSelectedData[playerSelectedIndex].PointsAvailable > 0)
            {
                playerProgressionUpdatePointsText.text = "+ " + playerSelectedData[playerSelectedIndex].PointsAvailable.ToString();
            }
            else
            {
                playerProgressionUpdatePointsText.text = "";
            }

            GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;

        }
        catch
        {
            return;
        }
    }

    // ============================  footer options activate - load scene/stats/quit/etc ==============================
    public void loadScene()
    {
        // tells character profile to load profile from LoadedData.instance
        GameOptions.gameModeHasBeenSelected = true;

        // update game options for game mode
        setGameOptions();

        // i create the string this way so that i can have a description of the level so i know what im opening
        string sceneName = GameOptions.levelSelected + "_" + levelSelectedData[levelSelectedIndex].LevelDescription;
        //Debug.Log("scene name : " + sceneName);

        // check if Player selected is locked
        if ((playerSelectedData[playerSelectedIndex].IsLocked || cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
            && !modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
        {
            Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
            messageText.text = " Bruh, it's locked. pick something else";
            // turn off text display after 5 seconds
            StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
        }
        if ((!playerSelectedData[playerSelectedIndex].IsLocked && !cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
            || modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
        {
            Debug.Log("load scene");
            // load player progression info
            PlayerData.instance.CurrentExperience = playerSelectedData[playerSelectedIndex].Experience;
            PlayerData.instance.CurrentLevel = playerSelectedData[playerSelectedIndex].Level;
            PlayerData.instance.UpdatePointsAvailable = playerSelectedData[playerSelectedIndex].PointsAvailable;
            PlayerData.instance.UpdatePointsAvailable = playerSelectedData[playerSelectedIndex].PointsUsed;

            SceneManager.LoadScene(sceneName);
        }
    }

    public void loadStatsMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ============================  set game options ==============================
    // this is necessary for setting Game Rules on game manager
    private void setGameOptions()
    {
        GameOptions.playerId = playerSelectedData[playerSelectedIndex].PlayerId;
        GameOptions.playerDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;

        // if Wizard of Boat selected, randomly choose which one to spawn
        if (playerSelectedData[playerSelectedIndex].PlayerDisplayName.ToLower().Contains("boat"))
        {
            GameOptions.playerObjectName = getRandomWizardOfBoat();
        }
        else
        {
            GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        }

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        GameOptions.levelId = levelSelectedData[levelSelectedIndex].LevelId;
        GameOptions.levelDisplayName = levelSelectedData[levelSelectedIndex].LevelDisplayName;

        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;

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
            Debug.Log("modeSelectedData[modeSelectedIndex].CustomTimer : " + modeSelectedData[modeSelectedIndex].CustomTimer);
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

        GameOptions.trafficEnabled = trafficEnabled;

        GameOptions.applicationVersion = Application.version;
        GameOptions.operatingSystemVersion = SystemInfo.operatingSystem;

        // send current selected options to game options for next load on start manager
        GameOptions.playerSelectedIndex = playerSelectedIndex;
        GameOptions.cheerleaderSelectedIndex = cheerleaderSelectedIndex;
        GameOptions.levelSelectedIndex = levelSelectedIndex;
        GameOptions.modeSelectedIndex = modeSelectedIndex;
        GameOptions.trafficEnabled = trafficEnabled;
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
        //Debug.Log("changeSelectedPlayerUp");
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
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        //Debug.Log("player selected : " + GameOptions.playerObjectName);
    }
    public void changeSelectedPlayerDown()
    {
        //Debug.Log("changeSelectedPlayerDown");
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
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
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
        //Debug.Log("player selected : " + GameOptions.playerSelected);
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
        //Debug.Log("player selected : " + GameOptions.playerSelected);
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
        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
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
        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
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
        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;
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

        GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;
    }

    // ============================  public var references  ==============================
    // dont think some of these are used, keep an eye on this on refactor
    public static string PlayerSelectOptionButtonName => playerSelectOptionButtonName;

    public static string CheerleaderSelectOptionButtonName => cheerleaderSelectOptionButtonName;

    public static string LevelSelectOptionButtonName => levelSelectOptionButtonName;

    public static string ModeSelectOptionButtonName => modeSelectOptionButtonName;

    public static string TrafficSelectOptionName => trafficSelectOptionName;

    public static string StartButtonName => startButtonName;

    public static string StatsMenuButtonName => statsMenuButtonName;

    public static string QuitButtonName => quitButtonName;

    public static string StatsMenuSceneName => statsMenuSceneName;

    public Button LevelSelectButton { get => levelSelectButton; set => levelSelectButton = value; }
    public Button TrafficSelectButton { get => trafficSelectButton; set => trafficSelectButton = value; }
    public Button PlayerSelectButton1 { get => playerSelectButton; set => playerSelectButton = value; }
    public Button CheerleaderSelectButton1 { get => CheerleaderSelectButton; set => CheerleaderSelectButton = value; }
    public Button ModeSelectButton { get => modeSelectButton; set => modeSelectButton = value; }
    public List<CharacterProfile> PlayerSelectedData { get => playerSelectedData; set => playerSelectedData = value; }
}
