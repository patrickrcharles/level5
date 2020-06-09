using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    int vehicleId;
    string direction;

    NavMeshAgent navMeshAgent;
    [SerializeField]
    //GameObject[] VehiclePositions;
    GameObject spawnPoint;

    Vector3 currentTarget;
    [SerializeField]
    float vehicleSpeed;

    int currentTargetIndex;
    int defaultTargetIndex = 0;

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

        currentTargetIndex = defaultTargetIndex;
    }

    // Update is called once per frame
    void Update()
    {

        //set animator speed to transition to move animation
        animator.SetFloat("speed", rigidbody.velocity.sqrMagnitude);

        // if reached destination
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance < 0.1f)
        {
            //GotoNextPoint();
        }
    }

    //void GotoNextPoint()
    //{
    //    // if vehicle target list is empty, bail
    //    if (TrafficManager.instance.VehicleTargetPositions.Length == 0)
    //        return;
    //    // if current target is final target in targets list, go to first target
    //    if (currentTargetIndex == TrafficManager.instance.VehicleTargetPositions.Length - 1)
    //    {
    //        currentTargetIndex = 0;
    //    }
    //    // else, go to next target
    //    else
    //    {
    //        currentTargetIndex++;
    //    }

    //    currentTarget = navMeshAgent.destination;
    //    navMeshAgent.destination = TrafficManager.instance.VehicleTargetPositions[currentTargetIndex].transform.position;
    //}

    void flip()
    {

    }

    public int VehicleId { get => vehicleId; set => vehicleId = value; }
    public string Direction { get => direction; set => direction = value; }
    public Vector3 CurrentTarget { get => currentTarget; set => currentTarget = value; }
}
