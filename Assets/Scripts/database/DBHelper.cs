using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Mono.Data.Sqlite;
using UnityEngine;
using Object = System.Object;

public class DBHelper : MonoBehaviour
{

    private String connection;
    private String databaseNamePath = "/level5.db";
    private String filepath;

    private const String allTimeStatsTableName = "AllTimeStats";
    private const String hitByCarTableName = "HitByCar";
    private const String achievementTableName = "Achievements";

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    public static DBHelper instance;

    void Awake()
    {
        instance = this;
        connection = "URI=file:" + Application.persistentDataPath + databaseNamePath; //Path to database
        filepath = Application.persistentDataPath + databaseNamePath;
    }

    // check if specified table is emoty
    public bool isTableEmpty(String tableName)
    {
        int count = 0;

        dbconn = new SqliteConnection(connection);
        dbconn.Open();
        dbcmd = dbconn.CreateCommand();

        String sqlQuery = "SELECT count(*) FROM '" + tableName + "'";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            int value = reader.GetInt32(0);
            count = value;
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        //if table contains records
        if (count == 0)
        {
            return true;
        }
        return false;
    }

    // list of string values by table/field
    public List<String> getStringListOfAllValuesFromTableByField(String tableName, String field)
    {
        List<String> listOfValues = new List<string>();
        String value;

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
            listOfValues.Add(value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }

    internal void InsertDefaultUserRecord()
    {

        //Debug.Log("InsertDefaultUserRecord");

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery1 =
           "INSERT INTO User( " +
           "id, " +
           "userName, " +
           "firstName," +
           "middleName," +
           "lastName," +
           "email," +
           "password," +
           "version," +
           "os, " +
           "prevScoresInserted)  " +

           "Values( '" + 1
           + "', '" + "placeholder"
           + "','" + "placeholder"
           + "','" + "placeholder"
           + "','" + "placeholder"
           + "','" + "email@placeholder.com"
           + "','" + "password"
           + "','" + "6.9.420"
           + "','" + "os version"
           + "','" + 0 + "')";

        dbcmd.CommandText = sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    // insert current game's stats and score
    internal void InsertGameScore(BasketBallStats stats)
    {
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        int trafficEnabled = 0;
        if (GameOptions.trafficEnabled)
        {
            trafficEnabled = 1;
        }

        string sqlQuery1 =
           "INSERT INTO HighScores( modeid, characterid, character, levelid, level, os, version ,date, time, " +
           "totalPoints, longestShot, totalDistance, maxShotMade, maxShotAtt, consecutiveShots, trafficEnabled )  " +
           "Values( '" + GameOptions.gameModeSelected
           + "', '" + GameOptions.playerId
           + "', '" + GameOptions.playerDisplayName
           + "','" + GameOptions.levelId
           + "','" + GameOptions.levelDisplayName
           + "','" + SystemInfo.operatingSystem
           + "','" + Application.version
           + "','" + DateTime.Now
           + "','" + GameRules.instance.CounterTime
           + "','" + stats.TotalPoints
           + "','" + stats.LongestShotMade
           + "','" + stats.TotalDistance + "','"
           + stats.ShotMade + "','"
           + stats.ShotAttempt + "','"
           + stats.MostConsecutiveShots + "','"
           + trafficEnabled + "')";

        dbcmd.CommandText = sqlQuery1;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    internal BasketBallStats getAllTimeStats()
    {
        BasketBallStats prevStats = gameObject.AddComponent<BasketBallStats>();

        String sqlQuery = "";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        //Debug.Log("table empty : " + isTableEmpty(allTimeStatsTableName));

        if (!isTableEmpty(allTimeStatsTableName))
        {
            //Debug.Log(" table is not empty");
            sqlQuery = "Select  * From " + allTimeStatsTableName;

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                prevStats.TwoPointerMade = reader.GetInt32(0);
                prevStats.TwoPointerAttempts = reader.GetInt32(1);
                prevStats.ThreePointerMade = reader.GetInt32(2);
                prevStats.ThreePointerAttempts = reader.GetInt32(3);
                prevStats.FourPointerMade = reader.GetInt32(4);
                prevStats.FourPointerAttempts = reader.GetInt32(5);
                prevStats.SevenPointerMade = reader.GetInt32(6);
                prevStats.SevenPointerAttempts = reader.GetInt32(7);
                prevStats.MoneyBallMade = reader.GetInt32(8);
                prevStats.MoneyBallAttempts = reader.GetInt32(9);
                prevStats.TotalPoints = reader.GetInt32(10);
                prevStats.TotalDistance = reader.GetFloat(11);
                prevStats.TimePlayed = reader.GetFloat(12);
            }
        }
        Destroy(prevStats, 5);
        return prevStats;
    }

    internal List<PlayerData.HitByCar> getPrevHitByCarStats()
    {
        List<PlayerData.HitByCar> prevStats = new List<PlayerData.HitByCar>();

        String sqlQuery = "";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        if (!isTableEmpty(hitByCarTableName))
        {
            sqlQuery = "Select  * From " + hitByCarTableName;

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                int vehicleId = reader.GetInt32(1);
                int count = reader.GetInt32(2);
                //Debug.Log(" vid : " + vehicleId + " count : " + count);
                prevStats.Add(new PlayerData.HitByCar(vehicleId, count));
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
        return prevStats;
    }

    internal List<Achievement> getAchievementStats()
    {
        List<Achievement> achieveStats = new List<Achievement>();

        String sqlQuery = "";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        if (!isTableEmpty(achievementTableName))
        {

            //sqlQuery = "Select aid, activevalue_progress_int, islocked From " + achievementTableName + " WHERE activevalue_progress_int IS NOT NULL";
            sqlQuery = "Select aid, activevalue_int, activevalue_progress_int, islocked From " + achievementTableName;
            //Debug.Log(sqlQuery);

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                int activateInt;
                int progressInt;
                //Debug.Log(" reader.GetInt32(1) : " + reader.GetInt32(0) + " reader.GetInt32(2) : " + reader.GetInt32(1));
                int aid = reader.GetInt32(0);
                // check for null activate value
                if (reader.IsDBNull(1))
                {
                    activateInt = 0;
                }
                else
                {
                    activateInt = reader.GetInt32(1);
                }
                // null check for progression value
                if (reader.IsDBNull(2))
                {
                    progressInt = 0;
                }
                else
                {
                    progressInt = reader.GetInt32(2);
                }
                int islocked = reader.GetInt32(3);
                //Debug.Log("db prgress : " + progressInt);

                achieveStats.Add(new Achievement(aid, activateInt, progressInt, islocked));
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
        }
        return achieveStats;
    }

    internal void UpdateAllTimeStats(BasketBallStats stats)
    {
        String sqlQuery = "";
        // get prev stats that current stats will be added to
        BasketBallStats prevStats = getAllTimeStats();

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        if (isTableEmpty(allTimeStatsTableName))
        {
            sqlQuery =
           "Insert INTO " + allTimeStatsTableName + " ( twoMade, twoAtt, threeMade, threeAtt, fourMade, FourAtt, sevenMade, " +
           "sevenAtt, totalPoints moneyBallMade, moneyBallAtt, totalDistance, timePlayed, longestShot)  " +
           "Values( '" +
           stats.TwoPointerMade + "', '" +
           stats.TwoPointerAttempts + "', '" +
           stats.ThreePointerMade + "','" +
           stats.ThreePointerAttempts + "','" +
           stats.FourPointerMade + "','" +
           stats.FourPointerAttempts + "','" +
           stats.SevenPointerMade + "','" +
           stats.SevenPointerAttempts + "','" +
           stats.TotalPoints + "','" +
           stats.MoneyBallMade + "','" +
           stats.MoneyBallAttempts + "','" +
           stats.TotalDistance + "','" +
           stats.TimePlayed + "," +
           stats.LongestShotMade + "')";
        }
        else
        {
            sqlQuery =
           "Update " + allTimeStatsTableName +
           " SET" +
           " twoMade = " + (prevStats.TwoPointerMade += stats.TwoPointerMade) +
           ", twoAtt = " + (prevStats.TwoPointerAttempts += stats.TwoPointerAttempts) +
           ", threeMade = " + (prevStats.ThreePointerMade += stats.ThreePointerMade) +
           ", threeAtt = " + (prevStats.ThreePointerAttempts += stats.ThreePointerAttempts) +
           ", fourMade = " + (prevStats.FourPointerMade += stats.FourPointerMade) +
           ", FourAtt = " + (prevStats.FourPointerAttempts += stats.FourPointerAttempts) +
           ", sevenMade = " + (prevStats.SevenPointerMade += stats.SevenPointerMade) +
           ", sevenAtt = " + (prevStats.SevenPointerAttempts += stats.SevenPointerAttempts) +
           ", moneyBallMade = " + (prevStats.MoneyBallMade += stats.MoneyBallMade) +
           ", moneyBallAtt = " + (prevStats.MoneyBallAttempts += stats.MoneyBallAttempts) +
           ", totalPoints = " + (prevStats.TotalPoints += stats.TotalPoints) +
           ", totalDistance =" + (prevStats.TotalDistance += stats.TotalDistance) +
           ", timePlayed = " + (prevStats.TimePlayed += stats.TimePlayed) +
           " WHERE ROWID = 1 ";
        }

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        reader.Close();

        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    internal void UpdateHitByCarStats()
    {
        List<PlayerData.HitByCar> prevHitByCarStats = getPrevHitByCarStats();
        String sqlQuery = "";

        var dbconn = new SqliteConnection(connection);
        using (dbconn)
        {
            dbconn.Open(); //Open connection to the database.
            using (SqliteTransaction tr = dbconn.BeginTransaction())
            {
                using (SqliteCommand cmd = dbconn.CreateCommand())
                {
                    cmd.Transaction = tr;
                    foreach (PlayerData.HitByCar hbc in PlayerData.instance.hitByCars)
                    {
                        bool entryExists = prevHitByCarStats.Any(x => x.vehicleId == hbc.vehicleId);
                        if (entryExists)
                        {
                            int prevCount = prevHitByCarStats.Where(x => x.vehicleId == hbc.vehicleId).SingleOrDefault().counter;
                            // if entry is NOT in list of stats
                            sqlQuery =
                            "UPDATE " + hitByCarTableName + " SET count = " + (prevCount + hbc.counter) + " WHERE vehicleId = " + hbc.vehicleId;
                        }
                        if (!entryExists)
                        {
                            // if entry is NOT in list of stats
                            sqlQuery =
                            "Insert INTO " + hitByCarTableName + " ( vehicleId, count) "
                            + " Values( '" + hbc.vehicleId + "', '" + hbc.counter + "')";
                            // else update hbc count + prev.count
                        }
                        cmd.CommandText = sqlQuery;
                        cmd.ExecuteNonQuery();
                    }
                }
                tr.Commit();
            }
            dbconn.Close();
        }
        //reset list of hit by cars
        PlayerData.instance.hitByCars.Clear();
    }

    internal void UpdateAchievementStats()
    {
        // need to save/ load data to database

        List<Achievement> achievementsList = getAchievementStats();
        String sqlQuery = "";

        var dbconn = new SqliteConnection(connection);
        using (dbconn)
        {
            dbconn.Open(); //Open connection to the database.
            using (SqliteTransaction tr = dbconn.BeginTransaction())
            {
                using (SqliteCommand cmd = dbconn.CreateCommand())
                {
                    cmd.Transaction = tr;
                    foreach (Achievement ach in AchievementManager.instance.AchievementList)
                    {
                        bool entryExists = achievementsList.Any(x => x.achievementId == ach.achievementId);
                        Achievement prevAchieve;
                        //if (entryExists)
                        //{
                        //    Debug.Log("aid : " + ach.achievementId + " exists : " + entryExists + " activaevalue_int : "+ ach.ActivationValueInt);
                        //}

                        if (entryExists)
                        {
                            //get object
                            prevAchieve = achievementsList.Where(x => x.achievementId == ach.achievementId).Single();

                            int achieveIsLocked; // default (1 ) ==  islocked = true
                            // if this achievement has been unlocked 
                            if (!ach.IsLocked && prevAchieve.IsLocked)
                            {
                                achieveIsLocked = 0;
                                // if entry IS in list of stats
                                sqlQuery =
                                "UPDATE " + achievementTableName + " SET islocked = " + achieveIsLocked
                                + " WHERE aid = " + ach.achievementId;
                                Debug.Log(sqlQuery);
                            }
                            // if db is unlocked and current isnt
                            if (!prevAchieve.IsLocked && ach.IsLocked)
                            {
                                ach.IsLocked = false;
                            }

                            // if this achievement is still LOCKED but is a progressive count, and current value > db value ;update progression
                            if (ach.IsLocked && prevAchieve.IsLocked
                                && ach.IsProgressiveCount)
                            {
                                // if needs to pudate progression value
                                if (ach.ActivationValueProgressionInt > prevAchieve.ActivationValueProgressionInt)
                                {
                                    // if entry IS in list of stats
                                    sqlQuery =
                                    "UPDATE " + achievementTableName + " SET activevalue_progress_int = " + ach.ActivationValueProgressionInt
                                    + " WHERE aid = " + ach.achievementId;
                                    Debug.Log(sqlQuery);
                                }
                                if (prevAchieve.ActivationValueProgressionInt == 0 && ach.ActivationValueProgressionInt == 0)
                                {
                                    sqlQuery =
                                    "UPDATE " + achievementTableName + " SET activevalue_progress_int = " + 0
                                    + " WHERE aid = " + ach.achievementId;
                                    //Debug.Log(sqlQuery);
                                }
                            }

                            if (prevAchieve.ActivationValueInt == 0)
                            {
                                sqlQuery = "UPDATE " + achievementTableName + " SET activevalue_int = " + ach.ActivationValueInt
                                    + " WHERE aid = " + ach.achievementId;
                                Debug.Log(sqlQuery);
                            }

                        }
                        // if no entry in db. create entry with activate value, progress, and islocked = 1
                        if (!entryExists)
                        {
                            Debug.Log("no entry exists");
                            // 1 - locked, 0 - unlocked
                            int unlockAchievement = 1;
                            // if entry is NOT in list of stats
                            sqlQuery =
                            "Insert INTO "
                            + achievementTableName + " ( activevalue_int, activevalue_progress_int, islocked) "
                            + " Values('" + ach.ActivationValueInt + "', '" + ach.ActivationValueProgressionInt + "', '" + unlockAchievement + "')";
                            //Debug.Log(sqlQuery);
                        }
                        if (!String.IsNullOrEmpty(sqlQuery))
                        {
                            cmd.CommandText = sqlQuery;
                            cmd.ExecuteNonQuery();
                            //Debug.Log("cmd.ExecuteNonQuery(); ");
                        }
                    }
                }
                tr.Commit();
            }
            dbconn.Close();
        }
    }

    public List<int> getIntListOfAllValuesFromTableByField(String tableName, String field)
    {
        List<int> listOfValues = new List<int>();
        int value;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
            listOfValues.Add(value);
            //Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }

    public List<float> getFloatListOfAllValuesFromTableByField(String tableName, String field)
    {
        //Debug.Log("getListOfAllValuesFromTableByField()");
        List<float> listOfValues = new List<float>();
        float value;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
            listOfValues.Add(value);
            //Debug.Log("table = " + tableName + " | field =" + field + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;
    }

    // ***************************** get values by USER ID *******************************************
    // return string from specified table by field and userid
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
            value = reader.GetString(0);
            //Debug.Log("tablename = " + tableName + " | field =" + field + " | id = " + userid + " | value = " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value.ToString();
    }

    // return int from specified table by field and userid
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

    // return string from specified table by field and userid
    public float getFloatValueFromTableByFieldAndId(String tableName, String field, float userid)
    {

        float value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE userid = " + userid;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    // ***************************** get values by MODE ID *******************************************
    // return string from specified table by field and userid
    public int getIntValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order)
    {

        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName
            + " WHERE modeid = " + modeid + " ORDER BY " + field + "  " + order + "  LIMIT 1";

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

    public float getFloatValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order)
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        float value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName
            + " WHERE modeid = " + modeid + " ORDER BY " + field + " " + order + " LIMIT 1";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    public float getFloatValueHighScoreFromTableByField(String tableName, String field, String order)
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        float value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName
            + " ORDER BY " + field + " " + order + " LIMIT 1";

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    // return string from specified table by field and userid
    public float updateFloatValueByTableAndField(String tableName, String field, float value)
    {
        Debug.Log("save to db: " + tableName + "  " + field + "  " + value);
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // if entry is NOT in list of stats
        string sqlQuery =
        "UPDATE " + tableName + " SET " + field + " = " + value;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetFloat(0);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }
}
