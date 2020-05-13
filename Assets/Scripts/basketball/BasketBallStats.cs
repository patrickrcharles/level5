using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallStats : MonoBehaviour
{
    private int _playerId;
    private string _playerName;

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

    [SerializeField]
    private float _makeThreePointersLowTime;
    [SerializeField]
    private float _makeFourPointersLowTime;
    [SerializeField]
    private float _makeAllPointersLowTime;

    private float _criticalRolled;

    //init from game options
    void Start()
    {
        // for saving character specific info
        // id and name use to construct key that will be stored
        PlayerId = GameOptions.playerId;
        PlayerName = GameOptions.playerObjectName;
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
}
