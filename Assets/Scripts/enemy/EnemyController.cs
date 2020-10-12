
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    Animator anim;
    private Rigidbody rigidBody;
    [SerializeField]
    EnemyDetection enemyDetection;
    SpriteRenderer spriteRenderer;

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
    [SerializeField]
    private float knockDownTime;

    const string lightningAnimName = "lightning";

    private AnimatorStateInfo currentStateInfo;
    static int currentState;
    static int AnimatorState_Attack = Animator.StringToHash("base.attack");
    static int AnimatorState_Walk = Animator.StringToHash("base.walk");
    static int AnimatorState_Idle = Animator.StringToHash("base.idle");
    static int AnimatorState_Knockdown = Animator.StringToHash("base.knockdown");

    public bool stateWalk = false;
    public bool stateIdle = false;
    public bool stateAttack = false;
    public bool statePatrol = false;
    public bool stateKnockDown = false;

    public bool canAttack;

    Vector3 originalPosition;
    public bool StateWalk { get => stateWalk; set => stateWalk = value; }
    public float RelativePositionToPlayer { get => relativePositionToPlayer; set => relativePositionToPlayer = value; }
    public float DistanceFromPlayer { get => distanceFromPlayer; set => distanceFromPlayer = value; }
    public Vector3 OriginalPosition { get => originalPosition; set => originalPosition = value; }
    public SpriteRenderer SpriteRenderer { get => spriteRenderer; set => spriteRenderer = value; }

    // Use this for initialization
    void Start()
    {
        facingRight = true;
        movementSpeed = walkMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        enemyDetection = gameObject.GetComponent<EnemyDetection>();
        originalPosition = gameObject.transform.position;
        canAttack = true;
        if(attackCooldown == 0)
        {
            attackCooldown = 1f;
        }
        if(knockDownTime == 0)
        {
            knockDownTime = 2f;
        }
        InvokeRepeating("UpdateDistanceFromPlayer", 0, 0.1f);
    }

    private void FixedUpdate()
    {
        if (stateWalk) 
        {
            pursuePlayer();
        }
        if (statePatrol)
        {
            returnToPatrol();
        }
    }

    void Update()
    {
        // current used to determine movement speed based on animator state. walk, knockedown, moonwalk, idle, attacking, etc
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;
        // ================== enemy facing player ==========================
        relativePositionToPlayer = GameLevelManager.instance.Player.transform.position.x - transform.position.x;

        if (!anim.isActiveAndEnabled)
        {
            Debug.Log("aniamtor is inactive");
        }
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
            && !stateAttack
            && canAttack)
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
            FreezeEnemyPosition();
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
        rigidBody.velocity = Vector3.zero;
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
    //void isWalking(float speed)
    //{
    //    // if moving
    //    if (speed > 0)
    //    {
    //        anim.SetBool("run", true);
    //    }
    //    else
    //    {
    //        anim.SetBool("run", false);
    //    }
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
        playAnimation("lightning");
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("lightning"));
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("knockdown"));
        yield return new WaitForSecondsRealtime(knockDownTime);
        anim.SetTrigger("exitAnimation");
        UnFreezeEnemyPosition();
        stateKnockDown = false;
        //Debug.Log("knockdown time finished");
        //anim.ResetTrigger("exitAnimation");
    }

    public IEnumerator knockedDown()
    {
        stateKnockDown = true;
        FreezeEnemyPosition();
        //GameObject.Find("camera_flash").GetComponent<Animator>().Play("camera_flash");
        playAnimation("knockdown");
        //yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("lightning"));
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("knockdown"));
        yield return new WaitForSecondsRealtime(knockDownTime);
        anim.SetTrigger("exitAnimation");
        UnFreezeEnemyPosition();
        stateKnockDown = false;
        //Debug.Log("knockdown time finished");
        //anim.ResetTrigger("exitAnimation");
    }

    //int RandomNumber(int min, int max)
    //{
    //    System.Random rnd = new System.Random();
    //    int randNum = rnd.Next(min, max);
    //    //Debug.Log("generate randNum : " + randNum);
    //    return randNum;
    //}

    public void pursuePlayer()
    {
        targetPosition = (GameLevelManager.instance.Player.transform.position - transform.position).normalized;
        movement = targetPosition * (movementSpeed * Time.deltaTime);
        rigidBody.MovePosition(transform.position + movement);
    }
    public void returnToPatrol()
    {
        //Debug.Log(gameObject.name + "  is returning to Vector3  : " + originalPosition);
        if (Vector3.Distance(gameObject.transform.position, OriginalPosition) > 1)
        {

            targetPosition = (originalPosition - transform.position).normalized;
            movement = targetPosition * (movementSpeed * Time.deltaTime);
            rigidBody.MovePosition(transform.position + movement);
        }
        else
        {
            statePatrol = false;
        }
    }

    public void UpdateDistanceFromPlayer()
    {
        distanceFromPlayer = Vector3.Distance(GameLevelManager.instance.Player.transform.position, transform.position);
    }
}
