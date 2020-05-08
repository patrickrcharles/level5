using System;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class BasketBallState : MonoBehaviour
{
    // constant values that have to be hardcoded
    private const float _threePointDistance = 3.8f;
    private const float _fourPointDistance = 5.2f;
    private const float _sevenPointDistance = 16.7f;

    private bool _twoPoints;
    private bool _threePoints;
    private bool _fourPoints;
    private bool _sevenPoints;
    private bool _twoAttempt;
    private bool _threeAttempt;
    private bool _fourAttempt;
    private bool _sevenAttempt;
    private bool _dunk;
    private bool _inAir;
    private bool _thrown;
    private bool _locked;
    private bool _canPullBall;
    private bool _grounded;

    [SerializeField]
    private bool _playerOnMarker;
    [SerializeField]
    private bool _playerOnMarkerOnShoot;

    [SerializeField]
    private int _currentShotMarkerId;
    private int _onShootShotMarkerId;

    [SerializeField]
    private float _ballDistanceFromRim;

    private GameObject _basketBallPosition;
    private GameObject _basketBallTarget;

    private int _currentShotType;
    public int CurrentShotType => _currentShotType;

    [SerializeField]
    private GameObject[] basketBallShotMarkerObjects;
    [SerializeField]
    private List<BasketBallShotMarker> _basketBallShotMarkersList;


    void Start()
    {

        // position of basketball infront of player
        _basketBallPosition = GameLevelManager.Instance.Player.transform.Find("basketBall_position").gameObject;

        //position to shoot basketball at (middle of rim)
        _basketBallTarget = GameObject.Find("basketBall_target");

        // get all shot position marker objects
        basketBallShotMarkerObjects = GameObject.FindGameObjectsWithTag("shot_marker");
        //load them into list
        foreach (var marker in basketBallShotMarkerObjects)
        {
            BasketBallShotMarker temp = marker.GetComponent<BasketBallShotMarker>();
            _basketBallShotMarkersList.Add(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {

        BallDistanceFromRim = Vector3.Distance(transform.position, _basketBallTarget.transform.position);

        PlayerOnMarker = BasketBallShotMarkersList[CurrentShotMarkerId].PlayerOnMarker;

        if (BallDistanceFromRim < ThreePointDistance)
        {
            TwoPoints = true;
            _currentShotType = 2;
        }
        else
        {
            TwoPoints = false;
        }

        if (BallDistanceFromRim > ThreePointDistance && BallDistanceFromRim < FourPointDistance)
        {
            ThreePoints = true;
            _currentShotType = 3;
        }
        else
        {
            ThreePoints = false;
        }

        if (BallDistanceFromRim > FourPointDistance && BallDistanceFromRim < SevenPointDistance)
        {
            FourPoints = true;
            _currentShotType = 4;
        }
        else
        {
            FourPoints = false;
        }

        if (BallDistanceFromRim > SevenPointDistance)
        {
            SevenPoints = true;
            _currentShotType = 7;
        }
        else
        {
            SevenPoints = false;
        }
    }


    public float ThreePointDistance => _threePointDistance;
    public float FourPointDistance => _fourPointDistance;
    public float SevenPointDistance => _sevenPointDistance;

    public List<BasketBallShotMarker> BasketBallShotMarkersList
    {
        get => _basketBallShotMarkersList;
        set => _basketBallShotMarkersList = value;
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
    public float BallDistanceFromRim
    {
        get => _ballDistanceFromRim;
        set => _ballDistanceFromRim = value;
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
