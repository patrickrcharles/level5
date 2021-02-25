
using Assets.Scripts.database;
using Assets.Scripts.Utility;
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
        // api web hosted
        const string publicApi = "http://13.58.224.237/api/";
        const string publicApiUsers = "http://13.58.224.237/api/users";
        const string publicApiUsersByUserid = "http://13.58.224.237/api/users/userid";
        const string publicApiUsersByUserName = "http://13.58.224.237/api/users/username/";
        const string publicApiUsersByEmail = "http://13.58.224.237/api/users/email/";
        const string publicApiHighScores = "http://13.58.224.237/api/highscores/";
        const string publicApiHighScoresByScoreid = "http://13.58.224.237/api/highscores/scoreid/";
        const string publicApiHighScoresByModeid = "http://13.58.224.237/api/highscores/modeid/";
        const string publicApiHighScoresCountByModeid = "http://13.58.224.237/api/highscores/modeid/count/";
        const string publicApiHighScoresByModeidInGameDisplay = "http://13.58.224.237/api/highscores/modeid/";
        const string publicApiHighScoresByPlatform = "http://13.58.224.237/api/highscores/platform/";
        const string publicApiToken = "http://13.58.224.237/api/token/";

        // localhost testing
        const string localHostHighScoresByModeidInGameDisplay = "https://localhost:44362/api/highscores/game/modeid/";
        const string localHostHighScoresCountByModeid = "https://localhost:44362/api/highscores/modeid/count/";

        static bool apiLocked;
        private static string bearerToken;

        public static string BearerToken { get => bearerToken; }
        public static bool ApiLocked { get => apiLocked; set => apiLocked = value; }

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
            //DBHelper.instance.DatabaseLocked = true;

            // wait for api operations
            yield return new WaitUntil(() => !apiLocked);
            apiLocked = true;

            //// verify unique scoreid does NOT exist in database already
            //if (!APIHelper.ScoreIdExists(dbHighScoreModel.Scoreid))
            //{
            //serialize highscore to json for HTTP POST
            string toJson = JsonUtility.ToJson(dbHighScoreModel);

            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiHighScores) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + bearerToken);
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
                //Debug.Log("----------------- HTTP POST successful : " + (int)statusCode + " " + statusCode);
                DBHelper.instance.setGameScoreSubmitted(dbHighScoreModel.Scoreid, true);
                apiLocked = false;
                DBHelper.instance.DatabaseLocked = false;
            }
            // failed
            else
            {
                //Debug.Log("----------------- HTTP POST failed : " + (int)statusCode + " " + statusCode);
                //unlock api + database
                DBHelper.instance.setGameScoreSubmitted(dbHighScoreModel.Scoreid, false);
                apiLocked = false;
                DBHelper.instance.DatabaseLocked = false;
            }
            //}
            //else
            //{
            //    //Debug.Log(" scoreid already exists : " + dbHighScoreModel.Scoreid);
            //    apiLocked = false;
            //    DBHelper.instance.DatabaseLocked = false;
            //}
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
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiHighScores + dbHighScoreModel.Scoreid) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "PUT";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

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
                //httpWebRequest.Headers.Add("Authorization", bearerToken);
                httpWebRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    Debug.Log(result);
                    dBHighScoreModels = (List<DBHighScoreModel>)JsonConvert.DeserializeObject<List<DBHighScoreModel>>(result);
                }

                //dBHighScoreModels = convertHttpWebResponseToDBHighscoreModelList(httpResponse);
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

        // GET highscore by scoreid by hitting api at
        // http://13.58.224.237/api/highscores/modeid/{modeid}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static List<StatsTableHighScoreRow> GetHighscoreByModeid(int modeid, int hardcore,
            int traffic, int enemies, int page, int results)
        {
            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;
            List<StatsTableHighScoreRow> highScoresList = new List<StatsTableHighScoreRow>();

            // build api request
            string apiRequest = publicApiHighScoresByModeidInGameDisplay + modeid
                + "?hardcore=" + hardcore
                + "&traffic=" + traffic
                + "&enemies=" + enemies
                + "&page=" + page
                + "&results=" + results;

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiRequest) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + bearerToken);

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    highScoresList = JsonConvert.DeserializeObject<List<StatsTableHighScoreRow>>(result);
                }
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
                //Debug.Log("----------------- GetHighscoreByModeid() successful : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }
            // failed
            else
            {
                //Debug.Log("----------------- GetHighscoreByModeid() : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }

            return highScoresList;
        }

        // GET highscore by scoreid by hitting api at
        // http://13.58.224.237/api/highscores/modeid/{modeid}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static int GetHighscoreCountByModeid(int modeid, int hardcore,
            int traffic, int enemies)
        {
            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;

            //build api request
            string apiRequest = publicApiHighScoresCountByModeid + modeid
                + "?hardcore=" + hardcore
                + "&traffic=" + traffic
                + "&enemies=" + enemies;

            //build api localhost request
            //string apiRequest = localHostHighScoresCountByModeid + modeid
            //    + "?hardcore=" + hardcore
            //    + "&traffic=" + traffic
            //    + "&enemies=" + enemies;

            int numResults = 0;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(apiRequest) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", "Bearer " + bearerToken);
                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    numResults = Convert.ToInt32(JsonConvert.DeserializeObject(result));
                }
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
                //Debug.Log("----------------- GetHighscoreCountByModeid() successful : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }
            // failed
            else
            {
                //Debug.Log("----------------- GetHighscoreCountByModeid() : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
            }

            return numResults;
        }


        // -------------------------------------- HTTTP POST User -------------------------------------------

        // POST highscore by scoreid by hitting api at
        // http://13.58.224.237/api/users/{username}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static IEnumerator PostUser(DBUserModel user)
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

            int userid;
            // verify unique scoreid does NOT exist in database already
            if (!APIHelper.UserNameExists(user.UserName))
            {
                //serialize highscore to json for HTTP POST
                string toJson = JsonUtility.ToJson(user);

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
                        DBUserModel model = (DBUserModel)JsonConvert.DeserializeObject<DBUserModel>(result);
                        userid = model.Userid;
                        user.Userid = userid;
                        Debug.Log("userid from api : " + userid);
                        Debug.Log("userid going to db : " + user.Userid);
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
                    // created on api, insert to local db
                    DBHelper.instance.InsertUser(user);
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
                Debug.Log(" scoreid already exists : " + user.UserName);
            }
        }

        // -------------------------------------- HTTTP GET User -------------------------------------------
        // GET highscore by scoreid by hitting api at
        // http://13.58.224.237/api/users/username/{username}
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static bool UserExists(string username)
        {

            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;
            //List<DBHighScoreModel> dBHighScoreModels = new List<DBHighScoreModel>();

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create((publicApiUsersByUserName + username)) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";

                httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

                //dBHighScoreModels = new List<DBHighScoreModel>();
                //dBHighScoreModels = convertHttpWebResponseToDBHighscoreModelList(httpResponse);
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
                return true;
            }
            // failed
            else
            {
                Debug.Log("----------------- GetHighscoreByScoreid() : " + (int)statusCode + " " + statusCode);
                apiLocked = false;
                return false;
            }
        }

        // --------------------------------------Utility functions -------------------------------------------

        // send HttpWebResponse to function to be serialized into List of DBHighScore objects
        public static List<DBHighScoreModel> convertHttpWebResponseToDBHighscoreModelList(HttpWebResponse httpWebResponse)
        {
            Debug.Log("convertHttpWebResponseToDBHighscoreModelList(HttpWebResponse httpWebResponse)");
            List<DBHighScoreModel> dBHighScoreModels = new List<DBHighScoreModel>();

            using (var streamReader = new StreamReader(httpWebResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Debug.Log("results 1 : \n" + result);
                dBHighScoreModels = (List<DBHighScoreModel>)JsonConvert.DeserializeObject<List<DBHighScoreModel>>(result);
                Debug.Log("results 2 : \n" + result);
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

            //Debug.Log("scoredidexists uri :" + (publicApiHighScoresByScoreid + scoreid));

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
        // http://13.58.224.237/api/users/username/{username}found
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
                //httpWebRequest.Headers.Add("Authorization", bearerToken);

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
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create((publicApiUsersByEmail + email)) as HttpWebRequest;
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Headers.Add("Authorization", bearerToken);
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

        // -------------------------------------- HTTTP POST Token -------------------------------------------

        // POST Token by User model to get token
        // http://13.58.224.237/api/token
        // return true if status code == 200 ok
        // return false if status code != 200 ok
        public static IEnumerator PostToken(DBUserModel dBUserModel)
        {
            // note * make this async. sending the request and hitting api should do
            // this automatically.
            // put something like if(!201 created, try again) limit to 10 tries
            // check uniquescoreid not already inserted. hit that api first, then proceed
            Debug.Log("...APIHelper.PostUser()");
            // wait for database operations
            yield return new WaitUntil(() => !DBHelper.instance.DatabaseLocked);

            // wait for api operations
            //yield return new WaitUntil(() => !apiLocked);
            //apiLocked = true;

            // verify unique scoreid does NOT exist in database already
            //if (!APIHelper.UserNameExists(dBUserModel.UserName))
            //{
            //serialize highscore to json for HTTP POST
            string toJson = JsonUtility.ToJson(dBUserModel);

            HttpWebResponse httpResponse = null;
            HttpStatusCode statusCode;
            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(publicApiToken) as HttpWebRequest;
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
                    bearerToken = result;
                    GameOptions.bearerToken = APIHelper.BearerToken;
                    Debug.Log("api : result :: "+ result);
                    Debug.Log("api : token :: " + bearerToken);
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
            if (httpResponse.StatusCode == HttpStatusCode.OK)
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

            yield return new WaitUntil(() => !apiLocked);

            //Debug.Log(APIHelper.bearerToken);
        }
    }
}
