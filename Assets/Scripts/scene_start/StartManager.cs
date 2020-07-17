using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamUtility.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    [SerializeField]
    private string currentHighlightedButton;

    //list of all shooter profiles with player data
    [SerializeField]
    private List<ShooterProfile> playerSelectedData;

    // list off cheerleader profile data
    [SerializeField]
    private List<StartScreenCheerleaderSelected> cheerleaderSelectedData;

    // list off level  data
    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;

    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;

    private Text playerSelectUnlockText;

    [SerializeField]
    private Text cheerleaderSelectUnlockText;

    //player selected display
    private Text playerSelectOptionText;
    private Image playerSelectOptionImage;
    private Text playerSelectOptionStatsText;
    private Text playerSelectCategoryStatsText;

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
    private const string quitButtonName = "quit_button";

    // scene name
    private const string statsMenuSceneName = "level_00_stats";

    private const string playerSelectButtonName = "player_select";
    private const string playerSelectOptionButtonName = "player_selected_name";
    private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";
    private const string playerSelectUnlockObjectName = "player_selected_unlock";
    private const string playerSelectIsLockedObjectName = "player_selected_lock_texture";
    private const string playerSelectStatsCategoryName = "player_selected_stats_category";

    //cheerleader objects
    private const string cheerleaderSelectButtonName = "cheerleader_select";
    private const string cheerleaderSelectOptionButtonName = "cheerleader_selected_name";
    private const string cheerleaderSelectImageObjectName = "cheerleader_selected_image";
    private const string cheerleaderSelectUnlockObjectName = "cheerleader_selected_unlock";
    private const string cheerleaderSelectIsLockedObjectName = "cheerleader_selected_lock_texture";

    //level objects
    private const string levelSelectButtonName = "level_select";
    private const string levelSelectOptionButtonName = "level_selected_name";
    private const string levelSelectImageObjectName = "level_selected_image";

    //mode objects
    private const string modeSelectButtonName = "mode_select";
    private const string modeSelectOptionButtonName = "mode_selected_name";
    private const string modeSelectDescriptionObjectName = "mode_selected_description";

    //traffic objects
    private const string trafficSelectButtonName = "traffic_select";
    private const string trafficSelectOptionName = "traffic_select_option";
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

    //private Text gameModeSelectText;
    void Awake()
    {
        // object with lock texture and unlock text
        playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
        playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        playerSelectOptionStatsText = GameObject.Find(playerSelectStatsObjectName).GetComponent<Text>();
        playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();
        playerSelectUnlockText = GameObject.Find(playerSelectUnlockObjectName).GetComponent<Text>();
        playerSelectCategoryStatsText = GameObject.Find(playerSelectStatsCategoryName).GetComponent<Text>();

        // object with lock texture and unlock text
        cheerleaderSelectedIsLockedObject = GameObject.Find(cheerleaderSelectIsLockedObjectName);
        cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
        cheerleaderSelectOptionImage = GameObject.Find(cheerleaderSelectImageObjectName).GetComponent<Image>();
        cheerleaderSelectUnlockText = GameObject.Find(cheerleaderSelectUnlockObjectName).GetComponent<Text>();

        trafficSelectOptionText = GameObject.Find(trafficSelectOptionName).GetComponent<Text>();

        //default index for player selected
        playerSelectedIndex = 0;
        cheerleaderSelectedIndex = 0;
        levelSelectedIndex = 0;
        modeSelectedIndex = 0;

        // load default data
        loadPlayerSelectDataList();
        loadCheerleaderSelectDataList();
        initializeTrafficOptionDisplay();
        loadLevelSelectDataList();
        loadModeSelectDataList();

    }

    // Start is called before the first frame update
    void Start()
    {
        // display default data
        initializeCheerleaderDisplay();
        initializePlayerDisplay();
        initializeLevelDisplay();
        intializeModeDisplay();
        setInitialGameOptions();
    }

    private void setInitialGameOptions()
    {
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeObjectName;
        GameOptions.gameModeSelected = modeSelectedData[modeSelectedIndex].ModeId;

        GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;
        GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
    }

    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }

        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";

        // if player highlighted, display player
        if (currentHighlightedButton.Equals(playerSelectButtonName))
        {
            initializePlayerDisplay();
        }
        // if cheerleader highlighted, display cheerleader
        if (currentHighlightedButton.Equals(cheerleaderSelectButtonName))
        {
            initializeCheerleaderDisplay();
        }
        // start button | start game
        if ((InputManager.GetKeyDown(KeyCode.Return)
             || InputManager.GetKeyDown(KeyCode.Space)
             || InputManager.GetButtonDown("Fire1"))
            && currentHighlightedButton.Equals(startButtonName))
        {
            loadScene();
        }
        // quit button | quit game
        if ((InputManager.GetKeyDown(KeyCode.Return)
             || InputManager.GetKeyDown(KeyCode.Space)
             || InputManager.GetButtonDown("Fire1"))
            && currentHighlightedButton.Equals(quitButtonName))
        {
            Application.Quit();
        }
        // stats menu button | load stats menu
        if ((InputManager.GetKeyDown(KeyCode.Return)
             || InputManager.GetKeyDown(KeyCode.Space)
             || InputManager.GetButtonDown("Fire1"))
            && currentHighlightedButton.Equals(statsMenuButtonName))
        {
            loadStatsMenu(statsMenuSceneName);
        }

        // ================================== navigation =====================================================================

        // up arrow navigation
        if (InputManager.GetKeyDown(KeyCode.UpArrow)
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
            && !currentHighlightedButton.Equals(trafficSelectOptionName))
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnUp().gameObject);
        }

        // down arrow navigation
        if (InputManager.GetKeyDown(KeyCode.DownArrow)
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
            && !currentHighlightedButton.Equals(trafficSelectOptionName))
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnDown().gameObject);
        }

        // right arrow on player select
        if (InputManager.GetKeyDown(KeyCode.RightArrow))
        {
            //Debug.Log("right : player select");
            if (currentHighlightedButton.Equals(playerSelectButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
            //Debug.Log("right : level select");
            if (currentHighlightedButton.Equals(levelSelectButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
            if (currentHighlightedButton.Equals(cheerleaderSelectButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
            if (currentHighlightedButton.Equals(trafficSelectButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
        }

        // left arrow navigation on player options
        if (InputManager.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("left : player select");
            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
            //Debug.Log("left : level select");
            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
            if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
            if (currentHighlightedButton.Equals(trafficSelectOptionName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
        }

        // up/down arrow on player options
        if ((InputManager.GetKeyDown(KeyCode.W) || InputManager.GetKeyDown(KeyCode.UpArrow)))
        {
            //Debug.Log("up : player option");
            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                changeSelectedPlayerUp();
                initializePlayerDisplay();
            }
            //Debug.Log("up : level option");
            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
            {
                changeSelectedLevelUp();
                initializeLevelDisplay();
            }
            //Debug.Log("up : level option");
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

        if ((InputManager.GetKeyDown(KeyCode.S) || InputManager.GetKeyDown(KeyCode.DownArrow)))
        {
            //Debug.Log("down : player option");
            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                changeSelectedPlayerDown();
                initializePlayerDisplay();
            }
            //Debug.Log("down : level option");
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
    }

    private void initializeTrafficOptionDisplay()
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

    private void changeSelectedTrafficOption()
    {
        trafficEnabled = !trafficEnabled;
    }

    private void initializeLevelDisplay()
    {
        levelSelectOptionText = GameObject.Find(levelSelectOptionButtonName).GetComponent<Text>();
        levelSelectOptionText.text = levelSelectedData[levelSelectedIndex].LevelDisplayName;
        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    }

    private void initializeCheerleaderDisplay()
    {
        cheerleaderSelectOptionImage.enabled = true;
        playerSelectOptionImage.enabled = false;
        playerSelectOptionStatsText.enabled = false;
        playerSelectedIsLockedObject.SetActive(false);
        playerSelectCategoryStatsText.enabled = false;
        playerSelectedIsLockedObject.SetActive(false);

        // check if players is locked
        foreach (StartScreenCheerleaderSelected cl in cheerleaderSelectedData)
        {
            if (AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cl.CheerleaderId) != null)
            {
                Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cl.CheerleaderId);
                cl.IsLocked = tempAchieve.IsLocked;
                cl.UnlockCharacterText = tempAchieve.AchievementDescription;
            }
            // none selected
            if (cl.CheerleaderId == 0)
            {
                cl.IsLocked = false;
                cl.UnlockCharacterText = "";
            }
        }

        if (cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
        {
            Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderId);
            // disable text and unlock text
            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
            cheerleaderSelectedIsLockedObject.SetActive(true);
            cheerleaderSelectUnlockText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].UnlockCharacterText
                + "\nprogress " + tempAchieve.ActivationValueProgressionInt
                + " / " + tempAchieve.ActivationValueInt;
        }
        else
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

    private void intializeModeDisplay()
    {
        modeSelectOptionText = GameObject.Find(modeSelectOptionButtonName).GetComponent<Text>();
        modeSelectOptionText.text = modeSelectedData[modeSelectedIndex].ModelDisplayName;

        ModeSelectOptionDescriptionText = GameObject.Find(modeSelectDescriptionObjectName).GetComponent<Text>();
        ModeSelectOptionDescriptionText.text = modeSelectedData[modeSelectedIndex].ModeDescription;
    }

    private void initializePlayerDisplay()
    {
        cheerleaderSelectOptionImage.enabled = false;
        cheerleaderSelectedIsLockedObject.SetActive(false);

        playerSelectOptionImage.enabled = true;
        playerSelectOptionStatsText.enabled = true;
        playerSelectedIsLockedObject.SetActive(true);
        playerSelectCategoryStatsText.enabled = true;

        // check if players is locked
        foreach (ShooterProfile sp in playerSelectedData)
        {
            if (AchievementManager.instance.AchievementList.Find(x => x.PlayerId == sp.PlayerId) != null)
            {
                Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.PlayerId == sp.PlayerId);
                sp.IsLocked = tempAchieve.IsLocked;
                sp.UnlockCharacterText = tempAchieve.AchievementDescription;
            }
        }

        if (playerSelectedData[playerSelectedIndex].IsLocked)
        {
            // find achievement that unlocks player
            //Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.PlayerId == playerSelectedData[playerSelectedIndex].PlayerId);
            // disable text and unlock text
            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
            playerSelectedIsLockedObject.SetActive(true);
            playerSelectUnlockText.text = playerSelectedData[playerSelectedIndex].UnlockCharacterText;
            // find achievement by player id
            // + "\nprogress : " + tempAchieve.ActivationValueProgressionInt;
        }
        else
        {
            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
            playerSelectedIsLockedObject.SetActive(false);
            playerSelectUnlockText.text = "";
        }


        playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
        playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;

        playerSelectOptionStatsText.text = playerSelectedData[playerSelectedIndex].Accuracy2Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].Accuracy3Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].Accuracy4Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].Accuracy7Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].calculateSpeedToPercent().ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0") + "\n"
            //+ playerSelectedData[playerSelectedIndex].Range.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].CriticalPercent.ToString("F0");

        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    }

    private void loadPlayerSelectDataList()
    {
        string path = "Prefabs/start_menu/player_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            ShooterProfile temp = obj.GetComponent<ShooterProfile>();
            //Debug.Log(" temp : " + temp.PlayerDisplayName);
            playerSelectedData.Add(temp);
        }
        // sort list by  character id
        playerSelectedData.Sort(sortByPlayerId);
    }

    private void loadCheerleaderSelectDataList()
    {

        string path = "Prefabs/start_menu/cheerleader_selected_object";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        //Debug.Log("objects : " + objects.Length);
        foreach (GameObject obj in objects)
        {
            StartScreenCheerleaderSelected temp = obj.GetComponentInChildren<StartScreenCheerleaderSelected>();
            //Debug.Log(" temp : " + temp.UnlockCharacterText);
            cheerleaderSelectedData.Add(temp);
        }
        // sort list by  character id
        cheerleaderSelectedData.Sort(sortByCheerleaderId);
    }

    static int sortByPlayerId(ShooterProfile p1, ShooterProfile p2)
    {
        return p1.PlayerId.CompareTo(p2.PlayerId);
    }

    static int sortByCheerleaderId(StartScreenCheerleaderSelected p1, StartScreenCheerleaderSelected p2)
    {
        return p1.CheerleaderId.CompareTo(p2.CheerleaderId);
    }

    static int sortByLevelId(StartScreenLevelSelected l1, StartScreenLevelSelected l2)
    {
        return l1.LevelId.CompareTo(l2.LevelId);
    }

    static int sortByModeId(StartScreenModeSelected m1, StartScreenModeSelected m2)
    {
        return m1.ModeId.CompareTo(m2.ModeId);
    }

    private void loadLevelSelectDataList()
    {
        //Debug.Log("loadPlayerSelectDataList()");

        string path = "Prefabs/start_menu/level_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            StartScreenLevelSelected temp = obj.GetComponent<StartScreenLevelSelected>();
            levelSelectedData.Add(temp);
        }
        // sort list by  level id
        levelSelectedData.Sort(sortByLevelId);
    }

    private void loadModeSelectDataList()
    {
        //Debug.Log("loadModeSelectDataList()");

        string path = "Prefabs/start_menu/mode_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            StartScreenModeSelected temp = obj.GetComponent<StartScreenModeSelected>();
            modeSelectedData.Add(temp);
        }
        // sort list by  mode id
        modeSelectedData.Sort(sortByModeId);

        foreach (StartScreenModeSelected s in modeSelectedData)
        {
            Debug.Log(" mode id : " + s.ModeId);
        }
    }

    private void changeSelectedPlayerUp()
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
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        //Debug.Log("player selected : " + GameOptions.playerSelected);
    }
    private void changeSelectedPlayerDown()
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
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        //Debug.Log("player selected : " + GameOptions.playerSelected);
    }

    private void changeSelectedCheerleaderUp()
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

    private void changeSelectedCheerleaderDown()
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

    private void changeSelectedLevelUp()
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
    private void changeSelectedLevelDown()
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
    private void changeSelectedModeUp()
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
        GameOptions.gameModeSelected = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;
    }

    private void changeSelectedModeDown()
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

        GameOptions.gameModeSelected = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;
    }


    public void loadScene()
    {
        // for testing sceneswihtout loading from start screen
        GameOptions.gameModeHasBeenSelected = true;

        // apply selected player stats to game options, which will be loaded into Player on spawn
        setPlayerProfileStats();

        // update game options for game mode
        setGameOptions();

        // i create the string this way so that i can have a description of the level so i know what im opening
        string sceneName = GameOptions.levelSelected + "_" + levelSelectedData[levelSelectedIndex].LevelDescription;
        //Debug.Log("scene name : " + sceneName);

        // check if Player selected is locked
        if (playerSelectedData[playerSelectedIndex].IsLocked || cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
        {
            Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
            messageText.text = " Bruh, it's locked. pick something else";
            // turn off text display after 5 seconds
            StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
        }
        if (!playerSelectedData[playerSelectedIndex].IsLocked && !cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
        {
            // load highscores before loading scene
            PlayerData.instance.loadStatsFromDatabase();
            SceneManager.LoadScene(sceneName);
        }
    }

    private void setPlayerProfileStats()
    {
        StartScreenModeSelected temp = modeSelectedData[modeSelectedIndex];
        // need object name and playerid
        GameOptions.playerDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        GameOptions.playerId = playerSelectedData[playerSelectedIndex].PlayerId;

        GameOptions.accuracy2pt = playerSelectedData[playerSelectedIndex].Accuracy2Pt;
        GameOptions.accuracy3pt = playerSelectedData[playerSelectedIndex].Accuracy3Pt;
        GameOptions.accuracy4pt = playerSelectedData[playerSelectedIndex].Accuracy4Pt;
        GameOptions.accuracy7pt = playerSelectedData[playerSelectedIndex].Accuracy7Pt;

        // if 3/4/All point contest, disable Luck/citical %
        if (temp.GameModeThreePointContest
            || temp.GameModeFourPointContest
            || temp.GameModeAllPointContest)
        {
            GameOptions.criticalPercent = 0;
        }
        else
        {
            GameOptions.criticalPercent = playerSelectedData[playerSelectedIndex].CriticalPercent;
        }
        GameOptions.jumpForce = playerSelectedData[playerSelectedIndex].JumpForce;
        GameOptions.speed = playerSelectedData[playerSelectedIndex].Speed;
        GameOptions.runSpeed = playerSelectedData[playerSelectedIndex].RunSpeed;
        GameOptions.runSpeedHasBall = playerSelectedData[playerSelectedIndex].RunSpeedHasBall;
        GameOptions.shootAngle = playerSelectedData[playerSelectedIndex].ShootAngle;
    }

    private void setGameOptions()
    {
        GameOptions.playerId = playerSelectedData[playerSelectedIndex].PlayerId;
        GameOptions.playerDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;

        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
        GameOptions.levelId = levelSelectedData[levelSelectedIndex].LevelId;
        GameOptions.levelDisplayName = levelSelectedData[levelSelectedIndex].LevelDisplayName;


        GameOptions.gameModeSelected = modeSelectedData[modeSelectedIndex].ModeId;
        GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;

        GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
        GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;

        GameOptions.gameModeRequiresShotMarkers3s = modeSelectedData[modeSelectedIndex].ModeRequiresShotMarkers3S;
        GameOptions.gameModeRequiresShotMarkers4s = modeSelectedData[modeSelectedIndex].ModeRequiresShotMarkers4S;

        GameOptions.gameModeThreePointContest = modeSelectedData[modeSelectedIndex].GameModeThreePointContest;
        GameOptions.gameModeFourPointContest = modeSelectedData[modeSelectedIndex].GameModeFourPointContest;
        GameOptions.gameModeAllPointContest = modeSelectedData[modeSelectedIndex].GameModeAllPointContest;

        if (modeSelectedData[modeSelectedIndex].CustomTimer > 0)
        {
            Debug.Log("modeSelectedData[modeSelectedIndex].CustomTimer : " + modeSelectedData[modeSelectedIndex].CustomTimer);
            GameOptions.customTimer = modeSelectedData[modeSelectedIndex].CustomTimer;
        }


        GameOptions.gameModeRequiresMoneyBall = modeSelectedData[modeSelectedIndex].ModeRequiresMoneyBall;
        GameOptions.gameModeRequiresConsecutiveShot = modeSelectedData[modeSelectedIndex].ModeRequiresConsecutiveShots;

        GameOptions.cheerleaderDisplayName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
        GameOptions.cheerleaderId = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderId;
        GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;

        GameOptions.trafficEnabled = trafficEnabled;


        GameOptions.applicationVersion = Application.version;
        GameOptions.operatingSystemVersion = SystemInfo.operatingSystem;
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

    private void loadStatsMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
