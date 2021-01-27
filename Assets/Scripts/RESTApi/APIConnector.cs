
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
    const string publicApiUsers = "http://13.58.224.237/api/users";

    bool apiLocked;

    // Start is called before the first frame update
    void Start()
    {

        DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(5);
        StartCoroutine(PostHighscore(dbs));
    }

    //public IEnumerator WaitForDatabase(int i)
    //{
    //    yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);
    //    DBHighScoreModel dbs = DBHelper.instance.getHighScoreFromDatabase(i);
    //    StartCoroutine(PostHighscore(dbs));
    //}

    public IEnumerator PostHighscore(DBHighScoreModel dbHighScoreModel)
    {
        Debug.Log("wait for API..." + Time.time);
        yield return new WaitUntil(() => !apiLocked);
        apiLocked = true;

        Debug.Log("wait for DB..." + Time.time);
        yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

        DBHighScoreModel dbTempObject = new DBHighScoreModel();

        //string toJson = JsonUtility.ToJson(dbHighScoreModel);
        string toJson = JsonUtility.ToJson(dbTempObject);
        //string toJson = "[{dsadasd}]";
        Debug.Log("Json : " + toJson);

        HttpWebResponse httpResponse = null;
        HttpStatusCode statusCode;

        try
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiHighScores) as HttpWebRequest;
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = toJson;

                streamWriter.Write(json);
                streamWriter.Flush();
            }

            httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            //Debug.Log("----------------- ...finished");
            //using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            //{
            //    var result = streamReader.ReadToEnd();
            //    Debug.Log("----------------- ...finished");
            //    //Debug.Log("----------------- RESPONSE\n" + result);
            //    //Debug.Log("----------------- code : " + httpResponse.StatusCode);
            //    //Debug.Log("----------------- code : " + (int)httpResponse.StatusCode);
        }
        // on web exception
        catch (WebException e)
        {
            httpResponse = (HttpWebResponse)e.Response;
            apiLocked = false;
        }

        statusCode = httpResponse.StatusCode;

        // if successful
        if (httpResponse.StatusCode == HttpStatusCode.Created)
        {
            Debug.Log("----------------- HTTP POST successful : " + (int)statusCode + " " + statusCode);
            apiLocked = false;
        }
        // failed
        else
        {
            Debug.Log("----------------- HTTP POST failed : " + (int)statusCode + " " +   statusCode);
            apiLocked = false;
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
