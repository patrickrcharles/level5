
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;

    //list of all shooter profiles with player data
    [SerializeField]
    private List<ShooterProfile> playerSelectedData;
    public List<ShooterProfile> PlayerSelectedData { get => playerSelectedData; }
    // list off cheerleader profile data
    [SerializeField]
    private List<StartScreenCheerleaderSelected> cheerleaderSelectedData;
    public List<StartScreenCheerleaderSelected> CheerleaderSelectedData { get => cheerleaderSelectedData; }
    // list off level  data
    [SerializeField]
    private List<StartScreenLevelSelected> levelSelectedData;
    public List<StartScreenLevelSelected> LevelSelectedData { get => levelSelectedData; }

    //mode selected objects
    [SerializeField]
    private List<StartScreenModeSelected> modeSelectedData;
    public List<StartScreenModeSelected> ModeSelectedData { get => modeSelectedData; }

    private int playerSelectedIndex;
    private int levelSelectedIndex;
    private int modeSelectedIndex;
    private int cheerleaderSelectedIndex;

    bool CharacterProfileTableExists = false;
    bool CharacterProfileTableCreated = false;

    public static LoadManager instance;

    internal bool playerDataLoaded = false;
    internal bool cheerleaderDataLoaded = false;
    internal bool levelDataLoaded = false;
    internal bool modeDataLoaded;

    const string startSceneName = "level_00_start";

    //private Text gameModeSelectText;
    void Awake()
    {
        instance = this;

        // if CharacterProfile table does exist
        if (DBConnector.instance.tableExists("CharacterProfile"))
        {
            CharacterProfileTableExists = true;
        }
        // if CharacterProfile table doesnt exist, create table
        else
        {
            // drop table just in case of error
            DBConnector.instance.dropDatabaseTable("CharacterProfile");
            //create table
            DBConnector.instance.createTableCharacterProfile();
            CharacterProfileTableCreated = true;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // insert default player profiles + table did not already exits
        if (CharacterProfileTableCreated && !CharacterProfileTableExists)
        {
            playerSelectedData = loadDefaultPlayerShooterProfiles();
            DBHelper.instance.InsertCharacterProfile(playerSelectedData);
        }
        //table already exists + does NOT require default records
        if (!CharacterProfileTableCreated && CharacterProfileTableExists)
        {
            playerSelectedData = loadPlayerSelectDataList();
        }

        cheerleaderSelectedData = loadCheerleaderSelectDataList();
        levelSelectedData = loadLevelSelectDataList();
        modeSelectedData = loadModeSelectDataList();

    }

    private void Update()
    {
        if (AchievementManager.instance.achievementsLoaded
            && LoadedData.instance.dataLoaded)
        {
            SceneManager.LoadScene(startSceneName);
        }
    }

    private List<ShooterProfile> loadPlayerSelectDataList()
    {
        List<ShooterProfile> dbShootStatsList = DBHelper.instance.getCharacterProfileStats();
        List<ShooterProfile> shooterList = new List<ShooterProfile>();

        string path = "Prefabs/menu_start/player_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            ShooterProfile temp = obj.GetComponent<ShooterProfile>();

            // load stats from DB, but load portrait from prefab
            //dbShootStatsList.Find(x => x.PlayerId == temp.PlayerId).PlayerPortrait = temp.PlayerPortrait;
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

            shooterList.Add(temp);
        }
        // sort list by  character id
        shooterList.Sort(sortByPlayerId);

        playerDataLoaded = true;

        return shooterList;
    }


    private List<ShooterProfile> loadDefaultPlayerShooterProfiles()
    {
        List<ShooterProfile> shooterList = new List<ShooterProfile>();

        string path = "Prefabs/menu_start/default_shooter_profiles";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            ShooterProfile temp = obj.GetComponent<ShooterProfile>();
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

    private List<StartScreenCheerleaderSelected> loadCheerleaderSelectDataList()
    {
        List<StartScreenCheerleaderSelected> cheerList = new List<StartScreenCheerleaderSelected>();

        string path = "Prefabs/menu_start/cheerleader_selected_object";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            StartScreenCheerleaderSelected temp = obj.GetComponent<StartScreenCheerleaderSelected>();
            cheerList.Add(temp);
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


    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

}