
using Assets.Scripts.database;
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

    // Start is called before the first frame update
    void Start()
    {
        //for (int i = 1; i < 106; i++)
        //{

        //    //DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(i);
        //    //StartCoroutine(PostHighscore(dbs));
        //    StartCoroutine(WaitForDatabase(i));
        //}
    }

    public IEnumerator WaitForDatabase(int i)
    {
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);
        DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(i);
        StartCoroutine(PostHighscore(dbs));
    }

    public IEnumerator PostHighscore(DBHighScoreModel dbHighScoreModel)
    {
        Debug.Log("wait for DB..." + Time.time);

        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

        Debug.Log("DB unlocked..." + Time.time);

        string toJson = JsonUtility.ToJson(dbHighScoreModel);

        Debug.Log("tojson : " + toJson);


        //ASCIIEncoding encoder = new ASCIIEncoding();
        //byte[] data = encoder.GetBytes(serializedObject); // a json object, or xml, whatever...

        var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiHighScores) as HttpWebRequest;
        httpWebRequest.ContentType = "application/json; charset=utf-8";
        httpWebRequest.Method = "POST";

        using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
        {
            string json = toJson;

            streamWriter.Write(json);
            streamWriter.Flush();
        }

        var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
        using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
        {
            var result = streamReader.ReadToEnd();
            Debug.Log(result);
        }
    }
}
    //public void getHighScores()
    //{
    //    Debug.Log("hit API...");
    //    HttpWebRequest request =
    //        (HttpWebRequest)WebRequest.Create(String.Format(publicApiHighScores + "?modeid=1"));
    //    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
    //    StreamReader reader = new StreamReader(response.GetResponseStream());

    //    //Debug.Log("publicApiHighScores : " + publicApiHighScores +"/1");

    //    string jsonResponse = @"{""Highscores"":" + reader.ReadToEnd() + "}";
    //    Debug.Log(jsonResponse);

    //    APIHighScores highScores =  JsonConvert.DeserializeObject<APIHighScores>(jsonResponse);

    //    int i = 0;
    //    foreach(APIHighScores.HighScore score in highScores.HighScores) 
    //    {
    //        Debug.Log( i + "start ==============================================");
    //        Debug.Log("id : " + score.Id);
    //        Debug.Log("userid : " + score.UserId);
    //        Debug.Log("modeid : " + score.ModeId);
    //        Debug.Log("cid : " + score.CharacterId);
    //        Debug.Log("lid : " + score.Level);
    //        Debug.Log("lid : " + score.Character);
    //        Debug.Log("lid : " + score.Date);
    //        Debug.Log("lid : " + score.Device);
    //        Debug.Log("lid : " + score.Platform);
    //        Debug.Log(i + " end ==============================================");
    //        i++;
    //    }   
    //}
