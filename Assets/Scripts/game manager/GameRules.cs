using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameRules : MonoBehaviour
{
    [SerializeField]
    private int gameModeId;
    //private float timerStart;

    private bool gameOver;
    //private bool gameStart;
    [SerializeField]
    private bool gameRulesEnabled;
    private bool modeRequiresCounter;
    private bool modeRequiresCountDown;

    bool gameModeRequiresShotMarkers3s;
    bool gameModeRequiresShotMarkers4s;
    bool gameModeRequiresShotMarkers7s;

    [SerializeField]
    bool gameModeThreePointContest;
    [SerializeField]
    bool gameModeFourPointContest;
    [SerializeField]
    private bool gameModeSevenPointContest;
    bool gameModeAllPointContest;
    [SerializeField]
    float customTimer;

    bool gameModeRequiresMoneyBall;
    bool moneyBallEnabled;
    bool gameModeRequiresConsecutiveShots;

    private Timer timer;
    private GameStats gameStats1;

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
    [SerializeField]
    private Text displayP1ScoreText;
    [SerializeField]
    private Text displayP2ScoreText;
    [SerializeField]
    private Text displayP3ScoreText;
    [SerializeField]
    private Text displayP4ScoreText;

    public string player1DisplayName;
    public string player2DisplayName;
    public string player3DisplayName;
    public string player4DisplayName;

    //private float timeCompleted;

    // all these specific game rules for each will need to moved to a different file eventually on refactor
    [SerializeField] private GameObject[] basketBallShotMarkerObjects;
    [SerializeField] private List<BasketBallShotMarker> _basketBallShotMarkersList;

    [SerializeField]
    private int markersRemaining;
    [SerializeField]
    private bool positionMarkersRequired;
    public bool PositionMarkersRequired => positionMarkersRequired;

    private float counterTime; // this is set when shot is made that ends game : class BasketBallShotMade (attached to rim)

    public static GameRules instance;

    [SerializeField]
    float timePlayedStart;
    [SerializeField]
    float timePlayedEnd;
    [SerializeField]
    int inThePocketActivateValue;

    [SerializeField]
    private GameObject _rakesClone;

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
        inThePocketActivateValue = 0;
    }

    private void Start()
    {
        gameOver = false;
        gameModeId = GameOptions.gameModeSelectedId;

        // components
        // player 1 game stats
        gameStats1 = GameLevelManager.instance.Player1.gameStats;

        displayScoreText = GameObject.Find(displayScoreObjectName).GetComponent<Text>();
        displayCurrentScoreText = GameObject.Find(displayCurrentScoreObjectName).GetComponent<Text>();
        displayHighScoreText = GameObject.Find(displayHighScoreObjectName).GetComponent<Text>();
        displayMoneyText = GameObject.Find(displayMoneyObjectName).GetComponent<Text>();
        displayMoneyBallText = GameObject.Find(displayMoneyBallObjectName).GetComponent<Text>();
        displayOtherMessageText = GameObject.Find(displayOtherMessageName).GetComponent<Text>();
        timer = GameObject.Find("timer").GetComponent<Timer>();

        player1DisplayName = GameLevelManager.instance.Player1 != null ? GameLevelManager.instance.players[0].characterProfile.PlayerDisplayName : "player1";
        player2DisplayName = GameLevelManager.instance.Player2 != null ? GameLevelManager.instance.players[1].characterProfile.PlayerDisplayName : "player2";
        player3DisplayName = GameLevelManager.instance.Player3 != null ? GameLevelManager.instance.players[2].characterProfile.PlayerDisplayName : "player3";
        player4DisplayName = GameLevelManager.instance.Player4 != null ? GameLevelManager.instance.players[3].characterProfile.PlayerDisplayName : "player4";

        //updatePlayerScore();

        // rules flags
        modeRequiresCounter = GameOptions.gameModeRequiresCounter;
        modeRequiresCountDown = GameOptions.gameModeRequiresCountDown;

        gameModeRequiresShotMarkers3s = GameOptions.gameModeRequiresShotMarkers3s;
        gameModeRequiresShotMarkers4s = GameOptions.gameModeRequiresShotMarkers4s;
        gameModeRequiresShotMarkers7s = GameOptions.gameModeRequiresShotMarkers7s;
        gameModeRequiresMoneyBall = GameOptions.gameModeRequiresMoneyBall;

        gameModeThreePointContest = GameOptions.gameModeThreePointContest;
        gameModeFourPointContest = GameOptions.gameModeFourPointContest;
        gameModeSevenPointContest = GameOptions.gameModeSevenPointContest;
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
        displayP1ScoreText.text = "";
        displayP2ScoreText.text = "";
        displayP3ScoreText.text = "";
        displayP4ScoreText.text = "";

        // init markers
        gameRulesEnabled = true;

        // enable/disable necessary shot markers for game mode
        if (gameModeRequiresShotMarkers3s || gameModeRequiresShotMarkers4s || gameModeRequiresShotMarkers7s)
        {
            positionMarkersRequired = true;
            SetPositionMarkers();
        }
        if (GameOptions.obstaclesEnabled)
        {
            Vector3 vector = new Vector3(GameLevelManager.instance.BasketballRimVector.x,
                GameLevelManager.instance.TerrainHeight,
                GameLevelManager.instance.BasketballRimVector.z);
            Instantiate(_rakesClone, vector, Quaternion.identity);
        }
    }

    // ================================================ Update ============================================
    void Update()
    {
        //// update current score
        if (gameRulesEnabled && GameOptions.numPlayers >= 1)
        {
            SetScoreDisplayText();
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
            displayScoreText.text = GetDisplayText(GameModeId);

            List<PlayerIdentifier> gameStatsList = new();
            if (GameOptions.gameModeSelectedId == 23)
            {
                gameStatsList = GameLevelManager.instance.getSortedGameStatsList();
            }
            else
            {
                gameStatsList = GameLevelManager.instance.players;
            }

            // ******** important : convert basketball stats to high score model
            HighScoreModel dBHighScoreModel = new();
            HighScoreModel user = dBHighScoreModel.convertBasketBallStatsToModel(gameStatsList);
            //user = dBHighScoreModel.convertBasketBallStatsToModel(gameStats);
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
                DBConnector.instance.savePlayerAllTimeStats(GameLevelManager.instance.Player1.gameStats);
                DBConnector.instance.savePlayerProfileProgression(GameLevelManager.instance.Player1.gameStats.getExperienceGainedFromSession());

                // post to API
            }
            // alert game manager. trigger
            GameLevelManager.instance.GameOver = true;
        }

        //// enable moneyball if game requires moneyball
        //if (GameLevelManager.instance.Controls.Player.action.triggered && GameModeRequiresMoneyBall)
        //{
        //    ToggleMoneyBall();
        //}

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
        // if player is killed in a game mode that requires a counter
        // if player is killed, high score is being set as time killed
        // must complete game mode to get high score
        gameStats1.TimePlayed = timePlayedEnd - timePlayedStart;
        //if (GameOptions.gameModeRequiresPlayerSurvive
        //    && GameLevelManager.instance.PlayerHealth.IsDead)
        //{
        //    gameStats1.TimePlayed = 0;
        //}
        //else
        //{
        //    gameStats1.TimePlayed = timePlayedEnd - timePlayedStart;
        //}
    }

    //===================================================== toggle money ball ====================================================

    public void updatePlayerScore()
    {
        List<PlayerIdentifier> players = GameLevelManager.instance.getSortedGameStatsList();
        Timer.instance.ScoreClockText.text = players[0].gameStats.TotalPoints.ToString();
        string playerType;
        if (GameOptions.numPlayers > 0 && players[0] != null)
        {
            playerType = players[0].isCpu ? "CPU" : "Player";
            if (!players[0].isCpu) { displayP1ScoreText.color = Color.red; } else { displayP1ScoreText.color = Color.white; }
            displayP1ScoreText.text = playerType + " " + (players[0].pid + 1)
                + "\n" + players[0].characterProfile.PlayerDisplayName
                + "\n" + "points : " + players[0].gameStats.TotalPoints
                + "\n" + players[0].gameStats.ShotMade + "/" + players[0].gameStats.ShotAttempt
                + " " + players[0].gameStats.getTotalPointAccuracy().ToString("0.00") + "%";
        }
        if (GameOptions.numPlayers > 1 && players[1] != null)
        {
            playerType = players[1].isCpu ? "CPU" : "Player";
            if (!players[1].isCpu) { displayP2ScoreText.color = Color.red; } else { displayP2ScoreText.color = Color.white; }
            displayP2ScoreText.text = playerType + " " + (players[1].pid + 1)
                + "\n" + players[1].characterProfile.PlayerDisplayName
                + "\n" + "points : " + players[1].gameStats.TotalPoints
                + "\n" + players[1].gameStats.ShotMade + "/" + players[1].gameStats.ShotAttempt
                + " " + players[1].gameStats.getTotalPointAccuracy().ToString("0.00") + "%";
        }
        else
        {
            displayP2ScoreText.gameObject.SetActive(false);
        }
        if (GameOptions.numPlayers > 2 && players[2] != null)
        {
            playerType = players[2].isCpu ? "CPU" : "Player";
            if (!players[2].isCpu) { displayP3ScoreText.color = Color.red; } else { displayP3ScoreText.color = Color.white; }
            displayP3ScoreText.text = playerType + " " + (players[2].pid + 1)
                + "\n" + players[2].characterProfile.PlayerDisplayName
                + "\n" + "points : " + players[2].gameStats.TotalPoints
                + "\n" + players[2].gameStats.ShotMade + "/" + players[2].gameStats.ShotAttempt
                + " " + players[2].gameStats.getTotalPointAccuracy().ToString("0.00") + "%";
        }
        else
        {
            displayP3ScoreText.gameObject.SetActive(false);
        }
        if (GameOptions.numPlayers > 3 && players[3] != null)
        {
            playerType = players[3].isCpu ? "CPU" : "Player";
            if (!players[3].isCpu) { displayP4ScoreText.color = Color.red; } else { displayP4ScoreText.color = Color.white; }
            displayP4ScoreText.text = playerType + " " + (players[3].pid + 1)
                + "\n" + players[3].characterProfile.PlayerDisplayName
                + "\n" + "points : " + players[3].gameStats.TotalPoints
                + "\n" + players[3].gameStats.ShotMade + "/" + players[3].gameStats.ShotAttempt
                + " " + players[3].gameStats.getTotalPointAccuracy().ToString("0.00") + "%";
        }
        else
        {
            displayP4ScoreText.gameObject.SetActive(false);
        }
    }

    private void ToggleMoneyBall()
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
    private void SetPositionMarkers()
    {
        // get all shot position marker objects
        basketBallShotMarkerObjects = GameObject.FindGameObjectsWithTag("shot_marker");

        gameModeRequiresShotMarkers3s = GameOptions.gameModeRequiresShotMarkers3s;
        gameModeRequiresShotMarkers4s = GameOptions.gameModeRequiresShotMarkers4s;
        gameModeRequiresShotMarkers7s = GameOptions.gameModeRequiresShotMarkers7s;

        //load them into list
        foreach (var marker in basketBallShotMarkerObjects)
        {
            BasketBallShotMarker temp = marker.GetComponent<BasketBallShotMarker>();
            // if 0 markers not required, disable them
            if (!gameModeRequiresShotMarkers3s && temp.ShotTypeThree && GameOptions.numPlayers >= 1)
            {
                marker.SetActive(false);
            }
            // if 4 markers not required, disable them
            if (!gameModeRequiresShotMarkers4s && temp.ShotTypeFour && GameOptions.numPlayers >= 1)
            {
                marker.SetActive(false);
            }
            // if 4 markers not required, disable them
            if (!gameModeRequiresShotMarkers7s && temp.ShotTypeSeven && GameOptions.numPlayers >= 1)
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
        BasketBallShotMarkersList.Sort(SortByMarkerId);
        // number of markers to complete ( all active and enabled sshot markers based on game options
        markersRemaining = BasketBallShotMarkersList.Count;
    }

    static int SortByMarkerId(BasketBallShotMarker p1, BasketBallShotMarker p2)
    {
        return p1.PositionMarkerId.CompareTo(p2.PositionMarkerId);
    }

    // ================================================ set score display ============================================
    public void SetScoreDisplayText()
    {
        GameStats gameStats = GameLevelManager.instance.Player1.basketball.GetComponent<GameStats>();
        if (PlayerData.instance != null)
        {
            if (gameModeId == 1)
            {
                displayCurrentScoreText.text = "total points : " + gameStats.TotalPoints
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPoints;
            }
            if (gameModeId == 2)
            {
                displayCurrentScoreText.text = "0s made : " + gameStats.ThreePointerMade
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = gameStats.ThreePointerMade.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.ThreePointerMade;
            }
            if (gameModeId == 0)
            {
                displayCurrentScoreText.text = "4s made : " + gameStats.FourPointerMade
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = gameStats.FourPointerMade.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointerMade;
            }
            if (gameModeId == 4)
            {
                displayCurrentScoreText.text = "7s made : " + gameStats.SevenPointerMade
                                                            + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType;
                Timer.instance.ScoreClockText.text = gameStats.SevenPointerMade.ToString();

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
                displayCurrentScoreText.text = "total distance : " + (gameStats.TotalDistance).ToString("0.00")
                + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("0.00");
                Timer.instance.ScoreClockText.text = (gameStats.TotalDistance).ToString("0.00");

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
                    + "\nCurrent : " + GameLevelManager.instance.Player1.gameStats.ConsecutiveShotsMade
                    + "\nHigh Shots : " + gameStats.MostConsecutiveShots;
                Timer.instance.ScoreClockText.text = GameLevelManager.instance.Player1.gameStats.ConsecutiveShotsMade.ToString();

                displayHighScoreText.text = "high score : " + PlayerData.instance.MostConsecutiveShots;
                //displayMoneyText.text = "$" + PlayerStats.instance.Money;
            }
            if (gameModeId == 15)
            {
                displayCurrentScoreText.text = "total points : " + gameStats.TotalPoints
                    + "\ncurrent shot : " + BasketBall.instance.BasketBallState.CurrentShotType
                    + "\nCurrent Consecutive: " + GameLevelManager.instance.players[0].gameStats.ConsecutiveShotsMade;
                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();

                // in the pocket is active, display text notifier
                if (GameLevelManager.instance.Player1.gameStats.ConsecutiveShotsMade >= inThePocketActivateValue)
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
                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();
            }
            if (gameModeId == 17)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.FourPointContestScore;
                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();
            }
            if (gameModeId == 18)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.AllPointContestScore;
                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();
            }
            if (gameModeId == 19)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.TotalPointsByDistance;

                displayCurrentScoreText.text =
                    "current distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("00.00")
                    + "\nlast shot : " + Mathf.FloorToInt((BasketBall.instance.LastShotDistance * 6) / 10)
                    + "\ntotal points : " + gameStats.TotalPoints;

                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();
            }
            if (gameModeId == 20)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.EnemiesKilled;

                displayCurrentScoreText.text =
                    "nerds bashed : " + (gameStats.EnemiesKilled);
                if (Timer.instance.ScoreClockText != null)
                {
                    Timer.instance.ScoreClockText.text = (gameStats.EnemiesKilled).ToString();
                }
            }
            if (gameModeId == 21)
            {
                displayHighScoreText.text = "high score : " + PlayerData.instance.EnemiesKilledBattleRoyal;

                displayCurrentScoreText.text =
                    "nerds bashed : " + (gameStats.EnemiesKilled);
                if (Timer.instance.ScoreClockText != null)
                {
                    Timer.instance.ScoreClockText.text = (gameStats.EnemiesKilled).ToString();
                }
            }
            if (gameModeId == 23)
            {
                Timer.instance.ScoreClockText.text = gameStats.TotalPoints.ToString();
                updatePlayerScore();
            }
            //if (gameModeId == 21)
            //{
            //    displayHighScoreText.text = "high score : " + PlayerData.instance.EnemiesKilled;

            //    displayCurrentScoreText.text =
            //        "nerds bashed : " + (gameStats.EnemiesKilled);
            //    if (Timer.instance.ScoreClockText != null)
            //    {
            //        Timer.instance.ScoreClockText.text = (gameStats.EnemiesKilled).ToString();
            //    }
            //}

            if (gameModeId == 0 || gameModeId == 99 || gameModeId == 98)
            {
                displayCurrentScoreText.text = "longest shot : " + (gameStats1.LongestShotMade).ToString("0.00")
                                                                 + "\ncurrent distance : " + (BasketBall.instance.BasketBallState.PlayerDistanceFromRim * 6).ToString("00.00");
                Timer.instance.ScoreClockText.text = (gameStats1.LongestShotMade).ToString("0.00");

                if (GameOptions.gameModeSelectedName.ToLower().Contains("free"))
                {
                    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00")
                        + "\nexp gained : " + gameStats1.getExperienceGainedFromSession();
                }
                else
                {
                    displayHighScoreText.text = "high score : " + PlayerData.instance.LongestShotMadeFreePlay.ToString("0.00");
                }
                // if longest shot > saved longest shot
                if ((gameStats.LongestShotMade) > PlayerData.instance.LongestShotMadeFreePlay)
                {
                    //PlayerData.instance.saveStats();
                    PlayerData.instance.LongestShotMadeFreePlay = gameStats1.LongestShotMade;
                    // save to db
                    DBHelper.instance.updateFloatValueByTableAndField("AllTimeStats", "longestShot", PlayerData.instance.LongestShotMadeFreePlay);
                }
            }
        }
    }

    // ================================================ get end game display text ============================================
    private string GetDisplayText(int modeId)
    {
        string displayText = "";

        if (gameModeId == 1)
        {
            displayText = "You scored " + gameStats1.TotalPoints + " total points\n\n" + GetStatsTotals();
        }
        if (gameModeId == 2)
        {
            displayText = "You made " + gameStats1.ThreePointerMade + " total 3 pointers\n\n" + GetStatsTotals();
        }
        if (gameModeId == 0)
        {
            displayText = "You made " + gameStats1.FourPointerMade + " total 4 pointers\n\n" + GetStatsTotals();
        }
        if (gameModeId == 4)
        {
            displayText = "You made " + gameStats1.SevenPointerMade + " total 4 pointers\n\n" + GetStatsTotals();
        }
        if (gameModeId == 5)
        {
            displayText = "Your longest shot made was " + (gameStats1.LongestShotMade).ToString("0.00") + " ft.\n\n" + GetStatsTotals();
        }
        if (gameModeId == 6)
        {
            displayText = "Your total distance for shots made was " + (gameStats1.TotalDistance).ToString("0.00") + " ft.\n\n" + GetStatsTotals();
        }
        if (gameModeId > 6 && gameModeId <= 12)
        {
            int minutes = Mathf.FloorToInt(gameStats1.TimePlayed / 60);
            float seconds = (gameStats1.TimePlayed - (minutes * 60));
            //displayText = "Your time was " + (counterTime).ToString("0.000") + "\n\n" + getStatsTotals();
            displayText = "Your time was " + minutes.ToString("0") + ":" + seconds.ToString("00.000") + "\n\n" + GetStatsTotals();
        }
        if (gameModeId == 14)
        {
            displayText = "Your most consecutive shots was " + gameStats1.MostConsecutiveShots + "\n\n" + GetStatsTotals();
        }
        //if (gameModeId == 15)
        //{
        //    displayText = "You scored " + basketBallStats.TotalPoints + " total points\n\n" + getStatsTotals();
        //}
        if (gameModeId == 15 || gameModeId == 16 || gameModeId == 17 || gameModeId == 18 || gameModeId == 19)
        {
            displayText = "You scored " + gameStats1.TotalPoints + " total points\n\n" + GetStatsTotals();
        }
        if (gameModeId == 20)
        {
            displayText = "You Bashed up " + gameStats1.EnemiesKilled + " nerds"
                + "\n\nexperience gained : " + gameStats1.getExperienceGainedFromSession();
        }
        if (gameModeId == 21)
        {
            int minutes = Mathf.FloorToInt(gameStats1.TimePlayed / 60);
            float seconds = (gameStats1.TimePlayed - (minutes * 60));
            displayText = "You Bashed up " + gameStats1.EnemiesKilled + " nerds"
                + "\n\nYou survived for  : " + minutes.ToString("0") + ":" + seconds.ToString("00.000") + "\n\n"
                + "\n\nexperience gained : " + gameStats1.getExperienceGainedFromSession();
        }
        if (gameModeId == 23)
        {
            List<PlayerIdentifier> players = GameLevelManager.instance.getSortedGameStatsList();
            displayText = players[0].characterProfile.PlayerDisplayName + " wins!"
                + "\n---------------------------------"
                + "\n" + players[0].characterProfile.PlayerDisplayName + " : " + players[0].gameStats.TotalPoints;
            if (GameOptions.numPlayers >1)
            {
                displayText += "\n" + players[1].characterProfile.PlayerDisplayName + " : " + players[1].gameStats.TotalPoints;
            }
            if (GameOptions.numPlayers > 2)
            {
                displayText += "\n" + players[2].characterProfile.PlayerDisplayName + " : " + players[2].gameStats.TotalPoints;
            }
            if (GameOptions.numPlayers > 3)
            {
                displayText += "\n" + players[3].characterProfile.PlayerDisplayName + " : " + players[3].gameStats.TotalPoints;
            }
        }
        if (gameModeId == 98)
        {
            displayText = "Arcade mode\n\n" + GetStatsTotals();
        }
        if (gameModeId == 99 || gameModeId == 0)
        {
            displayText = "Free Play mode\n\n" + GetStatsTotals();
        }

        return displayText;
    }
    // ================================================ get stats total ============================================

    string GetStatsTotals()
    {
        string scoreText;
        if ((gameModeAllPointContest || gameModeFourPointContest || gameModeThreePointContest || gameModeSevenPointContest)
            && !GameOptions.sniperEnabled)
        {
            scoreText = "shots  : " + gameStats1.ShotMade + " / " + gameStats1.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                             + "points : " + gameStats1.TotalPoints + "\n"
                             //+ "bonus points : " + gameStats1.BonusPoints + "\n"
                             + "2 pointers : " + gameStats1.TwoPointerMade + " / " + gameStats1.TwoPointerAttempts + "    "
                             + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                             + "3 pointers : " + gameStats1.ThreePointerMade + " / " + gameStats1.ThreePointerAttempts + "    "
                             + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                             + "4 pointers : " + gameStats1.FourPointerMade + " / " + gameStats1.FourPointerAttempts + "    "
                             + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                             + "7 pointers : " + gameStats1.SevenPointerMade + " / " + gameStats1.SevenPointerAttempts + "    "
                             + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                             + "money ball : " + gameStats1.MoneyBallMade + " / " + gameStats1.MoneyBallAttempts + "    "
                             + BasketBall.instance.getAccuracy(gameStats1.MoneyBallMade, gameStats1.MoneyBallAttempts).ToString("00.0") + "%\n"
                             + "longest shot distance : " + (Math.Round(gameStats1.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                             + "total shots made distance : " + (Math.Round(gameStats1.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                             + "most consecutive shots : " + gameStats1.MostConsecutiveShots + "\n"
                             + "experience gained : " + gameStats1.getExperienceGainedFromSession();
        }
        else if (GameOptions.sniperEnabled)
        {
            scoreText = "shots  : " + gameStats1.ShotMade + " / " + gameStats1.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                 + "points : " + gameStats1.TotalPoints + "\n"
                 + "2 pointers : " + gameStats1.TwoPointerMade + " / " + gameStats1.TwoPointerAttempts + "    "
                 + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                 + "3 pointers : " + gameStats1.ThreePointerMade + " / " + gameStats1.ThreePointerAttempts + "    "
                 + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                 + "4 pointers : " + gameStats1.FourPointerMade + " / " + gameStats1.FourPointerAttempts + "    "
                 + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                 + "7 pointers : " + gameStats1.SevenPointerMade + " / " + gameStats1.SevenPointerAttempts + "    "
                 + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                 + "longest shot distance : " + (Math.Round(gameStats1.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                 + "total shots made distance : " + (Math.Round(gameStats1.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                 + "most consecutive shots : " + gameStats1.MostConsecutiveShots + "\n"
                 + "sniper accuracy : " + gameStats1.SniperHits + " / " + gameStats1.SniperShots
                    + " " + UtilityFunctions.getPercentageFloat(gameStats1.SniperHits, gameStats1.SniperShots).ToString("00.0") + "%\n"
                 + "experience gained : " + gameStats1.getExperienceGainedFromSession();
        }
        else
        {
            scoreText = "shots  : " + gameStats1.ShotMade + " / " + gameStats1.ShotAttempt + " " + BasketBall.instance.getTotalPointAccuracy().ToString("0.00") + "%\n"
                 + "points : " + gameStats1.TotalPoints + "\n"
                 + "2 pointers : " + gameStats1.TwoPointerMade + " / " + gameStats1.TwoPointerAttempts + "    "
                 + BasketBall.instance.getTwoPointAccuracy().ToString("00.0") + "%\n"
                 + "3 pointers : " + gameStats1.ThreePointerMade + " / " + gameStats1.ThreePointerAttempts + "    "
                 + BasketBall.instance.getThreePointAccuracy().ToString("00.0") + "%\n"
                 + "4 pointers : " + gameStats1.FourPointerMade + " / " + gameStats1.FourPointerAttempts + "    "
                 + BasketBall.instance.getFourPointAccuracy().ToString("00.0") + "%\n"
                 + "7 pointers : " + gameStats1.SevenPointerMade + " / " + gameStats1.SevenPointerAttempts + "    "
                 + BasketBall.instance.getSevenPointAccuracy().ToString("00.0") + "%\n"
                 + "longest shot distance : " + (Math.Round(gameStats1.LongestShotMade, 2)).ToString("0.00") + " ft.\n"
                 + "total shots made distance : " + (Math.Round(gameStats1.TotalDistance, 2)).ToString("0.00") + " ft.\n"
                 + "most consecutive shots : " + gameStats1.MostConsecutiveShots + "\n"
                 + "experience gained : " + gameStats1.getExperienceGainedFromSession();
        }
        return scoreText;
    }

    public bool IsGameOver()
    {
        // if all shot markers are cleared
        if (MarkersRemaining <= 0)
        {
            ////set counter timer
            //float bonusTime = Timer.instance.Seconds;
            //Debug.Log("Timer.instance.Seconds : " + Timer.instance.Seconds);
            //if (gameModeThreePointContest || gameModeFourPointContest || gameModeAllPointContest)
            //{
            //    //// add remaining counter time FLOOR to total points  as bonus points
            //    GameLevelManager.instance.Player1.gameStats.BonusPoints = (int)(Mathf.Floor(bonusTime/2));
            //    Debug.Log("bonusTime : "+bonusTime);
            //    Debug.Log("(int)(Mathf.Floor(bonusTime) : " + (int)(Mathf.Floor(bonusTime/2)));
            //    // add bonus points
            //    GameLevelManager.instance.Player1.gameStats.TotalPoints += GameLevelManager.instance.players[0].gameStats.BonusPoints;
            //}
            //// if game has a time counter
            //if (modeRequiresCounter)
            //{
            //    // set timer score
            //    SetRequiresCounterLowScore();
            //}
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SetRequiresCounterLowScore()
    {
        //*NOTE, these if statements could be replaced based on game mode ids but that would be hard coded
        // so unfortunately this is probably the best way to do it

        // mode 7
        if (gameModeRequiresShotMarkers3s && !gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            gameStats1.MakeThreePointersLowTime = counterTime;
        }
        // mode 8
        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            gameStats1.MakeFourPointersLowTime = counterTime;
        }
        // mode 9
        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && !GameModeRequiresMoneyBall)
        {
            gameStats1.MakeAllPointersLowTime = counterTime;
        }
        // mode 10
        if (gameModeRequiresShotMarkers3s && !gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            gameStats1.MakeThreePointersMoneyBallLowTime = counterTime;
        }
        // mode 11
        if (!gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            gameStats1.MakeFourPointersMoneyBallLowTime = counterTime;
        }
        // mode 12
        if (gameModeRequiresShotMarkers3s && gameModeRequiresShotMarkers4s && GameModeRequiresMoneyBall)
        {
            gameStats1.MakeAllPointersMoneyBallLowTime = counterTime;
        }
    }

    public bool GameModeRequiresMoneyBall => gameModeRequiresMoneyBall;

    public bool MoneyBallEnabled
    {
        get => moneyBallEnabled;
        set => moneyBallEnabled = value;
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
    public bool GameModeSevenPointContest { get => gameModeSevenPointContest; set => gameModeSevenPointContest = value; }
}
