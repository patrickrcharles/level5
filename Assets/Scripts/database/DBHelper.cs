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

    // For Strings
    // create a helper class with overrides for other types
    public String getAllValuesFromTableByField(String tableName, String field)
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
}
