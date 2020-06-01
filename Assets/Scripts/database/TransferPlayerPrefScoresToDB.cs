
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;


public class TransferPlayerPrefScoresToDB : MonoBehaviour
{
    private int currentPlayerId;

    private String connection;
    private String databaseNamePath = "/level5.db";
    private int currentGameVersion;
    private String filepath;


    enum prevVersionsWithNoDB
    {
        v1 = 201,
        v2 = 300,
        v3 = 301,
        v4 = 302
    };


    void Start()
    {
        //Debug.Log("TransferPlayerPrefScoresToDB - start");
        //currentGameVersion = getCurrentGameVersionToInt(Application.version);

        //currentPlayerId = getIntValueFromTableByFieldAndId("User", "userid", 1);
        //Debug.Log("currentPlayerId : " + currentPlayerId);

        //// if database doesnt exist
        //if (!File.Exists(filepath))
        //{
        //    createDatabase();
        //}

        //if (currentGameVersion == (int)prevVersionsWithNoDB.v1
        //    || currentGameVersion == (int)prevVersionsWithNoDB.v2
        //    || currentGameVersion == (int)prevVersionsWithNoDB.v3
        //    || currentGameVersion == (int)prevVersionsWithNoDB.v4
        //    && getPrevHighScoreInserted() == 0) // hasnt been run before
        //{
        //    Debug.Log(" put playerprefs into db");
        //    InsertPrevVersionHighScoresToDB();
        //}

       // setPrevHighScoreInsertedTrue();
    }

    public void InsertPrevVersionHighScoresToDB()
    {
        Debug.Log("InsertPrevVersionHighScoresToDB()");

        connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
        filepath = Application.dataPath + databaseNamePath;

        Debug.Log("connection : " + connection);

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        int m1score = PlayerPrefs.GetInt("mode_" + 1 + "_totalPoints");
        string m1Version = PlayerPrefs.GetString("mode_" + 1 + "_totalPointsAppVersion");
        string m1Date = PlayerPrefs.GetString("mode_" + 1 + "_totalPointsDate");
        string m1Level = PlayerPrefs.GetString("mode_" + 1 + "_totalPointsLevel");
        string m1Player = PlayerPrefs.GetString("mode_" + 1 + "_totalPointsPlayer");
        string m1os = PlayerPrefs.GetString("mode_" + 1 + "_operatingSystem");
        string m1version = PlayerPrefs.GetString("mode_1_totalPointsAppVersion");


        int m2score = PlayerPrefs.GetInt("mode_" + 2 + "_threePointersMade");
        string m2Version = PlayerPrefs.GetString("mode_" + 2 + "_threePointersMadeAppVersion");
        string m2Date = PlayerPrefs.GetString("mode_" + 2 + "_threePointersMadeDate");
        string m2Level = PlayerPrefs.GetString("mode_" + 2 + "_threePointersMadeLevel");
        string m2Player = PlayerPrefs.GetString("mode_" + 2 + "_threePointersMadePlayer");
        string m2os = PlayerPrefs.GetString("mode_" + 2 + "_operatingSystem");
        string m2version = PlayerPrefs.GetString("mode_" + 2 + "_longestShotMadeFreePlayAppVersion");

        int m3score = PlayerPrefs.GetInt("mode_" + 3 + "_fourPointersMade");
        string m3Version = PlayerPrefs.GetString("mode_" + 3 + "_fourPointersMadeAppVersion");
        string m3Date = PlayerPrefs.GetString("mode_" + 3 + "_fourPointersMadeDate");
        string m3Level = PlayerPrefs.GetString("mode_" + 3 + "_fourPointersMadeLevel");
        string m3Player = PlayerPrefs.GetString("mode_" + 3 + "_fourPointersMadePlayer");
        string m3os = PlayerPrefs.GetString("mode_" + 3 + "_operatingSystem");
        string m3version = PlayerPrefs.GetString("mode_" + 3 + "_longestShotMadeFreePlayAppVersion");

        int m4score = PlayerPrefs.GetInt("mode_" + 4 + "_sevenPointersMade");
        string m4Version = PlayerPrefs.GetString("mode_" + 4 + "_sevenPointersMadeAppVersion");
        string m4Date = PlayerPrefs.GetString("mode_" + 4 + "_sevenPointersMadeDate");
        string m4Level = PlayerPrefs.GetString("mode_" + 4 + "_sevenPointersMadeLevel");
        string m4Player = PlayerPrefs.GetString("mode_" + 4 + "_sevenPointersMadePlayer");
        string m4os = PlayerPrefs.GetString("mode_" + 4 + "_operatingSystem");
        string m4version = PlayerPrefs.GetString("mode_" + 4 + "_longestShotMadeFreePlayAppVersion");

        float m5score = PlayerPrefs.GetFloat("mode_" + 5 + "_longestShotMade");
        string m5Version = PlayerPrefs.GetString("mode_" + 5 + "_longestShotMadeAppVersion");
        string m5Date = PlayerPrefs.GetString("mode_" + 5 + "_longestShotMadeDate");
        string m5Level = PlayerPrefs.GetString("mode_" + 5 + "_longestShotMadeLevel");
        string m5Player = PlayerPrefs.GetString("mode_" + 5 + "_longestShotMadePlayer");
        string m5os = PlayerPrefs.GetString("mode_" + 5 + "_operatingSystem");
        string m5version = PlayerPrefs.GetString("mode_" + 5 + "_longestShotMadeFreePlayAppVersion");

        float m6score = PlayerPrefs.GetFloat("mode_" + 6 + "_totalDistance");
        string m6Version = PlayerPrefs.GetString("mode_" + 6 + "_totalDistanceAppVersion");
        string m6Date = PlayerPrefs.GetString("mode_" + 6 + "_totalDistanceDate");
        string m6Level = PlayerPrefs.GetString("mode_" + 6 + "_totalDistanceLevel");
        string m6Player = PlayerPrefs.GetString("mode_" + 6 + "_totalDistancePlayer");
        string m6os = PlayerPrefs.GetString("mode_" + 6 + "_operatingSystem");
        string m6version = PlayerPrefs.GetString("mode_" + 6 + "_longestShotMadeFreePlayAppVersion");

        float m7score = PlayerPrefs.GetFloat("mode_" + 7 + "_lowThreeTime");
        string m7Version = PlayerPrefs.GetString("mode_" + 7 + "_lowThreeTimeAppVersion");
        string m7Date = PlayerPrefs.GetString("mode_" + 7 + "_lowThreeTimeDate");
        string m7Level = PlayerPrefs.GetString("mode_" + 7 + "_lowThreeTimeLevel");
        string m7Player = PlayerPrefs.GetString("mode_" + 7 + "_lowThreeTimePlayer");
        string m7os = PlayerPrefs.GetString("mode_" + 7 + "_operatingSystem");
        string m7version = PlayerPrefs.GetString("mode_" + 7 + "_longestShotMadeFreePlayAppVersion");

        float m8score = PlayerPrefs.GetFloat("mode_" + 8 + "_lowFourTime");
        string m8Version = PlayerPrefs.GetString("mode_" + 8 + "_lowFourTimeAppVersion");
        string m8Date = PlayerPrefs.GetString("mode_" + 8 + "_lowFourTimeDate");
        string m8Level = PlayerPrefs.GetString("mode_" + 8 + "_lowFourTimeLevel");
        string m8Player = PlayerPrefs.GetString("mode_" + 8 + "_lowFourTimePlayer");
        string m8os = PlayerPrefs.GetString("mode_" + 8 + "_operatingSystem");
        string m8version = PlayerPrefs.GetString("mode_" + 8 + "_longestShotMadeFreePlayAppVersion");


        float m9score = PlayerPrefs.GetFloat("mode_" + 9 + "_lowAllTime");
        string m9Version = PlayerPrefs.GetString("mode_" + 9 + "_lowAllTimeAppVersion");
        string m9Date = PlayerPrefs.GetString("mode_" + 9 + "_lowAllTimeDate");
        string m9Level = PlayerPrefs.GetString("mode_" + 9 + "_lowAllTimeLevel");
        string m9Player = PlayerPrefs.GetString("mode_" + 9 + "_lowAllTimePlayer");
        string m9os = PlayerPrefs.GetString("mode_" + 9 + "_operatingSystem");
        string m9version = PlayerPrefs.GetString("mode_" + 9 + "_longestShotMadeFreePlayAppVersion");

        float m10score = PlayerPrefs.GetFloat("mode_" + 10 + "_lowThreeTimeMoneyBall");
        string m10Version = PlayerPrefs.GetString("mode_" + 10 + "_lowThreeTimeMoneyBallAppVersion");
        string m10Date = PlayerPrefs.GetString("mode_" + 10 + "_lowThreeTimeMoneyBallDate");
        string m10Level = PlayerPrefs.GetString("mode_" + 10 + "_lowThreeTimeMoneyBallLevel");
        string m10Player = PlayerPrefs.GetString("mode_" + 10 + "_lowThreeTimeMoneyBallPlayer");
        string m10os = PlayerPrefs.GetString("mode_" + 10 + "_operatingSystem");
        string m10version = PlayerPrefs.GetString("mode_" + 10 + "_longestShotMadeFreePlayAppVersion");

        float m11score = PlayerPrefs.GetFloat("mode_" + 11 + "_lowFourTimeMoneyBall");
        string m11Version = PlayerPrefs.GetString("mode_" + 11 + "_lowFourTimeMoneyBallAppVersion");
        string m11Date = PlayerPrefs.GetString("mode_" + 11 + "_lowFourTimeMoneyBallDate");
        string m11Level = PlayerPrefs.GetString("mode_" + 11 + "_lowFourTimeMoneyBallLevel");
        string m11Player = PlayerPrefs.GetString("mode_" + 11 + "_lowFourTimeMoneyBallPlayer");
        string m11os = PlayerPrefs.GetString("mode_" + 11 + "_operatingSystem");
        string m11version = PlayerPrefs.GetString("mode_11_longestShotMadeFreePlayAppVersion");

        float m12score = PlayerPrefs.GetFloat("mode_" + 12 + "_lowAllTimeMoneyBall");
        string m12Version = PlayerPrefs.GetString("mode_" + 12 + "_lowAllTimeMoneyBallAppVersion");
        string m12Date = PlayerPrefs.GetString("mode_" + 12 + "_lowAllTimeMoneyBallDate");
        string m12Level = PlayerPrefs.GetString("mode_" + 12 + "_lowAllTimeMoneyBallLevel");
        string m12Player = PlayerPrefs.GetString("mode_" + 12 + "_lowAllTimeMoneyBallPlayer");
        string m12os = PlayerPrefs.GetString("mode_" + 12 + "_operatingSystem");
        string m12version = PlayerPrefs.GetString("mode_12_longestShotMadeFreePlayAppVersion");

        float m13score = PlayerPrefs.GetFloat("mode_" + 13 + "_longestShotMadeFreePlay");
        string m13Version = PlayerPrefs.GetString("mode_" + 13 + "_longestShotMadeFreePlayAppVersion");
        string m13Date = PlayerPrefs.GetString("mode_" + 13 + "_longestShotMadeFreePlayDate");
        string m13Level = PlayerPrefs.GetString("mode_" + 13 + "_longestShotMadeFreePlayLevel");
        string m13Player = PlayerPrefs.GetString("mode_" + 13 + "_longestShotMadeFreePlayPlayer");
        string m13os = PlayerPrefs.GetString("mode_" + 13 + "_operatingSystem");
        string m13version = PlayerPrefs.GetString("mode_13_longestShotMadeFreePlayAppVersion");


        string sqlQuery1 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, totalPoints )  " +
            "Values( '" + 1 + "',  '"+ m1Player + "', '" + m1Level + "','" + m1os + "','" + m1version + "','" + m1Date + "','" + m1score+ "')";

        string sqlQuery2 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, maxShotMade )  " +
            "Values( '" + 2 + "',  '" + m2Player + "', '" + m2Level + "','" + m2os + "','" + m2version + "','" + m2Date + "','" + m2score + "')";


        string sqlQuery3 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, maxShotMade )  " +
            "Values( '" + 3 + "',  '" + m3Player + "', '" + m3Level + "','" + m3os + "','" + m3version + "','" + m3Date + "','" + m3score + "')";


        string sqlQuery4 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, maxShotMade )  " +
            "Values( '" + 4 + "',  '" + m4Player + "', '" + m4Level + "','" + m4os + "','" + m4version + "','" + m4Date + "','" + m4score + "')";


        string sqlQuery5 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, longestShot )  " +
            "Values( '" + 5 + "',  '" + m5Player + "', '" + m5Level + "','" + m5os + "','" + m5version + "','" + m5Date + "','" + m5score + "')";

        string sqlQuery6 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, totalDistance )  " +
            "Values( '" + 6 + "',  '" + m6Player + "', '" + m6Level + "','" + m6os + "','" + m6version + "','" + m6Date + "','" + m6score + "')";

        string sqlQuery7 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, time )  " +
            "Values( '" + 7 + "',  '" + m7Player + "', '" + m7Level + "','" + m7os + "','" + m7version + "','" + m7Date + "','" + m7score + "')";

        string sqlQuery8 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, time )  " +
            "Values( '" + 8 + "',  '" + m8Player + "', '" + m8Level + "','" + m8os + "','" + m8version + "','" + m8Date + "','" + m8score + "')";

        string sqlQuery9 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, time )  " +
            "Values( '" + 9 + "',  '" + m9Player + "', '" + m9Level + "','" + m9os + "','" + m9version + "','" + m9Date + "','" + m9score + "')";

        string sqlQuery10 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, time )  " +
            "Values( '" + 10 + "',  '" + m10Player + "', '" + m10Level + "','" + m10os + "','" + m10version + "','" + m10Date + "','" + m10score + "')";

        string sqlQuery11 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, time )  " +
            "Values( '" + 11 + "',  '" + m11Player + "', '" + m11Level + "','" + m11os + "','" + m11version + "','" + m11Date + "','" + m11score + "')";

        string sqlQuery12 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, time )  " +
            "Values( '" + 12 + "',  '" + m12Player + "', '" + m12Level + "','" + m12os + "','" + m12version + "','" + m12Date + "','" + m12score + "')";

        string sqlQuery13 =
            "INSERT INTO HighScores( modeid, character, level, os, version ,date, longestShot )  " +
            "Values( '" + 13 + "',  '" + m13Player + "', '" + m13Level + "','" + m13os + "','" + m13version + "','" + m13Date + "','" + m13score + "')";


        dbcmd.CommandText = sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery2;
        reader = dbcmd.ExecuteReader();
        reader.Close();

        dbcmd.CommandText = sqlQuery3;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery4;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery5;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery6;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery7;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery8;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery9;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery10;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery11;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery12;
        reader = dbcmd.ExecuteReader();
        reader.Close();


        dbcmd.CommandText = sqlQuery13;
        reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    //void InsertPrevVersionHighScoresToDB()
    //{
    //    Debug.Log("add prev high scores to DB");

    //    ///int maxGameModes = 13;

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();


    //    //string sqlQuery = "INSERT INTO User Values(?,?), ('" + uname + "', '" + pass + "')";


    //    string sqlQuery1 =
    //        "INSERT INTO HighScores( playerid, modeid, totalPoints )  " +
    //        "Values('" + currentPlayerId + "', '" + 1 + "',  '" + PlayerData.instance.TotalPoints + "')";

    //    string sqlQuery2 =
    //        "INSERT INTO HighScores( playerid, modeid, maxShotMade )  " +
    //        "Values('" + currentPlayerId + "', '" + 2 + "',  '" + PlayerData.instance.ThreePointerMade + "')";

    //    string sqlQuery3 =
    //        "INSERT INTO HighScores( playerid, modeid, maxShotMade )  " +
    //        "Values('" + currentPlayerId + "', '" + 3 + "',  '" + PlayerData.instance.FourPointerMade + "')";

    //    string sqlQuery4 =
    //        "INSERT INTO HighScores( playerid, modeid, maxShotMade )  " +
    //        "Values('" + currentPlayerId + "', '" + 4 + "',  '" + PlayerData.instance.SevenPointerMade + "')";

    //    string sqlQuery5 =
    //        "INSERT INTO HighScores( playerid, modeid, longestShot ) " +
    //        "Values('" + currentPlayerId + "', '" + 5 + "',  '" + PlayerData.instance.LongestShotMade + "')";

    //    string sqlQuery6 =
    //        "INSERT INTO HighScores( playerid, modeid, totalDistance )  " +
    //        "Values('" + currentPlayerId + "', '" + 6 + "',  '" + PlayerData.instance.TotalDistance + "')";

    //    string sqlQuery7 =
    //        "INSERT INTO HighScores( playerid, modeid, time )  " +
    //        "Values('" + currentPlayerId + "', '" + 7 + "',  '" + PlayerData.instance.MakeThreePointersLowTime + "')";

    //    string sqlQuery8 =
    //        "INSERT INTO HighScores( playerid, modeid, time ) " +
    //        "Values('" + currentPlayerId + "', '" + 8 + "',  '" + PlayerData.instance.MakeFourPointersLowTime + "')";

    //    string sqlQuery9 =
    //        "INSERT INTO HighScores( playerid, modeid, time )  " +
    //        "Values('" + currentPlayerId + "', '" + 9 + "',  '" + PlayerData.instance.MakeAllPointersLowTime + "')";

    //    string sqlQuery10 =
    //        "INSERT INTO HighScores( playerid, modeid, time ) " +
    //        " Values('" + currentPlayerId + "', '" + 10 + "',  '" + PlayerData.instance.MakeAllPointersMoneyBallLowTime + "')";

    //    string sqlQuery11 =
    //        "INSERT INTO HighScores( playerid, modeid, time )  " +
    //        "Values('" + currentPlayerId + "', '" + 11 + "',  '" + PlayerData.instance.MakeFourPointersMoneyBallLowTime + "')";

    //    string sqlQuery12 =
    //        "INSERT INTO HighScores( playerid, modeid, time )  " +
    //        "Values('" + currentPlayerId + "', '" + 12 + "',  '" + PlayerData.instance.MakeAllPointersMoneyBallLowTime + "')";

    //    string sqlQuery13 =
    //        "INSERT INTO HighScores( playerid, modeid, longestShot )  " +
    //        "Values('" + currentPlayerId + "', '" + 13 + "',  '" + PlayerData.instance.LongestShotMadeFreePlay + "')";


    //    dbcmd.CommandText = sqlQuery1;
    //    IDataReader reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery2;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();

    //    dbcmd.CommandText = sqlQuery3;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery4;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery5;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery6;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery7;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery8;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery9;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery10;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery11;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery12;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();


    //    dbcmd.CommandText = sqlQuery13;
    //    reader = dbcmd.ExecuteReader();
    //    reader.Close();

    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //}

    //int getCurrentGameVersionToInt(String version)
    //{
    //    // parse out ".", convert to int
    //    var temp = Regex.Replace(version, "[.]", "");
    //    var versionInt = Int16.Parse(temp);

    //    return versionInt;
    //}

    //int getPrevHighScoreInserted()
    //{
    //    int value = 0;

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "SELECT prevScoresInserted from User where userid = " + currentPlayerId;

    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        value = reader.GetInt32(0);
    //        Debug.Log(" value = " + value);
    //    }
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //    return value;
    //}

    //int setPrevHighScoreInsertedTrue()
    //{
    //    int value = 0;

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "Update User set prevScoresInserted  = 1 ";

    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        value = reader.GetInt32(0);
    //        Debug.Log(" value = " + value);
    //    }
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //    return value;
    //}

    ////void SetCurrentUserDevice()
    ////{
    ////    dbconn = new SqliteConnection(connection);
    ////    dbconn.Open();
    ////    dbcmd = dbconn.CreateCommand();

    ////    string version = Application.version;
    ////    string os = SystemInfo.operatingSystem;

    ////    String sqlQuery = "Update User SET os = '" + os + "', version = '" + version + "'WHERE id = 1 ";

    ////    dbcmd.CommandText = sqlQuery;
    ////    dbcmd.ExecuteScalar();
    ////    dbconn.Close();
    ////}

    //// For Strings
    //// create a helper class with overrides for other types
    //String getAllValuesFromTableByField(String tableName, String field)
    //{

    //    String value = null;

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "SELECT " + field + " FROM " + tableName;

    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        value = reader.GetString(0);
    //        Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
    //    }
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //    return value;
    //}

    //String getStringValueFromTableByFieldAndId(String tableName, String field, int userid)
    //{

    //    String value = null;

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = " + userid;

    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        //int value = reader.GetInt32(0);
    //        value = reader.GetString(0);
    //        //string name = reader.GetString(1);
    //        //string email = reader.GetString(2);
    //        //string password = reader.GetString(3);
    //        ////int rand = reader.GetInt32(2);

    //        Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
    //    }
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //    return value.ToString();
    //}

    //int getIntValueFromTableByFieldAndId(String tableName, String field, int userid)
    //{

    //    int value = 0;

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = " + userid;

    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        //int value = reader.GetInt32(0);
    //        value = reader.GetInt32(0);
    //        //string name = reader.GetString(1);
    //        //string email = reader.GetString(2);
    //        //string password = reader.GetString(3);
    //        ////int rand = reader.GetInt32(2);

    //        //Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
    //    }
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //    return value;
    //}


    //bool isTableEmpty(String tableName)
    //{
    //    int count = 0;

    //    dbconn = new SqliteConnection(connection);
    //    dbconn.Open();
    //    dbcmd = dbconn.CreateCommand();

    //    string version = Application.version;
    //    string os = SystemInfo.operatingSystem;

    //    String sqlQuery = "SELECT count(*) FROM '" + tableName + "'";

    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();

    //    while (reader.Read())
    //    {
    //        int value = reader.GetInt32(0); count = reader.GetInt16(0);
    //    }

    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;

    //    if (count == 0)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //void createDatabase()
    //{
    //    Debug.Log("create database");

    //    dbconn = new SqliteConnection(connection);
    //    dbconn.Open();
    //    dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = String.Format(
    //        "CREATE TABLE if not exists HighScores(" +
    //        "playerid  INTEGER," +
    //        "modeid    INTEGER UNIQUE," +
    //        "characterid   INTEGER, " +
    //        "levelid   INTEGER," +
    //        "os    TEXT," +
    //        "version   TEXT," +
    //        "date  TEXT," +
    //        "time  TEXT," +
    //        "totalPoints   INTEGER," +
    //        "longestShot   REAL," +
    //        "totalDistance REAL);" +

    //        "CREATE TABLE if not exists User( " +
    //        "id    INTEGER, " +
    //        "userName  INTEGER, " +
    //        "firstName TEXT, " +
    //        "middleName    INTEGER, " +
    //        "lastName  INTEGER, " +
    //        "email TEXT, " +
    //        "password  TEXT, " +
    //        "version   TEXT, " +
    //        "os    TEXT, " +
    //        "PRIMARY KEY(id));"
    //    );

    //    dbcmd.CommandText = sqlQuery;
    //    dbcmd.ExecuteScalar();
    //    dbconn.Close();

    //}

    //void ReadDatabase()
    //{
    //    Debug.Log("read database");

    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(connection);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "SELECT id, name, email, password " + "FROM User";
    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();
    //    while (reader.Read())
    //    {
    //        //int value = reader.GetInt32(0);
    //        int id = reader.GetInt16(0);
    //        string name = reader.GetString(1);
    //        string email = reader.GetString(2);
    //        string password = reader.GetString(3);
    //        //int rand = reader.GetInt32(2);

    //        Debug.Log("id = " + id + " | name =" + name + " | email = " + email + " | password : " + password);
    //    }
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;
    //}

    ////void AddUser()
    ////{
    ////    Debug.Log("add user to database");

    ////    string conn = "URI=file:" + Application.dataPath + "/level5.db"; //Path to database.
    ////    IDbConnection dbconn;
    ////    dbconn = (IDbConnection)new SqliteConnection(conn);
    ////    dbconn.Open(); //Open connection to the database.
    ////    IDbCommand dbcmd = dbconn.CreateCommand();

    ////    string sqlQuery = "INSERT INTO User Values(?,?), ('" + uname + "', '" + pass + "')";
    ////    dbcmd.CommandText = sqlQuery;
    ////    IDataReader reader = dbcmd.ExecuteReader();
    ////    //while (reader.Read())
    ////    //{
    ////    //    //int value = reader.GetInt32(0);
    ////    //    int id = reader.GetInt16(0);
    ////    //    string name = reader.GetString(1);
    ////    //    string email = reader.GetString(2);
    ////    //    string password = reader.GetString(3);
    ////    //    //int rand = reader.GetInt32(2);

    ////    //    Debug.Log("id = " + id + " | name =" + name + " | email = " + email + " | password : " + password);
    ////    //}
    ////    reader.Close();
    ////    reader = null;
    ////    dbcmd.Dispose();
    ////    dbcmd = null;
    ////    dbconn.Close();
    ////    dbconn = null;
    ////}
}





