using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;

public class PlayerData : MonoBehaviour
{
    private BasketBallStats basketBallStats;

    public  int _playerId;
    public  string _playerName;

    public  float _totalPoints;
    public  float _twoPointerMade;
    public  float _threePointerMade;

    public  float _fourPointerMade;
    public  float _twoPointerAttempts;
    public  float _threePointerAttempts;
    public  float _fourPointerAttempts;

    public  float _shotAttempt;
    public  float _shotMade;
    public  float _longestShotMade;
    public  float _longestShotMadeFreePlay;
    public  float _totalDistance;

    [SerializeField]
    private bool _isCheating;
    public bool IsCheating
    {
        get => _isCheating;
        set => _isCheating = value;
    }

    public static PlayerData instance;

    static bool created = false;

    void Awake()
    {
        instance = this;
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
        else
        {
            Destroy(this.gameObject);
        }
        loadStats();
    }

    void Start()
    {

        // forcing this for testing purpose
        //GameOptions.gameModeSelected = 2;
    }
    // Update is called once per frame
    void Update()
    {
        ////turn off accuracy modifer 6+9
        //if (InputManager.GetKey(KeyCode.LeftShift)
        //    && InputManager.GetKeyDown(KeyCode.Alpha8))
        //{
        //    save(basketBallStats);
        //    Debug.Log("save stats");
        //}
        ////turn off accuracy modifer 6+9
        //if (InputManager.GetKey(KeyCode.LeftShift)
        //    && InputManager.GetKeyDown(KeyCode.Alpha9))
        //{
        //    load(basketBallStats);
        //    Debug.Log("load stats");
        //}
    }

    public void saveStats()
    {
        Debug.Log("saveData");
        basketBallStats = GameObject.FindWithTag("basketball").GetComponent<BasketBallStats>();

        if (IsCheating && GameOptions.gameModeSelected != 6)
        {
            Debug.Log("dont save");
            Debug.Log("ischeating : "+IsCheating + " mode : "+ GameOptions.gameModeSelected);
            return;
            //if (_longestShotMade < (basketBallStats.LongestShotMade * 6) && GameOptions.gameModeSelected == 6)
            //{
            //    PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlay", (float)Math.Round(basketBallStats.LongestShotMade * 6, 4));
            //    _longestShotMadeFreePlay = 0;
            //    loadStats();
        }
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
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_totalPoints", (int) (basketBallStats.TotalPoints));
            _totalPoints = 0;
            loadStats();
        }
        // save mode 2 (3s)
        if (_threePointerMade < basketBallStats.ThreePointerMade && GameOptions.gameModeSelected == 2)
        {
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_threePointersMade", (int)(basketBallStats.ThreePointerMade));
            _threePointerMade = 0;
            loadStats();
        }
        // save mode 3 (4s)
        if (_fourPointerMade < basketBallStats.FourPointerMade && GameOptions.gameModeSelected == 3)
        {
            PlayerPrefs.SetInt("mode_" + GameOptions.gameModeSelected + "_fourPointersMade", (int)(basketBallStats.FourPointerMade));
            _fourPointerMade = 0;
            loadStats();
        }
        // save mode 4 (long shot)
        if (_longestShotMade < (basketBallStats.LongestShotMade * 6) && GameOptions.gameModeSelected == 4)
        {
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_longestShotMade", (float)Math.Round(basketBallStats.LongestShotMade * 6, 4));
            _longestShotMade = 0;
            loadStats();
        }
        // save mode 5 (total shot distance made)
        if (_totalDistance < (basketBallStats.TotalDistance * 6) && GameOptions.gameModeSelected == 5)
        {
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_totalDistance", (float)Math.Round(basketBallStats.TotalDistance * 6, 4));
            _totalDistance = 0;
            loadStats();
        }
        if (_longestShotMadeFreePlay < (basketBallStats.LongestShotMade * 6) && GameOptions.gameModeSelected == 6)
        {
            PlayerPrefs.SetFloat("mode_" + GameOptions.gameModeSelected + "_longestShotMadeFreePlay", (float)Math.Round(basketBallStats.LongestShotMade * 6, 4));
            _longestShotMadeFreePlay = 0;
            loadStats();
        }
        // after save, data zeroed. can reload
    }

    public void loadStats()
    {
        Debug.Log("load()");
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

        //_twoPointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_twoPointersAttempt");
        //_threePointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_threePointersAttempt");
        //_fourPointerAttempts = PlayerPrefs.GetInt("mode_" + stats.PlayerName + "_fourPointersAttempt");

        _longestShotMade = PlayerPrefs.GetFloat("mode_" + 4 + "_longestShotMade");
        _totalDistance = PlayerPrefs.GetFloat("mode_" + 5 + "_totalDistance");

        _longestShotMadeFreePlay = PlayerPrefs.GetFloat("mode_" + 6 + "_longestShotMadeFreePlay");


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

        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalPoints");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalShotMade");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalShotAttempt");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_twoPointersMade");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_threePointersMade");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_fourPointersMade");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_twoPointersAttempt");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_threePointersAttempt");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_fourPointersAttempt");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_longestShotMade");
        PlayerPrefs.DeleteKey("mode_" + stats.PlayerName + "_totalDistance");
    }

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

    public float ShotAttempt => _shotAttempt;

    public float ShotMade => _shotMade;

    public float LongestShotMade => _longestShotMade;

    public float TotalDistance => _totalDistance;

    public static PlayerData Instance => instance;

}
