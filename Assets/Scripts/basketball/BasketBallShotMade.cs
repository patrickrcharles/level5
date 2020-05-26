using System;
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
        playerState = GameLevelManager.Instance.PlayerState;

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


            audioSource.PlayOneShot(SFXBB.Instance.basketballNetSwish);

            // add to total shot distance made total
            _basketBallStats.TotalDistance += (BasketBall.instance.LastShotDistance * 6);

            // is this the longest shot made?
            if (BasketBall.instance.LastShotDistance > _basketBallStats.LongestShotMade)
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

        if (_basketBallState.TwoAttempt)
        {
            _basketBallStats.TotalPoints += 2;
            _basketBallStats.TwoPointerMade++;
            _basketBallStats.ShotMade ++;
        }

        if (_basketBallState.ThreeAttempt)
        {
            _basketBallStats.TotalPoints += 3;
            _basketBallStats.ThreePointerMade++;
            _basketBallStats.ShotMade ++;
        }

        if (_basketBallState.FourAttempt)
        {
            _basketBallStats.TotalPoints += 4;
            _basketBallStats.FourPointerMade++;
            _basketBallStats.ShotMade ++;
        }

        if (_basketBallState.SevenAttempt)
        {
            _basketBallStats.TotalPoints += 7;
            _basketBallStats.SevenPointerMade++;
            _basketBallStats.ShotMade ++;
        }

        if (_basketBallState.MoneyBallEnabledOnShoot)
        {
            _basketBallStats.MoneyBallMade++;
        }

        if (_basketBallState.PlayerOnMarkerOnShoot)
        {
            //Debug.Log("if(_basketBallState.PlayerOnMarkerOnShoot)");
            //Debug.Log("GameRules.instance.MoneyBallEnabled : " + GameRules.instance.MoneyBallEnabled);
            if (_basketBallState.MoneyBallEnabledOnShoot)
            {
                //Debug.Log("clear marker");
                int max = GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].MaxShotMade;
                GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].ShotMade = max;
                //Debug.Log("max : "+ max);
            }
            else
            {
                //Debug.Log(" else : shot made");
                GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].ShotMade++;
            }
        }

        // get current state of made/attempt
        _currentShotMade = (int)_basketBallStats.ShotMade;
        _currentShotAttempts = (int)_basketBallStats.ShotAttempt;

        // if current is == expected made/attempt, increment consecutive
        if (_currentShotMade == _expectedShotMade && _currentShotAttempts == _expectedShotAttempts)
        {
            Debug.Log(" expected = current");
            ConsecutiveShotsMade++;
            _expectedShotMade = _currentShotMade + 1;
            _expectedShotAttempts = _currentShotAttempts + 1;
        }
        // else, not consecutive shot. get current, increment for next expected consecutive shot
        else
        {
            Debug.Log(" else");
            ConsecutiveShotsMade = 1;
            _expectedShotMade = _currentShotMade + 1;
            _expectedShotAttempts = _currentShotAttempts + 1;  
        }
        if(ConsecutiveShotsMade > _basketBallStats.MostConsecutiveShots)
        {
            _basketBallStats.MostConsecutiveShots = ConsecutiveShotsMade;
        }
    }

    public int ConsecutiveShotsMade { get => _consecutiveShotsMade; set => _consecutiveShotsMade = value; }
}

