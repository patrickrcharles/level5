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

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;


    void Start()
    {
        connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
        filepath = Application.dataPath + databaseNamePath;
    }

    // check if spcified table is emoty
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
            int value = reader.GetInt32(0); count = reader.GetInt16(0);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        if (count == 0)
        {
            return true;
        }
        return false;
    }

    // rewrite function to insert relevant game mode data based on current mode
    //**** should be called in GameRules.cs
    public void InsertGameModeSaveData( BasketBallStats stats)
    {
        Debug.Log("InsertPrevVersionHighScoresToDB()");
        connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
        filepath = Application.dataPath + databaseNamePath;

        Debug.Log("connection : " + connection);

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        /*what to save 
         * modeid
         * character
         * characterid
         * level
         * levelid
         * os
         * version
         * date
         * time
         * totalpoints
         * long shot
         * total distance
         * max shots made
         */

        string sqlQuery1 = "";
        //    "INSERT INTO HighScores( modeid, character, level, os, version ,date, totalPoints )  " +
        //    "Values( '" + 1 + "',  '" + m1Player + "', '" + m1Level + "','" + m1os + "','" + m1version + "','" + m1Date + "','" + m1score + "')";

        dbcmd.CommandText =sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public List<String> getStringListOfAllValuesFromTableByField(String tableName, String field)
    {
        Debug.Log("getListOfAllValuesFromTableByField()");
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

            Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }


    internal void InsertGameScore(BasketBallStats stats)
    {
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery1 =
           "INSERT INTO HighScores( modeid, characterid, character, levelid, level, os, version ,date, time, " +
           "totalPoints, longestShot, totalDistance, maxShotMade )  " +
           "Values( '" + GameOptions.gameModeSelected + "', '" + GameOptions.playerId + "', '" + GameOptions.playerDisplayName 
           + "','" + GameOptions.levelId + "','" + GameOptions.levelDisplayName + "','" + SystemInfo.operatingSystem + "','" 
           + Application.version + "','" + DateTime.Now + "','" + GameRules.instance.CounterTime 
           + "','" + stats.TotalPoints   + "','" + stats.LongestShotMade + "','"+ stats.TotalDistance + "','"+ stats.ShotMade+"')";

        dbcmd.CommandText = sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

    }

    internal void UpdateAllTimeStats(BasketBallStats stats)
    {
        throw new NotImplementedException();
    }


    public List<int> getIntListOfAllValuesFromTableByField(String tableName, String field)
    {
        Debug.Log("getListOfAllValuesFromTableByField()");
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

            Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
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
        Debug.Log("getListOfAllValuesFromTableByField()");
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

            Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }


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
            //int value = reader.GetInt32(0);
            value = reader.GetString(0);
            //string name = reader.GetString(1);
            //string email = reader.GetString(2);
            //string password = reader.GetString(3);
            ////int rand = reader.GetInt32(2);

            Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
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
            //int value = reader.GetInt32(0);
            value = reader.GetInt32(0);
            //string name = reader.GetString(1);
            //string email = reader.GetString(2);
            //string password = reader.GetString(3);
            ////int rand = reader.GetInt32(2);

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
            Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
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
