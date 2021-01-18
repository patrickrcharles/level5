
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

    private const int currentDatabaseAppVersion = 2;

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
        //Debug.Log(filepath);
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
                createDatabase();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
        // if database doesnt exist
        if (!File.Exists(filepath))
        //|| !integrityCheck()
        //|| !Application.version.Equals(getDatabaseVersion()))
        {
            try
            {
                SqliteConnection.CreateFile(filepath);
                //Debug.Log("create file / !existed");
                dropDatabase();
                createDatabase();
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
        if (getDatabaseVersion() != currentDatabaseAppVersion)
        {
            StartCoroutine(dbHelper.UpgradeDatabaseToVersion2());
            StartCoroutine(setDatabaseVersion());
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

            //Debug.Log("sqlQuery : " + sqlQuery);

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                version = reader.GetString(0);
                //Debug.Log(version);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            databaseCreated = true;
            //Debug.Log("databaseCreated : " + databaseCreated);
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            //messageText.text += "\n" + e;
        }
    }

    private int getDatabaseVersion()
    {
        int value = 0;

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
            //Debug.Log("pragma version : " + value);
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    public IEnumerator setDatabaseVersion()
    {
        yield return new WaitUntil(() => !dbHelper.DatabaseLocked);
        dbHelper.DatabaseLocked = true;

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        //string sqlQuery = "SELECT prevScoresInserted from User where rowid = 1";

        //string sqlQuery = "PRAGMA integrity_check";
        string sqlQuery = "PRAGMA main.user_version = '" + currentDatabaseAppVersion + "'";

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        dbHelper.DatabaseLocked = false;

        //Debug.Log("db version : ");
        //Debug.Log("     current db version : " + getDatabaseVersion());
        ////Debug.Log("     sqlite getversion : " + dbconn.);
        //Debug.Log("     new db version : " + currentDatabaseAppVersion);
    }


    // ============================ Save stats ===============================
    public void savePlayerGameStats(BasketBallStats stats)
    {
        dbHelper.InsertGameScore(stats);
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

    // create tables if not created
    void createDatabase()
    {
        //Debug.Log("createDatabase()");
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = String.Format(
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
                "enemiesKilled INTEGER DEFAULT 0," +
                "platform    TEXT," +
                "device    TEXT," +
                "ipaddress   TEXT);" +

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
                "timePlayed   REAL," +
                "enemiesKilled INTEGER DEFAULT 0); " +

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
                "islocked  INTEGER DEFAULT 0);"+

                "CREATE TABLE if not exists User( " +
                "userid INTEGER PRIMARY KEY," +
                "userName  INTEGER, " +
                "firstName TEXT, " +
                "middleName    INTEGER, " +
                "lastName  INTEGER, " +
                "email TEXT, " +
                "password  TEXT);");

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
            return;
        }
    }

    void dropDatabase()
    {
        Debug.Log("drop database");
        try
        {
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            // DROP TABLE [IF EXISTS] [schema_name.]table_name;
            string sqlQuery = String.Format(
                "DROP TABLE if exists AllTimeStats; " +
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
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            //messageText.text += "\n" + e;
            //SendEmail.instance.//SendEmailOnEvent("ERROR : dropDatabase", e.ToString());
            return;
        }
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
            //messageText.text += "\n" + e;
            //string text = "ERROR : dropDatabaseTable(" + tableName + ")";
            //SendEmail.instance.//SendEmailOnEvent(text, e.ToString());
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
            Debug.Log("ERROR : " + e);
            //messageText.text += "\n" + e;
            //string text = "createTableCharacterProfile()";
            //SendEmail.instance.//SendEmailOnEvent(text, e.ToString());

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
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
    }
}





