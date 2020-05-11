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

    bool gameModeRequiresShotMarkers3s;
    bool gameModeRequiresShotMarkers4s;

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

    // all these specific game rules for each will need to moved to a different file eventually on refactor
    private GameObject[] basketBallShotMarkerObjects;
    [SerializeField]
    private List<BasketBallShotMarker> _basketBallShotMarkersList;

    private int markersRemaining;
    private bool positionMarkersRequired;
    public bool PositionMarkersRequired => positionMarkersRequired;

    private float counterTime; // this is set when shot is made that ends game : class BasketBallShotMade (attached to rim)

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        gameOver = false;
        gameModeId = GameOptions.gameModeSelected;
        timer = GameObject.Find("timer").GetComponent<Timer>();

        modeRequiresCounter = GameOptions.gameModeRequiresCounter;
        modeRequiresCountDown = GameOptions.gameModeRequiresCountDown;

        basketBallStats = BasketBall.instance.BasketBallStats;
        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();
        displayHighScoreText = GameObject.Find(displayHighScoreObjectName).GetComponent<Text>();

        displayScoreText.text = "";
        displayCurrentScoreText.text = "";
        displayHighScoreText.text = "";

        gameRulesEnabled = true;

        // enable/disable necessary shot markers for game mode
        if (GameOptions.gameModeRequiresShotMarkers3s || GameOptions.gameModeRequiresShotMarkers4s)
        {
            positionMarkersRequired = true;
            setPositionMarkers();
        }
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
            Debug.Log("game over, pause");
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";

            //pause on game over
            Pause.instance.TogglePause();
            displayScoreText.text = getDisplayText(GameModeId);

            //save
            if (GameObject.Find("player_data") != null)
            {
                PlayerData.instance.saveStats();
            }

            // alert game manager
            GameLevelManager.Instance.GameOver = true;
        }
    }

    //===================================================== Position markers set up ====================================================
    private void setPositionMarkers()
    {
        // get all shot position marker objects
        basketBallShotMarkerObjects = GameObject.FindGameObjectsWithTag("shot_marker");

        gameModeRequiresShotMarkers3s = GameOptions.gameModeRequiresShotMarkers3s;
        gameModeRequiresShotMarkers4s = GameOptions.gameModeRequiresShotMarkers4s;

        //load them into list
        foreach (var marker in basketBallShotMarkerObjects)
        {
            BasketBallShotMarker temp = marker.GetComponent<BasketBallShotMarker>();
            // if 3 markers not required, disable them
            if (!gameModeRequiresShotMarkers3s && temp.ShotTypeThree)
            {
                marker.SetActive(false);
            }
            // if 4 markers not required, disable them
            if (!gameModeRequiresShotMarkers4s && temp.ShotTypeFour)
            {
                marker.SetActive(false);
            }
            // add all active and enabled markers to list
            if (temp.isActiveAndEnabled)
            {
                BasketBallShotMarkersList.Add(temp);
                temp.PositionMarkerId = BasketBallShotMarkersList.Count - 1;
            }
        }
        // number of markers to complete ( all active and enabled sshot markers based on game options
        markersRemaining = BasketBallShotMarkersList.Count;
    }

    private void setScoreDisplayText()
    {
        if (gameModeId == 1)
        {
            displayCurrentScoreText.text = "total points : " + BasketBall.instance.BasketBallStats.TotalPoints
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
            displayCurrentScoreText.text = "";
            //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance._makeThreePointersLowTime;
        }
        if (gameModeId == 8)
        {
            displayCurrentScoreText.text = "";
            //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
            //displayHighScoreText.text = "high score : " + PlayerData.instance.TotalDistance.ToString("0.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance._makeFourPointersLowTime;
        }
        if (gameModeId == 9)
        {
            displayCurrentScoreText.text = "";
            //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance._makeAllPointersLowTime;
        }
        if (gameModeId == 10)
        {
            displayCurrentScoreText.text = "longest shot : " + (BasketBall.instance.BasketBallStats.LongestShotMade * 6).ToString("0.00")
                                                             + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("00.00");
            displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00");
            // if longest shot > saved longest shot
            if ((BasketBall.instance.BasketBallStats.LongestShotMade * 6) > PlayerData.instance.LongestShotMadeFreePlay)
            {
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
        if (gameModeId == 7 || gameModeId == 8 || gameModeId == 9)
        {
            displayText = "Your time to complete all shots was " + (counterTime).ToString("0.000") + "\n\n" + getStatsTotals();
        }

        return displayText;
    }

    string getStatsTotals()
    {
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

    public bool isGameOver()
    {
        if (MarkersRemaining <= 0)
        {
            //set counter timer
            counterTime = Timer.instance.CurrentTime;
            if (modeRequiresCounter)
            {
                setRequiresCounterLowScore();
            }

            return true;
        }
        else
        {
            return false;
        }
    }

    private void setRequiresCounterLowScore()
    {
        Debug.Log("setRequiresCounterLowScore()");
        if (gameModeRequiresShotMarkers3s && !gameModeRequiresShotMarkers4s)
        {
            basketBallStats.MakeThreePointersLowTime = counterTime;
            Debug.Log("basketBallStats.MakeThreePointersLowTime : " + basketBallStats.MakeThreePointersLowTime);
        }

        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s)
        {
            basketBallStats.MakeFourPointersLowTime = counterTime;
            Debug.Log("basketBallStats.Make4PointersLowTime : " + basketBallStats.MakeFourPointersLowTime);
        }

        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s)
        {
            basketBallStats.MakeAllPointersLowTime = counterTime;
            Debug.Log("basketBallStats.MakeallPointersLowTime : " + basketBallStats.MakeAllPointersLowTime);
        }
    }

    public float CounterTime
    {
        get => counterTime;
        set => counterTime = value;
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
