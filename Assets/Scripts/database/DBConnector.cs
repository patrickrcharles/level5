
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

    private String connection;
    private String databaseNamePath = "/level5.db";
    private int currentGameVersion;
    private String filepath;

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    TransferPlayerPrefScoresToDB transferPlayerPrefScoresToDB;


    enum prevVersionsWithNoDB
    {
        v1 = 201,
        v2 = 300,
        v3 = 301,
        v4 = 302
    }; 


    void Start()
    {
        connection = "URI=file:" + Application.dataPath + databaseNamePath; //Path to database
        filepath = Application.dataPath + databaseNamePath;
        currentGameVersion = getCurrentGameVersionToInt(Application.version);
        currentPlayerId =  getIntValueFromTableByFieldAndId("User", "userid", 1) ;
        Debug.Log( "currentPlayerId : "+ currentPlayerId);

        transferPlayerPrefScoresToDB = new TransferPlayerPrefScoresToDB();

        // if database doesnt exist
        if (!File.Exists(filepath))
        {
            createDatabase();
        }

        //if(currentGameVersion.Equals())

        //Debug.Log("enum test v1 : "+ (int)prevVersionsWithNoDB.v1);
        //Debug.Log("enum test v1 : "+ (int)prevVersionsWithNoDB.v2);
        //Debug.Log("enum test v1 : "+ (int)prevVersionsWithNoDB.v3);

        Debug.Log(" v to int " + getCurrentGameVersionToInt(Application.version));

        SetCurrentUserDevice();

        if (currentGameVersion == (int) prevVersionsWithNoDB.v1
            || currentGameVersion == (int) prevVersionsWithNoDB.v2
            || currentGameVersion == (int) prevVersionsWithNoDB.v3 
            || currentGameVersion == (int) prevVersionsWithNoDB.v4
            && getPrevHighScoreInserted() == 0)
        {
            Debug.Log(" put playerprefs into db");
            Debug.Log("long shot made : "+PlayerData.instance.LongestShotMade);

             transferPlayerPrefScoresToDB.InsertPrevVersionHighScoresToDB();
        }

        setPrevHighScoreInsertedTrue();

        //Debug.Log(isTableEmpty("User") );
        //Debug.Log(isTableEmpty("HighScores") );
        //getAllValuesFromTableByField("User", "userName");
        //getAllValuesFromTableByField("User", "email");
        //getAllValuesFromTableByField("User", "version");
        //getValueFromTableByFieldAndId("User", "email", 1);
    }

    void InsertPrevVersionHighScoresToDB()
    {
        Debug.Log("add prev high scores to DB");

        ///int maxGameModes = 13;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();


        //string sqlQuery = "INSERT INTO User Values(?,?), ('" + uname + "', '" + pass + "')";

        string sqlQuery1 =
            "INSERT INTO HighScores( playerid, modeid, totalPoints )  " +
            "Values('" + currentPlayerId + "', '" + 1 + "',  '" + PlayerData.instance.TotalPoints  + "')";

        string sqlQuery2 =
            "INSERT INTO HighScores( playerid, modeid, maxShotMade )  " +
            "Values('" + currentPlayerId + "', '" + 2 + "',  '" + PlayerData.instance.ThreePointerMade + "')";

        string sqlQuery3 =
            "INSERT INTO HighScores( playerid, modeid, maxShotMade )  " +
            "Values('" + currentPlayerId + "', '" + 3 + "',  '" + PlayerData.instance.FourPointerMade + "')";

        string sqlQuery4 =
            "INSERT INTO HighScores( playerid, modeid, maxShotMade )  " +
            "Values('" + currentPlayerId + "', '" + 4 + "',  '" + PlayerData.instance.SevenPointerMade + "')";

        string sqlQuery5 =
            "INSERT INTO HighScores( playerid, modeid, longestShot ) " +
            "Values('" + currentPlayerId + "', '" + 5 + "',  '" + PlayerData.instance.LongestShotMade + "')";

        string sqlQuery6 =
            "INSERT INTO HighScores( playerid, modeid, totalDistance )  " +
            "Values('" + currentPlayerId + "', '" + 6 + "',  '" + PlayerData.instance.TotalDistance + "')";

        string sqlQuery7 =
            "INSERT INTO HighScores( playerid, modeid, time )  " +
            "Values('" + currentPlayerId + "', '" + 7 + "',  '" + PlayerData.instance.MakeThreePointersLowTime + "')";

        string sqlQuery8 =
            "INSERT INTO HighScores( playerid, modeid, time ) " +
            "Values('" + currentPlayerId + "', '" + 8 + "',  '" + PlayerData.instance.MakeFourPointersLowTime + "')";

        string sqlQuery9 =
            "INSERT INTO HighScores( playerid, modeid, time )  " +
            "Values('" + currentPlayerId + "', '" + 9 + "',  '" + PlayerData.instance.MakeAllPointersLowTime + "')";

        string sqlQuery10 =
            "INSERT INTO HighScores( playerid, modeid, time ) " +
            " Values('" + currentPlayerId + "', '" + 10 + "',  '" + PlayerData.instance.MakeAllPointersMoneyBallLowTime + "')";

        string sqlQuery11 =
            "INSERT INTO HighScores( playerid, modeid, time )  " +
            "Values('" + currentPlayerId + "', '" + 11 + "',  '" + PlayerData.instance.MakeFourPointersMoneyBallLowTime + "')";

        string sqlQuery12 =
            "INSERT INTO HighScores( playerid, modeid, time )  " +
            "Values('" + currentPlayerId + "', '" + 12 + "',  '" + PlayerData.instance.MakeAllPointersMoneyBallLowTime + "')";

        string sqlQuery13 =
            "INSERT INTO HighScores( playerid, modeid, longestShot )  " +
            "Values('" + currentPlayerId + "', '" + 13 + "',  '" + PlayerData.instance.LongestShotMadeFreePlay + "')";


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


        //while (reader.Read())
        //{
        //    //int value = reader.GetInt32(0);
        //    int id = reader.GetInt16(0);
        //    string name = reader.GetString(1);
        //    string email = reader.GetString(2);
        //    string password = reader.GetString(3);
        //    //int rand = reader.GetInt32(2);

        //    Debug.Log("id = " + id + " | name =" + name + " | email = " + email + " | password : " + password);
        //}
    }


    int getCurrentGameVersionToInt(String version)
    {
        // parse out ".", convert to int
        var temp =  Regex.Replace(version, "[.]", "");
        var versionInt = Int16.Parse(temp);

        return versionInt;
    }

    int getPrevHighScoreInserted()
    {
        int value = 0; 

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT prevScoresInserted from User where userid = "+ currentPlayerId ;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
            Debug.Log(" value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

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
            Debug.Log(" value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

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

    // For Strings
    // create a helper class with overrides for other types
    String getAllValuesFromTableByField(String tableName, String field)
    {
 
        String value = null;

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
            Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    String getStringValueFromTableByFieldAndId(String tableName, String field, int userid)
    {

        String value = null;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = "+ userid;

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

    int getIntValueFromTableByFieldAndId(String tableName, String field, int userid)
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


    bool isTableEmpty(String tableName)
    {
        int count = 0;

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string version = Application.version;
        string os = SystemInfo.operatingSystem;

        String sqlQuery = "SELECT count(*) FROM '"+ tableName + "'";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            int value = reader.GetInt32(0);count = reader.GetInt16(0);
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

    void createDatabase()
    {
        Debug.Log("create database");

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        string sqlQuery = String.Format(
            "CREATE TABLE if not exists HighScores(" +
            "playerid  INTEGER," +
            "modeid    INTEGER UNIQUE," +
            "characterid   INTEGER, " +
            "levelid   INTEGER," +
            "os    TEXT," +
            "version   TEXT," +
            "date  TEXT," +
            "time  TEXT," +
            "totalPoints   INTEGER," +
            "longestShot   REAL," +
            "totalDistance REAL);" +

            "CREATE TABLE if not exists User( " +
            "id    INTEGER, " +
            "userName  INTEGER, " +
            "firstName TEXT, " +
            "middleName    INTEGER, " +
            "lastName  INTEGER, " +
            "email TEXT, " +
            "password  TEXT, " +
            "version   TEXT, " +
            "os    TEXT, " +
            "PRIMARY KEY(id));"
        );

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbconn.Close();

    }

    void ReadDatabase()
    {
        Debug.Log("read database");

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT id, name, email, password " + "FROM User";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            //int value = reader.GetInt32(0);
            int id = reader.GetInt16(0);
            string name = reader.GetString(1);
            string email = reader.GetString(2);
            string password = reader.GetString(3);
            //int rand = reader.GetInt32(2);

            Debug.Log("id = " + id + " | name =" + name + " | email = " + email + " | password : " + password);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    //void AddUser()
    //{
    //    Debug.Log("add user to database");

    //    string conn = "URI=file:" + Application.dataPath + "/level5.db"; //Path to database.
    //    IDbConnection dbconn;
    //    dbconn = (IDbConnection)new SqliteConnection(conn);
    //    dbconn.Open(); //Open connection to the database.
    //    IDbCommand dbcmd = dbconn.CreateCommand();

    //    string sqlQuery = "INSERT INTO User Values(?,?), ('" + uname + "', '" + pass + "')";
    //    dbcmd.CommandText = sqlQuery;
    //    IDataReader reader = dbcmd.ExecuteReader();
    //    //while (reader.Read())
    //    //{
    //    //    //int value = reader.GetInt32(0);
    //    //    int id = reader.GetInt16(0);
    //    //    string name = reader.GetString(1);
    //    //    string email = reader.GetString(2);
    //    //    string password = reader.GetString(3);
    //    //    //int rand = reader.GetInt32(2);

    //    //    Debug.Log("id = " + id + " | name =" + name + " | email = " + email + " | password : " + password);
    //    //}
    //    reader.Close();
    //    reader = null;
    //    dbcmd.Dispose();
    //    dbcmd = null;
    //    dbconn.Close();
    //    dbconn = null;
    //}
}





