
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;
    private Rigidbody rigidBody;
    [SerializeField]
    EnemyDetection enemyDetection;

    // how long after attacking the enemy can attack again
    public float attackCooldown;
    // target for enemy to move to
    private Vector3 targetPosition;

    Vector3 movement;
    private float movementSpeed;
    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;

    [SerializeField]
    public bool facingRight;
    [SerializeField]
    private float relativePositionToPlayer;
    [SerializeField]
    private float distanceFromPlayer;
    [SerializeField]
    private float minDistanceToAttack;

    private AnimatorStateInfo currentStateInfo;
    static int currentState;
    static int AnimatorState_Attack = Animator.StringToHash("base.attack");
    static int AnimatorState_Walk = Animator.StringToHash("base.walk");
    static int AnimatorState_Idle = Animator.StringToHash("base.idle");

    public bool stateWalk = false;
    public bool stateIdle = false;
    public bool stateAttack = false;

    public bool canAttack;

    public bool StateWalk { get => stateWalk; set => stateWalk = value; }
    public float RelativePositionToPlayer { get => relativePositionToPlayer; set => relativePositionToPlayer = value; }
    public float DistanceFromPlayer { get => distanceFromPlayer; set => distanceFromPlayer = value; }

    // Use this for initialization
    void Start()
    {
        facingRight = true;
        movementSpeed = walkMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        enemyDetection = gameObject.GetComponent<EnemyDetection>();

        canAttack = true;
        if (attackCooldown == 0)
        {
            attackCooldown = 1f;
        }
        InvokeRepeating("UpdateDistanceFromPlayer", 0, 0.1f);
    }

    private void FixedUpdate()
    {
        if (stateWalk)
        {
            pursuePlayer();
        }
    }

    void Update()
    {
        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        // ================== enemy facing player ==========================
        relativePositionToPlayer = GameLevelManager.instance.Player.transform.position.x - transform.position.x;

        // ================== enemy idle ==========================
        if ((GameLevelManager.instance.PlayerState.KnockedDown
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
        if (math.abs(relativePositionToPlayer) < minDistanceToAttack
            && canAttack)
        {
            stateAttack = true;
        }
        else
        {
            stateAttack = false;
        }
        // ================== enemy walk state ==========================
        if (enemyDetection.PlayerSighted
            && !stateAttack)
        {
            stateWalk = true;
        }
        else
        {
            stateWalk = false;
        }
        // ================== animation walk state ==========================
        //if (rigidBody.velocity.sqrMagnitude > 0)
        if (stateWalk)
        {
            anim.SetBool("walk", true);
        }
        else
        {
            anim.SetBool("walk", false);
        }
        if (stateAttack)
        {
            anim.SetTrigger("attack");
            FreezeEnemyPosition();
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


        //currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        //currentState = currentStateInfo.fullPathHash;

        //// ----- control speed based on commands----------

        //if (currentState == idleState || currentState == walkState
        //|| currentState == idleState2)
        //{
        //    movementSpeed = walkMovementSpeed;
        //}
        //else
        //{
        //    movementSpeed = runMovementSpeed;
        //}
        ////rigidBody.velocity = movement * movementSpeed;
        //navmeshAgent.speed = movementSpeed;
        //if (rigidBody != null)
        //{
        //    anim.SetFloat("speed", rigidBody.velocity.sqrMagnitude);
        //}

        ////check if walking
        ////  function will flip sprite if needed
        //isWalking(navmeshAgent.velocity.magnitude);
    }

    //void UpdateStateTruthTable()
    //{
    //    if (stateAttack)
    //    {
    //        stateIdle = false;
    //        stateWalk = false;
    //    }
    //    if (stateIdle)
    //    {
    //        stateAttack
    //    }
    //    if (stateWalk)
    //    {

    //    }
    //}

    public void FreezeEnemyPosition()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezePositionY
            | RigidbodyConstraints.FreezePositionZ
            | RigidbodyConstraints.FreezePositionX;
    }

    public void UnFreezeEnemyPosition()
    {
        rigidBody.constraints = RigidbodyConstraints.FreezeRotationX
            | RigidbodyConstraints.FreezeRotationZ
            | RigidbodyConstraints.FreezeRotationY
            | RigidbodyConstraints.FreezePositionY;
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

    void isWalking(float speed)
    {
        // if moving
        if (speed > 0)
        {
            anim.SetBool("run", true);
        }
        else
        {
            anim.SetBool("run", false);
        }
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



    int RandomNumber(int min, int max)
    {
        System.Random rnd = new System.Random();
        int randNum = rnd.Next(min, max);
        //Debug.Log("generate randNum : " + randNum);
        return randNum;
    }

    public void pursuePlayer()
    {
        targetPosition = (GameLevelManager.instance.Player.transform.position - transform.position).normalized;
        movement = targetPosition * (movementSpeed * Time.deltaTime);
        rigidBody.MovePosition(transform.position + movement);
    }

    public void UpdateDistanceFromPlayer()
    {
        distanceFromPlayer = Vector3.Distance(GameLevelManager.instance.Player.transform.position, transform.position);
    }
}
