
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

public class APIConnector : MonoBehaviour
{
    const string publicApi = "http://13.58.224.237/api/";
    const string publicApiHighScores = "http://13.58.224.237/api/highscores";

    // Start is called before the first frame update
    void Start()
    {
        getHighScores();
    }


    public void getHighScores()
    {
        Debug.Log("hit API...");
        HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(String.Format(publicApiHighScores));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        StreamReader reader = new StreamReader(response.GetResponseStream());

        Debug.Log("publicApiHighScores : " + publicApiHighScores);

        //string jsonResponse = reader.ReadToEnd();
        string jsonResponse = @"{""Highscores"":" + reader.ReadToEnd() + "}";
        Debug.Log(jsonResponse);

        APIHighScores highScores =  JsonConvert.DeserializeObject<APIHighScores>(jsonResponse);

        foreach(APIHighScores.HighScore score in highScores.HighScores) 
        {
            Debug.Log("id : " + score.Id);
            Debug.Log("userid : " + score.UserId);
            Debug.Log("modeid : " + score.ModeId);
        }
      
    }
}
