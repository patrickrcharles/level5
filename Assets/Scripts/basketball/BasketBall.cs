﻿using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

// ReSharper disable InconsistentNaming
// ReSharper disable All


public class BasketBall : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PlayerController playerState;
    new Rigidbody rigidbody;
    AudioSource audioSource;

    private Vector3 dropShadowPosition;

    GameObject basketBallSprite;
    GameObject playerDunkPos;
    GameObject bplayerDunkPos;
    GameObject basketBallPosition;
    GameObject basketBallTarget;

    GameObject player;
    GameObject dropShadow;
    [SerializeField]
    ShooterProfile shooterProfile;
    [SerializeField]
    BasketBallState basketBallState;
    [SerializeField]
    BasketBallStats basketBallStats;

    // text objects
    public GameObject TextObject;
    Text scoreText;

    public GameObject shootProfile;
    Text shootProfileText;


    [Range(20f, 70f)] public float _angle;

    float releaseVelocityY;
    float _playerRigidBody;
    float accuracy;
    float twoAccuracy;
    float threeAccuracy;
    float fourAccuracy;
    float sevenAccuracy;

    private float lastShotDistance;

    bool addAccuracyModifier;

    private bool isCheating = false;
    public bool IsCheating => isCheating;

    bool playHitRimSound;
    bool locked;
    [SerializeField] private bool uiStatsEnabled;

    //public BasketballTestStats testStats;
    //public BasketBallTestStatsConclusions testConclusions;
    public int currentTestStatsIndex = 0;
    public BasketBallShotMade basketBallShotMade;

    public static BasketBall instance;

    [SerializeField]
    float accuracyModifierX;

    // =========================================================== Start() ========================================================
    // Use this for initialization
    void Start()
    {
        instance = this;

        player = GameLevelManager.Instance.Player;
        playerState = GameLevelManager.Instance.PlayerState;
        rigidbody = GetComponent<Rigidbody>();
        basketBallStats = GameLevelManager.Instance.Basketball.GetComponent<BasketBallStats>();
        basketBallState = GameLevelManager.Instance.Basketball.GetComponent<BasketBallState>();
        shooterProfile = GameLevelManager.Instance.Player.GetComponent<ShooterProfile>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        basketBallShotMade = GameObject.Find("basketBallMadeShot").GetComponent<BasketBallShotMade>();

        //basketball drop shadow
        dropShadow = transform.root.Find("drop shadow").gameObject;
        dropShadowPosition = dropShadow.transform.position;

        //objects
        basketBallSprite = GameObject.Find("basketball_sprite"); //used to reset drop shadow. on launch, euler position gets changed
        basketBallPosition = player.transform.Find("basketBall_position").gameObject;
        //playerDunkPos = GameObject.Find("dunk_transform"); //not used yet

        //bool flags
        basketBallState.Locked = false;
        basketBallState.CanPullBall = true;
        addAccuracyModifier = true;
        playHitRimSound = true;

        // for test data
        //testStats = GetComponent<BasketballTestStats>();
        //testConclusions = GetComponent<BasketBallTestStatsConclusions>();

        //todo: move to game manager
        uiStatsEnabled = false;
        // check for ui stats ON/OFF. i know this is sloppy. its just a quick test
        if (GameObject.Find("ui_stats") != null)
        {
            shootProfileText = GameObject.Find("shooterProfileTextObject").GetComponent<Text>();
            TextObject = GameObject.Find("shootStatsTextObject");
            scoreText = TextObject.GetComponent<Text>();
            if (uiStatsEnabled)
            {
                shootProfileText.text = "ball distance : " +
                                        (Math.Round(basketBallState.BallDistanceFromRim, 2)).ToString("0.00") + "\n"
                                        + "shot distance : " +
                                        (Math.Round(basketBallState.BallDistanceFromRim, 2) * 6f).ToString("0.00") +
                                        " ft.\n"
                                        + "shooter : Dr Blood\n"
                                        + "2 point accuracy : " + ((1 - shooterProfile.Accuracy2Pt) * 100) + "\n"
                                        + "3 point accuracy : " + ((1 - shooterProfile.Accuracy3Pt) * 100) + "\n"
                                        + "4 point accuracy : " + ((1 - shooterProfile.Accuracy4Pt) * 100) + "\n"
                                        + "7 point accuracy : " + ((1 - shooterProfile.Accuracy7Pt) * 100);
            }
        }
    }

    // =========================================================== Update() ========================================================

    // Update is called once per frame
    private void Update()
    {
        // drop shadow lock to bball transform on the ground
        dropShadow.transform.position = new Vector3(transform.root.position.x, 0.02f, transform.root.position.z);

        // change this to reduce opacity
        if (!playerState.hasBasketball)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // is about 100 % transparent
            //spriteRenderer.enabled = true;
            dropShadow.SetActive(true);
        }

        if (!addAccuracyModifier && PlayerData.instance != null)
        {
            PlayerData.instance.IsCheating = true;
        }

        if (uiStatsEnabled)
        {
            updateScoreText();
            shootProfileText.text = "distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2)) + "\n"
                                    + "shot distance : " +
                                    (Math.Round(basketBallState.BallDistanceFromRim, 2) * 6f).ToString("0.00") +
                                    " ft.\n"
                                    + "shooter : " + shooterProfile.PlayerDisplayName + "\n"
                                    + "2 point accuracy : " + shooterProfile.Accuracy2Pt + "\n"
                                    + "3 point accuracy : " + shooterProfile.Accuracy3Pt + "\n"
                                    + "4 point accuracy : " + shooterProfile.Accuracy4Pt + "\n"
                                    + "7 point accuracy : " + shooterProfile.Accuracy7Pt;
        }
        else
        {
            scoreText.text = "";
            shootProfileText.text = "";
        }

        //if player has ball and hasnt shot
        if (playerState.hasBasketball && !basketBallState.Thrown)
        {
            transform.position = new Vector3(basketBallState.BasketBallPosition.transform.position.x,
                basketBallState.BasketBallPosition.transform.position.y,
                basketBallState.BasketBallPosition.transform.position.z);
            // if grounded
            if (playerState.grounded)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                dropShadow.SetActive(false);
                playerState.setPlayerAnim("hasBasketball", true);
                //playerState.setPlayerAnim("walking", false);
                playerState.setPlayerAnim("moonwalking", false);
            }
            else
            {
                //basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
                //playerState.setPlayerAnim("hasBasketball", false);
            }
        }

        // if player doesnt have ball, reset rotation i think
        if (!playerState.hasBasketball)
        {
            basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
        }

        // if has ball, is in air, and pressed shoot button.
        if (playerState.inAir
            && playerState.hasBasketball
            && InputManager.GetButtonDown("Fire1")
            //&& playerState.jumpPeakReached
            && !basketBallState.Locked)
        {
            basketBallState.Locked = true;
            releaseVelocityY = playerState.rigidBodyYVelocity;
            playerState.hasBasketball = false;
            playerState.setPlayerAnim("hasBasketball", false);

            // set side or front shooting animation
            if (playerState.facingFront) // facing straight toward bball goal
            {
                playerState.setPlayerAnimTrigger("basketballShootFront");
            }
            else // side of goal, relative postion
            {
                playerState.setPlayerAnimTrigger("basketballShoot");
            }

            // identify is in 2 or 3 point range for stat counters
            if (basketBallState.TwoPoints)
            {
                basketBallState.TwoAttempt = true;
                basketBallStats.TwoPointerAttempts++;
            }

            if (basketBallState.ThreePoints)
            {
                basketBallState.ThreeAttempt = true;
                basketBallStats.ThreePointerAttempts++;
            }

            if (basketBallState.FourPoints)
            {
                basketBallState.FourAttempt = true;
                basketBallStats.FourPointerAttempts++;
            }

            if (basketBallState.SevenPoints)
            {
                basketBallState.SevenAttempt = true;
                basketBallStats.SevenPointerAttempts++;
            }
            if (basketBallState.PlayerOnMarker)
            {
                // on shoot. 
                basketBallState.PlayerOnMarkerOnShoot = true;
                basketBallState.OnShootShotMarkerId = basketBallState.CurrentShotMarkerId;
                // update shot attempt stat for marker position shot from
                GameRules.instance.BasketBallShotMarkersList[basketBallState.OnShootShotMarkerId].ShotAttempt++;
            }

            //launch ball to goal      
            Launch();


            //calculate shot distance 
            Vector3 tempPos = new Vector3(basketBallState.BasketBallTarget.transform.position.x,
                0, basketBallState.BasketBallTarget.transform.position.z);
            float tempDist = Vector3.Distance(tempPos, basketBallPosition.transform.position);
            lastShotDistance = tempDist;

            //reset state flags
            basketBallState.Thrown = true;
            basketBallState.InAir = true;
            basketBallState.Locked = false;
        }
    }

    // =========================================================== Collisions ========================================================

    private void OnCollisionEnter(Collision other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim") && playHitRimSound)
        {
            playHitRimSound = false;
            audioSource.PlayOneShot(SFXBB.Instance.basketballHitRim);
            basketBallState.CanPullBall = true;
        }

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            basketBallState.InAir = false;
            basketBallState.Grounded = true;
            //reset rotation
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            dropShadow.transform.rotation = Quaternion.Euler(90, 0, 0);
            basketBallState.CanPullBall = true;
            audioSource.PlayOneShot(SFXBB.Instance.basketballBounce);
        }

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("fence"))
        {
            audioSource.PlayOneShot(SFXBB.Instance.basketballHitFence);
            basketBallState.CanPullBall = true;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim") && !playHitRimSound)
        {
            playHitRimSound = true;
        }

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            basketBallState.Grounded = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.CompareTag("playerHitbox"))
        {
            playerState.hasBasketball = true;
            playerState.turnOffMoonWalkAudio();
            basketBallState.CanPullBall = false;
        }

        if (gameObject.CompareTag("basketball") && other.CompareTag("ground"))
        {
            basketBallState.Grounded = true;
        }

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            basketBallState.Dunk = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("playerHitbox") &&
            basketBallState.Thrown)
        {
            basketBallState.Thrown = false;
            playerState.hasBasketball = false;
            basketBallState.Locked = false;
        }

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            basketBallState.Dunk = false;
        }
    }

    // =================================== Launch ball function =======================================
    void Launch()
    {
        // shot attempted
        basketBallStats.ShotAttempt++;

        Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 targetXZPos = new Vector3(basketBallState.BasketBallTarget.transform.position.x,
            basketBallState.BasketBallTarget.transform.position.y,
            basketBallState.BasketBallTarget.transform.position.z);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(shooterProfile.ShootAngle * Mathf.Deg2Rad);
        float H = basketBallState.BasketBallTarget.transform.position.y - transform.position.y;
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // accuracy modifier logic
        //accuracyModifierX;
        //if (rollForCriticalShotChance(shooterProfile.CriticalPercent))
        //{
        //    accuracyModifierX = 0;
        //    //Jessica might take a photo
        //    BehaviorJessica.instance.playAnimationTakePhoto();
        //}
        //else
        //{
        //    accuracyModifierX = getAccuracyModifier();
        //}

        float xVector = 0 + accuracyModifierX;
        float yVector = Vy; // + (accuracyModifier * shooterProfile.shootYVariance);
        float zVector = Vz; // + (accuracyModifier * shooterProfile.shootZVariance);

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0, Vy, Vz);

        // turn on/off accuracy modifier
        if (addAccuracyModifier)
        {
            localVelocity = new Vector3(xVector, yVector, zVector);
        }

        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rigidbody.velocity = globalVelocity;


        // ================================== for SHOT TEST STATS ====================================

        //BasketballTestStats testStats = gameObject.GetComponent<BasketballTestStats>();

        //testStats.AccuracyModifier = accuracyModifierX;
        //testStats.Distance = lastShotDistance;
        //testStats.LocalVelocity = localVelocity;
        //testStats.GlobalVelocity = globalVelocity;
        //testStats.ReleaseVelocity = releaseVelocityY;
        //if (basketBallState.TwoPoints)
        //{
        //    testStats.Accuracy = shooterProfile.Accuracy2Pt;
        //    testStats.Two = true;
        //}
        //if (basketBallState.ThreePoints)
        //{
        //    testStats.Accuracy = shooterProfile.Accuracy3Pt;
        //    testStats.Three = true;
        //}
        //if (basketBallState.FourPoints)
        //{
        //    testStats.Accuracy = shooterProfile.Accuracy4Pt;
        //    testStats.Four = true;
        //}

        //testConclusions.shotStats.Add(testStats);
        //currentTestStatsIndex = (testConclusions.shotStats.Count) - 1;
        //basketBallShotMade.currentShotTestIndex = currentTestStatsIndex;

        // ================================================================================================
    }

    // =========================================================== Functions and Properties ========================================================
    public bool UiStatsEnabled => uiStatsEnabled;
    public BasketBallState BasketBallState => basketBallState;

    bool rollForCriticalShotChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);
        //Debug.Log("percent : " + percent + " maxPercent : " + maxPercent);
        if (percent <= maxPercent)
        {
            //Debug.Log("********************** critical shot rolled");
            BasketBallStats.CriticalRolled++;
            return true;
        }

        return false;
    }

    private float getAccuracyModifier()
    {
        //int direction = getRandomPositiveOrNegative();
        int direction = 1; //for testing to do stat analysis
        float accuracyModifier = 1;
        if (basketBallState.TwoPoints)
        {
            accuracyModifier = (100 - shooterProfile.Accuracy2Pt) * 0.01f;
        }

        if (basketBallState.ThreePoints)
        {
            accuracyModifier = (100 - shooterProfile.Accuracy3Pt) * 0.01f;
        }

        if (basketBallState.FourPoints)
        {
            accuracyModifier = (100 - shooterProfile.Accuracy4Pt) * 0.01f;
        }

        if (basketBallState.SevenPoints)
        {
            accuracyModifier = (100 - shooterProfile.Accuracy7Pt) * 0.01f;
        }

        return (accuracyModifier) * direction;
    }

    private int getRandomPositiveOrNegative()
    {
        var Random = new Random();
        List<int> list = new List<int> { 1, -1 };
        int finder = Random.Next(list.Count); //Then you just use this; nameDisplayString = names[finder];
        int shotDirectionModifier = list[finder];

        return shotDirectionModifier;
    }

    //public bool getDunk()
    //{
    //    return basketBallState.Dunk;
    //}

    public void toggleAddAccuracyModifier()
    {
        addAccuracyModifier = !addAccuracyModifier;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "accuracy modifier = " + addAccuracyModifier + "\nhigh score saving disabled";

        // turn off text display after 5 seconds
        StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
    }

    public void toggleUiStats()
    {
        uiStatsEnabled = !uiStatsEnabled;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "ui stats = " + uiStatsEnabled;

        // turn off text display after 5 seconds
        StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

    public void updateScoreText()
    {
        scoreText.text = "shots  : " + basketBallStats.ShotMade + " / " + basketBallStats.ShotAttempt + "  " +
                         getTotalPointAccuracy().ToString("0.00") + "\n"
                         + "points : " + basketBallStats.TotalPoints + "\n"
                         + "2 pointers : " + basketBallStats.TwoPointerMade + " / " +
                         basketBallStats.TwoPointerAttempts + "  " + getTwoPointAccuracy().ToString("0.00") + "%\n"
                         + "3 pointers : " + basketBallStats.ThreePointerMade + " / " +
                         basketBallStats.ThreePointerAttempts + "  " + getThreePointAccuracy().ToString("0.00") + "%\n"
                         + "4 pointers : " + basketBallStats.FourPointerMade + " / " +
                         basketBallStats.FourPointerAttempts + "  : " + getFourPointAccuracy().ToString("0.00") + "%\n"
                         + "7 pointers : " + basketBallStats.SevenPointerMade + " / " +
                         basketBallStats.SevenPointerAttempts + "  " + getSevenPointAccuracy().ToString("0.00") + "%\n"
                         + "last shot distance : " + (Math.Round(lastShotDistance, 2) * 6f).ToString("0.00") + " ft." +
                         "\n"
                         + "longest shot distance : " +
                         (Math.Round(basketBallStats.LongestShotMade, 2) * 6f).ToString("0.00") + " ft." + "\n" +
                         "criticals rolled : " + basketBallStats.CriticalRolled + " / " + basketBallStats.ShotAttempt + "  " + getCriticalPercentage().ToString("0.00") + "%";
    }

    public float getCriticalPercentage()
    {
        if (basketBallStats.CriticalRolled > 0)
        {
            accuracy = basketBallStats.CriticalRolled / basketBallStats.ShotAttempt;
            return (accuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getTotalPointAccuracy()
    {
        if (basketBallStats.ShotAttempt > 0)
        {
            accuracy = basketBallStats.ShotMade / basketBallStats.ShotAttempt;
            return (accuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getTwoPointAccuracy()
    {
        if (basketBallStats.TwoPointerAttempts > 0)
        {
            twoAccuracy = basketBallStats.TwoPointerMade / basketBallStats.TwoPointerAttempts;
            return (twoAccuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getThreePointAccuracy()
    {
        if (basketBallStats.ThreePointerAttempts > 0)
        {
            threeAccuracy = basketBallStats.ThreePointerMade / basketBallStats.ThreePointerAttempts;
            return (threeAccuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getFourPointAccuracy()
    {
        if (basketBallStats.FourPointerAttempts > 0)
        {
            fourAccuracy = basketBallStats.FourPointerMade / basketBallStats.FourPointerAttempts;
            return (fourAccuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getSevenPointAccuracy()
    {
        if (basketBallStats.FourPointerAttempts > 0)
        {
            sevenAccuracy = basketBallStats.SevenPointerMade / basketBallStats.SevenPointerAttempts;
            return (sevenAccuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float LastShotDistance
    {
        get => lastShotDistance;
        set => lastShotDistance = value;
    }

    public BasketBallStats BasketBallStats
    {
        get => basketBallStats;
    }
}