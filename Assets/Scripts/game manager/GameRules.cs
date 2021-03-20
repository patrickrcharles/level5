using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRules : MonoBehaviour
{

    //private int playerId;
    //private int levelId;
    private int gameModeId;
    //private float timerStart;

    private bool gameOver;
    //private bool gameStart;
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
    private GameStats gameStats;

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

    //private float timeCompleted;

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
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        timePlayedStart = Time.time;
        inThePocketActivateValue = 3;
    }

    private void Start()
    {
        gameOver = false;
        gameModeId = GameOptions.gameModeSelectedId;

        // components
        gameStats = BasketBall.instance.GameStats;

        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();
        displayHighScoreText = GameObject.Find(displayHighScoreObjectName).GetComponent<Text>();
        displayMoneyText = GameObject.Find(displayMoneyObjectName).GetComponent<Text>();
        displayMoneyBallText = GameObject.Find(displayMoneyBallObjectName).GetComponent<Text>();
        displayOtherMessageText = GameObject.Find(displayOtherMessageName).GetComponent<Text>();
        timer = GameObject.Find("timer").GetComponent<Timer>();

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
        //// update current score
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
        if ((gameOver || GameLevelManager.instance.PlayerHealth.IsDead) && !Pause.instance.Paused && gameRulesEnabled)
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

            // ******** important : convert basketball stats to high score model
            HighScoreModel dBHighScoreModel = new HighScoreModel();
            HighScoreModel user = new HighScoreModel();
            user = dBHighScoreModel.convertBasketBallStatsToModel(gameStats);

            //save if at leat 1 minte played
            if (GameObject.FindGameObjectWithTag("database") != null)//&& basketBallStats.TimePlayed > 60)
            {
                // dont save free play game score
                if (gameModeId != 99)
                {
                    DBConnector.instance.savePlayerGameStats(user);
                    // if username is logged in
                    if (!string.IsNullOrEmpty(GameOptions.userName) && GameOptions.userid != 0)
                    {
                        StartCoroutine(APIHelper.PostHighscore(user));
                    }
                    // if user not logged in, set submitted score to false
                    else
                    {
                        DBHelper.instance.setGameScoreSubmitted(user.Scoreid, false);
                    }
                }

                DBConnector.instance.savePlayerAllTimeStats(BasketBall.instance.GameStats);
                DBConnector.instance.savePlayerProfileProgression(BasketBall.instance.GameStats.getExperienceGainedFromSession());

                // post to API
            }
            if (GameOptions.enemiesEnabled)
            {
                AnaylticsManager.PointsScoredEnemiesEnabled(gameStats);
            }
            else
            {
                AnaylticsManager.PointsScoredEnemiesDisabled(gameStats);
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
        gameStats.TimePlayed = timePlayedEnd - timePlayedStart;
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

    // ================================================ set score display ============================================
    public void setScoreDisplayText()
    {
        if (PlayerData.instance != null)
        {
            if (gameModeId == 1)
            {
                displayCurrentScoreText.text = "total points : " + BasketBall.instance.GameStats.TotalPoints
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.TotalPoints.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPoints;
            }
            if (gameModeId == 2)
            {
                displayCurrentScoreText.text = "3s made : " + BasketBall.instance.GameStats.ThreePointerMade
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.ThreePointerMade.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.ThreePointerMade;
            }
            if (gameModeId == 3)
            {
                displayCurrentScoreText.text = "4s made : " + BasketBall.instance.GameStats.FourPointerMade
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.FourPointerMade.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointerMade;
            }
            if (gameModeId == 4)
            {
                displayCurrentScoreText.text = "7s made : " + BasketBall.instance.GameStats.SevenPointerMade
                                                            + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.SevenPointerMade.ToString();

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
                displayCurrentScoreText.text = "total distance : " + (BasketBall.instance.GameStats.TotalDistance).ToString("0.00")
                + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("0.00");
                Timer.instance.ScoreClockText.text = (BasketBall.instance.GameStats.TotalDistance).ToString("0.00");

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
                    + "\nHigh Shots : " + BasketBall.instance.GameStats.MostConsecutiveShots;
                Timer.instance.ScoreClockText.text = BasketBallShotMade.instance.ConsecutiveShotsMade.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.MostConsecutiveShots;
                //displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 15)
            {
                displayCurrentScoreText.text = "total points : " + BasketBall.instance.GameStats.TotalPoints
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType
                    + "\nCurrent Consecutive: " + BasketBallShotMade.instance.ConsecutiveShotsMade;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.TotalPoints.ToString();

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
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.TotalPoints.ToString();
            }
            if (gameModeId == 17)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointContestScore;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.TotalPoints.ToString();
            }
            if (gameModeId == 18)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.AllPointContestScore;
                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.TotalPoints.ToString();
            }
            if (gameModeId == 19)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPointsByDistance;

                displayCurrentScoreText.text =
                    "current distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("00.00")
                    + "\nlast shot : " + Mathf.FloorToInt((BasketBall.instance.LastShotDistance * 6) / 10)
                    + "\ntotal points : " + BasketBall.instance.GameStats.TotalPoints;

                Timer.instance.ScoreClockText.text = BasketBall.instance.GameStats.TotalPoints.ToString();
            }
            if (gameModeId == 20)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.EnemiesKilled;

                displayCurrentScoreText.text =
                    "nerds bashed : " + (BasketBall.instance.GameStats.EnemiesKilled);
                Timer.instance.ScoreClockText.text = (BasketBall.instance.GameStats.EnemiesKilled).ToString();
            }
            if (gameModeId == 0 || gameModeId == 99 || gameModeId == 98)
            {
                displayCurrentScoreText.text = "longest shot : " + (gameStats.LongestShotMade).ToString("0.00")
                                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("00.00");
                Timer.instance.ScoreClockText.text = (gameStats.LongestShotMade).ToString("0.00");

                if (GameOptions.gameModeSelectedName.ToLower().Contains("free"))
                {
                    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00")
                        + "\nexp gained : " + gameStats.getExperienceGainedFromSession();
                }
                else
                {
                    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00");
                }
                // if longest shot > saved longest shot
                if ((BasketBall.instance.GameStats.LongestShotMade) > PlayerData.instance.LongestShotMadeFreePlay)
                {
                    //PlayerData.instance.saveStats();
                    PlayerData.instance.LongestShotMadeFreePlay = gameStats.LongestShotMade;
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
            displayText = "You scored " + gameStats.TotalPoints + " total points\n\n" + getStatsTotals();
        }
        if (gameModeId == 2)
        {
            displayText = "You made " + gameStats.ThreePointerMade + " total 3 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 3)
        {
            displayText = "You made " + gameStats.FourPointerMade + " total 4 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 4)
        {
            displayText = "You made " + gameStats.SevenPointerMade + " total 4 pointers\n\n" + getStatsTotals();
        }
        if (gameModeId == 5)
        {
            displayText = "Your longest shot made was " + (gameStats.LongestShotMade).ToString("0.00") + " ft.\n\n" + getStatsTotals();
        }
        if (gameModeId == 6)
        {
            displayText = "Your total distance for shots made was " + (gameStats.TotalDistance).ToString("0.00") + " ft.\n\n" + getStatsTotals();
        }
        if (gameModeId > 6 && gameModeId <= 12)
        {
            int minutes = Mathf.FloorToInt(gameStats.TimePlayed / 60);
            float seconds = (gameStats.TimePlayed - (minutes * 60));
            //displayText = "Your time was " + (counterTime).ToString("0.000") + "\n\n" + getStatsTotals();
            displayText = "Your time was " + minutes.ToString("0") + ":" + seconds.ToString("00.000") + "\n\n" + getStatsTotals();
        }
        if (gameModeId == 14)
        {
            displayText = "Your most consecutive shots was " + gameStats.MostConsecutiveShots + "\n\n" + getStatsTotals();
        }
        //if (gameModeId == 15)
        //{
        //    displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        //}
        if (gameModeId == 15 || gameModeId == 16 || gameModeId == 17 || gameModeId == 18 || gameModeId == 19)
        {
            displayText = "You scored " + gameStats.TotalPoints + " total points\n\n" + getStatsTotals();
        }
        if (gameModeId == 20)
        {
            displayText = "You Bashed up " + gameStats.EnemiesKilled + " nerds"
                + "\n\nexperience gained : " + gameStats.getExperienceGainedFromSession(); ;
        }
        if (gameModeId == 98)
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
        if ((gameModeAllPointContest || gameModeFourPointContest || gameModeThreePointContest) 
            && !GameOptions.sniperEnabled)
        {
            scoreText = "shots  : " + gameStats.ShotMade + " / " + gameStats.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                             + "points : " + gameStats.TotalPoints + "\n"
                             + "bonus points : " + gameStats.BonusPoints + "\n"
                             + "2 pointers : " + gameStats.TwoPointerMade + " / " + gameStats.TwoPointerAttempts + "    "
                             + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                             + "3 pointers : " + gameStats.ThreePointerMade + " / " + gameStats.ThreePointerAttempts + "    "
                             + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                             + "4 pointers : " + gameStats.FourPointerMade + " / " + gameStats.FourPointerAttempts + "    "
                             + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                             + "7 pointers : " + gameStats.SevenPointerMade + " / " + gameStats.SevenPointerAttempts + "    "
                             + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                             + "money ball : " + gameStats.MoneyBallMade + " / " + gameStats.MoneyBallAttempts + "    "
                             + BasketBall.instance.getAccuracy(gameStats.MoneyBallMade, gameStats.MoneyBallAttempts).ToString("00.0") + "%\n"
                             + "longest shot distance : " + (Math.Round(gameStats.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                             + "total shots made distance : " + (Math.Round(gameStats.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                             + "most consecutive shots : " + gameStats.MostConsecutiveShots + "\n"
                             + "experience gained : " + gameStats.getExperienceGainedFromSession();
        }
        else if (GameOptions.sniperEnabled)
        {
            scoreText = "shots  : " + gameStats.ShotMade + " / " + gameStats.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                 + "points : " + gameStats.TotalPoints + "\n"
                 + "2 pointers : " + gameStats.TwoPointerMade + " / " + gameStats.TwoPointerAttempts + "    "
                 + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                 + "3 pointers : " + gameStats.ThreePointerMade + " / " + gameStats.ThreePointerAttempts + "    "
                 + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                 + "4 pointers : " + gameStats.FourPointerMade + " / " + gameStats.FourPointerAttempts + "    "
                 + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                 + "7 pointers : " + gameStats.SevenPointerMade + " / " + gameStats.SevenPointerAttempts + "    "
                 + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                 + "longest shot distance : " + (Math.Round(gameStats.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                 + "total shots made distance : " + (Math.Round(gameStats.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                 + "most consecutive shots : " + gameStats.MostConsecutiveShots + "\n"
                 + "sniper accuracy : " + gameStats.SniperHits + " / "+ gameStats.SniperShots 
                    + " " + UtilityFunctions.getPercentageFloat(gameStats.SniperHits, gameStats.SniperShots).ToString("00.0") + "%\n"
                 + "experience gained : " + gameStats.getExperienceGainedFromSession();
        }
        else
        {
            scoreText = "shots  : " + gameStats.ShotMade + " / " + gameStats.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                 + "points : " + gameStats.TotalPoints + "\n"
                 + "2 pointers : " + gameStats.TwoPointerMade + " / " + gameStats.TwoPointerAttempts + "    "
                 + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                 + "3 pointers : " + gameStats.ThreePointerMade + " / " + gameStats.ThreePointerAttempts + "    "
                 + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                 + "4 pointers : " + gameStats.FourPointerMade + " / " + gameStats.FourPointerAttempts + "    "
                 + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                 + "7 pointers : " + gameStats.SevenPointerMade + " / " + gameStats.SevenPointerAttempts + "    "
                 + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                 + "longest shot distance : " + (Math.Round(gameStats.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                 + "total shots made distance : " + (Math.Round(gameStats.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                 + "most consecutive shots : " + gameStats.MostConsecutiveShots + "\n"
                 + "experience gained : " + gameStats.getExperienceGainedFromSession();
        }
        return scoreText;
    }

    public bool isGameOver()
    {
        // if all shot markers are cleared
        if (MarkersRemaining <= 0)
        {
            //set counter timer
            float bonusTime = Timer.instance.Seconds * 0.5f;
            if (gameModeThreePointContest || gameModeFourPointContest || gameModeAllPointContest)
            {
                //// add remaining counter time FLOOR to total points  as bonus points
                BasketBall.instance.GameStats.BonusPoints = (int)(Mathf.Floor(bonusTime));
                // add bonus points
                BasketBall.instance.GameStats.TotalPoints += BasketBall.instance.GameStats.BonusPoints;
            }
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
            gameStats.MakeThreePointersLowTime = counterTime;
        }
        // mode 8
        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            gameStats.MakeFourPointersLowTime = counterTime;
        }
        // mode 9
        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            gameStats.MakeAllPointersLowTime = counterTime;
        }
        // mode 10
        if (gameModeRequiresShotMarkers3s && !gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            gameStats.MakeThreePointersMoneyBallLowTime = counterTime;
        }
        // mode 11
        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            gameStats.MakeFourPointersMoneyBallLowTime = counterTime;
        }
        // mode 12
        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            gameStats.MakeAllPointersMoneyBallLowTime = counterTime;
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
    //public Text DisplayCurrentScoreText { get => displayCurrentScoreText; set => displayCurrentScoreText = value; }
    //public Text DisplayHighScoreText { get => displayHighScoreText; set => displayHighScoreText = value; }
    //public bool GameRulesEnabled { get => gameRulesEnabled; set => gameRulesEnabled = value; }
}
