using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private BasketBallStats basketBallStats;

    private int _playerId;
    private  string _playerName;

    private  float _totalPoints;
    private  float _totalPointsBonus;
    private  float _twoPointerMade;
    private  float _threePointerMade;
    private  float _sevenPointerMade;

    private  float _fourPointerMade;
    private  float _twoPointerAttempts;
    private  float _threePointerAttempts;
    private  float _fourPointerAttempts;
    private  float _sevenPointerAttempts;

    private  float _shotAttempt;
    private  float _shotMade;
    private  float _longestShotMade;
    private  float _longestShotMadeFreePlay;
    private  float _totalDistance;

    private  float _makeThreePointersLowTime;
    private  float _makeFourPointersLowTime;
    private  float _makeAllPointersLowTime;

    private  float _makeThreePointersMoneyBallLowTime;
    private  float _makeFourPointersMoneyBallLowTime;
    private  float _makeAllPointersMoneyBallLowTime;

    private int _mostConsecutiveShots;

    private float _threePointContestScore;
    private float _fourPointContestScore;
    private float _allPointContestScore;

    public static PlayerData instance;
    public List<HitByCar> hitByCars;

    public class HitByCar
    {
        public int vehicleId;
        public int counter = 0;

        public HitByCar(int vehId)
        {
            vehicleId = vehId;
        }
        public HitByCar(int vehId, int count)
        {
            vehicleId = vehId;
            counter = count;
        }
    }

    void Awake()
    {

        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        hitByCars = new List<HitByCar>();
        if (GameObject.Find("database") != null)
        {
            loadStatsFromDatabase();
        }
    }

    public void AddHitByCarInstanceToList(int vehId)
    {
        // if vehicle entry exists, eg, hit by this vehicleId already
        HitByCar temp = hitByCars.Where(x => x.vehicleId == vehId).SingleOrDefault();
        if (temp != null) // if not hit by this car
        {
            // increase counter
            temp.counter++;
            //Debug.Log(GameOptions.playerDisplayName + " hit by : vehicle " + vehId + " on level : " + GameOptions.levelDisplayName 
            //    + " " + temp.counter + " times");
        }
        else // create the vehicle entry
        {
            temp = new HitByCar(vehId);
            hitByCars.Add(temp);
            temp.counter++;
            //Debug.Log(GameOptions.playerDisplayName + " hit by : vehicle" + vehId + " on level : " 
            //    + GameOptions.levelDisplayName + " " + temp.counter + " times");
        } 
    }

    public void loadStatsFromDatabase()
    {
        //Debug.Log("load from database");

        if (DBHelper.instance != null)
        {
            _totalPoints = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 1, "DESC");
            TotalPointsBonus = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 15, "DESC");
            _threePointerMade = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "maxShotMade", 2, "DESC");
            _fourPointerMade = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "maxShotMade", 3, "DESC");
            _sevenPointerMade = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "maxShotMade", 4, "DESC");
            _longestShotMade = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "longestShot", 5, "DESC");
            _totalDistance = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "totalDistance", 6, "DESC");
            _makeThreePointersLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 7, "ASC");
            _makeFourPointersLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 8, "ASC");
            _makeAllPointersLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 9, "ASC");
            _makeThreePointersMoneyBallLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 10, "ASC");
            _makeFourPointersMoneyBallLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 11, "ASC");
            _makeAllPointersMoneyBallLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 12, "ASC");
            LongestShotMadeFreePlay = DBHelper.instance.getFloatValueHighScoreFromTableByField("AllTimeStats", "longestShot", "DESC");
            _mostConsecutiveShots = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "consecutiveShots", 14, "DESC");
            _totalPointsBonus = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 15, "DESC");
            _threePointContestScore = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 16, "DESC");
            _fourPointContestScore = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 17, "DESC");
            _allPointContestScore = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 18, "DESC");
        }
    }

    public void resetCurrentPlayerDataInMemory()
    {
        Debug.Log("resetCurrentPlayerDataInMemory");

        _totalPoints = 0;
        _threePointerMade = 0;
        _fourPointerMade = 0;
        _sevenPointerMade = 0;
        _longestShotMade = 0;
        _totalDistance = 0;
        _makeThreePointersLowTime = 0;
        _makeFourPointersLowTime = 0;
        _makeFourPointersLowTime = 0;
        _makeThreePointersMoneyBallLowTime = 0;
        _makeFourPointersMoneyBallLowTime = 0;
        _makeAllPointersMoneyBallLowTime = 0;
        LongestShotMadeFreePlay = 0;
    }

    public void deleteAllSavedData()
    {
        // use wth caution. nuclear option
        PlayerPrefs.DeleteAll();
    }

    public float SevenPointerAttempts =>_sevenPointerAttempts;
   
    public int PlayerId => _playerId;

    public string PlayerName => _playerName;

    public float TotalPoints => _totalPoints;

    public float TwoPointerMade => _twoPointerMade;

    public float ThreePointerMade => _threePointerMade;

    public float FourPointerMade => _fourPointerMade;

    public float TwoPointerAttempts => _twoPointerAttempts;

    public float ThreePointerAttempts => _threePointerAttempts;

    public float FourPointerAttempts => _fourPointerAttempts;

    public float SevenPointerMade => _sevenPointerMade;

    public float ShotAttempt => _shotAttempt;

    public float ShotMade => _shotMade;

    public float LongestShotMade => _longestShotMade;

    public float TotalDistance => _totalDistance;

    public float MakeThreePointersLowTime => _makeThreePointersLowTime;

    public float MakeFourPointersLowTime => _makeFourPointersLowTime;

    public float MakeAllPointersLowTime => _makeAllPointersLowTime;

    public float MakeThreePointersMoneyBallLowTime => _makeThreePointersMoneyBallLowTime;

    public float MakeFourPointersMoneyBallLowTime => _makeFourPointersMoneyBallLowTime;

    public float MakeAllPointersMoneyBallLowTime => _makeAllPointersMoneyBallLowTime;

    public float LongestShotMadeFreePlay { get => _longestShotMadeFreePlay; set => _longestShotMadeFreePlay = value; }

    public List<HitByCar> HitByCars { get; set; }
    public int MostConsecutiveShots { get => _mostConsecutiveShots; set => _mostConsecutiveShots = value; }
    public float TotalPointsBonus { get => _totalPointsBonus; set => _totalPointsBonus = value; }
    public float ThreePointContestScore { get => _threePointContestScore; set => _threePointContestScore = value; }
    public float FourPointContestScore { get => _fourPointContestScore; set => _fourPointContestScore = value; }
    public float AllPointContestScore { get => _allPointContestScore; set => _allPointContestScore = value; }
}
