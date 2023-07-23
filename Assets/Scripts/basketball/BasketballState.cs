﻿using UnityEngine;


public class BasketBallState : MonoBehaviour
{
    [SerializeField]
    private bool _twoPoints;
    [SerializeField]
    private bool _threePoints;
    [SerializeField]
    private bool _fourPoints;
    [SerializeField]
    private bool _sevenPoints;
    private bool _twoAttempt;
    private bool _threeAttempt;
    private bool _fourAttempt;
    private bool _sevenAttempt;
    private bool _dunk;
    [SerializeField]
    private bool _inAir;
    [SerializeField]
    private bool _thrown;
    [SerializeField]
    private bool _locked;
    private bool _canPullBall;
    [SerializeField]
    private bool _grounded;

    private bool _playerOnMarker;
    private bool _playerOnMarkerOnShoot;
    private bool __moneyBallEnabledOnShoot;

    private int _currentShotMarkerId;
    private int _onShootShotMarkerId;

    [SerializeField]
    private float _playerDistanceFromRim;

    private GameObject _basketBallPosition;
    private GameObject _basketBallTarget;
       [SerializeField]
    private int _currentShotType;
    public int CurrentShotType => _currentShotType;
    public static BasketBallState instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {

        switch (GameLevelManager.instance.IsAutoPlayer)
        {
            case true:
                _basketBallPosition = GameLevelManager.instance.AutoPlayer.transform.Find("basketBall_position").gameObject;
                break;
            case false:
                _basketBallPosition = GameLevelManager.instance.Player1.transform.Find("basketBall_position").gameObject;
                break;
        }
        //// position of basketball infront of player
        //_basketBallPosition = GameLevelManager.instance.Player.transform.Find("basketBall_position").gameObject;

        //position to shoot basketball at (middle of rim)
        _basketBallTarget = GameObject.Find("basketBall_target");

    }

    // Update is called once per frame
    void Update()
    {

        PlayerDistanceFromRim = GameLevelManager.instance.IsAutoPlayer 
            ? GameLevelManager.instance.AutoPlayerController.PlayerDistanceFromRim  
            : GameLevelManager.instance.PlayerController1.PlayerDistanceFromRim;
        //PlayerDistanceFromRim = Vector3.Distance(GameLevelManager.instance.Player.transform.position, _basketBallTarget.transform.position);
        //PlayerDistanceFromRim = Mathf.Abs( GameLevelManager.instance.Player.transform.position.z - _basketBallTarget.transform.position.z);

        // is player on  marker  +  is marker required for game mode
        if (GameRules.instance.PositionMarkersRequired)
        {
            PlayerOnMarker = GameRules.instance.BasketBallShotMarkersList[CurrentShotMarkerId].PlayerOnMarker;
        }


        if (PlayerDistanceFromRim < Constants.DISTANCE_3point)
        {
            TwoPoints = true;
            _currentShotType = 2;
        }
        else
        {
            TwoPoints = false;
        }

        if (PlayerDistanceFromRim >= Constants.DISTANCE_3point && PlayerDistanceFromRim < Constants.DISTANCE_4point)
        {
            ThreePoints = true;
            _currentShotType = 3;
        }
        else
        {
            ThreePoints = false;
        }

        if (PlayerDistanceFromRim >= Constants.DISTANCE_4point && PlayerDistanceFromRim < Constants.DISTANCE_7point)
        {
            FourPoints = true;
            _currentShotType = 4;
        }
        else
        {
            FourPoints = false;
        }

        if (PlayerDistanceFromRim > Constants.DISTANCE_7point)
        {
            SevenPoints = true;
            _currentShotType = 7;
        }
        else
        {
            SevenPoints = false;
        }
    }

    //bool isPlayerOnMarker()
    //{
    //    // is player on 3point marker  + game requires 3 pt markers
    //    //if (GameRules.instance.GameModeRequiresShotMarkers3S)
    //    //{
    //    //    PlayerOnMarker = GameRules.instance.BasketBallShotMarkersList[CurrentShotMarkerId].PlayerOnMarker;
    //    //    return true;
    //    //}

    //    //// is player on 4p oint marker  + game requires 4 pt markers
    //    //if (GameRules.instance.GameModeRequiresShotMarkers4S)
    //    //{
    //    //    PlayerOnMarker = GameRules.instance.BasketBallShotMarkers4ptList[CurrentShotMarkerId].PlayerOnMarker;
    //    //    return true;
    //    //}

    //    return false;
    //}


    public bool MoneyBallEnabledOnShoot
    {
        get => __moneyBallEnabledOnShoot;
        set => __moneyBallEnabledOnShoot = value;
    }

    public bool PlayerOnMarker
    {
        get => _playerOnMarker;
        set => _playerOnMarker = value;
    }
    public bool PlayerOnMarkerOnShoot
    {
        get => _playerOnMarkerOnShoot;
        set => _playerOnMarkerOnShoot = value;
    }


    public int CurrentShotMarkerId
    {
        get => _currentShotMarkerId;
        set => _currentShotMarkerId = value;
    }

    public int OnShootShotMarkerId
    {
        get => _onShootShotMarkerId;
        set => _onShootShotMarkerId = value;
    }
    public bool TwoPoints
    {
        get => _twoPoints;
        set => _twoPoints = value;
    }

    public bool ThreePoints
    {
        get => _threePoints;
        set => _threePoints = value;
    }

    public bool FourPoints
    {
        get => _fourPoints;
        set => _fourPoints = value;
    }

    public bool SevenPoints
    {
        get => _sevenPoints;
        set => _sevenPoints = value;
    }

    public bool TwoAttempt
    {
        get => _twoAttempt;
        set => _twoAttempt = value;
    }

    public bool ThreeAttempt
    {
        get => _threeAttempt;
        set => _threeAttempt = value;
    }

    public bool FourAttempt
    {
        get => _fourAttempt;
        set => _fourAttempt = value;
    }

    public bool SevenAttempt
    {
        get => _sevenAttempt;
        set => _sevenAttempt = value;
    }

    public bool Dunk
    {
        get => _dunk;
        set => _dunk = value;
    }

    public bool InAir
    {
        get => _inAir;
        set => _inAir = value;
    }

    public bool Thrown
    {
        get => _thrown;
        set => _thrown = value;
    }

    public bool Locked
    {
        get => _locked;
        set => _locked = value;
    }

    public bool CanPullBall
    {
        get => _canPullBall;
        set => _canPullBall = value;
    }

    public bool Grounded
    {
        get => _grounded;
        set => _grounded = value;
    }

    public float PlayerDistanceFromRim
    {
        get => _playerDistanceFromRim;
        set => _playerDistanceFromRim = value;
    }

    public GameObject BasketBallPosition
    {
        get => _basketBallPosition;
        set => _basketBallPosition = value;
    }

    public GameObject BasketBallTarget
    {
        get => _basketBallTarget;
        set => _basketBallTarget = value;
    }
}
