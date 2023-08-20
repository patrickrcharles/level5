﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Touch = UnityEngine.Touch;

public class AutoPlayerController : MonoBehaviour
{
    // components 
    private Animator anim;
    private AnimatorStateInfo currentStateInfo;
    private GameObject dropShadow;
    private Rigidbody rigidBody;
    private CharacterProfile characterProfile;
    private ShotMeter shotmeter;
    private PlayerSwapAttack playerSwapAttack;
    private PlayerHealth playerHealth;
    private BasketBallAuto basketball;
    private GameStats gameStats;
    CallBallToPlayer callBallToPlayer;

    public bool isCPU;

    // walk speed #review can potentially remove
    [SerializeField]
    private float movementSpeed;
    [SerializeField]
    private float inAirSpeed; // leave serialized
    [SerializeField]
    private float blockSpeed; 
    // leave serialized
    [SerializeField]
    private float attackSpeed; // leave serialized

    // get/set for following at bottom of class
    [SerializeField]
    private bool _facingRight;
    [SerializeField]
    private bool _facingFront;
    private bool _locked;
    [SerializeField]
    private bool _inAir;
    [SerializeField]
    private bool _grounded;
    private bool _knockedDown;
    private bool _takeDamage;
    private bool _avoidedKnockDown;
    private bool canAttack;
    private bool canBlock;

    // player state bools
    //private bool running = false;
    private bool runningToggle;
    public bool hasBasketball;

    // trigger player jump. bool used because activated in fixed update
    // to ensure animaion is synced with camera. camera is updated in fixed update 
    // as well
    private bool jumpTrigger = false;
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
    [SerializeField]
    Vector3 movement;
    float movementHorizontal;
    float movementVertical;
    [SerializeField]
    float distanceToTarget;

    // player take damage display
    Text damageDisplayValueText;
    GameObject damageDisplayObject;
    const string damageDisplayValueName = "player_damage_display_text";

    // control movement speed based on state
    // * NOTE these can be put in a constants file probably unless custom animator
    // need to move these to function to load on start

    public int currentState;
    public int idleState;
    public int walkState;
    public int run;
    public int bWalk;
    public int bIdle;
    public int knockedDownState;
    public int takeDamageState;
    public int specialState;
    public int attackState;
    public int blockState;
    public int inAirDunkState;
    public int inAirHasBasketballFrontState;
    public int inAirHasBasketballSideState;
    public int inAirShootState;
    public int inAirShootFrontState;
    public int jumpState;
    public int inAirHasBasketball;
    public int disintegratedState;
    public bool arrivedAtTarget = false;
    public bool stateWalk = false;
    public bool stateIdle = false;
    public bool stateKnockDown = false;
    [SerializeField]
    private Vector3 targetPosition;
    GameObject basketballRim;
    //player sprite object
    GameObject spriteObject;

    [SerializeField]
    //private float relativePositionToGoal;

    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;

    public bool shootTrigger;
    private float terrainYHeight;

    void Start()
    {
        getAnimatorStateHashes();
        basketball = GetComponent<PlayerIdentifier>().autoBasketball.GetComponent<BasketBallAuto>();
        gameStats = GetComponent<PlayerIdentifier>().autoBasketball.GetComponent<GameStats>();
        inAirHasBasketballFrontState = Animator.StringToHash("base.inair.inair_hasBasketball_front");
        inAirHasBasketballSideState = Animator.StringToHash("base.inair.inair_hasBasketball_side");
        callBallToPlayer = GetComponent<CallBallToPlayer>();
        anim = GetComponentInChildren<Animator>();
        characterProfile = GetComponent<CharacterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GetComponentInChildren<ShotMeter>();
        PlayerHealth = GameLevelManager.instance.PlayerHealth;
        spriteObject = transform.GetComponentInChildren<SpriteRenderer>().gameObject;
        // bball rim vector, used for relative positioning
        bballRimVector = GameLevelManager.instance.BasketballRimVector;

        dropShadow = transform.root.transform.Find("drop_shadow").gameObject;
        FacingRight = true;

        movementSpeed = characterProfile.Speed;
        runningToggle = true;

        if (_knockDownTime == 0) { _knockDownTime = 1.5f; }
        if (_takeDamageTime == 0) { _takeDamageTime = 0.5f; }
        if (blockSpeed == 0) { blockSpeed = 0.2f; }
        //if (attackSpeed == 0) { attackSpeed = 0f; }

        damageDisplayObject = GameObject.Find(damageDisplayValueName);
        damageDisplayValueText = damageDisplayObject.GetComponent<Text>();

        //GameOptions.sniperEnabled = true; // test flag;
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled || GameOptions.sniperEnabled)
        {
            playerSwapAttack = GetComponent<PlayerSwapAttack>();
            damageDisplayObject.GetComponent<Canvas>().worldCamera = Camera.main;
        }
        else
        {
            damageDisplayObject.SetActive(false);
        }
        if (GameOptions.customCamera)
        {
            spriteObject.transform.rotation = Quaternion.Euler(0, 0, 0);
            damageDisplayObject.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        // custom knockdown time for sniper mode
        if (GameOptions.sniperEnabled)
        {
            _knockDownTime = 0.75f;
        }

        _facingRight = true;
        movementSpeed = runMovementSpeed;
        //rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        basketballRim = GameObject.Find("rim");
        // put enemy on the ground. some are spawning up pretty high
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);
    }

    // not affected by framerate
    void FixedUpdate()
    {
        movementHorizontal = movement.x;
        movementVertical = movement.y;
        movement = new Vector3(movementHorizontal, 0, movementVertical) * (movementSpeed * Time.fixedDeltaTime);
        //if (stateWalk
        //    && currentState != knockedDownState)
        //{
        //    StartCoroutine(GoToShootingPosition());
        //}
        if (//stateWalk
            //&& !stateIdle
            Grounded
            && !InAir
            && !(currentState == inAirHasBasketballFrontState || currentState == inAirHasBasketballSideState)
            //&& distanceToTarget > 0.05f
            && currentState != knockedDownState
            && currentState != disintegratedState
            && !arrivedAtTarget)
        {
            if (distanceToTarget > 0.05f)
            {
                moveToPosition(targetPosition);
            }
            if (distanceToTarget <= 0.05f)
            {
                arrivedAtTarget = true;
            }
        }
        if (currentState != specialState)
        {
            IsWalking(movementHorizontal, movementVertical);
        }
        if (jumpTrigger)
        {
            jumpTrigger = false;
            AutoPlayerJump();
        }

        //if (positionMarkerCounter >= positionMarkers.Length )
        //{
        //    positionMarkerCounter = 0;
        //    //targetPosition = positionMarkers[positionMarkerCounter].transform.position;
        //    //targetPosition = postionMarkers[getClosestPositionMarker()].transform.position;
        //}
        // call ball
        if (!hasBasketball
            && !InAir
            && basketball.BasketBallState.CanPullBall
            && !basketball.BasketBallState.Locked
            //&& !BasketBallAuto.instance.BasketBallState.InAir
            && !basketball.BasketBallState.Thrown
            //&& BasketBallAuto.instance.BasketBallState.Grounded
            && Grounded
            && callBallToPlayer.CallEnabled
            //&& !callBallToPlayer.Locked
            && arrivedAtTarget)
        {
            //callBallToPlayer.Locked = true;
            StartCoroutine(CallBall());
        }
    }


    // Update :: once once per frame
    void Update()
    {
        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        // ================== auto player facing goal ==========================
        //relativePositionToGoal = GameLevelManager.instance.BasketballRimVector.x + transform.position.x;
        if (!arrivedAtTarget)
        {
            targetPosition = getClosestPositionMarker();
            //targetPosition = positionMarkers[closestPositionMarkerIndex].transform.position;
        }
        // knocked down
        if (KnockedDown && !Locked)
        {
            Locked = true;
            StartCoroutine(PlayerKnockedDown());
        }
        if (!KnockedDown && TakeDamage && !Locked)
        {
            Locked = true;
            StartCoroutine(PlayerTakeDamage());
        }

        // keep drop shadow on ground at all times
        if (Grounded)
        {
            dropShadow.transform.position = new Vector3(transform.root.position.x, transform.root.position.y+0.01f,
            transform.root.position.z);
        }
        if (!Grounded) // player in air
        {
            terrainYHeight = Terrain.activeTerrain.SampleHeight(transform.position) + 0.02f;
            dropShadow.transform.position = new Vector3(transform.root.position.x, terrainYHeight,
            transform.root.position.z);
        }

        bballRelativePositioning = bballRimVector.x - rigidBody.position.x;
        playerRelativePositioning = rigidBody.position - bballRimVector;
        playerDistanceFromRim = Vector3.Distance(transform.position, new Vector3(bballRimVector.x, transform.position.y, bballRimVector.z));
        playerDistanceFromRimFeet = playerDistanceFromRim * 6;
        distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        // determine if player animation is shooting from or facing basket
        if (Math.Abs(playerRelativePositioning.x) > 2 &&
            Math.Abs(playerRelativePositioning.z) < 2)
        {
            FacingFront = false;
        }
        else
        {
            FacingFront = true;
        }

        // set player shoot anim based on position
        if (FacingFront) // facing straight toward bball goal
        {
            SetPlayerAnim("basketballFacingFront", true);
        }
        else // side of goal, relative postion
        {
            SetPlayerAnim("basketballFacingFront", false);
        }

        // ----- control speed based on commands----------
        // idle, walk, walk with ball state
        if (currentState == idleState || currentState == walkState || currentState == bIdle
            && !InAir
            && !KnockedDown)
        {
            movementSpeed = characterProfile.Speed;
        }
        // if run state
        if (currentState == run && !hasBasketball) 
        {
            movementSpeed = characterProfile.RunSpeed; ;
        }
        // if run state has ball
        if (currentState == bWalk && hasBasketball)
        {
            movementSpeed = characterProfile.RunSpeedHasBall; ;
        }
        // if block state
        if (currentState == attackState || currentState == blockState)
        {
            movementSpeed = blockSpeed;
        }
        // inair state
        if (InAir)//&& currentState != inAirDunkState)
        {
            CheckIsPlayerFacingGoal();
            if (currentState != inAirDunkState)
            {
                movementSpeed = inAirSpeed;
            }
        }
        // -------------- states
        if (!arrivedAtTarget &&/*stateWalk && */distanceToTarget <= 0.05f /*&& !arrivedAtTarget*/ && Grounded)
        {
            //Debug.Log("arrivedAtTarget");
            arrivedAtTarget = true;
            stateWalk = false;
            stateIdle = true;
            //positionMarkerCounter++;
            rigidBody.velocity = Vector3.zero;
        }
        if (!stateWalk && distanceToTarget >= 0.05f && !arrivedAtTarget && Grounded)
        {
            stateWalk = true;
            stateIdle = false;
        }
        //------------------- attack conditions----------------------
        // can attack conditions
        if (Grounded
            && !KnockedDown
            && !hasBasketball
            && !InAir
            && currentState != inAirDunkState)
        {
            canAttack = true;
            canBlock = true;
        }
        else
        {
            canBlock = false;
            canAttack = false;
        }

        /* conditions for auto moving to position and shooting
         * - determine position. this will be affected by game rules
         * game rules : need to look at game rules file. game rules has a list of basketball markers for game mode
         * - go to position
         * - determine if arrived at position.
         * - call ball
         * - jump
         * - shoot
         */
        //========== testing controls
        //testing
        //if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha0))
        //{
        //    stateWalk = !stateWalk;
        //}
        //if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha8) 
        //    && hasBasketball
        //    && !InAir)
        //{
        //    jumpTrigger = !jumpTrigger;
        //}

        //testing
        //======================================

        //------------------ jump -----------------------------------
        if (hasBasketball
            //&& stateIdle
            && arrivedAtTarget
            && Grounded
            && !KnockedDown
            && !jumpTrigger
            && !shootTrigger
            && !InAir)
        {
            //arrivedAtTarget = false;
            jumpTrigger = true;
            arrivedAtTarget = false;
        }

        //------------------ shoot -----------------------------------
        // if has ball, is in air, and pressed shoot button.
        // note -- At top of the jump
        if (InAir
            && hasBasketball
            && !GameOptions.EnemiesOnlyEnabled
            && rigidBody.velocity.y <= 0
            && (currentState == inAirHasBasketballFrontState || currentState == inAirHasBasketballSideState)
            && !shootTrigger)
        {
            shootTrigger = true;
            callBallToPlayer.Locked = true;
            basketball.BasketBallState.Locked = true;
            CheckIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true; // this determines ball launch. find top of the jump
            PlayerShoot();
        }
    }

    private Vector3 getClosestPositionMarker()
    {
        // * note factor in range and other variables for shot type
        // also clutch
        // and time based
        // if( < 1 minute. clutch increases accuracy)

        float distance3 =( Constants.DISTANCE_3point - playerDistanceFromRim) + 0.5f;
        float distance4 = (Constants.DISTANCE_4point - playerDistanceFromRim) + 0.5f;
        float distance7 = (Constants.DISTANCE_7point - playerDistanceFromRim) + 0.5f;
        Vector3 finalDirection = new();
        Vector3 targetPosition = new();
        Vector3 directionOfTravelSeven = new();

        Vector3 directionOfTravel = transform.position - GameLevelManager.instance.BasketballRimVector;
        // set direction of travel to 7pt line based on which side of goal player is on
        if(playerRelativePositioning.x > 0) { directionOfTravelSeven = Vector3.right; }
        else { directionOfTravelSeven = Vector3.left; }
        // cnditions for type of shot
        if (characterProfile.Accuracy3Pt > characterProfile.Accuracy4Pt
             && gameStats.TotalPoints >= GameLevelManager.instance.currentHighScoreTotalPoints)
        {
            finalDirection = directionOfTravel + directionOfTravel.normalized * distance3;
            targetPosition = GameLevelManager.instance.BasketballRimVector + finalDirection;
        }
        if (characterProfile.Accuracy3Pt <= characterProfile.Accuracy4Pt
            || gameStats.TotalPoints < GameLevelManager.instance.currentHighScoreTotalPoints)
        {
            finalDirection = directionOfTravel + directionOfTravel.normalized * distance4;
            targetPosition = GameLevelManager.instance.BasketballRimVector + finalDirection;
        }
        if (characterProfile.Accuracy7Pt > characterProfile.Accuracy4Pt
            || ((GameLevelManager.instance.currentHighScoreTotalPoints - gameStats.TotalPoints) > 14)
            && GameOptions.levelHasSevenPointers)
        {
            finalDirection = directionOfTravelSeven + directionOfTravelSeven.normalized * distance7;
            targetPosition = transform.position + finalDirection;
        }
        //Debug.Log("finalDirection : " + finalDirection);
        //Debug.Log("directionOfTravel : " + finalDirection);

        return targetPosition;
    }

    private void getAnimatorStateHashes()
    {
        idleState = Animator.StringToHash("base.idle");
        walkState = Animator.StringToHash("base.movement.walk");
        run = Animator.StringToHash("base.movement.run");
        bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
        bIdle = Animator.StringToHash("base.movement.basketball_idle");
        knockedDownState = Animator.StringToHash("base.knockedDown");
        takeDamageState = Animator.StringToHash("base.takeDamage");
        specialState = Animator.StringToHash("base.special");
        attackState = Animator.StringToHash("base.attack.attack");
        blockState = Animator.StringToHash("base.attack.block");
        inAirDunkState = Animator.StringToHash("base.inair.inair_dunk");
        inAirHasBasketballFrontState = Animator.StringToHash("inair.inair_hasBasketball_front");
        inAirHasBasketballSideState = Animator.StringToHash("inair.inair_hasBasketball_side");
        inAirShootState = Animator.StringToHash("base.inair.basketball_shoot");
        inAirShootFrontState = Animator.StringToHash("base.inair.basketball_shoot_front");
        jumpState = Animator.StringToHash("base.inair.jump");
        inAirHasBasketball = Animator.StringToHash("base.inair.inair_hasBasketball");
        disintegratedState = Animator.StringToHash("base.disintegrated");
    }

    IEnumerator CallBall()
    {
        yield return new WaitForSeconds(0.5f);
        if (!basketball.BasketBallState.Thrown)
        {
            callBallToPlayer.pullBallToPlayerAuto(basketball.gameObject);
        }
    }
    public void moveToPosition(Vector3 target)
    {
        targetPosition = (target-transform.position).normalized;
        movement = targetPosition * (movementSpeed * Time.deltaTime);
        rigidBody.MovePosition(transform.position +  movement);
    }

    //public void PlayerAttack()
    //{
    //    if (playerCanAttack)
    //    {
    //        // get random close attack if more than one
    //        playerSwapAttack.setCloseAttack();
    //        anim.Play("attack");
    //    }
    //}

    //public void PlayerBlock()
    //{
    //    anim.SetBool("block", true);
    //}

    public void PlayerShoot()
    {
        basketball.shootBasketBall(basketball.BasketBallState.TwoPoints,
            basketball.BasketBallState.ThreePoints,
            basketball.BasketBallState.FourPoints,
            basketball.BasketBallState.SevenPoints);
        arrivedAtTarget = false;
    }

    //public void PlayerSpecial()
    //{
    //    PlayAnim("special");
    //}
    public void CheckIsPlayerFacingGoal()
    {
        if (bballRelativePositioning > 0 && !FacingRight
            && currentState != specialState
            && currentState != attackState)
        {
            Flip();
        }

        if (bballRelativePositioning < 0f && FacingRight
            && currentState != specialState
            && currentState != attackState)
        {
            Flip();
        }
    }

    public void PlayerJump()
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
    void IsWalking(float horizontal, float vertical)
    {
        // if moving
        //if (horizontal > 0f || horizontal < 0f || vertical > 0f || vertical < 0f)
        if (horizontal != 0 || vertical != 0f)
        {
            // not in air
            if (!InAir) // dont want walking animation playing while inAir
            {
                anim.SetBool("walking", true);
                // walking but running toggle is ON
                //if (runningToggle)
                //{
                //    anim.SetBool("moonwalking", true);
                //}
            }
        }
        // not moving
        else
        {
            anim.SetBool("walking", false);
            anim.SetBool("moonwalking", false);
        }

        // player moving right, not facing right
        if (horizontal > 0 && !FacingRight)//&& canMove)
        {
            Flip();
        }
        // player moving left, and facing right
        if (horizontal < 0f && FacingRight)//&& canMove)
        {
            Flip();
        }
    }

    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;

        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled || GameOptions.sniperEnabled)
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
        Locked = false;

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
        Locked = false;

        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public void PlayerAvoidKnockedDown()
    {
        anim.Play("knockedDown");
        AvoidedKnockDown = false;
        Locked = false;
    }

    public void AutoPlayerJump()
    {
        rigidBody.velocity = Vector3.up * characterProfile.JumpForce; //+ (Vector3.forward * rigidBody.velocity.x)) 
        //jumpStartTime = Time.time;

        Shotmeter.MeterStarted = true;
        Shotmeter.MeterStartTime = Time.time;
        // if not dunking, start shot meter
        if (currentState != inAirDunkState)
        {
            Shotmeter.MeterStarted = true;
            Shotmeter.MeterStartTime = Time.time;
        }
    }
    // *NOTE most of these can be in a utility class
    //------------------------- set animator parameters -----------------------
    public void SetPlayerAnim(string animationName, bool isTrue)
    {
        anim.SetBool(animationName, isTrue);
    }

    //------------------------- set animator parameters -----------------------
    public void SetPlayerAnimTrigger(string animationName)
    {
        anim.SetTrigger(animationName);
    }

    //-------------------play animation function ------------------------------
    // provide access to what should be private animator
    public void PlayAnim(string animationName)
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
        rigidBody.constraints = RigidbodyConstraints.FreezeRotation;
    }

    public bool Grounded
    {
        get { return _grounded; }
        set { _grounded = value; }
    }

    public bool InAir
    {
        get { return _inAir; }
        set { _inAir = value; }
    }

    public bool Locked
    {
        get { return _locked; }
        set { _locked = value; }
    }

    public bool FacingFront
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
    public bool TakeDamage { get => _takeDamage; set => _takeDamage = value; }
    public bool FacingRight { get => _facingRight; set => _facingRight = value; }
    public float PlayerDistanceFromRim { get => playerDistanceFromRim; set => playerDistanceFromRim = value; }
    public PlayerHealth PlayerHealth { get => playerHealth; set => playerHealth = value; }
    public CallBallToPlayer CallBallToPlayer { get => callBallToPlayer; set => callBallToPlayer = value; }
    public Rigidbody RigidBody { get => rigidBody; set => rigidBody = value; }
    public CharacterProfile CharacterProfile { get => characterProfile; set => characterProfile = value; }
}
