using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UserAccountManager : MonoBehaviour
{

    [SerializeField]
    private List<UserModel> userAccountData;

    private bool usersLoaded = false;
    const int guestUserid = 74;
    const string guestPassword = "guest";

    [SerializeField]
    GameObject localAccountPrefab;
    [SerializeField]
    GameObject localAccountPrefabSpawnLocation;
    [SerializeField]
    List<GameObject> localAccounsList;
    [SerializeField]
    string userNameSelected;
    [SerializeField]
    Text messageText;

    PlayerControls controls;
    public static UserAccountManager instance;

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
    }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        if (!SceneManager.GetActiveScene().name.Equals(SceneNameConstants.SCENE_NAME_level_00_loading))
        {
            StartCoroutine(loadUserData());
        }
        controls = new PlayerControls();
    }

    void Update()
    {

        //currentSelectedButton = EventSystem.current.currentSelectedGameObject.name;

        // based on login button selected, get sibling text name. 
        // the sibling object text is a USERNAME stored locally and loaded into list
        if (!SceneManager.GetActiveScene().name.Equals(SceneNameConstants.SCENE_NAME_level_00_start))
        {
            if (EventSystem.current.currentSelectedGameObject != null )//&& usersLoaded)
            {
                userNameSelected = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0).GetComponent<Text>().text;
            }
            if (controls.UINavigation.Submit.triggered)
            {
                Debug.Log("loginbutton");
                if (EventSystem.current.currentSelectedGameObject.name.Equals("userAccountLoginButton"))
                {
                    Debug.Log("loginbutton");
                    LoginButton();
                }
                if (EventSystem.current.currentSelectedGameObject.name.Equals("userAccountRemoveButton"))
                {
                    Debug.Log("loginbutton");
                    RemoveUserButton();
                }
                if (EventSystem.current.currentSelectedGameObject.name.Equals("continueButton"))
                {
                    Debug.Log("loginbutton");
                    GameOptions.userName = null;
                    GameOptions.userid = 0;
                    ContinueButton();
                }
            }
        }
    }

    public void LoginButton()
    {
        UserModel user = new UserModel();

        GameOptions.userName = userNameSelected;
        if (usersLoaded)
        {
            user = userAccountData.Where(x => x.UserName == userNameSelected).Single();
            GameOptions.userid = user.Userid;
        }
        else
        {
            user.Userid = guestUserid;
            user.UserName = userNameSelected;
            user.Password = "guest";
        }

        // if connected to internet
        if (UtilityFunctions.IsConnectedToInternet())
        {
            StartCoroutine(APIHelper.PostToken(user));
        }
        else
        {
            SceneManager.LoadScene(SceneNameConstants.SCENE_NAME_level_00_loading);
        }
    }

    public void ContinueButton()
    {
        Debug.Log("ContinueButton");
        GameOptions.userName = "";
        GameOptions.userid = 0;
        SceneManager.LoadScene(SceneNameConstants.SCENE_NAME_level_00_loading);
    }

    public void RemoveUserButton()
    {
        // remove user from local database
        // reload login scene
        DBHelper.instance.deleteLocalUser(userNameSelected);
        SceneManager.LoadScene(SceneNameConstants.SCENE_NAME_level_00_login);

    }

    IEnumerator loadUserData()
    {
        yield return new WaitUntil(() => DBHelper.instance != null);
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

        try
        {
            userAccountData = DBHelper.instance.getUserProfileStats();
            GameOptions.numOfLocalUsers = userAccountData.Count;

            //Debug.Log("GameOptions.numOfLocalUsers : " + GameOptions.numOfLocalUsers);
            //Debug.Log("userAccountData.Count : " + userAccountData.Count);

            if (userAccountData.Count > 0)
            {
                usersLoaded = true;
                if (messageText != null)
                {
                    messageText.text = "select user to log in";
                }
                DBHelper.instance.DatabaseLocked = false;
            }
            else
            {
                usersLoaded = false;
                if (messageText != null)
                {
                    messageText.text = "no users found";
                }
                DBHelper.instance.DatabaseLocked = false;
            }
            StartCoroutine(CreateUserButtons());
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            usersLoaded = false;
            DBHelper.instance.DatabaseLocked = false;
            messageText.text = e.ToString();
            StartCoroutine(CreateUserButtons());
        }
        //StartCoroutine(CreateUserButtons());
    }
    IEnumerator CreateUserButtons()
    {
        yield return new WaitUntil(() => DBHelper.instance != null);
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

        // for each, create empty object
        // add text + button that is clickable

        // flag for testing
        //usersLoaded = false;

        int index = 0;
        if (usersLoaded)
        {
            foreach (UserModel u in userAccountData)
            {
                // instantiate a max of 5 rows
                if (index < 10)
                {
                    GameObject prefabClone =
                    Instantiate(localAccountPrefab, localAccountPrefabSpawnLocation.transform.position, Quaternion.identity);
                    // set parent to object with vertical layout
                    prefabClone.transform.SetParent(localAccountPrefabSpawnLocation.transform, false);
                    // add to list
                    localAccounsList.Add(prefabClone);
                    //set text
                    localAccounsList[index].GetComponentInChildren<Text>().text = u.UserName;
                }
                index++;
            }
            //EventSystem.current.SetSelectedGameObject(GameObject.FindObjectOfType<Button>().gameObject);
        }
        else
        {
            //GameObject prefabClone =
            //    Instantiate(localAccountPrefab, localAccountPrefabSpawnLocation.transform.position, Quaternion.identity);
            //prefabClone.transform.SetParent(localAccountPrefabSpawnLocation.transform, false);
            //// get username text
            //prefabClone.transform.Find("userAccount").GetComponent<Text>().text = "no local user account found" +
            //    "\ngo to account and create one";
            //prefabClone.transform.Find("userAccountLoginButton").GetComponent<Text>().text = "continue";
            ////EventSystem.current.SetSelectedGameObject(GameObject.FindObjectOfType<Button>().gameObject);
            UserModel u = new UserModel();
            u.UserName = "guest";
            u.Password = "guest";

            GameObject prefabClone =
                Instantiate(localAccountPrefab, localAccountPrefabSpawnLocation.transform.position, Quaternion.identity);
            // set parent to object with vertical layout
            prefabClone.transform.SetParent(localAccountPrefabSpawnLocation.transform, false);
            // add to list
            localAccounsList.Add(prefabClone);
            //set text
            localAccounsList[index].GetComponentInChildren<Text>().text = u.UserName;
        }
    }
    public List<UserModel> UserAccountData { get => userAccountData; }
    public bool UsersLoaded { get => usersLoaded; set => usersLoaded = value; }

    public static int GuestUserid => guestUserid;
    public static string GuestPassword => guestPassword;
}