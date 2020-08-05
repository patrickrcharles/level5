
using System.Collections.Generic;
using System.Linq;
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
    const string alltimeSelectButtonName = "all_time_select";
    const string mainMenuButtonName = "main_menu";
    // table names
    const string highScoreTableName = "high_scores_table";
    const string allTimeTableName = "all_time_table";

    // tag find high score rows that are instantiated
    const string highScoreRowTag = "high_score_row";

    const string mainMenuSceneName = "level_00_start";

    GameObject scoreOptionButtonObject;
    GameObject highscoreSelectButtonObject;
    GameObject modeSelectButtonObject;
    GameObject alltimeSelectButtonObject;
    GameObject mainMenuButtonObject;

    GameObject allTimeTableObject;
    GameObject highScoreTableObject;

    Text modeSelectButtonText;

    [SerializeField]
    List<StatsTableHighScoreRow> highScoreRowsDataList;
    List<mode> modesList;
    List<GameObject> highScoreRowsObjectsList;

    int defaultModeSelectedIndex;
    int currentModeSelectedIndex;

    GameObject highScoreRowPrefab;
    const string highScoreRowPrefabPath = "Prefabs/stats/highScoreRow";

    const string highScoresRowsName = "high_scores_rows";
    GameObject highScoresRowsObject;

    PlayerControls controls;

    public static StatsManager instance;

    // for input system
    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
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
        destroyInstanceIfAlreadyExists();

        controls = new PlayerControls();

        // find objects/buttons
        modeSelectButtonText = GameObject.Find(modeSelectButtonName).GetComponent<Text>();
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

        // row prefab to be instantiated
        highScoreRowPrefab = Resources.Load(highScoreRowPrefabPath) as GameObject;

        // get mode id of default game mode
        string field = modesList[defaultModeSelectedIndex].modeSelectedHighScoreField;

        // get data for default mode to be displayed
        highScoreRowsDataList =
            DBHelper.instance.getListOfHighScoreRowsFromTableByModeIdAndField(field, modesList[defaultModeSelectedIndex].modeSelectedId);
    }

    private void Start()
    {
        // create rows dor data display
        for (int i = 0; i < highScoreRowsDataList.Count; i++)
        {
            // set data for prefabs from list retrieved from database
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().score = highScoreRowsDataList[i].score;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().character = highScoreRowsDataList[i].character;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().level = highScoreRowsDataList[i].level;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().date = highScoreRowsDataList[i].date;
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
        if (currentHighlightedButton.Equals(modeSelectButtonName))
        {
            highScoreTableObject.SetActive(true);
            allTimeTableObject.SetActive(false);

            if (controls.UINavigation.Left.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("left");
                changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplay();
            }

            if (controls.UINavigation.Right.triggered)
            {
                // change selected mode and display data based on mode selected
                changeSelectedMode("right");
                changeHighScoreModeNameDisplay();
                changeHighScoreDataDisplay();
            }

            if (controls.UINavigation.Up.triggered)//|| InputManager.GetKeyDown(KeyCode.W))
            {
                navigateUp();
            }

            // down arrow navigation
            if (controls.UINavigation.Down.triggered)//|| InputManager.GetKeyDown(KeyCode.S))
            {
                navigateDown();
            }
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

    private static void navigateUp()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
            .GetComponent<Button>().FindSelectableOnUp().gameObject);
    }

    private static void navigateDown()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
            .GetComponent<Button>().FindSelectableOnDown().gameObject);
    }

    private void loadMainMenu(string sceneName)
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

        string path = "Prefabs/start_menu/mode_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];
        //Debug.Log(objects.Length);

        foreach (GameObject obj in objects)
        {
            StartScreenModeSelected temp = obj.GetComponent<StartScreenModeSelected>();
            // add to list
            if (!temp.ModelDisplayName.ToLower().Contains("free")) // exclude freeplay
            {
                tempList.Add(new mode(temp.ModeId, temp.ModelDisplayName, temp.HighScoreField));
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

    private void changeSelectedMode(string direction)
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

    private void changeHighScoreModeNameDisplay()
    {
        modeSelectButtonText.text = modesList[currentModeSelectedIndex].modeSelectedName;
    }

    private void changeHighScoreDataDisplay()
    {
        // counts number entries returned.
        int index = 0;

        // get highscore field from mode prefab
        string field = modesList[currentModeSelectedIndex].modeSelectedHighScoreField;

        // get new list of scores based on currently selected game mode
        highScoreRowsDataList
            = DBHelper.instance.getListOfHighScoreRowsFromTableByModeIdAndField(field, modesList[currentModeSelectedIndex].modeSelectedId);

        // updates row with new data
        for (int i = 0; i < highScoreRowsDataList.Count; i++)
        {
            // set data for prefabs from list retrieved from database
            highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().score = highScoreRowsDataList[i].score.ToString();
            highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().character = highScoreRowsDataList[i].character;
            highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().level = highScoreRowsDataList[i].level;
            highScoreRowsObjectsList[i].GetComponent<StatsTableHighScoreRow>().date = highScoreRowsDataList[i].date;
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
        }
    }
}
