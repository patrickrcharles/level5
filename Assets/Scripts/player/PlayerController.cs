using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

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
    private bool _knockedDown_alternate1;
    private bool _avoidedKnockDown;

    [SerializeField]
    float _knockDownTime;

    //#review no longer use, but some it could be useful
    public float initialHeight, finalHeight;
    public bool jumpPeakReached = false;
    private float _rigidBodyYVelocity;

    // used to calculate shot meter time
    public float jumpStartTime;
    public float jumpEndTime;

    Vector2 movementInput;
    Vector3 movement;

    // for touch controls
    public Joystick joystick;
    public Button jumpButton;
    public Button shootButton;

    float movementHorizontal;
    float movementVertical;

    //PlayerControls controls;

    private void Awake()
    {
        //controls = new PlayerControls();
    }

    void Start()
    {

        moonwalkAudio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        basketball = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
        shooterProfile = GetComponent<ShooterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GameObject.FindWithTag("shot_meter").GetComponent<ShotMeter>();

        joystick = FindObjectOfType<Joystick>();

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

            // touch controls variables -----------------------------------------------------------------------

            movementHorizontal = joystick.Horizontal;
            movementVertical = joystick.Vertical;
            movement = new Vector3(movementHorizontal, 0, movementVertical) * movementSpeed * Time.deltaTime;
            //Debug.Log("movement : " + movement);

            // Input Sytem 1.0.0 controls variables ---------------------------------------------------------------

            //movementInput = GameLevelManager.Instance.Controls.Player.movement.ReadValue<Vector2>();
            //movement = new Vector3(movementInput.x, 0, movementInput.y) * movementSpeed * Time.deltaTime;

            //-----------------------------------------------------------------------------------------------------

            rigidBody.MovePosition(transform.position + movement);
            isWalking(movement);
        }
    }


    public void TouchControlJump()
    {
        if (grounded && !KnockedDown)
        //&& !isSetShooter))
        {
            playerJump();
        }
    }

    public void touchControlShoot()
    {
        //Debug.Log(" shoot clicked");
        // if has ball, is in air, and pressed shoot button.
        // shoot ball
        if (inAir
            && hasBasketball
            && !IsSetShooter)
        {
            //Debug.Log("     shoot ");
            CallBallToPlayer.instance.Locked = true;
            basketball.BasketBallState.Locked = true;
            checkIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true;
            playerShoot();
        }
        // call ball
        if (!hasBasketball
            && !inAir
            && basketball.BasketBallState.CanPullBall
            && !basketball.BasketBallState.Locked
            && grounded
            && !CallBallToPlayer.instance.Locked)
        {
            //Debug.Log("     call ball");
            CallBallToPlayer.instance.Locked = true;
            CallBallToPlayer.instance.pullBallToPlayer();
            CallBallToPlayer.instance.Locked = false;
        }
    }

    // Update :: once once per frame
    void Update()
    {


        if ((KnockedDown || KnockedDown_Alternate1) && !locked)
        {
            locked = true;
            rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            // if alternate knockdown animation
            if (KnockedDown_Alternate1)
            {
                StartCoroutine(PlayerKnockedDown("knockedDown_alternate"));
            }
            else
            {
                StartCoroutine(PlayerKnockedDown("knockedDown"));
            }
        }
        if (AvoidedKnockDown && !locked)
        {
            //Debug.Log("        if (AvoidedKnockDown && !locked)");
            locked = true;
            //rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            // coroutine that holds animation with WaitUntil knock down time is through
            //StartCoroutine(PlayerAvoidKnockedDown());
            PlayerAvoidKnockedDown();
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
        if (/*InputManager.GetButton("Run")*/
            //GameLevelManager.Instance.Controls.Player.run.ReadValue<float>() == 1 //if button is held
            GameLevelManager.Instance.Controls.Player.run.ReadValue<float>() == 1 //if button is held
            && canMove
            && !inAir
            && !KnockedDown
            && !locked)
        {
            running = true;
            anim.SetBool("moonwalking", true);
        }
        else
        {
            running = false;
            //moonwalkAudio.enabled = false;
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
            && !inAir
            && !KnockedDown)
        {
            if (runningToggle || running)
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
        if (//(InputManager.GetButtonDown("Jump")
            GameLevelManager.Instance.Controls.Player.jump.triggered
            && !GameLevelManager.Instance.Controls.Player.shoot.triggered
            //&& !(InputManager.GetButtonDown("Fire1"))
            && grounded
            && !KnockedDown)
        //&& !isSetShooter))
        {
            playerJump();
        }

        //    if (
        //GameLevelManager.Instance.Controls.Player.jump.triggered
        //&& !GameLevelManager.Instance.Controls.Player.shoot.triggered
        //&& grounded
        //&& !KnockedDown)
        //    //&& !isSetShooter))
        //    {
        //        playerJump();
        //    }


        //------------------ shoot -----------------------------------
        // if has ball, is in air, and pressed shoot button.
        if (inAir
            && hasBasketball
            && GameLevelManager.Instance.Controls.Player.shoot.triggered
            //&& playerState.jumpPeakReached
            && !IsSetShooter)
        //&& !basketBallState.Locked)
        {
            CallBallToPlayer.instance.Locked = true;
            basketball.BasketBallState.Locked = true;
            checkIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true;
            playerShoot();
        }

        // if has ball, is in air, and pressed shoot button.
        if (!inAir
            && hasBasketball
            && GameLevelManager.Instance.Controls.Player.shoot.triggered
            //&& InputManager.GetButtonDown("Jump")
            //&& playerState.jumpPeakReached
            && IsSetShooter)
        {
            basketball.BasketBallState.Locked = true;
            checkIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true;
            playerShoot();
        }


        //------------------ special -----------------------------------
        if (GameLevelManager.Instance.Controls.Player.special.triggered
            //(InputManager.GetKeyDown(KeyCode.G) || InputManager.GetButtonDown("Fire3"))
            && !inAir
            && grounded
            && !KnockedDown)
        //&& !isSetShooter))
        {
            playerSpecial();
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

    public void playerShoot()
    {
        basketball.shootBasketBall();
    }

    private void playerSpecial()
    {
        playAnim("special");
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
    void isWalking(Vector3 movement)
    {
        //Debug.Log("walking : knocked down " + KnockedDown);;
        // if moving/ not holding item. 
        if (movement.x > 0f || movement.x < 0f || movement.z > 0f || movement.z < 0f)
        {
            if (!inAir && !running) // dont want walking animation playing while inAir
            {
                anim.SetBool("walking", true);
            }
        }
        // not moving
        else
        {
            anim.SetBool("walking", false);
            //anim.SetBool("moonwalking", false);
            running = false;
        }

        // if running enabled
        if ((runningToggle || running) && !inAir && currentState == walkState)
        {
            //Debug.Log("if (runningToggle && canMove && !inAir)");
            if (!hasBasketball)
            {
                //anim.SetBool("walking", false);
                anim.SetBool("moonwalking", true);
                movementSpeed = shooterProfile.RunSpeed;
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

        if (movement.x > 0 && !facingRight && canMove)
        {
            Flip();
        }
        if (movement.x < 0f && facingRight && canMove)
        {
            Flip();
        }
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
    public IEnumerator PlayerKnockedDown(string knockDownAnim)
    {
        float startTime = Time.time;
        float endTime = startTime + _knockDownTime;

        anim.SetBool(knockDownAnim, true);
        anim.Play(knockDownAnim);

        yield return new WaitUntil(() => Time.time > endTime);

        anim.SetBool(knockDownAnim, false);
        KnockedDown = false;
        KnockedDown_Alternate1 = false;

        locked = false;
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void PlayerAvoidKnockedDown()
    {
        anim.Play("knockedDown");
        AvoidedKnockDown = false;
        locked = false;
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
    public bool AvoidedKnockDown
    {
        get => _avoidedKnockDown;
        set => _avoidedKnockDown = value;
    }
    public bool KnockedDown_Alternate1
    {
        get => _knockedDown_alternate1;
        set => _knockedDown_alternate1 = value;
    }
    public Rigidbody RigidBody { get => rigidBody; set => rigidBody = value; }

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
