using Assets.Scripts.database;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;

public class DBHelper : MonoBehaviour
{
    private String connection;
    private String databaseNamePath = "/level5.db";
    private String filepath;
    private const String allTimeStatsTableName = "AllTimeStats";
    private const String characterProfileTableName = "CharacterProfile";
    private const String cheerleaderProfileTableName = "CheerleaderProfile";
    private const String highScoresTableName = "HighScores";

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    bool databaseLocked = false;

    public static DBHelper instance;

    public IDbConnection Dbconn { get => dbconn; set => dbconn = value; }
    public bool DatabaseLocked { get => databaseLocked; set => databaseLocked = value; }

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        filepath = Application.persistentDataPath + databaseNamePath;
        connection = "Data source=" + filepath; //Path to database
    }

    // check if specified table is emoty
    public bool isTableEmpty(String tableName)
    {
        try
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
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return false;
        }
    }

    internal void InsertDefaultUserRecord()
    {
        try
        {
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
               "os)  " +

               "Values( '" + 1
               + "', '" + "placeholder"
               + "','" + "placeholder"
               + "','" + "placeholder"
               + "','" + "placeholder"
               + "','" + "email@placeholder.com"
               + "','" + "password"
               + "','" + "6.9.420"
               + "','" + "os version')";

            dbcmd.CommandText = sqlQuery1;
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
            DatabaseLocked = false;
            return;
        }
    }

    public string RemoveWhitespace(string str)
    {
        return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    string generateUnqueScoreID()
    {
        string macAddress = "";
        string uniqueScoreId = "";
        string uniqueModeDateIdentifier = "";
        TimeZone localZone = TimeZone.CurrentTimeZone;

        uniqueModeDateIdentifier
            = RemoveWhitespace(DateTime.Now.ToString())
            + RemoveWhitespace(localZone.StandardName)
            + RemoveWhitespace(GameOptions.levelDisplayName)
            + RemoveWhitespace(GameOptions.playerDisplayName)
            + RemoveWhitespace(GameOptions.gameModeSelectedName);

        foreach (NetworkInterface ninf in NetworkInterface.GetAllNetworkInterfaces())
        {
            if (ninf.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
            if (ninf.OperationalStatus == OperationalStatus.Up)
            {
                macAddress += ninf.GetPhysicalAddress().ToString();
                break;
            }
        }
        uniqueScoreId = macAddress + uniqueModeDateIdentifier + SystemInfo.deviceUniqueIdentifier;
        return uniqueScoreId;
    }

    // insert current game's stats and score
    internal void InsertGameScore(BasketBallStats stats)
    {
        try
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
            int hardcoreEnabled = 0;
            if (GameOptions.hardcoreModeEnabled)
            {
                hardcoreEnabled = 1;
            }

            string sqlQuery1 =
               "INSERT INTO HighScores( scoreidUnique, modeid, characterid, character, levelid, level, os, version ,date, time, " +
               " totalPoints, longestShot, totalDistance, maxShotMade, maxShotAtt, consecutiveShots, trafficEnabled, " +
               "hardcoreEnabled, enemiesKilled, platform, device, ipaddress, twoMade, twoAtt, threeMade, threeAtt, " +
               "fourMade, fourAtt, sevenMade, sevenAtt )  " +
               "Values( '" + generateUnqueScoreID()
               + "', '" + GameOptions.gameModeSelectedId
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
               + trafficEnabled + "','"
               + hardcoreEnabled + "','"
               + stats.EnemiesKilled + "','"
               + SystemInfo.deviceType + "','"
               + SystemInfo.deviceModel + "','"
               + GetExternalIpAdress() + "','"
               + stats.TwoPointerMade + "','"
               + stats.TwoPointerAttempts + "','"
               + stats.ThreePointerMade + "','"
               + stats.ThreePointerAttempts + "','"
               + stats.FourPointerMade + "','"
               + stats.FourPointerAttempts + "','"
               + stats.SevenPointerMade + "','"
               + stats.SevenPointerAttempts + "')";

            dbcmd.CommandText = sqlQuery1;
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
            DatabaseLocked = false;
            Debug.Log(e);
            return;
        }
    }

    // add default cheerleader data from PREFABS to DATABASE
    public void InsertCheerleaderProfile(List<CheerleaderProfile> cheerleaderSelectedData)
    {
        try
        {
            databaseLocked = true;
            var dbconn = new SqliteConnection(connection);
            using (dbconn)
            {
                dbconn.Open(); //Open connection to the database.
                using (SqliteTransaction tr = dbconn.BeginTransaction())
                {
                    using (SqliteCommand cmd = dbconn.CreateCommand())
                    {
                        cmd.Transaction = tr;
                        foreach (CheerleaderProfile ch in cheerleaderSelectedData)
                        {
                            string sqlQuery =
                            "Insert INTO "
                            + cheerleaderProfileTableName + " ( cid, name, objectName, unlockText, isLocked) "
                            + " Values('" + ch.CheerleaderId
                            + "', '" + ch.CheerleaderDisplayName
                            + "', '" + ch.CheerleaderObjectName
                            + "', '" + ch.UnlockCharacterText
                            + "', '" + Convert.ToInt32(ch.IsLocked)
                            + "')";

                            cmd.CommandText = sqlQuery;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                dbconn.Close();
            }
            databaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return;
        }
    }

    // add experience gained to database
    internal void UpdatePlayerProfileProgression(float expGained)
    {
        try
        {
            int prevLevel = PlayerData.instance.CurrentExperience / 3000;
            int currentLevel = ((int)((PlayerData.instance.CurrentExperience + expGained) / 3000));

            // gained a level
            if (currentLevel > prevLevel)
            {
                PlayerData.instance.UpdatePointsAvailable++;
            }

            int updatePointsAvailable = PlayerData.instance.UpdatePointsAvailable;
            int updatePointsUsed = PlayerData.instance.UpdatePointsUsed;

            // course correction if points available/used dont line up
            if (!((updatePointsAvailable + updatePointsUsed) == currentLevel))
            {
                updatePointsAvailable = currentLevel - updatePointsUsed;
            }

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery1 =
               "UPDATE " + characterProfileTableName
               + " SET experience = " + (PlayerData.instance.CurrentExperience + expGained)
               + ", level = " + currentLevel
               + ", pointsAvailable = " + updatePointsAvailable
               + " WHERE charid = " + GameOptions.playerId;

            //Debug.Log(sqlQuery1);

            dbcmd.CommandText = sqlQuery1;
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
            DatabaseLocked = false;
            return;
        }
    }

    // insert default Player profiles
    public void InsertCharacterProfile(List<CharacterProfile> shooterProfileList)
    {
        try
        {
            databaseLocked = true;
            var dbconn = new SqliteConnection(connection);
            using (dbconn)
            {
                dbconn.Open(); //Open connection to the database.
                using (SqliteTransaction tr = dbconn.BeginTransaction())
                {
                    using (SqliteCommand cmd = dbconn.CreateCommand())
                    {
                        cmd.Transaction = tr;
                        foreach (CharacterProfile shooter in shooterProfileList)
                        {
                            string sqlQuery =
                            "Insert INTO "
                            + characterProfileTableName + " ( charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, " +
                            "speed, runSpeed, runSpeedHasBall, luck, shootAngle, experience, level, pointsAvailable, pointsUsed, range, release, isLocked) "
                            + " Values('" + shooter.PlayerId
                            + "', '" + shooter.PlayerDisplayName
                            + "', '" + shooter.PlayerObjectName
                            + "', '" + shooter.Accuracy2Pt
                            + "', '" + shooter.Accuracy3Pt
                            + "', '" + shooter.Accuracy4Pt
                            + "', '" + shooter.Accuracy7Pt
                            + "', '" + shooter.JumpForce
                            + "', '" + shooter.Speed
                            + "', '" + shooter.RunSpeed
                            + "', '" + shooter.RunSpeedHasBall
                            + "', '" + shooter.Luck
                            + "', '" + shooter.ShootAngle
                            + "', '" + shooter.Experience
                            + "', '" + shooter.Level
                            + "', '" + shooter.PointsAvailable
                            + "', '" + shooter.PointsUsed
                            + "', '" + shooter.Range
                            + "', '" + shooter.Release
                            + "', '" + Convert.ToInt32(shooter.IsLocked)
                            + "')";

                            cmd.CommandText = sqlQuery;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tr.Commit();
                }
                dbconn.Close();
            }
            databaseLocked = false;
        }
        catch(Exception e)
        {
            DatabaseLocked = false;
            return;
        }
    }

    // insert a specific character to database. Example, new character added to game, 
    // this will update Database with new character info
    public void InsertCharacterProfile(CharacterProfile character)
    {
        try
        {
            databaseLocked = true;
            var dbconn = new SqliteConnection(connection);
            using (dbconn)
            {
                dbconn.Open(); //Open connection to the database.
                using (SqliteTransaction tr = dbconn.BeginTransaction())
                {
                    using (SqliteCommand cmd = dbconn.CreateCommand())
                    {
                        cmd.Transaction = tr;

                        string sqlQuery =
                        "Insert INTO "
                        + characterProfileTableName + " ( charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, " +
                        "speed, runSpeed, runSpeedHasBall, luck, shootAngle, experience, level, pointsAvailable, pointsUsed, range, release, islocked) "
                        + " Values('" + character.PlayerId
                        + "', '" + character.PlayerDisplayName
                        + "', '" + character.PlayerObjectName
                        + "', '" + character.Accuracy2Pt
                        + "', '" + character.Accuracy3Pt
                        + "', '" + character.Accuracy4Pt
                        + "', '" + character.Accuracy7Pt
                        + "', '" + character.JumpForce
                        + "', '" + character.Speed
                        + "', '" + character.RunSpeed
                        + "', '" + character.RunSpeedHasBall
                        + "', '" + character.Luck
                        + "', '" + character.ShootAngle
                        + "', '" + character.Experience
                        + "', '" + character.Level
                        + "', '" + character.PointsAvailable
                        + "', '" + character.PointsUsed
                        + "', '" + character.Range
                        + "', '" + character.Release
                        + "', '" + Convert.ToInt32(character.IsLocked)
                        + "')";

                        cmd.CommandText = sqlQuery;
                        cmd.ExecuteNonQuery();

                    }
                    tr.Commit();
                }
                dbconn.Close();
            }
            databaseLocked = false;
        }
        catch(Exception e)
        {
            DatabaseLocked = false;
            return;
        }
    }

    // update a character profile.
    // used in Progression scene on Save progress
    public void UpdateCharacterProfile(CharacterProfile character)
    {
        try
        {
            databaseLocked = true;
            var dbconn = new SqliteConnection(connection);
            using (dbconn)
            {
                dbconn.Open(); //Open connection to the database.
                using (SqliteTransaction tr = dbconn.BeginTransaction())
                {
                    using (SqliteCommand cmd = dbconn.CreateCommand())
                    {
                        cmd.Transaction = tr;

                        string sqlQuery =
                        "Update " + characterProfileTableName
                        + " SET accuracy2 = " + character.Accuracy2Pt
                        + ", accuracy3 = " + character.Accuracy3Pt
                        + ", accuracy4 = " + character.Accuracy4Pt
                        + ", accuracy7 = " + character.Accuracy7Pt
                        + ", range = " + character.Range
                        + ", release = " + character.Release
                        + ", luck = " + character.Luck
                        + ", pointsAvailable = " + character.PointsAvailable
                        + ", pointsUsed = " + character.PointsUsed
                        + " WHERE charid = " + character.PlayerId;

                        cmd.CommandText = sqlQuery;
                        cmd.ExecuteNonQuery();

                    }
                    tr.Commit();
                }
                dbconn.Close();
            }
            databaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return;
        }
    }
    // insert a specific cheerleader to database. Example, new cheerleader added to game, 
    // this will update Database with new cheerleader info
    public void InsertCheerleaderProfile(CheerleaderProfile cheerleader)
    {
        try
        {
            databaseLocked = true;
            var dbconn = new SqliteConnection(connection);
            using (dbconn)
            {
                dbconn.Open(); //Open connection to the database.
                using (SqliteTransaction tr = dbconn.BeginTransaction())
                {
                    using (SqliteCommand cmd = dbconn.CreateCommand())
                    {
                        cmd.Transaction = tr;

                        string sqlQuery =
                        "Insert INTO "
                        + cheerleaderProfileTableName + " ( cid, name, objectName, unlockText, isLocked ) "
                        + " Values('" + cheerleader.CheerleaderId
                        + "', '" + cheerleader.CheerleaderDisplayName
                        + "', '" + cheerleader.CheerleaderObjectName
                        + "', '" + cheerleader.UnlockCharacterText
                        + "', '" + Convert.ToInt32(cheerleader.IsLocked)
                        + "')";

                        cmd.CommandText = sqlQuery;
                        cmd.ExecuteNonQuery();

                    }
                    tr.Commit();
                }
                dbconn.Close();
            }
            databaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return;
        }
    }

    // get All time stats. Used to update all time stats after a game session
    internal BasketBallStats getAllTimeStats()
    {
        try
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
                    prevStats.LongestShotMade = reader.GetFloat(12);
                    prevStats.TimePlayed = reader.GetFloat(13);
                    if (reader.IsDBNull(14))
                    {
                        prevStats.EnemiesKilled = 0;
                    }
                    else
                    {
                        prevStats.EnemiesKilled = reader.GetInt32(14);
                    }
                }
            }
            Destroy(prevStats, 5);
            return prevStats;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return null;
        }
    }


    // get Character Data from Database
    public List<CharacterProfile> getCharacterProfileStats()
    {
        List<CharacterProfile> characterStats = new List<CharacterProfile>();
        try
        {
            DatabaseLocked = true;

            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (!isTableEmpty(characterProfileTableName))
            {
                sqlQuery = "Select charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, speed,"
                    + "runSpeed, runSpeedHasBall, luck, shootAngle, experience, level, pointsAvailable, pointsUsed, range, release, isLocked"
                    + " From " + characterProfileTableName;

                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                //CharacterProfile temp = new CharacterProfile();
                while (reader.Read())
                {
                    //CharacterProfile temp = null;
                    //CharacterProfile temp = new CharacterProfile();
                    CharacterProfile temp = gameObject.AddComponent<CharacterProfile>();

                    //CharacterProfile temp = gameObject.AddComponent<CharacterProfile>();

                    temp.PlayerId = reader.GetInt32(0);
                    temp.PlayerDisplayName = reader.GetString(1);
                    temp.PlayerObjectName = reader.GetString(2);
                    temp.Accuracy2Pt = reader.GetInt32(3);
                    temp.Accuracy3Pt = reader.GetInt32(4);
                    temp.Accuracy4Pt = reader.GetInt32(5);
                    temp.Accuracy7Pt = reader.GetInt32(6);
                    temp.JumpForce = reader.GetFloat(7);
                    temp.Speed = reader.GetFloat(8);
                    temp.RunSpeed = reader.GetFloat(9);
                    temp.RunSpeedHasBall = reader.GetFloat(10);
                    temp.Luck = reader.GetInt32(11);
                    temp.ShootAngle = reader.GetInt32(12);
                    temp.Experience = reader.GetInt32(13);
                    temp.Level = reader.GetInt32(14);
                    temp.PointsAvailable = reader.GetInt32(15);
                    temp.PointsUsed = reader.GetInt32(16);
                    temp.Range = reader.GetInt32(17);
                    temp.Release = reader.GetInt32(18);
                    temp.IsLocked = Convert.ToBoolean(reader.GetValue(19));
                    characterStats.Add(temp);

                    Destroy(temp);
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            databaseLocked = false;
            return characterStats;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return new List<CharacterProfile>();
        }
    }

    // get cheerleader data from Database
    public List<CheerleaderProfile> getCheerleaderProfileStats()
    {
        try
        {
            DatabaseLocked = true;
            List<CheerleaderProfile> cheerleaderStats = new List<CheerleaderProfile>();

            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (!isTableEmpty(cheerleaderProfileTableName))
            {
                sqlQuery = "Select cid, name, objectName, unlockText, isLocked "
                    + " From " + cheerleaderProfileTableName;

                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    //CheerleaderProfile temp = new CheerleaderProfile();
                    CheerleaderProfile temp = gameObject.AddComponent<CheerleaderProfile>();

                    temp.CheerleaderId = reader.GetInt32(0);
                    temp.CheerleaderDisplayName = reader.GetString(1);
                    temp.CheerleaderObjectName = reader.GetString(2);
                    temp.UnlockCharacterText = reader.GetString(3);
                    temp.IsLocked = Convert.ToBoolean(reader.GetInt32(4));

                    cheerleaderStats.Add(temp);

                    Destroy(temp);
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            databaseLocked = false;
            return cheerleaderStats;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return null;
        }
    }
    // update all time stats
    internal void UpdateAllTimeStats(BasketBallStats stats)
    {
        try
        {
            databaseLocked = true;
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
               "sevenAtt, totalPoints, moneyBallMade, moneyBallAtt, totalDistance, timePlayed, longestShot, enemiesKilled)  " +
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
               stats.LongestShotMade + "','" +
               stats.EnemiesKilled + "')";

                //Debug.Log(sqlQuery);
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
               ", enemiesKilled = " + (prevStats.EnemiesKilled += stats.EnemiesKilled) +
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

            DatabaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return;
        }
    }

    // return int from specified table by field and userid
    public int getIntValueFromTableByFieldAndCharId(String tableName, String field, int charid)
    {
        int value = 0;
        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = "SELECT " + field + " FROM " + tableName + " WHERE charid = " + charid;

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

            DatabaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            return value;
        }
    }

    // ***************************** get values by MODE ID *******************************************
    // return string from specified table by field and userid
    public int getIntValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order, int hardcore)
    {

        int value = 0;
        databaseLocked = true;

        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(connection);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        // get all all values sort DESC, return top 1
        string sqlQuery = "SELECT " + field + " FROM " + tableName
            + " WHERE modeid = " + modeid + " AND hardcoreEnabled = " + hardcore + " ORDER BY " + field + "  " + order + "  LIMIT 1";

        //Debug.Log(sqlQuery);
        try
        {
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                // null check
                if (reader.IsDBNull(0))
                {
                    value = 0;
                }
                else
                {
                    value = reader.GetInt32(0);
                }
                //Debug.Log("value : "+ value);
            }
            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;
            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }

    public List<StatsTableHighScoreRow> getListOfHighScoreRowsFromTableByModeIdAndField(string field, int modeid, bool hardcoreValue, int pageNumber)
    {
        List<StatsTableHighScoreRow> listOfValues = new List<StatsTableHighScoreRow>();

        string score; // store as string, more effcient that wrting 3 versions of the function
        string character;
        string level;
        string date;
        string hardcore = "";
        float time;
        int hardcoreEnabled = 0;
        //int numberOfResultsPages = 0;
        //string numResultsQuery = "";

        int pageNumberOffset = pageNumber * 10;
        //int hardcoreEnabled = Convert.ToInt32(hardcoreValue);

        string sqlQuery = "";

        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            //numResultsQuery = "SELECT  * FROM HighScores  WHERE modeid = " + modeid
            //        + " AND hardcoreEnabled = 0 ORDER BY " + field + " ASC,time ASC LIMIT 10 OFFSET " + pageNumberOffset;
            //numberOfResultsPages = getNumberOfResults(numResultsQuery);

            // game modes that require float values/ low time as high score
            if (!hardcoreValue)
            {
                if (modeid > 4 && modeid < 14 && modeid != 6 && modeid != 99)
                {
                    sqlQuery = "SELECT  " + field + ", character, level, date, time,  hardcoreEnabled FROM HighScores  WHERE modeid = " + modeid
                        + " AND hardcoreEnabled = 0 ORDER BY " + field + " ASC,time ASC LIMIT 10 OFFSET " + pageNumberOffset;

                }
                else
                {
                    sqlQuery = "SELECT  " + field + ", character, level, date, time, hardcoreEnabled FROM HighScores  WHERE modeid = " + modeid
                        + " AND hardcoreEnabled = 0 ORDER BY " + field + " DESC, time ASC LIMIT 10 OFFSET " + pageNumberOffset;
                }
            }
            if (hardcoreValue)
            {
                if (modeid > 4 && modeid < 14 && modeid != 6 && modeid != 99)
                {
                    sqlQuery = "SELECT  " + field + ", character, level, date, hardcoreEnabled FROM HighScores  WHERE modeid = " + modeid
                        + " AND hardcoreEnabled = 1 ORDER BY " + field + " ASC, time DESC LIMIT 10 OFFSET " + pageNumberOffset;

                }
                else
                {
                    sqlQuery = "SELECT  " + field + ", character, level, date, hardcoreEnabled FROM HighScores  WHERE modeid = " + modeid
                        + " AND hardcoreEnabled = 1 ORDER BY " + field + " DESC, time DESC LIMIT 10 OFFSET " + pageNumberOffset;
                }
            }

            //Debug.Log(sqlQuery);

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                // game modes that require float values
                if ((modeid > 4 && modeid < 14) || modeid == 99)
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
                time = reader.GetFloat(4);
                // null check
                if (reader.IsDBNull(5))
                {
                    hardcoreEnabled = 0;
                }
                else
                {
                    hardcoreEnabled = reader.GetInt32(5);
                }
                if (hardcoreEnabled != 0)
                {
                    hardcore = "yes";
                }
                else
                {
                    hardcore = "no";
                }
                // add to list
                listOfValues.Add(new StatsTableHighScoreRow(score, character, level, date, hardcore));
                //Debug.Log("score : " + score + " character : " + character + " level : " + level + " date : " + date);
            }

            reader.Close();
            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            // if less than 10 values in list, add empty values
            if (listOfValues.Count < 10)
            {
                int numToAdd = 10 - listOfValues.Count;
                for (int i = 0; i < numToAdd; i++)
                {
                    listOfValues.Add(new StatsTableHighScoreRow("", "", "", "", ""));
                }
            }

            databaseLocked = false;
            return listOfValues;
        }
        catch(Exception e)
        {
            databaseLocked = false;
            return listOfValues;
        }
    }

    public int getNumberOfResults(string field, int modeid, bool hardcoreValue, int pageNumber)
    {
        int rowCount = 0;
        string sqlQuery = "";

        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (hardcoreValue)
            {
                sqlQuery = "SELECT Count(*) FROM HighScores  WHERE modeid = " + modeid
                        + " AND hardcoreEnabled = 1 ORDER BY " + field;
            }
            else
            {
                sqlQuery = "SELECT Count(*) FROM HighScores  WHERE modeid = " + modeid
                        + " AND hardcoreEnabled = 0 ORDER BY " + field;
            }

            //numberOfResultsPages = getNumberOfResults(numResultsQuery);
            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            while (reader.Read())
            {
                rowCount = reader.GetInt32(0);
                //Debug.Log("rowCount : " + rowCount);
            }

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            databaseLocked = false;

            return rowCount;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return rowCount;
        }
    }


    //============================== get all time stats ===================================================
    public float getFloatValueAllTimeFromTableByField(String tableName, String field)
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        float value = 0;

        try
        {
            databaseLocked = true;

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

            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }
    public int getIntValueAllTimeFromTableByField(String tableName, String field)
    {
        int value = 0;

        try
        {
            databaseLocked = true;

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

            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }

    public int getIntSumByTableByField(String tableName, String field)
    {

        int value = 0;

        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            // sum all values in column
            string sqlQuery = "SELECT SUM(" + field + ") FROM " + tableName;

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();

            // check if table is empty
            if (!isTableEmpty(tableName))
            {
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
            }

            databaseLocked = false;
            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }
    //====================================================================================================
    public float getFloatValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order, int hardcore)
    {

        float value = 0;

        try
        {
            databaseLocked = true; ;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            // get all all values sort DESC, return top 1
            string sqlQuery = "SELECT " + field + " FROM " + tableName
                + " WHERE modeid = " + modeid + " AND hardcoreEnabled = " + hardcore + " ORDER BY " + field + " " + order + " LIMIT 1";

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

            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }

    //====================================================================================================
    public int getMostConsecutiveShots()
    {
        //Debug.Log("getFloatValueHighScoreFromTableByFieldAndModeId");
        int value = 0;

        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            // get all all values sort DESC, return top 1
            string sqlQuery = "SELECT consecutiveShots from HighScores ORDER BY consecutiveShots DESC LIMIT 1";

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

            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }

    // return string from specified table by field and userid
    public float updateFloatValueByTableAndField(String tableName, String field, float value)
    {
        try
        {
            databaseLocked = true;

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

            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }

    public float deleteRecordFromTableByID(String tableName, String idName, int id)
    {
        float value = 0;

        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = "DELETE FROM " + tableName + " Where " + idName + " = " + id;

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

            databaseLocked = false;

            return value;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            return value;
        }
    }

    public void alterTableAddColumn(string tableName, string columnName, string type)
    {
        try
        {
            databaseLocked = true;
            if (!doesColumnExist(tableName, columnName))
            {
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(connection);
                dbconn.Open(); //Open connection to the database.
                IDbCommand dbcmd = dbconn.CreateCommand();

                string sqlQuery = "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + type + " NOT NULL DEFAULT 0;";
                dbcmd.CommandText = sqlQuery;

                IDataReader reader = dbcmd.ExecuteReader();

                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            databaseLocked = false;
        }
        catch
        {
            databaseLocked = false;
            return;
        }
    }

    public IEnumerator UpgradeDatabaseToVersion2()
    {
        string table1 = "HighScores";

        string col1 = "scoreidUnique";
        string col2 = "platform";
        string col3 = "device";
        string col4 = "ipaddress";
        string col5 = "twoMade";
        string col6 = "twoAtt";
        string col7 = "threeMade";
        string col8 = "threeAtt";
        string col9 = "fourMade";
        string col10 = "fourAtt";
        string col11 = "sevenMade";
        string col12 = "sevenAtt";

        string typeText = "text";
        string typeInteger = "integer";

        // add scoreidunique column
        if (!doesColumnExist(table1, col1))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col1, typeText);
            yield return new WaitUntil(() => doesColumnExist(table1, col1));
            databaseLocked = false;
        }
        // add platform column
        if (!doesColumnExist(table1, col2))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col2, typeText);
            yield return new WaitUntil(() => doesColumnExist(table1, col2));
            databaseLocked = false;
        }
        // add device column
        if (!doesColumnExist(table1, col3))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col3, typeText);
            yield return new WaitUntil(() => doesColumnExist(table1, col3));
            databaseLocked = false;
        }
        // add ipaddress column
        if (!doesColumnExist(table1, col4))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col4, typeText);
            yield return new WaitUntil(() => doesColumnExist(table1, col4));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col5))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col5, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col5));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col6))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col6, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col6));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col7))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col7, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col7));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col8))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col8, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col8));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col9))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col9, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col9));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col10))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col10, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col10));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col11))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col11, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col11));
            databaseLocked = false;
        }
        if (!doesColumnExist(table1, col12))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col12, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col12));
            databaseLocked = false;
        }

        /*
         * x add to HighScores scoreidUnique
         * x add to HighScores platform
         * x add to HighScores device
         * x add to HighScores ipaddress
         */
    }

    public DBHighScoreModel getHighScoreFromDatabase(int id)
    {
        DBHighScoreModel highscore = new DBHighScoreModel();
        databaseLocked = true;
        try
        {
            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            //Debug.Log("table empty : " + isTableEmpty(allTimeStatsTableName));

            if (!isTableEmpty(highScoresTableName))
            {
                //Debug.Log(" table is not empty");
                sqlQuery = "Select  * From " + highScoresTableName + " WHERE scoreid ="+ id;
                Debug.Log(sqlQuery);
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    highscore.Id = reader.GetInt32(0);

                    if (reader.IsDBNull(1)) { highscore.Userid = 0; }
                    else { highscore.Userid = reader.GetInt32(1); }

                    highscore.Modeid = reader.GetInt32(2);
                    highscore.Characterid = reader.GetInt32(3);
                    highscore.Character = reader.GetString(4);
                    highscore.Levelid = reader.GetInt32(5);
                    highscore.Level = reader.GetString(6);
                    highscore.Os = reader.GetString(7);
                    highscore.Version = reader.GetString(8);
                    highscore.Date = reader.GetString(9);
                    highscore.Time = reader.GetFloat(10);
                    highscore.TotalPoints = reader.GetInt32(11);
                    highscore.LongestShot = reader.GetFloat(12);
                    highscore.TotalDistance = reader.GetFloat(13);
                    highscore.MaxShotMade = reader.GetInt32(14);
                    highscore.MaxShotAtt = reader.GetInt32(15);
                    highscore.ConsecutiveShots = reader.GetInt32(16);
                    highscore.TrafficEnabled = reader.GetInt32(17);
                    highscore.HardcoreEnabled = reader.GetInt32(18);
                    highscore.EnemiesKilled = reader.GetInt32(19);
                    highscore.Scoreid = reader.GetString(20);
                    highscore.Platform = reader.GetString(21);
                    highscore.Device = reader.GetString(22);
                    highscore.Ipaddress = reader.GetString(23);
                    highscore.TwoMade = reader.GetInt32(24);
                    highscore.TwoAtt = reader.GetInt32(25);
                    highscore.ThreeMade = reader.GetInt32(26);
                    highscore.ThreeAtt = reader.GetInt32(27);
                    highscore.FourMade = reader.GetInt32(28);
                    highscore.FourAtt = reader.GetInt32(29);
                    highscore.SevenMade = reader.GetInt32(30);
                    highscore.SevenAtt = reader.GetInt32(31);

                    Debug.Log("------------------------------------------ db action finished");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("EXCEPTION : "+ e);
            DatabaseLocked = false;
            return null;
        }
        databaseLocked = false;
        //Destroy(highscore, 5);
        return highscore;
    }

    public bool doesColumnExist(string tableName, string columnName)
    {
        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQueryCheckForColumn = "PRAGMA table_info(" + tableName + ")";

            dbcmd.CommandText = sqlQueryCheckForColumn;
            IDataReader reader = dbcmd.ExecuteReader();

            int nameIndex = reader.GetOrdinal("Name");

            while (reader.Read())
            {
                if (reader.GetString(nameIndex).Equals(columnName))
                {
                    //Debug.Log("column found");
                    return true;
                }
            }

            databaseLocked = false;
        }
        catch
        {
            databaseLocked = false;
            return false;
        }
        return false;
    }

    public string GetExternalIpAdress()
    {
        string pubIp = new WebClient().DownloadString("https://api.ipify.org");
        return pubIp;
    }

    //public void alterTableRemoveColumn(string tableName, string columnName, string type)
    //{
    //    string sqlQueryCheckForColumn = "SELECT* FROM sqlite_master WHERE type = '" + tableName + "' AND name = '" + columnName + "' AND sql LIKE '%skiptime%'";

    //    string sqlQuery = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName + " " + type + " NOT NULL DEFAULT 0;";

    //    Debug.Log(sqlQueryCheckForColumn);
    //    Debug.Log(sqlQuery);
    //}
}
