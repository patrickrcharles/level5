using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;
using System;

public class PlayerController : MonoBehaviour
{

    //#todo start breaking this up into separate classes: playerState, player move (walk / jump /input actions) 

    // components 
    Animator anim;
    AnimatorStateInfo currentStateInfo;
    GameObject dropShadow;
    AudioSource moonwalkAudio;
    SpriteRenderer spriteRenderer;
    private Rigidbody rigidBody;
    ShooterProfile shooterProfile;
    BasketBall basketball;
    private ShotMeter shotmeter;

    // walk speed #review can potentially remove
    private float movementSpeed;
    [SerializeField]
    private float inAirSpeed; // leave serialized

    // player state bools
    private bool running;
    private bool runningToggle;
    public bool hasBasketball;

    // #note this is work a work in progress feature. it works but it's bugged
    private bool isSetShooter; 
    public bool IsSetShooter => isSetShooter;
    public bool canMove; // #todo add player knock downs, this could be used
                         // will be useful for when play knockdowns implemented
    public GameObject playerHitbox;

    Vector3 bballRimVector;
    float bballRelativePositioning;
    Vector3 playerRelativePositioning;
    public float playerDistanceFromRim;


    // control movement speed based on state
    static int currentState;
    static int idleState = Animator.StringToHash("base.idle");
    static int walkState = Animator.StringToHash("base.movement.walk");
    static int run = Animator.StringToHash("base.movement.moonwalk");
    static int bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
    static int bIdle = Animator.StringToHash("base.movement.basketball_idle");
    static int knockedDownState = Animator.StringToHash("base.takeDamage.knockedDown");

    //controller axis
    [SerializeField]
    bool triggerLeftAxisInUse, triggerRightAxisInUse;

    // get/set for following at bottom of class
    private bool _facingRight;
    private bool _facingFront;
    private bool _locked;
    private bool _jump;
    private bool _inAir;
    private bool _grounded;
    private bool _knockedDown;

    [SerializeField]
    float _knockDownTime;

    //#review no longer use, but some it could be useful
    public float initialHeight, finalHeight;
    public bool jumpPeakReached = false;
    private float _rigidBodyYVelocity;

    // used to calculate shot meter time
    public float jumpStartTime;
    public float jumpEndTime;

    void Start()
    {

        moonwalkAudio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        basketball = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
        shooterProfile = GetComponent<ShooterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GameObject.FindWithTag("shot_meter").GetComponent<ShotMeter>();

        // bball rim vector, used for relative positioning
        bballRimVector = GameObject.Find("rim").transform.position;

        // #note used for testing scenes
        if (GameOptions.gameModeHasBeenSelected)
        {
            //Debug.Log(" set shoot profile");
            setShooterProfileStats();
        }

        //Debug.Log(" initial shooter profile stats =====================================================================================");
        //printShooterProfileStats();
        dropShadow = transform.root.transform.Find("drop_shadow").gameObject;
        playerHitbox.SetActive(true);
        facingRight = true;
        canMove = true;
        movementSpeed = shooterProfile.Speed;

        runningToggle = true;
    }



    // not affected by framerate
    void FixedUpdate()
    {
        //------MOVEMENT---------------------------
        if (!KnockedDown)
        {
            //Debug.Log("movement : knocked down "+ KnockedDown);
            float moveHorizontal = InputManager.GetAxis("Horizontal");
            float moveVertical = InputManager.GetAxis("Vertical");
            Vector3 movement;

            movement = new Vector3(moveHorizontal, 0, moveVertical) * movementSpeed * Time.deltaTime;

            rigidBody.MovePosition(transform.position + movement);
            /*
            //set limits for player movement
            rigidBody.transform.position = new Vector3(
               Mathf.Clamp(rigidBody.position.x, xMin, xMax),
               Mathf.Clamp(rigidBody.position.y, yMin, yMax),
               Mathf.Clamp(rigidBody.position.z, zMin, zMax)
               );
               */
            //check if walking
            //  function will flip sprite if needed

            isWalking(moveHorizontal, moveVertical);
        }
    }

    // Update :: once once per frame
    void Update()
    {

        if (KnockedDown && !locked)
        {
            locked = true;
            rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            // coroutine that holds animation with WaitUntil knock down time is through
            StartCoroutine(PlayerKnockedDown());
        }

        // keep drop shadow on ground at all times
        // dont like having hard code values. add variables
        if (grounded)
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, transform.root.position.y + 0.02f,
                transform.root.position.z);
        }
        else
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
                transform.root.position.z);
        }

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        bballRelativePositioning = bballRimVector.x - rigidBody.position.x;
        playerRelativePositioning = rigidBody.position - bballRimVector;

        playerDistanceFromRim = Vector3.Distance(transform.position, bballRimVector);

        // if run input or run toggle on
        if ( (InputManager.GetButton("Run") 
            && canMove 
            && !inAir
            && !KnockedDown
            && !locked))
        {
            running = true;
        }
        else
        {
            running = false;
            moonwalkAudio.enabled = false;
        }

        //player reaches peak of jump. this will be useful for creating AI with auto shoot
        if (rigidBodyYVelocity > 0 && inAir)
        {
            jumpPeakReached = true;
        }
        else
        {
            jumpPeakReached = false;
        }

        // determine if player animation is shooting from or facing basket
        if (Math.Abs(playerRelativePositioning.x) > 2 &&
            Math.Abs(playerRelativePositioning.z) < 2)
        {
            facingFront = false;
        }
        else
        {
            facingFront = true;
        }

        // set player shoot anim based on position
        if (facingFront) // facing straight toward bball goal
        {
            //setPlayerAnimTrigger("basketballShootFront");
            setPlayerAnim("basketballFacingFront", true);
        }
        else // side of goal, relative postion
        {
            // setPlayerAnimTrigger("basketballShoot");
            setPlayerAnim("basketballFacingFront", false);
        }

        // ----- control speed based on commands----------
        if (currentState == idleState
            || currentState == walkState
            || currentState == bIdle
            && ! inAir
            && !KnockedDown)
        {
            if (runningToggle)
            {
                movementSpeed = shooterProfile.RunSpeed;
            }
            else
            {
                movementSpeed = shooterProfile.Speed;
            }
        }

        else if (currentState == run)
        {
            movementSpeed = shooterProfile.RunSpeed; ;
        }

        if (inAir)
        {
            checkIsPlayerFacingGoal();
            movementSpeed = inAirSpeed;
        }

        //------------------ jump -----------------------------------
        if ((InputManager.GetButtonDown("Jump")
            && !(InputManager.GetButtonDown("Fire1"))
            && grounded
            && !KnockedDown))
            //&& !isSetShooter))
        {
            playerJump();
        }


        // if player is falling, nto sure what this is useful for. comment out
        //if (rigidBody.velocity.y > 0)
        //{
        //    //updates "highest point" as long at player still moving upwards ( velcoity > 0)
        //    finalHeight = transform.position.y;
        //    //Debug.Log("intialHeight : " + initialHeight);  
        //    //Debug.Log("finalHeight : " + finalHeight);
        //}
    }

    public void checkIsPlayerFacingGoal()
    {
        if (bballRelativePositioning > 0 && !facingRight)
        {
            Flip();
        }

        if (bballRelativePositioning < 0f && facingRight)
        {
            Flip();
        }
    }

    public void playerJump()
    {
        if (!isSetShooter)
        {
            rigidBody.velocity = (Vector3.up * shooterProfile.JumpForce); // + (Vector3.forward * rigidBody.velocity.x);
        }

        //jumpStartTime = Time.time;
        Shotmeter.MeterStarted = true;
        Shotmeter.MeterStartTime = Time.time;
        //Debug.Log("jump time: "+ Time.time);
 
    }

    //-----------------------------------Walk function -----------------------------------------------------------------------
    void isWalking(float moveHorz, float moveVert)
    {
        //Debug.Log("walking : knocked down " + KnockedDown);;
        // if moving/ not holding item. 
        if (moveHorz > 0f || moveHorz < 0f || moveVert > 0f || moveVert < 0f)
        {
            if (!inAir && !running ) // dont want walking animation playing while inAir
            {
                anim.SetBool("walking", true);
            }
        }
        // not moving
        else
        {
            anim.SetBool("walking", false);
            anim.SetBool("moonwalking", false);
        }

        // if running enabled
        if ((runningToggle || running) &&  canMove && !inAir && currentState == walkState)
        {
            //Debug.Log("if (runningToggle && canMove && !inAir)");
            if (!hasBasketball)
            {
                anim.SetBool("moonwalking", true);
                //movementSpeed = shooterProfile.RunSpeed;
            }

            //if (hasBasketball)
            //{
            //    movementSpeed = shooterProfile.RunSpeedHasBall; ;
            //}
            //else
            //{
            //    movementSpeed = runMovementSpeed;
            //}
        }

        if (moveHorz > 0 && !facingRight && canMove)
        {
            Flip();
        }
        if (moveHorz < 0f && facingRight && canMove)
        {
            Flip();
        }

        //if (running && canMove && !inAir)
        //{
        //    if (!hasBasketball)
        //    {
        //        anim.SetBool("moonwalking", true);
        //        movementSpeed = shooterProfile.RunSpeed;
        //    }

        //    if (hasBasketball)
        //    {
        //        movementSpeed = shooterProfile.RunSpeedHasBall;
        //    }
        //}
    }

    void Flip()
    {
        //Debug.Log("flip");
        facingRight = !facingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
    }

    // ------------------------------- Knocked down -------------------------------------------------------
    IEnumerator PlayerKnockedDown()
    {
        float startTime = Time.time;
        float endTime = startTime + _knockDownTime;

        Debug.Log("================   IEnumerator KnockedDown() ===========================");
        //yield return new WaitUntil(() => grounded == true);
        anim.SetBool("knockedDown", true);
        anim.Play("knockedDown");
        //yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length + anim.GetCurrentAnimatorStateInfo(0).normalizedTime);
        yield return new WaitUntil(() => Time.time > endTime);

        anim.SetBool("knockedDown", false);
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        KnockedDown = false;
        locked = false;
        Debug.Log("finish - KnockedDown() - knocked down time.time : " + Time.timeSinceLevelLoad);
    }

    //------------------------- set animator parameters -----------------------
    public void setPlayerAnim(string animationName, bool isTrue)
    {
        anim.SetBool(animationName, isTrue);
    }

    //------------------------- set animator parameters -----------------------
    public void setPlayerAnimTrigger(string animationName)
    {
        anim.SetTrigger(animationName);
    }

    //-------------------play animation function ------------------------------
    // provide access to what should be private animator
    public void playAnim(string animationName)
    {
        anim.Play(animationName);
    }

    //can be used as generic turn off audio by adding paramter to pass (Audio audioToTurnOff)
    public void turnOffMoonWalkAudio()
    {
        // moonwalkAudio.enabled = false;
    }

    //*** need to update this
    void setShooterProfileStats()
    {
        shooterProfile.Speed = GameOptions.speed;
        shooterProfile.RunSpeed = GameOptions.runSpeed;
        shooterProfile.RunSpeed = GameOptions.runSpeedHasBall;
        shooterProfile.JumpForce = GameOptions.jumpForce;
        shooterProfile.CriticalPercent = GameOptions.criticalPercent;
        shooterProfile.ShootAngle = GameOptions.shootAngle;
        shooterProfile.Accuracy2Pt = GameOptions.accuracy2pt;
        shooterProfile.Accuracy3Pt = GameOptions.accuracy3pt;
        shooterProfile.Accuracy4Pt = GameOptions.accuracy4pt;
        shooterProfile.Accuracy7Pt = GameOptions.accuracy7pt;

        //gravityModifier = shooterProfile.HangTime;
        //_angle = shooterProfile.ShootAngle;
    }

    //*** need to update this
    void printShooterProfileStats()
    {
        Debug.Log(shooterProfile.Speed);
        Debug.Log(shooterProfile.RunSpeed);
        Debug.Log(shooterProfile.RunSpeed);
        Debug.Log(shooterProfile.JumpForce);
        Debug.Log(shooterProfile.CriticalPercent);
        Debug.Log(shooterProfile.ShootAngle);
        Debug.Log(shooterProfile.Accuracy2Pt);
        Debug.Log(shooterProfile.Accuracy3Pt);
        Debug.Log(shooterProfile.Accuracy4Pt);
        Debug.Log(shooterProfile.Accuracy7Pt);

        //gravityModifier = shooterProfile.HangTime;
        //_angle = shooterProfile.ShootAngle;
    }
    public bool grounded
    {
        get { return _grounded; }
        set { _grounded = value; }
    }

    public bool inAir
    {
        get { return _inAir; }
        set { _inAir = value; }
    }
    public bool jump
    {
        get { return _jump; }
        set { _jump = value; }
    }
    public bool locked
    {
        get { return _locked; }
        set { _locked = value; }
    }
    public bool facingRight
    {
        get { return _facingRight; }
        set { _facingRight = value; }
    }

    public float rigidBodyYVelocity
    {
        get { return rigidBody.velocity.y; }
    }
    public bool facingFront
    {
        get => _facingFront;
        set => _facingFront = value;
    }
    public ShotMeter Shotmeter 
    { 
        get => shotmeter; 
        set => shotmeter = value; 
    }

    public bool KnockedDown 
    { 
        get => _knockedDown; 
        set => _knockedDown = value; 
    }

    // #todo find all these messageDisplay coroutines and move to seprate generic class MessageLog od something
    public void toggleRun() 
    {
        runningToggle = !runningToggle;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "running toggle = " + runningToggle;

        // turn off text display after 5 seconds
        StartCoroutine(BasketBall.instance.turnOffMessageLogDisplayAfterSeconds(5));
    }
}
