
using UnityEngine;
using Random = System.Random;


public class BasketBallShotMade : MonoBehaviour
{

    BasketBallState _basketBallState;
    BasketBallStats _basketBallStats;

    //public int currentShotTestIndex;

    AudioSource audioSource;
    public GameObject rimSprite;
    Animator anim;
    PlayerController playerState;
    bool isColliding;

    const string moneyPrefabPath = "Prefabs/objects/money";
    private GameObject moneyClone;

    [SerializeField]
    int _consecutiveShotsMade;

    int _currentShotMade;
    int _currentShotAttempts;

    int _expectedShotMade;
    int _expectedShotAttempts;

    public static BasketBallShotMade instance;


    private void Awake()
    {
        instance = this;
        _currentShotMade = 0;
        _currentShotAttempts = 0;
        _expectedShotMade = 1;
        _expectedShotAttempts = 1;
    }

    // Use this for initialization
    void Start()
    {
        _basketBallState = BasketBall.instance.GetComponent<BasketBallState>();
        _basketBallStats = BasketBall.instance.GetComponent<BasketBallStats>();
        audioSource = GetComponent<AudioSource>();
        anim = rimSprite.GetComponent<Animator>();
        playerState = GameLevelManager.instance.PlayerState;

        // path to money prfab
        moneyClone = Resources.Load(moneyPrefabPath) as GameObject;

    }

    // Update is called once per frame
    void Update()
    {
        isColliding = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("basketball") && !playerState.hasBasketball && !isColliding)
        {
            if (isColliding) return;
            else { isColliding = true; }

            audioSource.PlayOneShot(SFXBB.instance.basketballNetSwish);

            // add to total shot distance made total
            _basketBallStats.TotalDistance += (BasketBall.instance.LastShotDistance * 6);

            //Debug.Log(" last shot : " + BasketBall.instance.LastShotDistance * 6);
            //Debug.Log(" long shot : " + _basketBallStats.LongestShotMade);
            // is this the longest shot made?
            if ((BasketBall.instance.LastShotDistance * 6) > _basketBallStats.LongestShotMade)
            {
                _basketBallStats.LongestShotMade = BasketBall.instance.LastShotDistance * 6;
            }
            // play rim animation
            anim.Play("madeshot");

            // updates shots made/shot attempted
            updateShotMadeBasketBallStats();

            // instantiate money if game requires it
            if (GameRules.instance.GameModeRequiresMoneyBall
                && _basketBallState.PlayerOnMarkerOnShoot)
            //&& _basketBallState.MoneyBallEnabledOnShoot)
            //&& PlayerStats.instance.Money >= 5
            //&& GameRules.instance.MoneyBallEnabled)
            {
                //Debug.Log(" instantiate moeny : player on marker at shoot");
                instantiateMoney(1);
            }

            // reset states
            _basketBallState.TwoAttempt = false;
            _basketBallState.ThreeAttempt = false;
            _basketBallState.FourAttempt = false;
            _basketBallState.SevenAttempt = false;
            _basketBallState.MoneyBallEnabledOnShoot = false;
            _basketBallState.PlayerOnMarkerOnShoot = false;

        }

        // update onscreen ui stats
        if (BasketBall.instance.UiStatsEnabled)
        {
            BasketBall.instance.updateScoreText();
        }

        //// object test shot data in list
        //BasketBall.instance.testConclusions.shotStats[currentShotTestIndex].made = true;
    }

    void instantiateMoney(float value)
    {
        // set value of shot
        moneyClone.GetComponent<PickupObject>().updateMoneyValue(value);
        Random random = new Random();
        float distance = (float)(random.NextDouble());
        Vector3 tempPos = new Vector3(transform.position.x + distance, 0, transform.position.z - 2);
        Instantiate(moneyClone, tempPos, Quaternion.identity);
    }

    private void updateShotMadeBasketBallStats()
    {
        // first thing, update shot made total
        _basketBallStats.ShotMade++;

        // ==================== consecutive shots logic ==============================

        // get current state of shots made/attempted
        _currentShotMade = (int)_basketBallStats.ShotMade;
        _currentShotAttempts = (int)_basketBallStats.ShotAttempt;

        // if current is == expected made/attempt, increment consecutive and not a 2 point shot
        // 
        if (_currentShotMade == _expectedShotMade
            && _currentShotAttempts == _expectedShotAttempts
            && !_basketBallState.TwoAttempt)
        {
            _consecutiveShotsMade++;
            // increment expected values for next shot
            _expectedShotMade = _currentShotMade + 1;
            _expectedShotAttempts = _currentShotAttempts + 1;
            //Debug.Log(" Consecutive shots : " + ConsecutiveShotsMade);
        }
        // else, not consecutive shot. get current, increment for next expected consecutive shot
        else
        {
            _consecutiveShotsMade = 1;
            // increment expected values for next shot
            _expectedShotMade = _currentShotMade + 1;
            _expectedShotAttempts = _currentShotAttempts + 1;
        }
        // if current consecutive greater than previous high consecutive
        if (ConsecutiveShotsMade > _basketBallStats.MostConsecutiveShots)
        {
            _basketBallStats.MostConsecutiveShots = ConsecutiveShotsMade;
        }

        // ==================== point total logic ==============================
        // if not 3/4/all point contest
        if (!GameRules.instance.GameModeThreePointContest
            && !GameRules.instance.GameModeFourPointContest
            && !GameRules.instance.GameModeAllPointContest)
        {
            if (_basketBallState.TwoAttempt)
            {
                //Debug.Log("twopointer made");
                _basketBallStats.TwoPointerMade++;
                _basketBallStats.TotalPoints += 2;
            }

            if (_basketBallState.ThreeAttempt)
            {
                _basketBallStats.ThreePointerMade++;
                // if consecutive > 5 and game mode for 'Total Points+'
                if (ConsecutiveShotsMade >= GameRules.instance.InThePocketActivateValue && GameOptions.gameModeSelectedId == 15)
                {
                    _basketBallStats.TotalPoints += 4;
                }
                else
                {
                    _basketBallStats.TotalPoints += 3;
                }
            }

            if (_basketBallState.FourAttempt)
            {
                _basketBallStats.FourPointerMade++;
                // if consecutive > 5 and game mode for 'Total Points+'
                if (ConsecutiveShotsMade >= GameRules.instance.InThePocketActivateValue && GameOptions.gameModeSelectedId == 15)
                {
                    _basketBallStats.TotalPoints += 6;
                }
                else
                {
                    _basketBallStats.TotalPoints += 4;
                }

            }
            if (_basketBallState.SevenAttempt)
            {
                _basketBallStats.SevenPointerMade++;
                // if consecutive > 5 and game mode for 'Total Points+'
                if (ConsecutiveShotsMade >= GameRules.instance.InThePocketActivateValue && GameOptions.gameModeSelectedId == 15)
                {
                    _basketBallStats.TotalPoints += 10;
                }
                else
                {
                    _basketBallStats.TotalPoints += 7;
                }
            }
        }
        else
        {
            // if player is on marker and marker enabled
            if (_basketBallState.PlayerOnMarkerOnShoot
                && GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].MarkerEnabled)
            {
                int pointsScored = 0;
                //_basketBallStats.ShotMade++;
                // if moneyball
                if (_basketBallState.TwoAttempt)
                {
                    _basketBallStats.TwoPointerMade++;
                    pointsScored = 2;
                }

                if (_basketBallState.ThreeAttempt)
                {
                    _basketBallStats.ThreePointerMade++;
                    pointsScored = 3;
                }

                if (_basketBallState.FourAttempt)
                {
                    _basketBallStats.FourPointerMade++;
                    pointsScored = 4;
                }

                if (_basketBallState.SevenAttempt)
                {
                    _basketBallStats.SevenPointerMade++;
                    pointsScored = 7;
                }
                // if moneyball / last shot on marker (5/5)
                if (GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].ShotAttempt == 5)
                {
                    _basketBallStats.TotalPoints += (pointsScored * 2);
                }
                // not last shot on marker (1-4/5)
                else
                {
                    _basketBallStats.TotalPoints += pointsScored;
                }
            }
        }
        // moneyball stats
        if (_basketBallState.MoneyBallEnabledOnShoot)
        {
            _basketBallStats.MoneyBallMade++;
        }

        // ==================== requires position markers logic ==============================
        if (_basketBallState.PlayerOnMarkerOnShoot)
        {
            // if money ball enabled
            if (_basketBallState.MoneyBallEnabledOnShoot)
            {
                int max = GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].MaxShotMade;
                GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].ShotMade = max;
            }
            // no money ball, update current shot marker stats
            else
            {
                GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].ShotMade++;
            }
        }
    }

    public int ConsecutiveShotsMade { get => _consecutiveShotsMade; }
}

