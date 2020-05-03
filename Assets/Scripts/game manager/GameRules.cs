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

    public static GameRules instance;

    // object name that displays score
    private const string displayScoreObjectName = "display_score";
    private const string displayCurrentScoreObjectName = "display_current_score";
    [SerializeField]
    private Text displayScoreText;
    private Text displayCurrentScoreText;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        gameModeId = 1;
        //gameModeId = GameOptions.gameModeSelected;
        timer = GameObject.Find("timer").GetComponent<Timer>();
        setTimer(timeStart);

        basketBallStats = BasketBall.instance.BasketBallStats;
        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();

        displayScoreText.text = "";
        displayCurrentScoreText.text = " total points : " + BasketBall.instance.BasketBallStats._totalPoints;
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
        displayCurrentScoreText.text = "points : " + BasketBall.instance.BasketBallStats._totalPoints;
        //turn off accuracy modifer 6+9
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha7))
        {
            displayScore(gameModeId);
        }

        if (GameOver)
        {
            displayCurrentScoreText.text = "";
            Pause.instance.TogglePause();
            //setBackgroundFade(true);
            displayScore(GameRules.instance.GameModeId);
        }
    }

    public void displayScore(int modeId)
    {
        displayScoreText.text = getDisplayText(modeId);
        if (gameOver)
        {
            displayCurrentScoreText.text = "";
        }
    }


    private string getDisplayText(int modeId)
    {
        string displayText = "";
        string statsSummary = "";

        if (gameModeId == 1)
        {
            displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n";// + getStatsTotals();
        }
        if (gameModeId == 2)
        {
            displayText = "You made " + basketBallStats.ThreePointerMade + " total 3 pointers\n\n";// + getStatsTotals();
        }
        if (gameModeId == 3)
        {
            displayText = "Your longest shot made was " + basketBallStats.LongestShotMade + " ft.\n\n";// + getStatsTotals();
        }
        if (gameModeId == 4)
        {
            displayText = "Your total distance for shots made was " + basketBallStats.TotalPoints + " ft.\n\n";// + getStatsTotals();
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
