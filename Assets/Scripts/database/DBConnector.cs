
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections;

public class DBConnector : MonoBehaviour
{
    private String connection;
    private String databaseNamePath = "/level5.db";
    //private String currentGameVersion = "3.3.2";
    //private String previousGameVersion = "3.3.1";
    private String filepath;

    // table names
    const String tableNameAchievements = "Achievements";
    const String tableNameAllTimeStats = "AllTimeStats";
    const String tableNameCharacterProfile = "CharacterProfile";
    const String tableNameCheerleaderProfile = "CheerleaderProfile";
    const String tableNameHighScores = "HighScores";
    const String tableNameUser = "User";

    private string currentDatabaseAppVersion;

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    DBHelper dbHelper;
    public static DBConnector instance;

    void Awake()
    {
        // keep Database object persistent
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

        //// create database / add tables if not exist
        if (File.Exists(filepath))
            //|| !Application.version.Equals(getDatabaseVersion()))// && integrityCheck())
        {
            createDatabase();
            //try
            //{
            //    createDatabase();
            //}
            //catch (Exception e)
            //{
            //    Debug.Log("ERROR : " + e);
            //    return;
            //}
        }
        // if database doesnt exist
        if (!File.Exists(filepath)) 
            //|| !integrityCheck()
            //|| !Application.version.Equals(getDatabaseVersion()))
        {
            //databaseCreated = false;
            try
            {
                //Debug.Log("create database");
                dropDatabase();
                createDatabase();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
    }

    void Start()
    {

        //create default user 
        if (tableExists(tableNameUser))
        {
            if (dbHelper.isTableEmpty(tableNameUser))
            {
                Debug.Log("dbHelper.isTableEmpty(tableNameUser)");
                dbHelper.InsertDefaultUserRecord();
            }
        }

        // get device user is currently using
        SetCurrentUserDevice();
    }


    private string getDatabaseVersion()
    {
        string version = "";

        if (tableExists("User"))
        {
            try
            {
                dbconn = new SqliteConnection(connection);
                dbconn.Open();
                dbcmd = dbconn.CreateCommand();

                //string os = SystemInfo.operatingSystem;

                String sqlQuery = "SELECT version FROM User WHERE id = 1 ";

                //Debug.Log("sqlQuery : " + sqlQuery);

                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    version = reader.GetString(0);
                }

                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
            }
        }
        Debug.Log("db version : "+ version);
        return version;
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

    //IEnumerator updateAchievements()
    //{
    //    // wait for achievement list to be created
    //    yield return new WaitUntil(() => AchievementManager.instance.ListCreated == true);

    //    // check if dbhelper is null and if achievement table is null
    //    if (dbHelper != null )
    //    {
    //        try
    //        {
    //            dbHelper.UpdateAchievementStats();
    //        }
    //        catch
    //        {
    //            Debug.Log("exception : dbhelper == null");
    //        }
    //    }
    //}

    // ============================ Save stats ===============================
    public void savePlayerGameStats(BasketBallStats stats)
    {
        dbHelper.InsertGameScore(stats);
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

    // =========================================================================

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

            "CREATE TABLE if not exists Achievements(" +
            "aid   INTEGER PRIMARY KEY, " +
            "charid   INTEGER DEFAULT 0 ," +
            "cheerid   INTEGER DEFAULT 0 ," +
            "levelid   INTEGER DEFAULT 0 ," +
            "modeid    INTEGER DEFAULT 0 ," +
            "name  TEXT NOT NULL," +
            "description   TEXT NOT NULL," +
            "required_charid   INTEGER DEFAULT 0 ," +
            "required_cheerid   INTEGER DEFAULT 0 ," +
            "required_levelid  INTEGER DEFAULT 0 ," +
            "required_modeid   INTEGER DEFAULT 0 ," +
            "activevalue_int   INTEGER DEFAULT 0 ," +
            "activevalue_float REAL," +
            "activevalue_progress_int  INTEGER DEFAULT 0 ," +
            "activevalue_progress_float  REAL," +
            "islocked  INTEGER DEFAULT 0 );" +
            //"islocked  INTEGER INTEGER DEFAULT 1 );" +

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
            "experience   INTEGER DEFAULT 0," +
            "level   INTEGER DEFAULT 0," +
            "pointsAvailable   INTEGER DEFAULT 0," +
            "pointsUsed   INTEGER DEFAULT 0," +
            "range   INTEGER DEFAULT 0," +
            "release   INTEGER DEFAULT 0," +
            "isLocked   INTEGER DEFAULT 0);" +

            "CREATE TABLE if not exists CheerleaderProfile(" +
            "cid   INTEGER PRIMARY KEY, " +
            "name   TEXT NOT NULL," +
            "objectName   TEXT NOT NULL," +
            "unlockText   TEXT NOT NULL," +
            "islocked  INTEGER DEFAULT 0);" +
            //"islocked  INTEGER DEFAULT 1);" +

            "CREATE TABLE if not exists User( " +
            "id    INTEGER PRIMARY KEY, " +
            "userName  INTEGER, " +
            "firstName TEXT, " +
            "middleName    INTEGER, " +
            "lastName  INTEGER, " +
            "email TEXT, " +
            "password  TEXT, " +
            "version   TEXT, " +
            "os    TEXT );");

        Debug.Log(sqlQuery);

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
        Debug.Log("drop database");
        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        // DROP TABLE [IF EXISTS] [schema_name.]table_name;
        string sqlQuery = String.Format(
            "DROP TABLE if exists Achievements; " +
            "DROP TABLE if exists AllTimeStats; " +
            "DROP TABLE if exists CharacterProfile; " +
            "DROP TABLE if exists CheerleaderProfile; " +
            "DROP TABLE if exists HighScores; " +
            "DROP TABLE if exists User; ");

        Debug.Log(sqlQuery);

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
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
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
        Debug.Log("createDatabase()");
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format(
                "CREATE TABLE if not exists CharacterProfile(" +
                "id   INTEGER PRIMARY KEY, " +
                "charid   INTEGER," +
                "playerName   TEXT NOT NULL," +
                "objectName   TEXT NOT NULL," +
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
                "experience   INTEGER DEFAULT 0," +
                "level   INTEGER DEFAULT 0," +
                "pointsAvailable   INTEGER DEFAULT 0," +
                "pointsUsed   INTEGER DEFAULT 0," +
                "range   INTEGER DEFAULT 0," +
                "release   INTEGER DEFAULT 0," +
                "isLocked   INTEGER DEFAULT 0);");
            //"isLocked   INTEGER DEFAULT 1);");

            Debug.Log(sqlQuery);

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbconn.Close();
            dbconn = null;

            dbcmd.Dispose();
            dbcmd = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    public void createTableAchievements()
    {

        Debug.Log("createTableAchievements");

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery =
            "CREATE TABLE if not exists Achievements(" +
            "aid   INTEGER PRIMARY KEY, " +
            "charid   INTEGER DEFAULT 0," +
            "cheerid   INTEGER DEFAULT 0," +
            "levelid   INTEGER DEFAULT 0," +
            "modeid    INTEGER DEFAULT 0," +
            "name  TEXT NOT NULL," +
            "description   TEXT NOT NULL," +
            "required_charid   INTEGER DEFAULT 0," +
            "required_cheerid   INTEGER DEFAULT 0," +
            "required_levelid  INTEGER DEFAULT 0," +
            "required_modeid   INTEGER DEFAULT 0," +
            "activevalue_int   INTEGER DEFAULT 0," +
            "activevalue_float REAL DEFAULT 0," +
            "activevalue_progress_int  INTEGER DEFAULT 0," +
            "activevalue_progress_float    REAL DEFAULT 0," +
            "islocked  INTEGER DEFAULT 0);";
            //"islocked  INTEGER DEFAULT 1);";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }


    public void createTableCheerleaderProfile()
    {

        Debug.Log(" public void createTableCheerleaderProfile()");

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery =
            "CREATE TABLE if not exists CheerleaderProfile(" +
            "cid   INTEGER PRIMARY KEY, " +
            "name   TEXT NOT NULL," +
            "objectName   TEXT NOT NULL," +
            "unlockText   TEXT NOT NULL," +
            "islocked  INTEGER DEFAULT 0);";
            //"islocked  INTEGER DEFAULT 1);";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    //public bool DatabaseCreated { get => databaseCreated; set => databaseCreated = value; }
}





