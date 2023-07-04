using System;
using System.Collections;
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

    [SerializeField]
    bool isCPU;
    [SerializeField]
    bool isPlayer1;
    [SerializeField]
    bool isPlayer2;
    [SerializeField]
    bool isPlayer3;
    [SerializeField]
    bool isPlayer4;

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
    [SerializeField]
    private bool jumpTrigger = false;
    //private bool dunkTrigger;

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
    public int inAirHasBasketballFrontState = Animator.StringToHash("inair.inair_hasBasketball_front");
    public int inAirHasBasketballSideState = Animator.StringToHash("inair.inair_hasBasketball_side");
    public int inAirShootState = Animator.StringToHash("base.inair.basketball_shoot");
    public int inAirShootFrontState = Animator.StringToHash("base.inair.basketball_shoot_front");
    public int jumpState = Animator.StringToHash("base.inair.jump");
    public int inAirHasBasketball = Animator.StringToHash("base.inair.inair_hasBasketball");
    public int disintegratedState = Animator.StringToHash("base.disintegrated");
    [SerializeField]
    private bool arrivedAtTarget;
    [SerializeField]
    public bool stateWalk = false;
    [SerializeField]
    public bool stateIdle = false;
    public bool stateKnockDown = false;

    Vector3 randomShootingPosition;

    [SerializeField]
    private Vector3 targetPosition;
    //[SerializeField]
    //GameObject prefabClone;
    [SerializeField]
    GameObject basketballRim;

    [SerializeField]
    private float relativePositionToGoal;

    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;

    public bool facingRight;
    public bool shootTrigger;

    [SerializeField]
    GameObject[] postionMarkers;
    [SerializeField]
    int positionMarkerCounter = 0;
    void Start()
    {
        inAirHasBasketballFrontState = Animator.StringToHash("base.inair.inair_hasBasketball_front");
        inAirHasBasketballSideState = Animator.StringToHash("base.inair.inair_hasBasketball_side");
        //audiosource = GameLevelManager.instance.GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        //spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        //basketball = GameLevelManager.instance.Basketball;
        characterProfile = GetComponent<CharacterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        Shotmeter = GetComponentInChildren<ShotMeter>();
        PlayerHealth = GameLevelManager.instance.PlayerHealth;

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

        // custom knockdown time for sniper mode
        if (GameOptions.sniperEnabled)
        {
            _knockDownTime = 0.75f;
        }

        facingRight = true;
        movementSpeed = walkMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        basketballRim = GameObject.Find("rim");
        // put enemy on the ground. some are spawning up pretty high
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);

        postionMarkers = GameObject.FindGameObjectsWithTag("shot_marker");
        
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
        if (stateWalk
            && !stateIdle
            && Grounded
            && !InAir
            && !(currentState == inAirHasBasketballFrontState || currentState == inAirHasBasketballSideState)
            //&& distanceToTarget > 0.05f
            && currentState != knockedDownState
            && currentState != disintegratedState
            && !arrivedAtTarget)
        {
            moveToPosition(postionMarkers[positionMarkerCounter].transform.position);
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
        if (positionMarkerCounter < postionMarkers.Length)
        {
            targetPosition = postionMarkers[positionMarkerCounter].transform.position;
        }
        else
        {
            positionMarkerCounter = 0;
        }
        //if (!hasBasketball
        //    && Grounded
        //    && !InAir
        //    && currentState != inAirHasBasketball
        //    && currentState != inAirHasBasketballFrontState
        //    && currentState != inAirHasBasketballSideState
        //    && distanceToTarget >= 0.1f
        //    && BasketBallAuto.instance.BasketBallState.CanPullBall)
        //{
        //    CallBallToPlayer.instance.pullBallToPlayer();
        //}
        //else
        //{
        //    stateWalk = false;
        //}
        // call ball
        // call ball
        if (!hasBasketball
            && !InAir
            && BasketBallAuto.instance.BasketBallState.CanPullBall
            && !BasketBallAuto.instance.BasketBallState.Locked
            && !BasketBallAuto.instance.BasketBallState.InAir
            && !BasketBallAuto.instance.BasketBallState.Thrown
            && BasketBallAuto.instance.BasketBallState.Grounded
            && Grounded
            && !CallBallToPlayer.instance.Locked
            && (currentState == idleState || currentState == walkState))
        {
            CallBallToPlayer.instance.Locked = true;
            Debug.Log("CallBallToPlayer.instance.Locked : " + CallBallToPlayer.instance.Locked);
            //CallBallToPlayer.instance.pullBallToPlayer();
            //CallBallToPlayer.instance.Locked = false;
            Debug.Log("call ball if");
            StartCoroutine(CallBall());
            //CallBallToPlayer.instance.Locked = false;
        }
        //if (!hasBasketball
        //    && !InAir
        //    && BasketBallAuto.instance.BasketBallState.CanPullBall
        //    && !BasketBallAuto.instance.BasketBallState.Locked
        //    && Grounded
        //    && !CallBallToPlayer.instance.Locked
        //    && currentState != inAirHasBasketball
        //    && currentState != inAirHasBasketballFrontState
        //    && currentState != inAirHasBasketballSideState)
        //{
        //    //CallBallToPlayer.instance.Locked = true;
        //    //CallBallToPlayer.instance.pullBallToPlayer();
        //    //CallBallToPlayer.instance.Locked = false;
        //    StartCoroutine(CallBall());
        //}
        //if (Grounded
        //    && currentState != inAirHasBasketball
        //    && currentState != inAirHasBasketballFrontState
        //    && currentState != inAirHasBasketballSideState
        //    && distanceToTarget >= 0.05f)
        //{
        //    stateWalk = true;
        //}
    }


    // Update :: once once per frame
    void Update()
    {
        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        // ================== auto player facing goal ==========================
        relativePositionToGoal = GameLevelManager.instance.BasketballRimVector.x + transform.position.x;

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
        distanceToTarget = Vector3.Distance(transform.position, new Vector3(targetPosition.x, 0, targetPosition.z));

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
        if (stateWalk && distanceToTarget <= 0.05f && !arrivedAtTarget && Grounded)
        {
            Debug.Log(" arrived idle : distanceToTarget : "+ distanceToTarget);
            arrivedAtTarget = true;
            stateWalk = false;
            stateIdle = true;
            positionMarkerCounter++;
            rigidBody.velocity = Vector3.zero;
        }
        if (!stateWalk && distanceToTarget >= 0.05f && !arrivedAtTarget && Grounded)
        {
            Debug.Log(" arrived idle : walking to target : " + distanceToTarget);
            stateWalk = true;
            stateIdle = false;
            //positionMarkerCounter++;
            //rigidBody.velocity = Vector3.zero;
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
        if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha0))
        {
            stateWalk = !stateWalk;
        }
        if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha8) 
            && hasBasketball
            && !InAir)
        {
            //PlayerShoot();
            jumpTrigger = !jumpTrigger;
        }

        //testing
        //======================================

        //------------------ jump -----------------------------------
        if (hasBasketball
            && stateIdle
            && arrivedAtTarget
            && Grounded
            && !KnockedDown
            && !jumpTrigger
            && !shootTrigger
            && !InAir)
        {
            arrivedAtTarget = false;
            jumpTrigger = true;
        }

        //------------------ shoot -----------------------------------
        // if has ball, is in air, and pressed shoot button.
        // note -- At top of the jump
        //Debug.Log("curren state : " + currentState);
        //Debug.Log("curren state : " + currentStateInfo.fullPathHash);
        if (InAir
            && hasBasketball
            && !GameOptions.EnemiesOnlyEnabled
            && rigidBody.velocity.y <= 0
            && (currentState == inAirHasBasketballFrontState || currentState == inAirHasBasketballSideState)
            && !shootTrigger)
        {
            shootTrigger = true;
            CallBallToPlayer.instance.Locked = true;
            BasketBallState.instance.Locked = true;
            CheckIsPlayerFacingGoal(); // turns player facing rim
            Shotmeter.MeterEnded = true; // this determines ball launch. find top of the jump
            PlayerShoot();
        }
    }

    IEnumerator CallBall()
    {
        yield return new WaitForSeconds(0.75f);
        if (!BasketBallAuto.instance.BasketBallState.InAir)
        {
            CallBallToPlayer.instance.pullBallToPlayer();
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
        BasketBallAuto.instance.shootBasketBall();
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
    //public IEnumerator GoToShootingPosition()
    //{
    //    Debug.Log("goToShootingPosition()");
    //    //generateRandomShootingPosition();
    //    yield return new WaitUntil(()=> testPosition != null);
    //    //targetPosition = prefabMarkerToInstantiate.transform.position;
    //    //yield return new WaitUntil(() => prefabMarkerToInstantiate != null);
    //    //targetPosition = generateRandomShootingPosition().transform.position;
    //    targetPosition = testPosition.transform.position;
    //    stateWalk = true;
    //    Debug.Log("goToShootingPosition() 2");
    //    //targetPosition = (randomShootingPosition - transform.position).normalized;
    //    //Debug.Log("targetPosition : "+ targetPosition);
    //    //movement = targetPosition * (movementSpeed * Time.deltaTime);
    //    //Debug.Log("movement : " + movement);
    //    //rigidBody.MovePosition(transform.position + movement);
    //}

    //public void AutoPlayerArrivedAtMarker()
    //{
    //    Debug.Log("AutoPlayerArrivedAtMarker");
    //    stateWalk = false;
    //    stateIdle = true;
    //    jumpTrigger = true;
    //}

    //private GameObject generateRandomShootingPosition()
    //{

    //    Debug.Log("-----generateRandomShootingPosition()");
    //    randomShootingPosition = GetRandomFourPointPosition(relativePositionToGoal);

    //    prefabClone =
    //    Instantiate(testPosition, randomShootingPosition, Quaternion.Euler(new Vector3(-90, 0, 0)));

    //    return prefabClone;
    //    // set parent to object with vertical layout
    //    //prefabClone.transform.SetParent(basketballRim.transform, false);
    //    //Destroy(prefabClone.gameObject, 3);
    //    //return 0.0f;
    //    //targetCreated = true;
    //}

    //private Vector3 GetRandomThreePointPosition(float relativePos)
    //{
    //    //// get random side of basketball goal (left or right, viewed head on)
    //    //List<int> list = new List<int> { 1, -1 };
    //    //int finder = Random.Range(0, 2); //Then you just use this; nameDisplayString = names[finder];
    //    //int randomXDirection = list[finder];

    //    // get basketball goal posiiton
    //    Vector3 rimVectorOnGround = basketballRim.transform.position;
    //    // set to ground (Y vector)
    //    rimVectorOnGround.y = 0.0f;
    //    // get random radius between 3 and 4 point line
    //    float randomRadius = (UnityEngine.Random.Range(Constants.DISTANCE_3point + 0.5f, Constants.DISTANCE_4point - 0.8f));
    //    // random angle
    //    float randomAngle = 0;
    //    // get X, Z points for vector
    //    float x = 0;
    //    // generate position on same side of goal as shooter
    //    if (relativePos < 0)
    //    {
    //        // between pi - 3pi/2
    //        randomAngle = Random.Range(math.PI, 1.5f * math.PI);
    //        x = (math.cos(randomAngle) * randomRadius) + rimVectorOnGround.x;
    //    }
    //    if (relativePos > 0)
    //    {
    //        // between  3pi/2 - 2pi
    //        randomAngle = Random.Range(1.5f * math.PI, 2 * math.PI);
    //        x = math.cos(randomAngle) * randomRadius + rimVectorOnGround.x;
    //    }
    //    float z = math.sin(randomAngle) * randomRadius + rimVectorOnGround.z;

    //    Vector3 randomShootingPosition = new Vector3(x, 0, z);
    //    randomShootingPosition.y = 0.01f;

    //    return randomShootingPosition;
    //}

    //private Vector3 GetRandomFourPointPosition(float relativePos)
    //{
    //    //// get random side of basketball goal (left or right, viewed head on)
    //    //List<int> list = new List<int> { 1, -1 };
    //    //int finder = Random.Range(0, 2); //Then you just use this; nameDisplayString = names[finder];
    //    //int randomXDirection = list[finder];

    //    // get basketball goal posiiton
    //    Vector3 rimVectorOnGround = basketballRim.transform.position;
    //    // set to ground (Y vector)
    //    rimVectorOnGround.y = 0.0f;
    //    // get random radius between 3 and 4 point line
    //    float randomRadius = (Random.Range(Constants.DISTANCE_4point, Constants.DISTANCE_4point + 1f));
    //    //Debug.Log("randomRadius : " + randomRadius);
    //    // random angle
    //    float randomAngle = 0;
    //    // get X, Z points for vector
    //    float x = 0;
    //    // generate position on same side of goal as shooter
    //    if (relativePos < 0)
    //    {
    //        // between pi - 3pi/2
    //        randomAngle = Random.Range(math.PI, 1.5f * math.PI);
    //        x = (math.cos(randomAngle) * randomRadius) + rimVectorOnGround.x;
    //    }
    //    if (relativePos > 0)
    //    {
    //        // between  3pi/2 - 2pi
    //        randomAngle = Random.Range(1.5f * math.PI, 2 * math.PI);
    //        x = math.cos(randomAngle) * randomRadius + rimVectorOnGround.x;
    //    }
    //    float z = math.sin(randomAngle) * randomRadius + rimVectorOnGround.z;

    //    Vector3 randomShootingPosition = new Vector3(x, 0, z);
    //    randomShootingPosition.y = 0.01f;

    //    return randomShootingPosition;
    //}

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
        //Debug.Log("UnFreezePlayerPosition");
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
}
