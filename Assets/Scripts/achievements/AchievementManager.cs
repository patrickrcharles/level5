using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    [SerializeField]
    private List<Achievement> achievementList; // achievements from prefabs

    [SerializeField]
    List<Achievement> databaseAchieveList;  // achievements from DB

    const string notificationObjectName = "achievement_notification_object";
    const string notificationTextObjectName = "achievement_text";
    const string notificationImageObjectName = "achievement_background";

    bool wait;

    [SerializeField]
    GameObject notificationObject;
    Text notificationObjectText;
    Image notificationObjectImage;

    public static AchievementManager instance;
    internal bool achievementsLoaded;

    void Awake()
    {
        //instance = this;
        if (AchievementList == null)
        {
            AchievementList = new List<Achievement>();
        }
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        NotificationObject = GameObject.Find(notificationObjectName);
        NotificationObjectText = GameObject.Find(notificationTextObjectName).GetComponent<Text>();
        notificationObjectImage = GameObject.Find(notificationImageObjectName).GetComponent<Image>();
        NotificationObject.SetActive(false);

        /*
         * check if achievement table exists
         * check if empty
         * if load default values
         * else load into list
         */
        Debug.Log(" Achievement MAnager awake ");
        // DB object exists
        if (GameObject.FindGameObjectWithTag("database") != null)
        {
            Debug.Log(" database object exists ");
            // if LoadManager is through loading data lists
            // should be in a coroutine with a waituntil this is true
            try
            {
                // achievement table exists
                if (DBConnector.instance.tableExists("Achievements"))
                {
                    StartCoroutine(LoadAchievements());
                }
                else
                {
                    // create achievement table
                    // retry load
                    DBConnector.instance.createTableAchievements();
                    StartCoroutine(LoadAchievements());
                }
            }
            catch
            {
                // on error
                return;
            }
        }
    }

    IEnumerator LoadAchievements()
    {
        // wait until load manager is finished
        yield return new WaitUntil(() => LoadedData.instance.DataLoaded == true);
        loadAchievementsFromPrefabs();
        resolveAchievementConflicts();
        //checkAllAchievementsPreLoad();
        StartCoroutine( updatePlayerUnlockText() );
    }

    IEnumerator updatePlayerUnlockText()
    {
        yield return new WaitUntil(() => LoadedData.instance.DataLoaded == true);

        //int index = 0;
        foreach (CharacterProfile c in LoadedData.instance.PlayerSelectedData)
        {
            if (achievementList.Find(x => x.PlayerId == c.PlayerId) != null)
            {
                Achievement tempAchieve = achievementList.Find(x => x.PlayerId == c.PlayerId);
                c.UnlockCharacterText = tempAchieve.AchievementDescription;
            }
        }
        achievementsLoaded = true;
    }

    void loadAchievementsFromPrefabs()
    {
        Debug.Log("loadAchievementsFromPrefabs() ");
        // where are the prefabs, load them
        string path = "Prefabs/achievements";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        // get all the objects in folder, create list of the vehicleControllers
        foreach (GameObject obj in objects)
        {
            Achievement temp = obj.GetComponent<Achievement>();
            AchievementList.Add(temp);
        }
    }

    void resolveAchievementConflicts()
    {
        databaseAchieveList = DBHelper.instance.loadAchievementsFromDB();

        // check DB contains all Prefab Achievements
        foreach (Achievement a in achievementList)
        {
            // database contains prefab achievement
            // load progress/locked data
            if (databaseAchieveList.Any(x => x.achievementId == a.achievementId))
            {
                a.ActivationValueProgressionInt = databaseAchieveList.Find(x => x.achievementId == a.achievementId).ActivationValueProgressionInt ;
                a.IsLocked = databaseAchieveList.Find(x => x.achievementId == a.achievementId).IsLocked;

                continue;
            }
            // database does NOT contains prefab achievement
            else
            {
                //insert into database
                // add to databaselist
                DBHelper.instance.insertNewAchievmentInDB(a);
                databaseAchieveList.Add(a);
            }
        }
        // check DB does NOT contain Achievements that are not in prefab folder
        foreach (Achievement a in databaseAchieveList)
        {
            if (achievementList.Any(x => x.achievementId == a.achievementId))
            {
                continue;
            }
            else
            {
                // delete achievement from DB
                DBHelper.instance.deleteRecordFromTableByID("Achievements", "aid", a.achievementId);
            }
        }
    }

    // this will be getting a major rewrite
    public void checkAllAchievements(int pid, int cid, int lid, int mid, int activateValue)
    {
        string achText = "";
        bool achUnlocked = false;
        foreach (Achievement ach in AchievementList)
        {
            // if achievement is locked
            if (ach.IsLocked)
            {
                ach.checkUnlockConditions(pid, cid, lid, mid, activateValue);

                // if achievement is unlocked
                if (!ach.IsLocked)
                {
                    //achieveUnlockedList.Add(ach.achievementName);
                    achText += "\n" + ach.achievementName;
                    achUnlocked = true;
                    // update database
                    DBHelper.instance.updateIntValueFromTableByFieldAndAchievementID("Achievements", "islocked", 0, ach.achievementId);
                }
            }
        }
        if (achUnlocked)
        {
            StartCoroutine(displayAchievementUnlockNotification(5, achText));
        }

        // update db values
        DBConnector.instance.saveAchievementStats();
    }

    public void checkAllAchievementsPreLoad()
    {
        string achText = "";
        bool achUnlocked = false;
        foreach (Achievement ach in databaseAchieveList)
        {
            // if achievement is locked
            if (ach.IsLocked)
            {
                ach.checkUnlockConditions(ach.PlayerId, ach.CheerleaderId, ach.LevelId , ach.ModeId, ach.ActivationValueInt );

                // if achievement is unlocked
                if (!ach.IsLocked)
                {
                    achText += "\n" + ach.achievementName;
                    achUnlocked = true;
                    // update database
                    DBHelper.instance.updateIntValueFromTableByFieldAndAchievementID("Achievements", "islocked", 0, ach.achievementId);
                    Debug.Log("ach : "+ ach.achievementName + " unlocked");
                }
            }
        }
        if (achUnlocked)
        {
            StartCoroutine(displayAchievementUnlockNotification(5, achText));
        }
        // update db values
        DBConnector.instance.saveAchievementStats();
        achievementsLoaded = true;
    }

    public IEnumerator displayAchievementUnlockNotification(float seconds, string achText)
    {
        if (SFXBB.instance != null)
        {
            SFXBB.instance.playSFX(SFXBB.instance.unlockAchievement);
        }

        //wait = true;
        yield return new WaitUntil(() => wait == false);

        //yield return new WaitUntil(() => wait == true);

        notificationObject.SetActive(true);
        yield return new WaitUntil(() => notificationObject.activeInHierarchy == true);

        notificationObjectText.text = "Achievement unlocked : " + achText;

        yield return new WaitForSecondsRealtime(seconds);

        notificationObjectText.text = "";
        notificationObject.SetActive(false);
    }

    public GameObject NotificationObject { get => notificationObject; set => notificationObject = value; }
    public Text NotificationObjectText { get => notificationObjectText; set => notificationObjectText = value; }
    public List<Achievement> AchievementList { get => achievementList; set => achievementList = value; }
    public bool ListCreated { get => achievementsLoaded; set => achievementsLoaded = value; }
    public bool Wait { get => wait; set => wait = value; }
}
