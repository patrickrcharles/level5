using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TeamUtility.IO;
using System;
using UnityEditorInternal;
using UnityEngine.AI;

public class playercontrollerscriptAutoPlay : MonoBehaviour
{
    // components 
    [SerializeField]
    Animator anim;
    AnimatorStateInfo currentStateInfo;
    //GameObject dropShadow;
    AudioSource moonwalkAudio;
    //continueGame continueGame;
    //GUIStyle guiStyle = new GUIStyle();
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
    //public float attackCooldown;
    //public float chargeSpeed;

    /* ::: level boundary clamping
     the only current use for this smokebomb, because of the random direction
     you reappear in, can cause player to teleport out of level area
    TODO: reevaluate how this works, ex. when random direction selected, 
    check if the transform is null (nothing is occupying the space)
    */

    //[SerializeField]
    //float xMin, xMax, zMin, zMax, yMin, yMax;

    // player state bools
    bool moonwalking, blocking;
    public bool hasBasketball;
    //public bool smokingEnabled = true;
    public bool soundPlayed;
    public bool canMove; // save this when i cars that can knock player down

    [SerializeField]
    Vector3 bballRimVector;
    [SerializeField]
    float bballRelativePositioning;
    [SerializeField]
    Vector3 playerRelativePositioning;
    [SerializeField]
    public float playerDistanceFromRim;
    //public GameObject playerHitbox;

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
    [SerializeField]
    private bool _facingRight;
    [SerializeField]
    private bool _locked;
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

    private float _rigidBodyYVelocity;
    [SerializeField]
    public bool facingFront;
    shooterProfile shooterProfile;
    [SerializeField]
    GameObject basketball;
    [SerializeField]
    private Vector3 basketballPosition;

    private NavMeshAgent navmeshAgent;
    [SerializeField] private bool moveToNewPosition;
    private Vector3 newVector;

    private float timer =0.0f;
    private int seconds;

    void Start()
    {
        //Debug.Log("playercontrollerscript : start");
        navmeshAgent = GetComponent<NavMeshAgent>();
        rigidBody = GetComponent<Rigidbody>();
        moonwalkAudio = GetComponent<AudioSource>();
        anim = gameManagerAutoPlay.instance.anim;
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        basketball = GameObject.FindWithTag("basketball");
        basketballPosition = gameManagerAutoPlay.instance.Basketball.transform.position;
        shooterProfile = gameManagerAutoPlay.instance.player.GetComponent<shooterProfile>();
        // bball rim vector, used for relative positioning
        bballRimVector = GameObject.Find("rim").transform.position;

        setShooterProfileStats();

        //dropShadow = transform.root.transform.Find("drop_shadow").gameObject;
        //playerHitbox.SetActive(true);
        facingRight = true;
        canMove = true;
        movementSpeed = walkMovementSpeed;

    }

    // not affected by framerate
    void FixedUpdate()
    {
        //rigidBody.useGravity = false;
        //if (useGravity)
        //{
        //   //Debug.Log("gravityModifier : " + gravityModifier);
        //    //rigidBody.AddForce(0f, gravityModifier, 0f);
        //    rigidBody.AddForce(Vector3.up * gravityModifier * Time.deltaTime);
        //}

        //------MOVEMENT---------------------------
        float moveHorizontal = InputManager.GetAxis("Horizontal");
        float moveVertical = InputManager.GetAxis("Vertical");
        Vector3 movement;

        movement = new Vector3(moveHorizontal, 0, moveVertical) * movementSpeed * Time.deltaTime;
        rigidBody.MovePosition(transform.position + movement);

        //check if walking
        //  function will flip sprite if needed
        isWalking(navmeshAgent.velocity.magnitude);
        //isWalking(moveHorizontal, moveVertical);

    }

    // Update :: once once per frame
    void Update()
    {
        // keep drop shadow on ground at all times
        // dont like having hard code values. add variables
        //dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, 0.02f, dropShadow.transform.position.z);

        timer += Time.deltaTime;
        seconds = (int)(timer % 60);

        Debug.Log("seconds : " + seconds);

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        bballRelativePositioning = bballRimVector.x - rigidBody.position.x;
        playerRelativePositioning =  rigidBody.position - bballRimVector;
        playerDistanceFromRim = Vector3.Distance(transform.position, bballRimVector);

        basketballPosition = basketball.transform.position;
        //Debug.Log("rigidBodyYVelocity : " +  rigidBodyYVelocity);
        if (rigidBody.velocity.y <= 0 && inAir) {
            jumpPeakReached = true;
            rigidBodyYVelocity = rigidBody.velocity.y;
        }
        else{
            jumpPeakReached = false;
        }

        if (!hasBasketball
            && !pathComplete()
            && grounded
            && !locked)
        {
            locked = true;
            navmeshAgent.enabled = true;
            goToBall();
        }
        // ================= State machine ================================

        // have basketball. can shoot or move
        if (hasBasketball 
            && grounded
            && !locked)
        {
            //navmeshAgent.enabled = true;
            //go to a new destination 
            //locked = true;
            //move to new position
            //moveToNewPosition = true;
            if (navmeshAgent.enabled)
            {
                locked = true;

                newVector = getRandomTransformFromPlayerPosition();
                //Vector3 oldVector = transform.position;
                //navmeshAgent.SetDestination(newVector);

                navmeshAgent.SetDestination(newVector);
                navmeshAgent.updateRotation = false;
                Debug.Log("pathComplete() : " + pathComplete());
                //Debug.Log("hasball-notmoving ::  navmeshAgent Mesh destination : " + navmeshAgent.destination);
            }
        }

        if (navmeshAgent.enabled 
            && pathComplete() 
            && !locked
            && boundaryCheck(transform.position.x, transform.position.y))
        {

            playerJump();
        }



        // ================= State machine ================================

        // determine if player animation is shooting from or facing basket
        if (Math.Abs(playerRelativePositioning.x) > 2 &&
            Math.Abs(playerRelativePositioning.z) < 2)
        {
            facingFront = false;
        }
        else{
            facingFront = true ;
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
            || inAir
            || currentState == bIdle)
        {
            movementSpeed = walkMovementSpeed;
        }

        else if (currentState == mWalk)
        {
            movementSpeed = moonwalkMovementSpeed;
        }
    }

    private void playerJump()
    {
        navmeshAgent.enabled = false;
        rigidBody.velocity = (Vector3.up * jumpForce) + (Vector3.forward * rigidBody.velocity.x);
        if (bballRelativePositioning > 0 && !facingRight)
        {
            Flip();
        }
        if (bballRelativePositioning < 0f && facingRight)
        {
            Flip();
        }
        //rigidBody.isKinematic = false;
        locked = false;
    }

    private void goToBall()
    {
        // ball shot
        // new destination to ball

        //movingToTarget = true;

        //Vector3 newVector = getRandomTransformFromPlayerPosition();
        //Vector3 oldVector = transform.position;
        //Vector3 relativePosition = newVector - oldVector;

        // new = bball transform
        Vector3 newVector = basketballPosition;
        Vector3 oldVector = transform.position;
        //Vector3 relativePosition = newVector - oldVector;

        navmeshAgent.SetDestination(newVector);

        //Debug.Log("gotoball ::: navmeshAgent Mesh destination : " + navmeshAgent.destination);
        //disable rotation
        navmeshAgent.updateRotation = false;
        locked = false;
    }


    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("========================== collion enter: " + transform.gameObject.tag + " and " + other.gameObject.tag);
        if (gameObject.CompareTag("Player") && other.CompareTag("basketball") && !hasBasketball)
        {
            hasBasketball = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("========================== collision exit: " + transform.gameObject.tag + " and " + other.gameObject.tag);
        if (gameObject.CompareTag("Player") && other.CompareTag("basketball") && hasBasketball)
        {
            hasBasketball = false;
        }
    }

    //-----------------------------------Walk function -----------------------------------------------------------------------
     void isWalking(float speed)
    {
        if (speed > 0)
        {
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
        }

        if (navmeshAgent.velocity.x > 0 && !facingRight)
        {
            Flip();
        }

        if (navmeshAgent.velocity.x < 0 && facingRight)
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


    Vector3 getRandomTransformFromPlayerPosition()
    {
        int randomX = RandomNumber(1, 2);
        int randomZ = RandomNumber(1, 2);
        bool isInBounds = boundaryCheck(randomX, randomZ);

        Vector3 newTransform = new Vector3(transform.position.x + randomX * getRandomPositiveOrNegtaive(),
            transform.position.y,
            transform.position.z + randomZ * getRandomPositiveOrNegtaive());
        //Debug.Log("generate new transform : " + newTransform);

        if (isInBounds)
        {
            //anim.Play("smokeBomb");
            //yield return new WaitForSecondsRealtime(playerAnimations.Instance.smokeBomb.length);
            //transform.position = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
            //anim.Play("smokeBombSpawn");
            ////playerHitbox.SetActive(true);
            return newTransform;
        }
        else
        {
            getRandomTransformFromPlayerPosition();
        }

        return newTransform;
    }

    // -------------- boundary check for Throw smokebomb ---------------------------------------------------------
    bool boundaryCheck(int x, int z)
    {
        //Debug.Log("boundary check");
        if ((transform.position.x + x) < 8.5f
            && (transform.position.x + x) > -5
            && (transform.position.z + z) < -4
            && (transform.position.z + z) > -9)
        {
            //Debug.Log("pos + x : " + (transform.position.x+x) + "  pos + z : " + (transform.position.z + z));
            return true;
        }
        else
            return false;
    }

    bool boundaryCheck(float x, float z)
    {
        //Debug.Log("boundary check");
        if ((transform.position.x + x) < 8.5f
            && (transform.position.x + x) > -5
            && (transform.position.z + z) < -4
            && (transform.position.z + z) > -9)
        {
            //Debug.Log("pos + x : " + (transform.position.x+x) + "  pos + z : " + (transform.position.z + z));
            return true;
        }
        else
            return false;
    }

    int RandomNumber(int min, int max)
    {
        System.Random rnd = new System.Random();
        int randNum = rnd.Next(-4, 4);
        //Debug.Log("generate randNum : " + randNum);
        return randNum;
    }
    private int getRandomPositiveOrNegtaive()
    {
        System.Random random = new System.Random();
        List<int> list = new List<int> { 1, -1 };
        int finder = random.Next(list.Count); //Then you just use this; nameDisplayString = names[finder];
        int shotDirectionModifier = list[finder];

        return shotDirectionModifier;
    }

    bool pathComplete()
    {
        //Debug.Log("path complete");
        //Debug.Log("     basketballPosition : " + basketballPosition);
        //Debug.Log("     navmeshAgent.transform.position : " + navmeshAgent.transform.position);
        //Debug.Log("     navmeshAgent.stoppingDistance : " + navmeshAgent.stoppingDistance);
        //Debug.Log("     Vector3.Distance(navmeshAgent.destination, navmeshAgent.transform.position) : " + Vector3.Distance(basketballPosition, navmeshAgent.transform.position));
        //Debug.Log("     navmeshAgent.destination : " + navmeshAgent.destination);

        if (Vector3.Distance(basketballPosition, navmeshAgent.transform.position) <= navmeshAgent.stoppingDistance
        && !inAir)
        {
            //Debug.Log("!navmeshAgent.hasPath : " + !navmeshAgent.hasPath);
            //Debug.Log("navmeshAgent.velocity.sqrMagnitude : " + navmeshAgent.velocity.sqrMagnitude);

            if (!navmeshAgent.hasPath || navmeshAgent.velocity.sqrMagnitude == 0f)
            {
                locked = false;
                Debug.Log("pathcomplete()");
                return true;
            }
        }
        return false;
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
        moonwalkAudio.enabled = false;
    }

    public void setShooterProfileStats()
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
        get { return _rigidBodyYVelocity; }
        set { _rigidBodyYVelocity = value; }
    }
}
