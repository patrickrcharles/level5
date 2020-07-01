using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Editor;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    private List<Achievement> achievementList; // dictionary of achievements

    const string notificationObjectName = "achievement_notification_object";
    const string notificationTextObjectName = "achievement_text";
    const string notificationImageObjectName = "achievement_background";

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
            AchievementList.Add(temp);
        }
        ListCreated = true;
    }

    public void checkAllAchievements(int pid, int cid, int lid, int mid, int activateValue, BasketBallStats basketBallStats)
    {
        foreach (Achievement ach in AchievementList)
        {
            ach.checkUnlockConditions(pid, cid, lid, mid, activateValue, basketBallStats);
            if (!ach.IsLocked)
            {
                Debug.Log(" unlock achievement");
                StartCoroutine(displayAchievementUnlockNotification(5, ach.achievementName));
            }
        }
    }

    public IEnumerator displayAchievementUnlockNotification(float seconds, string achName)
    {
        //GameObject messageObject = AchievementManager.instance.NotificationObject;
        //Text messageText = AchievementManager.instance.NotificationObjectText;
        //Image messageText = AchievementManager.instance.NotificationObjectText;

        notificationObject.SetActive(true);
        yield return new WaitUntil(() => notificationObject.activeInHierarchy == true);

        notificationObjectText.text = "Achievement unlocked : \n" + achName;

        yield return new WaitForSecondsRealtime(seconds);

        notificationObjectText.text = "";
        notificationObject.SetActive(false);

    }

    public GameObject NotificationObject { get => notificationObject; set => notificationObject = value; }
    public Text NotificationObjectText { get => notificationObjectText; set => notificationObjectText = value; }
    public List<Achievement> AchievementList { get => achievementList; set => achievementList = value; }
    public bool ListCreated { get => listCreated; set => listCreated = value; }
}
