﻿
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

public class DBConnector : MonoBehaviour
{
    private int currentPlayerId;
 
    private String connection;
    private String databaseNamePath = "/level5.db";
    private string currentGameVersion;
    private string previousGameVersion;
    private String filepath;

    const String tableNameHighScores = "HighScores";
    const String tableNameAllTimeStats = "AllTimeStats";
    const String tableNameUser = "User";
    const String tableNameAchievements = "Achievements";

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    DBHelper dbHelper;

    bool databaseCreated;

    public static DBConnector instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        connection = "URI=file:" + Application.persistentDataPath + databaseNamePath; //Path to database
        filepath = Application.persistentDataPath + databaseNamePath;
        dbHelper = gameObject.GetComponent<DBHelper>();

        // if database doesnt exist
        if (!File.Exists(filepath))
        {
            //databaseCreated = false;
            try
            {
                //Debug.Log("create database");
                dropDatabase();
                createDatabase();
            }
            catch
            {
                return;
            }
        }

        //previousGameVersion = "3.1.0";
        //currentGameVersion = Application.version;

        //// if user table not empty
        //if (!dbHelper.isTableEmpty(tableNameUser))
        //{
        //    previousGameVersion = dbHelper.getStringValueFromTableByFieldAndId("User", "version", 1);
        //}

        //try // check if old game version
        //{
        //    // if database does exist but isnt is outdated schema
        //    if (File.Exists(filepath) && !previousGameVersion.Equals(currentGameVersion))
        //    {
        //        databaseCreated = false;
        //        Debug.Log("if (File.Exists(filepath) && !previousGameVersion.Equals(currentGameVersion))");
        //        // drop tables
        //        dropDatabase();
        //        // create upgraded db
        //        createDatabase();
        //    }
        //}
        //catch // if db errors, drop tables and create db
        //{
        //    Debug.Log("catch : create database");
        //    // drop tables
        //    dropDatabase();
        //    // create upgraded db
        //    createDatabase();
        //}
    }

    void Start()
    {

        //create default user 
        if (dbHelper.isTableEmpty(tableNameUser))
        {
            dbHelper.InsertDefaultUserRecord();
        }

        // get device user is currently using
        SetCurrentUserDevice();

        // check for achievement manager
        // note - create game manager constants list for things like this
        if (GameObject.Find("achievement_manager") != null)
        {
            StartCoroutine(updateAchievements());
        }
    }

    private void Update()
    {
        //if (!EditorApplication.isPlayingOrWillChangePlaymode &&
        //    EditorApplication.isPlaying)
        //{
        //    Debug.Log("editor closing, close db conn");
        //    dbconn.Close();
        //}
    }

    IEnumerator updateAchievements()
    {
        // wait for achievement list to be created
        yield return new WaitUntil(() => AchievementManager.instance.ListCreated == true);

        // check if dbhelper is null and if achievement table is null
        if (dbHelper != null)
        {
            try
            {
                dbHelper.UpdateAchievementStats();
            }
            catch
            {
                Debug.Log("exception : dbhelper == null");
            }
        }
    }

    public void savePlayerGameStats(BasketBallStats stats)
    {
        dbHelper.InsertGameScore(stats);
    }

    public void saveHitByCarGameStats()
    {
        dbHelper.UpdateHitByCarStats();
    }

    public void saveAchievementStats()
    {
        dbHelper.UpdateAchievementStats();
    }

    public void savePlayerAllTimeStats(BasketBallStats stats)
    {
        dbHelper.UpdateAllTimeStats(stats);
    }

    // strip string to convert to an int that can be used for comparisons with enum (int)var
    int getCurrentGameVersionToInt(String version)
    {
        // parse out ".", convert to int
        var temp = Regex.Replace(version, "[.]", "");
        var versionInt = Int16.Parse(temp);

        return versionInt;
    }
    // have high scores been transferred already. checks flag in User table
    int getPrevHighScoreInserted()
    {
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
        //Debug.Log("createDatabase()");

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format(
            "CREATE TABLE if not exists HighScores(" +
            "scoreid   INTEGER PRIMARY KEY AUTOINCREMENT," +
            "playerid  INTEGER," +
            "modeid    INTEGER, " +
            "characterid   INTEGER, " +
            "character   TEXT, " +
            "levelid   INTEGER, " +
            "level    TEXT, " +
            "os    TEXT, " +
            "version   TEXT, " +
            "date  TEXT, " +
            "time  REAL, " +
            "totalPoints   INTEGER, " +
            "longestShot   REAL, " +
            "totalDistance REAL, " +
            "maxShotMade   INTEGER, " +
            "maxShotAtt    INTEGER, " +
            "consecutiveShots   INTEGER," +
            "trafficEnabled	INTEGER); " +

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
            "totalPoints  INTEGER, " +
            "totalDistance REAL, " +
            "longestShot REAL, " +
            "timePlayed    REAL);" +

            "CREATE TABLE if not exists HitByCar( " +
            "id  INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "vehicleId  INTEGER UNIQUE, " +
            "count INTEGER );" +

            "CREATE TABLE Achievements(" +
            "aid   INTEGER PRIMARY KEY, " +
            "charid   INTEGER," +
            "levelid   INTEGER," +
            "modeid    INTEGER," +
            "name  TEXT," +
            "description   TEXT," +
            "required_charid   INTEGER," +
            "required_levelid  INTEGER," +
            "required_modeid   INTEGER," +
            "activevalue_int   INTEGER," +
            "activevalue_float REAL," +
            "activevalue_progress_int  INTEGER," +
            "activevalue_progress_float    REAL," +
            "islocked  INTEGER );" +

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

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();

        dbconn.Close();
        dbconn = null;

        dbcmd.Dispose();
        dbcmd = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();

        //databaseCreated = true;

    }

    void dropDatabase()
    {

        //Debug.Log("dropDatabase()");

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        // DROP TABLE [IF EXISTS] [schema_name.]table_name;
        string sqlQuery = String.Format(
            "DROP TABLE if exists HighScores; " +

            "DROP TABLE if exists AllTimeStats; " +

            "DROP TABLE if exists HitByCar; " +

            "DROP TABLE if exists Achievements; " +

            "DROP TABLE if exists User; ");

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public bool DatabaseCreated { get => databaseCreated; set => databaseCreated = value; }
}





