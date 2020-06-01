﻿
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;


public class DBConnector : MonoBehaviour
{
    private int currentPlayerId;
    bool _created;
    private String connection;
    private String databaseNamePath = "/level5.db";
    private int currentGameVersion;
    private String filepath;

    const String tableNameHighScores = "HighScores";
    const String tableNameAllTimeStats = "AllTimeStats";
    const String tableNameUser = "User";

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    // move ld playerpref scores to db
    TransferPlayerPrefScoresToDB transferPlayerPrefScoresToDB;
    // DB function
    DBHelper dbHelper;

    public static DBConnector instance;

    // game versions that use playerprefs for scores
    enum prevVersionsWithNoDB
    {
        v1 = 201,
        v2 = 300,
        v3 = 301,
        v4 = 302
    };


    void Awake()
    {

       //Debug.Log(" DBconnector");

        instance = this;

        // only create player data once
        if (!_created)
        {
            DontDestroyOnLoad(this.gameObject);
            _created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }

       //Debug.Log(" DBconnector : Start");

        connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
       //Debug.Log(connection);
        filepath = Application.dataPath + databaseNamePath;
       //Debug.Log(filepath);
        currentGameVersion = getCurrentGameVersionToInt(Application.version);

        dbHelper = gameObject.GetComponent<DBHelper>();
        transferPlayerPrefScoresToDB = gameObject.GetComponent<TransferPlayerPrefScoresToDB>();

        // im the only person with a player id right now. this will come from  registration
        //currentPlayerId =  dbHelper.getIntValueFromTableByFieldAndId("User", "userid", 1) ;

        // if database doesnt exist
        if (!File.Exists(filepath))
        {
           //Debug.Log(" DBconnector : if (!File.Exists(filepath))");
            createDatabase();
        }
    }


    void Start()
    {


        //Debug.Log(" DBconnector : Start");

        //connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
        //Debug.Log(connection);
        //filepath = Application.dataPath + databaseNamePath;
        //Debug.Log(filepath);
        //currentGameVersion = getCurrentGameVersionToInt(Application.version);

        //dbHelper = gameObject.GetComponent<DBHelper>();
        //transferPlayerPrefScoresToDB = gameObject.GetComponent<TransferPlayerPrefScoresToDB>();

        //// im the only person with a player id right now. this will come from  registration
        ////currentPlayerId =  dbHelper.getIntValueFromTableByFieldAndId("User", "userid", 1) ;

        //// if database doesnt exist
        //if (!File.Exists(filepath))
        //{
        //   //Debug.Log(" DBconnector : if (!File.Exists(filepath))");
        //    createDatabase();
        //}

        //create default user 
        if (dbHelper.isTableEmpty(tableNameUser))
        {
            dbHelper.InsertDefaultUserRecord();
        }

        ////if (currentGameVersion.Equals())
        //Debug.Log("enum test v1 : " + (int)prevVersionsWithNoDB.v1);
        //Debug.Log("enum test v1 : " + (int)prevVersionsWithNoDB.v2);
        //Debug.Log("enum test v1 : " + (int)prevVersionsWithNoDB.v3);

        // get device user is currently using
        SetCurrentUserDevice();

        // check if scores need to be transferred, flag is set in User table
        if (getPrevHighScoreInserted() == 0)
        {
           //Debug.Log(" put playerprefs into db");
            // get object instance needed to transfer scores
            transferPlayerPrefScoresToDB.InsertPrevVersionHighScoresToDB();
            setPrevHighScoreInsertedTrue();
        }
        // setf lag that scores have been transferred and dont need to be transferred again

        //setPrevHighScoreInsertedTrue();

        //dbHelper.getStringListOfAllValuesFromTableByField(tableNameHighScores, "level");
        //Debug.Log(isTableEmpty("User") );
        //Debug.Log(isTableEmpty("HighScores") );
        //getAllValuesFromTableByField("User", "userName");
        //getAllValuesFromTableByField("User", "email");
        //getAllValuesFromTableByField("User", "version");
        //getValueFromTableByFieldAndId("User", "email", 1);
    }

    public void savePlayerGameStats(BasketBallStats stats)
    {
        dbHelper.InsertGameScore(stats);
    }

    public void savePlayerAllTimeStats(BasketBallStats stats)
    {
        dbHelper.UpdateAllTimeStats(stats);
    }

    // strip string to convert to an int that can be used for comparisons with enum (int)var
    int getCurrentGameVersionToInt(String version)
    {
        // parse out ".", convert to int
        var temp =  Regex.Replace(version, "[.]", "");
        var versionInt = Int16.Parse(temp);

        return versionInt;
    }
    // have high scores been transferred already. checks flag in User table
    int getPrevHighScoreInserted()
    {
       //Debug.Log("getPrevHighScoreInserted");
        int value = 0; 

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT prevScoresInserted from User where rowid = 1";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
           //Debug.Log(" value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }
    // set high sores transferred flag to true
    int setPrevHighScoreInsertedTrue()
    {
       //Debug.Log("insert previous high scores");

        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "Update User set prevScoresInserted  = 1 ";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
           //Debug.Log(" value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }
    // set user's current device in User table
    void SetCurrentUserDevice()
    {
        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string version = Application.version;
        string os = SystemInfo.operatingSystem;

        String sqlQuery = "Update User SET os = '" + os + "', version = '" + version + "'WHERE id = 1 ";

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbconn.Close();

    }

    // create tables if not created
    void createDatabase()
    {
       //Debug.Log("create database");

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format(
            "CREATE TABLE if not exists HighScores(" +
            "playerid  INTEGER," +
            "modeid    INTEGER, " +
            "characterid   INTEGER, " +
            "character   TEXT, " +
            "levelid   INTEGER, " +
            "level    TEXT, " +
            "os    TEXT, " +
            "version   TEXT, " +
            "date  TEXT, " +
            "time  TEXT, " +
            "totalPoints   INTEGER, " +
            "longestShot   REAL, " +
            "totalDistance REAL, " +
            "maxShotMade   INTEGER, "+
            "maxShotAtt    INTEGER, " +
            "consecutiveShots   INTEGER); " +

            "CREATE TABLE if not exists AllTimeStats(" +
            "twoMade   INTEGER, " +
            "twoAtt    INTEGER, " +
            "threeMade INTEGER, " +
            "threeAtt  INTEGER, " +
            "fourMade  INTEGER, " +
            "fourAtt   INTEGER, " +
            "sevenMade INTEGER, " +
            "sevenAtt  INTEGER, " +
            "moneyBallMade INTEGER, " +
            "moneyBallAtt  INTEGER, " +
            "totalDistance REAL, " +
            "timePlayed    REAL, "+
            "consecutiveShots    INTEGER);" +

            "CREATE TABLE if not exists User( " +
            "id    INTEGER PRIMARY KEY, " +
            "userName  INTEGER, " +
            "firstName TEXT, " +
            "middleName    INTEGER, " +
            "lastName  INTEGER, " +
            "email TEXT, " +
            "password  TEXT, " +
            "version   TEXT, " +
            "os    TEXT, " +
            "prevScoresInserted  INTEGER DEFAULT 0 NOT NULL);");

       //Debug.Log(sqlQuery);

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

       //Debug.Log("close database on CREATE");
    }
}





