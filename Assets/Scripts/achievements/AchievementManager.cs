using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    private List<Achievement> achievementList; // dictionary of achievements
    public List<Achievement> AchievementList { get => achievementList; set => achievementList = value; }
    public bool ListCreated { get => listCreated; set => listCreated = value; }

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

    public void checkAllAchievements(int pid, int lid, int mid, int activateValue)
    {
        foreach (Achievement ach in AchievementList)
        {
            ach.checkCharacterUnlockConditions(pid, lid, mid, activateValue);
        }
    }
}
