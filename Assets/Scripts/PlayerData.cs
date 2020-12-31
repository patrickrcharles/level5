using UnityEngine;

/* This class is used to store Player data from the database such as high scores
 * and data that will be used to update high scores and inserted into database
 */

public class PlayerData : MonoBehaviour
{
    private int _playerId = 0;
    private string _playerName = "";

    private float _totalPoints = 0;
    private float _totalPointsByDistance = 0;
    private float _totalPointsBonus = 0;
    private float _twoPointerMade = 0;
    private float _threePointerMade = 0;
    private float _sevenPointerMade = 0;

    private float _fourPointerMade = 0;
    private float _twoPointerAttempts = 0;
    private float _threePointerAttempts = 0;
    private float _fourPointerAttempts = 0;
    private float _sevenPointerAttempts = 0;

    //private float _shotAttempt = 0;
    //private float _shotMade = 0;
    //private float _longestShotMade = 0;
    private float _longestShotMadeFreePlay = 0;
    private float _longestShotMadeArcade = 0;
    private float _totalDistance = 0;

    private float _makeThreePointersLowTime = 0;
    private float _makeFourPointersLowTime = 0;
    private float _makeAllPointersLowTime = 0;

    private float _makeThreePointersMoneyBallLowTime = 0;
    private float _makeFourPointersMoneyBallLowTime = 0;
    private float _makeAllPointersMoneyBallLowTime = 0;

    private int _mostConsecutiveShots = 0;

    private float _threePointContestScore = 0;
    private float _fourPointContestScore = 0;
    private float _allPointContestScore = 0;

    private int _enemiesKilled = 0;

    [SerializeField]
    private int _currentExperience = 0;
    [SerializeField]
    private int _currentLevel = 0;
    [SerializeField]
    private int _updatePointsAvailable = 0;
    [SerializeField]
    private int _updatePointsUsed = 0;

    public static PlayerData instance;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //if (GameObject.FindGameObjectsWithTag("database") != null)
        //{
        //    Debug.Log("load player high scores");
        //    loadStatsFromDatabase();
        //}
        if (GameObject.FindGameObjectsWithTag("database") != null)
        {
            //Debug.Log("load player high scores");
            loadStatsFromDatabase();
        }
    }

    public void loadStatsFromDatabase()
    {
        //Debug.Log("Player Data -- load from database -- high scores");

        if (DBHelper.instance != null)
        {
            _totalPoints = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 1, "DESC");

            TotalPointsBonus = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 15, "DESC");
            _threePointerMade = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "maxShotMade", 2, "DESC");
            _fourPointerMade = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "maxShotMade", 3, "DESC");
            _sevenPointerMade = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "maxShotMade", 4, "DESC");

            _longestShotMadeFreePlay = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "longestShot", 99, "DESC");
            _longestShotMadeArcade = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "longestShot", 98, "DESC");

            _totalDistance = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "totalDistance", 6, "DESC");
            _makeThreePointersLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 7, "ASC");
            _makeFourPointersLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 8, "ASC");
            _makeAllPointersLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 9, "ASC");
            _makeThreePointersMoneyBallLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 10, "ASC");
            _makeFourPointersMoneyBallLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 11, "ASC");
            _makeAllPointersMoneyBallLowTime = DBHelper.instance.getFloatValueHighScoreFromTableByFieldAndModeId("HighScores", "time", 12, "ASC");
            LongestShotMadeFreePlay = DBHelper.instance.getFloatValueHighScoreFromTableByField("AllTimeStats", "longestShot", "DESC");
            _mostConsecutiveShots = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "consecutiveShots", 14, "DESC");
            _totalPointsBonus = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 15, "DESC");
            _threePointContestScore = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 16, "DESC");
            _fourPointContestScore = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 17, "DESC");
            _allPointContestScore = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 18, "DESC");
            TotalPointsByDistance = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "totalPoints", 19, "DESC");
            EnemiesKilled = DBHelper.instance.getIntValueHighScoreFromTableByFieldAndModeId("HighScores", "enemiesKilled", 20, "DESC");
        }
    }

    public float TotalPoints => _totalPoints;

    //public float TwoPointerMade => _twoPointerMade;

    public float ThreePointerMade => _threePointerMade;

    public float FourPointerMade => _fourPointerMade;

    //public float TwoPointerAttempts => _twoPointerAttempts;

    //public float ThreePointerAttempts => _threePointerAttempts;

    //public float FourPointerAttempts => _fourPointerAttempts;

    public float SevenPointerMade => _sevenPointerMade;

    //public float ShotAttempt => _shotAttempt;

    //public float ShotMade => _shotMade;

    //public float LongestShotMade => _longestShotMade;

    public float TotalDistance => _totalDistance;

    public float MakeThreePointersLowTime => _makeThreePointersLowTime;

    public float MakeFourPointersLowTime => _makeFourPointersLowTime;

    public float MakeAllPointersLowTime => _makeAllPointersLowTime;

    public float MakeThreePointersMoneyBallLowTime => _makeThreePointersMoneyBallLowTime;

    public float MakeFourPointersMoneyBallLowTime => _makeFourPointersMoneyBallLowTime;

    public float MakeAllPointersMoneyBallLowTime => _makeAllPointersMoneyBallLowTime;

    public float LongestShotMadeFreePlay { get => _longestShotMadeFreePlay; set => _longestShotMadeFreePlay = value; }
    public int MostConsecutiveShots { get => _mostConsecutiveShots; set => _mostConsecutiveShots = value; }
    public float TotalPointsBonus { get => _totalPointsBonus; set => _totalPointsBonus = value; }
    public float ThreePointContestScore { get => _threePointContestScore; set => _threePointContestScore = value; }
    public float FourPointContestScore { get => _fourPointContestScore; set => _fourPointContestScore = value; }
    public float AllPointContestScore { get => _allPointContestScore; set => _allPointContestScore = value; }
    public int CurrentExperience { get => _currentExperience; set => _currentExperience = value; }
    public int CurrentLevel { get => _currentLevel; set => _currentLevel = value; }
    public int UpdatePointsAvailable { get => _updatePointsAvailable; set => _updatePointsAvailable = value; }
    public int UpdatePointsUsed { get => _updatePointsUsed; set => _updatePointsUsed = value; }
    public float TotalPointsByDistance { get => _totalPointsByDistance; set => _totalPointsByDistance = value; }
    public int EnemiesKilled { get => _enemiesKilled; set => _enemiesKilled = value; }
    //public float LongestShotMadeArcade { get => _longestShotMadeArcade; set => _longestShotMadeArcade = value; }
}
