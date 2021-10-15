using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rigidBody;
    private EnemyDetection enemyDetection;
    private SpriteRenderer spriteRenderer;
    private PlayerSwapAttack playerSwapAttack;
    // target for enemy to move to
    private Vector3 targetPosition;

    Vector3 movement;
    private float movementSpeed;
    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;

    [SerializeField]
    public bool facingRight;
    // how long after attacking the enemy can attack again
    public float attackCooldown;
    [SerializeField]
    private float relativePositionToPlayer;
    [SerializeField]
    private float distanceFromPlayer;
    [SerializeField]
    private float distanceFromBodyGuard;
    [SerializeField]
    private float minDistanceCloseAttack;
    [SerializeField]
    private float maxDistanceLongRangeAttack;
    [SerializeField]
    private float minDistanceLongRangeAttack;
    [SerializeField]
    private float knockBackForce;
    [SerializeField]
    bool hasLongRangeAttack;
    [SerializeField]
    private bool longRangeAttack;
    [SerializeField]
    private float knockDownTime;
    [SerializeField]
    private float takeDamageTime;
    [SerializeField]
    bool isMinion;
    [SerializeField]
    bool isBoss;

    const string lightningAnimName = "lightning";

    private AnimatorStateInfo currentStateInfo;
    static int currentState;
    static int AnimatorState_Attack = Animator.StringToHash("base.attack");
    static int AnimatorState_Walk = Animator.StringToHash("base.walk");
    static int AnimatorState_Idle = Animator.StringToHash("base.idle");
    static int AnimatorState_Knockdown = Animator.StringToHash("base.knockdown");
    static int AnimatorState_Lightning = Animator.StringToHash("base.lightning");
    static int AnimatorState_Disintegrated = Animator.StringToHash("base.disintegrated");

    public bool stateWalk = false;
    public bool stateIdle = false;
    public bool stateAttack = false;
    public bool statePatrol = false;
    public bool stateKnockDown = false;

    //bool playerInLineOfSight = false;
    [SerializeField]
    private float lineOfSight;
    public float lineOfSightVariance;

    public bool canAttack;
    bool inAttackQueue;

    [SerializeField]
    bool enemyUsesPhysics;
    [SerializeField]
    GameObject dropShadow;

    Vector3 originalPosition;
    //[SerializeField]
    //float currentSpeed;

    // Use this for initialization
    void Start()
    {
        facingRight = true;
        movementSpeed = walkMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyDetection = gameObject.GetComponent<EnemyDetection>();
        originalPosition = transform.position;
        canAttack = true;

        playerSwapAttack = GetComponent<PlayerSwapAttack>();

        if (attackCooldown == 0) { attackCooldown = 0.75f; }
        //if (knockDownTime == 0) { knockDownTime = 2f; }
        if (lineOfSightVariance == 0) { lineOfSightVariance = 0.4f; }
        //if (takeDamageTime == 0) { takeDamageTime = 0.3f; }
        if (minDistanceCloseAttack == 0) { minDistanceCloseAttack = 0.6f; }
        if(knockBackForce == 0) { knockBackForce = 3f; }

        if (isMinion)
        {
            attackCooldown = 1.5f;
            walkMovementSpeed = 1.5f;
            runMovementSpeed = 2f;
            takeDamageTime = 0.4f;
        }
        if (isBoss)
        {
            attackCooldown = 1f;
            walkMovementSpeed = 2f;
            runMovementSpeed = 2.5f;
            takeDamageTime = 0.3f;
        }

        if (GameOptions.hardcoreModeEnabled)
        {
            movementSpeed *= 1.25f;
            attackCooldown *= 0.5f;
        }

        // put enemy on the ground. some are spawning up pretty high
        //gameObject.transform.position = new Vector3(gameObject.transform.position.x, GameLevelManager.instance.TerrainHeight, gameObject.transform.position.z);

        InvokeRepeating("UpdateDistanceFromPlayer", 0, 0.1f);
    }

    private void FixedUpdate()
    {
        if (stateWalk && currentState != AnimatorState_Knockdown && currentState != AnimatorState_Disintegrated
            && enemyDetection.Attacking)
        {
            pursueTarget();
        }
        if (statePatrol)
        {
            returnToPatrol();
        }
        if (enemyUsesPhysics)
        {
            dropShadow.transform.position = new Vector3(dropShadow.transform.position.x,
                gameObject.transform.position.y + 0.01f, dropShadow.transform.position.z);
        }
    }

    void Update()
    {
        //currentSpeed = rigidBody.velocity.magnitude;

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;
        // ================== enemy facing player ==========================
        if (PlayerAttackQueue.instance.BodyGuards.Count == 0 && !PlayerAttackQueue.instance.BodyGuardEngaged)
        {
            relativePositionToPlayer = GameLevelManager.instance.Player.transform.position.x - transform.position.x;
        }
        else
        {
            relativePositionToPlayer = PlayerAttackQueue.instance.BodyGuards[0].transform.position.x - transform.position.x;
        }

        // ================== enemy idle ==========================
        if ((GameLevelManager.instance.PlayerController.KnockedDown
            || !canAttack
            || !enemyDetection.PlayerSighted)
            && currentState != AnimatorState_Attack)
        {
            stateIdle = true;
            //if idle stop rigidbody
            rigidBody.velocity = Vector3.zero;
        }
        else
        {
            stateIdle = false;
        }
        // ================== enemy attack state ==========================
        if (math.abs(relativePositionToPlayer) <= maxDistanceLongRangeAttack
            && math.abs(relativePositionToPlayer) >= minDistanceLongRangeAttack
            && hasLongRangeAttack
            && math.abs(lineOfSight) <= lineOfSightVariance
            && canAttack)
        {
            longRangeAttack = true;
            stateAttack = true;
        }

        else if (math.abs(relativePositionToPlayer) < minDistanceCloseAttack
            && math.abs(lineOfSight) <= lineOfSightVariance
            && !longRangeAttack
            && canAttack)
        {
            stateAttack = true;
            longRangeAttack = false;
        }
        else
        {
            stateAttack = false;
            longRangeAttack = false;
        }
        // ================== enemy walk state ==========================
        if (enemyDetection.PlayerSighted
            && !stateAttack
            && !stateIdle
            && canAttack
            && currentState != AnimatorState_Knockdown
            && currentState != AnimatorState_Disintegrated)
        {
            stateWalk = true;
        }
        else
        {
            stateWalk = false;
        }
        // ================== animation walk state ==========================
        //if (rigidBody.velocity.sqrMagnitude > 0)
        if (stateWalk || statePatrol)
        {
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }
        if (stateAttack)
        {
            FreezeEnemyPosition();
            if (playerSwapAttack != null
                && !longRangeAttack
                && playerSwapAttack.closeAttacks != null
                && playerSwapAttack.AnimatorOverrideController != null)
            {
                playerSwapAttack.setCloseAttack();
            }
            if (playerSwapAttack != null
                && playerSwapAttack.AnimatorOverrideController != null
                && longRangeAttack
                && playerSwapAttack.longRangeAttack != null)
            {
                playerSwapAttack.setLongRangeAttack();
            }

            anim.SetTrigger("attack");
            StartCoroutine(AttackCooldown(attackCooldown));
        }
        if (relativePositionToPlayer < 0 && facingRight)
        {
            Flip();
        }
        if (relativePositionToPlayer > 0 && !facingRight)
        {
            Flip();
        }
    }

    public void FreezeEnemyPosition()
    {
        if (enemyUsesPhysics)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezePositionZ;
                //| RigidbodyConstraints.FreezePositionX;
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY
                //| RigidbodyConstraints.FreezePositionY
                | RigidbodyConstraints.FreezePositionZ
                | RigidbodyConstraints.FreezePositionX;
        }
    }

    public void UnFreezeEnemyPosition()
    {
        if (enemyUsesPhysics)
        {
            rigidBody.velocity = Vector3.zero;
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY;
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY;
                //| RigidbodyConstraints.FreezePositionY;
        }
    }

    IEnumerator AttackCooldown(float seconds)
    {

        canAttack = false;
        // wait for animator state to get to attack 
        yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"));
        // wait for animation to finish
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"));
        stateAttack = false;
        // enemy can move again
        UnFreezeEnemyPosition();
        //wait for cooldown
        yield return new WaitForSecondsRealtime(seconds);
        canAttack = true;
    }

    void Flip()
    {
        //Debug.Log(" Flip()");
        facingRight = !facingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
    }

    public void setPlayerAnim(string animationName, bool isTrue)
    {
        anim.SetBool(animationName, isTrue);
    }
    public void playAnimation(string animationName)
    {
        anim.Play(animationName);
    }

    public IEnumerator struckByLighning()
    {
        stateKnockDown = true;
        FreezeEnemyPosition();
        GameObject.Find("camera_flash").GetComponent<Animator>().Play("camera_flash");
        anim.Play("lightning");
        yield return new WaitUntil(() => currentState == AnimatorState_Lightning);
        //anim.SetBool("knockdown", true);
        //yield return new WaitForSeconds(1);
        StartCoroutine(knockedDown());

        ////anim.SetBool("knockdown", true);
        //playAnimation("knockdown");
        //yield return new WaitForSeconds(knockDownTime);
        //anim.SetBool("knockdown", false);
        //stateKnockDown = false;
        //UnFreezeEnemyPosition();

        //stateKnockDown = false;
    }


    public IEnumerator knockedDown()
    {
        stateKnockDown = true;
        FreezeEnemyPosition();
        //UnFreezeEnemyPosition(); 
        anim.SetBool("knockdown", true);
        yield return new WaitUntil(() => currentState != AnimatorState_Lightning);
        playAnimation("knockdown");
        // get direction facing
        if (facingRight)
        {
            UnFreezeEnemyPosition();
            rigidBody.velocity = Vector3.zero;
            //apply to X
            RigidBody.AddForce(-knockBackForce, knockBackForce/2, 0, ForceMode.VelocityChange);
        }
        if (!facingRight)
        {
            UnFreezeEnemyPosition();
            rigidBody.velocity = Vector3.zero;
            RigidBody.AddForce(knockBackForce, knockBackForce / 2, 0, ForceMode.VelocityChange);
        }
        yield return new WaitForSeconds(knockDownTime);
        anim.SetBool("knockdown", false);
        stateKnockDown = false;
        //UnFreezeEnemyPosition();

        stateKnockDown = false;
    }

    public IEnumerator killEnemy()
    {
        stateKnockDown = true;
        FreezeEnemyPosition();
        playAnimation("disintegrated");
        yield return new WaitForSeconds(1.5f);

        if (enemyDetection.Attacking)
        {
            //Debug.Log("========================== enemy killed : " + gameObject.name + " :  remove from attack queue");
            int attackPositionId = enemyDetection.AttackPositionId;
            PlayerAttackQueue.instance.removeEnemyFromQueue(gameObject, attackPositionId);
        }
        //yield return new WaitUntil( ()=> PlayerAttackQueue.instance.AttackSlotOpen);
        Destroy(gameObject);
    }

    public IEnumerator takeDamage()
    {
        stateKnockDown = true;
        FreezeEnemyPosition();
        //GameObject.Find("camera_flash").GetComponent<Animator>().Play("camera_flash");
        anim.SetBool("takeDamage", true);
        playAnimation("takeDamage");
        //yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("lightning"));
        //yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("knockdown"));
        if (facingRight)
        {
            UnFreezeEnemyPosition();
            //apply to X
            RigidBody.AddForce(-knockBackForce/2, 0, 0, ForceMode.VelocityChange);
        }
        if (!facingRight)
        {
            UnFreezeEnemyPosition();
            RigidBody.AddForce(knockBackForce/2, 0, 0, ForceMode.VelocityChange);
        }
        yield return new WaitForSecondsRealtime(takeDamageTime);
        anim.SetBool("takeDamage", false);
        UnFreezeEnemyPosition();

        stateKnockDown = false;
        //anim.ResetTrigger("exitAnimation");
    }

    public IEnumerator disintegrated()
    {
        stateKnockDown = true;
        FreezeEnemyPosition();
        playAnimation("disintegrated");
        //yield return new WaitUntil(() => currentState == AnimatorState_Disintegrated);
        yield return new WaitForSeconds(1.5f);
        Destroy(gameObject);
        stateKnockDown = false;
    }

    //int RandomNumber(int min, int max)
    //{
    //    System.Random rnd = new System.Random();
    //    int randNum = rnd.Next(min, max);
    //    //Debug.Log("generate randNum : " + randNum);
    //    return randNum;
    //}

    public void pursueTarget()
    {
        //targetPosition = (GameLevelManager.instance.Player.transform.position - transform.position).normalized;

        // if no bodyguards found
        if (PlayerAttackQueue.instance.BodyGuards.Count == 0 && !PlayerAttackQueue.instance.BodyGuardEngaged)
        {
            targetPosition = (PlayerAttackQueue.instance.AttackPositions[enemyDetection.AttackPositionId].transform.position - transform.position).normalized;
        }
        // if bodyguards, attack 1 first bodyguard
        else
        {
            targetPosition = (PlayerAttackQueue.instance.BodyGuards[0].transform.position - transform.position).normalized;
        }
        movement = targetPosition * (movementSpeed * Time.deltaTime);
        //movement = targetPosition * (movementSpeed * Time.deltaTime);
        rigidBody.MovePosition(transform.position + movement);
        //transform.Translate(movement);

        //Debug.Log(gameObject.transform.root.name + " -- currentSpeed : " + currentSpeed);

    }

    public void moveToTarget(List<GameObject> waypoints)
    {
        //targetPosition = (GameLevelManager.instance.Player.transform.position - transform.position).normalized;

        //// if no bodyguards found
        //if (PlayerAttackQueue.instance.BodyGuards.Count == 0 && !PlayerAttackQueue.instance.BodyGuardEngaged)
        //{
        //    targetPosition = (PlayerAttackQueue.instance.AttackPositions[enemyDetection.AttackPositionId].transform.position - transform.position).normalized;
        //}
        //// if bodyguards, attack 1 first bodyguard
        //else
        //{
        //    targetPosition = (PlayerAttackQueue.instance.BodyGuards[0].transform.position - transform.position).normalized;
        //}
        movement = targetPosition * (movementSpeed * Time.deltaTime);
        //movement = targetPosition * (movementSpeed * Time.deltaTime);
        rigidBody.MovePosition(transform.position + movement);
        //transform.Translate(movement);

        //Debug.Log(gameObject.transform.root.name + " -- currentSpeed : " + currentSpeed);

    }
    public void returnToPatrol()
    {
        //Debug.Log(gameObject.name + "  is returning to Vector3  : " + originalPosition);
        if (Vector3.Distance(gameObject.transform.position, OriginalPosition) > 1)
        {
            targetPosition = (originalPosition - transform.position).normalized;
            movement = targetPosition * (movementSpeed * Time.deltaTime);
            //movement = targetPosition * (movementSpeed * Time.deltaTime);
            rigidBody.MovePosition(transform.position + movement);
        }
        else
        {
            statePatrol = false;
        }
    }

    public void UpdateDistanceFromPlayer()
    {
        if (PlayerAttackQueue.instance.BodyGuards.Count == 0 && !PlayerAttackQueue.instance.BodyGuardEngaged)
        {
            distanceFromPlayer = Vector3.Distance(GameLevelManager.instance.Player.transform.position, transform.position);
            lineOfSight = GameLevelManager.instance.Player.transform.position.z - transform.position.z;
        }
        else
        {
            distanceFromPlayer = Vector3.Distance(PlayerAttackQueue.instance.BodyGuards[0].transform.position, transform.position);
            lineOfSight = PlayerAttackQueue.instance.BodyGuards[0].transform.position.z - transform.position.z;
        }
    }

    public bool StateWalk { get => stateWalk; set => stateWalk = value; }
    public float RelativePositionToPlayer { get => relativePositionToPlayer; set => relativePositionToPlayer = value; }
    public float DistanceFromPlayer { get => distanceFromPlayer;  }
    public Vector3 OriginalPosition { get => originalPosition; set => originalPosition = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }
    public bool InAttackQueue { get => inAttackQueue; set => inAttackQueue = value; }
    public Vector3 TargetPosition { get => targetPosition; set => targetPosition = value; }
    public Rigidbody RigidBody { get => rigidBody; set => rigidBody = value; }
    public bool IsMinion { get => isMinion; set => isMinion = value; }
    public bool IsBoss { get => isBoss; set => isBoss = value; }
    public float DistanceFromBodyGuard { get => distanceFromBodyGuard; }
}
