
using System.Collections;
using Unity.Mathematics;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;
    public float punchCooldown;
    public float chargeSpeed;

    [SerializeField]
    public bool facingRight;

    float distanceFromStartPos;
    bool locked;
    //GameObject player;

    private float movementSpeed;
    private Rigidbody rigidBody;
    public SpriteRenderer currentSprite;

    Animator anim;

    public float maxDistance;

    private GameObject[] returnPositions;

    public bool stateWalk = false;
    public bool stateIdle = false;
    public bool stateAttack = false;
    public bool canAttack;

    public float attackCooldown;

    private Vector3 targetPosition;

    Vector3 movement;
    [SerializeField]
    private float relativePositionToPlayer;
    [SerializeField]
    private float distanceFromPlayer;
    [SerializeField]
    private float minDistanceToAttack;



    public bool StatePursue { get => stateWalk; set => stateWalk = value; }

    // Use this for initialization
    void Start()
    {
        facingRight = true;
        movementSpeed = walkMovementSpeed;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        canAttack = true;
        if(attackCooldown == 0)
        {
            attackCooldown = 1.5f;
        }
        InvokeRepeating("UpdateDistanceFromPlayer", 0, 0.1f);
    }

    private void FixedUpdate()
    {
        if (stateWalk  && !stateIdle)
        {
            pursuePlayer();
        }
    }

    void Update()
    {
        // ================== enemy facing player ==========================
        relativePositionToPlayer = GameLevelManager.instance.Player.transform.position.x - transform.position.x;

        if (GameLevelManager.instance.PlayerState.KnockedDown || !canAttack)
        {
            Debug.Log(" enemy should be idle");
            stateIdle = true;
            Debug.Log("     stateIdle : " + stateIdle);
            Debug.Log("     stateWalk : " + stateWalk);

        }
        else
        {
            stateIdle = false;
        }


        if (relativePositionToPlayer < 0 && facingRight)
        {
            Flip();
        }
        if (relativePositionToPlayer > 0 && !facingRight)
        {
            Flip();
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
        // ================== animation attack state ==========================
        if( math.abs(relativePositionToPlayer) < minDistanceToAttack 
            && canAttack 
            && !stateIdle)
        {
            // attack
            Debug.Log("enemy attack");
            anim.SetTrigger("attack");
            StartCoroutine(AttackCooldown(attackCooldown));
        }

        //if (statePursue)
        //{
        //    pursuePlayer();
        //}
        //if (!statePursue && navmeshAgent.speed > 0)
        //{
        //    navmeshAgent.ResetPath();
        //}
        //distanceFromStartPos = Vector3.Distance(transform.position, pos1.transform.position);
        ////Debug.Log("distanceFromStartPos : " + ( distanceFromStartPos > maxDistance));

        //if (distanceFromStartPos >= maxDistance && movingToTarget)
        //{
        //    outsideRange = true;
        //    insideRange = false;
        //    //Debug.Log("if(distanceFromStartPos > maxDistance && !movingToTarget)");
        //}

        //if (distanceFromStartPos < maxDistance && movingToTarget)
        //{
        //    outsideRange = false;
        //    insideRange = true;
        //    //Debug.Log("if (distanceFromStartPos <= maxDistance)");
        //}

        //// navmesh has no target and inside range
        //if (pathComplete())
        //{
        //    movingToTarget = false;
        //    ignoreCollision = false;
        //}

        //// if outside area
        //if (outsideRange && !movingToTarget && !locked)
        //{
        //    locked = true;
        //    ignoreCollision = true;
        //    movingToTarget = true;

        //    StartCoroutine(waitOutsideRangeForXSeconds(5));
        //}


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

    //protected bool pathComplete()
    //{
    //    if (Vector3.Distance(navmeshAgent.destination, navmeshAgent.transform.position) <= navmeshAgent.stoppingDistance)
    //    {
    //        if (!navmeshAgent.hasPath || navmeshAgent.velocity.sqrMagnitude == 0f)
    //        {
    //            // if not facing goal, flip
    //            if (GameLevelManager.instance.BasketballRimVector.x < 0 && facingRight)
    //            {
    //                Flip();
    //            }
    //            if (GameLevelManager.instance.BasketballRimVector.x > 0 && !facingRight)
    //            {
    //                Flip();
    //            }
    //            return true;
    //        }
    //    }
    //    return false;
    //}

    IEnumerator AttackCooldown(float seconds)
    {
        canAttack = false;
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

    //IEnumerator setWaitForXSeconds(float seconds)
    //{
    //    yield return new WaitForSecondsRealtime(seconds);
    //    waiting = false;
    //}

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
        rigidBody.MovePosition(transform.position + movement );
    }

    public void UpdateDistanceFromPlayer()
    {
        distanceFromPlayer = Vector3.Distance(GameLevelManager.instance.Player.transform.position , transform.position);
    }
}
