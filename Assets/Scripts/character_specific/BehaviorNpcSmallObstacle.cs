﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BehaviorNpcSmallObstacle : MonoBehaviour
{

    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;
    public float punchCooldown;
    public float chargeSpeed;

    [SerializeField]
    public bool facingRight;
    public bool walking;
    //public bool canMove;

    public GameObject pos1, pos2, pos3;

    float distanceFromStartPos;
    bool locked;
    //GameObject player;

    private float movementSpeed;
    private Rigidbody rigidBody;
    private NavMeshAgent navmeshAgent;
    public SpriteRenderer currentSprite;

    //public GameObject playerHitbox;
    Animator anim;
    AnimatorStateInfo currentStateInfo;

    static int currentState;
    static int idleState = Animator.StringToHash("base.idle");
    static int idleState2 = Animator.StringToHash("base.idle2");
    static int walkState = Animator.StringToHash("base.walk");
    static int runState = Animator.StringToHash("base.run");
    [SerializeField]
    Vector3 playerRelativePosition;
    [SerializeField]
    bool waiting;

    public bool ignoreCollision;
    public bool idle;
    public bool moving;
    public bool outsideRange;
    public bool insideRange;
    public bool movingToTarget;

    public float maxDistance;

    private GameObject[] returnPositions;

    // Use this for initialization
    void Start()
    {
        //player = GameLevelManager.instance.Player;
        facingRight = true;
        //canMove = true;
        movementSpeed = walkMovementSpeed;
        currentSprite = transform.Find("sprite").GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("sprite").GetComponent<Animator>();

        // positions flash will retreat to
        returnPositions = GameObject.FindGameObjectsWithTag("flash_return_position");
        pos1 = returnPositions[0];
        pos2 = returnPositions[1];
        pos3 = returnPositions[2];

        locked = false;

        InvokeRepeating("checkNPCState", 0, 1f);
    }


    void Update()
    {
       
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        // ----- control speed based on commands----------
        if (currentState == idleState || currentState == walkState
        || currentState == idleState2)
        {
            movementSpeed = walkMovementSpeed;
        }
        else
        {
            movementSpeed = runMovementSpeed;
        }
        //rigidBody.velocity = movement * movementSpeed;
        navmeshAgent.speed = movementSpeed;
        if (rigidBody != null)
        {
            anim.SetFloat("speed", rigidBody.velocity.sqrMagnitude);
        }
        ////check if walking
        ////  function will flip sprite if needed
        isWalking(navmeshAgent.velocity.magnitude);
    }

    private void checkNPCState()
    {
        distanceFromStartPos = Vector3.Distance(transform.position, pos1.transform.position);
        //Debug.Log("distanceFromStartPos : " + ( distanceFromStartPos > maxDistance));

        if (distanceFromStartPos >= maxDistance && movingToTarget)
        {
            outsideRange = true;
            insideRange = false;
            //Debug.Log("if(distanceFromStartPos > maxDistance && !movingToTarget)");
        }

        if (distanceFromStartPos < maxDistance && movingToTarget)
        {
            outsideRange = false;
            insideRange = true;
            //Debug.Log("if (distanceFromStartPos <= maxDistance)");
        }

        // navmesh has no target and inside range
        if (pathComplete())
        {
            movingToTarget = false;
            ignoreCollision = false;
        }

        // if outside area
        if (outsideRange && !movingToTarget && !locked)
        {
            locked = true;
            ignoreCollision = true;
            movingToTarget = true;

            StartCoroutine(waitOutsideRangeForXSeconds(5));
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if ((gameObject.name.Contains("flash") || gameObject.name.Contains("mouse") || gameObject.name.Contains("ghost")) 
        if (gameObject.CompareTag("auto_npc")
            && (other.CompareTag("Player") || other.CompareTag("basketball") || other.CompareTag("knock_down_attack"))
            && !ignoreCollision && !movingToTarget)
        {
            movingToTarget = true;

            Vector3 newVector = getRandomTransformFromPlayerPosition();
            Vector3 oldVector = transform.position;
            Vector3 relativePosition = newVector - oldVector;

            if (relativePosition.x < 0 && facingRight)
            {
                Flip();
            }
            if (relativePosition.x > 0 && !facingRight)
            {
                Flip();
            }
            navmeshAgent.SetDestination(newVector);
            //disable rotation
            navmeshAgent.updateRotation = false;
        }
    }

    IEnumerator waitOutsideRangeForXSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        List<GameObject> list = new List<GameObject> { pos1, pos2, pos3 };
        int finder = Random.Range(0, list.Capacity - 1); //Then you just use this; nameDisplayString = names[finder];
        GameObject randPos = list[finder];

        Vector3 relativePosition = randPos.transform.position - transform.position;

        if (relativePosition.x < 0 && facingRight)
        {
            Flip();
        }

        if (relativePosition.x > 0 && !facingRight)
        {
            Flip();
        }

        navmeshAgent.SetDestination(randPos.transform.position);
        navmeshAgent.updateRotation = false;
        locked = false;
    }

    protected bool pathComplete()
    {
        if (Vector3.Distance(navmeshAgent.destination, navmeshAgent.transform.position) <= navmeshAgent.stoppingDistance)
        {
            if (!navmeshAgent.hasPath || navmeshAgent.velocity.sqrMagnitude == 0f)
            {
                // if not facing goal, flip
                if (GameLevelManager.instance.BasketballRimVector.x < 0 && facingRight)
                {
                    Flip();
                }
                if (GameLevelManager.instance.BasketballRimVector.x > 0 && !facingRight)
                {
                    Flip();
                }
                return true;
            }
        }
        return false;
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


    IEnumerator setWaitForXSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        waiting = false;
    }

    private Vector3 getRandomTransformFromPlayerPosition()
    {
        Vector3 newTransform = new Vector3(transform.position.x + RandomNumber(-5, 5),
            transform.position.y,
            transform.position.z + RandomNumber(-3, 2));

        return newTransform;
    }

    int RandomNumber(int min, int max)
    {
        System.Random rnd = new System.Random();
        int randNum = rnd.Next(min, max);
        return randNum;
    }
}

