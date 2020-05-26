﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallStats : MonoBehaviour
{
    private int _playerId;
    private string _playerName;    
    
    private int _levelId;
    private string _levelName;

    private float _totalPoints;
    private float _twoPointerMade;
    private float _threePointerMade;
    private float _fourPointerMade;
    private float _sevenPointerMade;
    private float _moneyBallMade;

    private float _twoPointerAttempts;
    private float _threePointerAttempts;
    private float _fourPointerAttempts;
    private float _sevenPointerAttempts;
    private float _moneyBallAttempts;

    private float _shotAttempt;
    private float _shotMade;
    private float _longestShotMade;
    private float _totalDistance;

    private float _makeThreePointersLowTime;
    private float _makeFourPointersLowTime;
    private float _makeAllPointersLowTime;

    private float _makeThreePointersMoneyBallLowTime;
    private float _makeFourPointersMoneyBallLowTime;
    private float _makeAllPointersMoneyBallLowTime;

    private float _criticalRolled;
    private float _timePlayed;
    private float _mostConsecutiveShots;

    //init from game options
    void Start()
    {
        // for saving character specific info
        // id and name use to construct key that will be stored
        PlayerId = GameOptions.playerId;
        PlayerName = GameOptions.playerObjectName;
    }

    public float MakeThreePointersMoneyBallLowTime
    {
        get => _makeThreePointersMoneyBallLowTime;
        set => _makeThreePointersMoneyBallLowTime = value;
    }

    public float MakeFourPointersMoneyBallLowTime
    {
        get => _makeFourPointersMoneyBallLowTime;
        set => _makeFourPointersMoneyBallLowTime = value;
    }

    public float MakeAllPointersMoneyBallLowTime
    {
        get => _makeAllPointersMoneyBallLowTime;
        set => _makeAllPointersMoneyBallLowTime = value;
    }

    public float MakeThreePointersLowTime
    {
        get => _makeThreePointersLowTime;
        set => _makeThreePointersLowTime = value;
    }

    public float MakeFourPointersLowTime
    {
        get => _makeFourPointersLowTime;
        set => _makeFourPointersLowTime = value;
    }

    public float MakeAllPointersLowTime
    {
        get => _makeAllPointersLowTime;
        set => _makeAllPointersLowTime = value;
    }

    public float CriticalRolled
    {
        get => _criticalRolled;
        set => _criticalRolled = value;
    }

    public float ShotAttempt
    {
        get => _shotAttempt;
        set => _shotAttempt = value;
    }

    public float ShotMade
    {
        get => _shotMade;
        set => _shotMade = value;
    }

    public float LongestShotMade
    {
        get => _longestShotMade;
        set => _longestShotMade = value;
    }

    public float TotalDistance
    {
        get => _totalDistance;
        set => _totalDistance = value;
    }

    public int PlayerId
    {
        get => _playerId;
        set => _playerId = value;
    }

    public string PlayerName
    {
        get => _playerName;
        set => _playerName = value;
    }

    public float TotalPoints
    {
        get => _totalPoints;
        set => _totalPoints = value;
    }

    public float TwoPointerMade
    {
        get => _twoPointerMade;
        set => _twoPointerMade = value;
    }

    public float ThreePointerMade
    {
        get => _threePointerMade;
        set => _threePointerMade = value;
    }

    public float FourPointerMade
    {
        get => _fourPointerMade;
        set => _fourPointerMade = value;
    }

    public float TwoPointerAttempts
    {
        get => _twoPointerAttempts;
        set => _twoPointerAttempts = value;
    }

    public float ThreePointerAttempts
    {
        get => _threePointerAttempts;
        set => _threePointerAttempts = value;
    }

    public float FourPointerAttempts
    {
        get => _fourPointerAttempts;
        set => _fourPointerAttempts = value;
    }
    public float SevenPointerMade
    {
        get => _sevenPointerMade;
        set => _sevenPointerMade = value;
    }

    public float SevenPointerAttempts
    {
        get => _sevenPointerAttempts;
        set => _sevenPointerAttempts = value;
    }


    public float MoneyBallMade
    {
        get => _moneyBallMade;
        set => _moneyBallMade = value;
    }

    public float MoneyBallAttempts
    {
        get => _moneyBallAttempts;
        set => _moneyBallAttempts = value;
    }

    public float TimePlayed
    {
        get => _timePlayed;
        set => _timePlayed = value;
    }
    public float MostConsecutiveShots 
    { get => _mostConsecutiveShots;
      set => _mostConsecutiveShots = value; 
    }
}
