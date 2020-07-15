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

        GC.Collect();
        GC.WaitForPendingFinalizers();

        //if table contains records
        if (count == 0)
        {
            //Debug.Log(tableName + " is empty");
            return true;
        }
        //Debug.Log(tableName + " is NOT empty");
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
                //Debug.Log("db aid" + aid + " islocked : " + islocked);
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
           "sevenAtt, totalPoints, moneyBallMade, moneyBallAtt, totalDistance, timePlayed, longestShot)  " +
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
           stats.TimePlayed + "','" +
           stats.LongestShotMade + "')";

            Debug.Log(sqlQuery);
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

    // this is a mess. needs to be redone and split into functions
    internal void UpdateAchievementStats()
    {
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
                    foreach (Achievement prefabAchievement in AchievementManager.instance.AchievementList)
                    {
                        bool entryExists = achievementsList.Any(x => x.achievementId == prefabAchievement.achievementId);
                        Achievement databaseAchievement;

                        if (entryExists)
                        {
                            //get achievement database object to update
                            databaseAchievement = achievementsList.Where(x => x.achievementId == prefabAchievement.achievementId).Single();
                            // if db is unlocked and current ISNT
                            if (databaseAchievement.IsLocked && !prefabAchievement.IsLocked)
                            {
                                prefabAchievement.IsLocked = true;
                            }
                            // if db is NOT unlocked and current IS. this a course correction. DB value takes precedent
                            if (!databaseAchievement.IsLocked && prefabAchievement.IsLocked)
                            {
                                prefabAchievement.IsLocked = false;
                            }

                            // if this achievement is LOCKED but is a progressive count, and current value > db value ;update progression
                            if (prefabAchievement.IsLocked && databaseAchievement.IsLocked
                                && prefabAchievement.IsProgressiveCount)
                            {
                                // if needs to update progression value
                                if (prefabAchievement.ActivationValueProgressionInt > databaseAchievement.ActivationValueProgressionInt)
                                {
                                    sqlQuery =
                                    "UPDATE " + achievementTableName + " SET activevalue_progress_int = " + prefabAchievement.ActivationValueProgressionInt
                                    + " WHERE aid = " + prefabAchievement.achievementId;
                                }
                            }
                            // if DB doesnt have an activation value, use prefab value to add it in
                            if (databaseAchievement.ActivationValueInt != prefabAchievement.ActivationValueInt)
                            {
                                sqlQuery = "UPDATE " + achievementTableName + " SET activevalue_int = " + prefabAchievement.ActivationValueInt
                                    + " WHERE aid = " + prefabAchievement.achievementId;
                            }

                        }
                        // if no entry in db. create entry with activate value, progress, and islocked = 1
                        if (!entryExists)
                        {
                            // 1 - locked, 0 - unlocked
                            int unlockAchievement = 1;
                            // if entry is NOT in list of stats
                            sqlQuery =
                            "Insert INTO "
                            + achievementTableName + " ( activevalue_int, activevalue_progress_int, islocked) "
                            + " Values('" + prefabAchievement.ActivationValueInt + "', '" 
                            + prefabAchievement.ActivationValueProgressionInt + "', '" 
                            + unlockAchievement + "')";
                        }
                        // check if sql query is empty/null. time saving method (skip query if not necessary to run)
                        if (!String.IsNullOrEmpty(sqlQuery))
                        {
                            //Debug.Log(sqlQuery);
                            cmd.CommandText = sqlQuery;
                            cmd.ExecuteNonQuery();
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

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE id = " + userid;

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

    public List<StatsTableHighScoreRow> getListOfHighScoreRowsFromTableByModeIdAndField(string field, int modeid)
    {
        List<StatsTableHighScoreRow> listOfValues = new List<StatsTableHighScoreRow>();

        string score; // store as string, more effcient that wrting 3 versions of the function
        string character;
        string level;
        string date;

        string sqlQuery = "";

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // game modes that require float values/ low time as high score
        if (modeid > 4 && modeid < 14 && modeid !=6)
        {
            sqlQuery = "SELECT  " + field + ", character, level, date FROM HighScores  WHERE modeid = " + modeid + " ORDER BY " + field + " ASC LIMIT 10";
        }
        else
        {
            sqlQuery = "SELECT  " + field + ", character, level, date FROM HighScores  WHERE modeid = " + modeid + " ORDER BY " + field + " DESC LIMIT 10";
        }

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        int i = 0;
        while (reader.Read())
        {
            StatsTableHighScoreRow temp = null;

            // game modes that require float values
            if (modeid > 4 && modeid < 14)
            {
                score = reader.GetFloat(0).ToString();
            }
            else
            {
                score = reader.GetInt32(0).ToString();
            }
            character = reader.GetString(1);
            level = reader.GetString(2);
            date = reader.GetString(3);
            // add to list
            listOfValues.Add(new StatsTableHighScoreRow(score, character, level, date));
            //Debug.Log("score : " + score + " character : " + character + " level : " + level + " date : " + date);
        }


        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return listOfValues;

    }

    //============================== get all time stats ===================================================
    public float getFloatValueAllTimeFromTableByField(String tableName, String field)
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        float value = 0;

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
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }
    public int getIntValueAllTimeFromTableByField(String tableName, String field)
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
            //Debug.Log(" value : " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    public int getIntSumByTableByField(String tableName, String field)
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // sum all values in column
        string sqlQuery = "SELECT SUM(" + field + ") FROM " + tableName;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        while (reader.Read())
        {
            value = reader.GetInt32(0);
            Debug.Log(" value : " + value);
        }
        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }
    //====================================================================================================
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

    // return int from specified table by field and achievement id
    public int getIntValueFromTableByFieldAndAchievementID(String tableName, String field, int aid)
    {
        int value = 0;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE aid = " + aid;

        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();

        // null check
        if (reader.IsDBNull(0))
        {
            value = 0;
        }
        else
        {
            while (reader.Read())
            {
                value = reader.GetInt32(0);
            }
        }

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return value;
    }

    // return int from specified table by field and achievement id
    public void updateIntValueFromTableByFieldAndAchievementID(String tableName, String field, int value, int aid)
    {
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        //string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE aid = " + aid;

        string sqlQuery = "UPDATE " + achievementTableName + " SET " + field + "  = " + value + " WHERE aid = " + aid;

        Debug.Log(sqlQuery);

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
}
