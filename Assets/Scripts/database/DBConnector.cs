
using System.Collections.Generic;
using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.IO;
using System.Threading;

public class DBConnector : MonoBehaviour
{
    private String uname = "hot lunch";
    private String pass = "teddy";

    private String connection;

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbcon;

    void Start()
    {
        connection = "URI=file:" + Application.dataPath + "/level5.db"; //Path to database
        String filepath = Application.dataPath + "/level5.db";

        // if database doesnt exist
        if (!File.Exists(filepath))
        {
            createDatabase();
        }

        SetCurrentUserDevice();
        Debug.Log(isTableEmpty("User") );
        Debug.Log(isTableEmpty("HighScores") );

        //getAllValuesFromTableByField("User", "userName");
        //getAllValuesFromTableByField("User", "email");
        //getAllValuesFromTableByField("User", "version");

        //getValueFromTableByFieldAndId("User", "email", 1);
    }

    void SetCurrentUserDevice()
    {
        dbcon = new SqliteConnection(connection);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

        string version = Application.version;
        string os = SystemInfo.operatingSystem;

        String sqlQuery = "Update User SET os = '" + os + "', version = '" + version + "'WHERE id = 1 ";

        Debug.Log("insertScore sql query : " + sqlQuery);
        Debug.Log("     version : "+version);
        Debug.Log("     os : "+ os);

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteScalar();
        dbcon.Close();

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

    String getValueFromTableByFieldAndId(String tableName, String field, int userid)
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

        return value;
    }

    bool isTableEmpty(String tableName)
    {
        int count = 0;

        dbcon = new SqliteConnection(connection);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

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
        dbcon.Close();
        dbcon = null;

        if (count == 0)
        {
            return true;
        }
        return false;
    }

    void createDatabase()
    {
        Debug.Log("create database");

        dbcon = new SqliteConnection(connection);
        dbcon.Open();
        dbcmd = dbcon.CreateCommand();

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
        dbcon.Close();

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

    void AddUser()
    {
        Debug.Log("add user to database");

        string conn = "URI=file:" + Application.dataPath + "/level5.db"; //Path to database.
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "INSERT INTO User Values(?,?), ('" + uname + "', '" + pass + "')";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
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
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
}





