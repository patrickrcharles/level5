﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRules : MonoBehaviour
{

    private int playerId;
    private int levelId;
    private int gameModeId;
    private float timerStart;

    [SerializeField]
    private bool gameOver;
    private bool gameStart;
    private bool gameRulesEnabled;

    private bool modeRequiresCounter;
    private bool modeRequiresCountDown;

    bool gameModeRequiresShotMarkers3s;
    bool gameModeRequiresShotMarkers4s;

    [SerializeField]
    bool gameModeThreePointContest;
    [SerializeField]
    bool gameModeFourPointContest;
    [SerializeField]
    bool gameModeAllPointContest;
    [SerializeField]
    float customTimer;

    bool gameModeRequiresMoneyBall;
    bool moneyBallEnabled;
    bool gameModeRequiresConsecutiveShots;

    private Timer timer;
    private BasketBallStats basketBallStats;

    // object name that displays score
    private const string displayScoreObjectName = "display_score";
    private const string displayCurrentScoreObjectName = "display_current_score";
    private const string displayHighScoreObjectName = "display_high_score";
    private const string displayMoneyObjectName = "money_display";
    private const string displayMoneyBallObjectName = "money_ball_enabled";
    private const string displayOtherMessageName = "other_message";

    // text objects
    private Text displayScoreText;
    [SerializeField]
    private Text displayCurrentScoreText;
    [SerializeField]
    private Text displayHighScoreText;
    private Text displayMoneyText;
    private Text displayMoneyBallText;
    private Text displayOtherMessageText;

    private float timeCompleted;

    // all these specific game rules for each will need to moved to a different file eventually on refactor
    [SerializeField] private GameObject[] basketBallShotMarkerObjects;
    [SerializeField] private List<BasketBallShotMarker> _basketBallShotMarkersList;

    [SerializeField]
    private int markersRemaining;
    private bool positionMarkersRequired;
    public bool PositionMarkersRequired => positionMarkersRequired;

    private float counterTime; // this is set when shot is made that ends game : class BasketBallShotMade (attached to rim)

    public static GameRules instance;

    [SerializeField]
    float timePlayedStart;
    [SerializeField]
    float timePlayedEnd;
    int inThePocketActivateValue;

    private void Awake()
    {
        instance = this;
        timePlayedStart = Time.time;
        inThePocketActivateValue = 3;
    }

    private void Start()
    {
        gameOver = false;
        gameModeId = GameOptions.gameModeSelectedId;
        timer = GameObject.Find("timer").GetComponent<Timer>();

        // components
        basketBallStats = BasketBall.instance.BasketBallStats;

        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();
        displayHighScoreText = GameObject.Find(displayHighScoreObjectName).GetComponent<Text>();
        displayMoneyText = GameObject.Find(displayMoneyObjectName).GetComponent<Text>();
        displayMoneyBallText = GameObject.Find(displayMoneyBallObjectName).GetComponent<Text>();
        displayOtherMessageText = GameObject.Find(displayOtherMessageName).GetComponent<Text>();

        // rules flags
        modeRequiresCounter = GameOptions.gameModeRequiresCounter;
        modeRequiresCountDown = GameOptions.gameModeRequiresCountDown;

        gameModeRequiresShotMarkers3s = GameOptions.gameModeRequiresShotMarkers3s;
        gameModeRequiresShotMarkers4s = GameOptions.gameModeRequiresShotMarkers4s;
        gameModeRequiresMoneyBall = GameOptions.gameModeRequiresMoneyBall;

        gameModeThreePointContest = GameOptions.gameModeThreePointContest;
        gameModeFourPointContest = GameOptions.gameModeFourPointContest;
        gameModeAllPointContest = GameOptions.gameModeAllPointContest;

        if (GameOptions.customTimer > 0)
        {
            setTimer(GameOptions.customTimer);
        }
        else
        {
            setTimer(180);
        }

        GameModeRequiresConsecutiveShots = GameOptions.gameModeRequiresConsecutiveShot;

        // init text
        displayScoreText.text = "";
        displayCurrentScoreText.text = "";
        displayHighScoreText.text = "";
        displayMoneyText.text = "";
        displayMoneyBallText.text = "";
        displayOtherMessageText.text = "";

        // init markers
        gameRulesEnabled = true;

        // enable/disable necessary shot markers for game mode
        if (gameModeRequiresShotMarkers3s || gameModeRequiresShotMarkers4s)
        {
            positionMarkersRequired = true;
            setPositionMarkers();
        }
    }
    // ================================================ Update ============================================
    void Update()
    {
        // update current score
        if (gameRulesEnabled)
        {
            setScoreDisplayText();
        }

        // if game over, empty text display
        if (gameOver && gameRulesEnabled)
        {
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";
            displayMoneyText.text = "";
            displayMoneyBallText.text = "";
            displayOtherMessageText.text = "";
        }

        // game over. pause / display end game / save
        if ( (gameOver || GameLevelManager.instance.PlayerHealth.IsDead) && !Pause.instance.Paused && gameRulesEnabled )
        {
            displayCurrentScoreText.text = "";
            displayHighScoreText.text = "";
            displayMoneyText.text = "";
            displayMoneyBallText.text = "";
            displayOtherMessageText.text = "";

            // set end time for time played, store in basketballstats.timeplayed
            setTimePlayed();

            //pause on game over
            Pause.instance.TogglePause();
            displayScoreText.text = getDisplayText(GameModeId);

            //save if at leat 1 minte played
            if (GameObject.FindGameObjectWithTag("database") != null)//&& basketBallStats.TimePlayed > 60)
            {
                // dont save free play game score
                if (gameModeId != 99)
                {
                    DBConnector.instance.savePlayerGameStats(BasketBall.instance.BasketBallStats);
                }

                DBConnector.instance.savePlayerAllTimeStats(BasketBall.instance.BasketBallStats);
                DBConnector.instance.savePlayerProfileProgression(BasketBall.instance.BasketBallStats.getExperienceGainedFromSession());

                //// check if achievements reached, send bball stats object
                //AchievementManager.instance.checkAllAchievements(GameOptions.playerId, GameOptions.cheerleaderId,
                //    GameOptions.levelId, GameOptions.gameModeSelectedId, basketBallStats.TotalPoints);
            }
            if (GameOptions.enemiesEnabled)
            {
                AnaylticsManager.PointsScoredEnemiesEnabled(basketBallStats);
            }
            else
            {
                AnaylticsManager.PointsScoredEnemiesDisabled(basketBallStats);
            }

            // alert game manager. trigger
            GameLevelManager.instance.GameOver = true;
        }

        // enable moneyball if game requires moneyball
        if (GameLevelManager.instance.Controls.Player.action.triggered && GameModeRequiresMoneyBall)
        {
            toggleMoneyBall();
        }

        // if not enough money and moneyball required, disabled by default
        if (GameModeRequiresMoneyBall && PlayerStats.instance.Money < 5)
        {
            moneyBallEnabled = false;
            //displayMoneyBallText.text = "";
        }
        if (!moneyBallEnabled)
        {
            displayMoneyBallText.text = "";
        }
    }

    public void setTimePlayed()
    {
        // time played end
        timePlayedEnd = Time.time;
        basketBallStats.TimePlayed = timePlayedEnd - timePlayedStart;
    }

    //===================================================== toggle money ball ====================================================

    private void toggleMoneyBall()
    {
        if (PlayerStats.instance.Money >= 5 && !moneyBallEnabled)
        {
            moneyBallEnabled = true;
            if (moneyBallEnabled)
            {
                displayMoneyBallText.text = "Money Ball enabled";
            }
        }
        else
        {
            moneyBallEnabled = false;
            displayMoneyBallText.text = "";
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
        // sort markers list by positionid
        BasketBallShotMarkersList.Sort(sortByMarkerId);
        // number of markers to complete ( all active and enabled sshot markers based on game options
        markersRemaining = BasketBallShotMarkersList.Count;
    }

    static int sortByMarkerId(BasketBallShotMarker p1, BasketBallShotMarker p2)
    {
        return p1.PositionMarkerId.CompareTo(p2.PositionMarkerId);
    }

    private void setTimedPointContestPositionMarkers()
    {
        //// get all shot position marker objects
        //basketBallShotMarkerObjects = GameObject.FindGameObjectsWithTag("shot_marker");

        //gameModeRequiresShotMarkers3s = GameOptions.gameModeRequiresShotMarkers3s;
        //gameModeRequiresShotMarkers4s = GameOptions.gameModeRequiresShotMarkers4s;

        ////load them into list
        //foreach (var marker in basketBallShotMarkerObjects)
        //{
        //    BasketBallShotMarker temp = marker.GetComponent<BasketBallShotMarker>();
        //    // if 3 markers not required, disable them
        //    if (!gameModeRequiresShotMarkers3s && temp.ShotTypeThree)
        //    {
        //        marker.SetActive(false);
        //    }
        //    // if 4 markers not required, disable them
        //    if (!gameModeRequiresShotMarkers4s && temp.ShotTypeFour)
        //    {
        //        marker.SetActive(false);
        //    }
        //    // add all active and enabled markers to list
        //    if (temp.isActiveAndEnabled)
        //    {
        //        BasketBallShotMarkersList.Add(temp);
        //        temp.PositionMarkerId = BasketBallShotMarkersList.Count - 1;
        //    }
        //}
        //// number of markers to complete ( all active and enabled sshot markers based on game options
        //markersRemaining = BasketBallShotMarkersList.Count;
    }

    // ================================================ set score display ============================================
    private void setScoreDisplayText()
    {
        if (PlayerData.instance != null)
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
            //if (gameModeId == 5)
            //{
            //    displayCurrentScoreText.text = "longest shot : " + (BasketBall.instance.BasketBallStats.LongestShotMade).ToString("0.00")
            //        + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim).ToString("00.00");
            //    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMade.ToString("0.00");
            //}
            if (gameModeId == 6)
            {
                displayCurrentScoreText.text = "total distance : " + (BasketBall.instance.BasketBallStats.TotalDistance).ToString("0.00")
                + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalDistance.ToString("0.00");
            }
            if (gameModeId == 7)
            {
                displayCurrentScoreText.text = "";
                //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.MakeThreePointersLowTime;
                //displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 8)
            {
                displayCurrentScoreText.text = "";
                //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
                //displayHighScoreText.text = "high score : " + PlayerData.instance.TotalDistance.ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.MakeFourPointersLowTime;
                //displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 9)
            {
                displayCurrentScoreText.text = "";
                //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.MakeAllPointersLowTime;
                //displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 10)
            {
                displayCurrentScoreText.text = "";
                //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.MakeThreePointersMoneyBallLowTime;
                displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 11)
            {
                displayCurrentScoreText.text = "";
                //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
                //displayHighScoreText.text = "high score : " + PlayerData.instance.TotalDistance.ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.MakeFourPointersMoneyBallLowTime;
                displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 12)
            {
                displayCurrentScoreText.text = "";
                //                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.BallDistanceFromRim * 6).ToString("0.00");
                displayHighScoreText.text = "high score : " + PlayerData.instance.MakeAllPointersMoneyBallLowTime;
                displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 14)
            {
                displayCurrentScoreText.text = "Consecutive Shots"
                    + "\nCurrent : " + BasketBallShotMade.instance.ConsecutiveShotsMade
                    + "\nHigh Shots : " + BasketBall.instance.BasketBallStats.MostConsecutiveShots;
                displayHighScoreText.text = "high score : " + PlayerData.instance.MostConsecutiveShots;
                //displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 15)
            {
                displayCurrentScoreText.text = "total points : " + BasketBall.instance.BasketBallStats.TotalPoints
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType
                    + "\nCurrent Consecutive: " + BasketBallShotMade.instance.ConsecutiveShotsMade;
                // in the pocket is active, display text notifier
                if (BasketBallShotMade.instance.ConsecutiveShotsMade >= inThePocketActivateValue)
                {
                    displayOtherMessageText.text = "In The Pocket";
                }
                // in the pocket not active, no notifier
                else
                {
                    displayOtherMessageText.text = "";
                }
                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPointsBonus;
            }
            if (gameModeId == 16)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.ThreePointContestScore;
            }
            if (gameModeId == 17)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointContestScore;
            }
            if (gameModeId == 18)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.AllPointContestScore;
            }
            if (gameModeId == 19)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPointsByDistance;
                displayCurrentScoreText.text =
                    "current distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("00.00")
                    + "\nlast shot : " + Mathf.FloorToInt((BasketBall.instance.LastShotDistance * 6) / 10)
                    + "\ntotal points : " + BasketBall.instance.BasketBallStats.TotalPoints;
            }
            if (gameModeId == 20)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.EnemiesKilled;
                displayCurrentScoreText.text =
                    "nerds bashed : " + (BasketBall.instance.BasketBallStats.EnemiesKilled);
            }
            if (gameModeId == 0 || gameModeId == 99 || gameModeId == 98 )
            {
                displayCurrentScoreText.text = "longest shot : " + (basketBallStats.LongestShotMade).ToString("0.00")
                                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("00.00");
                if (GameOptions.gameModeSelectedName.ToLower().Contains("free"))
                {
                    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00")
                        + "\nexp gained : " + basketBallStats.getExperienceGainedFromSession();
                }
                else
                {
                    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00");
                }
                // if longest shot > saved longest shot
                if ((BasketBall.instance.BasketBallStats.LongestShotMade) > PlayerData.instance.LongestShotMadeFreePlay)
                {
                    //PlayerData.instance.saveStats();
                    PlayerData.instance.LongestShotMadeFreePlay = basketBallStats.LongestShotMade;
                    // save to db
                    DBHelper.instance.updateFloatValueByTableAndField("AllTimeStats", "longestShot", PlayerData.instance.LongestShotMadeFreePlay);
                }
            }
            
        }
    }

    // ================================================ get end game display text ============================================
    private string getDisplayText(int modeId)
    {
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
            displayText = "Your longest shot made was " + (basketBallStats.LongestShotMade).ToString("0.00") + " ft.\n\n" + getStatsTotals();
        }
        if (gameModeId == 6)
        {
            displayText = "Your total distance for shots made was " + (basketBallStats.TotalDistance).ToString("0.00") + " ft.\n\n" + getStatsTotals();
        }
        if (gameModeId >= 6 && gameModeId <= 12)
        {
            int minutes = Mathf.FloorToInt(counterTime / 60);
            float seconds = (counterTime - (minutes * 60));
            //displayText = "Your time to complete all shots was " + (counterTime).ToString("0.000") + "\n\n" + getStatsTotals();
            displayText = "Your time was " + minutes.ToString("0") + ":" + seconds.ToString("00.000") + "\n\n" + getStatsTotals();
        }
        if (gameModeId == 14)
        {
            displayText = "Your most consecutive shots was " + basketBallStats.MostConsecutiveShots + "\n\n" + getStatsTotals();
        }
        //if (gameModeId == 15)
        //{
        //    displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        //}
        if (gameModeId == 15 || gameModeId == 16 || gameModeId == 17 || gameModeId == 18 || gameModeId == 19)
        {
            displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        }
        if (gameModeId == 20)
        {
            displayText = "You Bashed up " + basketBallStats.EnemiesKilled + " nerds"
                + "\n\nexperience gained : " + basketBallStats.getExperienceGainedFromSession(); ;
        }
        if (gameModeId == 98 )
        {
            displayText = "Arcade mode\n\n" + getStatsTotals();
        }
        if (gameModeId == 99 || gameModeId == 0)
        {
            displayText = "Free Play mode\n\n" + getStatsTotals();
        }

        return displayText;
    }
    // ================================================ get stats total ============================================

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
                         + "longest shot distance : " + (Math.Round(basketBallStats.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                         + "total shots made distance : " + (Math.Round(basketBallStats.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                         + "most consecutive shots : " + basketBallStats.MostConsecutiveShots + "\n"
                         + "experience gained : " + basketBallStats.getExperienceGainedFromSession();
        return scoreText;
    }

    public bool isGameOver()
    {
        //Debug.Log("isGameOver()");
        // if all shot markers are cleared
        if (MarkersRemaining <= 0)
        {
            //set counter timer
            counterTime = Timer.instance.CurrentTime;
            // add remaining counter time FLOOR to total points  as bonus points
            BasketBall.instance.BasketBallStats.TotalPoints += (int)(Mathf.Floor(Timer.instance.Seconds));

            // if game has a time counter
            if (modeRequiresCounter)
            {
                // set timer score
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
        //*NOTE, these if statements could be replaced based on game mode ids but that would be hard coded
        // so unfortunately this is probably the best way to do it

        // mode 7
        if (gameModeRequiresShotMarkers3s && !gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            basketBallStats.MakeThreePointersLowTime = counterTime;
        }
        // mode 8
        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            basketBallStats.MakeFourPointersLowTime = counterTime;
        }
        // mode 9
        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            basketBallStats.MakeAllPointersLowTime = counterTime;
        }

        // mode 10
        if (gameModeRequiresShotMarkers3s && !gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            basketBallStats.MakeThreePointersMoneyBallLowTime = counterTime;
        }
        // mode 11
        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            basketBallStats.MakeFourPointersMoneyBallLowTime = counterTime;
        }
        // mode 12
        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            basketBallStats.MakeAllPointersMoneyBallLowTime = counterTime;
        }
    }

    public bool GameModeRequiresMoneyBall => gameModeRequiresMoneyBall;

    public bool MoneyBallEnabled
    {
        get => moneyBallEnabled;
        set => moneyBallEnabled = value;
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

    // TODO: used to allow pause toggle. never set to false. still works somehow. 
    // this needs a deeper look when i get time 
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
    public bool GameModeRequiresConsecutiveShots { get => gameModeRequiresConsecutiveShots; set => gameModeRequiresConsecutiveShots = value; }
    public bool GameModeThreePointContest { get => gameModeThreePointContest; }
    public bool GameModeFourPointContest { get => gameModeFourPointContest; }
    public bool GameModeAllPointContest { get => gameModeAllPointContest; }
    public int InThePocketActivateValue { get => inThePocketActivateValue; set => inThePocketActivateValue = value; }
    public Text DisplayCurrentScoreText { get => displayCurrentScoreText; set => displayCurrentScoreText = value; }
    public Text DisplayHighScoreText { get => displayHighScoreText; set => displayHighScoreText = value; }
}
