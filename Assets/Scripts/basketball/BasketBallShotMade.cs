using System;
using UnityEngine;


public class BasketBallShotMade : MonoBehaviour
{

    BasketBallState _basketBallState;
    BasketBallStats _basketBallStats;

    int currentShotTestIndex;

    AudioSource audioSource;
    //SpriteRenderer spriteRenderer;
    public GameObject rimSprite;
    Animator anim;
    playercontrollerscript playerState;
    bool isColliding;

    //public AudioClip shotMissed;

    // Use this for initialization
    void Start()
    {
        _basketBallState = BasketBall.instance.GetComponent<BasketBallState>();
        _basketBallStats = BasketBall.instance.GetComponent<BasketBallStats>();
        audioSource = GetComponent<AudioSource>();
        anim = rimSprite.GetComponent<Animator>();
        playerState = GameLevelManager.instance.PlayerState;
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
            // is this the longest shot made?
            if (BasketBall.instance.LastShotDistance > _basketBallStats.LongestShotMade)
            {
                _basketBallStats.LongestShotMade = BasketBall.instance.LastShotDistance * 6f;
            }
            anim.Play("madeshot");

            if (_basketBallState.TwoAttempt)
            {
                _basketBallStats.TotalPoints += 2;
                _basketBallStats.TwoPointerMade++;
                _basketBallStats.ShotMade++;

            }
            if (_basketBallState.ThreeAttempt)
            {
                _basketBallStats.TotalPoints += 3;
                _basketBallStats.ThreePointerMade++;
                _basketBallStats.ShotMade++;
            }
            if (_basketBallState.FourAttempt)
            {
                _basketBallStats.TotalPoints += 4;
                _basketBallStats.FourPointerMade++;
                _basketBallStats.ShotMade++;

            }
            _basketBallState.TwoAttempt = false;
            _basketBallState.ThreeAttempt = false;
            _basketBallState.FourAttempt = false;
        }
        // update onscreen ui stats
        BasketBall.instance.updateScoreText();

        //// object test shot data in list
        //BasketBall.instance.testConclusions.shotStats[currentShotTestIndex].made = true;
    }
}

