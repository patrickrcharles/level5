﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TeamUtility.IO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private BasketBallStats basketBallStats;

    private  int _playerId;
    private  string _playerName;

    private  float _totalPoints;
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

    private  bool _callBallAchievementUnlocked;

    private bool _isCheating; // if cheats enabled, no saving

    //prevent PlayerData from creating multiple objects
    static bool _created = false; 

    public static PlayerData instance;

    void Awake()
    {
        instance = this;
        // only create player data once
        if (!_created)
        {
            DontDestroyOnLoad(this.gameObject);
            _created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
        // load stats wake
        loadStats();
    }

    public void saveStats()
    {

        //Debug.Log("save");

        // get stats object
        //basketBallStats = GameObject.FindWithTag("basketball").GetComponent<BasketBallStats>();
        basketBallStats = GameLevelManager.Instance.Basketball.GetComponent<BasketBallStats>();


        //Debug.Log("money 3s high" + MakeThreePointersMoneyBallLowTime );
        //Debug.Log("money 3s current " + basketBallStats.MakeThreePointersMoneyBallLowTime);


        ////cheating and game mode name doesnt contain 'free' as in, Free Play mode
        //if (IsCheating && !GameOptions.gameModeSelectedName.ToLower().Contains("free"))
        //{
        //    // no save for you
        //    return;
        //}

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

        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_totalPoints",  (int)(_totalPoints + stats.TotalPoints));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_totalShotMade", (int)(_shotMade +stats.ShotMade));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_totalShotAttempt", (int)(_shotAttempt+  stats.ShotAttempt));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_twoPointersMade", (int)(_twoPointerMade+stats.TwoPointerMade));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+ "_threePointersMade", (int)(_threePointerMade+stats.ThreePointerMade));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+ "_fourPointersMade", (int)(_fourPointerMade+stats.FourPointerMade));
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_twoPointersAttempt", (int)(_twoPointerAttempts+stats.TwoPointerAttempts));
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_threePointersAttempt", (int)(stats.ThreePointerAttempts));
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_fourPointersAttempt", (int)(stats.FourPointerAttempts));

        //if (stats.LongestShotMade > _longestShotMade)
        //{
        //    PlayerPrefs.SetFloat("mode_" + stats.PlayerName + "_longestShotMade",  (stats.LongestShotMade * 6));
        //}
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_totalDistance", (int)(stats.TotalDistance));

        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_totalPoints",  (int)(_totalPoints + stats.TotalPoints));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_totalShotMade", (int)(_shotMade +stats.ShotMade));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_totalShotAttempt", (int)(_shotAttempt+  stats.ShotAttempt));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+"_twoPointersMade", (int)(_twoPointerMade+stats.TwoPointerMade));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+ "_threePointersMade", (int)(_threePointerMade+stats.ThreePointerMade));
        //PlayerPrefs.SetInt("mode_"+stats.PlayerName+ "_fourPointersMade", (int)(_fourPointerMade+stats.FourPointerMade));
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_twoPointersAttempt", (int)(_twoPointerAttempts+stats.TwoPointerAttempts));
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_threePointersAttempt", (int)(stats.ThreePointerAttempts));
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_fourPointersAttempt", (int)(stats.FourPointerAttempts));

        //if (stats.LongestShotMade > _longestShotMade)
        //{
        //    PlayerPrefs.SetFloat("mode_" + stats.PlayerName + "_longestShotMade",  (stats.LongestShotMade * 6));
        //}
        //PlayerPrefs.SetInt("mode_" + stats.PlayerName + "_totalDistance", (int)(stats.TotalDistance));

        //Debug.Log(_totalPoints + " : basketBallStats.pioints : "+ basketBallStats.TotalPoints);

        //save game mode 1 high score
        if (_totalPoints < basketBallStats.TotalPoints && GameOptions.gameModeSelected == 1)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_totalPoints", (int) (basketBallStats.TotalPoints));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalPointsPlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalPointsLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalPointsDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalPointsAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _totalPoints = 0;
            loadStats();
        }
        // save mode 2 (3s)
        if (_threePointerMade < basketBallStats.ThreePointerMade && GameOptions.gameModeSelected == 2)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_threePointersMade", (int)(basketBallStats.ThreePointerMade));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_threePointersMadePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_threePointersMadeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_threePointersMadeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _threePointerMade = 0;
            loadStats();
        }
        // save mode 3 (4s)
        if (_fourPointerMade < basketBallStats.FourPointerMade && GameOptions.gameModeSelected == 3)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_fourPointersMade", (int)(basketBallStats.FourPointerMade));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_fourPointersMadePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_fourPointersMadeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_fourPointersMadeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _fourPointerMade = 0;
            loadStats();
        }
        // save mode 4 (7s)
        if (_sevenPointerMade < basketBallStats.SevenPointerMade && GameOptions.gameModeSelected == 4)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_sevenPointersMade", (int)(basketBallStats.SevenPointerMade));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_sevenPointersMadePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_sevenPointersMadeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_sevenPointersMadeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _fourPointerMade = 0;
            loadStats();
        }
        // save mode 5 (long shot)
        if (_longestShotMade < (basketBallStats.LongestShotMade * 6) && GameOptions.gameModeSelected == 5)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_longestShotMade", (float)Math.Round(basketBallStats.LongestShotMade * 6, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _longestShotMade = 0;
            loadStats();
        }
        // save mode 6 (total shot distance made)
        if (_totalDistance < (basketBallStats.TotalDistance * 6) && GameOptions.gameModeSelected == 6)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_totalDistance", (float)Math.Round(basketBallStats.TotalDistance * 6, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalDistancePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalDistanceLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_totalDistanceDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _totalDistance = 0;
            loadStats();
        }
        // save mode 7 low time 3s
        if ((_makeThreePointersLowTime > basketBallStats.MakeThreePointersLowTime && GameOptions.gameModeSelected == 7)
            || _makeThreePointersLowTime == 0)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_lowThreeTime", (float)Math.Round(basketBallStats.MakeThreePointersLowTime, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowThreeTimePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowThreeTimeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowThreeTimeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _makeThreePointersLowTime = 0;
            loadStats();
        }
        // save mode 8 low time 4s
        if ((_makeFourPointersLowTime > basketBallStats.MakeFourPointersLowTime && GameOptions.gameModeSelected == 8)
            || _makeFourPointersLowTime == 0)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_lowFourTime", (float)Math.Round(basketBallStats.MakeFourPointersLowTime, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowFourTimePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowFourTimeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowFourTimeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _makeFourPointersLowTime = 0;
            loadStats();
        }
        // save mode 9 low time 3s + 4s
        if ((_makeAllPointersLowTime > basketBallStats.MakeAllPointersLowTime && GameOptions.gameModeSelected == 9)
            || _makeAllPointersLowTime == 0)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_lowAllTime", (float)Math.Round(basketBallStats.MakeAllPointersLowTime, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowAllTimePlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowAllTimeLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowAllTimeDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _makeAllPointersLowTime = 0;
            loadStats();
        }
        // save mode 10 low time 3s
        if ((_makeThreePointersMoneyBallLowTime > basketBallStats.MakeThreePointersMoneyBallLowTime && GameOptions.gameModeSelected == 10) 
            || _makeThreePointersMoneyBallLowTime == 0)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_lowThreeTimeMoneyBall", (float)Math.Round(basketBallStats.MakeThreePointersMoneyBallLowTime, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowThreeTimeMoneyBallPlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowThreeTimeMoneyBallLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowThreeTimeMoneyBallDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _makeThreePointersLowTime = 0;
            loadStats();
        }
        // save mode 11 low time 4s
        if ((_makeFourPointersMoneyBallLowTime > basketBallStats.MakeFourPointersMoneyBallLowTime && GameOptions.gameModeSelected == 11 ) 
            || _makeFourPointersMoneyBallLowTime == 0)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_lowFourTimeMoneyBall", (float)Math.Round(basketBallStats.MakeFourPointersMoneyBallLowTime, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowFourTimeMoneyBallPlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowFourTimeMoneyBallLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowFourTimeMoneyBallDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _makeFourPointersLowTime = 0;
            loadStats();
        }
        // save mode 12 low time 3s + 4s
        if ((_makeAllPointersMoneyBallLowTime > basketBallStats.MakeAllPointersMoneyBallLowTime && GameOptions.gameModeSelected == 12) 
            || _makeAllPointersMoneyBallLowTime == 0)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_lowAllTimeMoneyBall", (float)Math.Round(basketBallStats.MakeAllPointersMoneyBallLowTime, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowAllTimeMoneyBallPlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowAllTimeMoneyBallLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_lowAllTimeMoneyBallDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _makeAllPointersLowTime = 0;
            loadStats();
        }

        // save mode 13 (longest shot in free play)
        if (_longestShotMadeFreePlay < (basketBallStats.LongestShotMade * 6) && GameOptions.gameModeSelected == 13)
        {
            messageLog.instance.toggleMessageDisplay("New High Score");
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlay", (float)Math.Round(basketBallStats.LongestShotMade * 6, 4));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayPlayer", GameOptions.playerObjectName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayLevel", GameOptions.levelSelectedName);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayDate", DateTime.Now.ToString(CultureInfo.CurrentCulture));
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlayAppVersion", Application.version);
            PlayerPrefs.SetString("mode_" + GameOptions.gameModeSelected + "_operatingSystem", SystemInfo.operatingSystem);
            _longestShotMadeFreePlay = 0;
            loadStats();
        }
        // after save, data zeroed. can reload
    }

    public void loadStats()
    {
       //Debug.Log("load()");
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

        //_totalPoints = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_totalPoints");
        //_shotMade = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_totalShotMade");
        //_shotAttempt = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_totalShotAttempt");
        //_twoPointerMade = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_twoPointersMade");
        //_threePointerMade = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_threePointersMade");
        //_fourPointerMade = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_fourPointersMade");
        //_twoPointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_twoPointersAttempt");
        //_threePointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_threePointersAttempt");
        //_fourPointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_fourPointersAttempt");
        //_longestShotMade = PlayerPrefs.GetFloat("mode_" + stats.PlayerName + "_longestShotMade");
        //_totalDistance  = PlayerPrefs.GetFloat("mode_" + stats.PlayerName + "_totalDistance", (int)(stats.TotalDistance));

        _totalPoints = PlayerPrefs.GetInt("mode_" + 1 + "_totalPoints");

        //_shotMade = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_totalShotMade");
        //_shotAttempt = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_totalShotAttempt");
        //_twoPointerMade = PlayerPrefs.GetInt("mode_" + 2 + "_twoPointersMade");

        _threePointerMade = PlayerPrefs.GetInt("mode_" + 2 + "_threePointersMade");
        _fourPointerMade = PlayerPrefs.GetInt("mode_" + 3 + "_fourPointersMade");
        _sevenPointerMade = PlayerPrefs.GetInt("mode_" + 4 + "_sevenPointersMade");

        //_twoPointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_twoPointersAttempt");
        //_threePointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_threePointersAttempt");
        //_fourPointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_fourPointersAttempt");

        _longestShotMade = PlayerPrefs.GetFloat("mode_" + 5 + "_longestShotMade");
        _totalDistance = PlayerPrefs.GetFloat("mode_" + 6 + "_totalDistance");

        _makeThreePointersLowTime = PlayerPrefs.GetFloat("mode_" + 7 + "_lowThreeTime");
        _makeFourPointersLowTime = PlayerPrefs.GetFloat("mode_" + 8 + "_lowFourTime");
        _makeAllPointersLowTime = PlayerPrefs.GetFloat("mode_" + 9 + "_lowAllTime");

        _makeThreePointersMoneyBallLowTime = PlayerPrefs.GetFloat("mode_" + 10 + "_lowThreeTimeMoneyBall");
        _makeFourPointersMoneyBallLowTime = PlayerPrefs.GetFloat("mode_" + 11 + "_lowFourTimeMoneyBall");
        _makeAllPointersMoneyBallLowTime = PlayerPrefs.GetFloat("mode_" + 12 + "_lowAllTimeMoneyBall");

        _longestShotMadeFreePlay = PlayerPrefs.GetFloat("mode_" + 13 + "_longestShotMadeFreePlay");
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

        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalPoints");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalShotMade");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalShotAttempt");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_twoPointersMade");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_threePointersMade");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_fourPointersMade");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_twoPointersAttempt");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_threePointersAttempt");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_fourPointersAttempt");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_longestShotMade");
        //PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalDistance");
    }

    public bool IsCheating
    {
        get => _isCheating;
        set => _isCheating = value;
    }

    public float SevenPointerAttempts =>_sevenPointerAttempts;
    
    public float LongestShotMadeFreePlay => _longestShotMadeFreePlay;

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
}
