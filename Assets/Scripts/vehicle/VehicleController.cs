using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VehicleController : MonoBehaviour
{
    [SerializeField]
    int vehicleId;

    NavMeshAgent navMeshAgent;
    [SerializeField]
    //GameObject[] VehiclePositions;
    GameObject startPoint;

    Vector3 currentTarget;
    [SerializeField]
    float vehicleSpeed;
    int currentTargetIndex = 1;

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

        //VehiclePositions = GameObject.FindGameObjectsWithTag(vehiclePosMarkersTag);
        //startPoint = VehiclePositions[0];
        //gameObject.transform.position = startPoint.transform.position;
        animator = GetComponentInChildren<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        //get vehicle to go to destination
        //currentTarget = VehiclePositions[1].transform.position;
        //currentTarget = navMeshAgent.destination;
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
    //    if (VehiclePositions.Length == 0)
    //        return;

    //    if(currentTargetIndex == VehiclePositions.Length - 1)
    //    {
    //        currentTargetIndex = 0;
    //    }
    //    else
    //    {
    //        currentTargetIndex++;
    //    }

    //    currentTarget = navMeshAgent.destination;
    //    navMeshAgent.destination = VehiclePositions[currentTargetIndex].transform.position;
    //}

    void flip()
    {

    }
}
