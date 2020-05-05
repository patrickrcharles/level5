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
    private bool summaryDisplay;

    [SerializeField]
    private float timeStart;
    private Timer timer;
    [SerializeField]
    private BasketBallStats basketBallStats;

    public static GameRules instance;

    // object name that displays score
    private const string displayScoreObjectName = "display_score";
    private const string displayCurrentScoreObjectName = "display_current_score";
    private const string displayHighScoreObjectName = "display_high_score";

    private Text displayScoreText;
    private Text displayCurrentScoreText;
    private Text displayHighScoreText;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameOver = false;
        gameModeId = GameOptions.gameModeSelected;
        //gameModeId = GameOptions.gameModeSelected;
        timer = GameObject.Find("timer").GetComponent<Timer>();
        setTimer(timeStart);

        basketBallStats = BasketBall.instance.BasketBallStats;
        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();
        displayHighScoreText = GameObject.Find(displayHighScoreObjectName).GetComponent<Text>();

        displayScoreText.text = "";


        //fadeTexture = GameObject.Find("fade_texture").GetComponent<Image>();
    }


    void Update()
    {
        // if game is over,
        // get stats for display
        //if (gameOver)
        //{
        //    displayScore(gameModeId);
        //}

        // update current score
        setScoreDisplayText();
        if (gameOver)
        {
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";
        }

        if (GameOver && !Pause.instance.Paused)
        {
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";

            Pause.instance.TogglePause();
            summaryDisplay = true;
            displayScoreText.text = getDisplayText(GameModeId);
            //save
            PlayerData.instance.saveStats();
            // alert game manager
            GameLevelManager.instance.GameOver = true;
        }
    }

    //public void displayScore(int modeId)
    //{
    //    displayScoreText.text = getDisplayText(modeId);
    //}
    private void setScoreDisplayText()
    {
        if (gameModeId == 1)
        {
            displayCurrentScoreText.text = "total points : " + BasketBall.instance.BasketBallStats._totalPoints;
            displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPoints;
        }
        if (gameModeId == 2)
        {
            displayCurrentScoreText.text = "3s made : " + BasketBall.instance.BasketBallStats.ThreePointerMade;
            displayHighScoreText.text = "high score : " + PlayerData.instance.ThreePointerMade;
        }
        if (gameModeId == 3)
        {
            displayCurrentScoreText.text = "4s made : " + BasketBall.instance.BasketBallStats.FourPointerMade;
            displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointerMade;
        }
        if (gameModeId == 4)
        {
            displayCurrentScoreText.text = "longest shot : " + (BasketBall.instance.BasketBallStats.LongestShotMade * 6).ToString("0.00")
                +"\ncurrent distance : "+ (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMade.ToString("0.00");
        }
        if (gameModeId == 5)
        {
            displayCurrentScoreText.text = "total distance : " + (BasketBall.instance.BasketBallStats.TotalDistance * 6).ToString("0.00")
            +"\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance.TotalDistance.ToString("0.00");
        }
    }

    private string getDisplayText(int modeId)
    {
        Debug.Log("display data  mode: "+ GameModeId);

        string displayText = "";

        if (gameModeId == 1)
        {
            displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        }
        if (gameModeId == 2)
        {
            displayText = "You made " + basketBallStats.ThreePointerMade + " total 3 pointers\n\n"+ getStatsTotals();
        }
        if (gameModeId == 3)
        {
            displayText = "You made " + basketBallStats.FourPointerMade + " total 4 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 4)
        {
            displayText = "Your longest shot made was " + (basketBallStats.LongestShotMade * 6).ToString("0.00") + " ft.\n\n"+ getStatsTotals();
        }
        if (gameModeId == 5)
        {
            displayText = "Your total distance for shots made was " + (basketBallStats.TotalDistance * 6).ToString("0.00")+ " ft.\n\n"+ getStatsTotals();
        }
        //string temp = displayText + "\n" + "current high score is : "+ displayHighScoreText.text;

        return displayText;
    }

    string getStatsTotals()
    {
        Debug.Log("getStatsTotals()");
        string scoreText = "";
        scoreText = "shots  : " + basketBallStats.ShotMade + " / " + basketBallStats.ShotAttempt + "\n"
                         + "accuracy : " + BasketBall.instance.getTotalPointAccuracy() + "%\n"
                         + "points : " + basketBallStats.TotalPoints + "\n"
                         + "2 pointers : " + basketBallStats.TwoPointerMade + " / " + basketBallStats.TwoPointerAttempts + "    " 
                         + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                         + "3 pointers : " + basketBallStats.ThreePointerMade + " / " + basketBallStats.ThreePointerAttempts + "    " 
                         + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                         + "4 pointers : " + basketBallStats.FourPointerMade + " / " + basketBallStats.FourPointerAttempts + "    "
                         + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                         + "longest shot distance : " + (Math.Round(basketBallStats.LongestShotMade, 2) * 6f).ToString("0.00") + " ft.\n"
                         + "total shots made distance : " + (Math.Round(basketBallStats.TotalDistance, 2) * 6f).ToString("0.00") + " ft.";

        Debug.Log("scoreText : " + scoreText);

        return scoreText;
    }

    private void setTimer(float seconds)
    {
        timer.TimeStart = seconds;
    }

    public int GameModeId
    {
        get => gameModeId;
        set => gameModeId = value;
    }

    public bool GameOver
    {
        get => gameOver;
        set => gameOver = value;
    }
}
