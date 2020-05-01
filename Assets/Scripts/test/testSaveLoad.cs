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

    public float shotAttempt;
    public float shotMade;
    public float longestShotMade;

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
        //load(basketBallStatsballStats);

        PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalPoints",  (int)(_totalPoints + stats.TotalPoints));
        PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalShotMade", (int)(shotMade +stats.ShotMade));
        PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_totalShotAttempt", (int)(shotAttempt+  stats.ShotAttempt));
        PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+"_twoPointersMade", (int)(_twoPointerMade+stats.TwoPointerMade));
        PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+ "_threePointersMade", (int)(_threePointerMade+stats.ThreePointerMade));
        PlayerPrefs.SetInt(stats.PlayerId+"_"+stats.PlayerName+ "_fourPointersMade", (int)(_fourPointerMade+stats.FourPointerMade));
        PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersAttempt", (int)(_twoPointerAttempts+stats.TwoPointerAttempts));
        PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersAttempt", (int)(stats.ThreePointerAttempts));
        PlayerPrefs.SetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersAttempt", (int)(stats.FourPointerAttempts));

        if (stats.LongestShotMade > longestShotMade)
        {
            PlayerPrefs.SetFloat(stats.PlayerId + "_" + stats.PlayerName + "_longestShotMade",  stats.LongestShotMade);
        }
    }

    public void load(BasketBallStats stats)
    {
        //int temp = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalPoints", (int)stats.TotalPoints);

        //Debug.Log(" load : total points : " + temp);
        _totalPoints = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalPoints");
        shotMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalShotMade");
        shotAttempt = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_totalShotAttempt");
        _twoPointerMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersMade");
        _threePointerMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersMade");
        _fourPointerMade = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersMade");
        _twoPointerAttempts = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_twoPointersAttempt");
        _threePointerAttempts = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_threePointersAttempt");
        _fourPointerAttempts = PlayerPrefs.GetInt(stats.PlayerId + "_" + stats.PlayerName + "_fourPointersAttempt");
        longestShotMade = PlayerPrefs.GetFloat(stats.PlayerId + "_" + stats.PlayerName + "_longestShotMade");
    }
}
