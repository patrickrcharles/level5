
using Assets.Scripts.database;
using Assets.Scripts.restapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{
    [SerializeField]
    private string currentHighlightedButton;

    //const string scoreOptionButtonName = "score_options";
    //const string highscoreSelectButtonName = "high_score_select";
    const string modeSelectButtonName = "mode_select_name";
    const string modeSelectButtonHardcoreName = "mode_select_name_hardcore";
    const string modeSelectButtonOnlineName = "mode_select_name_online";
    const string alltimeSelectButtonName = "all_time_select";
    const string mainMenuButtonName = "main_menu";
    // table names
    const string highScoreTableName = "high_scores_table";
    const string allTimeTableName = "all_time_table";

    // tag find high score rows that are instantiated
    const string highScoreRowTag = "high_score_row";
    const string mainMenuSceneName = "level_00_start";

    GameObject allTimeTableObject;
    GameObject highScoreTableObject;

    Text modeSelectButtonText;
    Text modeSelectButtonHardcoreText;
    Text modeSelectButtonOnlineText;

    // list of high score rows
    [SerializeField]
    List<StatsTableHighScoreRow> highScoreRowsDataList;
    //list of high score row objects
    [SerializeField]
    List<GameObject> highScoreRowsObjectsList;
    // list of modes
    [SerializeField]
    List<mode> modesList;

    int defaultModeSelectedIndex;
    int currentModeSelectedIndex;

    // high score results pagination
    int highScoresResultsPageNumber;

    // high score rows
    const string highScoreRowPrefabPath = "Prefabs/stats/highScoreRow";
    const string highScoresRowsName = "high_scores_rows";
    GameObject highScoresRowsObject;
    GameObject highScoreRowPrefab;

    bool regularLoaded;
    bool hardcoreLoaded;
    bool onlineLoaded;

    public int numResults;

    PlayerControls controls;

    public static StatsManager instance;

    // for input system
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

    // store data for each row
    public class mode
    {
        public int modeSelectedId;
        public string modeSelectedName;
        public string modeSelectedHighScoreField;

        // constructor
        public mode(int modeid, string modeName, string field)
        {
            modeSelectedId = modeid;
            modeSelectedName = modeName;
            modeSelectedHighScoreField = field;
        }
    }

    void Awake()
    {
        //check for existsing instance of statmanager
        //destroyInstanceIfAlreadyExists();

        instance = this;

        controls = new PlayerControls();

        // find objects/buttons
        modeSelectButtonText = GameObject.Find(modeSelectButtonName).GetComponent<Text>();
        modeSelectButtonHardcoreText = GameObject.Find(modeSelectButtonHardcoreName).GetComponent<Text>();
        modeSelectButtonOnlineText = GameObject.Find(modeSelectButtonOnlineName).GetComponent<Text>();

        highScoreTableObject = GameObject.Find(highScoreTableName);
        allTimeTableObject = GameObject.Find(allTimeTableName);

        // parent object where rows will be instantiated
        // ex. usage Instantiate(prefab, position, quaternion, parent object);
        highScoresRowsObject = GameObject.Find(highScoresRowsName);

        // get mode ids and display names. mode ids will be used for queries to display data
        modesList = getModeSelectDataList();

        defaultModeSelectedIndex = 0;
        currentModeSelectedIndex = defaultModeSelectedIndex;

        // set default game mode name
        modeSelectButtonText.text = modesList[defaultModeSelectedIndex].modeSelectedName;
        modeSelectButtonHardcoreText.text = modesList[defaultModeSelectedIndex].modeSelectedName;
        modeSelectButtonOnlineText.text = modesList[defaultModeSelectedIndex].modeSelectedName;

        //Debug.Log("modesList[defaultModeSelectedIndex].modeSelectedName : " + modesList[defaultModeSelectedIndex].modeSelectedName);
        //Debug.Log("modesList[defaultModeSelectedIndex].modeSelectedName : " + modesList[defaultModeSelectedIndex].modeSelectedId);

        // row prefab to be instantiated
        highScoreRowPrefab = Resources.Load(highScoreRowPrefabPath) as GameObject;

        // get mode id of default game mode
        string field = modesList[defaultModeSelectedIndex].modeSelectedHighScoreField;

        // get data for default mode to be displayed
        if (GameObject.FindGameObjectWithTag("database") != null)
        {
            try
            {
                highScoreRowsDataList = 
                    DBHelper.instance.getListOfHighScoreRowsFromTableByModeIdAndField(field, modesList[defaultModeSelectedIndex].modeSelectedId, false, highScoresResultsPageNumber);

                numResults = DBHelper.instance.getNumberOfResults(field, modesList[defaultModeSelectedIndex].modeSelectedId, false, highScoresResultsPageNumber);
                //Debug.Log("numResults : " + numResults);
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
    }

    private void Start()
    {
        // default page number value, start on first page
        highScoresResultsPageNumber = 0;

        AnaylticsManager.MenuStatsLoaded();

        // create rows dor data display
        for (int i = 0; i < highScoreRowsDataList.Count; i++)
        {
            // set data for prefabs from list retrieved from database
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().score = highScoreRowsDataList[i].score;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().character = highScoreRowsDataList[i].character;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().level = highScoreRowsDataList[i].level;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().date = highScoreRowsDataList[i].date;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().hardcoreEnabled = highScoreRowsDataList[i].hardcoreEnabled;
            // instantiate row on necessary table object
            Instantiate(highScoreRowPrefab, highScoresRowsObject.transform.position, Quaternion.identity, highScoresRowsObject.transform);
        }
        // list of row onjects that contain the Text displays
        highScoreRowsObjectsList = GameObject.FindGameObjectsWithTag(highScoreRowTag).ToList();

        // default table view
        highScoreTableObject.SetActive(true);
        allTimeTableObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            //Debug.Log("if (EventSystem.current.currentSelectedGameObject == null) : ");
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }

        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";

        // ================================== navigation =====================================================================

        // high scores table button selected
        if (currentHighlightedButton.Equals(modeSelectButtonName) )
        {
            highScoreTableObject.SetActive(true);
            allTimeTableObject.SetActive(false);
            //regularLoaded = true;

            if (controls.UINavigation.Left.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("left");
                //changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplay(false);
            }

            if (controls.UINavigation.Right.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("right");
                //changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplay(false);
            }

            if (controls.UINavigation.Up.triggered)//|| InputManager.GetKeyDown(KeyCode.W))
            {
                navigateUp();
                regularLoaded = false;
            }

            // down arrow navigation
            if (controls.UINavigation.Down.triggered)//|| InputManager.GetKeyDown(KeyCode.S))
            {
                navigateDown();
                regularLoaded = false;
            }
            //changeHighScoreDataDisplay(false);
            if (!regularLoaded)
            {
                regularLoaded = true;
                changeHighScoreDataDisplay(false);
            }
        }
        else
        {
            regularLoaded = false;
        }
        // high scores table button selected
        if (currentHighlightedButton.Equals(modeSelectButtonHardcoreName))
        {
            highScoreTableObject.SetActive(true);
            allTimeTableObject.SetActive(false);
            if (controls.UINavigation.Left.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("left");
                //changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplay(true);
            }

            if (controls.UINavigation.Right.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("right");
                //changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplay(true);
            }

            if (controls.UINavigation.Up.triggered)//|| InputManager.GetKeyDown(KeyCode.W))
            {
                navigateUp();
                hardcoreLoaded = false;
            }

            // down arrow navigation
            if (controls.UINavigation.Down.triggered)//|| InputManager.GetKeyDown(KeyCode.S))
            {
                navigateDown();
                hardcoreLoaded = false;
            }
            //changeHighScoreDataDisplay(true);
            if (!hardcoreLoaded)
            {
                hardcoreLoaded = true;
                changeHighScoreDataDisplay(true);
            }
        }
        else
        {
            hardcoreLoaded = false;
        }
        // high scores table button selected
        if (currentHighlightedButton.Equals(modeSelectButtonOnlineName))
        {
            highScoreTableObject.SetActive(true);
            allTimeTableObject.SetActive(false);
            //regularLoaded = true;

            if (controls.UINavigation.Left.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("left");
                //changeHighScoreModeNameDisplay();
                //changeHighScoreDataDisplay(false);
                changeHighScoreDataDisplayOnline(false);
            }

            if (controls.UINavigation.Right.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("right");
                //changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplayOnline(false);
            }

            if (controls.UINavigation.Up.triggered)//|| InputManager.GetKeyDown(KeyCode.W))
            {
                navigateUp();
                onlineLoaded = false;
            }

            // down arrow navigation
            if (controls.UINavigation.Down.triggered)//|| InputManager.GetKeyDown(KeyCode.S))
            {
                navigateDown();
                onlineLoaded = false;
            }
            //changeHighScoreDataDisplay(false);
            if (!onlineLoaded)
            {
                onlineLoaded = true;
                changeHighScoreDataDisplayOnline(false);
            }
        }
        else
        {
            regularLoaded = false;
        }

        // all time stats table button selected
        if (currentHighlightedButton.Equals(alltimeSelectButtonName))
        {
            allTimeTableObject.SetActive(true);
            highScoreTableObject.SetActive(false);

            // up arrow navigation
            if (controls.UINavigation.Up.triggered)//|| InputManager.GetKeyDown(KeyCode.W))
            {
                navigateUp();
            }

            // down arrow navigation
            if (controls.UINavigation.Down.triggered)// || InputManager.GetKeyDown(KeyCode.S))
            {
                navigateDown();
            }
        }

        // main menu button selected
        if (currentHighlightedButton.Equals(mainMenuButtonName))
        {
            if (controls.UINavigation.Submit.triggered)
            {
                loadMainMenu(mainMenuSceneName);
            }

            // up arrow navigation
            if (controls.UINavigation.Up.triggered)// || InputManager.GetKeyDown(KeyCode.W))
            {
                navigateUp();
            }

            // down arrow navigation
            if (controls.UINavigation.Down.triggered)// || InputManager.GetKeyDown(KeyCode.S))
            {
                navigateDown();
            }
        }
    }

    public static void navigateUp()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
            .GetComponent<Button>().FindSelectableOnUp().gameObject);
    }

    public static void navigateDown()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
            .GetComponent<Button>().FindSelectableOnDown().gameObject);
    }

    public void loadMainMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    private void destroyInstanceIfAlreadyExists()
    {
        // make sure only once instance
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private List<mode> getModeSelectDataList()
    {
        List<mode> tempList = new List<mode>();

        string path = "Prefabs/menu_start/mode_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];
        //Debug.Log(objects.Length);

        foreach (GameObject obj in objects)
        {
            StartScreenModeSelected temp = obj.GetComponent<StartScreenModeSelected>();

            //Debug.Log("!temp.ModeDisplayName.ToLower().Contains(free) : " + !temp.ModeDisplayName.ToLower().Contains("free"));
            //Debug.Log("!temp.ModeDisplayName.ToLower().Contains(arcade) : " + !temp.ModeDisplayName.ToLower().Contains("arcade"));

            //// add to list
            //if (!temp.ModeDisplayName.ToLower().Contains("free") 
            //    && !temp.ModeDisplayName.ToLower().Contains("arcade")) // exclude freeplay
            //{
            //    tempList.Add(new mode(temp.ModeId, temp.ModeDisplayName, temp.HighScoreField));

            //}
            if (temp.ModeId != 98)
            {
                tempList.Add(new mode(temp.ModeId, temp.ModeDisplayName, temp.HighScoreField));
            }
        }

        // sort list by  level id
        tempList.Sort(sortByModeId);

        return tempList;
    }

    static int sortByModeId(mode m1, mode m2)
    {
        return m2.modeSelectedId.CompareTo(m2.modeSelectedId);
    }

    public void changeSelectedMode(string direction)
    {
        // left option || decrement
        if (direction.ToLower().Equals("left"))
        {
            // if default index (first in list), go to end of list
            if (currentModeSelectedIndex == 0)
            {
                currentModeSelectedIndex = modesList.Count - 1;
            }
            else
            {
                // if not first index, decrement
                currentModeSelectedIndex--;
            }
        }

        // right option || increment
        if (direction.ToLower().Equals("right"))
        {
            // if default index (first in list
            if (currentModeSelectedIndex == modesList.Count - 1)
            {
                currentModeSelectedIndex = 0;
            }
            else
            {
                //if not first index, increment
                currentModeSelectedIndex++;
            }
        }

        modeSelectButtonText.text = modesList[currentModeSelectedIndex].modeSelectedName;
        modeSelectButtonHardcoreText.text = modesList[currentModeSelectedIndex].modeSelectedName;
        modeSelectButtonOnlineText.text = modesList[currentModeSelectedIndex].modeSelectedName;
    }

    public void changeHighScoreModeNameDisplay()
    {
        modeSelectButtonText.text = modesList[currentModeSelectedIndex].modeSelectedName;
        modeSelectButtonHardcoreText.text = modesList[currentModeSelectedIndex].modeSelectedName;
    }

    public void changeHighScoreDataDisplay(bool hardcoreValue)
    {
        if (GameObject.FindGameObjectWithTag("database") != null)
        {
            try
            {
                // counts number entries returned.
                int index = 0;
                // get highscore field from mode prefab
                string field = modesList[currentModeSelectedIndex].modeSelectedHighScoreField;
                // get new list of scores based on currently selected game mode
                highScoreRowsDataList
                    = DBHelper.instance.getListOfHighScoreRowsFromTableByModeIdAndField(field, modesList[currentModeSelectedIndex].modeSelectedId, hardcoreValue, highScoresResultsPageNumber);
                numResults = DBHelper.instance.getNumberOfResults(field, modesList[currentModeSelectedIndex].modeSelectedId, hardcoreValue, highScoresResultsPageNumber);
                //Debug.Log("numResults for modeId: "+ modesList[currentModeSelectedIndex].modeSelectedId +  " : "+ numResults);

                // updates row with new data
                for (int i = 0; i < highScoreRowsDataList.Count; i++)
                {
                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().score = highScoreRowsDataList[i].score.ToString();
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().character = highScoreRowsDataList[i].character;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().level = highScoreRowsDataList[i].level;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().date = highScoreRowsDataList[i].date;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().hardcoreEnabled = highScoreRowsDataList[i].hardcoreEnabled;
                    index++;
                }
                // empty out rows if scores do not exist or there isnt at least 10
                for (int i = index; i < highScoreRowsObjectsList.Count; i++)
                {
                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().score = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().character = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().level = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().date = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().hardcoreEnabled = "";
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
    }

    public void changeHighScoreDataDisplayOnline(bool hardcoreValue)
    {
        if (GameObject.Find("restapi") != null)
        {
            try
            {
                // counts number entries returned.
                int index = 0;
                int modeid = modesList[currentModeSelectedIndex].modeSelectedId;
                // get highscore field from mode prefab
                string field = modesList[currentModeSelectedIndex].modeSelectedHighScoreField;

                //// game modes that require float values
                //if ((modeid > 4 && modeid < 14) || modeid == 99)
                //{
                //    //score = reader.GetFloat(0).ToString();
                //    // float
                //}
                //else
                //{
                //    //score = reader.GetInt32(0).ToString();
                //    //float
                //}

                List<DBHighScoreModel> dBHighScoreModelList = new List<DBHighScoreModel>();
                dBHighScoreModelList =  APIHelper.GetHighscoreByModeid(modeid, field );

                // updates row with new data
                for (int i = 0; i < dBHighScoreModelList.Count; i++)
                {

                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().score = dBHighScoreModelList[i].TotalPoints.ToString();
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().character = dBHighScoreModelList[i].Character;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().level = dBHighScoreModelList[i].Level;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().date = dBHighScoreModelList[i].Date;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().hardcoreEnabled = dBHighScoreModelList[i].HardcoreEnabled.ToString();
                    index++;
                }
                // empty out rows if scores do not exist or there isnt at least 10
                for (int i = index; i < dBHighScoreModelList.Count; i++)
                {
                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().score = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().character = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().level = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().date = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().hardcoreEnabled = "";
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
    }

    public static string ModeSelectButtonName => modeSelectButtonName;
    public static string AlltimeSelectButtonName => alltimeSelectButtonName;
    public static string MainMenuButtonName => mainMenuButtonName;
    public static string MainMenuSceneName => mainMenuSceneName;
    public static string ModeSelectButtonHardcoreName => modeSelectButtonHardcoreName;
}
