using System;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class BasketBallAutoPlay : MonoBehaviour
{
    //GameObject score; //reference to the ScoreText gameobject, set in editor
    AudioClip basket; //reference to the basket sound
    GameObject player; // , dropShadow;


    //Vector3 dropShadowPosition;
    GameObject basketBallSprite, playerDunkPos;

    PlayerControllerAutoPlay playerState;
    new Rigidbody rigidbody;

    private BasketballStateAutoPlay basketballState;
    private BasketBallStats basketBallStats;


    //Physics variables
    float velocityFinal;
    float velocityInitialY, velocityInitialX, velocityInitialD;
    [SerializeField]
    float displacement, Zdistance;
    float acceleration = -9.8f;
    float time;
    float launchAngle;
    //float point1, point2;


    //[SerializeField]
    // float distanceOfShot;

    public GameObject TextObject;
    Text scoreText;

    public float longestShot { get; set; }
    public float lastShotDistance { get; set; }

    public GameObject shootProfile;
    Text shootProfileText;

    //public float shotAttempt, shotMade;


    [Range(20f, 70f)]
    public float _angle;

    bool playHitRimSound;
    public AudioClip shotMiss;
    AudioSource audioSource;
    [SerializeField]
    SpriteRenderer spriteRenderer;

    GameObject basketBallPosition;
    GameObject basketBallTarget;


    ShooterProfile shooterProfile;
    float releaseVelocityY;

    private float _playerRigidBody;

    public float accuracy;
    public float twoAccuracy;
    public float threeAccuracy;
    public float fourAccuracy;
    public bool addAccuracyModifier;

    private bool locked;

    // Use this for initialization
    void Start()
    {
        player = gameManagerAutoPlay.instance.player;
        playerState = gameManagerAutoPlay.instance.playerState;
        rigidbody = GetComponent<Rigidbody>();

        basketBallStats = GetComponent<BasketBallStats>();
        basketballState = GetComponent<BasketballStateAutoPlay>();


        //basketball drop shadow
        //dropShadow = transform.root.Find("drop shadow").gameObject;
        //dropShadowPosition = dropShadow.transform.position;
        audioSource = GetComponent<AudioSource>();
        basketBallSprite = GameObject.Find("basketball_sprite");
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        shooterProfile = gameManagerAutoPlay.instance.player.GetComponent<ShooterProfile>();

        basketBallPosition = player.transform.Find("basketBall_position").gameObject;
        //displacement = Vector3.Distance(basketBallTarget.transform.position, gameObject.transform.position);
        //shooterProfile = GameObject.Find("shooterProfileTextObject").GetComponent<Text>();


        longestShot = 0;
        playerDunkPos = GameObject.Find("dunk_transform");
        basketballState.Locked = false;
        basketballState.CanPullBall = true;
        addAccuracyModifier = true;

        //shootProfileText.text = "ball distance : " + (Math.Round(basketballState.BallDistanceFromRim, 2)) + "\n"
        //                        + "shot distance : " + (Math.Round(basketballState.BallDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
        //                        + "shooter : Dr Blood\n"
        //                        + "2 point accuracy : " + ((1 - shooterProfile.accuracy2pt) * 100) + "\n"
        //                        + "3 point accuracy : " + ((1 - shooterProfile.accuracy3pt) * 100) + "\n"
        //                        + "4 point accuracy : " + ((1 - shooterProfile.accuracy4pt) * 100);

    }

    // Update is called once per frame
    void Update()
    {
        //dropShadowPosition = dropShadow.transform.position;
        ////dropShadow.transform.position = new Vector3(transform.position.x, 0.01f, transform.root.position.z);
        //dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, 0.02f, dropShadow.transform.position.z);
        ////dropShadow.transform.rotation = Quaternion.Euler(90, 0, 0);

        //basketballState.BallDistanceFromRim = Vector3.Distance(transform.position, basketBallTarget.transform.position);
        // change this to reduce opacity
        if (!playerState.hasBasketball)
        {
            spriteRenderer.enabled = true;
            //dropShadow.SetActive(true);
        }

        //updateScoreText();

        //shootProfileText.text = "distance : " + (Math.Round(basketballState.BallDistanceFromRim, 2)) + "\n"
        //                        + "shot distance : " + (Math.Round(basketballState.BallDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
        //                        + "shooter : Dr Blood\n"
        //                        + "2 point accuracy : " + shooterProfile.accuracy2pt + "\n"
        //                        + "3 point accuracy : " + shooterProfile.accuracy3pt + "\n"
        //                        + "4 point accuracy : " + shooterProfile.accuracy4pt;





        // =========== this is the conditional for auto shooting =============================
        // this will work great for  playing against CPU

        //if (playerState.inAir && playerState.hasBasketball && playerState.jumpPeakReached)
        //{
        //}
        if (playerState.hasBasketball && !basketballState.Thrown)
        {
            //Debug.Log("if (playerState.hasBasketball && !basketballState.Thrown)");
            transform.position = new Vector3(basketballState.BasketBallPosition.transform.position.x,
                basketballState.BasketBallPosition.transform.position.y,
                basketballState.BasketBallPosition.transform.position.z);
            //Debug.Log("playerState.grounded" + playerState.grounded);
            spriteRenderer.enabled = false;
            //dropShadow.SetActive(false);
            playerState.setPlayerAnim("hasBasketball", true);
            //playerState.setPlayerAnim("walking", false);
            playerState.setPlayerAnim("moonwalking", false);
            //if (playerState.grounded)
            //{
            //    spriteRenderer.enabled = false;
            //    dropShadow.SetActive(false);
            //    playerState.setPlayerAnim("hasBasketball", true);
            //    //playerState.setPlayerAnim("walking", false);
            //    playerState.setPlayerAnim("moonwalking", false);
            //}
            //else
            //{
            //    //basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
            //    //playerState.setPlayerAnim("hasBasketball", false);
            //}
        }

        if (!playerState.hasBasketball)
        {
            basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
        }


        if (playerState.inAir 
            && playerState.hasBasketball
            && playerState.jumpPeakReached
            && !basketballState.Locked)
        {
            basketballState.Locked = true;
            Debug.Log(" shoot");
            releaseVelocityY = playerState.rigidBodyYVelocity;
            Debug.Log("releaseVelocityY : " + releaseVelocityY);

            Launch();

            //playerState.hasBasketball = false;
            playerState.setPlayerAnim("hasBasketball", false);

            if (playerState.facingFront) // facing straight toward bball goal
            {
                playerState.setPlayerAnimTrigger("basketballShootFront");
            }
            else // side of goal, relative postion
            {
                playerState.setPlayerAnimTrigger("basketballShoot");
            }

            // identify is in 2 or 3 point range for stat counters
            if (basketballState.TwoPoints)
            {
                //Debug.Log(" 2 point attempt");
                basketballState.TwoAttempt = true;
                basketBallStats.TwoPointerAttempts++;
                //Debug.Log("TwoAttempt :: " + TwoAttempt);
            }
            if (basketballState.ThreePoints)
            {
                //Debug.Log(" 3 point attempt");
                basketballState.ThreeAttempt = true;
                basketBallStats.ThreePointerAttempts++;
                //Debug.Log("ThreeAttempt :: "+ ThreeAttempt);
            }
            if (basketballState.FourPoints)
            {
                //Debug.Log(" 3 point attempt");
                basketballState.FourAttempt = true;
                basketBallStats.FourPointerAttempts++;
                //Debug.Log("ThreeAttempt :: "+ ThreeAttempt);
            }

            //launch ball to goal      

            //Jessica might take a photo
            //behavior_jessica.instance.playAnimationTakePhoto();

            //calculate shot distance 
            Vector3 tempPos = new Vector3(basketballState.BasketBallTarget.transform.position.x,
                0, basketballState.BasketBallTarget.transform.position.z);
            float tempDist = Vector3.Distance(tempPos, basketBallPosition.transform.position);
            lastShotDistance = tempDist;

            basketballState.Thrown = true;
            basketballState.InAir = true;
        }

    }
    void OnCollisionEnter(Collision other)
    {
        //Debug.Log("Collision Enter : " + transform.name + " other.name : " + other.gameObject.name);

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim") && playHitRimSound)
        {
            playHitRimSound = false;
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
            audioSource.PlayOneShot(SFXBB.Instance.basketballHitRim);
            basketballState.CanPullBall = true;
        }
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
            basketballState.InAir = false;
            //basketballState.Grounded = true;
            basketballState.CanPullBall = true;
            basketballState.Locked = false;
            audioSource.PlayOneShot(SFXBB.Instance.basketballBounce);

        }

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("fence"))
        {
            //Debug.Log("$$$$$$$$$$$$$$$$$");
            //Debug.Log("Collision Enter : " + transform.name + " other.name : " + other.gameObject.name);
            basketballState.InAir = false;
            //Grounded = true;
            basketballState.Locked = false;
            audioSource.PlayOneShot(SFXBB.Instance.basketballHitFence);
            basketballState.CanPullBall = true;
        }
    }

    void OnCollisionExit(Collision other)
    {

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim") && !playHitRimSound)
        {
            playHitRimSound = true;
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
        }
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
            basketballState.Grounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("COLLISION between : " + transform.root.name + " and name: " + other.name+ "| tag: "+other.tag);
        if (gameObject.CompareTag("basketball") && other.CompareTag("Player"))
        {
            Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.name);
            //playerState.hasBasketball = true;
            playerState.turnOffMoonWalkAudio();
            basketballState.CanPullBall = false;

        }
        if (gameObject.CompareTag("basketball") && other.CompareTag("ground"))
        {
            basketballState.Grounded = true;
            basketballState.Thrown = false;
        }

        //if (gameObject.name.Contains("basketBallMadeShot") && other.gameObject.name == "basketball")
        //{
        //    shotMade++;
        //}

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            basketballState.Dunk = true;
        }
        //if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        //{
        //    facingFront = true;
        //    playerState.setPlayerAnim("basketballFacingFront", true);
        //}
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("Player") && basketballState.Thrown)
        {
            //Debug.Log("$$$$$$$$$$$$$$$$$$");
            playerState.hasBasketball = false;
            basketballState.Locked = false;
        }

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            basketballState.Dunk = false;
        }
        //if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        //{
        //    facingFront = false;
        //    playerState.setPlayerAnim("basketballFacingFront", false);
        //}

    }

    void Launch()
    {
        basketBallStats.ShotAttempt++;
       Debug.Log("Launch()");

        //Debug.Log("Launch() : addAccuracyModifier : " + addAccuracyModifier);
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.

        //Vector3 playerProjectileXZPos  = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 targetXZPos = new Vector3(basketballState.BasketBallTarget.transform.position.x,
            basketballState.BasketBallTarget.transform.position.y,
            basketballState.BasketBallTarget.transform.position.z);

       //Debug.Log("projectileXZPos :: " + projectileXZPos);
       Debug.Log("ball position" + transform.position);
       // Debug.Log("targetXZPos :: " + targetXZPos);
       //Debug.Log("target position"+ basketballState.BasketBallTarget.transform.position);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        //float Rplayer = Vector3.Distance(playerProjectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(_angle * Mathf.Deg2Rad);
        float H = basketballState.BasketBallTarget.transform.position.y - transform.position.y;
        //float Hplayer = basketBallTarget.transform.position.y - player.transform.position.y;

        //Debug.Log("R :: " + R);
        //Debug.Log("G :: " + G);
        //Debug.Log("tanAlpha :: " + tanAlpha);
        //Debug.Log("H :: " + H);
       //Debug.Log("_angle :: " + _angle);

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        //Debug.Log("Vz :: " + Vz);
        //Debug.Log("Vy :: " + Vy);

        //Debug.Log("old x : " + 0 + " y : " + Vy + " z : " + Vz);;

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
            //Debug.Log("addAccuracyModifier On");
            localVelocity = new Vector3(xVector, yVector, zVector);
        }
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        rigidbody.velocity = globalVelocity;

    }

    bool rollForCriticalShotChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);
        //Debug.Log("percent : " + percent + " maxPercent : " + maxPercent);
        if (percent <= shooterProfile.CriticalPercent)
        {
            //Debug.Log("critical shot rolled");
            return true;
        }
        return false;

    }
    private float getAccuracyModifier()
    {
        int direction = getRandomPositiveOrNegtaive();
        float accuracyModifier = 1;
        if (basketballState.TwoPoints) { accuracyModifier = (100 - shooterProfile.Accuracy2Pt) * 0.01f; }
        if (basketballState.ThreePoints) { accuracyModifier = (100 - shooterProfile.Accuracy3Pt) * 0.01f; }
        if (basketballState.FourPoints) { accuracyModifier = (100 - shooterProfile.Accuracy4Pt) * 0.01f; }

        Debug.Log("accuracyModifier : " + accuracyModifier);
        return (accuracyModifier ) * direction;
    }



    private int getRandomPositiveOrNegtaive()
    {
        var Random = new Random();
        List<int> list = new List<int> { 1, -1 };
        int finder = Random.Next(list.Count); //Then you just use this; nameDisplayString = names[finder];
        int shotDirectionModifier = list[finder];

        return shotDirectionModifier;
    }

    //public void addToShotMade(int value)
    //{
    //    shotMade += value;
    //}
    public bool getDunk()
    {
        return basketballState.Dunk;
    }


    public void toggleAddAccuracyModifier()
    {
        //Debug.Log("toggleAddAccuracyModifier()");
        //if (addAccuracyModifier == true)
        //{
        //   //Debug.log("$$$$$$$$$$$$$$$$ FALSE");
        //    addAccuracyModifier = false;
        //}
        //else
        //{
        //   //Debug.log("$$$$$$$$$$$$$$$$ TRUE");
        //    addAccuracyModifier = true;
        //}

        addAccuracyModifier = !addAccuracyModifier;
        //Debug.Log("toggleAddAccuracyModifier() : addAccuracyModifier : " + addAccuracyModifier);

        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "accuracy modifier = " + addAccuracyModifier;
        //messageLog.instance.toggleMessageDisplay();
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
                         + "longest shot distance : " + (Math.Round(longestShot, 2) * 6f).ToString("0.00") + " ft.";
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

}



