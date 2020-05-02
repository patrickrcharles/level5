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

    [SerializeField]
    private List<shooterProfile> playerSelectedData;

    //list of all scenes in build
    [SerializeField]
    private List<string> scenes;

    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;

    //player selected display
    private Text playerSelectOptionText;
    private Image playerSelectOptionImage;
    private Text playerSelectOptionStatsText;

    private Text levelSelectOptionText;
    //private Image levelSelectOptionImage;

    //player objects
    private const string startButtonName = "press_start";
    private const string playerSelectButtonName = "player_select";
    [SerializeField]
    private const string playerSelectOptionButtonName = "player_selected_name";
    private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";

    //level objects
    private const string levelSelectButtonName = "level_select";
    private const string levelSelectOptionButtonName = "level_selected_name";
    private const string levelSelectImageObectName = "level_selected_image";

    private int playerSelectedIndex;
    private int levelselectedIndex;


    //private Text gameModeSelectText;
    void Awake()
    {
        //default index for player selected
        playerSelectedIndex = 0;
        levelselectedIndex = 0;
        loadPlayerSelectDataList();
        loadLevelSelectDataList();

    }

    // Start is called before the first frame update
    void Start()
    {
        initializePlayerDisplay();
        initializeLevelDisplay();

        GameOptions.playerSelected = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        GameOptions.levelSelected = levelSelectOptionText.text;

        scenes = new List<string>();
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
                scenes.Add(scene.path);
        }

        //string gamesModeNamesPath = "Prefabs/start_menu/mode_names";
        //gameModeNamesList =Resources.LoadAll<GameObject>(gamesModeNamesPath);

        ////temp = GameObject.Find("mode_5_description");
        //gameModeDescriptionsObject = GameObject.Find("game_mode_descriptions");
        //gameModeDescriptionSpawnPoint = gameModeDescriptionsObject.transform.position;
        //gameModeNameObject = GameObject.Find("game_mode_name");        //gameModeNamesList =Resources.LoadAll<GameObject>(gamesModeNamesPath);

        ////temp = GameObject.Find("mode_5_description");
        //gameModeDescriptionsObject = GameObject.Find("game_mode_descriptions");
        //gameModeDescriptionSpawnPoint = gameModeDescriptionsObject.transform.position;
        //gameModeNameObject = GameObject.Find("game_mode_name");
    }

    // Update is called once per frame
    void Update()
    {
        //// detect if highlighted object changes
        //if (EventSystem.current.currentSelectedGameObject.name != currentHighlightedButton)
        //{
        //    if (cloneDescription != null)
        //    {
        //        getNextModeDecription();
        //    }
        //    else
        //    {
        //        createFirstModeDescription();
        //    }
        //}

        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
        //Debug.Log("current : "+ currentHighlightedButton);

        // start game
        if ((InputManager.GetKeyDown(KeyCode.Return) || InputManager.GetKeyDown(KeyCode.Space))
            && currentHighlightedButton.Equals(startButtonName))
        {
            Debug.Log("pressed enter");
            loadScene();
        }
        // up arrow navigation
        if (InputManager.GetKeyDown(KeyCode.UpArrow)
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName))
        {
            Debug.Log("up : Arrow !player/level Options");
            if (!currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnUp().gameObject);
            }
        }

        // down arrow navigation
        if (InputManager.GetKeyDown(KeyCode.DownArrow)
            && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
            && !currentHighlightedButton.Equals(levelSelectOptionButtonName))
        {

            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnDown().gameObject);

        }


        // right arrow on player select
        if (InputManager.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("right : player select");
            if (currentHighlightedButton.Equals(playerSelectButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
            Debug.Log("right : level select");
            if (currentHighlightedButton.Equals(levelSelectButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
        }

        // left arrow navigation on player options
        if (InputManager.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("left : player select");
            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
            Debug.Log("left : level select");
            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
        }

        // up/down arrow on player options
        if ((InputManager.GetKeyDown(KeyCode.W) || InputManager.GetKeyDown(KeyCode.UpArrow)))
        {
            Debug.Log("up : player option");
            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                changeSelectedPlayerUp();
                initializePlayerDisplay();
            }
            Debug.Log("up : level option");
            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
            {
                changeSelectedLevelUp();
                initializeLevelDisplay();
            }
        }

        if ((InputManager.GetKeyDown(KeyCode.S) || InputManager.GetKeyDown(KeyCode.DownArrow)))
        {
            Debug.Log("down : player option");
            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
            {
                changeSelectedPlayerDown();
                initializePlayerDisplay();
            }
            Debug.Log("down : level option");
            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
            {
                changeSelectedLevelDown();
                initializeLevelDisplay();
            }
        }
    }


    private void initializeLevelDisplay()
    {
        levelSelectOptionText = GameObject.Find(levelSelectOptionButtonName).GetComponent<Text>();
        levelSelectOptionText.text = levelSelectedData[levelselectedIndex].LevelDisplayName;
        GameOptions.levelSelected = levelSelectedData[levelselectedIndex].LevelObjectName;
    }

    private void initializePlayerDisplay()
    {

        playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        playerSelectOptionStatsText = GameObject.Find(playerSelectStatsObjectName).GetComponent<Text>();
        playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();

        playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
        playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;
        playerSelectOptionStatsText.text = playerSelectedData[playerSelectedIndex].Accuracy2Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].Accuracy3Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].Accuracy4Pt.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].Range.ToString("F0") + "\n"
            + playerSelectedData[playerSelectedIndex].CriticalPercent.ToString("F0");

        GameOptions.playerSelected = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    }

    private void loadPlayerSelectDataList()
    {
        Debug.Log("loadPlayerSelectDataList()");

        string path = "Prefabs/start_menu/player_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            shooterProfile temp = obj.GetComponent<shooterProfile>();
            playerSelectedData.Add(temp);
        }
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
        GameOptions.playerSelected = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        Debug.Log("player selected : " + GameOptions.playerSelected);
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
        GameOptions.playerSelected = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        Debug.Log("player selected : " + GameOptions.playerSelected);
    }

    private void changeSelectedLevelUp()
    {
        // if default index (first in list), go to end of list
        if (levelselectedIndex == 0)
        {
            levelselectedIndex = levelSelectedData.Count - 1;
        }
        else
        {
            // if not first index, decrement
            levelselectedIndex--;
        }
        GameOptions.levelSelected = levelSelectedData[levelselectedIndex].LevelObjectName;
        Debug.Log("level selected : " + GameOptions.levelSelected);
    }
    private void changeSelectedLevelDown()
    {
        // if default index (first in list
        if (levelselectedIndex == levelSelectedData.Count - 1)
        {
            levelselectedIndex = 0;
        }
        else
        {
            //if not first index, increment
            levelselectedIndex++;
        }
        GameOptions.levelSelected = levelSelectedData[levelselectedIndex].LevelObjectName;
        Debug.Log("level selected : " + GameOptions.levelSelected);
    }



    //private void createFirstModeDescription()
    //{

    //    //Debug.Log("description created");
    //    string tempString = EventSystem.current.currentSelectedGameObject.name + "_description";

    //    //Debug.Log("tempString : " + tempString);
    //    string tempPath = "Prefabs/start_menu/mode_descriptions/" + tempString;
    //    //string tempPath = "Prefabs/start_menu/mode_descriptions/mode_10_description";

    //    cloneDescription = Resources.Load(tempPath) as GameObject;
    //    Instantiate(cloneDescription, gameModeDescriptionSpawnPoint, Quaternion.identity, gameModeDescriptionsObject.transform);
    //}

    //private void getNextModeDecription()
    //{
    //    GameObject tempClone = GameObject.FindWithTag("mode_description");
    //    Destroy(tempClone);
    //    //Debug.Log("highlight changed");
    //    string tempString = EventSystem.current.currentSelectedGameObject.name + "_description";

    //    //Debug.Log("tempString : " + tempString);
    //    string tempPath = "Prefabs/start_menu/mode_descriptions/" + tempString;
    //    //string tempPath = "Prefabs/start_menu/mode_descriptions/mode_10_description";

    //    cloneDescription = Resources.Load(tempPath) as GameObject;
    //    Instantiate(cloneDescription, gameModeDescriptionSpawnPoint, Quaternion.identity, gameModeDescriptionsObject.transform);
    //}

    public void loadScene()
    {
        // i create the string this way so that i can have a description of the level so i know what im opening
        string sceneName = GameOptions.levelSelected + "_" + levelSelectedData[levelselectedIndex].LevelDescription;
        Debug.Log("player : " + GameOptions.playerSelected);
        Debug.Log("level : " + GameOptions.levelSelected);
        Debug.Log("scene name : " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}
