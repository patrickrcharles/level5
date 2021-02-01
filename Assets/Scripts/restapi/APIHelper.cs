
using Assets.Scripts.database;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.restapi
{
    public static class APIHelper
    {
        const string publicApi = "http://13.58.224.237/api/";
        const string publicApiHighScores = "http://13.58.224.237/api/highscores";
        const string publicApiHighScoresByScoreid = "http://13.58.224.237/api/highscores/scoreid/";
        const string publicApiUsers = "http://13.58.224.237/api/users";

        static bool apiLocked;


        // -------------------------------------- HTTTP POST  -------------------------------------------

        // POST highscore by scoreid by hitting api at
        // http://13.58.224.237/api/highscores/scoreid/{scoreid}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static IEnumerator PostHighscore(DBHighScoreModel dbHighScoreModel)
        {
            // note * make this async. sending the request and hitting api should do
            // this automatically.
            // put something like if(!201 created, try again) limit to 10 tries
            // check uniquescoreid not already inserted. hit that api first, then proceed

            yield return new WaitUntil(() => !apiLocked);
            apiLocked = true;

            yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

            DBHighScoreModel dbTempObject = new DBHighScoreModel();
            string toJson = JsonUtility.ToJson(dbTempObject);
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
                Debug.Log("----------------- HTTP POST failed : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }
        }

        // -------------------------------------- HTTTP GET  -------------------------------------------
        // GET highscore by scoreid by hitting api at
        // http://13.58.224.237/api/highscores/scoreid/{scoreid}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static List<DBHighScoreModel> GetHighscoreByScoreid(string scoreid)
        {

            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;
            List<DBHighScoreModel> dBHighScoreModels = new List<DBHighScoreModel>();

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create((publicApiHighScoresByScoreid + scoreid)) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                dBHighScoreModels = new List<DBHighScoreModel>();
                dBHighScoreModels = convertHttpWebResponseToDBHighscoreModelList(httpResponse);
            }
            // on web exception
            catch (WebException e)
            {
                httpResponse = (HttpWebResponse)e.Response;
                apiLocked = false;
            }

            statusCode = httpResponse.StatusCode;

            // if successful
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                Debug.Log("----------------- GetHighscoreByScoreid() successful : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }
            // failed
            else
            {
                Debug.Log("----------------- GetHighscoreByScoreid() : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }

            return dBHighScoreModels;
        }

        // --------------------------------------Utility functions -------------------------------------------

        // send HttpWebResponse to function to be serialized into List of DBHighScore objects
        public static List<DBHighScoreModel> convertHttpWebResponseToDBHighscoreModelList(HttpWebResponse httpWebResponse)
        {

            List<DBHighScoreModel> dBHighScoreModels = new List<DBHighScoreModel>();
            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dBHighScoreModels = (List<DBHighScoreModel>)JsonConvert.DeserializeObject<List<DBHighScoreModel>>(result);
            }

            return dBHighScoreModels;
        }

        // send HttpWebResponse to function to be serialized into a single DBHighScore object
        public static DBHighScoreModel ConvertHttpWebResponseToDBHighscoreModel(HttpWebResponse httpWebResponse)
        {

            DBHighScoreModel dBHighScoreModels = new DBHighScoreModel();
            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                dBHighScoreModels = (DBHighScoreModel)JsonConvert.DeserializeObject<DBHighScoreModel>(result);
            }

            return dBHighScoreModels;
        }

        // check if scoreid exists by hitting api at
        // http://13.58.224.237/api/highscores/scoreid/{scoreid}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static bool ScoreIdExists(string scoreid)
        {

            bool exists = false;
            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create((publicApiHighScoresByScoreid + scoreid)) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            // on web exception
            catch (WebException e)
            {
                httpResponse = (HttpWebResponse)e.Response;
            }

            statusCode = httpResponse.StatusCode;

            // if successful
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                Debug.Log("----------------- ScoreIdExists() successful : " + (int)statusCode + " " + statusCode);
                exists = true;
            }
            // failed
            else
            {
                Debug.Log("----------------- ScoreIdExists() failed : " + (int)statusCode + " " + statusCode);
                exists = false;
            }

            return exists;
        }
    }
}
