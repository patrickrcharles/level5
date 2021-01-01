﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class BasketBall : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    PlayerController playerState;
    Rigidbody rigidbody;
    AudioSource audioSource;
    CharacterProfile characterProfile;
    BasketBallState basketBallState;
    BasketBallStats basketBallStats;
    Animator anim;

    GameObject basketBallSprite;
    GameObject basketBallPosition;
    GameObject basketBallTarget;

    GameObject player;
    GameObject dropShadow;

    Text scoreText;
    Text shootProfileText;
    GameObject uiStatsBackground;

    float releaseVelocityY;
    float accuracy;
    float lastShotDistance;
    float maxBasketballSpeed = 0f;

    bool playHitRimSound;
    bool locked;
    bool facingRight = true;

    float accuracyModifierX;
    private float accuracyModifierY;
    private float accuracyModifierZ;

    public static BasketBall instance;

    // =========================================================== Start() ========================================================
    // Use this for initialization
    void Start()
    {
        instance = this;

        player = GameLevelManager.instance.Player;
        playerState = GameLevelManager.instance.PlayerState;
        rigidbody = GetComponent<Rigidbody>();
        basketBallStats = GameLevelManager.instance.Basketball.GetComponent<BasketBallStats>();
        basketBallState = GameLevelManager.instance.Basketball.GetComponent<BasketBallState>();
        characterProfile = GameLevelManager.instance.Player.GetComponent<CharacterProfile>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        //basketBallShotMade = GameObject.Find("basketBallMadeShot").GetComponent<BasketBallShotMade>();
        anim = GetComponentInChildren<Animator>();

        //basketball drop shadow
        dropShadow = transform.root.Find("drop shadow").gameObject;

        //objects
        basketBallSprite = GameObject.Find("basketball_sprite"); //used to reset drop shadow. on launch, euler position gets changed
        basketBallPosition = player.transform.Find("basketBall_position").gameObject;

        //bool flags
        basketBallState.Locked = false;
        basketBallState.CanPullBall = true;
        playHitRimSound = true;

        //todo: move to game manager
        UiStatsEnabled = false;

        // cap ball speed
        maxBasketballSpeed = 25f;
        // check for ui stats ON/OFF. i know this is sloppy. its just a quick test
        if (GameObject.Find("ui_stats") != null)
        {
            shootProfileText = GameObject.Find("shooterProfileTextObject").GetComponent<Text>();
            scoreText = GameObject.Find("shootStatsTextObject").GetComponent<Text>();
            uiStatsBackground = GameObject.Find("textBackground");

            if (UiStatsEnabled)
            {
                updateScoreText();
                updateShooterProfileText();
                uiStatsBackground.SetActive(true);
            }
            else
            {
                scoreText.text = "";
                shootProfileText.text = "";
                uiStatsBackground.SetActive(false); 
            }
        }
        InvokeRepeating("checkIsBallFacingGoal", 0, 0.5f);
        InvokeRepeating("displayUiStats", 0, 0.5f);

        if (GameOptions.EnemiesOnlyEnabled)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 20, transform.position.z);
            rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }

    // =========================================================== Update() ========================================================

    // Update is called once per frame
    private void Update()
    {
        // get speed for basketball animation
        //checkIsBallFacingGoal();
        if (!GameOptions.EnemiesOnlyEnabled)
        {
            if (rigidbody.velocity.magnitude > maxBasketballSpeed && !basketBallState.InAir)
            {
                rigidbody.velocity = rigidbody.velocity.normalized * maxBasketballSpeed;
            }
            // drop shadow lock to bball transform on the ground
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f, transform.root.position.z);

            // change this to reduce opacity
            if (!playerState.hasBasketball)
            {
                spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // is about 100 % transparent
                dropShadow.SetActive(true);
                basketBallState.CanPullBall = true;
                basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
            }

            //if player has ball and hasnt shot
            if (playerState.hasBasketball 
                && playerState.currentState!= playerState.dunkState)//&& !basketBallState.Thrown)
            {
                basketBallState.CanPullBall = false;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
                dropShadow.SetActive(false);
                playerState.setPlayerAnim("hasBasketball", true);
                //playerState.setPlayerAnim("walking", false);
                playerState.setPlayerAnim("moonwalking", false);

                // move basketball to launch position and disable sprite
                transform.position = new Vector3(basketBallState.BasketBallPosition.transform.position.x,
                    basketBallState.BasketBallPosition.transform.position.y,
                    basketBallState.BasketBallPosition.transform.position.z);
            }
        }
    }

    public void checkIsBallFacingGoal()
    {
        anim.SetFloat("speed", rigidbody.velocity.sqrMagnitude);
        //bballRelativePositioning = GameLevelManager.instance.BasketballRimVector.x - rigidbody.position.x;

        if (rigidbody.velocity.x > 0 && !facingRight)
        {
            Flip();
        }

        if (rigidbody.velocity.x < 0f && facingRight)
        {
            Flip();
        }
    }

    void Flip()
    {
        facingRight = !facingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
    }

    private void updateShooterProfileText()
    {
        shootProfileText.text = characterProfile.PlayerDisplayName + "\n"
                                + "2 point : " + (characterProfile.Accuracy2Pt) + "\n"
                                + "3 point : " + (characterProfile.Accuracy3Pt) + "\n"
                                + "4 point : " + (characterProfile.Accuracy4Pt) + "\n"
                                + "7 point : " + (characterProfile.Accuracy7Pt) + "\n"
                                + "release : " + characterProfile.Release + "\n"
                                + "range : " + characterProfile.Range + "\n"
                                + "speed : " + characterProfile.RunSpeed + "\n"
                                + "jump : " + characterProfile.JumpForce + "\n"
                                + "luck : " + characterProfile.Luck;
    }

    // =========================================================== Collisions ========================================================

    private void OnCollisionEnter(Collision other)
    {
        // collision : basketball + rim
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim")
            && playHitRimSound
            && !playerState.hasBasketball)
        {
            playHitRimSound = false;
            audioSource.PlayOneShot(SFXBB.instance.basketballHitRim);
            basketBallState.CanPullBall = true;
            basketBallState.Thrown = false;
            basketBallState.Locked = false;
        }
        // collision : basketball + ground
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground")
            && !playerState.hasBasketball)
        {
            basketBallState.CanPullBall = true;
            //reset rotation
            transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
            dropShadow.transform.rotation = Quaternion.Euler(90, 0, 0);
            basketBallState.Locked = false;
            basketBallState.Thrown = false;
            audioSource.PlayOneShot(SFXBB.instance.basketballBounce);
        }
        // collision : basketball + fence
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("fence")
            && !playerState.hasBasketball)
        {
            audioSource.PlayOneShot(SFXBB.instance.basketballHitFence);
            basketBallState.CanPullBall = true;
            basketBallState.Thrown = false;
            basketBallState.Locked = false;
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim") && !playHitRimSound)
        {
            playHitRimSound = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if basketball enters player hitbox
        if (gameObject.CompareTag("basketball") 
            && other.gameObject.CompareTag("playerHitbox")
            && !basketBallState.Thrown)
        {
            playerState.hasBasketball = true;
            basketBallState.Thrown = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // if basketball exits player hitbox
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("playerHitbox") &&
            basketBallState.Thrown)
        {
            basketBallState.Thrown = true;
        }
    }

    // =================================== shoot ball function =======================================

    public void shootBasketBall()
    {

        // set side or front shooting animation
        if (playerState.facingFront) // facing straight toward bball goal
        {
            playerState.setPlayerAnimTrigger("basketballShootFront");
        }
        else // side of goal, relative postion
        {
            playerState.setPlayerAnimTrigger("basketballShoot");
        }

        // reset ball rotation
        // #NOTE : hopefully this check works for issue : ball is hot but doesnt go toward goal
        transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));

        //================== anim stuff 
        // check for and set money ball
        if (GameRules.instance.MoneyBallEnabled)
        {
            basketBallState.MoneyBallEnabledOnShoot = true;
            PlayerStats.instance.Money -= 5; // moneyball spent
            BasketBallStats.MoneyBallAttempts++;
            GameRules.instance.MoneyBallEnabled = false;
        }
        else
        {
            basketBallState.MoneyBallEnabledOnShoot = false;
        }

        //// let basketball rim know current statistics of made/attempt for every shot
        //// this is more determining consecutive shots
        //// on make, if made = made+1 && attempt = attempt +1 ---> consecutive++
        //BasketBallShotMade.instance.setCurrentShotsMadeAttempted((int)basketBallStats.ShotMade, (int)basketBallStats.ShotAttempt);

        // let basketball state know what type of shot is attempted
        updateBasketBallStateShotTypeOnShoot();

        // player on shot marker and game mode requires markers
        if (basketBallState.PlayerOnMarker && GameRules.instance.PositionMarkersRequired)
        {
            // on shoot. 
            basketBallState.PlayerOnMarkerOnShoot = true;
            basketBallState.OnShootShotMarkerId = basketBallState.CurrentShotMarkerId;
            // update shot attempt stat for marker position shot from
            GameRules.instance.BasketBallShotMarkersList[basketBallState.OnShootShotMarkerId].ShotAttempt++;

            if (GameRules.instance.MoneyBallEnabled)
            {
                basketBallState.MoneyBallEnabledOnShoot = true;
                basketBallStats.MoneyBallAttempts++;
            }
        }
        //calculate shot distance 
        Vector3 tempPos = new Vector3(basketBallState.BasketBallTarget.transform.position.x,
            0, basketBallState.BasketBallTarget.transform.position.z);
        float tempDist = Vector3.Distance(tempPos, basketBallPosition.transform.position);
        lastShotDistance = tempDist;

        // wait for shot meter to finish calculations for accurate launch values
        StartCoroutine(LaunchBasketBall());

        //reset state flags
        basketBallState.Thrown = true;
        CallBallToPlayer.instance.Locked = false;
    }

    public void updateBasketBallStateShotTypeOnShoot()
    {
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
        // update total; shot attempst
        basketBallStats.ShotAttempt++;
    }

    // =================================== Launch ball function =======================================
    void Launch(GameObject ballPositionAtLaunch)
    {
        Vector3 projectileXZPos = ballPositionAtLaunch.transform.position;
        Vector3 targetXZPos = new Vector3(basketBallState.BasketBallTarget.transform.position.x,
            basketBallState.BasketBallTarget.transform.position.y,
            basketBallState.BasketBallTarget.transform.position.z);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha;
        // check last shot distance. if > 500, angle = 55 degrees. almost impossible to make shot 
        // >500ft with shoot angle 45-52 that most characters have 
        if (LastShotDistance >= 500)
        {
            tanAlpha = Mathf.Tan(55 * Mathf.Deg2Rad);
        }
        else
        {
            tanAlpha = Mathf.Tan(characterProfile.ShootAngle * Mathf.Deg2Rad);
        }
        float H = targetXZPos.y - projectileXZPos.y;
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        bool critical = rollForCriticalShotChance(characterProfile.Luck);

        string shotMeterMessage = "";
        string shotMeterMessageX = "";
        string shotMeterMessageY = "";
        string shotMeterMessageZ = "";

        // if rolled critical
        if (critical)
        {
            accuracyModifierX = 0;
            accuracyModifierY = 0;
            shotMeterMessage = "critical";
        }
        // if >= 95 and NOT critical (release stat factored in)
        if (playerState.Shotmeter.SliderValueOnButtonPress >= 95
            && !critical)
        {
            accuracyModifierX = 0;
            accuracyModifierY = getReleaseModifier();
            accuracyModifierZ = 0;
            shotMeterMessage = ">= 95";
            shotMeterMessageY = "+ release modifier";
        }
        // NOT critical and NOT >= 95 (get X, Y modifiers)
        if (playerState.Shotmeter.SliderValueOnButtonPress < 95
            && !critical)
        {
            accuracyModifierX = getAccuracyModifier();
            accuracyModifierY = getReleaseModifier();

            shotMeterMessage = "< 95";
            shotMeterMessageX = "+ accuracy modifier";
            shotMeterMessageY = "+ release modifier";
        }

        // range modifier always factors in
        accuracyModifierZ = getRangeModifier();

        if(accuracyModifierZ != 0)
        {
            shotMeterMessageZ = "+ range modifer";
        }

        // set shot meter message
        if (shotMeterMessage != null)
        {
            shotMeterMessage = shotMeterMessage + "\n" + shotMeterMessageX + "\n" + shotMeterMessageY + "\n" + shotMeterMessageZ;
        }
        else
        {
            shotMeterMessage = shotMeterMessageX + "\n" + shotMeterMessageY + "\n" + shotMeterMessageZ;
        }

        // if no mods, cheerleader action
        if (accuracyModifierX == 0 && accuracyModifierY == 0 && accuracyModifierZ == 0)
        {
            if (BehaviorNpcCritical.instance != null)
            {
                BehaviorNpcCritical.instance.playAnimationCriticalSuccesful();
            }
            // shot meter message 
            if(critical)
            {
                shotMeterMessage = "swish + critical";
            }
            else
            {
                shotMeterMessage = "swish";
            }
        }

        ShotMeter.instance.displaySliderMessageText(shotMeterMessage);

        float xVector = 0 + accuracyModifierX;
        float yVector = Vy + accuracyModifierY; // + (accuracyModifier * shooterProfile.shootYVariance);
        float zVector = Vz - accuracyModifierZ; //+ accuracyModifierZ; // + (accuracyModifier * shooterProfile.shootZVariance);

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(xVector, yVector, zVector);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);
        //Debug.Log("globalVelocity : " + globalVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rigidbody.velocity = globalVelocity;
        playerState.hasBasketball = false;
        playerState.setPlayerAnim("hasBasketball", false);

        // analytics
        AnaylticsManager.PlayerShoot(playerState.Shotmeter.SliderValueOnButtonPress);
    }

    // ============================ Functions and Properties ==========================================

    // wair for shotmeter value calculation, launch ball
    IEnumerator LaunchBasketBall()
    {
        // get position of ball when shot
        GameObject currentBallPosition = player.transform.Find("basketBall_position").gameObject;
        // wait for shot meter to finish
        yield return new WaitUntil(() => playerState.Shotmeter.MeterEnded == false);
        //launch ball to goal      
        Launch(basketBallPosition);
    }

    // ========================== shot accuracy functions ==========================================
    bool rollForCriticalShotChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);
        if (percent <= maxPercent)
        {
            BasketBallStats.CriticalRolled++;
            return true;
        }

        return false;
    }
    bool rollForCriticalRangeChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);

        if (percent <= maxPercent)
        {
            return true;
        }
        return false;
    }

    bool rollForCriticalReleaseChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);

        if (percent <= maxPercent)
        {
            return true;
        }
        return false;
    }

    private float getAccuracyModifier()
    {
        int direction = getRandomPositiveOrNegative();
        int slider = Mathf.CeilToInt(playerState.Shotmeter.SliderValueOnButtonPress);

        float sliderModifer = (100 - slider) * 0.025f;
        float accuracyModifier = 0;

        if (basketBallState.TwoPoints)
        {
            accuracyModifier = (100 - characterProfile.Accuracy2Pt) * 0.01f;
        }

        if (basketBallState.ThreePoints)
        {
            accuracyModifier = (100 - characterProfile.Accuracy3Pt) * 0.02f;
        }

        if (basketBallState.FourPoints)
        {
            accuracyModifier = (100 - characterProfile.Accuracy4Pt) * 0.01f;
        }

        if (basketBallState.SevenPoints)
        {
            accuracyModifier = (100 - characterProfile.Accuracy7Pt) * 0.01f;
        }

        return ((sliderModifer + (accuracyModifier * sliderModifer)) * direction);
    }


    private float getRangeModifier()
    {
        int direction = 1;
        // range divided by distance to get %
        // ex. range 50 ft / shot distance 100 = 50% change of reaching rim
        float rangeAccuracy = (float)(characterProfile.Range / (lastShotDistance * 6));
        float modifier = (rangeAccuracy * direction);

        // send max percent change
        // should 1/2 of modifer
        float maxChance = modifier * 100;


        if (modifier >= 1 || rollForCriticalRangeChance(maxChance))
        {
            return 0;
        }
        else
        {
            return modifier;
        }
    }

    private float getReleaseModifier()
    {
        int direction = getRandomPositiveOrNegative();
        float accuracyModifier = 0;

        accuracyModifier = (100 - characterProfile.Release) * 0.01f;

        // get random chance for removing release modifier
        // ex if release = 85, 15% chance to remove modifiers
        if (rollForCriticalReleaseChance(characterProfile.Release))
        {
            return 0;
        }
        else
        {
            return ((accuracyModifier * 0.75f)) * direction;
        }
    }

    private int getRandomPositiveOrNegative()
    {
        var Random = new Random();
        List<int> list = new List<int> { 1, -1 };
        int finder = Random.Next(list.Count); //Then you just use this; nameDisplayString = names[finder];
        int shotDirectionModifier = list[finder];

        return shotDirectionModifier;
    }

    // ========================== ui display ===============================

    public bool displayUiStats()
    {
        //Debug.Log("displayUiStats() -- UiStatsEnabled : "+ UiStatsEnabled);
        if (UiStatsEnabled)
        {
            updateScoreText();
            updateShooterProfileText();
            uiStatsBackground.SetActive(true);
            return true;
        }
        else
        {
            scoreText.text = "";
            shootProfileText.text = "";
            uiStatsBackground.SetActive(false);
            return false;
        }
    }

    public void toggleUiStats()
    {
        UiStatsEnabled = !UiStatsEnabled;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "ui stats = " + UiStatsEnabled;

        // turn off text display after 5 seconds
        StartCoroutine(turnOffMessageLogDisplayAfterSeconds(3));
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
                         (Math.Round(basketBallStats.LongestShotMade, 2)).ToString("0.00") + " ft." + "\n" +
                         "criticals rolled : " + basketBallStats.CriticalRolled + " / " + basketBallStats.ShotAttempt
                         + "  " + getCriticalPercentage().ToString("0.00") + "%\n"
                         + "consecutive shots made : " + BasketBallShotMade.instance.ConsecutiveShotsMade + "\n"
                         + "current exp : " + basketBallStats.getExperienceGainedFromSession();
    }

    // ============================= convert to percentages ======================================
    // * NOTE : cast to float has to be (float) num1 / num2 to work;
    //  this format will not work for some reason -- (float)(num1 / num2 to work);
    public float getCriticalPercentage()
    {
        if (basketBallStats.CriticalRolled > 0)
        {
            float accuracy = (float)basketBallStats.CriticalRolled / basketBallStats.ShotAttempt;
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
            accuracy = (float)basketBallStats.ShotMade / basketBallStats.ShotAttempt;
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
            float accuracy = (float)basketBallStats.TwoPointerMade / basketBallStats.TwoPointerAttempts;
            return (accuracy * 100);
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
            float accuracy = (float)basketBallStats.ThreePointerMade / basketBallStats.ThreePointerAttempts;
            return (accuracy * 100);
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
            float accuracy = (float)basketBallStats.FourPointerMade / basketBallStats.FourPointerAttempts;
            return (accuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getSevenPointAccuracy()
    {
        if (basketBallStats.SevenPointerAttempts > 0)
        {
            float accuracy = (float)basketBallStats.SevenPointerMade / basketBallStats.SevenPointerAttempts;
            return (accuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    // ============================= getters/ setters ======================================

    public float LastShotDistance { get => lastShotDistance; set => lastShotDistance = value; }
    public BasketBallStats BasketBallStats => basketBallStats;
    public BasketBallState BasketBallState => basketBallState;
    public bool UiStatsEnabled { get; private set; }
    public GameObject BasketBallPosition { get => basketBallPosition; set => basketBallPosition = value; }
    public Rigidbody Rigidbody { get => rigidbody; set => rigidbody = value; }
}