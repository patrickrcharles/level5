using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEditor.U2D.Sprites;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameRules : MonoBehaviour
{
    private int playerId;
    private int levelId;
    private int gameModeId;
    private float timerStart;

    private bool gameOver;
    private bool gameStart;
    private bool gameRulesEnabled;

    private bool modeRequiresCounter;
    private bool modeRequiresCountDown;

    //private float timeStart;
    private Timer timer;
    private BasketBallStats basketBallStats;

    public static GameRules instance;

    // object name that displays score
    private const string displayScoreObjectName = "display_score";
    private const string displayCurrentScoreObjectName = "display_current_score";
    private const string displayHighScoreObjectName = "display_high_score";

    private Text displayScoreText;
    private Text displayCurrentScoreText;
    private Text displayHighScoreText;

    private float timeCompleted;

    [SerializeField]
    private GameObject[] basketBallShotMarkerObjects;
    [SerializeField]
    private List<BasketBallShotMarker> _basketBallShotMarkersList;
    private int markersRemaining;

    private bool positionMarkersRequired;


    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameOver = false;
        gameModeId = GameOptions.gameModeSelected;
        timer = GameObject.Find("timer").GetComponent<Timer>();
        //setTimer(timeStart);

        basketBallStats = BasketBall.instance.BasketBallStats;
        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();
        displayHighScoreText = GameObject.Find(displayHighScoreObjectName).GetComponent<Text>();

        displayScoreText.text = "";
        displayCurrentScoreText.text = "";
        displayHighScoreText.text = "";

        gameRulesEnabled = true;

        //===================================================== Position markers set up ====================================================
        // * move to method after testing

        // if mode requires 3s or 4s
        // enable 3s or 4s

        // after testing insert this
        /*
         * if( game options. mode requires position markers)
         */

        //testing line
        positionMarkersRequired = GameOptions.gameModeSelected == 8;
        // get all shot position marker objects
        basketBallShotMarkerObjects = GameObject.FindGameObjectsWithTag("shot_marker");
        //load them into list
        foreach (var marker in basketBallShotMarkerObjects)
        {
            BasketBallShotMarker temp = marker.GetComponent<BasketBallShotMarker>();
            _basketBallShotMarkersList.Add(temp);
        }

        if (positionMarkersRequired) 
        {
            Debug.Log("        if (positionMarkersRequired) ");
            markersRemaining = BasketBallShotMarkersList.Count;
        }
        else
        {
            Debug.Log(" else :: disable marker ::::::::::::::::::");
            foreach (var marker in BasketBallShotMarkersList)
            {
                // 
                marker.enabled = false;
            }
        }

        //===================================================== Position markers set up ====================================================
    }


    void Update()
    {

        // update current score
        if (gameRulesEnabled)
        {
            setScoreDisplayText();
        }
        if (gameOver && gameRulesEnabled)
        {
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";
        }

        if (GameOver && !Pause.instance.Paused && gameRulesEnabled)
        {
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";
            //pause on game over
            Pause.instance.TogglePause();
            displayScoreText.text = getDisplayText(GameModeId);
            //save
            if (GameObject.Find("PlayerData") != null)
            {
                PlayerData.instance.saveStats();
            }

            // alert game manager
            GameLevelManager.Instance.GameOver = true;
        }
    }

    private void setScoreDisplayText()
    {
        if (gameModeId == 1)
        {
            displayCurrentScoreText.text = "total points : " + BasketBall.instance.BasketBallStats._totalPoints
                + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
            displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPoints;
        }
        if (gameModeId == 2)
        {
            displayCurrentScoreText.text = "3s made : " + BasketBall.instance.BasketBallStats.ThreePointerMade
                + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
            displayHighScoreText.text = "high score : " + PlayerData.instance.ThreePointerMade;
        }
        if (gameModeId == 3)
        {
            displayCurrentScoreText.text = "4s made : " + BasketBall.instance.BasketBallStats.FourPointerMade
                + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
            displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointerMade;
        }
        if (gameModeId == 4)
        {
            displayCurrentScoreText.text = "7s made : " + BasketBall.instance.BasketBallStats.SevenPointerMade
                                                        + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
            displayHighScoreText.text = "high score : " + PlayerData.instance.SevenPointerMade;
        }
        if (gameModeId == 5)
        {
            displayCurrentScoreText.text = "longest shot : " + (BasketBall.instance.BasketBallStats.LongestShotMade * 6).ToString("0.00")
                + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("00.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMade.ToString("0.00");
        }
        if (gameModeId == 6)
        {
            displayCurrentScoreText.text = "total distance : " + (BasketBall.instance.BasketBallStats.TotalDistance * 6).ToString("0.00")
            + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance.TotalDistance.ToString("0.00");
        }
        if (gameModeId == 7)
        {
            displayCurrentScoreText.text = "longest shot : " + (BasketBall.instance.BasketBallStats.LongestShotMade * 6).ToString("0.00")
                                                             + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("00.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00");
            // if longest shot > saved longest shot
            if ((BasketBall.instance.BasketBallStats.LongestShotMade * 6) > PlayerData.instance.LongestShotMadeFreePlay)
            {
                Debug.Log("save");
                PlayerData.instance.saveStats();
            }
        }
    }

    private string getDisplayText(int modeId)
    {
        //Debug.Log("display data  mode: "+ GameModeId);

        string displayText = "";

        if (gameModeId == 1)
        {
            displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        }
        if (gameModeId == 2)
        {
            displayText = "You made " + basketBallStats.ThreePointerMade + " total 3 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 3)
        {
            displayText = "You made " + basketBallStats.FourPointerMade + " total 4 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 4)
        {
            displayText = "You made " + basketBallStats.SevenPointerMade + " total 4 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 5)
        {
            displayText = "Your longest shot made was " + (basketBallStats.LongestShotMade * 6).ToString("0.00") + " ft.\n\n" + getStatsTotals();
        }
        if (gameModeId == 6)
        {
            displayText = "Your total distance for shots made was " + (basketBallStats.TotalDistance * 6).ToString("0.00") + " ft.\n\n" + getStatsTotals();
        }

        return displayText;
    }

    string getStatsTotals()
    {
        //Debug.Log("getStatsTotals()");
        string scoreText = "";
        scoreText = "shots  : " + basketBallStats.ShotMade + " / " + basketBallStats.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                         + "points : " + basketBallStats.TotalPoints + "\n"
                         + "2 pointers : " + basketBallStats.TwoPointerMade + " / " + basketBallStats.TwoPointerAttempts + "    "
                         + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                         + "3 pointers : " + basketBallStats.ThreePointerMade + " / " + basketBallStats.ThreePointerAttempts + "    "
                         + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                         + "4 pointers : " + basketBallStats.FourPointerMade + " / " + basketBallStats.FourPointerAttempts + "    "
                         + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                         + "7 pointers : " + basketBallStats.SevenPointerMade + " / " + basketBallStats.SevenPointerAttempts + "    "
                         + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                         + "longest shot distance : " + (Math.Round(basketBallStats.LongestShotMade, 2) * 6f).ToString("0.00") + " ft.\n"
                         + "total shots made distance : " + (Math.Round(basketBallStats.TotalDistance, 2) * 6f).ToString("0.00") + " ft.";

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

    public List<BasketBallShotMarker> BasketBallShotMarkersList
    {
        get => _basketBallShotMarkersList;
        set => _basketBallShotMarkersList = value;
    }

    public int MarkersRemaining
    {
        get => markersRemaining;
        set => markersRemaining = value;
    }
}
