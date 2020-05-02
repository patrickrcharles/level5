using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;

public class GameRules : MonoBehaviour
{
    private int playerId;
    private int levelId;
    [SerializeField]
    private int gameModeId;
    private float timerStart;

    private bool gameMode1;
    private bool gameMode2;
    private bool gameMode3;
    private bool gameMode4;

    private bool gameOver;
    private bool gameStart;

    [SerializeField]
    private float timeStart;
    private Timer timer;
    [SerializeField]
    private BasketBallStats basketBallStats;

    // object name that displays score
    private const string displayScoreObjectName = "display_score";
    [SerializeField]
    private Text displayScoreText;

    void Start()
    {
        gameModeId = 1;
        //gameModeId = GameOptions.gameModeSelected;
        timer = GameObject.Find("timer").GetComponent<Timer>();
        setTimer(timeStart);

        basketBallStats = BasketBall.instance.BasketBallStats;
        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayScoreText.text = "";
    }

    void Update()
    {
        // if game is over,
        // get stats for display
        //if (gameOver)
        //{
        //    displayScore(gameModeId);
        //}

        //turn off accuracy modifer 6+9
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha7))
        {
            displayScore(gameModeId);
        }
    }

    private void displayScore(int modeId)
    {
        displayScoreText.text = getDisplayText(modeId);

    }

    private string getDisplayText(int modeId)
    {
        string displayText = "";
        string statsSummary = "";

        Debug.Log("getDisplayText()");
        Debug.Log("game mode id " + gameModeId);
        if (gameModeId == 1)
        {

            displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        }

        return displayText;
    }

    string getStatsTotals()
    {
        Debug.Log("getStatsTotals()");
        string scoreText = "";
        scoreText = "shots  : " + basketBallStats.ShotMade + " / " + basketBallStats.ShotAttempt + "\n"
                         + "accuracy : " + BasketBall.instance.getTotalPointAccuracy() + "%\n"
                         + "points : " + basketBallStats.TotalPoints + "\n"
                         + "2 pointers : " + basketBallStats.TwoPointerMade + " / " + basketBallStats.TwoPointerAttempts + " accuracy : " + BasketBall.instance.getTwoPointAccuracy() + "%\n"
                         + "3 pointers : " + basketBallStats.ThreePointerMade + " / " + basketBallStats.ThreePointerAttempts + " accuracy : " + BasketBall.instance.getThreePointAccuracy() + "%\n"
                         + "4 pointers : " + basketBallStats.FourPointerMade + " / " + basketBallStats.FourPointerAttempts + " accuracy : " + BasketBall.instance.getFourPointAccuracy() + "%\n"
                         + "longest shot distance : " + (Math.Round(basketBallStats.LongestShotMade, 2) * 6f).ToString("0.00") + " ft.";

        Debug.Log("scoreText : " + scoreText);

        return scoreText;
    }

    private void setTimer(float seconds)
    {
        timer.TimeStart = seconds;
    }


}
