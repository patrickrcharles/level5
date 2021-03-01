using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = System.Random;

public class LoginManager : MonoBehaviour
{
    Text messageDisplay;
    string errorMessageEmail = "";
    string errorMessageUserName = "";

    DBUserModel user;
    PlayerControls controls;
    APIConnector apiConnector;
    //buttonobject names
    const string checkEmailButtonName = "checkEmail";
    const string checkUserNameButtonName = "checkUserName";
    const string loginNameButtonName = "login";
    const string createUserNameButtonName = "createUser";
    //input field object names
    const string emailAddressInputFieldName = "EmailInputField";
    const string userNameInputFieldName = "UserNameInputField";
    const string passwordInputFieldName = "PasswordInputField";
    const string firstNameInputFieldName = "FirstNameInputField";
    const string lastNameInputFieldName = "LastNameInputField";
    // footer button names
    const string mainMenuButtonName = "press_start";
    const string statsMenuButtonName = "stats_menu";
    const string progressionMenuButtonName = "update_menu";
    const string creditsMenuButtonName = "credits_menu";

    string emailInput;
    string userNameInput;
    string passwordInput;
    string firstNameInput;
    string lastNameInput;

    InputField emailInputField;
    InputField usernameInputField;
    InputField passwordInputField;
    InputField firstNameInputField;
    InputField lastNameInputField;

    //[SerializeField]
    //GameObject currentSelectedObject;

    // button objects
    [SerializeField]
    GameObject emailAddressTextButtonObject;
    [SerializeField]
    GameObject checkEmailButtonObject;
    [SerializeField]
    GameObject userNameTextButtonObject;
    [SerializeField]
    GameObject checkUserNameButtonObject;
    [SerializeField]
    GameObject passwordTextButtonObject;
    [SerializeField]
    GameObject firstNameTextButtonObject;
    [SerializeField]
    GameObject lastNameTextButtonObject;

    bool buttonPressed;
    [SerializeField]
    bool emailAddressIsValid;
    [SerializeField]
    bool userNameIsValid;

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
        //controls.PlayerTouch.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
        //controls.PlayerTouch.Disable();
    }

    void Awake()
    {
        // mapped controls
        controls = new PlayerControls();
        apiConnector = GameObject.FindObjectOfType<APIConnector>();
    }

    private void Start()
    {
        emailInputField = GameObject.Find("EmailInputField").GetComponent<InputField>();
        usernameInputField = GameObject.Find("UserNameInputField").GetComponent<InputField>();
        passwordInputField = GameObject.Find("PasswordInputField").GetComponent<InputField>();
        firstNameInputField = GameObject.Find("FirstNameInputField").GetComponent<InputField>();
        lastNameInputField = GameObject.Find("LastNameInputField").GetComponent<InputField>();
        messageDisplay = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageDisplay.text = "";
    }

    void Update()
    {
#if UNITY_ANDROID || UNITY_IOS
        // test onscreen keyboard
        if (controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha9) 
            && !buttonPressed)
        {
            Debug.Log("test on screen keyboard");
        }
#endif
        if (controls.UINavigation.Submit.triggered && !buttonPressed)
        {
            buttonPressed = true;

            //check email
            if (EventSystem.current.currentSelectedGameObject.name.Equals(checkEmailButtonName))
            {
                EventSystem.current.SetSelectedGameObject(emailAddressTextButtonObject);
            }
            // check user name
            if (EventSystem.current.currentSelectedGameObject.name.Equals(checkUserNameButtonName))
            {
                EventSystem.current.SetSelectedGameObject(userNameTextButtonObject);
            }
            // enter email field
            if (EventSystem.current.currentSelectedGameObject.name.Equals(emailAddressInputFieldName))
            {
                EventSystem.current.SetSelectedGameObject(checkEmailButtonObject);
            }
            // enter username field
            if (EventSystem.current.currentSelectedGameObject.name.Equals(userNameInputFieldName))
            {
                EventSystem.current.SetSelectedGameObject(checkUserNameButtonObject);
            }
            // enter password field
            if (EventSystem.current.currentSelectedGameObject.name.Equals(passwordInputFieldName))
            {
                EventSystem.current.SetSelectedGameObject(passwordTextButtonObject);
            }
            // enter first name field
            if (EventSystem.current.currentSelectedGameObject.name.Equals(firstNameInputFieldName))
            {
                EventSystem.current.SetSelectedGameObject(firstNameTextButtonObject);
            }
            // enter last name field
            if (EventSystem.current.currentSelectedGameObject.name.Equals(lastNameInputFieldName))
            {
                EventSystem.current.SetSelectedGameObject(lastNameTextButtonObject);
            }
            // footer
            // main menu
            if (EventSystem.current.currentSelectedGameObject.name.Equals(mainMenuButtonName))
            {
                SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_start);
            }
            //stats
            if (EventSystem.current.currentSelectedGameObject.name.Equals(statsMenuButtonName))
            {
                SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_stats);
            }
            //progression
            if (EventSystem.current.currentSelectedGameObject.name.Equals(progressionMenuButtonName))
            {
                SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_progression);
            }
            //credits
            if (EventSystem.current.currentSelectedGameObject.name.Equals(creditsMenuButtonName))
            {
                SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_credits);
            }
            buttonPressed = false;
        }
    }

    public void checkEmailAddressFormat()
    {
        if (!RegexUtilities.IsValidEmail(emailInput))
        {
            emailAddressIsValid = false;
        }
        if (string.IsNullOrWhiteSpace(emailInput))
        {
            emailAddressIsValid = false;
        }
        else
        {
            emailAddressIsValid = true;
        }
        messageDisplay.text = getCheckEmailAddress();
    }

    public string getCheckEmailAddress()
    {
        messageDisplay.text = "";
        errorMessageUserName = "";
        errorMessageEmail = "";

        if (!RegexUtilities.IsValidEmail(emailInput))
        {
            errorMessageEmail += "\nemail address is invalid format";
        }
        else
        {
            errorMessageEmail += "\nemail address is valid format";
        }
        if (string.IsNullOrWhiteSpace(emailInput))
        {
            errorMessageEmail += "\nemail address is empty or contains white space";
        }
        else
        {
            errorMessageEmail += "\nemail address : other problem";
        }

        //setErrorMessage();
        //messageDisplay.text = errorMessageEmail;
        if (string.IsNullOrEmpty(errorMessageUserName))
        {
            messageDisplay.text = errorMessageEmail;
        }
        else
        {
            messageDisplay.text = errorMessageUserName + errorMessageEmail;
        }
        return errorMessageEmail;
    }

    public void checkUserName()
    {
        // check if user already exists or null
        if (string.IsNullOrEmpty(userNameInput))
        {
            userNameIsValid = false;
        }
        if (APIHelper.UserNameExists(userNameInput))
        {
            userNameIsValid = false;
        }
        else
        {
            userNameIsValid = true;
        }
        messageDisplay.text = getCheckUserName();
    }

    public string getCheckUserName()
    {
        messageDisplay.text = "";
        errorMessageUserName = "";
        errorMessageEmail = "";

        // check if user already exists or null
        if (string.IsNullOrEmpty(userNameInput))
        {
            errorMessageUserName += "\nusername is empty or contains whitespace";
        }
        if (APIHelper.UserNameExists(userNameInput))
        {
            errorMessageUserName += "\nusername already exists";
        }
        if (!APIHelper.UserNameExists(userNameInput) && !string.IsNullOrWhiteSpace(userNameInput))
        {
            errorMessageUserName += "\nusername does not exist";
        }

        if (string.IsNullOrEmpty(errorMessageEmail))
        {
            messageDisplay.text = errorMessageUserName;
        }
        else
        {
            messageDisplay.text = errorMessageEmail + errorMessageUserName;
        }
        return errorMessageUserName;
    }

    public void createUser()
    {
        checkEmailAddressFormat();
        checkUserName();
        messageDisplay.text = getCheckEmailAddress() + getCheckUserName();

        if (userNameIsValid && emailAddressIsValid)
        {
            DBUserModel user = new DBUserModel();

            user.Email = emailInput;
            user.UserName = userNameInput;
            user.Password = passwordInput;
            user.FirstName = firstNameInput;
            user.LastName = lastNameInput;
            user.IpAddress = GetExternalIpAdress();
            user.SignUpDate = DateTime.Now.ToString();
            user.LastLogin = DateTime.Now.ToString();

            apiConnector.CreateNewUser(user);
        }
    }

    public void loginUser()
    {
        //checkEmailAddressFormat();
        //checkUserName();
        //messageDisplay.text = getCheckEmailAddress() + getCheckUserName();

        //if (userNameIsValid && emailAddressIsValid)
        //{
        //    DBUserModel user = new DBUserModel();

        //    user.Email = emailInput;
        //    user.UserName = userNameInput;
        //    user.Password = passwordInput;
        //    user.FirstName = firstNameInput;
        //    user.LastName = lastNameInput;
        //    user.IpAddress = GetExternalIpAdress();
        //    user.SignUpDate = DateTime.Now.ToString();
        //    user.LastLogin = DateTime.Now.ToString();

        //    apiConnector.CreateNewUser(user);
        //}
        Debug.Log("loginUser()");
        Debug.Log("----- check user valid / get token");
        Debug.Log("----- set user loggen in /load to gameoptions.username");
        SceneManager.LoadSceneAsync(SceneNameConstants.SCENE_NAME_level_00_login);
    }



    private string RandomString(int length)
    {
        var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var stringChars = new char[length];
        var random = new Random();

        for (int i = 0; i < stringChars.Length; i++)
        {
            stringChars[i] = chars[random.Next(chars.Length)];
        }

        var finalString = new String(stringChars);

        return finalString;
    }

    public void readEmailAddressInput(string s)
    {
        emailInput = emailInputField.text;
        Debug.Log(emailInput);
    }

    public void readUsernameInput(string s)
    {
        userNameInput = usernameInputField.text;
        Debug.Log(userNameInput);
    }

    public void readPasswordInput(string s)
    {
        passwordInput = passwordInputField.text;
        Debug.Log(passwordInput);
    }

    public void readFirstNameInput(string s)
    {
        firstNameInput = firstNameInputField.text;
        Debug.Log(firstNameInput);
    }
    public void readLastNameInput(string s)
    {
        lastNameInput = lastNameInputField.text;
        Debug.Log(lastNameInput);
    }

    public string GetExternalIpAdress()
    {
        string pubIp = new WebClient().DownloadString("https://api.ipify.org");
        return pubIp;
    }
}
