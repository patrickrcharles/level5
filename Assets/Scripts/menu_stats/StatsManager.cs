
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
    [SerializeField]
    private string previousHighlightedButton;

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
    [SerializeField]
    Text modeSelectButtonText;
    [SerializeField]
    Text modeSelectButtonHardcoreText;
    [SerializeField]
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

    [SerializeField]
    private bool trafficEnabled;
    [SerializeField]
    private bool hardcoreEnabled;
    [SerializeField]
    private bool enemiesEnabled;

    //selectable option text
    [SerializeField]
    private Text trafficSelectOptionText;
    [SerializeField]
    private Text hardcoreSelectOptionText;
    [SerializeField]
    private Text enemySelectOptionText;

    int defaultModeSelectedIndex;
    int currentModeSelectedIndex;

    // high score results pagination
    int highScoresResultsPageNumber;

    // high score rows
    const string highScoreRowPrefabPath = "Prefabs/stats/highScoreRow";
    const string highScoresRowsName = "high_scores_rows";
    [SerializeField]
    GameObject highScoresRowsObject;
    [SerializeField]
    GameObject highScoreRowPrefab;
    [SerializeField]
    bool localLoaded;
    [SerializeField]
    bool onlineLoaded;

    bool buttonPressed;

    //traffic objects
    //private const string trafficSelectButtonName = "traffic_name_button";
    private const string trafficSelectValueName = "traffic_value_button";

    //hardcore mode
    //private const string hardcoreSelectButtonName = "hardcore_name_button";
    private const string hardcoreSelectValueName = "hardcore_value_button";

    //hardcore mode
    //private const string enemySelectButtonName = "enemies_name_button";
    private const string enemySelectValueName = "enemies_value_button";

    //public int numResults;

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

        instance = this;
        controls = new PlayerControls();

        // table objects
        highScoreTableObject = GameObject.Find(highScoreTableName);
        allTimeTableObject = GameObject.Find(allTimeTableName);

        // parent object where rows will be instantiated
        // ex. usage Instantiate(prefab, position, quaternion, parent object);
        highScoresRowsObject = GameObject.Find(highScoresRowsName);

        // get mode ids and display names. mode ids will be used for queries to display data
        modesList = getModeSelectDataList();

        defaultModeSelectedIndex = 0;
        currentModeSelectedIndex = defaultModeSelectedIndex;

        // row prefab to be instantiated
        highScoreRowPrefab = Resources.Load(highScoreRowPrefabPath) as GameObject;

        // get mode id of default game mode
        string field = modesList[defaultModeSelectedIndex].modeSelectedHighScoreField;

        // get data for default mode to be displayed
        if (GameObject.FindGameObjectWithTag("database") != null)
        {
            try
            {
                // get default high score list + num results
                highScoreRowsDataList =
                DBHelper.instance.getListOfHighScoreRowsFromTableByModeIdAndField(field,
                    modesList[defaultModeSelectedIndex].modeSelectedId,
                    hardcoreEnabled,
                    trafficEnabled,
                    enemiesEnabled,
                    highScoresResultsPageNumber);

                //numResults = 
                //    DBHelper.instance.getNumberOfResults(field,
                //    modesList[defaultModeSelectedIndex].modeSelectedId,
                //    false, highScoresResultsPageNumber);
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
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().Score = highScoreRowsDataList[i].Score;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().Character = highScoreRowsDataList[i].Character;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().Level = highScoreRowsDataList[i].Level;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().Date = highScoreRowsDataList[i].Date;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().HardcoreEnabled = highScoreRowsDataList[i].HardcoreEnabled;
            // instantiate row on necessary table object
            Instantiate(highScoreRowPrefab, highScoresRowsObject.transform.position, Quaternion.identity, highScoresRowsObject.transform);
        }
        // list of row onjects that contain the Text displays
        highScoreRowsObjectsList = GameObject.FindGameObjectsWithTag(highScoreRowTag).ToList();

        // default table view
        if (!highScoreTableObject.activeSelf)
        {
            highScoreTableObject.SetActive(true);
        }
        if (allTimeTableObject.activeSelf)
        {
            allTimeTableObject.SetActive(false);
        }

        initializeTrafficOptionDisplay();
        initializeHardcoreOptionDisplay();
        initializeEnemyOptionDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            //Debug.Log("if (EventSystem.current.currentSelectedGameObject == null) : ");
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }

        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name;

        // ================================== navigation =====================================================================

        // high scores table button selected
        if (currentHighlightedButton.Equals(trafficSelectValueName) && !buttonPressed)
        {
            if (controls.UINavigation.Up.triggered || controls.UINavigation.Down.triggered)
            {
                buttonPressed = true;
                changeSelectedTrafficOption();
                initializeTrafficOptionDisplay();
                changeHighScoreDataDisplay();
                buttonPressed = false;
            }
        }
        // high scores table button selected
        if (currentHighlightedButton.Equals(hardcoreSelectValueName) && !buttonPressed)
        {
            if (controls.UINavigation.Up.triggered || controls.UINavigation.Down.triggered)
            {
                buttonPressed = true;
                changeSelectedHardcoreOption();
                initializeHardcoreOptionDisplay();
                changeHighScoreDataDisplay();
                buttonPressed = false;
            }
        }
        // high scores table button selected
        if (currentHighlightedButton.Equals(enemySelectValueName) && !buttonPressed)
        {
            if (controls.UINavigation.Up.triggered || controls.UINavigation.Down.triggered)
            {
                buttonPressed = true;
                changeSelectedEnemiesOption();
                initializeEnemyOptionDisplay();
                changeHighScoreDataDisplay();
                buttonPressed = false;
            }
        }

        // high scores table button selected
        if (currentHighlightedButton.Equals(modeSelectButtonName))
        {
            if (previousHighlightedButton != modeSelectButtonName)
            {
                changeHighScoreDataDisplay();
            }
            if (!highScoreTableObject.activeSelf)
            {
                highScoreTableObject.SetActive(true);
            }
            if (allTimeTableObject.activeSelf)
            {
                allTimeTableObject.SetActive(false);
            }

            if (controls.UINavigation.Left.triggered && !buttonPressed)
            {
                //save previous button
                previousHighlightedButton = currentHighlightedButton;

                buttonPressed = true;
                // change selected mode and display data based on mode selected
                changeSelectedMode("left");
                changeHighScoreDataDisplay();
                buttonPressed = false;
            }

            if (controls.UINavigation.Right.triggered && !buttonPressed)
            {
                //save previous button
                previousHighlightedButton = currentHighlightedButton;

                buttonPressed = true;
                // change selected mode and display data based on mode selected
                changeSelectedMode("right");
                changeHighScoreDataDisplay();
                buttonPressed = false;
            }
            modeSelectButtonText.text = modesList[currentModeSelectedIndex].modeSelectedName;
        }

        // high scores table button selected
        if (currentHighlightedButton.Equals(modeSelectButtonOnlineName))
        {
            if (previousHighlightedButton != modeSelectButtonOnlineName)
            {
                changeHighScoreDataDisplayOnline();
            }
            if (!highScoreTableObject.activeSelf)
            {
                highScoreTableObject.SetActive(true);
            }
            if (allTimeTableObject.activeSelf)
            {
                allTimeTableObject.SetActive(false);
            }

            if (controls.UINavigation.Left.triggered && !buttonPressed)
            {
                //save previous button
                previousHighlightedButton = currentHighlightedButton;

                buttonPressed = true;
                // change selected mode and display data based on mode selected
                changeSelectedMode("left");
                changeHighScoreDataDisplayOnline();
                buttonPressed = false;
            }

            if (controls.UINavigation.Right.triggered && !buttonPressed)
            {
                //save previous button
                previousHighlightedButton = currentHighlightedButton;

                buttonPressed = true;
                // change selected mode and display data based on mode selected
                changeSelectedMode("right");
                changeHighScoreDataDisplayOnline();
                buttonPressed = false;
            }
            modeSelectButtonOnlineText.text = modesList[currentModeSelectedIndex].modeSelectedName;
        }

        // all time stats table button selected
        if (currentHighlightedButton.Equals(alltimeSelectButtonName))
        {
            if (highScoreTableObject.activeSelf)
            {
                highScoreTableObject.SetActive(false);
            }
            if (!allTimeTableObject.activeSelf)
            {
                allTimeTableObject.SetActive(true);
            }
        }

        // main menu button selected
        if (currentHighlightedButton.Equals(mainMenuButtonName) && !buttonPressed)
        {
            if (controls.UINavigation.Submit.triggered)
            {
                buttonPressed = true;
                loadMainMenu(mainMenuSceneName);
                buttonPressed = false;
            }
        }
        // save at end of frame
        previousHighlightedButton = currentHighlightedButton;
    }

    public static void navigateUp()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
            .GetComponent<Button>().FindSelectableOnUp().gameObject);
    }

    public static void navigateDown()
    {
        Debug.Log("navigate down");
        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
            .GetComponent<Button>().FindSelectableOnDown().gameObject);
    }

    public void loadMainMenu(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
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
    }

    public void changeHighScoreDataDisplay()
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
                    = DBHelper.instance.getListOfHighScoreRowsFromTableByModeIdAndField(field,
                    modesList[currentModeSelectedIndex].modeSelectedId,
                    hardcoreEnabled,
                    trafficEnabled,
                    enemiesEnabled,
                    highScoresResultsPageNumber);
                //numResults = DBHelper.instance.getNumberOfResults(field, modesList[currentModeSelectedIndex].modeSelectedId, hardcoreEnabled, highScoresResultsPageNumber);
                //Debug.Log("numResults for modeId: "+ modesList[currentModeSelectedIndex].modeSelectedId +  " : "+ numResults);

                // updates row with new data
                for (int i = 0; i < highScoreRowsDataList.Count; i++)
                {
                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().UserName = highScoreRowsDataList[i].UserName;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Score = highScoreRowsDataList[i].Score;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Character = highScoreRowsDataList[i].Character;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Level = highScoreRowsDataList[i].Level;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Date = highScoreRowsDataList[i].Date;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().HardcoreEnabled = highScoreRowsDataList[i].HardcoreEnabled;
                    index++;
                }
                // empty out rows if scores do not exist or there isnt at least 10
                for (int i = index; i < highScoreRowsObjectsList.Count; i++)
                {
                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().UserName = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Score = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Character = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Level = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Date = "";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().HardcoreEnabled = "";
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
    }

    public void changeHighScoreDataDisplayOnline()
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

                List<StatsTableHighScoreRow> highScoreRowList = new List<StatsTableHighScoreRow>();

                highScoreRowList = APIHelper.GetHighscoreByModeid(modeid,
                    Convert.ToInt32(hardcoreEnabled),
                    Convert.ToInt32(trafficEnabled),
                    Convert.ToInt32(enemiesEnabled));

                Debug.Log("***** hardcore " + Convert.ToInt32(hardcoreEnabled));
                Debug.Log("***** traffic " + Convert.ToInt32(trafficEnabled));
                Debug.Log("***** enemies " + Convert.ToInt32(enemiesEnabled));

                int rowCount;
                if (highScoreRowList.Count < 10 && highScoreRowList != null)
                {
                    rowCount = highScoreRowList.Count;
                    index = highScoreRowList.Count;
                }
                else
                {
                    rowCount = 10;
                    index = 0;
                }
                // updates row with new data
                for (int i = 0; i < rowCount; i++)
                {

                    // set data for prefabs from list retrieved from database
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().UserName = highScoreRowList[i].UserName;
                    //highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().UserName = "userNamePlaceholder";
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Score = highScoreRowList[i].Score;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Character = highScoreRowList[i].Character;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Level = highScoreRowList[i].Level;
                    highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Date = highScoreRowList[i].Date;
                    index++;
                }

                index = highScoreRowList.Count;
                // empty out rows if scores do not exist or there isnt at least 10
                //for (int i = index; i < highScoreRowList.Count; i++)
                if (index < 10)
                {
                    for (int i = index; i < 10; i++)
                    {
                        // set data for prefabs from list retrieved from database
                        highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().UserName = "";
                        highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Score = "";
                        highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Character = "";
                        highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Level = "";
                        highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().Date = "";
                    }
                }
                //onlineLoaded = true;
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                //onlineLoaded = false;
                return;
            }
        }
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

    public static string ModeSelectButtonName => modeSelectButtonName;
    public static string AlltimeSelectButtonName => alltimeSelectButtonName;
    public static string MainMenuButtonName => mainMenuButtonName;
    public static string MainMenuSceneName => mainMenuSceneName;
    public static string ModeSelectButtonHardcoreName => modeSelectButtonHardcoreName;
}
