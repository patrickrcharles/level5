using System;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class BasketBallState : MonoBehaviour
{
    // constant values that have to be hardcoded
    const float _threePointDistance = 3.8f;
    const float _fourPointDistance = 5.2f;
    const float _sevenPointDistance = 16.7f;

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
    //private bool _facingFront;
    private bool _thrown;
    private bool _locked;
    [SerializeField]
    private bool _canPullBall;
    private bool _grounded;
    private bool _isTarget; // is ball a target for a player already

    [SerializeField]
    private float _ballDistanceFromRim;

    private GameObject basketBallPosition;
    private GameObject basketBallTarget;

    private GameObject player;
    private PlayerController playerState;

    private int _currentShotType;
    public int CurrentShotType => _currentShotType;


    void Start()
    {
        //player = GameLevelManager.instance.Player;
        //playerState = GameLevelManager.instance.PlayerState;

        // position of basketball infront of player
        basketBallPosition = GameLevelManager.Instance.Player.transform.Find("basketBall_position").gameObject;
        //position to shoot basketball at (middle of rim)
        basketBallTarget = GameObject.Find("basketBall_target");
        //IsTarget = false;
    }

    // Update is called once per frame
    void Update()
    {

        BallDistanceFromRim = Vector3.Distance(transform.position, basketBallTarget.transform.position);

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

    //public bool FacingFront
    //{
    //    get => _facingFront;
    //    set => _facingFront = value;
    //}

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


    public float ThreePointDistance
    {
        get => _threePointDistance;
    }

    public float FourPointDistance
    {
        get => _fourPointDistance;
    }

    public float SevenPointDistance
    {
        get => _sevenPointDistance;
    }


    public GameObject BasketBallPosition
    {
        get => basketBallPosition;
        set => basketBallPosition = value;
    }

    public GameObject BasketBallTarget
    {
        get => basketBallTarget;
        set => basketBallTarget = value;
    }
}
