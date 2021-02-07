
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
    const string publicApi = "http://13.58.224.237/api/";
    const string publicApiHighScores = "http://13.58.224.237/api/highscores";
    const string publicApiUsers = "http://13.58.224.237/api/users";

    bool apiLocked;

    //const string testScoreid = "7085C280BE161220213de8e03a50404e1b686d125c8919b9a3c47b7d9e3";

    // Start is called before the first frame update
    void Start()
    {
        //DBUserModel testuser = new DBUserModel();

        //testuser.UserName = "testUser" + RandomString(8);
        //testuser.FirstName = "testUser";
        //testuser.LastName = "testUser";
        //testuser.Password = "testUser";
        //testuser.Email = "testUser";
        //testuser.IpAddress = "testUser";
        //testuser.SignUpDate = DateTime.Now;
        //testuser.LastLogin = DateTime.Now;

        //CreateNewUser(testuser);

        //string email1 = "dasda@adsa.com";
        //string email2 = "sdfsfsdfa.com";

        //Debug.Log(email1 + " is valid : " + RegexUtilities.IsValidEmail(email1));
        //Debug.Log(email2 + " is valid : " + RegexUtilities.IsValidEmail(email2));


        //DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(104);

        ////Debug.Log("dbs : " + dbs.Scoreid);
        ////Debug.Log("dbs : " + dbs.Character);
        ////Debug.Log("dbs : " + dbs.Level);
        ////Debug.Log("dbs : " + dbs.Version);
        //if (!APIHelper.ScoreIdExists(dbs.Scoreid))
        //{
        //    StartCoroutine(APIHelper.PostHighscore(dbs));
        //}
        //else
        //{
        //    Debug.Log(" scoreid already exists : " + dbs.Scoreid);
        //}


        //if (!APIHelper.ScoreIdExists(dbs.Scoreid))
        //{
        //    StartCoroutine(APIHelper.PutHighscore(dbs));
        //}
        //else
        //{
        //    Debug.Log(" scoreid already exists : " + dbs.Scoreid);
        //}

        //if (APIHelper.ScoreIdExists(testScoreid))
        //{
        //    List<DBHighScoreModel> dBHighScoreModel = APIHelper.GetHighscoreByScoreid(testScoreid);
        //    Debug.Log("response[0] : " + dBHighScoreModel[0].Character);
        //    Debug.Log("response[0] : " + dBHighScoreModel[0].Level);
        //    Debug.Log("response[0] : " + dBHighScoreModel[0].Ipaddress);
        //}

        //string testUserName = "drblood";
        //bool exists =  APIHelper.UserNameExists(testUserName);
        //Debug.Log("user : " + testUserName + " exists : " + exists);

        //string testUserName1 = "drbloodfart";
        //bool exists1 = APIHelper.UserNameExists(testUserName1);
        //Debug.Log("user : " + testUserName1 + " exists : " + exists1);
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


