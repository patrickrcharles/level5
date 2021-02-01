
using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

public class APIConnector : MonoBehaviour
{
    const string publicApi = "http://13.58.224.237/api/";
    const string publicApiHighScores = "http://13.58.224.237/api/highscores";
    const string publicApiUsers = "http://13.58.224.237/api/users";

    bool apiLocked;

    const string testScoreid = "7085C280BE161220213de8e03a50404e1b686d125c8919b9a3c47b7d9e3";
    // Start is called before the first frame update
    void Start()
    {

        //DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(5);
        //StartCoroutine(PostHighscore(dbs));        
        if (APIHelper.ScoreIdExists(testScoreid))
        {
            List<DBHighScoreModel> dBHighScoreModel = APIHelper.GetHighscoreByScoreid(testScoreid);
            Debug.Log("response[0] : " + dBHighScoreModel[0].Character);
            Debug.Log("response[0] : " + dBHighScoreModel[0].Level);
            Debug.Log("response[0] : " + dBHighScoreModel[0].Ipaddress);
        }
    }

    //public IEnumerator WaitForDatabase(int i)
    //{
    //    yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);
    //    DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(i);
    //    StartCoroutine(PostHighscore(dbs));
    //}

}

