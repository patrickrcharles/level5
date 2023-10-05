
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;

    //list of all shooter profiles with player data
    [SerializeField]
    private List<CharacterProfile> playerSelectedData;
    public List<CharacterProfile> PlayerSelectedData { get => playerSelectedData; }

    [SerializeField]
    private List<CharacterProfile> cpuPlayerSelectedData;
    // list off cheerleader profile data
    [SerializeField]
    private List<CheerleaderProfile> cheerleaderSelectedData;
    public List<CheerleaderProfile> CheerleaderSelectedData { get => cheerleaderSelectedData; }
    // list off level  data
    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;
    public List<StartScreenLevelSelected> LevelSelectedData { get => levelSelectedData; }

    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;
    public List<StartScreenModeSelected> ModeSelectedData { get => modeSelectedData; }
    public List<CharacterProfile> CpuPlayerSelectedData { get => cpuPlayerSelectedData; set => cpuPlayerSelectedData = value; }

    bool CharacterProfileTableExists = false;
    bool CharacterProfileTableCreated = false;

    [SerializeField] internal bool playerDataLoaded = false;
    [SerializeField] internal bool cpuPlayerDataLoaded = false;
    [SerializeField] internal bool cheerleaderDataLoaded = false;
    [SerializeField] internal bool levelDataLoaded = false;
    [SerializeField] internal bool modeDataLoaded = false;

    private bool CheerleaderProfileTableExists;
    private bool CheerleaderProfileTableCreated;

    public static LoadManager instance;

    void Awake()
    {
        instance = this;

        LoadAllData();
    }

    private void Update()
    {
        // load start screen
        if (LoadedData.instance.DataLoaded)
        {
            // this is all confusing
            if (String.IsNullOrEmpty(GameOptions.previousSceneName))
            {
                SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
            }
            // go back to update manager
            else
            {
                SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
            }
        }
    }

    public void LoadAllData()
    {
        //yield return new WaitUntil(() => GameOptions.architectureInfoLoaded == true);
        //Debug.Log("LoadAllData : architectureInfoLoaded : " + GameOptions.architectureInfoLoaded);
        StartCoroutine(verifyCharacterProfileTable());
        StartCoroutine(verifyCheerleaderProfileTable());
        StartCoroutine(LoadGameData());
    }

    IEnumerator verifyCharacterProfileTable()
    {
        yield return new WaitUntil(() => DBConnector.instance.DatabaseCreated == true);

        // if CharacterProfile table does exist
        if (DBConnector.instance.tableExists("CharacterProfile")
            && !DBHelper.instance.isTableEmpty("CharacterProfile"))
        {
            CharacterProfileTableExists = true;
        }
        // if CharacterProfile table doesnt exist, create table
        else
        {
            // drop table just in case of error
            StartCoroutine(DBConnector.instance.dropDatabaseTable("CharacterProfile"));
            //create table
            StartCoroutine(DBConnector.instance.CreateTableCharacterProfile());
            CharacterProfileTableCreated = true;
        }
    }

    IEnumerator verifyCheerleaderProfileTable()
    {
        yield return new WaitUntil(() => DBConnector.instance.DatabaseCreated == true);

        if (DBConnector.instance.tableExists("CheerleaderProfile")
            && !DBHelper.instance.isTableEmpty("CheerleaderProfile"))
        {
            CheerleaderProfileTableExists = true;
        }
        // if CharacterProfile table doesnt exist, create table
        else
        {
            // drop table just in case of error
            StartCoroutine(DBConnector.instance.dropDatabaseTable("CheerleaderProfile"));
            //create table
            StartCoroutine( DBConnector.instance.CreateTableCheerleaderProfile());
            CheerleaderProfileTableCreated = true;
        }

    }


    IEnumerator LoadGameData()
    {
        yield return new WaitUntil(() => DBConnector.instance.DatabaseCreated == true);

        // insert default player profiles + table did not already exits
        if (CharacterProfileTableCreated && !CharacterProfileTableExists)
        {
            playerSelectedData = loadDefaultPlayerShooterProfiles();
            StartCoroutine(DBHelper.instance.InsertCharacterProfile(playerSelectedData));
        }
        //table already exists + does NOT require default records
        if (!CharacterProfileTableCreated && CharacterProfileTableExists)
        {
            playerSelectedData = loadPlayerSelectDataList();
        }
        // =============================================================================
        // insert default cheerleader profiles + table did not already exits
        if (CheerleaderProfileTableCreated && !CheerleaderProfileTableExists)
        {
            // load default data from prefabs
            cheerleaderSelectedData = loadDefaultCheerleaderProfiles();
            // insert default into DB
            StartCoroutine(DBHelper.instance.InsertCheerleaderProfile(cheerleaderSelectedData));
        }

        //table already exists + does NOT require default records
        if (!CheerleaderProfileTableCreated && CheerleaderProfileTableExists)
        {
            cheerleaderSelectedData = loadCheerleaderSelectDataList();
        }
        //cheerleaderSelectedData = loadDefaultCheerleaderProfiles();
        cpuPlayerSelectedData = loadCpuSelectDataList();
        levelSelectedData = loadLevelSelectDataList();
        modeSelectedData = loadModeSelectDataList();
    }

    IEnumerator InsertNewCharacterToDB(CharacterProfile character)
    {
        yield return new WaitUntil(() => DBHelper.instance.DatabaseLocked == false);
        DBHelper.instance.InsertCharacterProfile(character);
    }

    IEnumerator InsertNewCheerleaderToDB(CheerleaderProfile cheerleader)
    {
        yield return new WaitUntil(() => DBHelper.instance.DatabaseLocked == false);
        DBHelper.instance.InsertCheerleaderProfile(cheerleader);

        Debug.Log("cheerleader record inserted");
    }

    private List<CharacterProfile> loadPlayerSelectDataList()
    {
        List<CharacterProfile> dbShootStatsList = DBHelper.instance.getCharacterProfileStats(GameOptions.userid);
        List<CharacterProfile> shooterList = new List<CharacterProfile>();

        string path = "Prefabs/menu_start/player_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            /*
             * if prefab is not in DB, insert
             * ex. create new character, need to auto insert into db
             */
            CharacterProfile temp = obj.GetComponent<CharacterProfile>();

            // if character not in database, but prefab exists -- insert into DB and add to list
            if (!dbShootStatsList.Any(item => item.PlayerId == temp.PlayerId))
            {
                //isLocked = true;
                // get default profile for chracter to be inserted
                string defaultPath = "Prefabs/menu_start/default_shooter_profiles/player_selected_" + temp.PlayerObjectName;
                CharacterProfile defaultTemp = Resources.Load<GameObject>(defaultPath).GetComponent<CharacterProfile>();
                // insert to DB
                StartCoroutine(InsertNewCharacterToDB(defaultTemp));
                // add to current list to be loaded
                dbShootStatsList.Add(temp);
            }

            // load stats from DB, but load portrait from prefab
            temp.Accuracy2Pt = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Accuracy2Pt;
            temp.Accuracy3Pt = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Accuracy3Pt;
            temp.Accuracy4Pt = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Accuracy4Pt;
            temp.Accuracy7Pt = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Accuracy7Pt;
            temp.Speed = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Speed;
            temp.RunSpeed = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).RunSpeed;
            temp.RunSpeedHasBall = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).RunSpeedHasBall;
            temp.Luck = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Luck;
            temp.ShootAngle = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).ShootAngle;
            temp.Experience = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Experience;
            temp.Level = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Level;
            temp.PointsAvailable = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).PointsAvailable;
            temp.PointsUsed = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).PointsUsed;
            temp.Range = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Range;
            temp.Release = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).Release;
            temp.IsLocked = dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).IsLocked;

            shooterList.Add(temp);

        }
        // sort list by  character id
        shooterList.Sort(sortByPlayerId);

        if (shooterList.Count == objects.Length)
        {
            playerDataLoaded = true;
        }
        return shooterList;
    }

    private List<CharacterProfile> loadCpuSelectDataList()
    {
        List<CharacterProfile> shooterList = new List<CharacterProfile>();

        string path = "Prefabs/menu_start/cpu_players_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];
        foreach (GameObject obj in objects)
        {
            CharacterProfile temp = obj.GetComponent<CharacterProfile>();
            if (temp.isCpu)
            {
                temp.intializeCpuShooterStats();
            }
            shooterList.Add(temp);
        }
        // sort list by  character id
        shooterList.Sort(sortByPlayerId);

        if (shooterList.Count == objects.Length)
        {
            cpuPlayerDataLoaded = true;
        }

        return shooterList;
    }


    private List<CheerleaderProfile> loadDefaultCheerleaderProfiles()
    {
        List<CheerleaderProfile> cheerList = new List<CheerleaderProfile>();

        string path = "Prefabs/menu_start/cheerleader_default_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            CheerleaderProfile temp = obj.GetComponent<CheerleaderProfile>();
            cheerList.Add(temp);
        }
        // sort list by  character id
        cheerList.Sort(sortByCheerleaderId);

        //Debug.Log("***************************  cheerList.Count : " + cheerList.Count + "   objects.Length : " + objects.Length);

        if (cheerList.Count == objects.Length)
        {
            cheerleaderDataLoaded = true;
        }
        return cheerList;
    }

    private List<CharacterProfile> loadDefaultPlayerShooterProfiles()
    {
        List<CharacterProfile> shooterList = new List<CharacterProfile>();

        string path = "Prefabs/menu_start/default_shooter_profiles";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            CharacterProfile temp = obj.GetComponent<CharacterProfile>();
            shooterList.Add(temp);
        }
        // sort list by  character id
        shooterList.Sort(sortByPlayerId);

        if (shooterList.Count == objects.Length)
        {
            playerDataLoaded = true;
        }

        return shooterList;
    }

    private List<CheerleaderProfile> loadCheerleaderSelectDataList()
    {
        List<CheerleaderProfile> dbCheerList = DBHelper.instance.getCheerleaderProfileStats();
        List<CheerleaderProfile> cheerList = new List<CheerleaderProfile>();

        string path = "Prefabs/menu_start/cheerleader_selected_object";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            CheerleaderProfile temp = obj.GetComponent<CheerleaderProfile>();
            cheerList.Add(temp);
            // need to create a copy to keep prefab from changing

            //// if character not in database, but prefab exists -- insert into DB and add to list
            //if (!dbCheerList.Any(item => item.CheerleaderId == temp.CheerleaderId))
            //{
            //    //isLocked = true;
            //    // get default profile for chracter to be inserted
            //    string defaultPath = "Prefabs/menu_start/cheerleader_default_objects/cheerleader_selected_"
            //        + temp.CheerleaderId.ToString("00") + "_" + temp.CheerleaderObjectName;

            //    Debug.Log("defaultPath : " + defaultPath);

            //    CheerleaderProfile defaultTemp = Resources.Load<GameObject>(defaultPath).GetComponent<CheerleaderProfile>();
            //    //// insert to DB
            //    //StartCoroutine(InsertNewCheerleaderToDB(defaultTemp));
            //    // add to current list to be loaded
            //    dbCheerList.Add(temp);
            //}

            //// load stats from DB, but load portrait from prefab
            //temp.CheerleaderId = dbCheerList.Find(x => x.CheerleaderId == temp.CheerleaderId).CheerleaderId;
            //temp.CheerleaderDisplayName = dbCheerList.Find(x => x.CheerleaderId == temp.CheerleaderId).CheerleaderDisplayName;
            //temp.CheerleaderObjectName = dbCheerList.Find(x => x.CheerleaderId == temp.CheerleaderId).CheerleaderObjectName;
            //temp.UnlockCharacterText = dbCheerList.Find(x => x.CheerleaderId == temp.CheerleaderId).UnlockCharacterText;

            //temp.IsLocked = dbCheerList.Find(x => x.CheerleaderId == temp.CheerleaderId).IsLocked;
            /*
             * Portrait should already be loaded from prefab
             */

            //cheerList.Add(temp);
            //if (!temp.IsLocked)
            //{
            //    Debug.Log("--------------------------------- ADD CHEER DB DATA TO PREFAB--------------------------");
            //    Debug.Log("cheer unlock text :: " + temp.UnlockCharacterText + "  islocked :: " + temp.IsLocked);
            //}
        }
        // sort list by  character id
        cheerList.Sort(sortByCheerleaderId);

        if (cheerList.Count == objects.Length)
        {
            cheerleaderDataLoaded = true;
        }

        return cheerList;
    }

    private List<StartScreenLevelSelected> loadLevelSelectDataList()
    {
        List<StartScreenLevelSelected> levelList = new List<StartScreenLevelSelected>();

        string path = "Prefabs/menu_start/level_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            StartScreenLevelSelected temp = obj.GetComponent<StartScreenLevelSelected>();
            levelList.Add(temp);
        }

        // sort list by  level id
        levelList.Sort(sortByLevelId);

        if (levelList.Count == objects.Length)
        {
            levelDataLoaded = true;
        }

        return levelList;
    }

    private List<StartScreenModeSelected> loadModeSelectDataList()
    {
        List<StartScreenModeSelected> modeList = new List<StartScreenModeSelected>();

        string path = "Prefabs/menu_start/mode_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            StartScreenModeSelected temp = obj.GetComponent<StartScreenModeSelected>();
            modeList.Add(temp);
        }
        // sort list by  mode id
        modeList.Sort(sortByModeId);

        if (modeList.Count == objects.Length)
        {
            modeDataLoaded = true;
        }

        return modeList;
    }

    static int sortByPlayerId(CharacterProfile p1, CharacterProfile p2)
    {
        return p1.PlayerId.CompareTo(p2.PlayerId);
    }

    static int sortByCheerleaderId(CheerleaderProfile p1, CheerleaderProfile p2)
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


    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }
}

