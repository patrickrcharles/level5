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
    public bool achievementsLoaded;

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
        // DB object exists
        if (GameObject.FindGameObjectWithTag("database") != null )
        {
            //Debug.Log(" database object exists ");

            //if LoadManager is through loading data lists
            // should be in a coroutine with a waituntil this is true
            try
            {
                // achievement table exists
                if (DBConnector.instance.tableExists("Achievements"))
                {
                    //LoadAchievements();
                }
                else
                {
                    // create achievement table
                    // retry load
                    //Debug.Log("Achievements table DOESNT exists ");
                    //DBConnector.instance.createTableAchievements();
                    //LoadAchievements();
                }
            }
            catch
            {
                Debug.Log(" ERROR : Loading Achievements");
                return;
            }
            achievementsLoaded = true;
        }
    }

    public void LoadAchievements()
    {
        Debug.Log("LoadAchievements()");

        loadAchievementsFromPrefabs();
        resolveAchievementConflicts();
        checkAllAchievementsPreLoad();
        //checkAllAchievements();

        //updatePlayerUnlock();
    }

    private void updatePlayerUnlock()
    {
        //Debug.Log("updatePlayerUnlock()");
        //yield return new WaitUntil(() => LoadedData.instance.DataLoaded == true);
        //int index = 0;
        foreach (CharacterProfile c in LoadedData.instance.PlayerSelectedData)
        {
            if (achievementList.Find(x => x.PlayerId == c.PlayerId) != null)
            {
                Achievement tempAchieve = achievementList.Find(x => x.PlayerId == c.PlayerId);
                c.UnlockCharacterText = tempAchieve.AchievementDescription;
                //Debug.Log("     character.name : " + c.PlayerDisplayName);
                //Debug.Log("     character.islocked : " + c.IsLocked);
                //Debug.Log("     achieve.islocked : " + tempAchieve.IsLocked);
                if (tempAchieve.IsLocked != c.IsLocked)
                {
                    c.IsLocked = tempAchieve.IsLocked;
                    //DBHelper.instance.UpdateAchievementStats();
                    DBHelper.instance.updateIntValueFromTableByFieldAndId("CharacterProfile", "isLocked", 0, "charid", c.PlayerId);
                }
            }
        }
        foreach (CheerleaderProfile c in LoadedData.instance.CheerleaderSelectedData)
        {
            if (achievementList.Find(x => x.PlayerId == c.CheerleaderId) != null)
            {
                Achievement tempAchieve = achievementList.Find(x => x.PlayerId == c.CheerleaderId);
                c.UnlockCharacterText = tempAchieve.AchievementDescription;
                //Debug.Log("     character.name : " + c.PlayerDisplayName);
                //Debug.Log("     character.islocked : " + c.IsLocked);
                //Debug.Log("     achieve.islocked : " + tempAchieve.IsLocked);
                if (tempAchieve.IsLocked != c.IsLocked)
                {
                    c.IsLocked = tempAchieve.IsLocked;
                    //DBHelper.instance.UpdateAchievementStats();
                    DBHelper.instance.updateIntValueFromTableByFieldAndId("CheerleaderProfile", "isLocked", 0, "cid", c.CheerleaderId);
                }
            }
            Debug.Log("**********************************************************************");
            Debug.Log("     cheer name : "+ c.CheerleaderDisplayName + "  islocked : "+ c.IsLocked);
        }
        //achievementsLoaded = true;
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
            Achievement prefab = obj.GetComponent<Achievement>();
            Achievement temp = copyAchievement(prefab);
            AchievementList.Add(temp);
        }
    }

    private Achievement copyAchievement(Achievement prefab)
    {
        Achievement temp = new Achievement();
        temp = prefab;

        return temp;
    }

    void resolveAchievementConflicts()
    {
        databaseAchieveList = DBHelper.instance.loadAchievementsFromDB();
        Debug.Log("resolveAchievementConflicts()");
        //foreach(Achievement a in databaseAchieveList)
        //{

        //        Debug.Log("==================================== DATABASE ACHIEVEMENT LIST=============================");
        //        Debug.Log("     name : " + a.AchievementName + "     islocked : " + a.IsLocked);
        //}
        //Debug.Log("===========================================================================================");

        // check DB contains all Prefab Achievements
        foreach (Achievement a in achievementList)
        {
            // database contains prefab achievement
            // load progress/locked data
            if (databaseAchieveList.Any(x => x.achievementId == a.achievementId))
            {
                a.ActivationValueProgressionInt = databaseAchieveList.Find(x => x.achievementId == a.achievementId).ActivationValueProgressionInt ;
                a.IsLocked = databaseAchieveList.Find(x => x.achievementId == a.achievementId).IsLocked;
                if (!a.IsLocked)
                {
                    //Debug.Log(" ----------------- COPY DB PROGRESSION + ISLOCKED to PREFAB -------------------------------------------------");
                    Debug.Log(" name : " + a.achievementName + " islocked : " + a.IsLocked);
                }
                continue;
            }
            // database does NOT contains prefab achievement
            else
            {
                //Debug.Log(" ----------------- NEW PREFAB ACHIEVEMENT -- INSERT TO DB  -------------------------------------------------");
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
                Debug.Log(" ----------------- DB ACHIEVEMENT DOESNOT EXISTS IN PREFAB  -------------------------------------------------");
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

        //Debug.Log("---------------------------checkAllAchievements()");
        foreach (Achievement ach in AchievementList)
        {
            //Debug.Log("Achievement : " + ach.achievementName + " unlocked : " + ach.IsLocked);
            // if achievement is locked
            if (ach.IsLocked)
            {
                //Debug.Log("if (ach.IsLocked) ");
                ach.checkUnlockConditions( pid, cid, lid, mid, activateValue);

                // if achievement is unlocked
                if (!ach.IsLocked)
                {
                    //achieveUnlockedList.Add(ach.achievementName);
                    achText += "\n" + ach.achievementName;
                    achUnlocked = true;
                    // update database
                    DBHelper.instance.updateIntValueFromTableByFieldAndId("Achievements", "islocked", 0, "aid", ach.achievementId);
                }
            }
            //Debug.Log("     checkAllAchievements :  ach.name : " + ach.name + "\nach.islocked : "+ ach.IsLocked);
        }
        if (achUnlocked)
        {
            //Debug.Log("if (achUnlocked)");
            StartCoroutine(displayAchievementUnlockNotification(3, achText));
        }

        // update db values
        //DBConnector.instance.saveAchievementStats();
    }

    public void checkAllAchievementsPreLoad()
    {
        Debug.Log("------------------------------------------------------------------- checkAllAchievementsPreLoad()");
        string achText = "";
        bool achUnlocked = false;
        foreach (Achievement a in databaseAchieveList)
        {
            //Debug.Log("Achievement : " + a.achievementName + " islocked : " + a.IsLocked + " activate value : "+ a.ActivationValueInt);
            // if achievement is locked
            if (a.IsLocked)
            {
                a.checkUnlockConditions(a.PlayerId, a.CheerleaderId, a.LevelId, a.ModeId, 0);

                // if achievement is unlocked
                if (!a.IsLocked)
                {
                    achText += "\n" + a.achievementName;
                    achUnlocked = true;
                    Debug.Log("******************************************  Unlocked : " + a.achievementName );
                    // update database
                    //DBHelper.instance.updateIntValueFromTableByFieldAndAchievementID("Achievements", "islocked", 0, ach.achievementId);
                    DBHelper.instance.updateIntValueFromTableByFieldAndId("Achievements", "islocked", 0, "aid", a.achievementId);
                }
            }
            //Debug.Log("     checkAllAchievements :  ach.name : " + ach.name + "\nach.islocked : " + ach.IsLocked);

            /*
             * if player unlocked
             * update character profile databse
             * 
             * if cheerleader unlocked
             * update cheerleader profile databse
             */

        }
        if (achUnlocked)
        {
            StartCoroutine(displayAchievementUnlockNotification(3, achText));
        }
        // update db values
        //DBConnector.instance.saveAchievementStats();
        //achievementsLoaded = true;
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
