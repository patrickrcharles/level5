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
using UnityEngine.UI;

public class DBHelper : MonoBehaviour
{
    private String connection;
    private String databaseNamePath = "/level5.db";
    private String filepath;
    private const String allTimeStatsTableName = "AllTimeStats";
    private const String characterProfileTableName = "CharacterProfile";
    private const String cheerleaderProfileTableName = "CheerleaderProfile";
    private const String highScoresTableName = "HighScores";
    private const String userTableName = "User";

    private int currentDatabaseAppVersion = 4;
    bool databaseSuccessfullyUpgraded = true;

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    bool databaseLocked = false;

    Text message;

    public static DBHelper instance;
    public bool DatabaseLocked { get => databaseLocked; set => databaseLocked = value; }


    public int CurrentDatabaseAppVersion => currentDatabaseAppVersion;

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

    private void Start()
    {
        if (GameObject.Find("messageDisplay") != null)
        {
            message = GameObject.Find("messageDisplay").GetComponent<Text>();
        }
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    public string RemoveWhitespace(string str)
    {
        return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
    }

    string generateUniqueScoreID()
    {
        string macAddress = "";
        string uniqueScoreId = "";
        string uniqueModeDateIdentifier = "";
        //TimeZone localZone = TimeZone.CurrentTimeZone;

        uniqueModeDateIdentifier = DateTime.Now.Day.ToString()
            + DateTime.Now.Month.ToString()
            + DateTime.Now.Year.ToString()
            + DateTime.Now.Second;
        Debug.Log("----- uniqueScoreId : " + uniqueScoreId);
        //uniqueModeDateIdentifier
        //    = RemoveWhitespace(date)
        //    + RemoveWhitespace(localZone.StandardName)
        //    + RemoveWhitespace(GameOptions.levelDisplayName)
        //    + RemoveWhitespace(GameOptions.playerDisplayName)
        //    + RemoveWhitespace(GameOptions.gameModeSelectedName);

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
    public void InsertGameScore(DBHighScoreModel stats)
    {

        databaseLocked = true;
        try
        {
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery1 =
               "INSERT INTO HighScores( scoreidUnique, modeid, characterid, character, levelid, level, os, version ,date, time, " +
               " totalPoints, longestShot, totalDistance, maxShotMade, maxShotAtt, consecutiveShots, trafficEnabled, " +
               "hardcoreEnabled, enemiesEnabled, enemiesKilled, platform, device, ipaddress, twoMade, twoAtt, threeMade, threeAtt, " +
               "fourMade, fourAtt, sevenMade, sevenAtt, bonusPoints, moneyBallMade, moneyBallAtt, userName)  " +
               "Values( '" + stats.Scoreid
               + "', '" + stats.Modeid
               + "', '" + stats.Characterid
               + "', '" + stats.Character
               + "','" + stats.Levelid
               + "','" + stats.Level
               + "','" + stats.Os
               + "','" + stats.Version
               + "','" + stats.Date
               + "','" + stats.Time
               + "','" + stats.TotalPoints
               + "','" + stats.LongestShot
               + "','" + stats.TotalDistance + "','"
               + stats.MaxShotMade + "','"
               + stats.MaxShotAtt + "','"
               + stats.ConsecutiveShots + "','"
               + stats.TrafficEnabled + "','"
               + stats.HardcoreEnabled + "','"
               + stats.EnemiesEnabled + "','"
               + stats.EnemiesKilled + "','"
               + stats.Platform + "','"
               + stats.Device + "','"
               + stats.Ipaddress + "','"
               + stats.TwoMade + "','"
               + stats.TwoAtt + "','"
               + stats.ThreeMade + "','"
               + stats.ThreeAtt + "','"
               + stats.FourMade + "','"
               + stats.FourAtt + "','"
               + stats.SevenMade + "','"
               + stats.SevenAtt + "','"
               + stats.BonusPoints + "','"
               + stats.MoneyBallMade + "','"
               + stats.MoneyBallAtt + "','"
               + stats.UserName + "')";

            dbcmd.CommandText = sqlQuery1;
            IDataReader reader = dbcmd.ExecuteReader();
            reader.Close();

            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            databaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    // add experience gained to database
    internal void UpdatePlayerProfileProgression(float expGained)
    {
        //Debug.Log("UpdatePlayerProfileProgression()");
        //Debug.Log("exp gained : " + expGained);
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
               + " WHERE charid = " + GameOptions.characterId;

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
            Debug.Log("ERROR : " + e);
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
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
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
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
            return null;
        }
    }


    // insert current game's stats and score
    public void InsertUser(DBUserModel user)
    {
        StartCoroutine(InsertUserCoroutine(user));
    }


    private IEnumerator InsertUserCoroutine(DBUserModel user)
    {
        yield return new WaitUntil(() => !databaseLocked);
        databaseLocked = true;
        try
        {
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery1 =
               "INSERT INTO User(userid, username,firstname, lastname, email, ipaddress, signupdate, lastlogin, password)  " +
               "Values( '" + user.Userid
               + "', '" + user.UserName
               + "', '" + user.FirstName
               + "', '" + user.LastName
               + "','" + user.Email
               + "','" + user.IpAddress
               + "','" + user.SignUpDate
               + "','" + user.LastLogin
               + "','" + user.Password + "')";

            Debug.Log(sqlQuery1);

            dbcmd.CommandText = sqlQuery1;
            IDataReader reader = dbcmd.ExecuteReader();
            reader.Close();

            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            databaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            Debug.Log(e);
        }
    }

    // get user Data from Database
    public List<DBUserModel> getUserProfileStats()
    {
        List<DBUserModel> userModel = new List<DBUserModel>();
        try
        {
            DatabaseLocked = true;

            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (!isTableEmpty(userTableName))
            {
                sqlQuery = "Select userid, username, firstname, lastname, email, ipaddress, signupdate, lastlogin, password,"
                    + "bearerToken"
                    + " From " + userTableName;

                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    DBUserModel temp = new DBUserModel();

                    temp.Userid = reader.GetInt32(0);
                    temp.UserName = reader.GetString(1);
                    temp.FirstName = reader.GetString(2);
                    temp.LastName = reader.GetString(3);
                    temp.Email = reader.GetString(4);
                    temp.IpAddress = reader.GetString(5);
                    temp.SignUpDate = reader.GetString(6);
                    temp.LastLogin = reader.GetString(7);
                    temp.Password = reader.GetString(8);
                    temp.BearerToken = reader.GetString(9);

                    userModel.Add(temp);
                }
                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            databaseLocked = false;
            return userModel;
        }
        catch (Exception e)
        {
            databaseLocked = false;
            Debug.Log("ERROR : " + e);
            return new List<DBUserModel>();
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
            return value;
        }
    }

    public List<StatsTableHighScoreRow> getListOfHighScoreRowsFromTableByModeIdAndField(string field,
        int modeid,
        bool hardcoreValue,
        bool trafficValue,
        bool enemiesValue,
        int pageNumber)
    {
        List<StatsTableHighScoreRow> listOfValues = new List<StatsTableHighScoreRow>();

        string score; // store as string, more effcient that wrting 3 versions of the function
        string character;
        string level;
        string date;
        string hardcore = "";
        float time;
        string username;
        int hardcoreEnabled = 0;
        int trafficEnabled = 0;
        int enemiesEnabled = 0;
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


            if (modeid > 4 && modeid < 14 && modeid != 6 && modeid != 99)
            {
                sqlQuery = "SELECT  " + field + ", character, level, date, time, hardcoreEnabled, " +
                    "trafficEnabled, enemiesEnabled, userName FROM HighScores  WHERE modeid = " + modeid
                    + " AND hardcoreEnabled = " + Convert.ToInt32(hardcoreValue)
                    + " AND trafficEnabled = " + Convert.ToInt32(trafficValue)
                    + " AND enemiesEnabled = " + Convert.ToInt32(enemiesValue)
                    + " ORDER BY "
                    + field + " ASC,time ASC LIMIT 10 OFFSET " + pageNumberOffset;

            }
            else
            {
                sqlQuery = "SELECT  " + field + ", character, level, date, time, hardcoreEnabled," +
                    "trafficEnabled, enemiesEnabled, userName FROM HighScores  WHERE modeid = " + modeid
                    + " AND hardcoreEnabled = " + Convert.ToInt32(hardcoreValue)
                    + " AND trafficEnabled = " + Convert.ToInt32(trafficValue)
                    + " AND enemiesEnabled = " + Convert.ToInt32(enemiesValue)
                    + " ORDER BY "
                    + field + " DESC, time ASC LIMIT 10 OFFSET " + pageNumberOffset;
            }

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
                // null check
                if (reader.IsDBNull(6))
                {
                    trafficEnabled = 0;
                }
                else
                {
                    trafficEnabled = reader.GetInt32(6);
                }
                // null check
                if (reader.IsDBNull(7))
                {
                    enemiesEnabled = 0;
                }
                else
                {
                    enemiesEnabled = reader.GetInt32(7);
                }

                username = reader.GetString(8);

                StatsTableHighScoreRow row = gameObject.AddComponent<StatsTableHighScoreRow>();
                row.setRowValues(score, character, level, date, hardcore, username);

                // add to list
                //listOfValues.Add(new StatsTableHighScoreRow(score, character, level, date, hardcore, username));
                listOfValues.Add(row);
                Destroy(row);
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
                    StatsTableHighScoreRow row = gameObject.AddComponent<StatsTableHighScoreRow>();
                    row.setRowValues("", "", "", "", "", "");
                    listOfValues.Add(row);
                    Destroy(row);
                }
            }

            databaseLocked = false;
            return listOfValues;
        }
        catch (Exception e)
        {
            Debug.Log(" ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            Debug.Log("ERROR : " + e);
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
            //databaseLocked = false;
        }
        catch (Exception e)
        {
            databaseSuccessfullyUpgraded = false;
            Debug.Log("database upgrade to version " + currentDatabaseAppVersion + " failed");
            Debug.Log("ERROR : " + e);
            databaseLocked = false;
            return;
        }
    }

    public IEnumerator UpgradeDatabaseToVersion3()
    {
        //Debug.Log("UpgradeDatabaseToVersion3()");
        if (message != null)
        {
            message.text = "upgrading database...";
        }

        string table1 = "HighScores";
        string table2 = "User";

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
        string col13 = "submittedToApi";
        string col14 = "bonusPoints";
        string col15 = "moneyBallMade";
        string col16 = "moneyBallAtt";
        string col17 = "enemiesEnabled";
        string col18 = "userName";

        string col1a = "userid";
        string col2a = "username";
        string col3a = "firstname";
        string col4a = "lastname";
        string col5a = "email";
        string col6a = "password";
        string col7a = "ipaddress";
        string col8a = "signupdate";
        string col9a = "lastlogin";
        string col10a = "bearerToken";

        string typeText = "text";
        string typeInteger = "integer";

        // ------------------------- Upgrade HighScores table
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

        if (!doesColumnExist(table1, col13))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col13, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col13));
            databaseLocked = false;
        }

        if (!doesColumnExist(table1, col14))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col14, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col14));
            databaseLocked = false;
        }

        if (!doesColumnExist(table1, col15))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col15, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col15));
            databaseLocked = false;
        }

        if (!doesColumnExist(table1, col16))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col16, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col16));
            databaseLocked = false;
        }

        if (!doesColumnExist(table1, col17))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col17, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table1, col17));
            databaseLocked = false;
        }

        if (!doesColumnExist(table1, col18))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table1, col18, typeText);
            yield return new WaitUntil(() => doesColumnExist(table1, col18));
            databaseLocked = false;
        }

        // ------------------------- Upgrade Users table

        // drop user table
        StartCoroutine(DBConnector.instance.dropDatabaseTable("User"));
        StartCoroutine(DBConnector.instance.createTableUser());

        yield return new WaitUntil(() => DBConnector.instance.tableExists(userTableName));

        // add scoreidunique column
        if (!doesColumnExist(table2, col1a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col1a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col1a));
            databaseLocked = false;
        }

        // add platform column
        if (!doesColumnExist(table2, col2a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col2a, typeText);
            yield return new WaitUntil(() => doesColumnExist(table2, col2a));
            databaseLocked = false;
        }

        // add device column
        if (!doesColumnExist(table2, col3a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col3a, typeText);
            yield return new WaitUntil(() => doesColumnExist(table2, col3));
            databaseLocked = false;
        }

        // add ipaddress column
        if (!doesColumnExist(table2, col4a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col4a, typeText);
            yield return new WaitUntil(() => doesColumnExist(table2, col4a));
            databaseLocked = false;
        }

        if (!doesColumnExist(table2, col5a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col5a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col5a));
            databaseLocked = false;
        }

        if (!doesColumnExist(table2, col6a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col6a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col6a));
            databaseLocked = false;
        }

        if (!doesColumnExist(table2, col7a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col7a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col7a));
            databaseLocked = false;
        }

        if (!doesColumnExist(table2, col8a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col8a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col8a));
            databaseLocked = false;
        }

        if (!doesColumnExist(table2, col9a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col9a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col9a));
            databaseLocked = false;
        }

        if (!doesColumnExist(table2, col10a))
        {
            yield return new WaitUntil(() => !databaseLocked);
            databaseLocked = true;
            alterTableAddColumn(table2, col10a, typeInteger);
            yield return new WaitUntil(() => doesColumnExist(table2, col10a));
            databaseLocked = false;
        }

        Debug.Log("databaseSuccessfullyUpgraded : " + databaseSuccessfullyUpgraded);
        if (databaseSuccessfullyUpgraded)
        {
            StartCoroutine(setDatabaseVersion());
        }
        else
        {
            Debug.Log("database upgrade to version " + currentDatabaseAppVersion + " failed");
        }
        if (message != null)
        {
            message.text = "";
        }
    }

    public IEnumerator setDatabaseVersion()
    {
        //Debug.Log("setDatabaseVersion");
        yield return new WaitUntil(() => !DatabaseLocked);
        try
        {
            //Debug.Log("try...");
            DatabaseLocked = true;
            dbconn = new SqliteConnection(connection);
            dbconn.Open();
            dbcmd = dbconn.CreateCommand();

            string sqlQuery = "PRAGMA main.user_version = '" + currentDatabaseAppVersion + "'";

            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteScalar();

            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            DatabaseLocked = false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }

    public DBHighScoreModel getHighScoreFromDatabase(string scoreid)
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
                sqlQuery = "Select  * From " + highScoresTableName + " WHERE scoreid =" + scoreid;
                Debug.Log(sqlQuery);
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    //highscore.Id = reader.GetInt32(0);

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
                    highscore.BonusPoints = reader.GetInt32(31);
                    //highscore.submittedToApi = reader.GetInt32(32);
                    highscore.MoneyBallMade = reader.GetInt32(33);
                    highscore.MoneyBallAtt = reader.GetInt32(34);
                    highscore.EnemiesEnabled = reader.GetInt32(35);

                    Debug.Log("------------------------------------------ db action finished");
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log("EXCEPTION : " + e);
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
                    //Debug.Log("column : " + columnName + " found");
                    return true;
                }
            }
            //databaseLocked = false;
        }
        catch
        {
            databaseLocked = false;
            return false;
        }
        //Debug.Log("---------- column : " + columnName + " return FALSE");
        return false;
    }

    public string GetExternalIpAdress()
    {
        string pubIp = new WebClient().DownloadString("https://api.ipify.org");
        return pubIp;
    }

    // insert current game's stats and score
    public void setGameScoreSubmitted(string scoreid, bool value)
    {
        databaseLocked = true;
        int submittedValue = 0;
        if (value)
        {
            submittedValue = 1;
        }
        try
        {
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            // if entry is NOT in list of stats
            string sqlQuery = "UPDATE " + highScoresTableName + " SET submittedToApi" + " = " + submittedValue
                + " WHERE scoreidUnique = " + "'" + scoreid + "'";

            //Debug.Log(sqlQuery);

            dbcmd.CommandText = sqlQuery;
            IDataReader reader = dbcmd.ExecuteReader();
            reader.Close();

            reader = null;
            dbcmd.Dispose();
            dbcmd = null;
            dbconn.Close();
            dbconn = null;

            //databaseLocked = false;
            //Debug.Log("score submitted to api");
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }


    //public void alterTableRemoveColumn(string tableName, string columnName, string type)
    //{
    //    string sqlQueryCheckForColumn = "SELECT* FROM sqlite_master WHERE type = '" + tableName + "' AND name = '" + columnName + "' AND sql LIKE '%skiptime%'";

    //    string sqlQuery = "ALTER TABLE " + tableName + " DROP COLUMN " + columnName + " " + type + " NOT NULL DEFAULT 0;";

    //    Debug.Log(sqlQueryCheckForColumn);
    //    Debug.Log(sqlQuery);
    //}
}
