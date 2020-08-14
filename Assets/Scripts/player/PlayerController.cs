using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System;
using Touch = UnityEngine.Touch;

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
    static int run = Animator.StringToHash("base.movement.run");
    static int bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
    static int bIdle = Animator.StringToHash("base.movement.basketball_idle");
    static int knockedDownState = Animator.StringToHash("base.takeDamage.knockedDown");

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
    //public FloatingJoystick joystick;
    //public Button jumpButton;
    //public Button shootButton;

    float movementHorizontal;
    float movementVertical;

    Touch touch;

    //PlayerControls controls;

    private void Awake()
    {
        //joystick = GameLevelManager.instance.Joystick;
        //Debug.Log("GameLevelManager.instance.Joystick : " + GameLevelManager.instance.Joystick.enabled);
        ////controls = new PlayerControls();
        //Debug.Log("joystick found   active: " + joystick.enabled);
    }

    void Start()
    {
        //joystick = GameLevelManager.instance.Joystick;
        //Debug.Log("GameLevelManager.instance.Joystick : " + GameLevelManager.instance.Joystick.enabled);
        ////controls = new PlayerControls();
        //Debug.Log("joystick found   active: " + joystick.enabled);

        //moonwalkAudio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        basketball = GameLevelManager.instance.Basketball;
        shooterProfile = GetComponent<ShooterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GetComponentInChildren<ShotMeter>();
        //joystick = GameLevelManager.instance.Joystick;

        // bball rim vector, used for relative positioning
        bballRimVector = GameLevelManager.instance.BasketballRimVector;

        //// #note used for testing scenes
        //if (GameOptions.gameModeHasBeenSelected)
        //{
        //    setShooterProfileStats();
        //}

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
            if (GameLevelManager.instance.Joystick != null)
            {
                movementHorizontal = GameLevelManager.instance.Joystick.Horizontal;
                movementVertical = GameLevelManager.instance.Joystick.Vertical;
                movement = new Vector3(movementHorizontal, 0, movementVertical) * movementSpeed * Time.deltaTime;
            }

            // Input Sytem 1.0.0 controls variables ---------------------------------------------------------------
            //if (joystick == null)
            //{
            //    movementInput = GameLevelManager.Instance.Controls.Player.movement.ReadValue<Vector2>();
            //    movement = new Vector3(movementInput.x, 0, movementInput.y) * movementSpeed * Time.deltaTime;

            //}
            if (GameLevelManager.instance.Joystick == null)
            {
                movementHorizontal = GameLevelManager.instance.Controls.Player.movement.ReadValue<Vector2>().x;
                movementVertical = GameLevelManager.instance.Controls.Player.movement.ReadValue<Vector2>().y;
                movement = new Vector3(movementHorizontal, 0, movementVertical) * movementSpeed * Time.deltaTime;
            }

            //Input Sytem 1.0.0 Touch controls variables-------------------------------------------------------------- -
            //movementHorizontal = GameLevelManager.Instance.Controls.PlayerTouch.movement.ReadValue<Vector2>().x;
            //movementVertical = GameLevelManager.Instance.Controls.PlayerTouch.movement.ReadValue<Vector2>().y;

            //Debug.Log("movementHorizontal " + movementHorizontal);
            //Debug.Log("movementVertical " + movementVertical);
            //if (Input.touchCount > 0)
            //{
            //    touch = Input.touches[0];
            //    Debug.Log(touch.phase);
            //    Debug.Log(touch.deltaPosition.x);
            //    Debug.Log(touch.deltaPosition.y);
            //    Debug.Log(touch.position);
            //    Debug.Log(touch.radius);

            //    if (touch.phase == TouchPhase.Moved)
            //    {
            //        Vector3 newPosition = Input.touches[0].position - touch.position;
            //        //touch.position = Input.touches[0].position;

            //        movement = new Vector3(newPosition.x, 0, newPosition.y) * movementSpeed * Time.deltaTime;

            //        if (touch.phase == TouchPhase.Ended)
            //        {
            //            Debug.Log("TouchPhase.Ended ");
            //            rigidBody.velocity = Vector2.zero;
            //        }
            //    }

            //}


            //-----------------------------------------------------------------------------------------------------
            rigidBody.MovePosition(transform.position + movement);
            isWalking(movement);
        }
    }


    public void TouchControlJump()
    {
        if (grounded && !KnockedDown)
        {
            playerJump();
        }
    }

    public void touchControlShoot()
    {
        // if has ball, is in air, and pressed shoot button.
        // shoot ball
        if (inAir
            && hasBasketball
            && !IsSetShooter)
        {
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
            CallBallToPlayer.instance.Locked = true;
            CallBallToPlayer.instance.pullBallToPlayer();
            CallBallToPlayer.instance.Locked = false;
        }
    }

    // Update :: once once per frame
    void Update()
    {
        //if (GameLevelManager.Instance.Controls.PlayerTouch.jump.triggered)
        //{
        //    Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        //    messageText.text = " touch input jump";
        //    // turn off text display after 5 seconds
        //    StartCoroutine(basketball.turnOffMessageLogDisplayAfterSeconds(2));
        //}
        //if (GameLevelManager.Instance.Controls.PlayerTouch.shoot.triggered)
        //{
        //    Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        //    messageText.text = " touch input shoot";
        //    // turn off text display after 5 seconds
        //    StartCoroutine(basketball.turnOffMessageLogDisplayAfterSeconds(2));
        //}
        // knocked down
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
        // avoid knockdown
        if (AvoidedKnockDown && !locked)
        {
            locked = true;
            //rigidBody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

            // coroutine that holds animation with WaitUntil knock down time is through
            //StartCoroutine(PlayerAvoidKnockedDown());
            PlayerAvoidKnockedDown();
        }

        // keep drop shadow on ground at all times
        if (grounded)
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
                transform.root.position.z);
        }
        if (!grounded) // player in air
        {
            //dropShadow.transform.position = new Vector3(transform.root.position.x, transform.root.position.y,
            //    transform.root.position.z);
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
            transform.root.position.z);
        }

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        bballRelativePositioning = bballRimVector.x - rigidBody.position.x;
        playerRelativePositioning = rigidBody.position - bballRimVector;

        //playerDistanceFromRim = Vector3.Distance(transform.position, bballRimVector);

        // if run input or run toggle on
        if ((GameLevelManager.instance.Controls.Player.run.ReadValue<float>() == 1 //if button is held
            && canMove
            && !inAir
            && !KnockedDown
            && !locked))
        {
            running = true;
            anim.SetBool("moonwalking", true);
        }
        else
        {
            running = false;
        }

        //player reaches peak of jump. this will be useful for creating AI with auto shoot
        //if (rigidBodyYVelocity > 0 && inAir)
        //{
        //    jumpPeakReached = true;
        //}
        //else
        //{
        //    jumpPeakReached = false;
        //}

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
            setPlayerAnim("basketballFacingFront", true);
        }
        else // side of goal, relative postion
        {
            setPlayerAnim("basketballFacingFront", false);
        }

        // ----- control speed based on commands----------
        // idle, walk, walk with ball state
        if (currentState == idleState || currentState == walkState || currentState == bIdle
            && !inAir
            && !KnockedDown)
        {
                movementSpeed = shooterProfile.Speed;
        }
        // if run state
        if (currentState == run ) //|| (runningToggle || running) )
        {
            movementSpeed = shooterProfile.RunSpeed; ;
        }
        // inair state
        if (inAir)
        {
            checkIsPlayerFacingGoal();
            movementSpeed = inAirSpeed;
        }

        //------------------ jump -----------------------------------
        if (GameLevelManager.instance.Controls.Player.jump.triggered
            && !GameLevelManager.instance.Controls.Player.shoot.triggered
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
            && GameLevelManager.instance.Controls.Player.shoot.triggered
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
            && GameLevelManager.instance.Controls.Player.shoot.triggered
            && IsSetShooter)
        {
            basketball.BasketBallState.Locked = true;
            checkIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true;
            playerShoot();
        }

        //------------------ special -----------------------------------
        if (GameLevelManager.instance.Controls.Player.special.triggered
            && !inAir
            && grounded
            && !KnockedDown)
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
    }

    //-----------------------------------Walk function -----------------------------------------------------------------------
    void isWalking(Vector3 movement)
    {
        // if moving
        if (movement.x > 0f || movement.x < 0f || movement.z > 0f || movement.z < 0f)
        {
            // not in air
            if (!inAir) // dont want walking animation playing while inAir
            {
                anim.SetBool("walking", true);
                // walking but running toggle is ON
                if (runningToggle)
                {
                    anim.SetBool("moonwalking", true);
                }
            }
        }
        // not moving
        else
        {
            anim.SetBool("walking", false);
            anim.SetBool("moonwalking", false);
            running = false;
        }

        // player moving right, not facing right
        if (movement.x > 0 && !facingRight && canMove)
        {
            Flip();
        }
        // player moving left, and facing right
        if (movement.x < 0f && facingRight && canMove)
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
