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
    //private GameObject gameModeNameObject;
    //private Vector3 gameModeNameSpawnPoint;

    //[SerializeField]
    //private GameObject[] gameModeNamesList;

    //private GameObject gameModeDescriptionsObject;
    //private Vector3 gameModeDescriptionSpawnPoint;

    [SerializeField]
    private string currentHighlightedButton;

    //list of all shooter profiles with player data
    [SerializeField]
    private List<ShooterProfile> playerSelectedData;

    //list of all scenes in build
    //private List<string> scenes;

    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;

    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;

    //player selected display
    private Text playerSelectOptionText;
    private Image playerSelectOptionImage;
    private Text playerSelectOptionStatsText;
    private Text levelSelectOptionText;

    [SerializeField]
    private Text playerSelectUnlockText;

    //player selected display
    private Text modeSelectOptionText;
    private Text ModeSelectOptionDescriptionText;

    //player objects
    private const string startButtonName = "press_start";
    private const string playerSelectButtonName = "player_select";
    private const string playerSelectOptionButtonName = "player_selected_name";
    private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";
    private const string playerSelectUnlockObjectName = "player_selected_unlock";
    private const string playerSelectIsLockedObjectName = "player_selected_lock_texture";

    [SerializeField]
    GameObject playerSelectedIsLockedObject;

    //level objects
    private const string levelSelectButtonName = "level_select";
    private const string levelSelectOptionButtonName = "level_selected_name";
    private const string levelSelectImageObjectName = "level_selected_image";

    //mode objects
    private const string modeSelectButtonName = "mode_select";
    private const string modeSelectOptionButtonName = "mode_selected_name";
    private const string modeSelectDescriptionObjectName = "mode_selected_description";

    private int playerSelectedIndex;
    private int levelSelectedIndex;
    private int modeSelectedIndex;


    //private Text gameModeSelectText;
    void Awake()
    {
        // object with lock texture and unlock text
        playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
        playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        playerSelectOptionStatsText = GameObject.Find(playerSelectStatsObjectName).GetComponent<Text>();
        playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();
        playerSelectUnlockText = GameObject.Find(playerSelectUnlockObjectName).GetComponent<Text>();

        //default index for player selected
        playerSelectedIndex = 0;
        levelSelectedIndex = 0;
        modeSelectedIndex = 0;

        loadPlayerSelectDataList();
        loadLevelSelectDataList();
        loadModeSelectDataList();

    }

    // Start is called before the first frame update
    void Start()
    {
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
            Debug.Log("if (EventSystem.current.currentSelectedGameObject == null) : ");
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }

        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";

        ////Debug.Log("current : "+ currentHighlightedButton);

        // start game
        if ((InputManager.GetKeyDown(KeyCode.Return) 
             || InputManager.GetKeyDown(KeyCode.Space)
             || InputManager.GetButtonDown("Fire1"))
            && currentHighlightedButton.Equals(startButtonName))
        {
            //Debug.Log("pressed enter");
            loadScene();
        }
        // up arrow navigation
        if (InputManager.GetKeyDown(KeyCode.UpArrow)
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName))
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnUp().gameObject);
        }

        // down arrow navigation
        if (InputManager.GetKeyDown(KeyCode.DownArrow)
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
            && !currentHighlightedButton.Equals(modeSelectOptionButtonName))
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
        }

        if ((InputManager.GetKeyDown(KeyCode.S) || InputManager.GetKeyDown(KeyCode.DownArrow )))
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
        }
    }

    private void initializeLevelDisplay()
    {
        levelSelectOptionText = GameObject.Find(levelSelectOptionButtonName).GetComponent<Text>();
        levelSelectOptionText.text = levelSelectedData[levelSelectedIndex].LevelDisplayName;
        GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
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
        Debug.Log("initializePlayerDisplay()");

        if (playerSelectedData[playerSelectedIndex].IsLocked)
        {
            // disable text and unlock text
            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
            playerSelectedIsLockedObject.SetActive(true);
            playerSelectUnlockText.text = playerSelectedData[playerSelectedIndex].UnlockCharacterText;
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
            + playerSelectedData[playerSelectedIndex].Range.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].CriticalPercent.ToString("F0");

        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    }

    private void loadPlayerSelectDataList()
    {

        string path = "Prefabs/start_menu/player_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        Debug.Log("objects : " + objects.Length);
        foreach (GameObject obj in objects)
        {
            ShooterProfile temp = obj.GetComponent<ShooterProfile>();
            //Debug.Log(" temp : " + temp.PlayerDisplayName);
            playerSelectedData.Add(temp);
        }
        // sort list by  character id
        playerSelectedData.Sort(sortByPlayerId);
    }

    static int sortByPlayerId(ShooterProfile p1, ShooterProfile p2)
    {
        return p1.PlayerId.CompareTo(p2.PlayerId);
    }

    static int sortByLevelId(StartScreenLevelSelected l1, StartScreenLevelSelected l2)
    {
        return l1.LevelId.CompareTo(l2.LevelId);
    }

    static int sortByModeId(StartScreenModeSelected m1, StartScreenModeSelected m2)
    {
        return m2.ModeId.CompareTo(m2.ModeId);
    }

    private void loadLevelSelectDataList()
    {
        Debug.Log("loadPlayerSelectDataList()");

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
        if (playerSelectedData[playerSelectedIndex].IsLocked)
        {
            Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
            messageText.text = " Bruh, it's locked";
            // turn off text display after 5 seconds
            StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
        }
        if (!playerSelectedData[playerSelectedIndex].IsLocked)
        {
            SceneManager.LoadScene(sceneName);
        }
    }

    private void setPlayerProfileStats()
    {
        // need object name and playerid
        GameOptions.playerDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        GameOptions.playerId = playerSelectedData[playerSelectedIndex].PlayerId;

        GameOptions.accuracy2pt = playerSelectedData[playerSelectedIndex].Accuracy2Pt;
        GameOptions.accuracy3pt = playerSelectedData[playerSelectedIndex].Accuracy3Pt;
        GameOptions.accuracy4pt = playerSelectedData[playerSelectedIndex].Accuracy4Pt;
        GameOptions.accuracy7pt = playerSelectedData[playerSelectedIndex].Accuracy7Pt;

        GameOptions.criticalPercent = playerSelectedData[playerSelectedIndex].CriticalPercent;
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

        GameOptions.gameModeRequiresMoneyBall = modeSelectedData[modeSelectedIndex].ModeRequiresMoneyBall;

        GameOptions.applicationVersion = Application.version;
        GameOptions.operatingSystemVersion = SystemInfo.operatingSystem;
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }
}
