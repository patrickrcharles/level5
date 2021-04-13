using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyControllerAuto : MonoBehaviour
{
    private Animator anim;
    private Rigidbody rigidBody;
    private EnemyDetection enemyDetection;
    private SpriteRenderer spriteRenderer;
    private PlayerSwapAttack playerSwapAttack;
    // target for enemy to move to
    private Vector3 targetPosition;

    [SerializeField]
    GameObject prefabMarkerToInstantiate;
    [SerializeField]
    GameObject basketballRim;

    Vector3 movement;
    private float movementSpeed;
    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;

    // constant values that have to be hardcoded
    private const float threePointDistance = 3.8f;
    private const float fourPointDistance = 6.4f;
    private const float sevenPointDistance = 16.7f;

    [SerializeField]
    public bool facingRight;
    // how long after attacking the enemy can attack again
    public float attackCooldown;
    [SerializeField]
    private float relativePositionToGoal;
    //[SerializeField]
    //private float distanceFromPlayer;
    //[SerializeField]
    //private float minDistanceCloseAttack;
    //[SerializeField]
    //private float maxDistanceLongRangeAttack;
    //[SerializeField]
    //private float minDistanceLongRangeAttack;
    //[SerializeField]
    //bool hasLongRangeAttack;
    //[SerializeField]
    //private bool longRangeAttack;
    //[SerializeField]
    private float knockDownTime;
    [SerializeField]
    private float takeDamageTime;
    [SerializeField]
    bool isMinion;
    [SerializeField]
    bool isBoss;

    //const string lightningAnimName = "lightning";

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
    //public bool stateAttack = false;
    //public bool statePatrol = false;
    public bool stateKnockDown = false;

    //bool playerInLineOfSight = false;
    //[SerializeField]
    //private float lineOfSight;
    //public float lineOfSightVariance;

    //public bool canAttack;
    //bool inAttackQueue;

    [SerializeField]
    bool enemyUsesPhysics;
    [SerializeField]
    GameObject dropShadow;

    Vector3 originalPosition;

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
        //canAttack = true;

        //playerSwapAttack = GetComponent<PlayerSwapAttack>();

        //if (attackCooldown == 0) { attackCooldown = 1f; }
        ////if (knockDownTime == 0) { knockDownTime = 2f; }
        //if (lineOfSightVariance == 0) { lineOfSightVariance = 0.4f; }
        ////if (takeDamageTime == 0) { takeDamageTime = 0.3f; }
        //if (minDistanceCloseAttack == 0) { minDistanceCloseAttack = 0.6f; }

        if (isMinion)
        {
            attackCooldown = 1.5f;
            walkMovementSpeed = 1.25f;
            runMovementSpeed = 1.6f;
            takeDamageTime = 0.4f;
        }
        if (isBoss)
        {
            attackCooldown = 1.2f;
            walkMovementSpeed = 1.5f;
            runMovementSpeed = 2f;
            takeDamageTime = 0.3f;
        }

        //if (GameOptions.hardcoreModeEnabled)
        //{
        //    movementSpeed *= 1.25f;
        //    attackCooldown *= 0.5f;
        //}

        basketballRim = GameObject.Find("rim");

        // put enemy on the ground. some are spawning up pretty high
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, 0, gameObject.transform.position.z);

        InvokeRepeating("generateRandomShootingPosition", 0, 2f);
    }

    private void FixedUpdate()
    {
        if (stateWalk && currentState != AnimatorState_Knockdown && currentState != AnimatorState_Disintegrated
            && enemyDetection.Attacking)
        {
            goToShootingPosition();
        }
        //if (statePatrol)
        //{
        //    returnToPatrol();
        //}
        if (enemyUsesPhysics)
        {
            dropShadow.transform.position = new Vector3(dropShadow.transform.position.x, 0.01f, dropShadow.transform.position.z);
        }
    }

    void Update()
    {
        //currentSpeed = rigidBody.velocity.magnitude;

        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;
        // ================== enemy facing goal ==========================

        relativePositionToGoal = GameLevelManager.instance.BasketballRimVector.x + transform.position.x;

        //if (PlayerAttackQueue.instance.BodyGuards.Count == 0 && !PlayerAttackQueue.instance.BodyGuardEngaged)
        //{
        //    //relativePositionToPlayer = GameLevelManager.instance.Player.transform.position.x - transform.position.x;
        //    relativePositionToGoal = GameLevelManager.instance.BasketballRimVector.x - transform.position.x;
        //}
        //else
        //{
        //    relativePositionToPlayer = PlayerAttackQueue.instance.BodyGuards[0].transform.position.x - transform.position.x;
        //}

        // ================== enemy idle ==========================
        //if (currentState != AnimatorState_Attack)
        //{
        //    stateIdle = true;
        //    //if idle stop rigidbody
        //    rigidBody.velocity = Vector3.zero;
        //}
        //else
        //{
        //    stateIdle = false;
        //}
        // if ball is in air, enemy idle
        if (GameLevelManager.instance.Basketball.BasketBallState.InAir 
            && currentState != AnimatorState_Knockdown
            && currentState != AnimatorState_Walk
            && currentState != AnimatorState_Attack
            && currentState != AnimatorState_Lightning)
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
        //if (math.abs(relativePositionToGoal) <= maxDistanceLongRangeAttack
        //    && math.abs(relativePositionToGoal) >= minDistanceLongRangeAttack
        //    && hasLongRangeAttack
        //    //&& math.abs(lineOfSight) <= lineOfSightVariance
        //    && canAttack)
        //{
        //    longRangeAttack = true;
        //    stateAttack = true;
        //}

        //else if (math.abs(relativePositionToGoal) < minDistanceCloseAttack
        //    && math.abs(lineOfSight) <= lineOfSightVariance
        //    && !longRangeAttack
        //    && canAttack)
        //{
        //    stateAttack = true;
        //    longRangeAttack = false;
        //}
        //else
        //{
        //    stateAttack = false;
        //    longRangeAttack = false;
        //}
        // ================== enemy walk state ==========================
        if (currentState != AnimatorState_Knockdown
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
        if (stateWalk)//|| statePatrol)
        {
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }
        //if (stateAttack)
        //{
        //    FreezeEnemyPosition();
        //    if (playerSwapAttack != null
        //        && !longRangeAttack
        //        && playerSwapAttack.closeAttacks != null
        //        && playerSwapAttack.AnimatorOverrideController != null)
        //    {
        //        playerSwapAttack.setCloseAttack();
        //    }
        //    if (playerSwapAttack != null
        //        && playerSwapAttack.AnimatorOverrideController != null
        //        && longRangeAttack
        //        && playerSwapAttack.longRangeAttack != null)
        //    {
        //        playerSwapAttack.setLongRangeAttack();
        //    }

        //    anim.SetTrigger("attack");
        //    StartCoroutine(AttackCooldown(attackCooldown));
        //}
        if (relativePositionToGoal < 0 && !facingRight)
        {
            Flip();
        }
        if (relativePositionToGoal > 0 && facingRight)
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
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezePositionY
                | RigidbodyConstraints.FreezePositionZ
                | RigidbodyConstraints.FreezePositionX;
        }
    }

    public void UnFreezeEnemyPosition()
    {
        if (enemyUsesPhysics)
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY;
        }
        else
        {
            rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
                | RigidbodyConstraints.FreezeRotationZ
                | RigidbodyConstraints.FreezeRotationY
                | RigidbodyConstraints.FreezePositionY;
        }
    }

    //IEnumerator AttackCooldown(float seconds)
    //{

    //    canAttack = false;
    //    // wait for animator state to get to attack 
    //    yield return new WaitUntil(() => anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"));
    //    // wait for animation to finish
    //    yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("attack"));
    //    stateAttack = false;
    //    // enemy can move again
    //    UnFreezeEnemyPosition();
    //    //wait for cooldown
    //    yield return new WaitForSecondsRealtime(seconds);
    //    canAttack = true;
    //}

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
        anim.SetBool("knockdown", true);
        yield return new WaitUntil(() => currentState != AnimatorState_Lightning);
        playAnimation("knockdown");
        yield return new WaitForSeconds(knockDownTime);
        anim.SetBool("knockdown", false);
        stateKnockDown = false;
        UnFreezeEnemyPosition();

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

    public void goToShootingPosition()
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

    private void generateRandomShootingPosition()
    {

        //Vector3 randomShootingPosition = GetRandomThreePointPosition(relativePositionToGoal);
        Vector3 randomShootingPosition = GetRandomFourPointPosition(relativePositionToGoal);

        GameObject prefabClone =
        Instantiate(prefabMarkerToInstantiate, randomShootingPosition, Quaternion.Euler(new Vector3(-90, 0, 0)));
        // set parent to object with vertical layout
        //prefabClone.transform.SetParent(basketballRim.transform, false);
        Destroy(prefabClone.gameObject, 3);
        //return 0.0f;
    }

    private Vector3 GetRandomThreePointPosition(float relativePos)
    {
        //// get random side of basketball goal (left or right, viewed head on)
        //List<int> list = new List<int> { 1, -1 };
        //int finder = Random.Range(0, 2); //Then you just use this; nameDisplayString = names[finder];
        //int randomXDirection = list[finder];

        // get basketball goal posiiton
        Vector3 rimVectorOnGround = basketballRim.transform.position;
        // set to ground (Y vector)
        rimVectorOnGround.y = 0.0f;
        // get random radius between 3 and 4 point line
        float randomRadius = (Random.Range(threePointDistance + 0.5f, fourPointDistance - 0.8f));
        //Debug.Log("randomRadius : " + randomRadius);
        // random angle
        float randomAngle = 0;
        // get X, Z points for vector
        float x = 0;
        // generate position on same side of goal as shooter
        if (relativePos < 0)
        {
            // between pi - 3pi/2
            randomAngle = Random.Range(math.PI, 1.5f * math.PI);
            x = (math.cos(randomAngle) * randomRadius) + rimVectorOnGround.x;
        }
        if (relativePos > 0)
        {
            // between  3pi/2 - 2pi
            randomAngle = Random.Range( 1.5f * math.PI, 2 * math.PI);
            x = math.cos(randomAngle) * randomRadius + rimVectorOnGround.x;
        }
        float z = math.sin(randomAngle) * randomRadius + rimVectorOnGround.z;

        Vector3 randomShootingPosition = new Vector3(x, 0, z);
        randomShootingPosition.y = 0.01f;

        return randomShootingPosition;
    }

    private Vector3 GetRandomFourPointPosition(float relativePos)
    {
        //// get random side of basketball goal (left or right, viewed head on)
        //List<int> list = new List<int> { 1, -1 };
        //int finder = Random.Range(0, 2); //Then you just use this; nameDisplayString = names[finder];
        //int randomXDirection = list[finder];

        // get basketball goal posiiton
        Vector3 rimVectorOnGround = basketballRim.transform.position;
        // set to ground (Y vector)
        rimVectorOnGround.y = 0.0f;
        // get random radius between 3 and 4 point line
        float randomRadius = (Random.Range(fourPointDistance, fourPointDistance + 1f));
        //Debug.Log("randomRadius : " + randomRadius);
        // random angle
        float randomAngle = 0;
        // get X, Z points for vector
        float x = 0;
        // generate position on same side of goal as shooter
        if (relativePos < 0)
        {
            // between pi - 3pi/2
            randomAngle = Random.Range(math.PI, 1.5f * math.PI);
            x = (math.cos(randomAngle) * randomRadius) + rimVectorOnGround.x;
        }
        if (relativePos > 0)
        {
            // between  3pi/2 - 2pi
            randomAngle = Random.Range(1.5f * math.PI, 2 * math.PI);
            x = math.cos(randomAngle) * randomRadius + rimVectorOnGround.x;
        }
        float z = math.sin(randomAngle) * randomRadius + rimVectorOnGround.z;

        Vector3 randomShootingPosition = new Vector3(x, 0, z);
        randomShootingPosition.y = 0.01f;

        return randomShootingPosition;
    }
    //public void returnToPatrol()
    //{
    //    //Debug.Log(gameObject.name + "  is returning to Vector3  : " + originalPosition);
    //    if (Vector3.Distance(gameObject.transform.position, originalPosition) > 1)
    //    {
    //        targetPosition = (originalPosition - transform.position).normalized;
    //        movement = targetPosition * (movementSpeed * Time.deltaTime);
    //        //movement = targetPosition * (movementSpeed * Time.deltaTime);
    //        rigidBody.MovePosition(transform.position + movement);
    //    }
    //    else
    //    {
    //        statePatrol = false;
    //    }
    //}

    //public void UpdateDistanceFromPlayer()
    //{
    //    if (PlayerAttackQueue.instance.BodyGuards.Count == 0 && !PlayerAttackQueue.instance.BodyGuardEngaged)
    //    {
    //        distanceFromPlayer = Vector3.Distance(GameLevelManager.instance.Player.transform.position, transform.position);
    //        lineOfSight = GameLevelManager.instance.Player.transform.position.z - transform.position.z;
    //    }
    //    else
    //    {
    //        distanceFromPlayer = Vector3.Distance(PlayerAttackQueue.instance.BodyGuards[0].transform.position, transform.position);
    //        lineOfSight = PlayerAttackQueue.instance.BodyGuards[0].transform.position.z - transform.position.z;
    //    }
    //}
}
