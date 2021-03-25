
using Assets.Scripts.database;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Data;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class DBConnector : MonoBehaviour
{
    private String connection;
    private String databaseNamePath = "/level5.db";
    private String filepath;

    // table names
    const String tableNameAllTimeStats = "AllTimeStats";
    const String tableNameCharacterProfile = "CharacterProfile";
    const String tableNameCheerleaderProfile = "CheerleaderProfile";
    const String tableNameHighScores = "HighScores";
    const String tableNameUser = "User";
    const String verifyDatabaseSqlQuery = "SELECT name FROM sqlite_master WHERE type='table';";

    //private const int currentDatabaseAppVersion = 3;

    Text messageText;

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    bool databaseCreated = false;
    public bool DatabaseCreated { get => databaseCreated; }

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

        filepath = Application.persistentDataPath + databaseNamePath;
        connection = "URI=file:" + Application.persistentDataPath + databaseNamePath; //Path to database

        dbHelper = gameObject.GetComponent<DBHelper>();
    }

    void Start()
    {
        //// create database / add tables if not exist
        if (File.Exists(filepath))
        //|| !Application.version.Equals(getDatabaseVersion()))// && integrityCheck())
        {
            try
            {
                StartCoroutine(createDatabase());
            }
            catch (Exception e)
            {
                dbHelper.DatabaseLocked = false;
                Debug.Log("ERROR : " + e);
                return;
            }
        }
        // if database doesnt exist
        if (!File.Exists(filepath))
        {
            try
            {
                SqliteConnection.CreateFile(filepath);
                //Debug.Log("create file / !existed");
                StartCoroutine(dropDatabase());
                StartCoroutine(createDatabase());
            }
            catch (Exception e)
            {
                dbHelper.DatabaseLocked = false;
                Debug.Log("ERROR : " + e);
                return;
            }
        }
        if (getDatabaseVersion() < dbHelper.CurrentDatabaseAppVersion)
        {
            //StartCoroutine(dropDatabase());
            //StartCoroutine(createDatabase());
            StartCoroutine(dbHelper.UpgradeDatabaseToVersion3());
            //StartCoroutine(setDatabaseVersion());
        }
    }

    private void VerifyDatabase()
    {
        string version = "";

        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            //string os = SystemInfo.operatingSystem;

            String sqlQuery = verifyDatabaseSqlQuery;

            Debug.Log("sqlQuery : " + sqlQuery);

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                version = reader.GetString(0);
                Debug.Log(version);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            databaseCreated = true;
            dbHelper.DatabaseLocked = false;
        }
        catch (Exception e)
        {
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    private int getDatabaseVersion()
    {
        int value = 0;
        try
        {
            string sqlQuery = "PRAGMA user_version";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

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
        }
        catch (Exception e)
        {
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            return 0;
        }

        return value;
    }

    //public IEnumerator setDatabaseVersion()
    //{
    //    yield return new WaitUntil(() => !dbHelper.DatabaseLocked);
    //    try
    //    {
    //        dbHelper.DatabaseLocked = true;
    //        dbconn = new SqliteConnection(connection);
    //        dbconn.Open();
    //        dbcmd = dbconn.CreateCommand();

    //        string sqlQuery = "PRAGMA main.user_version = '" + currentDatabaseAppVersion + "'";

    //        dbcmd.CommandText = sqlQuery;
    //        dbcmd.ExecuteScalar();

    //        dbcmd.Dispose();
    //        dbcmd = null;
    //        dbconn.Close();
    //        dbconn = null;

    //        dbHelper.DatabaseLocked = false;
    //    }
    //    catch (Exception e)
    //    {
    //        dbHelper.DatabaseLocked = false;
    //    }
    //}


    // ============================ Save stats ===============================
    public void savePlayerGameStats(HighScoreModel dbHighScoreModel)
    {
        dbHelper.InsertGameScore(dbHighScoreModel);
    }

    public void savePlayerProfileProgression(float expGained)
    {
        dbHelper.UpdatePlayerProfileProgression(expGained);
    }

    public void savePlayerAllTimeStats(GameStats stats)
    {
        dbHelper.UpdateAllTimeStats(stats);
    }

    // =========================================================================

    // create tables if not created
    IEnumerator createDatabase()
    {
        yield return new WaitUntil(() => !dbHelper.DatabaseLocked);
        dbHelper.DatabaseLocked = true;
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format(

                //"DROP TABLE if exists HighScores; " +

                "CREATE TABLE if not exists HighScores(" +
                "scoreid   INTEGER PRIMARY KEY AUTOINCREMENT," +
                "scoreidUnique   TEXT," +
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
                "trafficEnabled	INTEGER DEFAULT 0," +
                "hardcoreEnabled INTEGER DEFAULT 0, " +
                "enemiesEnabled INTEGER DEFAULT 0, " +
                "enemiesKilled INTEGER DEFAULT 0," +
                "platform    TEXT," +
                "device    TEXT," +
                "ipaddress   TEXT," +
                "twoMade   INTEGER, " +
                "twoAtt    INTEGER, " +
                "threeMade INTEGER, " +
                "threeAtt  INTEGER, " +
                "fourMade  INTEGER, " +
                "fourAtt   INTEGER, " +
                "sevenMade INTEGER, " +
                "sevenAtt  INTEGER, " +
                "bonusPoints  INTEGER, " +
                "moneyBallMade  INTEGER, " +
                "moneyBallAtt  INTEGER, " +
                "submittedToApi  INTEGER, " +
                "userName  TEXT DEFAULT NULL, " +
                "sniperEnabled  INTEGER DEFAULT 0, " +
                "sniperMode  INTEGER DEFAULT 0, " +
                "sniperModeName  TEXT DEFAULT none, " +
                "sniperHits  INTEGER DEFAULT 0, " +
                "sniperShots  INTEGER DEFAULT 0); " +

                "DROP TABLE if exists Achievements; " +

                "CREATE TABLE if not exists AllTimeStats(" +
                "userid INTEGER UNIQUE," +
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
                "timePlayed   REAL," +
                "enemiesKilled INTEGER DEFAULT 0," +
                "sniperHits INTEGER DEFAULT 0," +
                "sniperShots INTEGER DEFAULT 0); " +

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

                //"DROP TABLE if exists Users; " +

                "CREATE TABLE if not exists User( " +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "userid INTEGER UNIQUE," +
                "username  TEXT UNIQUE, " +
                "firstname TEXT, " +
                "lastname  TEXT, " +
                "email TEXT, " +
                "ipaddress TEXT, " +
                "signupdate TEXT, " +
                "lastlogin TEXT, " +
                "password TEXT, " +
                "bearerToken TEXT);");

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbconn.Close();
            dbconn = null;

            dbcmd.Dispose();
            dbcmd = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            VerifyDatabase();

        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            dbHelper.DatabaseLocked = false;
            //return;
        }
    }

    IEnumerator dropDatabase()
    {
        yield return new WaitUntil(() => !dbHelper.DatabaseLocked);
        dbHelper.DatabaseLocked = true;

        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            // DROP TABLE [IF EXISTS] [schema_name.]table_name;
            string sqlQuery = String.Format(
                //"DROP TABLE if exists AllTimeStats; " +
                "DROP TABLE if exists Achievements; " +
                //"DROP TABLE if exists CharacterProfile; " +
                //"DROP TABLE if exists CheerleaderProfile; " +
                "DROP TABLE if exists HighScores; ");
            //"DROP TABLE if exists User; ");

            //Debug.Log(sqlQuery);

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            dbHelper.DatabaseLocked = false;
        }
        catch (Exception e)
        {
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            //return;
        }
    }

    public IEnumerator dropDatabaseTable(string tableName)
    {
        yield return new WaitUntil(() => !dbHelper.DatabaseLocked);
        dbHelper.DatabaseLocked = true;

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
            dbHelper.DatabaseLocked = false;
        }
        catch (Exception e)
        {
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }

    public bool tableExists(string tableName)
    {

        int count = 0;
        string value = null;

        try
        {
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
                dbHelper.DatabaseLocked = false;
                return true;
            }
            else
            {
                dbHelper.DatabaseLocked = false;
                return false;
            }
        }
        catch (Exception e)
        {
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
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

            //Debug.Log(sqlQuery);

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
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    public void createTableCheerleaderProfile()
    {
        try
        {
            //Debug.Log(" public void createTableCheerleaderProfile()");

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

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
        catch (Exception e)
        {
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    public IEnumerator createTableUser()
    {
        //Debug.Log("createDatabase()");
        yield return new WaitUntil(() => !dbHelper.DatabaseLocked);

        dbHelper.DatabaseLocked = true;
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format(
                "CREATE TABLE if not exists User( " +
                "id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                "userid INTEGER UNIQUE," +
                "username  TEXT UNIQUE, " +
                "firstname TEXT, " +
                "lastname  TEXT, " +
                "email TEXT, " +
                "ipaddress TEXT, " +
                "signupdate TEXT, " +
                "lastlogin TEXT, " +
                "password TEXT, " +
                "bearerToken TEXT);");

            //"isLocked   INTEGER DEFAULT 1);");

            //Debug.Log(sqlQuery);

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
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }

    public IEnumerator createTableAllTimeStats()
    {
        //Debug.Log("createDatabase()");
        yield return new WaitUntil(() => !dbHelper.DatabaseLocked);

        dbHelper.DatabaseLocked = true;
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format(
                "CREATE TABLE if not exists AllTimeStats(" +
                "userid INTEGER UNIQUE," +
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
                "timePlayed   REAL," +
                "enemiesKilled INTEGER DEFAULT 0," +
                "sniperHits INTEGER DEFAULT 0," +
                "sniperShots INTEGER DEFAULT 0); ");

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
            dbHelper.DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }
}





