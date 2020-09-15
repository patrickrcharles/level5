using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsTableHighScore : MonoBehaviour
{
    [SerializeField]
    List<StatsTableHighScoreRow> highScoreRowsList;

    //const string highScoresTableName = "high_scores_rows";
    //GameObject highScoresTableObject;

    // Start is called before the first frame update
    void Start()
    {

        // function to get list of all rows from db
        // insert into list of rows in retrieval

        // this where we'll spawn row prefabs
        //highScoresTableObject = GameObject.Find(highScoresTableName);

        //Debug.Log("StatsTableHighScore start()");

        //highScoreRowsList = DBHelper.instance.getListOfHighScoreRowsFromTableByModeId(1);

        //foreach (StatsTableHighScoreRow s in highScoreRowsList)
        //{
        //    Debug.Log("score = " + s.score + " | character =" + s.character + " | level = " + s.level + " | date = " + s.date);

        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void populateHighScoresListFromDatabase(int modeId, string order)
    {
        /* need a query
         * select a,b,c,d from highscores where mode id = currently selected
         * function exists already but for a single score. create a function that returns a list 
         *  include parameter for how many rresults to return
         * 
         *     public int getIntValueHighScoreFromTableByFieldAndModeId(String tableName, String field, int modeid, String order)
         *     
         * get from db
         * 
         * insert into list
         */
    }
}
