
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
        HttpWebRequest request =
            (HttpWebRequest)WebRequest.Create(String.Format(publicApiHighScores));
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();

        StreamReader reader = new StreamReader(response.GetResponseStream());
        //string jsonResponse = reader.ReadToEnd();
        string jsonResponse = @"{""Highscores"":" + reader.ReadToEnd() + "}";
        Debug.Log(jsonResponse);

        APIHighScores highScores =  JsonConvert.DeserializeObject<APIHighScores>(jsonResponse);

        Debug.Log(highScores.HighScores[1].Id);
        Debug.Log(highScores.HighScores[1].UserId);
        Debug.Log(highScores.HighScores[1].ModeId);
      
    }
}
