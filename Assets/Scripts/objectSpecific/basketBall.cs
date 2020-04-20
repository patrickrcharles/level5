using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;

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
    Rigidbody rb;
    [SerializeField]
    bool thrown, notlocked;
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
    public bool TwoPoints, ThreePoints, TwoAttempt, ThreeAttempt, dunk,grounded, inAir, facingFront;

    public int totalPoints, TwoPointerMade, ThreePointerMade,
        TwoPointerAttempts, ThreePointerAttempts;

    //[SerializeField]
    // float distanceOfShot;

    public GameObject TextObject;
    Text scoreText;

    public float shotAttempt, shotMade, lastShotDistance, longestShot;


    [Range(20f, 70f)]
    public float _angle;

    bool playHitRimSound;
    public AudioClip shotMiss;
    AudioSource audioSource;
    SpriteRenderer spriteRenderer;

    // Use this for initialization
    void Start()
    {
        player = gameManager.instance.player;
        playerState = gameManager.instance.playerState;
        rb = GetComponent<Rigidbody>();

        // position of basketball infront of player
        basketBallPosition = player.transform.Find("basketBall_position").gameObject;
        //position to shoot basketball at (middle of rim)
        basketBallTarget = GameObject.Find("basketBall_Target");
        //basketball drop shadow
        dropShadow = transform.Find("drop shadow").gameObject;
        //basketBallSprite = transform.FindChild("basketball_sprite").gameObject;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = basketBallSprite.GetComponent<SpriteRenderer>();

        //displacement = Vector3.Distance(basketBallTarget.transform.position, gameObject.transform.position);

        scoreText = TextObject.GetComponent<Text>();
        longestShot = 0;
        playerDunkPos = GameObject.Find("dunk_transform");
        notlocked = true;

        //InitialVelocityY();
        //timeInAir();
        //InitialVelocityX();
        //InitialVelocityD();
        //LaunchAngle();
    }

    // Update is called once per frame
    void Update()
    {
     
        /*   
        if (dunk && inAir)
        {
            // playerState.Launch();
            player.transform.position = playerDunkPos.transform.position;
        }
        */

        

        //displacement = Vector3.Distance(basketBallTarget.transform.position, basketBallPosition.transform.position);
        //Zdistance = basketBallTarget.transform.position.z - gameObject.transform.position.z;

        if (!playerState.hasBasketball)
        {
            spriteRenderer.enabled = true;
        }

        if (playerState.hasBasketball && !thrown)
        {
            transform.position = new Vector3(basketBallPosition.transform.position.x,
                basketBallPosition.transform.position.y,
                basketBallPosition.transform.position.z );

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

        if (playerState.inAir && playerState.hasBasketball && InputManager.GetButtonUp("Fire1") && notlocked)
        {
            //Debug.Log("if(playerState.inAir && playerState.hasBasketball && InputManager.GetButtonUp(Fire1))");
            //Debug.Log("throwSpeed :: " + throwSpeed);
            playerState.hasBasketball = false;
            playerState.setPlayerAnim("hasBasketball", false);

            if (facingFront) // facing straight toward bball goal
            {            
                playerState.setPlayerAnimTrigger("basketballShootFront");
            }
            else // side of goal, relative postion
            {
                playerState.setPlayerAnimTrigger("basketballShoot");             
            }

            notlocked = false;
            thrown = true;
            inAir = true;
 
            Vector3 tempPos  = new Vector3(basketBallTarget.transform.position.x,
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
            //launch ball to goal      
            Launch();
        }
    }
    void OnCollisionEnter(Collision other)
    {

        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("basketballrim") && playHitRimSound)
        {
            playHitRimSound = false;
            Debug.Log("COLLISIONbetween : " + transform.root.name + " and " + other.gameObject.name);
            audioSource.PlayOneShot(shotMiss);
        }
        if (gameObject.CompareTag("basketball") && other.gameObject.CompareTag("ground"))
        {
            inAir = false;
            grounded = true;
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

            //int currentScore = int.Parse(score.GetComponent<Text>() + 1); //add 1 to the score
            //score.GetComponent<Text>() = currentScore.ToString();
            //AudioSource.PlayClipAtPoint(basket, transform.position); //play basket sound
        }
        if (gameObject.CompareTag("basketball") && other.CompareTag("ground"))
        {
            grounded = true;
        }

        if (gameObject.name.Contains("basketBallMadeShot") && other.gameObject.name == "basketball")
        {
            shotMade++;
        }
        if (gameObject.name == "basketball" && other.CompareTag("basketballCourt") && !dunk)
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            TwoPoints = true;
            ThreePoints = false;
        }
        if (gameObject.name == "basketball" && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            dunk = true;
        }
        if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        {
            facingFront = true;
            playerState.setPlayerAnim("basketballFacingFront", true);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (gameObject.name == "basketball" && other.CompareTag("playerHitbox") && thrown)
        {
            thrown = false;
            playerState.hasBasketball = false;
            notlocked = true;
        }
        if (gameObject.name == "basketball" && other.CompareTag("basketballCourt"))
        {
            //Debug.Log("COLLISION EXIT between : " + transform.root.name + " and " + other.name);
            TwoPoints = false;
            ThreePoints = true;
        }
        if (gameObject.name == "basketball" && other.name.Contains("dunk_zone"))
        {
            //Debug.Log("COLLISION between : " + transform.root.name + " and " + other.name);
            dunk = false;
        }
        if (gameObject.CompareTag("basketball") && other.name.Contains("facingFront"))
        {
            facingFront = false;
            playerState.setPlayerAnim("basketballFacingFront", false);
        }

    }

    void Launch()
    {
        shotAttempt++;
        updateScore(); // updates shotAttemps/ calculates accuracy/score

        // think of it as top-down view of vectors: 
        //   we don't care about the y-component(height) of the initial and target position.

        //Vector3 playerProjectileXZPos  = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
        Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        Vector3 targetXZPos = new Vector3(basketBallTarget.transform.position.x, basketBallTarget.transform.position.y, basketBallTarget.transform.position.z);

        Debug.Log("projectileXZPos :: " + projectileXZPos);
        Debug.Log("targetXZPos :: " + targetXZPos);

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        //float Rplayer = Vector3.Distance(playerProjectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(_angle * Mathf.Deg2Rad);
        float H = basketBallTarget.transform.position.y - transform.position.y;
        //float Hplayer = basketBallTarget.transform.position.y - player.transform.position.y;

        Debug.Log("R :: " + R);
        Debug.Log("G :: " + G);
        Debug.Log("tanAlpha :: " + tanAlpha);
        Debug.Log("H :: " + H);
        Debug.Log("_angle :: " + _angle);

        // calculate the local space components of the velocity 
        // required to land the projectile on the target object 
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        Debug.Log("Vz :: " + Vz);
        Debug.Log("Vy :: " + Vy);


        //float VzPlayer = Mathf.Sqrt(G * Rplayer * Rplayer / (2.0f * (Hplayer - Rplayer * tanAlpha)));
        //float VyPlayer = tanAlpha * Vz;

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(0f, Vy, Vz);
        Vector3 globalVelocity = transform.TransformDirection(localVelocity);


        Debug.Log("localVelocity :: " + localVelocity);
        Debug.Log("globalVelocity :: " + globalVelocity);

        //Vector3 playerLocalVelocity = new Vector3(0f, VyPlayer, VzPlayer);
        //Vector3 playerGlobalVelocity = transform.TransformDirection(playerLocalVelocity);

        // launch the object by setting its initial velocity and flipping its state

        rb.velocity = globalVelocity;
        Debug.Log("rb.velocity :: " + rb.velocity);

    }


    public void addToShotMade(int value)
    {
        shotMade += value;
    }
    public bool getDunk()
    {
        return dunk;
    }

    public void updateScore()
    {
        if (shotAttempt > 0)
        {
            float accuracy = ((shotMade / shotAttempt) * 100);

            scoreText.text = "shots attempted : " + shotAttempt + "\n"
                + "shots made : " + shotMade + "\n"
                + "accuracy : " + Math.Round(accuracy, 2).ToString("0.00") + "%" + "\n"
                + "points : " + totalPoints + "\n"
                + "2 pointers : " + TwoPointerMade + " / " + TwoPointerAttempts + "\n"
                + "3 pointers : " + ThreePointerMade + " / " + ThreePointerAttempts + "\n"
                + "last shot distance : " + (Math.Round(lastShotDistance, 2) * 6f).ToString("0.00") +" ft." + "\n"
                + "longest shot distance : " + (Math.Round(longestShot, 2) * 6f).ToString("0.00") + " ft.";
        }
    }



}
