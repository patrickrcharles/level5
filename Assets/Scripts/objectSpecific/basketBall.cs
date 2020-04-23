using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class basketBall : MonoBehaviour
{

    public GameObject score; //reference to the ScoreText gameobject, set in editor
    public AudioClip basket; //reference to the basket sound
    public GameObject basketBallPosition, basketBallTarget;
    [SerializeField]
    GameObject player, dropShadow;
    public GameObject basketBallSprite, playerDunkPos;

    playercontrollerscript playerState;
    [SerializeField]
    new Rigidbody rigidbody;
    [SerializeField]
    public bool thrown, notlocked, canPullBall;
    [SerializeField]
    float throwSpeed;

    //Physics variables
    float velocityFinal;
    float velocityInitialY, velocityInitialX, velocityInitialD;
    [SerializeField]
    float displacement, Zdistance;
    float acceleration = -9.8f;
    float time;
    float launchAngle;
    //float point1, point2;

    [SerializeField]
    public bool TwoPoints, ThreePoints, FourPoints, TwoAttempt, ThreeAttempt, FourAttempt, dunk, grounded, inAir, facingFront;

    public float totalPoints, TwoPointerMade, ThreePointerMade, FourPointerMade,
        TwoPointerAttempts, ThreePointerAttempts, FourPointerAttempts;

    //[SerializeField]
    // float distanceOfShot;

    public GameObject TextObject;
    Text scoreText;

    public GameObject shootProfile;
    Text shootProfileText;

    public float shotAttempt, shotMade, lastShotDistance, longestShot;


    [Range(20f, 70f)]
    public float _angle;

    bool playHitRimSound;
    public AudioClip shotMiss;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;

    [SerializeField]
    float ballDistanceFromRim;

    [SerializeField]
    float twoPointDistance, threePointDistance, fourPointDistance;

    shooterProfile shooterProfile;
    float releaseVelocityY;

    private float _playerRigidBody;

    public float accuracy;
    public float twoAccuracy;
    public float threeAccuracy;
    public float fourAccuracy;
    public bool addAccuracyModifier;

    // Use this for initialization
    void Start()
    {
        player = gameManager.instance.player;
        playerState = gameManager.instance.playerState;
        rigidbody = GetComponent<Rigidbody>();

        // position of basketball infront of player
        basketBallPosition = player.transform.Find("basketBall_position").gameObject;
        //position to shoot basketball at (middle of rim)
        basketBallTarget = GameObject.Find("basketBall_target");
        //basketball drop shadow
        dropShadow = transform.Find("drop shadow").gameObject;
        //basketBallSprite = transform.FindChild("basketball_sprite").gameObject;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = basketBallSprite.GetComponent<SpriteRenderer>();
        shooterProfile =  gameManager.instance.player.GetComponent<shooterProfile>();


        //displacement = Vector3.Distance(basketBallTarget.transform.position, gameObject.transform.position);
        //shooterProfile = GameObject.Find("shooterProfileTextObject").GetComponent<Text>();

        TextObject = GameObject.Find("shootStatsTextObject");
        shootProfile = GameObject.Find("shooterProfileTextObject");

        shootProfileText = shootProfile.GetComponent<Text>();
        scoreText = TextObject.GetComponent<Text>();

        longestShot = 0;
        playerDunkPos = GameObject.Find("dunk_transform");
        notlocked = true;
        canPullBall = true;
        addAccuracyModifier = false;

        shootProfileText.text = "ball distance : " + (Math.Round(ballDistanceFromRim, 2)) + "\n"
            + "shot distance : " + (Math.Round(ballDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
            + "shooter : Dr Blood\n"
            + "2 point accuracy : " + ((1 - shooterProfile.accuracy2pt) * 100) + "\n"
            + "3 point accuracy : " + ((1 - shooterProfile.accuracy3pt) * 100) + "\n"
            + "4 point accuracy : " + ((1 - shooterProfile.accuracy4pt) * 100);

    }

    // Update is called once per frame
    void Update()
    {

        ballDistanceFromRim = Vector3.Distance(transform.position, basketBallTarget.transform.position);
        // change this to reduce opacity
        if (!playerState.hasBasketball)
        {
            spriteRenderer.enabled = true;
        }

        updateScoreText();

        shootProfileText.text = "distance : " + (Math.Round(ballDistanceFromRim, 2)) + "\n"
            + "shot distance : " + (Math.Round(ballDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.\n"
            + "shooter : Dr Blood\n"
            + "2 point accuracy : " + shooterProfile.accuracy2pt + "\n"
            + "3 point accuracy : " + shooterProfile.accuracy3pt + "\n"
            + "4 point accuracy : " + shooterProfile.accuracy4pt;

        //Debug.Log("shot distance : " + (Math.Round(ballDistanceFromRim, 2) * 6f).ToString("0.00") + " ft.");

        if (ballDistanceFromRim < threePointDistance) { TwoPoints = true; }
        else { TwoPoints = false; }

        if (ballDistanceFromRim > threePointDistance && ballDistanceFromRim < fourPointDistance) { ThreePoints = true; }
        else { ThreePoints = false; }

        if (ballDistanceFromRim > fourPointDistance) { FourPoints = true; }
        else { FourPoints = false; }


        if (playerState.hasBasketball && !thrown)
        {
            transform.position = new Vector3(basketBallPosition.transform.position.x,
                basketBallPosition.transform.position.y,
                basketBallPosition.transform.position.z);

            if (playerState.grounded)
            {
                basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, 0);
                spriteRenderer.enabled = false;
                playerState.setPlayerAnim("hasBasketball", true);
                //playerState.setPlayerAnim("walking", false);
                playerState.setPlayerAnim("moonwalking", false);
            }
            else
            {
                basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
                //playerState.setPlayerAnim("hasBasketball", false);
            }
        }

        if (!playerState.hasBasketball)
        {
            basketBallSprite.transform.rotation = Quaternion.Euler(13.6f, 0, transform.root.position.z);
        }

        dropShadow.transform.position = new Vector3(transform.root.position.x, 0.05f, transform.root.position.z - 0.1f);
        dropShadow.transform.rotation = Quaternion.Euler(90, 0, 0);

        // =========== this is the conditional for auto shooting =============================
        // this will work great for  playing against CPU

        //if (playerState.inAir && playerState.hasBasketball && playerState.jumpPeakReached)
        //{
        //}


        if (playerState.inAir && playerState.hasBasketball && InputManager.GetButtonDown("Fire1"))
        {
            releaseVelocityY = playerState.rigidBodyYVelocity;
            //Debug.Log("releaseVelocityY : " + releaseVelocityY);
            //Debug.Log("$$$$$$$$$$$$$$$$$$$ shoot : addAccuracyModifier : " + addAccuracyModifier);
            //Debug.Log("shoot ball");
            playerState.hasBasketball = false;
            playerState.setPlayerAnim("hasBasketball", false);
            // set player shoot anim based on position
            //if (playerState.facingFront) // facing straight toward bball goal
            //{
            //    //setPlayerAnimTrigger("basketballShootFront");
            //    playerState.setPlayerAnim("basketballShootFront", true);
            //}
            //else // side of goal, relative postion
            //{
            //    // setPlayerAnimTrigger("basketballShoot");
            //    playerState.setPlayerAnim("basketballShoot", false);
            //}

            if (playerState.facingFront) // facing straight toward bball goal
            {
                playerState.setPlayerAnimTrigger("basketballShootFront");
            }
            else // side of goal, relative postion
            {
                playerState.setPlayerAnimTrigger("basketballShoot");
            }

            //launch ball to goal      
            Launch();

            //Jessica might take a photo
            behavior_jessica.instance.playAnimationTakePhoto();

            notlocked = false;
            thrown = true;
            inAir = true;

            //calculate shot distance 
            Vector3 tempPos = new Vector3(basketBallTarget.transform.position.x,
                0, basketBallTarget.transform.position.z);
            float tempDist = Vector3.Distance(tempPos, basketBallPosition.transform.position);
            lastShotDistance = tempDist;

            // identify is in 2 or 3 point range for stat counters
            if (TwoPoints)
            {
                //Debug.Log(" 2 point attempt");
                TwoAttempt = true;
                TwoPointerAttempts++;
                //Debug.Log("TwoAttempt :: " + TwoAttempt);
            }
            if (ThreePoints)
            {
                //Debug.Log(" 3 point attempt");
                ThreeAttempt = true;
                ThreePointerAttempts++;
                //Debug.Log("ThreeAttempt :: "+ ThreeAttempt);
            }
            if (FourPoints)
            {
                //Debug.Log(" 3 point attempt");
                FourAttempt = true;
                FourPointerAttempts++;
                //Debug.Log("ThreeAttempt :: "+ ThreeAttempt);
            }

            //launch ball to goal      
            //aunch();
            //updateScoreText();

            //updateScore(); // updates shotAttemps/ calculates accuracy/score
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
            canPullBall = true;
        }
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
            inAir = false;
            grounded = true;
            canPullBall = true;
            audioSource.PlayOneShot(SFXBB.Instance.basketballBounce);
        }

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("fence"))
        {
            //Debug.Log("$$$$$$$$$$$$$$$$$");
            //Debug.Log("Collision Enter : " + transform.name + " other.name : " + other.gameObject.name);
            //inAir = false;
            //grounded = true;
            audioSource.PlayOneShot(SFXBB.Instance.basketballHitFence);
            canPullBall = true;
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
            grounded = false;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.CompareTag("playerHitbox"))
        {
            //Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.name);
            playerState.hasBasketball = true;
            playerState.turnOffMoonWalkAudio();
            canPullBall = false;

        }
        if (gameObject.CompareTag("basketball") && other.CompareTag("ground"))
        {
            grounded = true;
        }

        //if (gameObject.name.Contains("basketBallMadeShot") && other.gameObject.name == "basketball")
        //{
        //    shotMade++;
        //}

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            dunk = true;
        }
        //if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        //{
        //    facingFront = true;
        //    playerState.setPlayerAnim("basketballFacingFront", true);
        //}
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("playerHitbox") && thrown)
        {
            //Debug.Log("$$$$$$$$$$$$$$$$$$");
            thrown = false;
            playerState.hasBasketball = false;
            notlocked = true;
        }

        if (gameObject.CompareTag("basketball") && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            dunk = false;
        }
        //if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        //{
        //    facingFront = false;
        //    playerState.setPlayerAnim("basketballFacingFront", false);
        //}

    }

    void Launch()
    {
        shotAttempt++;
       //Debug.Log("Launch()");
       //Debug.Log("Launch() : addAccuracyModifier : " + addAccuracyModifier);
        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.

        //Vector3 playerProjectileXZPos  = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 targetXZPos = new Vector3(basketBallTarget.transform.position.x, basketBallTarget.transform.position.y, basketBallTarget.transform.position.z);

        //Debug.Log("projectileXZPos :: " + projectileXZPos);
        //Debug.Log("targetXZPos :: " + targetXZPos);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        //float Rplayer = Vector3.Distance(playerProjectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(_angle * Mathf.Deg2Rad);
        float H = basketBallTarget.transform.position.y - transform.position.y;
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

        //Debug.Log("old x : " + 0 + " y : " + Vy + " z : " + Vz);

        float accuracyModifierX = 0;
        if (rollForCriticalShotChance( shooterProfile.criticalPercent))
        {
            accuracyModifierX = 0;
        }
        else
        {
            accuracyModifierX = getAccuracyModifier();
        }

        float xVector = 0  + accuracyModifierX;
        float yVector = Vy; // + (accuracyModifier * shooterProfile.shootYVariance);
        float zVector = Vz; // + (accuracyModifier * shooterProfile.shootZVariance);

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = localVelocity = new Vector3(0, Vy, Vz);
        //Debug.Log("addAccuracyModifier : " + addAccuracyModifier);


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
        if (percent <= shooterProfile.criticalPercent)
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
        if (TwoPoints) { accuracyModifier = (100 - shooterProfile.accuracy2pt) * 0.01f; }
        if (ThreePoints) { accuracyModifier = (100 - shooterProfile.accuracy3pt) * 0.01f; }
        if (FourPoints) { accuracyModifier = (100 - shooterProfile.accuracy4pt) * 0.01f; }

       //Debug.Log("accuracyModifier : " + accuracyModifier);
        return (accuracyModifier/1.2f)  * direction;
    }



    private int getRandomPositiveOrNegtaive()
    {
        var Random = new Random();
        List<int> list = new List<int> { 1, -1 };
        int finder = Random.Next(list.Count); //Then you just use this; nameDisplayString = names[finder];
        int shotDirectionModifier = list[finder];

        return shotDirectionModifier;
    }

    public void addToShotMade(int value)
    {
        shotMade += value;
    }
    public bool getDunk()
    {
        return dunk;
    }


    public void toggleAddAccuracyModifier()
    {
        //Debug.Log("toggleAddAccuracyModifier()");
        //if (addAccuracyModifier == true)
        //{
        //    Debug.Log("$$$$$$$$$$$$$$$$ FALSE");
        //    addAccuracyModifier = false;
        //}
        //else
        //{
        //    Debug.Log("$$$$$$$$$$$$$$$$ TRUE");
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

        scoreText.text = "shots  : " + shotMade + " / " + shotAttempt + "\n"
        + "shots made : " + shotMade + "\n"
        + "accuracy : " + getTotalPointAccuracy() + "%\n"
        + "points : " + totalPoints + "\n"
        + "2 pointers : " + TwoPointerMade + " / " + TwoPointerAttempts + "\n" //+ " accuracy : " + getTwoPointAccuracy() + "%\n"
        + "3 pointers : " + ThreePointerMade + " / " + ThreePointerAttempts + "\n"// +" accuracy : " + getThreePointAccuracy() + "%\n"
        + "4 pointers : " + FourPointerMade + " / " + FourPointerAttempts + "\n"// + " accuracy : " + getFourPointAccuracy() + "%\n"
        + "last shot distance : " + (Math.Round(lastShotDistance, 2) * 6f).ToString("0.00") + " ft." + "\n"
        + "longest shot distance : " + (Math.Round(longestShot, 2) * 6f).ToString("0.00") + " ft.";
    }

    public float getTotalPointAccuracy()
    {
        if (shotAttempt > 0)
        {
            accuracy = shotMade / shotAttempt ;
            return (accuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getTwoPointAccuracy()
    {
        if(TwoPointerAttempts > 0)
        {
            twoAccuracy = TwoPointerMade / TwoPointerAttempts ;
            return (twoAccuracy * 100);
        }
        else
        {
           return 0;
        }
    }

    public float getThreePointAccuracy()
    {
        if (ThreePointerAttempts > 0)
        {
            threeAccuracy = ThreePointerMade / ThreePointerAttempts;
            return (threeAccuracy * 100);
        }
        else
        {
            return 0;
        }
    }

    public float getFourPointAccuracy()
    {
        if (FourPointerAttempts > 0)
        {
            twoAccuracy = FourPointerMade / FourPointerAttempts;
            return (fourAccuracy * 100);
        }
        else
        {
            return 0;
        }
    }

}
