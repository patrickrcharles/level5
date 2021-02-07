
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
        const string publicApiUsers = "http://13.58.224.237/api/users";
        const string publicApiUsersByUserid = "http://13.58.224.237/api/users/userid";
        const string publicApiUsersByUserName = "http://13.58.224.237/api/users/username/";
        const string publicApiUsersByEmail = "http://13.58.224.237/api/users/email/";
        const string publicApiHighScores = "http://13.58.224.237/api/highscores/";
        const string publicApiHighScoresByScoreid = "http://13.58.224.237/api/highscores/scoreid/";
        const string publicApiHighScoresByModeid = "http://13.58.224.237/api/highscores/modeid/";

        static bool apiLocked;


        // -------------------------------------- HTTTP POST Highscore -------------------------------------------

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

            // wait for database operations
            yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

            // wait for api operations
            yield return new WaitUntil(() => !apiLocked);
            apiLocked = true;

            // verify unique scoreid does NOT exist in database already
            if (!APIHelper.ScoreIdExists(dbHighScoreModel.Scoreid))
            {
                //serialize highscore to json for HTTP POST
                string toJson = JsonUtility.ToJson(dbHighScoreModel);

                HttpWebResponse httpResponse = null;
                HttpStatusCode statusCode;

                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiHighScores) as HttpWebRequest;
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Method = "POST";
                    //post
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = toJson;
                        streamWriter.Write(json);
                        streamWriter.Flush();
                    }
                    // response
                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }
                // on web exception
                catch (WebException e)
                {
                    httpResponse = (HttpWebResponse)e.Response;
                    Debug.Log("----------------- ERROR : " + e);
                    //unlock api + database
                    apiLocked = false;
                    DBHelper.instance.DatabaseLocked = false;
                }

                statusCode = httpResponse.StatusCode;

                // if successful
                if (httpResponse.StatusCode == HttpStatusCode.Created)
                {
                    Debug.Log("----------------- HTTP POST successful : " + (int)statusCode + " " + statusCode);
                    apiLocked = false;
                    DBHelper.instance.DatabaseLocked = false;
                }
                // failed
                else
                {
                    Debug.Log("----------------- HTTP POST failed : " + (int)statusCode + " " + statusCode);
                    //unlock api + database
                    apiLocked = false;
                    DBHelper.instance.DatabaseLocked = false;
                }
            }
            else
            {
                Debug.Log(" scoreid already exists : " + dbHighScoreModel.Scoreid);
            }
        }

        // -------------------------------------- HTTTP PUT Highscore -------------------------------------------

        // PUT (if doesnt exist, insert) highscore by scoreid by hitting api at
        // http://13.58.224.237/api/highscores/{scoreid}}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static IEnumerator PutHighscore(DBHighScoreModel dbHighScoreModel)
        {
            // note * make this async. sending the request and hitting api should do
            // this automatically.
            // put something like if(!201 created, try again) limit to 10 tries
            // check uniquescoreid not already inserted. hit that api first, then proceed


            yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

            yield return new WaitUntil(() => !apiLocked);
            apiLocked = true;

            string toJson = JsonUtility.ToJson(dbHighScoreModel);

            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiHighScores + dbHighScoreModel.Scoreid ) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    string json = toJson;

                    streamWriter.Write(json);
                    streamWriter.Flush();
                }

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Debug.Log(result);
                }
            }
            // on web exception
            catch (WebException e)
            {
                httpResponse = (HttpWebResponse)e.Response;
                Debug.Log("----------------- ERROR : " + e);
                apiLocked = false;
                DBHelper.instance.DatabaseLocked = false;
            }

            statusCode = httpResponse.StatusCode;

            // if successful
            if (httpResponse.StatusCode == HttpStatusCode.Created)
            {
                Debug.Log("----------------- HTTP PUT successful : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
                DBHelper.instance.DatabaseLocked = false;
            }
            // failed
            else
            {
                Debug.Log("----------------- HTTP PUT failed : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
                DBHelper.instance.DatabaseLocked = false;
            }
        }

        // -------------------------------------- HTTTP GET Highscore -------------------------------------------
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
                Debug.Log("----------------- ERROR : " + e);
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


        // -------------------------------------- HTTTP POST Highscore -------------------------------------------

        // POST highscore by scoreid by hitting api at
        // http://13.58.224.237/api/users/{username}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static IEnumerator PostUser(DBUserModel dBUserModel)
        {
            // note * make this async. sending the request and hitting api should do
            // this automatically.
            // put something like if(!201 created, try again) limit to 10 tries
            // check uniquescoreid not already inserted. hit that api first, then proceed
            Debug.Log("...APIHelper.PostUser()");
            // wait for database operations
            yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

            // wait for api operations
            yield return new WaitUntil(() => !apiLocked);
            apiLocked = true;

            // verify unique scoreid does NOT exist in database already
            if (!APIHelper.UserNameExists(dBUserModel.UserName))
            {
                //serialize highscore to json for HTTP POST
                string toJson = JsonUtility.ToJson(dBUserModel);

                HttpWebResponse httpResponse = null;
                HttpStatusCode statusCode;
                try
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiUsers) as HttpWebRequest;
                    httpWebRequest.ContentType = "application/json; charset=utf-8";
                    httpWebRequest.Method = "POST";
                    //post
                    using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                    {
                        string json = toJson;
                        streamWriter.Write(json);
                        streamWriter.Flush();
                        Debug.Log(json);
                    }
                    // response
                    httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                    }
                }
                // on web exception
                catch (WebException e)
                {
                    httpResponse = (HttpWebResponse)e.Response;
                    Debug.Log("----------------- ERROR : " + e);
                    //unlock api + database
                    apiLocked = false;
                    DBHelper.instance.DatabaseLocked = false;
                }

                statusCode = httpResponse.StatusCode;

                // if successful
                if (httpResponse.StatusCode == HttpStatusCode.Created)
                {
                    Debug.Log("----------------- HTTP POST successful : " + (int)statusCode + " " + statusCode);
                    apiLocked = false;
                    DBHelper.instance.DatabaseLocked = false;
                }
                // failed
                else
                {
                    Debug.Log("----------------- HTTP POST failed : " + (int)statusCode + " " + statusCode);
                    //unlock api + database
                    apiLocked = false;
                    DBHelper.instance.DatabaseLocked = false;
                }
            }
            else
            {
                Debug.Log(" scoreid already exists : " + dBUserModel.UserName);
            }
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

            Debug.Log("scoredidexists uri :" + (publicApiHighScoresByScoreid + scoreid));

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
                //Debug.Log("----------------- ERROR : " + e);
            }

            statusCode = httpResponse.StatusCode;

            // if successful
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                //Debug.Log("----------------- scoreid found : " + (int)statusCode + " " + statusCode);
                exists = true;
            }
            // failed
            else
            {
                //Debug.Log("----------------- scoreid not found : " + (int)statusCode + " " + statusCode);
                exists = false;
            }

            return exists;
        }

        // check if username exists by hitting api at
        // http://13.58.224.237/api/users/username/{username}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static bool UserNameExists(string username)
        {

            bool exists = false;
            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create((publicApiUsersByUserName + username)) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            // on web exception
            catch (WebException e)
            {
                httpResponse = (HttpWebResponse)e.Response;
                Debug.Log("----------------- ERROR : " + e);
            }

            statusCode = httpResponse.StatusCode;

            // if successful
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                Debug.Log("----------------- username found : " + (int)statusCode + " " + statusCode);
                exists = true;
            }
            // failed
            else
            {
                Debug.Log("----------------- username not found : " + (int)statusCode + " " + statusCode);
                exists = false;
            }

            return exists;
        }

        // check if username email by hitting api at
        // http://13.58.224.237/api/users/username/{username}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static bool EmailExists(string email)
        {

            bool exists = false;
            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;

            Debug.Log("username exists uri :" + (publicApiUsersByEmail + email));

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create((publicApiUsersByEmail + email)) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            }
            // on web exception
            catch (WebException e)
            {
                httpResponse = (HttpWebResponse)e.Response;
                Debug.Log("----------------- ERROR : " + e);
            }

            statusCode = httpResponse.StatusCode;

            // if successful
            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                Debug.Log("----------------- username found : " + (int)statusCode + " " + statusCode);
                exists = true;
            }
            // failed
            else
            {
                Debug.Log("----------------- username not found : " + (int)statusCode + " " + statusCode);
                exists = false;
            }

            return exists;
        }
    }
}
