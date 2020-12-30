﻿using UnityEngine;

public class BasketBallStats : MonoBehaviour
{
    private int _playerId;
    private string _playerName;

    private int _levelId;
    private string _levelName;

    private int _experienceGained;

    private int _totalPoints;
    [SerializeField]
    private int _twoPointerMade;
    [SerializeField]
    private int _threePointerMade;
    [SerializeField]
    private int _fourPointerMade;
    [SerializeField]
    private int _sevenPointerMade;
    private int _moneyBallMade;

    private int _twoPointerAttempts;
    private int _threePointerAttempts;
    private int _fourPointerAttempts;
    private int _sevenPointerAttempts;
    private int _moneyBallAttempts;

    private int _shotAttempt;
    private int _shotMade;
    private float _longestShotMade;
    private float _totalDistance;

    private float _makeThreePointersLowTime;
    private float _makeFourPointersLowTime;
    private float _makeAllPointersLowTime;

    private float _makeThreePointersMoneyBallLowTime;
    private float _makeFourPointersMoneyBallLowTime;
    private float _makeAllPointersMoneyBallLowTime;

    private int _criticalRolled;
    private int _mostConsecutiveShots;

    //enemies
    private int _enemiesKilled;
    private int _minionsKilled;
    private int _bossKilled;

    private float _timePlayed;

    //init from game options
    void Start()
    {
        // for saving character specific info
        // id and name use to construct key that will be stored
        PlayerId = GameOptions.playerId;
        PlayerName = GameOptions.playerObjectName;
    }

    public int getExperienceGainedFromSession()
    {
        int experience = 0;
        //experience += TotalPoints * 10;
        //Debug.Log("TotalPoints : " + TotalPoints);

        experience += (TwoPointerMade * 20);
        //Debug.Log("(TwoPointerMade * 2) : " + (TwoPointerMade * 10));

        experience += (ThreePointerMade * 30);
        //Debug.Log("(ThreePointerMade * 3) : " + (ThreePointerMade * 15));

        experience += (FourPointerMade * 40);
        //Debug.Log("(FourPointerMade * 4) : " + (FourPointerMade * 20));

        experience += (SevenPointerMade * 70);
        //Debug.Log("(SevenPointerMade * 7) : " + (SevenPointerMade * 35));

        experience += Mathf.RoundToInt(TotalDistance * 0.5f);
        //Debug.Log("TotalDistance : " + TotalDistance * 0.5f);

        //Debug.Log("MostConsecutiveShots * 50 : " + MostConsecutiveShots * 50);
        experience += (MostConsecutiveShots * 25);

        //Debug.Log("TotalPoints : " + TotalPoints);
        experience += TotalPoints;


        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            experience += (MinionsKilled * 100);
            experience += (BossKilled * 300);
            experience *= 2;
        }
        if (GameOptions.hardcoreModeEnabled)
        {
            experience *= 2;
        }

        ExperienceGained = experience;
        //Debug.Log("experience gained : " + ExperienceGained);
        //Debug.Log("modeid : " + GameOptions.gameModeSelectedId + " ExperienceGained : " + ExperienceGained);

        // if arcade mode, 0 out experience
        if (GameOptions.arcadeModeEnabled)
        {
            ExperienceGained = 0;
        }

        return ExperienceGained;
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

    public int CriticalRolled
    {
        get => _criticalRolled;
        set => _criticalRolled = value;
    }

    public int ShotAttempt
    {
        get => _shotAttempt;
        set => _shotAttempt = value;
    }

    public int ShotMade
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

    public int TotalPoints
    {
        get => _totalPoints;
        set => _totalPoints = value;
    }

    public int TwoPointerMade
    {
        get => _twoPointerMade;
        set => _twoPointerMade = value;
    }

    public int ThreePointerMade
    {
        get => _threePointerMade;
        set => _threePointerMade = value;
    }

    public int FourPointerMade
    {
        get => _fourPointerMade;
        set => _fourPointerMade = value;
    }

    public int TwoPointerAttempts
    {
        get => _twoPointerAttempts;
        set => _twoPointerAttempts = value;
    }

    public int ThreePointerAttempts
    {
        get => _threePointerAttempts;
        set => _threePointerAttempts = value;
    }

    public int FourPointerAttempts
    {
        get => _fourPointerAttempts;
        set => _fourPointerAttempts = value;
    }
    public int SevenPointerMade
    {
        get => _sevenPointerMade;
        set => _sevenPointerMade = value;
    }

    public int SevenPointerAttempts
    {
        get => _sevenPointerAttempts;
        set => _sevenPointerAttempts = value;
    }


    public int MoneyBallMade
    {
        get => _moneyBallMade;
        set => _moneyBallMade = value;
    }

    public int MoneyBallAttempts
    {
        get => _moneyBallAttempts;
        set => _moneyBallAttempts = value;
    }

    public float TimePlayed
    {
        get => _timePlayed;
        set => _timePlayed = value;
    }
    public int MostConsecutiveShots
    {
        get => _mostConsecutiveShots;
        set => _mostConsecutiveShots = value;
    }
    public int ExperienceGained
    {
        get => _experienceGained;
        set => _experienceGained = value;
    }
    public int EnemiesKilled { get => _enemiesKilled; set => _enemiesKilled = value; }
    public int BossKilled { get => _bossKilled; set => _bossKilled = value; }
    public int MinionsKilled { get => _minionsKilled; set => _minionsKilled = value; }
}
