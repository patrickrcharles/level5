using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatsTableHighScoreRow : MonoBehaviour
{
    const string scoreName = "score";
    const string characterName = "character";
    const string levelName = "level";
    const string dateName = "date";


    public Text scoreText;
    public Text characterText;
    public Text levelText;
    public Text dateText;

    [SerializeField]
    public string score;
    [SerializeField]
    public string character;
    [SerializeField]
    public string level;
    [SerializeField]
    public string date;

    // Start is called before the first frame update
    void Awake()
    {
        scoreText = transform.GetChild(0).GetComponent<Text>();
        characterText = transform.GetChild(1).GetComponent<Text>();
        levelText = transform.GetChild(2).GetComponent<Text>();
        dateText = transform.GetChild(3).GetComponent<Text>();
    }

    private void Update()
    {
        //if (score == 0)
        //{
        //    scoreText.text = "";
        //}
        //else
        //{
        //    scoreText.text = score.ToString();
        //}
        scoreText.text = score;
        characterText.text = character;
        levelText.text = level;
        dateText.text = date;

        //Debug.Log("score = " + score + " | character =" +character + " | level = " + level + " | date = " +date);
    }

    public StatsTableHighScoreRow(string scor, string charact, string lvl, string dat)
    {
        score = scor;
        character = charact;
        level = lvl;
        date = dat;
    }

}
