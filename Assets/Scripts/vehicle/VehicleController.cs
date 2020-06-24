using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    int vehicleId;
    [SerializeField]
    float vehicleSpeed;
    [SerializeField]
    float timeToRespawn;

    // these 2 vars need to be serialzed because they set before a clone of them is 
    // configured and instantiated

    public Vector3 currentTarget;
    public string direction;

    int currentTargetIndex;
    int defaultTargetIndex = 0;
    const string vehiclePosMarkersTag = "vehicle_position_marker";

    NavMeshAgent navMeshAgent;
    GameObject spawnPoint;
    Animator animator;
    Rigidbody rigidbody;


    public bool facingRight;
    Transform bballRimVector;
    public float relativePositioning;


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = true;
        navMeshAgent.updateRotation = false;
        navMeshAgent.speed = vehicleSpeed;

        bballRimVector = GameObject.Find("rim").transform;

        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        // where is vehicle spawned in relation to rim
        relativePositioning = bballRimVector.transform.position.x - gameObject.transform.position.x;

        // determine which way Gameobject is facing
        if(transform.localScale.x > 0)
        {
            facingRight = true;
        }
        else
        {
            facingRight = false;
        }

        //if vehicle is on right side of rim, flip
        if (relativePositioning < 0 && facingRight)
        {
            Flip();
        }
        //if vehicle is on right side of rim, flip
        if (relativePositioning > 0 && !facingRight)
        {
            Flip();
        }

        Debug.Log("vehicle : " + gameObject.name);

        navMeshAgent.destination = CurrentTarget;
    }

    // Update is called once per frame
    void Update()
    {

        //set animator speed to transition to move animation
        animator.SetFloat("speed", navMeshAgent.speed);

        // if reached destination, destroy
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
        {
            // call traffic manager coroutine to respawn a new instance
            TrafficManager.instance.spawnVehicle(VehicleId, Direction, timeToRespawn);
            resetVehicleDefaults();
            Destroy(gameObject);
        }
    }

    public void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
    }

    void resetVehicleDefaults()
    {
        Direction = "";
        if (!facingRight)
        {
            Flip();
        }
    }

    public int VehicleId { get => vehicleId; set => vehicleId = value; }
    public string Direction { get => direction; set => direction = value; }
    public Vector3 CurrentTarget { get => currentTarget; set => currentTarget = value; }
    public bool FacingRight { get => facingRight; set => facingRight = value; }
}
