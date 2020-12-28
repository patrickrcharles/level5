using System;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;
using Touch = UnityEngine.Touch;

public class PlayerController : MonoBehaviour
{
    // components 
    private Animator anim;
    private AnimatorStateInfo currentStateInfo;
    private GameObject dropShadow;
    //private AudioSource audiosource;
    private SpriteRenderer spriteRenderer;
    private Rigidbody rigidBody;
    private CharacterProfile characterProfile;
    private BasketBall basketball;
    private ShotMeter shotmeter;
    private PlayerSwapAttack playerSwapAttack;
    private PlayerHealth playerHealth;

    // walk speed #review can potentially remove
    private float movementSpeed;
    [SerializeField]
    private float inAirSpeed; // leave serialized
    [SerializeField]
    private float blockSpeed; // leave serialized
    //[SerializeField]
    //private float attackSpeed; // leave serialized

    // get/set for following at bottom of class
    private bool _facingRight;
    private bool _facingFront;
    private bool _locked;
    private bool _inAir;
    private bool _grounded;
    private bool _knockedDown;
    private bool _takeDamage;
    private bool _avoidedKnockDown;
    private bool canAttack;
    private bool canBlock;

    // player state bools
    private bool running;
    private bool runningToggle;
    public bool hasBasketball;

    // trigger player jump. bool used because activated in fixed update
    // to ensure animaion is synced with camera. camera is updated in fixed update 
    // as well
    private bool jumpTrigger = false;
    private bool dunkTrigger;

    //public GameObject playerHitbox;

    float bballRelativePositioning; // which side of the player the ball is on
    [SerializeField]
    float playerDistanceFromRim; // player distance from rim
    [SerializeField]
    float playerDistanceFromRimFeet; // player distance from rim

    Vector3 playerRelativePositioning;
    Vector3 bballRimVector;

    // customizable options
    [SerializeField]
    private bool playerCanBlock;
    [SerializeField]
    private bool playerCanAttack;
    [SerializeField]
    float _knockDownTime;
    [SerializeField]
    float _takeDamageTime;

    // movement variables
    Vector3 movement;
    float movementHorizontal;
    float movementVertical;

    // touch vars
    Touch touch;
    Vector2 startTouchPosition = new Vector2(0, 0);

    Text damageDisplayValueText;
    GameObject damageDisplayObject;
    const string damageDisplayValueName = "player_damage_display_text";

    // control movement speed based on state
    public int currentState;
    public int idleState = Animator.StringToHash("base.idle");
    public int walkState = Animator.StringToHash("base.movement.walk");
    public int run = Animator.StringToHash("base.movement.run");
    public int bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
    public int bIdle = Animator.StringToHash("base.movement.basketball_idle");
    public int knockedDownState = Animator.StringToHash("base.knockedDown");
    public int takeDamageState = Animator.StringToHash("base.takeDamage");
    public int specialState = Animator.StringToHash("base.special");
    public int attackState = Animator.StringToHash("base.attack.attack");
    public int blockState = Animator.StringToHash("base.attack.block");
    public int inAirDunkState = Animator.StringToHash("base.inair.inair_dunk");
    public int dunkState = Animator.StringToHash("base.inair.dunk");


    void Start()
    {
        //audiosource = GameLevelManager.instance.GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        basketball = GameLevelManager.instance.Basketball;
        characterProfile = GetComponent<CharacterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GetComponentInChildren<ShotMeter>();
        playerHealth = GameLevelManager.instance.PlayerHealth;

        // bball rim vector, used for relative positioning
        bballRimVector = GameLevelManager.instance.BasketballRimVector;

        dropShadow = transform.root.transform.Find("drop_shadow").gameObject;
        //playerHitbox.SetActive(true);
        facingRight = true;
        //canMove = true;
        movementSpeed = characterProfile.Speed;
        runningToggle = true;
        //if (_knockDownTime == 0) { _knockDownTime = 1.5f; }
        //if (_takeDamageTime == 0) { _takeDamageTime = 0.5f; }
        //if (attackSpeed == 0) { attackSpeed = 0f; }
        if (blockSpeed == 0) { blockSpeed = 0.2f; }

        //dunkPositionLeft = GameObject.Find("dunk_position_left").transform.position;
        //dunkPositionRight = GameObject.Find("dunk_position_right").transform.position;

        damageDisplayObject = GameObject.Find(damageDisplayValueName);
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            playerSwapAttack = GetComponent<PlayerSwapAttack>();            
            damageDisplayValueText = damageDisplayObject.GetComponent<Text>();
            damageDisplayObject.GetComponent<Canvas>().worldCamera = Camera.main;
        }
        else
        {
            damageDisplayObject.SetActive(false);
        }
    }

    // not affected by framerate
    void FixedUpdate()
    {

        //------MOVEMENT---------------------------
        if (!KnockedDown && currentState != takeDamageState)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

            if (Input.touchCount > 0)
            {
                Touch touch = Input.touches[0];
                if (touch.phase == TouchPhase.Began)
                {
                    startTouchPosition = touch.position;
                }

                movementHorizontal = GameLevelManager.instance.Joystick.Horizontal;
                movementVertical = GameLevelManager.instance.Joystick.Vertical;

                //percent of finger move distance from start to end range that will max speed of movement
                float XrangePercent = Mathf.Abs((touch.position.x - startTouchPosition.x) / screenXRange);
                float YrangePercent = Mathf.Abs((touch.position.y - startTouchPosition.y) / screenYRange);

                 //if max finger move distance not achieved, multiply by percent of distance so far
                if (XrangePercent < 1)
                {
                    movementHorizontal *= XrangePercent;
                }
                if (YrangePercent < 1)
                {
                    movementVertical *= YrangePercent;
                }
            }
            if (Input.touchCount == 0)
            {
                movementHorizontal = 0;
                movementVertical = 0;
            }

            //#if UNITY_ANDROID && !UNITY_EDITOR
            //#if UNITY_ANDROID 
#endif

#if UNITY_STANDALONE || UNITY_EDITOR

            movementHorizontal = GameLevelManager.instance.Controls.Player.movement.ReadValue<Vector2>().x;
            movementVertical = GameLevelManager.instance.Controls.Player.movement.ReadValue<Vector2>().y;
            //movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.deltaTime);
            //movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.fixedDeltaTime);

#endif

            //movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.deltaTime);
            movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.fixedUnscaledDeltaTime);
            //movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.fixedDeltaTime);
            // check jump trigger and execute jump
            if (jumpTrigger)
            {
                jumpTrigger = false;
                playerJump();
            }
            if (dunkTrigger 
                && (currentState != inAirDunkState || currentState != inAirDunkState)
                && !inAir
                && Grounded
                && !locked)
            {
                dunkTrigger = false;
                PlayerDunk.instance.playerDunk();
            }

            if (currentState != specialState)
            {
                //rigidBody.MovePosition(transform.position + movement);
                transform.Translate(movement);
                //isWalking(movement);
                isWalking(movementHorizontal, movementVertical);
            }
        }
    }


    // Update :: once once per frame
    void Update()
    {

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        // knocked down
        if (KnockedDown && !locked)
        {
            locked = true;
            StartCoroutine(PlayerKnockedDown());
        }
        if (!KnockedDown && TakeDamage && !locked)
        {
            locked = true;
            StartCoroutine(PlayerTakeDamage());
        }

        // keep drop shadow on ground at all times
        if (Grounded)
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
                transform.root.position.z);
        }
        if (!Grounded) // player in air
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
            transform.root.position.z);
        }

        bballRelativePositioning = bballRimVector.x - rigidBody.position.x;
        playerRelativePositioning = rigidBody.position - bballRimVector;

        playerDistanceFromRim = Vector3.Distance(transform.position, new Vector3(bballRimVector.x, 0, bballRimVector.z));
        playerDistanceFromRimFeet = playerDistanceFromRim * 6;

        // if run input or run toggle on
        if (GameLevelManager.instance.Controls.Player.run.ReadValue<float>() == 1 //if button is held
            && !inAir
            && !KnockedDown
            && rigidBody.velocity.magnitude > 0.1f
            && !locked)
        {
            running = true;
            anim.SetBool("moonwalking", true);
        }
        else
        {
            running = false;
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
            movementSpeed = characterProfile.Speed;
        }
        // if run state
        if (currentState == run && !hasBasketball) //|| (runningToggle || running) )
        {
            movementSpeed = characterProfile.RunSpeed; ;
        }
        // if run state
        if (currentState == bWalk && hasBasketball) //|| (runningToggle || running) )
        {
            movementSpeed = characterProfile.RunSpeedHasBall; ;
        }
        if (currentState == attackState || currentState == blockState)
        {
            movementSpeed = blockSpeed;
        }
        // inair state
        if (inAir )//&& currentState != inAirDunkState)
        {
            checkIsPlayerFacingGoal();
            if (currentState != inAirDunkState)
            {
                movementSpeed = inAirSpeed;
            }
        }
        if (Grounded
            && !KnockedDown
            && !hasBasketball
            && !inAir
            && currentState != dunkState)
        {
            canAttack = true;
            canBlock = true;
        }
        else
        {
            canBlock = false;
            canAttack = false;
        }

#if UNITY_STANDALONE || UNITY_EDITOR
        //------------------ jump -----------------------------------
        if (GameLevelManager.instance.Controls.Player.jump.triggered
            //&& !GameLevelManager.instance.Controls.Player.shoot.triggered
            && hasBasketball
            && Grounded
            && !KnockedDown
            && !GameOptions.EnemiesOnlyEnabled
            && !inAir)
        {
            if (playerDistanceFromRimFeet < PlayerDunk.instance.DunkRangeFeet
                && PlayerDunk.instance != null)
            {
                dunkTrigger = true;
            }
            else
            {
                jumpTrigger = true;
            }
        }
        //------------------ shoot -----------------------------------
        // if has ball, is in air, and pressed shoot button.
        if (inAir
            && hasBasketball
            && GameLevelManager.instance.Controls.Player.shoot.triggered
            && !GameOptions.EnemiesOnlyEnabled
            && currentState != inAirDunkState)
        {
            //Debug.Log("shoot");
            CallBallToPlayer.instance.Locked = true;
            basketball.BasketBallState.Locked = true;
            checkIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true;
            playerShoot();
        }
        //------------------ attack -----------------------------------

        if (GameLevelManager.instance.Controls.Player.shoot.triggered
            && GameLevelManager.instance.Controls.Player.jump.ReadValue<float>() == 1
            && !hasBasketball
            && canAttack
            && GameOptions.enemiesEnabled)
        {
            playerAttack();
        }
        else
        {
            anim.SetBool("attack", false);
        }

        if (GameLevelManager.instance.Controls.Player.jump.ReadValue<float>() == 1
            //&& GameLevelManager.instance.Controls.Player.run.ReadValue<float>() == 1
            && !hasBasketball
            && canBlock
            && GameOptions.enemiesEnabled
            && playerHealth.Block > 0)
        {
            if (playerCanBlock)
            {
                playerBlock();
            }
            if (!playerCanBlock)
            {
                jumpTrigger = true;
            }
        }
        else
        {
            // double check touch input not being used
            if(!TouchInputController.instance.HoldDetected)
            {
                anim.SetBool("block", false);
            }
        }

        //------------------ special -----------------------------------
        if (GameLevelManager.instance.Controls.Player.special.triggered
            && !inAir
            && Grounded
            && !KnockedDown
            && GameOptions.enemiesEnabled)
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
#endif 
    }


    public void touchControlJumpOrShoot(Vector2 touchPosition)
    {
        if (Grounded
            && !KnockedDown
            && hasBasketball
            && touchPosition.x > (Screen.width / 2))
        {
            playerJump();          
        }
        // if has ball, is in air, and pressed shoot button.
        // shoot ball
        if (inAir
            && hasBasketball
            //&& !IsSetShooter
            && touchPosition.x > (Screen.width / 2))
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
            && Grounded
            && !CallBallToPlayer.instance.Locked
            && touchPosition.x > (Screen.width / 2)
            && !GameOptions.hardcoreModeEnabled)
        {
            CallBallToPlayer.instance.Locked = true;
            CallBallToPlayer.instance.pullBallToPlayer();
            CallBallToPlayer.instance.Locked = false;
        }
    }
    public void playerAttack()
    {
        if (playerCanAttack)
        {
            // get random close attack if more than one
            playerSwapAttack.setCloseAttack();
            anim.Play("attack");
        }
    }

    public void playerBlock()
    {
        anim.SetBool("block", true);
    }

    public void playerShoot()
    {
        basketball.shootBasketBall();
    }

    public void playerSpecial()
    {
        playAnim("special");
    }
    public void checkIsPlayerFacingGoal()
    {
        if (bballRelativePositioning > 0 && !facingRight
            && currentState != specialState
            && currentState != attackState)
        {
            Flip();
        }

        if (bballRelativePositioning < 0f && facingRight
            && currentState != specialState
            && currentState != attackState)
        {
            Flip();
        }
    }

    public void playerJump()
    {
        rigidBody.velocity = Vector3.up * characterProfile.JumpForce; //+ (Vector3.forward * rigidBody.velocity.x)) 
        //jumpStartTime = Time.time;

        Shotmeter.MeterStarted = true;
        Shotmeter.MeterStartTime = Time.time;
        //// if not dunking, start shot meter
        //if (currentState != inAirDunkState)
        //{
        //    Shotmeter.MeterStarted = true;
        //    Shotmeter.MeterStartTime = Time.time;
        //}
    }

    //-----------------------------------Walk function -----------------------------------------------------------------------
    //void isWalking(Vector3 movement)
    void isWalking(float horizontal, float vertical)
    {
        // if moving
        //if (horizontal > 0f || horizontal < 0f || vertical > 0f || vertical < 0f)
        if (horizontal != 0 || vertical != 0f)
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
            //moonwalkAudio.enabled = false;
            running = false;
        }

        // player moving right, not facing right
        if (horizontal > 0 && !facingRight)//&& canMove)
        {
            Flip();
        }
        // player moving left, and facing right
        if (horizontal < 0f && facingRight)//&& canMove)
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

        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            Vector3 damageScale = damageDisplayObject.transform.localScale;
            damageScale.x *= -1;
            damageDisplayObject.transform.localScale = damageScale;
        }
    }

    // ------------------------------- take damage -------------------------------------------------------
    public IEnumerator PlayerTakeDamage()
    {
        //Debug.Log("PlayerTakeDamage");
        rigidBody.constraints =
        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        anim.SetBool("takeDamage", true);
        anim.Play("takeDamage");

        float startTime = Time.time;
        float endTime = startTime + _takeDamageTime;
        yield return new WaitUntil(() => Time.time > endTime);
        anim.SetBool("takeDamage", false);
        yield return new WaitUntil(() => currentState != takeDamageState);

        TakeDamage = false;
        KnockedDown = false;
        locked = false;

        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator PlayerFreezeForXSeconds(float time)
    {
        Debug.Log("freeze player");
        rigidBody.constraints =
        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        anim.SetBool("takeDamage", true);
        anim.Play("takeDamage");

        float startTime = Time.time;
        float endTime = startTime + time;
        yield return new WaitUntil(() => Time.time > endTime);
        anim.SetBool("takeDamage", false);
        yield return new WaitUntil(() => currentState != takeDamageState);

        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public IEnumerator PlayerKnockedDown()
    {
        //Debug.Log("PlayerKnockedDown");
        rigidBody.constraints =
        RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        anim.SetBool("knockedDown", true);
        anim.Play("knockedDown");
        //yield return new WaitUntil(() => currentState == knockedDownState); // anim started

        float startTime = Time.time;
        float endTime = startTime + _knockDownTime;
        yield return new WaitUntil(() => Time.time > endTime);
        anim.SetBool("knockedDown", false);
        yield return new WaitUntil(() => currentState != knockedDownState);

        KnockedDown = false;
        TakeDamage = false;
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
    // ----------------------- freeze player postion ------------------------
    public void FreezePlayerPosition()
    {
        //Debug.Log("FreezePlayerPosition");
            //rigidBody.velocity = Vector3.zero;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezeRotationZ
            | RigidbodyConstraints.FreezePositionX
            | RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezePositionZ;
    }

    public void UnFreezePlayerPosition()
    {
        //Debug.Log("UnFreezePlayerPosition");
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    // #todo find all these messageDisplay coroutines and move to seprate generic class MessageLog od something
    public void toggleRun()
    {
        runningToggle = !runningToggle;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "running toggle = " + runningToggle;

        // turn off text display after 5 seconds
        StartCoroutine(BasketBall.instance.turnOffMessageLogDisplayAfterSeconds(3));
    }

    public bool IsSpecialState()
    {
        return currentState == specialState;
    }

    public bool Grounded
    {
        get { return _grounded; }
        set { _grounded = value; }
    }

    public bool inAir
    {
        get { return _inAir; }
        set { _inAir = value; }
    }

    public bool locked
    {
        get { return _locked; }
        set { _locked = value; }
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

    public Rigidbody RigidBody { get => rigidBody; set => rigidBody = value; }
    //public float MovementSpeed { get => movementSpeed; set => movementSpeed = value; }
    public bool TakeDamage { get => _takeDamage; set => _takeDamage = value; }
    public int CurrentState { get => currentState; set => currentState = value; }
    public int AttackState { get => attackState; set => attackState = value; }
    public int BlockState { get => blockState; set => blockState = value; }
    public int SpecialState { get => specialState; set => specialState = value; }
    public bool facingRight { get => _facingRight; set => _facingRight = value; }
    public bool CanAttack { get => canAttack; set => canAttack = value; }
    public bool PlayerCanBlock { get => playerCanBlock; set => playerCanBlock = value; }
    public bool CanBlock { get => canBlock; set => canBlock = value; }
    public Animator Anim { get => anim; set => anim = value; }
    //public AudioSource Audiosource { get => audiosource; set => audiosource = value; }
    public Text DamageDisplayValueText { get => damageDisplayValueText; set => damageDisplayValueText = value; }
    public float PlayerDistanceFromRim { get => playerDistanceFromRim; set => playerDistanceFromRim = value; }
}
