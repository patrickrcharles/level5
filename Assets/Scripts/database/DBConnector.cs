
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
    [SerializeField]
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

    DBHelper dbHelper;

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
            Debug.Log("created, destroy this");
            Destroy(gameObject);
        }

        connection = "URI=file:" + Application.persistentDataPath + databaseNamePath; //Path to database
        filepath = Application.persistentDataPath + databaseNamePath;
        currentGameVersion = getCurrentGameVersionToInt(Application.version);
        dbHelper = gameObject.GetComponent<DBHelper>();

        // if database doesnt exist
        if (!File.Exists(filepath))
        {
            Debug.Log(" DBconnector : if (!File.Exists(filepath))");
            createDatabase();
        }
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
    }

    public void savePlayerGameStats(BasketBallStats stats)
    {
        dbHelper.InsertGameScore(stats);
    }

    public void saveHitByCarGameStats()
    {
        dbHelper.UpdateHitByCarStats();
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
            "time  REAL, " +
            "totalPoints   INTEGER, " +
            "longestShot   REAL, " +
            "totalDistance REAL, " +
            "maxShotMade   INTEGER, " +
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
            "timePlayed    REAL, " +
            "consecutiveShots    INTEGER);" +

            "CREATE TABLE if not exists HitByCar( " +
            "id  INTEGER PRIMARY KEY AUTOINCREMENT, " +
            "vehicleId  INTEGER UNIQUE, " +
            "count INTEGER );" +

            "CREATE TABLE Achievements(" +
            "aid   INTEGER PRIMARY KEY AUTOINCREMENT, " +
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
            "unlocked  INTEGER );" +


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

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
}





