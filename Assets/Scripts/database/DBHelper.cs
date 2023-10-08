using Assets.Scripts.database;
using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class DBHelper : MonoBehaviour
{
    private String connection;
    private String databaseNamePath = "/level5.db";
    private String filepath;

    //private int currentDatabaseAppVersion = 8;
    //bool databaseSuccessfullyUpgraded = true;

    IDbCommand dbcmd;
    IDataReader reader;
    private IDbConnection dbconn;

    [SerializeField]
    bool databaseLocked = false;

    Text message;

    public static DBHelper instance;

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
                DatabaseLocked = false;
                return true;
            }
            DatabaseLocked = false;
            return false;
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
            return false;
        }
    }

    // insert current game's stats and score
    public void InsertGameScore(HighScoreModel stats)
    {

        databaseLocked = true;
        try
        {
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            // todo : add p1-p4 toal points/1ST-4TH PLACE/winnIsCpu to query and insert, then into create database
            string sqlQuery1 =
               "INSERT INTO HighScores( scoreidUnique, modeid, characterid, character, levelid, level, os, version ,date, time, " +
               " totalPoints, longestShot, totalDistance, maxShotMade, maxShotAtt, consecutiveShots, trafficEnabled, " +
               "hardcoreEnabled, enemiesEnabled, enemiesKilled, platform, device, ipaddress, twoMade, twoAtt, threeMade, threeAtt, " +
               "fourMade, fourAtt, sevenMade, sevenAtt, bonusPoints, moneyBallMade, moneyBallAtt, userName, sniperEnabled, sniperMode, sniperModeName," +
               "sniperHits, sniperShots, p1TotalPoints,p2TotalPoints,p3TotalPoints,p4TotalPoints,first,second,third,fourth,p1IsCpu,p2IsCpu,p3IsCpu,p4IsCpu,numPlayers,difficulty)  " +
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
               + stats.UserName + "','"
               + stats.SniperEnabled + "','"
               + stats.SniperMode + "','"
               + stats.SniperModeName + "','"
               + stats.Sniperhits + "','"
               + stats.SniperShots + "','"
               + stats.p1TotalPoints + "','"
               + stats.p2TotalPoints + "','"
               + stats.p3TotalPoints + "','"
               + stats.p4TotalPoints + "','"
               + stats.firstPlace + "','"
               + stats.secondPlace + "','"
               + stats.thirdPlace + "','"
               + stats.fourthPlace + "','"
               + stats.p1IsCpu + "','"
               + stats.p2IsCpu + "','"
               + stats.p3IsCpu + "','"
               + stats.p4IsCpu + "','"
               + GameOptions.numPlayers + "','"
               + stats.Difficulty + "')";

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
    public IEnumerator InsertCheerleaderProfile(List<CheerleaderProfile> cheerleaderSelectedData)
    {
        yield return new WaitUntil(() => !databaseLocked);
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
                            + Constants.LOCAL_DATABASE_tableName_cheerleaderProfile + " ( cid, name, objectName, unlockText, isLocked) "
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
            //return;
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

            int updatePointsAvailable = PlayerData.instance.UpdatePointsAvailable;
            int updatePointsUsed = PlayerData.instance.UpdatePointsUsed;

            int counter = currentLevel - prevLevel;
            // check for levels gained. for loop in case of gaining multiple levels
            if (currentLevel > prevLevel)
            {
                for (int i = 0; i < counter; i++)
                {
                    PlayerData.instance.UpdatePointsAvailable++;
                }
            }

            // if used points is too much
            if ((updatePointsUsed + updatePointsAvailable) > currentLevel)
            {
                updatePointsUsed = currentLevel;
                updatePointsAvailable = 0;
            }
            // if used points is not enough
            if ((updatePointsUsed + updatePointsAvailable) < currentLevel)
            {
                updatePointsAvailable = currentLevel - (updatePointsUsed + updatePointsAvailable);
            }

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery1 =
               "UPDATE " + Constants.LOCAL_DATABASE_tableName_characterProfile
               + " SET experience = " + (PlayerData.instance.CurrentExperience + expGained)
               + ", level = " + currentLevel
               + ", pointsAvailable = " + updatePointsAvailable
               + ", pointsUsed = " + updatePointsUsed
               + " WHERE charid = " + GameOptions.characterId;

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
    public IEnumerator InsertCharacterProfile(List<CharacterProfile> shooterProfileList)
    {
        yield return new WaitUntil(() => !databaseLocked);
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
                            + Constants.LOCAL_DATABASE_tableName_characterProfile + " ( charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, " +
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
                        + Constants.LOCAL_DATABASE_tableName_characterProfile + " ( charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, " +
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
                        "Update " + Constants.LOCAL_DATABASE_tableName_characterProfile
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
                        //+ " AND userid = "+ GameOptions.userid;

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
                        + Constants.LOCAL_DATABASE_tableName_cheerleaderProfile + " ( cid, name, objectName, unlockText, isLocked ) "
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
    internal GameStats getAllTimeStats()
    {
        try
        {
            GameStats prevStats = gameObject.AddComponent<GameStats>();

            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            //Debug.Log("table empty : " + isTableEmpty(allTimeStatsTableName));

            if (!isTableEmpty(Constants.LOCAL_DATABASE_tableName_allTimeStats))
            {
                //Debug.Log(" table is not empty");
                sqlQuery = "Select * From " + Constants.LOCAL_DATABASE_tableName_allTimeStats;

                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    prevStats.TwoPointerMade = reader.GetInt32(1);
                    prevStats.TwoPointerAttempts = reader.GetInt32(2);
                    prevStats.ThreePointerMade = reader.GetInt32(3);
                    prevStats.ThreePointerAttempts = reader.GetInt32(4);
                    prevStats.FourPointerMade = reader.GetInt32(5);
                    prevStats.FourPointerAttempts = reader.GetInt32(6);
                    prevStats.SevenPointerMade = reader.GetInt32(7);
                    prevStats.SevenPointerAttempts = reader.GetInt32(8);
                    prevStats.MoneyBallMade = reader.GetInt32(9);
                    prevStats.MoneyBallAttempts = reader.GetInt32(10);
                    prevStats.TotalPoints = reader.GetInt32(11);
                    prevStats.TotalDistance = reader.GetFloat(12);
                    prevStats.LongestShotMade = reader.GetFloat(13);
                    prevStats.TimePlayed = reader.GetFloat(14);
                    if (reader.IsDBNull(15))
                    {
                        prevStats.EnemiesKilled = 0;
                    }
                    else
                    {
                        prevStats.EnemiesKilled = reader.GetInt32(15);
                    }
                    prevStats.SniperHits = reader.GetInt32(16);
                    //prevStats.SniperHits = Convert.ToInt32(reader.GetInt32(15));
                    prevStats.SniperShots = reader.GetInt32(17);
                    //prevStats.SniperShots = Convert.ToInt32(reader.GetInt32(16));
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
    public List<CharacterProfile> getCharacterProfileStats(int userid)
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

            if (!isTableEmpty(Constants.LOCAL_DATABASE_tableName_characterProfile))
            {
                sqlQuery = "Select charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, speed,"
                    + "runSpeed, runSpeedHasBall, luck, shootAngle, experience, level, pointsAvailable, pointsUsed, range, release, isLocked"
                    + " From " + Constants.LOCAL_DATABASE_tableName_characterProfile;
                //if (userid == 0)
                //{
                //    sqlQuery = "Select charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, speed,"
                //    + "runSpeed, runSpeedHasBall, luck, shootAngle, experience, level, pointsAvailable, pointsUsed, range, release, isLocked"
                //    + " From " + Constants.LOCAL_DATABASE_tableName_characterProfile;
                //}
                //else
                //{
                //    sqlQuery = "Select charid, playerName, objectName, accuracy2, accuracy3, accuracy4, accuracy7, jump, speed,"
                //        + "runSpeed, runSpeedHasBall, luck, shootAngle, experience, level, pointsAvailable, pointsUsed, range, release, isLocked"
                //        + " From " + Constants.LOCAL_DATABASE_tableName_characterProfile
                //        + "WHERE userid = " + userid;
                //}

                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                //CharacterProfile temp = new CharacterProfile();
                while (reader.Read())
                {
                    CharacterProfile temp = gameObject.AddComponent<CharacterProfile>();

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

            if (!isTableEmpty(Constants.LOCAL_DATABASE_tableName_cheerleaderProfile))
            {
                sqlQuery = "Select cid, name, objectName, unlockText, isLocked "
                    + " From " + Constants.LOCAL_DATABASE_tableName_cheerleaderProfile;

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
    public void InsertUser(UserModel user)
    {
        StartCoroutine(InsertUserCoroutine(user));
    }

    private IEnumerator InsertUserCoroutine(UserModel user)
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

            //Debug.Log(sqlQuery1);

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
    public List<UserModel> getUserProfileStats()
    {
        List<UserModel> userModel = new List<UserModel>();
        try
        {
            DatabaseLocked = true;

            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (!isTableEmpty(Constants.LOCAL_DATABASE_tableName_user))
            {
                sqlQuery = "Select userid, username, firstname, lastname, email, ipaddress, signupdate, lastlogin, password,"
                    + "bearerToken"
                    + " From " + Constants.LOCAL_DATABASE_tableName_user
                    + " ORDER BY lastlogin ASC";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();
                //Debug.Log(sqlQuery);

                while (reader.Read())
                {
                    UserModel temp = new UserModel();

                    temp.Userid = reader.GetInt32(0);
                    temp.UserName = reader.GetString(1);
                    temp.FirstName = reader.GetString(2);
                    temp.LastName = reader.GetString(3);
                    temp.Email = reader.GetString(4);
                    temp.IpAddress = reader.GetString(5);
                    temp.SignUpDate = reader.GetString(6);
                    temp.LastLogin = reader.GetString(7);
                    temp.Password = reader.GetString(8);
                    if (reader.IsDBNull(9))
                    {
                        temp.BearerToken = "";
                    }
                    else
                    {
                        temp.BearerToken = reader.GetString(9);
                    }
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
            return new List<UserModel>();
        }
    }

    public bool localUserExists(UserModel user)
    {
        int count = 0;
        try
        {
            DatabaseLocked = true;

            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (!isTableEmpty(Constants.LOCAL_DATABASE_tableName_user))
            {
                sqlQuery = "Select * From " + Constants.LOCAL_DATABASE_tableName_user + " WHERE username = '" + user.UserName + "'";
                //Debug.Log(sqlQuery);
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    count++;
                }

                reader.Close();
                reader = null;
                dbcmd.Dispose();
                dbcmd = null;
                dbconn.Close();
                dbconn = null;
            }
            databaseLocked = false;
            if (count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        catch (Exception e)
        {
            databaseLocked = false;
            Debug.Log("ERROR : " + e);
            return false;
        }
    }
    // update all time stats
    internal void UpdateAllTimeStats(GameStats stats)
    {
        try
        {
            databaseLocked = true;
            String sqlQuery = "";
            // get prev stats that current stats will be added to
            GameStats prevStats = getAllTimeStats();

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            if (isTableEmpty(Constants.LOCAL_DATABASE_tableName_allTimeStats))
            {
                sqlQuery =
               "Insert INTO " + Constants.LOCAL_DATABASE_tableName_allTimeStats + " ( twoMade, twoAtt, threeMade, threeAtt, fourMade, FourAtt, sevenMade, " +
               "sevenAtt, totalPoints, moneyBallMade, moneyBallAtt, totalDistance, timePlayed, longestShot, enemiesKilled, sniperHits, sniperShots)  " +
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
               stats.EnemiesKilled + "','" +
               stats.SniperHits + "','" +
               stats.SniperShots + "')";
            }
            else
            {
                sqlQuery =
               "Update " + Constants.LOCAL_DATABASE_tableName_allTimeStats +
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
               ", sniperHits = " + (prevStats.SniperHits += stats.SniperHits) +
               ", sniperShots = " + (prevStats.SniperShots += stats.SniperShots) +
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
        bool sniperValue,
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
        int sniperEnabled = 0;
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

            sqlQuery = BuildSqlQueryForGetHighScoreRows(field, modeid, hardcoreValue, trafficValue, enemiesValue, sniperValue, pageNumberOffset);
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
                // if filters selected
                if (hardcoreValue || trafficValue || enemiesValue || sniperValue)
                {
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
                    // null check
                    if (reader.IsDBNull(8))
                    {
                        sniperEnabled = 0;
                    }
                    else
                    {
                        sniperEnabled = reader.GetInt32(8);
                    }
                    username = reader.GetString(9);
                }
                // filters not selected
                else
                {
                    username = reader.GetString(5);
                }

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

    private static string BuildSqlQueryForGetHighScoreRows(string field, int modeid, bool hardcoreValue, bool trafficValue, bool enemiesValue, bool sniperValue, int pageNumberOffset)
    {
        string sqlQuery;
        // if no filter selected, return all
        if (!hardcoreValue && !trafficValue && !enemiesValue && !sniperValue)
        {
            // game modes that require float values/ low time as high score
            if (modeid > 4 && modeid < 14 && modeid != 6 && modeid != 99)
            {
                sqlQuery = "SELECT  " + field + ", character, level, date, time, userName FROM HighScores  WHERE modeid = " + modeid
                    + " ORDER BY "
                    + field + " ASC,time ASC LIMIT 10 OFFSET " + pageNumberOffset;
            }
            else
            {
                sqlQuery = "SELECT  " + field + ", character, level, date, time, userName FROM HighScores  WHERE modeid = " + modeid
                    + " ORDER BY "
                    + field + " DESC, time ASC LIMIT 10 OFFSET " + pageNumberOffset;
            }
        }
        // filters selected, filter results
        else
        {
            // game modes that require float values/ low time as high score
            if (modeid > 4 && modeid < 14 && modeid != 6 && modeid != 99)
            {
                sqlQuery = "SELECT  " + field + ", character, level, date, time, hardcoreEnabled, " +
                    "trafficEnabled, enemiesEnabled, sniperEnabled, userName FROM HighScores  WHERE modeid = " + modeid
                    + " AND hardcoreEnabled = " + Convert.ToInt32(hardcoreValue)
                    + " AND trafficEnabled = " + Convert.ToInt32(trafficValue)
                    + " AND enemiesEnabled = " + Convert.ToInt32(enemiesValue)
                    + " AND sniperEnabled = " + Convert.ToInt32(sniperValue)
                    + " ORDER BY "
                    + field + " ASC,time ASC LIMIT 10 OFFSET " + pageNumberOffset;

            }
            else
            {
                sqlQuery = "SELECT  " + field + ", character, level, date, time, hardcoreEnabled," +
                    "trafficEnabled, enemiesEnabled, sniperEnabled, userName FROM HighScores  WHERE modeid = " + modeid
                    + " AND hardcoreEnabled = " + Convert.ToInt32(hardcoreValue)
                    + " AND trafficEnabled = " + Convert.ToInt32(trafficValue)
                    + " AND enemiesEnabled = " + Convert.ToInt32(enemiesValue)
                    + " AND sniperEnabled = " + Convert.ToInt32(sniperValue)
                    + " ORDER BY "
                    + field + " DESC, time ASC LIMIT 10 OFFSET " + pageNumberOffset;
            }
        }

        return sqlQuery;
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
            Debug.Log(" value : " + value);
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


    //====================================================================================================
    public float getLongestShotMadeShots()
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

            // get all all values sort DESC, return top 1
            string sqlQuery = "SELECT longestShot from HighScores ORDER BY longestShot DESC LIMIT 1";

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

    public void deleteLocalUser(string username)
    {
        try
        {
            databaseLocked = true;

            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            string sqlQuery = "DELETE FROM User Where username  = '" + username + "'";
            //Debug.Log(sqlQuery);
            dbcmd.CommandText = sqlQuery;
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
            databaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }

    //public void alterTableAddColumn(string tableName, string columnName, string type)
    //{
    //    try
    //    {
    //        databaseLocked = true;
    //        if (!doesColumnExist(tableName, columnName))
    //        {
    //            IDbConnection dbconn;
    //            dbconn = (IDbConnection)new SqliteConnection(connection);
    //            dbconn.Open(); //Open connection to the database.
    //            IDbCommand dbcmd = dbconn.CreateCommand();

    //            string sqlQuery = "ALTER TABLE " + tableName + " ADD COLUMN " + columnName + " " + type + " NOT NULL DEFAULT none;";
    //            dbcmd.CommandText = sqlQuery;

    //            IDataReader reader = dbcmd.ExecuteReader();

    //            reader.Close();
    //            reader = null;
    //            dbcmd.Dispose();
    //            dbcmd = null;
    //            dbconn.Close();
    //            dbconn = null;
    //        }
    //    }
    //    catch (Exception e)
    //    {
    //        databaseSuccessfullyUpgraded = false;
    //        Debug.Log("database upgrade to version " + currentDatabaseAppVersion + " failed");
    //        Debug.Log("ERROR : " + e);
    //        databaseLocked = false;
    //        return;
    //    }
    //}

    //public bool doesColumnExist(string tableName, string columnName)
    //{
    //    try
    //    {
    //        databaseLocked = true;

    //        IDbConnection dbconn;
    //        dbconn = (IDbConnection)new SqliteConnection(connection);
    //        dbconn.Open(); //Open connection to the database.
    //        IDbCommand dbcmd = dbconn.CreateCommand();

    //        string sqlQueryCheckForColumn = "PRAGMA table_info(" + tableName + ")";

    //        dbcmd.CommandText = sqlQueryCheckForColumn;
    //        IDataReader reader = dbcmd.ExecuteReader();

    //        int nameIndex = reader.GetOrdinal("Name");

    //        while (reader.Read())
    //        {
    //            if (reader.GetString(nameIndex).Equals(columnName))
    //            {
    //                //Debug.Log("column : " + columnName + " found");
    //                return true;
    //            }
    //        }

    //        reader.Close();
    //        reader = null;
    //        dbcmd.Dispose();
    //        dbcmd = null;
    //        dbconn.Close();
    //        dbconn = null;

    //        databaseLocked = false;
    //    }
    //    catch
    //    {
    //        databaseLocked = false;
    //        return false;
    //    }
    //    return false;
    //}

    // insert current game's stats and score
    public void setGameScoreSubmitted(string scoreid, bool value)
    {
        //Debug.Log("setGameScoreSubmitted");
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
            string sqlQuery = "UPDATE " + Constants.LOCAL_DATABASE_tableName_highscores + " SET submittedToApi" + " = " + submittedValue
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

            databaseLocked = false;
            //Debug.Log("score submitted to database");
        }
        catch (Exception e)
        {
            DatabaseLocked = false;
            Debug.Log("ERROR : " + e);
        }
    }

    public List<HighScoreModel> getUnsubmittedHighScoreFromDatabase()
    {
        List<HighScoreModel> highscores = new List<HighScoreModel>();
        databaseLocked = true;
        try
        {
            String sqlQuery = "";
            IDbConnection dbconn;
            dbconn = (IDbConnection)new SqliteConnection(connection);
            dbconn.Open(); //Open connection to the database.
            IDbCommand dbcmd = dbconn.CreateCommand();

            //Debug.Log("table empty : " + isTableEmpty(allTimeStatsTableName));

            if (!isTableEmpty(Constants.LOCAL_DATABASE_tableName_highscores))
            {
                sqlQuery = "Select  * From " + Constants.LOCAL_DATABASE_tableName_highscores
                    + " WHERE submittedToApi = 0 "
                    + " AND modeid != 99";
                    //+ " AND userName = ''";
                dbcmd.CommandText = sqlQuery;
                IDataReader reader = dbcmd.ExecuteReader();

                while (reader.Read())
                {
                    HighScoreModel highscore = new HighScoreModel();

                    //if (reader.IsDBNull(1)) { highscore.Userid = 0; }
                    //else { highscore.Userid = reader.GetInt32(1); }
                    highscore.Scoreid = reader.GetString(1);
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
                    highscore.EnemiesEnabled = reader.GetInt32(19);
                    highscore.EnemiesKilled = reader.GetInt32(20);
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
                    highscore.BonusPoints = reader.GetInt32(32);
                    highscore.MoneyBallMade = reader.GetInt32(33);
                    highscore.MoneyBallAtt = reader.GetInt32(34);
                    //highscore.submittedToApi = reader.GetInt32(36);
                    highscore.UserName = reader.GetString(36).ToString();
                    //highscore.Userid = GameOptions.userid;
                    highscore.SniperEnabled = reader.GetInt32(37);
                    highscore.SniperMode = reader.GetInt32(38);
                    highscore.SniperModeName = reader.GetString(39);
                    highscore.Sniperhits = reader.GetInt32(40);
                    highscore.SniperShots = reader.GetInt32(41);
                    highscore.p1TotalPoints = reader.GetInt32(42);
                    highscore.p2TotalPoints = reader.GetInt32(43);
                    highscore.p3TotalPoints = reader.GetInt32(44);
                    highscore.p4TotalPoints = reader.GetInt32(45);
                    highscore.firstPlace = reader.GetString(46);
                    highscore.secondPlace = reader.GetString(47);
                    highscore.thirdPlace = reader.GetString(48);
                    highscore.fourthPlace = reader.GetString(49);
                    highscore.p1IsCpu = reader.GetInt32(50);
                    highscore.p2IsCpu = reader.GetInt32(51);
                    highscore.p3IsCpu = reader.GetInt32(52);
                    highscore.p4IsCpu = reader.GetInt32(53);
                    highscore.numPlayers = reader.GetInt32(54);

                    // if username empty on unsubmitted score
                    // but user logged in [gameoptions.username != null/empty
                    // add logged in username to score and submit
                    if ((string.IsNullOrEmpty(highscore.UserName) || string.IsNullOrWhiteSpace(highscore.UserName))
                        && (!string.IsNullOrWhiteSpace(GameOptions.userName) || !string.IsNullOrEmpty(GameOptions.userName)))
                    {
                        highscore.UserName = GameOptions.userName;
                        highscores.Add(highscore);
                    }
                    // if username != null or empty, add to list
                    // this will catch if user has logged in
                    if (!string.IsNullOrEmpty(highscore.UserName) || !string.IsNullOrWhiteSpace(highscore.UserName))
                    {
                        highscores.Add(highscore);
                    }
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
        return highscores;
    }

    public bool DatabaseLocked { get => databaseLocked; set => databaseLocked = value; }
}
