﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class behavior_primo : MonoBehaviour
{

    AudioSource moonwalkAudio;

    public float walkMovementSpeed;
    public float runMovementSpeed;
    public float attackMovementSpeed;
    public float punchCooldown;
    public float chargeSpeed;

    public float xMin, xMax, zMin, zMax, yMin, yMax;
    public bool facingRight, walking;
    public bool canMove;

    public GameObject pos1, pos2, pos3;

    [SerializeField]
    float distanceFromStartPos;
    [SerializeField]
    bool locked;
    GameObject player;

    private float movementSpeed;
    private Rigidbody rigidBody;
    [SerializeField]
    private NavMeshAgent navmeshAgent;
    [SerializeField]
    public SpriteRenderer currentSprite;

    public GameObject playerHitbox;
    [SerializeField]
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
    private bool reachedDestination;
    private bool isSleeping;
    private bool followPlayer;

    // Use this for initialization
    void Start()
    {
        player = GameLevelManager.instance.player;
        facingRight = true;
        canMove = true;
        followPlayer = false;
        movementSpeed = walkMovementSpeed;
        currentSprite = transform.Find("sprite").GetComponent<SpriteRenderer>();
        rigidBody = GetComponent<Rigidbody>();
        navmeshAgent = GetComponent<NavMeshAgent>();
        anim = transform.Find("sprite").GetComponent<Animator>();
        locked = false;
    }


    // not affected by framerate
    void FixedUpdate()
    {

    }

    void Update()
    {
        distanceFromStartPos = Vector3.Distance(transform.position, pos1.transform.position);
        //Debug.Log("distanceFromStartPos : " + ( distanceFromStartPos > maxDistance));

        if(distanceFromStartPos >= maxDistance  )
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
        if (pathComplete() && !outsideRange && !reachedDestination )
        {
           //Debug.Log("       if (pathComplete() && !outsideRange )");
            reachedDestination = true;
            movingToTarget = false;
            ignoreCollision = false;
        }

        if (reachedDestination && !isSleeping)
        {
            locked = true;
            StartCoroutine(PrimoSleepInRandomXSeconds());
        }

        // if outside area
        if (outsideRange && !movingToTarget && !locked && followPlayer )
        {
            locked = true;
            ignoreCollision = true;
            movingToTarget = true;

            StartCoroutine( waitOutsideRangeForXSeconds( 1));
        }


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

        ////set limits for player movement
        //rigidBody.transform.position = new Vector3(
        //   Mathf.Clamp(rigidBody.position.x, xMin, xMax),
        //   Mathf.Clamp(rigidBody.position.y, yMin, yMax),
        //   Mathf.Clamp(rigidBody.position.z, zMin, zMax)
        //   );

        //anim.SetFloat("speed", rigidBody.velocity.sqrMagnitude);

        ////check if walking
        ////  function will flip sprite if needed
        isWalking(navmeshAgent.velocity.magnitude);
    }

    IEnumerator PrimoSleepInRandomXSeconds()
    {
        int randomTimeToSleep = RandomNumber(7, 20);
        yield  return new WaitForSecondsRealtime(randomTimeToSleep);
        isSleeping = true;
        anim.SetBool("sleep", true);
        locked = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // if collsion, wake primo
        //Debug.Log(" OnTrigger primo :  " + gameObject.name + " other : " + other.name);

        if (gameObject.name == "primo" 
            && (other.name.Contains("Player") || other.CompareTag("basketball") || other.name.Contains("flash"))
            && !movingToTarget
            && followPlayer)
        {
           //Debug.Log(" OnTrigger primo :  " + gameObject.name + " other : " + other.name);
            anim.SetBool("sleep", false);
            isSleeping = false;

            //movingToTarget = true;
        }
        if (gameObject.name == "primo"
            && (other.name.Contains("Player") 
            && !movingToTarget
            && !followPlayer))
        {
            followPlayer = true;

           //Debug.Log(" OnTrigger primo :  " + gameObject.name + " other : " + other.name);
            anim.SetBool("sleep", false);
            isSleeping = false;
        }

    }

    IEnumerator waitOutsideRangeForXSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);

        //List<GameObject> list = new List<GameObject> { pos1, pos2, pos3 };
        //int finder = Random.Range(0, list.Capacity - 1); //Then you just use this; nameDisplayString = names[finder];
        //GameObject randPos = list[finder];

        Vector3 relativePosition = pos1.transform.position - transform.position;

        if (relativePosition.x < 0 && facingRight)
        {
            Flip();
        }

        if (relativePosition.x > 0 && !facingRight)
        {
            Flip();
        }

        navmeshAgent.SetDestination(pos1.transform.position);
        navmeshAgent.updateRotation = false;
        locked = false;
    }

    protected bool pathComplete()
    {
        if (Vector3.Distance(navmeshAgent.destination, navmeshAgent.transform.position) <= navmeshAgent.stoppingDistance)
        {
            if (!navmeshAgent.hasPath || navmeshAgent.velocity.sqrMagnitude == 0f)
            {
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
            anim.SetBool("walking", true);
        }
        else
        {
            anim.SetBool("walking", false);
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

        //Debug.Log("generate new transform : " + newTransform);

        return newTransform;
    }

    int RandomNumber(int min, int max)
    {
        System.Random rnd = new System.Random();
        int randNum = rnd.Next(min, max);
        //Debug.Log("generate randNum : " + randNum);
        return randNum;
    }
}

