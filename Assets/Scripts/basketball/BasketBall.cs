using System;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class BasketBall : MonoBehaviour
{
    //GameObject score; //reference to the ScoreText gameobject, set in editor
    AudioClip basket; //reference to the basket sound

    GameObject player, dropShadow;

    [SerializeField]
    Vector3 dropShadowPosition;
    GameObject basketBallSprite, playerDunkPos;

    playercontrollerscript playerState;
    new Rigidbody rigidbody;

    [SerializeField]
    BasketBallState basketBallState;
    BasketBallStats basketBallStats;

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

    [SerializeField]
    public GameObject shootProfile;
    [SerializeField]
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


    shooterProfile shooterProfile;
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
        player = GameLevelManager.instance.player;
        playerState = GameLevelManager.instance.playerState;
        rigidbody = GetComponent<Rigidbody>();

        basketBallStats = GetComponent<BasketBallStats>();
        basketBallState = GetComponent<BasketBallState>();

        Debug.Log(basketBallState);

        //basketball drop shadow
        dropShadow = transform.root.Find("drop shadow").gameObject;
        dropShadowPosition = dropShadow.transform.position;
        audioSource = GetComponent<AudioSource>();
        basketBallSprite = GameObject.Find("basketball_sprite");
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();


        basketBallPosition = player.transform.Find("basketBall_position").gameObject;
        //displacement = Vector3.Distance(basketBallTarget.transform.position, gameObject.transform.position);

        shooterProfile = GameLevelManager.instance.player.GetComponent<shooterProfile>();
        shootProfileText = GameObject.Find("shooterProfileTextObject").GetComponent<Text>();

        TextObject = GameObject.Find("shootStatsTextObject");
        scoreText = TextObject.GetComponent<Text>();

        longestShot = 0;
        playerDunkPos = GameObject.Find("dunk_transform");
        basketBallState.Locked = false;
        basketBallState.CanPullBall = true;
        addAccuracyModifier = true;

        shootProfileText.text = "ball distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2)) + "\n"
                                + "shot distance : " + (Math.Round(basketBallState.BallDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
                                + "shooter : Dr Blood\n"
                                + "2 point accuracy : " + ((1 - shooterProfile.Accuracy2Pt) * 100) + "\n"
                                + "3 point accuracy : " + ((1 - shooterProfile.Accuracy3Pt) * 100) + "\n"
                                + "4 point accuracy : " + ((1 - shooterProfile.Accuracy4Pt) * 100);

    }

    void FixedUpdate()
    {
        //Quaternion rot = Quaternion.Euler(0, rigidbody.rotation.eulerAngles.y, 0);
        //rigidbody.rotation = rot;
    }

    // Update is called once per frame
    void Update()
    {
        dropShadowPosition = dropShadow.transform.position;
        //dropShadow.transform.position = new Vector3(transform.position.x, 0.01f, transform.root.position.z);
        dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, 0.01f, dropShadow.transform.position.z);

        //basketballState.BallDistanceFromRim = Vector3.Distance(transform.position, basketBallTarget.transform.position);
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

        //if (playerState.inAir && playerState.hasBasketball && playerState.jumpPeakReached)
        //{
        //}
        if (playerState.hasBasketball && !basketBallState.Thrown)
        {
            //Debug.Log("if (playerState.hasBasketball && !basketballState.Thrown)");
            transform.position = new Vector3(basketBallState.BasketBallPosition.transform.position.x,
                basketBallState.BasketBallPosition.transform.position.y,
                basketBallState.BasketBallPosition.transform.position.z);
            //Debug.Log("playerState.grounded" + playerState.grounded);
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

        if (!playerState.hasBasketball)
        {
            basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
        }


        if (playerState.inAir 
            && playerState.hasBasketball 
            && InputManager.GetButtonDown("Fire1")
            && !basketBallState.Locked)
        {
            basketBallState.Locked = true;
            releaseVelocityY = playerState.rigidBodyYVelocity;
           //Debug.log("shoot button pressed : " + releaseVelocityY);
            //Debug.Log("$$$$$$$$$$$$$$$$$$$ shoot : addAccuracyModifier : " + addAccuracyModifier);
            //Debug.Log("shoot ball");
            playerState.hasBasketball = false;
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
            if (basketBallState.TwoPoints)
            {
                //Debug.Log(" 2 point attempt");
                basketBallState.TwoAttempt = true;
                basketBallStats.TwoPointerAttempts++;
                //Debug.Log("TwoAttempt :: " + TwoAttempt);
            }
            if (basketBallState.ThreePoints)
            {
                //Debug.Log(" 3 point attempt");
                basketBallState.ThreeAttempt = true;
                basketBallStats.ThreePointerAttempts++;
                //Debug.Log("ThreeAttempt :: "+ ThreeAttempt);
            }
            if (basketBallState.FourPoints)
            {
                //Debug.Log(" 3 point attempt");
                basketBallState.FourAttempt = true;
                basketBallStats.FourPointerAttempts++;
                //Debug.Log("ThreeAttempt :: "+ ThreeAttempt);
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

            basketBallState.Thrown = true;
            basketBallState.InAir = true;
            basketBallState.Locked = false;


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
            basketBallState.CanPullBall = true;
        }
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
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
            //Debug.Log("$$$$$$$$$$$$$$$$$");
            //Debug.Log("Collision Enter : " + transform.name + " other.name : " + other.gameObject.name);
            //inAir = false;
            //Grounded = true;
            audioSource.PlayOneShot(SFXBB.Instance.basketballHitFence);
            basketBallState.CanPullBall = true;
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
            basketBallState.Grounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.CompareTag("playerHitbox"))
        {
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.name);
            playerState.hasBasketball = true;
            playerState.turnOffMoonWalkAudio();
            basketBallState.CanPullBall = false;

        }
        if (gameObject.CompareTag("basketball") && other.CompareTag("ground"))
        {
            basketBallState.Grounded = true;
        }

        //if (gameObject.name.Contains("basketBallMadeShot") && other.gameObject.name == "basketball")
        //{
        //    shotMade++;
        //}

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            basketBallState.Dunk = true;
        }
        //if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        //{
        //    facingFront = true;
        //    playerState.setPlayerAnim("basketballFacingFront", true);
        //}
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("playerHitbox") && basketBallState.Thrown)
        {
            //Debug.Log("$$$$$$$$$$$$$$$$$$");
            basketBallState.Thrown = false;
            playerState.hasBasketball = false;
            basketBallState.Locked = false;
        }

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            basketBallState.Dunk = false;
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
       //Debug.log("Launch()");

        //Debug.Log("Launch() : addAccuracyModifier : " + addAccuracyModifier);
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.

        //Vector3 playerProjectileXZPos  = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 targetXZPos = new Vector3(basketBallState.BasketBallTarget.transform.position.x,
            basketBallState.BasketBallTarget.transform.position.y,
            basketBallState.BasketBallTarget.transform.position.z);

       //Debug.log("projectileXZPos :: " + projectileXZPos);
       //Debug.log("targetXZPos :: " + targetXZPos);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(_angle * Mathf.Deg2Rad);
        float H = basketBallState.BasketBallTarget.transform.position.y - transform.position.y;
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;


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
        if (basketBallState.TwoPoints) { accuracyModifier = (100 - shooterProfile.Accuracy2Pt) * 0.01f; }
        if (basketBallState.ThreePoints) { accuracyModifier = (100 - shooterProfile.Accuracy3Pt) * 0.01f; }
        if (basketBallState.FourPoints) { accuracyModifier = (100 - shooterProfile.Accuracy4Pt) * 0.01f; }

        //Debug.Log("accuracyModifier : " + accuracyModifier);
        return (accuracyModifier / 1.2f) * direction;
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



