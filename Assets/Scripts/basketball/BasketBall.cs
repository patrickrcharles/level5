using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class BasketBall : MonoBehaviour
{

    SpriteRenderer spriteRenderer;
    playercontrollerscript playerState;
    new Rigidbody rigidbody;
    AudioSource audioSource;

    Vector3 dropShadowPosition;

    GameObject basketBallSprite;
    GameObject playerDunkPos;
    GameObject bplayerDunkPos;
    GameObject basketBallPosition;
    GameObject basketBallTarget;

    GameObject player;
    GameObject dropShadow;

    shooterProfile shooterProfile;
    BasketBallState basketBallState;
    BasketBallStats basketBallStats;

    // text objects
    public GameObject TextObject;
    Text scoreText;

    public GameObject shootProfile;
    Text shootProfileText;


    [Range(20f, 70f)]
    public float _angle;

    float releaseVelocityY;
    float _playerRigidBody;
    float accuracy;
    float twoAccuracy;
    float threeAccuracy;
    float fourAccuracy;

    private float lastShotDistance;

    bool addAccuracyModifier;
    bool playHitRimSound;
    bool locked;

    //public BasketballTestStats testStats;
    public BasketBallTestStatsConclusions testConclusions;
    public int currentTestStatsIndex = 0;
    public BasketBallShotMade basketBallShotMade;

    public static BasketBall instance;

    // Use this for initialization
    void Start()
    {
        instance = this;

        player = GameLevelManager.instance.Player;
        playerState = GameLevelManager.instance.PlayerState;
        rigidbody = GetComponent<Rigidbody>();

        basketBallStats = GetComponent<BasketBallStats>();
        basketBallState = GetComponent<BasketBallState>();

        //basketball drop shadow
        dropShadow = transform.root.Find("drop shadow").gameObject;
        dropShadowPosition = dropShadow.transform.position;

        audioSource = GetComponent<AudioSource>();
        basketBallSprite = GameObject.Find("basketball_sprite");
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        basketBallPosition = player.transform.Find("basketBall_position").gameObject;

        shooterProfile = GameLevelManager.instance.Player.GetComponent<shooterProfile>();
        shootProfileText = GameObject.Find("shooterProfileTextObject").GetComponent<Text>();

        TextObject = GameObject.Find("shootStatsTextObject");
        scoreText = TextObject.GetComponent<Text>();

        playerDunkPos = GameObject.Find("dunk_transform");
        basketBallState.Locked = false;
        basketBallState.CanPullBall = true;
        addAccuracyModifier = true;

        basketBallStats.PlayerName = shooterProfile.PlayerObjectName;
        basketBallStats.PlayerId = shooterProfile.PlayerId;

        shootProfileText.text = "ball distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2)) + "\n"
                                + "shot distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
                                + "shooter : Dr Blood\n"
                                + "2 point accuracy : " + ((1 - shooterProfile.Accuracy2Pt) * 100) + "\n"
                                + "3 point accuracy : " + ((1 - shooterProfile.Accuracy3Pt) * 100) + "\n"
                                + "4 point accuracy : " + ((1 - shooterProfile.Accuracy4Pt) * 100);

        // for test data
        //testStats = GetComponent<BasketballTestStats>();
        testConclusions = GetComponent<BasketBallTestStatsConclusions>();
        basketBallShotMade = GameObject.Find("basketBallMadeShot").GetComponent<BasketBallShotMade>();

    }


    // Update is called once per frame
    void Update()
    {
        dropShadowPosition = dropShadow.transform.position;
        //dropShadow.transform.position = new Vector3(transform.position.x, 0.01f, transform.root.position.z);
        dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, 0.01f, dropShadow.transform.position.z);

        // change this to reduce opacity
        if (!playerState.hasBasketball)
        {
            spriteRenderer.enabled = true;
            dropShadow.SetActive(true);
        }

        updateScoreText();
        shootProfileText.text = "distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2)) + "\n"
                                + "shot distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
                                + "shooter : " + shooterProfile.PlayerDisplayName+ "\n"
                                + "2 point accuracy : " + shooterProfile.Accuracy2Pt + "\n"
                                + "3 point accuracy : " + shooterProfile.Accuracy3Pt + "\n"
                                + "4 point accuracy : " + shooterProfile.Accuracy4Pt;

        // =========== this is the conditional for auto shooting =============================
        // this will work great for  playing against CPU
        
        //if player has ball and hasnt shot
        if (playerState.hasBasketball && !basketBallState.Thrown)
        {
            transform.position = new Vector3(basketBallState.BasketBallPosition.transform.position.x,
                basketBallState.BasketBallPosition.transform.position.y,
                basketBallState.BasketBallPosition.transform.position.z);
            // if grounded
            if (playerState.grounded)
            {
                spriteRenderer.enabled = false;
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

            //launch ball to goal      
            Launch();


            //Jessica might take a photo
            behavior_jessica.instance.playAnimationTakePhoto();

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
    void OnCollisionEnter(Collision other)
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

    void OnCollisionExit(Collision other)
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

    void OnTriggerEnter(Collider other)
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

    void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("playerHitbox") && basketBallState.Thrown)
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
        float tanAlpha = Mathf.Tan(_angle * Mathf.Deg2Rad);
        float H = basketBallState.BasketBallTarget.transform.position.y - transform.position.y;
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        // accuracy modifier clogic
        float accuracyModifierX = 0;
        if (rollForCriticalShotChance(shooterProfile.CriticalPercent))
        {
            accuracyModifierX = 0;
        }
        else
        {
            accuracyModifierX = getAccuracyModifier();
        }

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

        //BasketballTestStats testStats = new BasketballTestStats();

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
        //currentTestStatsIndex = (testConclusions.shotStats.Count)-1;
        //basketBallShotMade.currentShotTestIndex = currentTestStatsIndex;
    }

    // ================================================================================================

    bool rollForCriticalShotChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);
        //Debug.Log("percent : " + percent + " maxPercent : " + maxPercent);
        if (percent <= shooterProfile.CriticalPercent)
        {
            Debug.Log("********************** critical shot rolled");
            return true;
        }
        return false;

    }
    private float getAccuracyModifier()
    {
        int direction = getRandomPositiveOrNegtaive();
        float accuracyModifier = 1;
        if (basketBallState.TwoPoints) { accuracyModifier = (100 - shooterProfile.Accuracy2Pt) * 0.01f; }
        if (basketBallState.ThreePoints) { accuracyModifier = (100 - shooterProfile.Accuracy3Pt) * 0.01f; }
        if (basketBallState.FourPoints) { accuracyModifier = (100 - shooterProfile.Accuracy4Pt) * 0.01f; }

        return (accuracyModifier / 1.6f) * direction;
    }

    private int getRandomPositiveOrNegtaive()
    {
        var Random = new Random();
        List<int> list = new List<int> { 1, -1 };
        int finder = Random.Next(list.Count); //Then you just use this; nameDisplayString = names[finder];
        int shotDirectionModifier = list[finder];

        return shotDirectionModifier;
    }

    public bool getDunk()
    {
        return basketBallState.Dunk;
    }

    public void toggleAddAccuracyModifier()
    {
        addAccuracyModifier = !addAccuracyModifier;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "accuracy modifier = " + addAccuracyModifier;
    }

    public void updateScoreText()
    {
        scoreText.text = "shots  : " + basketBallStats.ShotMade + " / " + basketBallStats.ShotAttempt + "\n"
                         + "accuracy : " + getTotalPointAccuracy() + "%\n"
                         + "points : " + basketBallStats.TotalPoints + "\n"
                         + "2 pointers : " + basketBallStats.TwoPointerMade + " / " + basketBallStats.TwoPointerAttempts + "\n" //+ " accuracy : " + getTwoPointAccuracy() + "%\n"
                         + "3 pointers : " + basketBallStats.ThreePointerMade + " / " + basketBallStats.ThreePointerAttempts + "\n"// +" accuracy : " + getThreePointAccuracy() + "%\n"
                         + "4 pointers : " + basketBallStats.FourPointerMade + " / " + basketBallStats.FourPointerAttempts + "\n"// + " accuracy : " + getFourPointAccuracy() + "%\n"
                         + "last shot distance : " + (Math.Round(lastShotDistance, 2) * 6f).ToString("0.00") + " ft." + "\n"
                         + "longest shot distance : " + (Math.Round(basketBallStats.LongestShotMade, 2) * 6f).ToString("0.00") + " ft.";
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
            twoAccuracy = basketBallStats.FourPointerMade / basketBallStats.FourPointerAttempts;
            return (fourAccuracy * 100);
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
}



