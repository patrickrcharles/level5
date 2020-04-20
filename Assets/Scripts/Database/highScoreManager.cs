using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data;
using Mono.Data.Sqlite;
using UnityEngine.UI;

public class highScoreManager : MonoBehaviour
{

    private string connection;
    List<string> nameList = new List<string>(10);
    List<string> levelList = new List<string>(10);
    List<int> numEnemiesList = new List<int>(10);
    List<string> timeList = new List<string>(10);
    List<int> killsList = new List<int>(10);
    List<int> currentScoreList = new List<int>(10);
    List<int> livesUsedList = new List<int>(10);
    List<string> dateList = new List<string>(10);
    List<string> scores;

    [SerializeField]
    Text nameObject, levelObject, enemiesObject, timeObject,
        killsObject, scoreObject, livesUsedObject, dateObject;

    string sqlQuery;
    string name, level, enemies, time, kills, score, lives, dates;

    // Use this for initialization
    void Start()
    {
        nameObject = GameObject.Find("name_text").GetComponent<Text>();
        levelObject = GameObject.Find("level_text").GetComponent<Text>();
        enemiesObject = GameObject.Find("enemies_text").GetComponent<Text>();
        timeObject = GameObject.Find("time_text").GetComponent<Text>();
        killsObject = GameObject.Find("kills_text").GetComponent<Text>();
        scoreObject = GameObject.Find("score_text").GetComponent<Text>();
        livesUsedObject = GameObject.Find("lives_text").GetComponent<Text>();
        dateObject = GameObject.Find("date_text").GetComponent<Text>();

        scores = new List<string>();
        connection = "URI=file:" + Application.dataPath + "/HighScoreDB.db";
        createTable();
        getScores();
    }

    // Update is called once per frame
    void Update()
    {


    }

    // for demo mode 1 scores
    private void getScores()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connection))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = "SELECT * FROM demo1 ORDER BY score DESC LImit 10";
                dbCmd.CommandText = sqlQuery;

                using (IDataReader reader = dbCmd.ExecuteReader())
                {
                    int i = 0;
                    while (reader.Read())
                    {
                        nameList.Add(reader.GetString(1));
                        levelList.Add(reader.GetString(2));
                        numEnemiesList.Add(reader.GetInt32(3));
                        timeList.Add(reader.GetString(4));
                        killsList.Add(reader.GetInt32(5));
                        currentScoreList.Add(reader.GetInt32(6));
                        dateList.Add(reader.GetString(7));
                        livesUsedList.Add(reader.GetInt32(8));
                        i++;
                    }
                    dbConnection.Close();
                    reader.Close();
                }
            }
            // create column of text for display
            createDisplayStrings();
        }
        int j = 0;
        foreach (int s in currentScoreList)
        {
            Debug.Log("currentScore[" + j + "] :: " + s);
            j++;
        }
    }

    private void createDisplayStrings()
    {
        for (int i = 0; i < 10; i++)
        {
            name += nameList[i] + "\n";
            level += levelList[i] + "\n";
            enemies += numEnemiesList[i].ToString() + "\n";
            time += timeList[i] + "\n";
            kills += killsList[i].ToString() + "\n";
            score += currentScoreList[i] + "\n";
            dates += dateList[i] + "\n";
            lives += livesUsedList[i].ToString() + "\n";
        }
        Debug.Log("name :: "+ name);
        nameObject.text = name;
        levelObject.text = level;
        enemiesObject.text = enemies;
        timeObject.text = time;
        killsObject.text = kills;
        scoreObject.text = score;
        dateObject.text = dates;
        livesUsedObject.text = lives;

    }


    public void insertScore(string nme, string lvl, int numEn, string tim,
        int kill, int scor, string dat, int pLives)
    {

        using (IDbConnection dbConnection = new SqliteConnection(connection))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                if (enemySpawner.instance.gameMode1 == true)
                {
                    sqlQuery = String.Format("INSERT INTO demo1(name,level,numEnemies,time,kills,score,date,livesUsed) " +
                    "VALUES(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\")", nme, lvl, numEn, tim, kill, scor, dat, pLives);
                    Debug.Log("insertScore sql query : " + sqlQuery);
                }
                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }


    public void deleteScore()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connection))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                /*
                string sqlQuery = String.Format("DELETE FROM demo1 WHERE (name,level,numEnemies,time,kills,score,date) " +
                    "VALUES(\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\")", nme, lvl, numEn, tim, kill, scor, dat);
                */

                // deletes row by lowest value
                string sqlQuery =
                    String.Format("DELETE FROM demo1 WHERE score IN (select min(score) from demo1 limit 1)");

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }

    private void createTable()
    {
        using (IDbConnection dbConnection = new SqliteConnection(connection))
        {
            dbConnection.Open();
            using (IDbCommand dbCmd = dbConnection.CreateCommand())
            {
                string sqlQuery = String.Format("CREATE TABLE if not exists demo1(" +
                "playerID  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "name  TEXT," +
                "level  TEXT," +
                "numEnemies  INTEGER," +
                "time  TEXT," +
                "kills  INTEGER," +
                "score  INTEGER," +
                "date  TEXT," +
                "livesUsed INTEGER);" +

                "CREATE TABLE if not exists demo2(" +
                "playerID  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL," +
                "name  TEXT," +
                "level  TEXT," +
                "numEnemies  INTEGER," +
                "time  TEXT," +
                "kills  INTEGER," +
                "score  INTEGER," +
                "date  TEXT," +
                "livesUsed INTEGER);");

                //Debug.Log("sql query : " + sqlQuery);

                dbCmd.CommandText = sqlQuery;
                dbCmd.ExecuteScalar();
                dbConnection.Close();
            }
        }
    }

    /*
        void displayScoresOnUI()
        {
            //name1.GetComponent<Text>().text = scores[0];

            //string scoreText="";
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                {
                    name1.GetComponent<Text>().text = scores[i] + "\n";
                    level1.GetComponent<Text>().text = scores[i] + "\n";
                    numEnemies1.GetComponent<Text>().text = scores[i] + "\n";
                    time1.GetComponent<Text>().text = scores[i] + "\n";
                    kills1.GetComponent<Text>().text = scores[i] + "\n";
                    score1.GetComponent<Text>().text = scores[i] + "\n";
                    date1.GetComponent<Text>().text = scores[i] + "\n";
                    livesUsed1.GetComponent<Text>().text = scores[i] + "\n";
                }
                else
                {
                    name1.GetComponent<Text>().text += scores[i] + "\n";
                    level1.GetComponent<Text>().text += scores[i] + "\n";
                    numEnemies1.GetComponent<Text>().text += scores[i] + "\n";
                    time1.GetComponent<Text>().text += scores[i] + "\n";
                    kills1.GetComponent<Text>().text += scores[i] + "\n";
                    score1.GetComponent<Text>().text += scores[i] + "\n";
                    date1.GetComponent<Text>().text += scores[i] + "\n";
                    livesUsed1.GetComponent<Text>().text += scores[i] + "\n";
                }
            }
        }
        */
}
