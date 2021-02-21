
using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using Random = System.Random;


public class APIConnector : MonoBehaviour
{

    // Start is called before the first frame update
    void Awake()
    {
        DBUserModel testuser = new DBUserModel();

        //testuser.UserName = "testUser" + RandomString(8);
        testuser.UserName = "drblood";
        testuser.Password = "admin";

        //StartCoroutine(APIHelper.PostToken(testuser));
        
        // check database for existing users
        // if true - offer option to load user
        // get token, save token
        // set logged in

        // else - create user
    }

    public void CreateNewUser(DBUserModel user)
    {
        if (!RegexUtilities.IsValidEmail(user.Email))
        {
            // halt, fix email
            Debug.Log("invalid email address format ");
        }
        // check if user already exists
        if (APIHelper.UserNameExists(user.UserName))
        {
            Debug.Log("username already exists...try again");
        }
        else
        {
            Debug.Log("username does not exist");
            StartCoroutine(APIHelper.PostUser(user));
        }

        // if not, create user
        // else return 'username already exists
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
}


