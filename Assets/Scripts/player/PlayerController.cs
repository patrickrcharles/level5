using System;
using System.Collections;
using System.Data.SqlTypes;
using UnityEngine;
using UnityEngine.UI;
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
    Rigidbody rigidBody;
    CharacterProfile characterProfile;
    BasketBall basketball;
    ShotMeter shotmeter;

    // walk speed #review can potentially remove
    private float movementSpeed;
    [SerializeField]
    private float inAirSpeed; // leave serialized
    //[SerializeField]
    //private float attackSpeed; // leave serialized
    [SerializeField]
    private float blockSpeed; // leave serialized

    // player state bools
    private bool running;
    private bool runningToggle;
    public bool hasBasketball;

    // #note this is work a work in progress feature. it works but it's bugged
    //[SerializeField]
    //private bool isSetShooter;
    //public bool IsSetShooter => isSetShooter;
    //public bool canMove; // #todo add player knock downs, this could be used
    //                     // will be useful for when play knockdowns implemented
    public GameObject playerHitbox;

    Vector3 bballRimVector;
    float bballRelativePositioning;
    Vector3 playerRelativePositioning;
    public float playerDistanceFromRim;


    // control movement speed based on state
    int currentState;
    int idleState = Animator.StringToHash("base.idle");
    int walkState = Animator.StringToHash("base.movement.walk");
    int run = Animator.StringToHash("base.movement.run");
    int bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
    int bIdle = Animator.StringToHash("base.movement.basketball_idle");
    int knockedDownState = Animator.StringToHash("base.knockedDown");
    int shootSideState = Animator.StringToHash("base.knockedDown");
    int takeDamageState = Animator.StringToHash("base.takeDamage");
    int specialState = Animator.StringToHash("base.special");
    int attackState = Animator.StringToHash("base.attack.attack");
    int blockState = Animator.StringToHash("base.attack.block");

    // get/set for following at bottom of class
    [SerializeField]
    private bool _facingRight;
    private bool _facingFront;
    private bool _locked;
    private bool _jump;
    private bool _inAir;
    private bool _grounded;
    [SerializeField]
    private bool _knockedDown;
    private bool _knockedDown_alternate;
    [SerializeField]
    private bool _takeDamage;
    private bool _avoidedKnockDown;
    [SerializeField]
    private bool canAttack;
    [SerializeField]
    private bool canBlock;

    [SerializeField]
    private bool playerCanBlock;

    [SerializeField]
    float _knockDownTime;
    [SerializeField]
    float _takeDamageTime;

    //#review no longer use, but some it could be useful
    //public float initialHeight, finalHeight;
    //public bool jumpPeakReached = false;
    private float _rigidBodyYVelocity;

    // used to calculate shot meter time
    //public float jumpStartTime;
    //public float jumpEndTime;

    //Vector2 movementInput;
    Vector3 movement;

    float movementHorizontal;
    float movementVertical;


    bool jumpTrigger = false;

    Touch touch;

    void Start()
    {

        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        basketball = GameLevelManager.instance.Basketball;
        characterProfile = GetComponent<CharacterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GetComponentInChildren<ShotMeter>();

        // bball rim vector, used for relative positioning
        bballRimVector = GameLevelManager.instance.BasketballRimVector;

        dropShadow = transform.root.transform.Find("drop_shadow").gameObject;
        playerHitbox.SetActive(true);
        facingRight = true;
        //canMove = true;
        movementSpeed = characterProfile.Speed;
        runningToggle = true;
        //if (_knockDownTime == 0) { _knockDownTime = 1.5f; }
        //if (_takeDamageTime == 0) { _takeDamageTime = 0.5f; }
        //if (attackSpeed == 0) { attackSpeed = 0f; }
        if (blockSpeed == 0) { blockSpeed = 0.2f; }

    }

    // not affected by framerate
    void FixedUpdate()
    {
        //------MOVEMENT---------------------------
        if (!KnockedDown)
        {
#if UNITY_ANDROID && !UNITY_EDITOR

                        movementHorizontal = GameLevelManager.instance.Joystick.Horizontal;
                        movementVertical = GameLevelManager.instance.Joystick.Vertical;
                        movement = new Vector3(movementHorizontal, 0, movementVertical) * movementSpeed * Time.deltaTime;
                        //movement = new Vector3(movementHorizontal, 0, movementVertical) * movementSpeed * Time.fixedDeltaTime;
#endif

#if UNITY_STANDALONE || UNITY_EDITOR

                        movementHorizontal = GameLevelManager.instance.Controls.Player.movement.ReadValue<Vector2>().x;
                        movementVertical = GameLevelManager.instance.Controls.Player.movement.ReadValue<Vector2>().y;
                        movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.deltaTime);
                        //movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.fixedDeltaTime);
#endif

            if (currentState != specialState)
            {
                //rigidBody.MovePosition(transform.position + movement);
                transform.Translate(movement);
                isWalking(movement);
            }
            //// jump
            //if (jumpTrigger)
            //{

            //}
        }
    }

    public void TouchControlJump()
    {
        if (grounded && !KnockedDown)
        {
            playerJump();
        }
    }

    public void touchControlJumpOrShoot(Vector2 touchPosition)
    {
        if (grounded
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
            && grounded
            && !CallBallToPlayer.instance.Locked
            && touchPosition.x > (Screen.width / 2))
        {
            CallBallToPlayer.instance.Locked = true;
            CallBallToPlayer.instance.pullBallToPlayer();
            CallBallToPlayer.instance.Locked = false;
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
        if (grounded)
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
                transform.root.position.z);
        }
        if (!grounded) // player in air
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, 0.01f,
            transform.root.position.z);
        }

        bballRelativePositioning = bballRimVector.x - rigidBody.position.x;
        playerRelativePositioning = rigidBody.position - bballRimVector;
        playerDistanceFromRim = Vector3.Distance(transform.position, bballRimVector);

        // if run input or run toggle on
        if (GameLevelManager.instance.Controls.Player.run.ReadValue<float>() == 1 //if button is held
            && !inAir
            && !KnockedDown
            && rigidBody.velocity.magnitude > 0.1f
            && !locked)
        {
            Debug.Log("rigidBody.velocity.sqrMagnitude : " + rigidBody.velocity.sqrMagnitude);
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
        if (inAir)
        {
            checkIsPlayerFacingGoal();
            movementSpeed = inAirSpeed;
        }
        if (grounded
            && !KnockedDown
            && !hasBasketball
            && !inAir)
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
            && grounded
            && !KnockedDown)
        {
            playerJump();
        }
        //------------------ shoot -----------------------------------
        // if has ball, is in air, and pressed shoot button.
        if (inAir
            && hasBasketball
            && GameLevelManager.instance.Controls.Player.shoot.triggered)
        {
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
            && canAttack)
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
            && canBlock)
        {
            if (playerCanBlock)
            {
                playerBlock();
            }
            if (!playerCanBlock)
            {
                playerJump();
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
#endif
    }

    public void playerAttack()
    {
        anim.Play("attack");
    }

    public void playerBlock()
    {
        //anim.Play("block");
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
            //moonwalkAudio.enabled = false;
            running = false;
        }

        // player moving right, not facing right
        if (movement.x > 0 && !facingRight)//&& canMove)
        {
            Flip();
        }
        // player moving left, and facing right
        if (movement.x < 0f && facingRight)//&& canMove)
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

    ////can be used as generic turn off audio by adding paramter to pass (Audio audioToTurnOff)
    //public void turnOffMoonWalkAudio()
    //{
    //    moonwalkAudio.enabled = false;
    //}

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
    public bool KnockedDown_Alternate
    {
        get => _knockedDown_alternate;
        set => _knockedDown_alternate = value;
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
    public bool PlayerCanBlock1 { get => playerCanBlock; set => playerCanBlock = value; }

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
}
