using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    private List<Achievement> achievementList; // dictionary of achievements

    const string notificationObjectName = "achievement_notification_object";
    const string notificationTextObjectName = "achievement_text";
    const string notificationImageObjectName = "achievement_background";

    bool wait;

    [SerializeField]
    GameObject notificationObject;
    Text notificationObjectText;
    Image notificationObjectImage;

    public static AchievementManager instance;
    bool listCreated;

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

        
    }

    private void Start()
    {
        loadAchievements();
    }

    void loadAchievements()
    {
        // where are the prefabs, load them
        string path = "Prefabs/achievements";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        // get all the objects in folder, create list of the vehicleControllers
        foreach (GameObject obj in objects)
        {
            Achievement temp = obj.GetComponent<Achievement>();
            // update with database values
            temp.ActivationValueInt = DBHelper.instance.getIntValueFromTableByFieldAndAchievementID("Achievements", "activevalue_int", temp.achievementId);
            temp.ActivationValueProgressionInt = DBHelper.instance.getIntValueFromTableByFieldAndAchievementID("Achievements", "activevalue_progress_int", temp.achievementId);
            AchievementList.Add(temp);
        }

        ListCreated = true;
    }

    public void checkAllAchievements(int pid, int cid, int lid, int mid, int activateValue, BasketBallStats basketBallStats)
    {
        string achText = "";
        bool achUnlocked = false;
        foreach (Achievement ach in AchievementList)
        {         
            // if achievement is locked
            if (ach.IsLocked)
            {
                ach.checkUnlockConditions(pid, cid, lid, mid, activateValue, basketBallStats);

                // if achievement is unlocked
                if (!ach.IsLocked)
                {
                    //achieveUnlockedList.Add(ach.achievementName);
                    achText += "\n" + ach.achievementName;
                    achUnlocked = true;
                }
            }           
        }
        if (achUnlocked)
        {
            StartCoroutine(displayAchievementUnlockNotification(5, achText));
        }

        //foreach (string s  in achieveUnlockedList)
        //{
        //  StartCoroutine(displayAchievementUnlockNotification(5));
        //}

        // update db values
        DBConnector.instance.saveAchievementStats();
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
        Debug.Log("Achievement unlocked : " + notificationObjectText.text);

        yield return new WaitForSecondsRealtime(seconds);

        notificationObjectText.text = "";
        notificationObject.SetActive(false);

        //wait = false;
    }

    public GameObject NotificationObject { get => notificationObject; set => notificationObject = value; }
    public Text NotificationObjectText { get => notificationObjectText; set => notificationObjectText = value; }
    public List<Achievement> AchievementList { get => achievementList; set => achievementList = value; }
    public bool ListCreated { get => listCreated; set => listCreated = value; }
    public bool Wait { get => wait; set => wait = value; }
}
