using Assets.Scripts.database;
using Assets.Scripts.restapi;
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
    private List<DBUserModel> userAccountData;

    private bool usersLoaded = false;

    [SerializeField]
    GameObject localAccountPrefab;
    [SerializeField]
    GameObject localAccountPrefabSpawnLocation;
    [SerializeField]
    List<GameObject> localAccounsList;

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
            if (EventSystem.current.currentSelectedGameObject != null && usersLoaded)
            {
                userNameSelected = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0).GetComponent<Text>().text;
            }
            if (controls.Player.submit.triggered)
            {
                //Debug.Log("login button pressed : " + userNameSelected);
                LoginButton();
            }
        }
    }

    public void LoginButton()
    {
        //usersLoaded = false;

        if (usersLoaded)
        {
            GameOptions.userName = userNameSelected;
            DBUserModel user = userAccountData.Where(x => x.UserName == userNameSelected).Single();
            GameOptions.userid = user.Userid;
            StartCoroutine(APIHelper.PostToken(user));
            //SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_loading);

            // update lastlogindate for local and online
        }
        //if(!usersLoaded)
        //{
        //    SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_loading);
        //}
        //SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_loading);
    }

    IEnumerator loadUserData()
    {
        yield return new WaitUntil(() => DBHelper.instance != null);
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

        try
        {
            userAccountData = DBHelper.instance.getUserProfileStats();
            GameOptions.numOfLocalUsers = userAccountData.Count;
            if (userAccountData.Count > 0)
            {
                usersLoaded = true;
                if (messageText != null)
                {
                    messageText.text = "select user to log in";
                }
            }
            else
            {
                usersLoaded = false;
                if (messageText != null)
                {
                    messageText.text = "no users found";
                }
                //SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_loading);
            }
        }
        catch (Exception e)
        {
            usersLoaded = false;
            Debug.Log("ERROR : " + e);
            messageText.text = e.ToString();
        }

        StartCoroutine(CreateUserButtons());

    }
    IEnumerator CreateUserButtons()
    {
        yield return new WaitUntil(() => DBHelper.instance != null);
        // for each, create empty object
        // add text + button that is clickable

        // flag for testing
        //usersLoaded = false;
        if (usersLoaded)
        {
            int index = 0;
            foreach (DBUserModel u in userAccountData)
            {
                // instantiate a max of 5 rows
                if (index < 5)
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
            EventSystem.current.SetSelectedGameObject(GameObject.FindObjectOfType<Button>().gameObject);
        }
        else
        {
            GameObject prefabClone =
                Instantiate(localAccountPrefab, localAccountPrefabSpawnLocation.transform.position, Quaternion.identity);
            prefabClone.transform.SetParent(localAccountPrefabSpawnLocation.transform, false);
            // get username text
            prefabClone.transform.Find("userAccount").GetComponent<Text>().text = "no local user account found" +
                "\ngo to account and create one";
            prefabClone.transform.Find("userAccountLoginButton").GetComponent<Text>().text = "continue";
            EventSystem.current.SetSelectedGameObject(GameObject.FindObjectOfType<Button>().gameObject);
        }
    }
    public List<DBUserModel> UserAccountData { get => userAccountData; }
}