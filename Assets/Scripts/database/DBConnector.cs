
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

        createDatabase();
        // if database doesnt exist
        if (!File.Exists(filepath))//|| !integrityCheck())
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

    private bool integrityCheck()
    {
        string value = "";

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        //string sqlQuery = "SELECT prevScoresInserted from User where rowid = 1";

        //string sqlQuery = "PRAGMA integrity_check";
        string sqlQuery = "pragma quick_check";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetString(0);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        if (value.ToLower() == "ok")
        {
            return true;
        }
        else
        {
            return false;
        }
    }


    private void schemaInfo()
    {
        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        //string sqlQuery = "SELECT prevScoresInserted from User where rowid = 1";

        //string sqlQuery = "PRAGMA integrity_check";
        //string sqlQuery = "PRAGMA table_info('Achievements')";
        string sqlQuery = "PRAGMA main.user_version";

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

        Debug.Log("schema app id: " + value);

        //if (value.ToLower() == "ok")
        //{
        //    return true;
        //}
        //else
        //{
        //    return false;
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

    public void savePlayerProfileProgression(float expGained)
    {
        dbHelper.UpdatePlayerProfileProgression(expGained);
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

            "CREATE TABLE if not exists Achievements(" +
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

            "CREATE TABLE if not exists CharacterProfile(" +
            "id   INTEGER PRIMARY KEY, " +
            "playerName   TEXT," +
            "objectName   TEXT," +
            "charid   INTEGER," +
            "accuracy2   INTEGER," +
            "accuracy3   INTEGER," +
            "accuracy4   INTEGER," +
            "accuracy7   INTEGER," +
            "jump   float," +
            "speed   float," +
            "runSpeed   float," +
            "runSpeedHasBall   float," +
            "luck   INTEGER," +
            "shootAngle   INTEGER," +
            "experience   INTEGER," +
            "level   INTEGER);" +

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
            "prevScoresInserted  INTEGER DEFAULT 0 NOT NULL);"); ;

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

    public void dropDatabaseTable(string tableName)
    {
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            // DROP TABLE [IF EXISTS] [schema_name.]table_name;
            string sqlQuery = String.Format(
                "DROP TABLE if exists " + tableName + ";");

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
        catch
        {
            return;
        }
    }

    public bool tableExists(string tableName)
    {

        int count = 0;
        string value = null;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format(
            "SELECT name FROM sqlite_master WHERE type = 'table' AND name = '" + tableName + "';");

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetString(0);
            count++;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        // if correct table name is returned and at least 1 table names exists
        if (count > 0 && value.Equals(tableName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void createTableCharacterProfile()
    {
        //Debug.Log("createDatabase()");
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format(
                "CREATE TABLE if not exists CharacterProfile(" +
                "id   INTEGER PRIMARY KEY, " +
                "charid   INTEGER," +
                "playerName   TEXT," +
                "objectName   TEXT," +
                "accuracy2   INTEGER," +
                "accuracy3   INTEGER," +
                "accuracy4   INTEGER," +
                "accuracy7   INTEGER," +
                "jump   float," +
                "speed   float," +
                "runSpeed   float," +
                "runSpeedHasBall   float," +
                "luck   INTEGER," +
                "shootAngle   INTEGER," +
                "experience   INTEGER," +
                "level   INTEGER);");

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbconn.Close();
            dbconn = null;

            dbcmd.Dispose();
            dbcmd = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        catch
        {
            return;
        }
    }

    public bool DatabaseCreated { get => databaseCreated; set => databaseCreated = value; }
}





