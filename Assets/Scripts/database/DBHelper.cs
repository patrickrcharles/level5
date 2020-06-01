using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine;

public class DBHelper : MonoBehaviour
{

    private String connection;
    private String databaseNamePath = "/level5.db";
    private String filepath;

    private const String allTimeStatsTableName = "AllTimeStats";

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    public static DBHelper instance;


    void Awake()
    {
        instance = this;
       //Debug.Log(" DBHelper : Awake");
        connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
        filepath = Application.dataPath + databaseNamePath;
    }

    // check if specified table is emoty
    public bool isTableEmpty(String tableName)
    {
        int count = 0;

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string version = Application.version;
        string os = SystemInfo.operatingSystem;

        String sqlQuery = "SELECT count(*) FROM '" + tableName + "'";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            int value = reader.GetInt32(0); 
            count = value;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        if (count == 0)
        {
           //Debug.Log(" is table empty : true");
            return true;
        }
       //Debug.Log(" is table empty : false");
        return false;
    }

    // list of string values by table/field
    public List<String> getStringListOfAllValuesFromTableByField(String tableName, String field)
    {
       //Debug.Log("getListOfAllValuesFromTableByField()");
        List<String> listOfValues = new List<string>();
        String value;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetString(0);
            listOfValues.Add(value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }

    internal void InsertDefaultUserRecord()
    {

       //Debug.Log("InsertDefaultUserRecord");

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery1 =
           "INSERT INTO User( " +
           "id, " +
           "userName, " +
           "firstName," +
           "middleName," +
           "lastName," +
           "email," +
           "password," +
           "version," +
           "os, " +
           "prevScoresInserted)  " +

           "Values( '" + 1
           + "', '" + "placeholder"
           + "','" + "placeholder"
           + "','" + "placeholder"
           + "','" + "placeholder"
           + "','" + "email@placeholder.com"
           + "','" + "password"
           + "','" + "6.9.420"
           + "','" + "os version"
           + "','" + 0  + "')";

       //Debug.Log(sqlQuery1);

        dbcmd.CommandText = sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    // insert current game's stats and score
    internal void InsertGameScore(BasketBallStats stats)
    {
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery1 =
           "INSERT INTO HighScores( modeid, characterid, character, levelid, level, os, version ,date, time, " +
           "totalPoints, longestShot, totalDistance, maxShotMade, maxShotAtt, consecutiveShots )  " +
           "Values( '" + GameOptions.gameModeSelected 
           + "', '" + GameOptions.playerId 
           + "', '" + GameOptions.playerDisplayName 
           + "','" + GameOptions.levelId 
           + "','" + GameOptions.levelDisplayName 
           + "','" + SystemInfo.operatingSystem 
           + "','" + Application.version 
           + "','" + DateTime.Now 
           + "','" + GameRules.instance.CounterTime 
           + "','" + stats.TotalPoints   
           + "','" + stats.LongestShotMade 
           + "','"+ stats.TotalDistance + "','"
           + stats.ShotMade + "','" 
           + stats.ShotAttempt + "','"
           + stats.MostConsecutiveShots +  "')";

       //Debug.Log(sqlQuery1);

        dbcmd.CommandText = sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    internal BasketBallStats getAllTimeStats()
    {
        BasketBallStats prevStats = gameObject.AddComponent<BasketBallStats>();

        String sqlQuery = "";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

       //Debug.Log("table empty : " + isTableEmpty(allTimeStatsTableName));

        if (!isTableEmpty(allTimeStatsTableName))
        {
           //Debug.Log(" table is not empty");
            sqlQuery ="Select  * From " + allTimeStatsTableName;

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                prevStats.TwoPointerMade = reader.GetInt32(0);
                prevStats.TwoPointerAttempts = reader.GetInt32(1);
                prevStats.ThreePointerMade = reader.GetInt32(2);
                prevStats.ThreePointerAttempts = reader.GetInt32(3);
                prevStats.FourPointerMade = reader.GetInt32(4);
                prevStats.FourPointerAttempts = reader.GetInt32(5);
                prevStats.SevenPointerMade = reader.GetInt32(6);
                prevStats.SevenPointerAttempts = reader.GetInt32(7);
                prevStats.MoneyBallMade = reader.GetInt32(8);
                prevStats.MoneyBallAttempts = reader.GetInt32(9);
                prevStats.TotalDistance = reader.GetFloat(10);
                prevStats.TimePlayed = reader.GetFloat(11);
            }

           //Debug.Log(" prevstats : " + prevStats.TwoPointerMade + " / " + prevStats.TwoPointerAttempts);

        }
        Destroy(prevStats, 5);
        return prevStats;
    }

    internal void UpdateAllTimeStats(BasketBallStats stats)
    {
        String sqlQuery = "";
        BasketBallStats prevStats = getAllTimeStats();

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        if (isTableEmpty(allTimeStatsTableName))
        {
           //Debug.Log(" table is empty");
            sqlQuery =
           "Insert INTO " + allTimeStatsTableName + " ( twoMade, twoAtt, threeMade, threeAtt, fourMade, FourAtt, sevenMade, " +
           "sevenAtt, moneyBallMade, moneyBallAtt, totalDistance, timePlayed)  " +
           "Values( '" + stats.TwoPointerMade + "', '" + stats.TwoPointerAttempts + "', '" + stats.ThreePointerMade
           + "','" + stats.ThreePointerAttempts + "','" + stats.FourPointerMade + "','" + stats.FourPointerAttempts + "','"
           + stats.SevenPointerMade + "','" + stats.SevenPointerAttempts + "','" + stats.MoneyBallMade + "','" + stats.MoneyBallAttempts
           + "','" + stats.TotalDistance + "','" + stats.TimePlayed + "')";
        }
        else
        {
           //Debug.Log(" table not empty");

            sqlQuery =
           "Update " + allTimeStatsTableName + 
           " SET" +
           " twoMade = " + (prevStats.TwoPointerMade += stats.TwoPointerMade) +
           ", twoAtt = " + (prevStats.TwoPointerAttempts += stats.TwoPointerAttempts) +
           ", threeMade = " + (prevStats.ThreePointerMade += stats.ThreePointerMade) +
           ", threeAtt = " + (prevStats.ThreePointerAttempts += stats.ThreePointerAttempts) +
           ", fourMade = " + (prevStats.FourPointerMade += stats.FourPointerMade) +
           ", FourAtt = " + (prevStats.FourPointerAttempts += stats.FourPointerAttempts) +
           ", sevenMade = " + (prevStats.SevenPointerMade += stats.SevenPointerMade) +
           ", sevenAtt = " + (prevStats.SevenPointerAttempts += stats.SevenPointerAttempts) +
           ", moneyBallMade = " + (prevStats.MoneyBallMade += stats.MoneyBallMade) +
           ", moneyBallAtt = " + (prevStats.MoneyBallAttempts += stats.MoneyBallAttempts) +
           ", totalDistance =" + (prevStats.TotalDistance += stats.TotalDistance) +
           ", timePlayed = " + (prevStats.TimePlayed += stats.TimePlayed) +
           " WHERE ROWID = 1 ";
        }

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public List<int> getIntListOfAllValuesFromTableByField(String tableName, String field)
    {
       //Debug.Log("getListOfAllValuesFromTableByField()");
        List<int> listOfValues = new List<int>();
        int value;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
            listOfValues.Add(value);
            //Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }

    public List<float> getFloatListOfAllValuesFromTableByField(String tableName, String field)
    {
       //Debug.Log("getListOfAllValuesFromTableByField()");
        List<float> listOfValues = new List<float>();
        float value;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
            listOfValues.Add(value);
            //Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }

    // ***************************** get values by USER ID *******************************************
    // return string from specified table by field and userid
    public String getStringValueFromTableByFieldAndId(String tableName, String field, int userid)
    {

        String value = null;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = " + userid;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetString(0);
            //Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value.ToString();
    }

    // return int from specified table by field and userid
    public int getIntValueFromTableByFieldAndId(String tableName, String field, int userid)
    {

        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = " + userid;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    // return string from specified table by field and userid
    public float getFloatValueFromTableByFieldAndId(String tableName, String field, float userid)
    {

        float value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = " + userid;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
            //Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    // ***************************** get values by MODE ID *******************************************
    // return string from specified table by field and userid
    public int getIntValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order)
    {
       //Debug.Log("getIntValueHighScoreFromTableByFieldAndModeId");

        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName 
            + " WHERE modeid = " + modeid + " ORDER BY " + field + "  "+ order +"  LIMIT 1";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
            //Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + modeid + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    public float getFloatValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order)
    {

        float value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName
            + " WHERE modeid = " + modeid + " ORDER BY " + field + " " + order + " LIMIT 1";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            //int value = reader.GetInt32(0);
            value = reader.GetFloat(0);
            //string name = reader.GetString(1);
            //string email = reader.GetString(2);
            //string password = reader.GetString(3);
            ////int rand = reader.GetInt32(2);

           //Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + modeid + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

}
