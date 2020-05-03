using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;

public class testSaveLoad : MonoBehaviour
{
    public testSaveLoad instance;
    public BasketBallStats basketBallStatsballStats;

    public int _playerId;
    public string _playerName;

    public float _totalPoints;
    public float _twoPointerMade;
    public float _threePointerMade;

    public float _fourPointerMade;
    public float _twoPointerAttempts;
    public float _threePointerAttempts;
    public float _fourPointerAttempts;

    public float _shotAttempt;
    public float _shotMade;
    public float _longestShotMade;
    public float _totalDistance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        basketBallStatsballStats = GameObject.FindWithTag("basketball").GetComponent<BasketBallStats>();

        //load stats from file
        load(basketBallStatsballStats);
    }
    // Update is called once per frame
    void Update()
    {
        //turn off accuracy modifer 6+9
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha8))
        {
            save(basketBallStatsballStats);
            Debug.Log("save stats");
        }
        //turn off accuracy modifer 6+9
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha9))
        {
            load(basketBallStatsballStats);
            Debug.Log("load stats");
        }
    }

    public void save(BasketBallStats stats)
    {
        // save can be called whenever as long as load isnt called as well.
        // if you call load, you need to reset the local variables
        // example load, totalpoint = 100, make 50, save totalpoint = 150.
        // load = 150, save again totalpoint = 250 (100 + 150)

        //specific to player, finish later. for now, just save any player

        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalPoints",  (int)(_totalPoints + stats.TotalPoints));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalShotMade", (int)(shotMade +stats.ShotMade));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalShotAttempt", (int)(shotAttempt+  stats.ShotAttempt));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_twoPointersMade", (int)(_twoPointerMade+stats.TwoPointerMade));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+ "_threePointersMade", (int)(_threePointerMade+stats.ThreePointerMade));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+ "_fourPointersMade", (int)(_fourPointerMade+stats.FourPointerMade));
        //PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersAttempt", (int)(_twoPointerAttempts+stats.TwoPointerAttempts));
        //PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersAttempt", (int)(stats.ThreePointerAttempts));
        //PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersAttempt", (int)(stats.FourPointerAttempts));

        //if (stats.LongestShotMade > longestShotMade)
        //{
        //    PlayerPrefs.SetFloat(stats.PlayerId + "_" + stats.PlayerName + "_longestShotMade",  stats.LongestShotMade);
        //}

        PlayerPrefs.SetInt("player_"+stats.PlayerName+"_totalPoints",  (int)(_totalPoints + stats.TotalPoints));
        PlayerPrefs.SetInt("player_"+stats.PlayerName+"_totalShotMade", (int)(_shotMade +stats.ShotMade));
        PlayerPrefs.SetInt("player_"+stats.PlayerName+"_totalShotAttempt", (int)(_shotAttempt+  stats.ShotAttempt));
        PlayerPrefs.SetInt("player_"+stats.PlayerName+"_twoPointersMade", (int)(_twoPointerMade+stats.TwoPointerMade));
        PlayerPrefs.SetInt("player_"+stats.PlayerName+ "_threePointersMade", (int)(_threePointerMade+stats.ThreePointerMade));
        PlayerPrefs.SetInt("player_"+stats.PlayerName+ "_fourPointersMade", (int)(_fourPointerMade+stats.FourPointerMade));
        PlayerPrefs.SetInt("player_" + stats.PlayerName + "_twoPointersAttempt", (int)(_twoPointerAttempts+stats.TwoPointerAttempts));
        PlayerPrefs.SetInt("player_" + stats.PlayerName + "_threePointersAttempt", (int)(stats.ThreePointerAttempts));
        PlayerPrefs.SetInt("player_" + stats.PlayerName + "_fourPointersAttempt", (int)(stats.FourPointerAttempts));

        if (stats.LongestShotMade > _longestShotMade)
        {
            PlayerPrefs.SetFloat("player_" + stats.PlayerName + "_longestShotMade",  stats.LongestShotMade);
        }
        PlayerPrefs.SetInt("player_" + stats.PlayerName + "_totalDistance", (int)(stats.TotalDistance));
    }

    public void load(BasketBallStats stats)
    {
        //only call at beginning of a game

        //int temp = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalPoints", (int)stats.TotalPoints);

        //Debug.Log(" load : total points : " + temp);
        //_totalPoints = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalPoints");
        //shotMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalShotMade");
        //shotAttempt = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalShotAttempt");
        //_twoPointerMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersMade");
        //_threePointerMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersMade");
        //_fourPointerMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersMade");
        //_twoPointerAttempts = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersAttempt");
        //_threePointerAttempts = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersAttempt");
        //_fourPointerAttempts = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersAttempt");
        //longestShotMade = PlayerPrefs.GetFloat(stats.PlayerId + "_" + stats.PlayerName + "_longestShotMade");

        _totalPoints = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_totalPoints");
        _shotMade = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_totalShotMade");
        _shotAttempt = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_totalShotAttempt");
        _twoPointerMade = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_twoPointersMade");
        _threePointerMade = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_threePointersMade");
        _fourPointerMade = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_fourPointersMade");
        _twoPointerAttempts = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_twoPointersAttempt");
        _threePointerAttempts = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_threePointersAttempt");
        _fourPointerAttempts = PlayerPrefs.GetInt("player_" + stats.PlayerName + "_fourPointersAttempt");
        _longestShotMade = PlayerPrefs.GetFloat("player_" + stats.PlayerName + "_longestShotMade");
        _totalDistance  = PlayerPrefs.GetFloat("player_" + stats.PlayerName + "_totalDistance", (int)(stats.TotalDistance));
    }

    public void deleteAllSavedData()
    {
        // use wth caution. nuclear option
        PlayerPrefs.DeleteAll();
    }

    public void deleteBasketballSaveData(BasketBallStats stats)
    {
        //specific to player, finish later. for now, just save any player

        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalPoints",  (int)(_totalPoints + stats.TotalPoints));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalShotMade", (int)(shotMade +stats.ShotMade));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalShotAttempt", (int)(shotAttempt+  stats.ShotAttempt));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_twoPointersMade", (int)(_twoPointerMade+stats.TwoPointerMade));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+ "_threePointersMade", (int)(_threePointerMade+stats.ThreePointerMade));
        //PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+ "_fourPointersMade", (int)(_fourPointerMade+stats.FourPointerMade));
        //PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersAttempt", (int)(_twoPointerAttempts+stats.TwoPointerAttempts));
        //PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersAttempt", (int)(stats.ThreePointerAttempts));
        //PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersAttempt", (int)(stats.FourPointerAttempts));

        //if (stats.LongestShotMade > longestShotMade)
        //{
        //    PlayerPrefs.SetFloat(stats.PlayerId + "_" + stats.PlayerName + "_longestShotMade",  stats.LongestShotMade);
        //}

        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_totalPoints");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_totalShotMade");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_totalShotAttempt");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_twoPointersMade");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_threePointersMade");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_fourPointersMade");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_twoPointersAttempt");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_threePointersAttempt");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_fourPointersAttempt");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_longestShotMade");
        PlayerPrefs.DeleteKey("player_" + stats.PlayerName + "_totalDistance");
    }
}
