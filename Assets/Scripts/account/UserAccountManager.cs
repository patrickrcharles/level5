using Assets.Scripts.database;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserAccountManager : MonoBehaviour
{
    [SerializeField]
    private List<DBUserModel> userAccountData;

    public static UserAccountManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        StartCoroutine(loadUserData());
    }

    IEnumerator loadUserData()
    {
        yield return new WaitUntil(() => DBHelper.instance != null);
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

        userAccountData = DBHelper.instance.getUserProfileStats();

        yield return new WaitUntil(() => GameObject.FindObjectOfType<GameOptions>() != null);

        if (userAccountData.Count > 0)
        {
            GameOptions.userName = userAccountData[1].UserName;
        }
        else
        {
            GameOptions.userName = "No User Accounts found. Create an account";
        }
    }
    public List<DBUserModel> UserAccountData { get => userAccountData; }
}
