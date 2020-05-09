using System;
using UnityEngine;
using Random = System.Random;


public class BasketBallShotMade : MonoBehaviour
{

    BasketBallState _basketBallState;
    BasketBallStats _basketBallStats;

    public int currentShotTestIndex;

    AudioSource audioSource;
    public GameObject rimSprite;
    Animator anim;
    PlayerController playerState;
    bool isColliding;

    const string moneyPrefabPath = "Prefabs/objects/money";
    [SerializeField]
    private GameObject moneyClone;

    //public AudioClip shotMissed;

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
            _basketBallStats.TotalDistance += BasketBall.instance.LastShotDistance;

            // is this the longest shot made?
            if (BasketBall.instance.LastShotDistance > _basketBallStats.LongestShotMade)
            {
                _basketBallStats.LongestShotMade = BasketBall.instance.LastShotDistance;
            }

            anim.Play("madeshot");

            if (_basketBallState.TwoAttempt)
            {
                _basketBallStats.TotalPoints += 2;
                _basketBallStats.TwoPointerMade++;
                _basketBallStats.ShotMade++;
                //instantiateMoney(0);

            }
            if (_basketBallState.ThreeAttempt)
            {
                _basketBallStats.TotalPoints += 3;
                _basketBallStats.ThreePointerMade++;
                _basketBallStats.ShotMade++;
                //instantiateMoney(0.5f);
            }
            if (_basketBallState.FourAttempt)
            {
                _basketBallStats.TotalPoints += 4;
                _basketBallStats.FourPointerMade++;
                _basketBallStats.ShotMade++;
                //instantiateMoney(1f);
            }
            if (_basketBallState.SevenAttempt)
            {
                _basketBallStats.TotalPoints += 7;
                _basketBallStats.SevenPointerMade++;
                _basketBallStats.ShotMade++;
                //instantiateMoney(2f);
            }

            if (_basketBallState.PlayerOnMarkerOnShoot)
            {
                Debug.Log("_id  : " + GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId]);
                GameRules.instance.BasketBallShotMarkersList[_basketBallState.OnShootShotMarkerId].ShotMade++;
            }
            _basketBallState.TwoAttempt = false;
            _basketBallState.ThreeAttempt = false;
            _basketBallState.FourAttempt = false;
            _basketBallState.SevenAttempt = false;

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
        Debug.Log("distance : "+ distance);
        Vector3 tempPos = new Vector3(transform.position.x + distance, 0, transform.position.z - 2);
        Instantiate(moneyClone, tempPos, Quaternion.identity);
    }
}

