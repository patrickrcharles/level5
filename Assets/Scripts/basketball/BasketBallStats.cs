using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallStats : MonoBehaviour
{

    private float _totalPoints;
    private float _twoPointerMade;
    private float _threePointerMade;

    private float _fourPointerMade;
    private float _twoPointerAttempts;
    private float _threePointerAttempts;
    private float _fourPointerAttempts;

    private float shotAttempt;
    private float shotMade;


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

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

    public float ShotAttempt
    {
        get => shotAttempt;
        set => shotAttempt = value;
    }

    public float ShotMade
    {
        get => shotMade;
        set => shotMade = value;
    }

}
