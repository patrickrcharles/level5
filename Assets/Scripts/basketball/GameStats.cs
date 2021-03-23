using Assets.Scripts.Utility;
using UnityEngine;

public class GameStats : MonoBehaviour
{
    //private int _playerId;
    //private string _playerName;

    private int _levelId;
    private string _levelName;

    private int _experienceGained;

    private int _totalPoints;
    private int _bonusPoints;
    private int _twoPointerMade;
    private int _threePointerMade;
    private int _fourPointerMade;
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

    private int _sniperShots;
    private int _sniperHits;

    private float _timePlayed;

    ////init from game options
    //void Start()
    //{
    //    // for saving character specific info
    //    // id and name use to construct key that will be stored
    //    PlayerId = GameOptions.characterId;
    //    PlayerName = GameOptions.characterObjectName;
    //}

    public int getExperienceGainedFromSession()
    {
        int experience = 0;

        // get 1 - sniper accuracy. ex. if sniper accuracy = 30%, return 70%
        // player will receive that percentage of xp bonus. higher % for lower snipe accuracy
        float inverseSniperAccuracy = (1 - UtilityFunctions.getPercentageFloat(SniperHits, SniperShots));
        experience += Mathf.RoundToInt(500 * inverseSniperAccuracy);
        // +15 for gettign hit
        experience += (SniperHits * 15);

        experience += (ShotAttempt * 10);

        experience += (TwoPointerMade * 20);

        experience += (ThreePointerMade * 30);

        experience += (FourPointerMade * 40);

        experience += (SevenPointerMade * 70);

        experience += Mathf.RoundToInt(TotalDistance * 0.5f);

        experience += (MostConsecutiveShots * 25);

        experience += TotalPoints;

        if (GameOptions.trafficEnabled)
        {
            experience *= (int)1.15f;
        }
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            experience += (MinionsKilled * 50);
            experience += (BossKilled * 150);
            experience *= (int)1.25f;
        }
        if (GameOptions.hardcoreModeEnabled)
        {
            experience *= (int)1.5f;
        }
        if (GameOptions.sniperEnabled)
        {
            experience *= (int)1.25f;
        }

        ExperienceGained = experience;
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

    //public int PlayerId
    //{
    //    get => _playerId;
    //    set => _playerId = value;
    //}

    //public string PlayerName
    //{
    //    get => _playerName;
    //    set => _playerName = value;
    //}

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
    public int BonusPoints { get => _bonusPoints; set => _bonusPoints = value; }
    public int SniperShots { get => _sniperShots; set => _sniperShots = value; }
    public int SniperHits { get => _sniperHits; set => _sniperHits = value; }
}
