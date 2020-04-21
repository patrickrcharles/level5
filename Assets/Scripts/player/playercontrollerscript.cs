using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;
using System;

public class playercontrollerscript : MonoBehaviour
{
    // components 
    Animator anim;
    AnimatorStateInfo currentStateInfo;
    GameObject dropShadow;
    AudioSource moonwalkAudio;
    continueGame continueGame;
    GUIStyle guiStyle = new GUIStyle();
    [SerializeField]
    SpriteRenderer spriteRenderer;
    private Rigidbody rigidBody;
    [Range(20f, 90f)]
    public float _angle; // for bball mini game trajectory

    // walk speed
    [SerializeField]
    private float movementSpeed;

    // jump vars 
    [SerializeField]
    float jumpForce;
    float groundcheckRadius = 0.2f;
    //public float fallMuliplier = 2.5f;
    //public float lowJumpMuliplier = 2f;
    public float moonwalkMovementSpeed;
    public float basketballRunSpeed;
    public float walkMovementSpeed;
    public float attackMovementSpeed;
    public float attackCooldown;
    public float chargeSpeed;

    /* ::: level boundary clamping
     the only current use for this smokebomb, because of the random direction
     you reappear in, can cause player to teleport out of level area
    TODO: reevaluate how this works, ex. when random direction selected, 
    check if the transform is null (nothing is occupying the space)
    */

    [SerializeField]
    float xMin, xMax, zMin, zMax, yMin, yMax;

    // player state bools
    bool moonwalking, blocking;
    public bool hasBasketball;
    public bool smokingEnabled = true;
    public bool soundPlayed;
    public bool canMove;

    [SerializeField]
    Vector3 bballRimVector;
    [SerializeField]
    float relativePositioning;

    //hitbox (take damage)
    public GameObject playerHitbox;

    // spawn psotions for projectiles
    public Transform projectileSpawnPoint, rocketSpawnPoint, throwSpawnPoint;

    // projectiles
    public GameObject projectile, rocket, molotov;

    // control movement speed based on state
    static int currentState;
    static int idleState = Animator.StringToHash("base.idle");
    static int walkState = Animator.StringToHash("base.movement.walk");
    static int mWalk = Animator.StringToHash("base.movement.moonwalk");
    static int bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
    static int bIdle = Animator.StringToHash("base.movement.basketball_idle");
    static int knockedDownState = Animator.StringToHash("base.takeDamage.knockedDown");

    //controller axis
    [SerializeField]
    bool triggerLeftAxisInUse, triggerRightAxisInUse;

    // get/set for following at bottom of class
    private bool _facingRight;
    [SerializeField]
    private bool _notLocked;
    private bool _jump;
    [SerializeField]
    private bool _inAir;
    [SerializeField]
    private bool _grounded;

    public float initialHeight, finalHeight;
    // custom gravity for player from shooterprofile
    public float gravityModifier;
    public bool jumpPeakReached = false;
    [SerializeField]
    bool useGravity = true;

    shooterProfile shooterProfile;

    private float _rigidBodyYVelocity;

    void Start()
    {
        // sets player limits from level manager. eventually remove by setting up
        // collider boundaries
        //setPlayerBounds(); // can remove once physical colliders finished
        notLocked = true; // default needs to be true

        shooterProfile = GameObject.Find("basketball").GetComponent<shooterProfile>();
        setShooterProfileStats();

        dropShadow = transform.root.transform.Find("drop_shadow").gameObject;

        // bball rim vector, used for relative positioning
        bballRimVector = GameObject.Find("rim").transform.position;

        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        playerHitbox.SetActive(true);
        facingRight = true;
        canMove = true;
        movementSpeed = walkMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        moonwalkAudio = GetComponent<AudioSource>();
        anim = GetComponentInChildren<Animator>();
        //continueGame = gameManager.instance.GetComponentInChildren<continueGame>();

    }

    // not affected by framerate
    void FixedUpdate()
    {
        //rigidBody.useGravity = false;
        //if (useGravity)
        //{
        //    Debug.Log("gravityModifier : " + gravityModifier);
        //    //rigidBody.AddForce(0f, gravityModifier, 0f);
        //    rigidBody.AddForce(Vector3.up * gravityModifier * Time.deltaTime);
        //}

        //------MOVEMENT---------------------------
        float moveHorizontal = InputManager.GetAxis("Horizontal");
        float moveVertical = InputManager.GetAxis("Vertical");
        Vector3 movement;

        movement = new Vector3(moveHorizontal, 0, moveVertical) * movementSpeed * Time.deltaTime;
        //Debug.Log("transform.position + movement :: " + transform.position + movement);

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

    // Update :: once once per frame
    void Update()
    {
        // keep drop shadow on ground at all times
        // dont like having hard code values. add variables
        dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, 0.02f, dropShadow.transform.position.z);

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        //Debug.Log("-------------- animation state :: " + currentState);
        //Debug.Log("-------------- bwalk :: " + bWalk);

        // ----- control speed based on commands----------
        if (currentState == idleState
            || currentState == walkState
            || inAir
            || currentState == bIdle)
        {
            movementSpeed = walkMovementSpeed;
        }

        else if (currentState == mWalk)
        {
            movementSpeed = moonwalkMovementSpeed;
            //Debug.Log("speed ::: moonwalkMovementSpeed");
        }

        //------------------ jump -----------------------------------
        if ((InputManager.GetButtonDown("Jump") && grounded)
            && !(InputManager.GetButtonDown("Fire1")))
        {
            Debug.Log("player jumped");
            rigidBody.velocity = (Vector3.up * jumpForce) + (Vector3.forward * rigidBody.velocity.x);

            //rigidBody.AddForce((Vector3.up * jumpForce) + (Vector3.forward * rigidBody.velocity.x), ForceMode.VelocityChange);
            //rigidBody.AddForce(new Vector3(0f, 10f, 0f), ForceMode.VelocityChange);
        }


        if (inAir)
        {
            anim.SetBool("walking", false);
        }

        //relativePositioning = bballRimVector.x - rigidBody.position.x;

        // if player is falling
        if (rigidBody.velocity.y > 0)
        {
            //updates "highest point" as long at player still moving upwards ( velcoity > 0)
            finalHeight = transform.position.y;
            //Debug.Log("intialHeight : " + initialHeight);  
            //Debug.Log("finalHeight : " + finalHeight);
        }


    }
    // =================================== END OF UPDATE() ==============================================================




    //-----------------------------------Walk function -----------------------------------------------------------------------
    void isWalking(float moveHorz, float moveVert)
    {

        // if moving/ not holding item. bool holdingItem set in AtttackCollision.cs
        if (moveHorz > 0f || moveHorz < 0f || moveVert > 0f || moveVert < 0f)
        {
            if (!inAir) // dont want walking animation playing while inAir
            {
                anim.SetBool("walking", true);
                //walking = true;
                //Debug.Log("anim.SetBool(walking, true) = " + anim.GetBool("walking"));
            }
        }
        else
        {
            anim.SetBool("walking", false);
            //walking = false;
            //Debug.Log("anim.SetBool(walking, true) = " + anim.GetBool("walking"));
        }

        if (moveHorz > 0 && !facingRight && canMove)
        {
            //Debug.Log("if (moveHorz > 0 && !facingRight && canMove)");     
            Flip();
        }
        if (moveHorz < 0f && facingRight && canMove)
        {
            //Debug.Log("if (moveHorz < 0f && facingRight && canMove)");
            Flip();
        }
        if ((InputManager.GetButton("Run") && canMove && !inAir))
        {
            //Debug.Log("if ((InputManager.GetButton(Run)");
            if (!hasBasketball)
            {
                anim.SetBool("moonwalking", true);
                if (anim.GetBool("moonwalking"))
                {
                    moonwalking = true;
                    moonwalkAudio.enabled = true;
                }
            }
            if (hasBasketball)
            {
                movementSpeed = basketballRunSpeed;
            }
            else
            {
                movementSpeed = moonwalkMovementSpeed;
            }
        }
        else
        {
            anim.SetBool("moonwalking", false);
            moonwalking = false;
            moonwalkAudio.enabled = false;
        }
    }

    void Flip()
    {
        //Debug.Log("void Flip()");

        facingRight = !facingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
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

    // -----------------generic wait coroutine ----------------------------
    IEnumerator Wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        soundPlayed = true;
        notLocked = true;
    }
    
    void setPlayerBounds()
    {
        xMin = levelManager.instance.xMinPlayer;
        xMax = levelManager.instance.xMaxPlayer;
        yMin = levelManager.instance.yMinPlayer;
        yMax = levelManager.instance.yMaxPlayer;
        zMin = levelManager.instance.zMinPlayer;
        zMax = levelManager.instance.zMaxPlayer;
    }

    //can be used as generic turn off audio by adding paramter to pass (Audio audioToTurnOff)
    public void turnOffMoonWalkAudio()
    {
        moonwalkAudio.enabled = false;
    }

    void setShooterProfileStats()
    {
        walkMovementSpeed = shooterProfile.speed;
        basketballRunSpeed = shooterProfile.runSpeed;
        jumpForce = shooterProfile.jumpForce;
        gravityModifier = shooterProfile.hangTime;
        _angle = shooterProfile.shootAngle;

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
    public bool notLocked
    {
        get { return _notLocked; }
        set { _notLocked = value; }
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
}
