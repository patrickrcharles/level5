using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StatsManager : MonoBehaviour
{

    [SerializeField]
    private string currentHighlightedButton;

    const string scoreOptionButtonName = "score_options";
    const string highscoreSelectButtonName = "high_score_select";
    const string modeSelectButtonName = "mode_select_name";
    const string alltimeSelectButtonName = "all_time_select";
    const string mainMenuButtonName = "all_time_select";
    const string allTimeTableName = "high_scores_table";
    const string highScoreTableName = "all_time_table";

    GameObject scoreOptionButtonObject;
    GameObject highscoreSelectButtonObject;
    GameObject modeSelectButtonObject;
    GameObject alltimeSelectButtonObject;
    GameObject mainMenuButtonObject;

    //GameObject allTimeTableObject;
    //GameObject highScoreTableObject;

    Text modeSelectButtonText;

    [SerializeField]
    List<StatsTableHighScoreRow> highScoreRowsList;
    List<mode> modesList;

    int defaultModeSelectedIndex;
    int currentModeSelectedIndex;

    GameObject highScoreRowPrefab;
    string highScoreRowPrefabPath = "Prefabs/stats/highScoreRow";

    const string highScoresRowsName = "high_scores_rows";
    GameObject highScoresRowsObject;

    public static StatsManager instance;

    public class mode
    {
        public int modeSelectedId;
        public string modeSelectedName;

        public mode(int modeid, string modeName)
        {
            modeSelectedId = modeid;
            modeSelectedName = modeName;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        destroyInstanceIfAlreadyExists();

        scoreOptionButtonObject = GameObject.Find(scoreOptionButtonName);
        highscoreSelectButtonObject = GameObject.Find(highscoreSelectButtonName);
        modeSelectButtonText = GameObject.Find(modeSelectButtonName).GetComponent<Text>();
        alltimeSelectButtonObject = GameObject.Find(alltimeSelectButtonName);
        mainMenuButtonObject = GameObject.Find(mainMenuButtonName);

        // where rows need to be instantiated
        highScoresRowsObject = GameObject.Find(highScoresRowsName);

        // get mode ids and display names. mode ids will be used for queries to display data
        modesList = getModeSelectDataList();
        defaultModeSelectedIndex = 0;
        currentModeSelectedIndex = defaultModeSelectedIndex;


        modeSelectButtonText.text = modesList[defaultModeSelectedIndex].modeSelectedName;

        // row prefab to be instantiated
        highScoreRowPrefab = Resources.Load(highScoreRowPrefabPath) as GameObject;

        // highscores from db
        highScoreRowsList = DBHelper.instance.getListOfHighScoreRowsFromTableByModeId(15);

        foreach (StatsTableHighScoreRow row in highScoreRowsList)
        {
            Debug.Log("score = " + row.score + " | character =" + row.character
        + " | level = " + row.level + " | date = " + row.date);
        }
    }

    private void Start()
    {
        for(int i = 0; i < highScoreRowsList.Count; i++)
        {
            // set data for prefabs from list retrieved from database
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().score = highScoreRowsList[i].score;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().character = highScoreRowsList[i].character;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().level = highScoreRowsList[i].level;
            highScoreRowPrefab.GetComponent<StatsTableHighScoreRow>().date = highScoreRowsList[i].date;
            // instantiate row on necessary table object
            Instantiate(highScoreRowPrefab, highScoresRowsObject.transform.position, Quaternion.identity, highScoresRowsObject.transform);
        }
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
        //Debug.Log("currentHighlightedButton : " + currentHighlightedButton);

        // if high score selected
        if (currentHighlightedButton.Equals(scoreOptionButtonName))
        {

        }
        // if all time stats selected
        if (currentHighlightedButton.Equals(alltimeSelectButtonName))
        {

        }

        // ================================== navigation =====================================================================

        // up arrow navigation
        //if (InputManager.GetKeyDown(KeyCode.UpArrow)
        //    && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
        //    && !currentHighlightedButton.Equals(trafficSelectOptionName))
        //{
        //    EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
        //        .GetComponent<Button>().FindSelectableOnUp().gameObject);
        //}

        //// down arrow navigation
        //if (InputManager.GetKeyDown(KeyCode.DownArrow)
        //    && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
        //    && !currentHighlightedButton.Equals(trafficSelectOptionName))
        //{
        //    EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
        //        .GetComponent<Button>().FindSelectableOnDown().gameObject);
        //}

        // right arrow on player select
        if (InputManager.GetKeyDown(KeyCode.RightArrow))
        {
            //Debug.Log("right : player select");
            if (currentHighlightedButton.Equals(highscoreSelectButtonName))
            {
                Debug.Log("right : mode select");
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnRight().gameObject);
            }
        }


        // left arrow navigation on player options
        if (InputManager.GetKeyDown(KeyCode.LeftArrow))
        {
            //Debug.Log("left : player select");
            if (currentHighlightedButton.Equals(modeSelectButtonName))
            {
                Debug.Log("left : mode select");
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
        }
        //        //Debug.Log("left : level select");
        //        if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
        //        {
        //            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
        //                .GetComponent<Button>().FindSelectableOnLeft().gameObject);
        //        }
        //        if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
        //        {
        //            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
        //                .GetComponent<Button>().FindSelectableOnLeft().gameObject);
        //        }
        //        if (currentHighlightedButton.Equals(trafficSelectOptionName))
        //        {
        //            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
        //                .GetComponent<Button>().FindSelectableOnLeft().gameObject);
        //        }
        //    }

        //    // up/down arrow on player options
        //    if ((InputManager.GetKeyDown(KeyCode.W) || InputManager.GetKeyDown(KeyCode.UpArrow)))
        //    {
        //        //Debug.Log("up : player option");
        //        if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
        //        {
        //            changeSelectedPlayerUp();
        //            initializePlayerDisplay();
        //        }
        //        //Debug.Log("up : level option");
        //        if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
        //        {
        //            changeSelectedLevelUp();
        //            initializeLevelDisplay();
        //        }
        //        //Debug.Log("up : level option");
        //        if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
        //        {
        //            changeSelectedModeUp();
        //            intializeModeDisplay();
        //        }
        //        if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
        //        {
        //            changeSelectedCheerleaderUp();
        //            initializeCheerleaderDisplay();
        //        }
        //        if (currentHighlightedButton.Equals(trafficSelectOptionName))
        //        {
        //            changeSelectedTrafficOption();
        //            initializeTrafficOptionDisplay();
        //        }
        //    }

        //    if ((InputManager.GetKeyDown(KeyCode.S) || InputManager.GetKeyDown(KeyCode.DownArrow)))
        //    {
        //        //Debug.Log("down : player option");
        //        if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
        //        {
        //            changeSelectedPlayerDown();
        //            initializePlayerDisplay();
        //        }
        //        //Debug.Log("down : level option");
        //        if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
        //        {
        //            changeSelectedLevelDown();
        //            initializeLevelDisplay();
        //        }
        //        if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
        //        {
        //            changeSelectedModeDown();
        //            intializeModeDisplay();
        //        }
        //        if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
        //        {
        //            changeSelectedCheerleaderDown();
        //            initializeCheerleaderDisplay();
        //        }
        //        if (currentHighlightedButton.Equals(trafficSelectOptionName))
        //        {
        //            changeSelectedTrafficOption();
        //            initializeTrafficOptionDisplay();
        //        }
        //    }

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
            //Debug.Log("created, destroy this");
            Destroy(gameObject);
        }
    }

    private List<mode> getModeSelectDataList()
    {
        //Debug.Log("loadPlayerSelectDataList()");

        List<mode> tempList = new List<mode>();

        string path = "Prefabs/start_menu/mode_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];
        //Debug.Log(objects.Length);

        foreach (GameObject obj in objects)
        {
            StartScreenModeSelected temp = obj.GetComponent<StartScreenModeSelected>();
            //Debug.Log("modeid : " + temp.ModeId + "    name : " + temp.ModelDisplayName);
            tempList.Add(new mode(temp.ModeId, temp.ModelDisplayName));
        }

        // sort list by  level id
        tempList.Sort(sortByModeId);

        return tempList;
    }

    static int sortByModeId(mode m1, mode m2)
    {
        return m2.modeSelectedId.CompareTo(m2.modeSelectedId);
    }
}
