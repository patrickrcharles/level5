using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    int vehicleId;
    [SerializeField]
    string direction;

    NavMeshAgent navMeshAgent;
    GameObject spawnPoint;

    [SerializeField]
    Vector3 currentTarget;
    [SerializeField]
    float vehicleSpeed;

    [SerializeField]
    int currentTargetIndex;
    int defaultTargetIndex = 0;

    [SerializeField]
    float timeToRespawn;

    //bool facingRight;

    Animator animator;
    Rigidbody rigidbody;

   

    const string vehiclePosMarkersTag = "vehicle_position_marker";


    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.autoBraking = true;
        navMeshAgent.updateRotation = false;
        navMeshAgent.speed = vehicleSpeed;

        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        // if going right --> left, flip object.
        if(direction == "left")
        {
            Flip();
        }
        navMeshAgent.destination = currentTarget;
    }

    // Update is called once per frame
    void Update()
    {

        //set animator speed to transition to move animation
        animator.SetFloat("speed", navMeshAgent.speed);

        //Debug.Log("navMeshAgent.speed : " + navMeshAgent.speed);

        // if reached destination, destroy
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
        {
            // call traffic manager coroutine to respawn a new instance
            // respawn(vehicle, time to respawn)

            //Debug.Log(" spawn vehicle before destroy : "+ this.name);
            TrafficManager.instance.spawnVehicle(VehicleId, Direction, timeToRespawn);
            Destroy(gameObject);
        }
    }

    void GotoNextPoint()
    {
        //// if vehicle target list is empty, bail
        //if (TrafficManager.instance.VehicleTargetPositions.Length == 0)
        //    return;
        //// if current target is final target in targets list, go to first target
        //if (currentTargetIndex == TrafficManager.instance.VehicleTargetPositions.Length - 1)
        //{
        //    currentTargetIndex = 0;
        //}
        //// else, go to next target
        //else
        //{
        //    currentTargetIndex++;
        //}

        //currentTarget = navMeshAgent.destination;
        navMeshAgent.destination = currentTarget;
    }

    void Flip()
    {
        //Debug.Log("flip");
        //facingRight = !facingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
    }

    public int VehicleId { get => vehicleId; set => vehicleId = value; }
    public string Direction { get => direction; set => direction = value; }
    public Vector3 CurrentTarget { get => currentTarget; set => currentTarget = value; }
    //public bool FacingRight { get => facingRight; set => facingRight = value; }
}
