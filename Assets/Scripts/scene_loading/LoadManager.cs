
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;

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

    private int playerSelectedIndex;
    private int levelSelectedIndex;
    private int modeSelectedIndex;
    private int cheerleaderSelectedIndex;

    [SerializeField]
    GameObject playerSelectedIsLockedObject;

    [SerializeField]
    GameObject cheerleaderSelectedIsLockedObject;

    bool CharacterProfileTableExists = false;
    bool CharacterProfileTableCreated = false;

    //private Text gameModeSelectText;
    void Awake()
    {
        // if CharacterProfile table does exist
        if (DBConnector.instance.tableExists("CharacterProfile") )
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
            loadDefaultPlayerShooterProfiles();
            DBHelper.instance.InsertCharacterProfile(playerSelectedData);
        }
        //table already exists + does NOT require default records
        if (!CharacterProfileTableCreated && CharacterProfileTableExists)
        {
            Debug.Log("Player profile data already exists");
        }
    }


    private void loadDefaultPlayerShooterProfiles()
    {
        string path = "Prefabs/start_menu/default_shooter_profiles";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            ShooterProfile temp = obj.GetComponent<ShooterProfile>();
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

        //foreach (StartScreenModeSelected s in modeSelectedData)
        //{
        //    Debug.Log(" mode id : " + s.ModeId);
        //}
    }


    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

}