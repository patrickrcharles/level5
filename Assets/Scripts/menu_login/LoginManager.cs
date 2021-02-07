using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    Button checkEmailButton;
    [SerializeField]
    Button checkUserNameButton;
    [SerializeField]
    string emailInput;
    [SerializeField]
    string userNameInput;
    [SerializeField]
    InputField emailInputField;
    [SerializeField]
    InputField usernameInputField;
    [SerializeField]
    Text messageDisplay;

    DBUserModel testuser;

    private void Start()
    {
        emailInputField = GameObject.Find("EmailInputField").GetComponent<InputField>();
        usernameInputField = GameObject.Find("UserNameInputField").GetComponent<InputField>();
        messageDisplay = GameObject.Find("messageDisplay").GetComponent<Text>();


        testuser = new DBUserModel();
        testuser.UserName = "drblood";// + RandomString(8);
        testuser.FirstName = "testUser";
        testuser.LastName = "testUser";
        testuser.Password = "testUser";
        testuser.Email = "testUser@cxzxz.cnet";
        testuser.IpAddress = "testUser";
        testuser.SignUpDate = DateTime.Now;
        testuser.LastLogin = DateTime.Now;
        //Debug.Log("LoginManager Awake()");
        //Debug.Log("testUser.Email : " + testuser.Email);
        //Debug.Log("testUser.UserName : " + testuser.UserName);
    }

    public void checkEmailAddress()
    {
        Debug.Log("checkEmailAddress()");
        //Debug.Log("testUser.Email : "+ testuser.Email);

        if (!RegexUtilities.IsValidEmail(emailInput))
        {
            // halt, fix email
            Debug.Log("invalid email address format ");
            messageDisplay.text = "invalid email address format";
        }
        else
        {
            Debug.Log("valid email address format ");
            messageDisplay.text = "valid email address format";
        }
    }

    public void checkUserName()
    {
        Debug.Log("checkUserName()");
        //Debug.Log("testuser.UserName : " + testuser.UserName);
        // check if user already exists
        if (APIHelper.UserNameExists(userNameInput))
        {
            Debug.Log("username already exists...try again");
            messageDisplay.text = "username already exists...try again";
        }
        else
        {
            Debug.Log("username does not exist");
            messageDisplay.text = "username does not exist";
            //StartCoroutine(APIHelper.PostUser(testuser));
        }
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
}
