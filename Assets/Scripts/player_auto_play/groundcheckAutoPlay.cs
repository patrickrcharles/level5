using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class groundcheckAutoPlay : MonoBehaviour {

    [SerializeField]
    GameObject player;
    playercontrollerscriptAutoPlay playerState;
    private NavMeshAgent navMeshAgent;

    public float initialHeight, finalHeight;
    Rigidbody rigidBody;


    void Start()
    {

        player = gameManagerAutoPlay.instance.player;
        playerState = gameManagerAutoPlay.instance.playerState;
        navMeshAgent = player.GetComponent<NavMeshAgent>();
    }
    
    void Update()
    {

    }

    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if ( other.gameObject.layer == 11 )
        {
            //Debug.Log("groundcheck enter    other.gameobject: " + other.gameObject );
            //Debug.Log("----------------- if (other.gameObject.layer == 11)");
            //finalHeight = player.transform.position.y;
            initialHeight = player.transform.position.y;
            if ((finalHeight - initialHeight) > 1)
            {
                //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            }

            playerState.grounded = true;
            playerState.inAir = false;
            playerState.setPlayerAnim("jump", false);

        }
        
    }
    
    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("groundcheck exit     gameobject: " + gameObject + " layer : " + other.gameObject.layer);

        if ( other.gameObject.layer == 11) //ground layer
        {
            //navMeshAgent.enabled = false;
            playerState.grounded = false;
            playerState.inAir = true;
            playerState.setPlayerAnim("jump", true);

        }
        
    }
}
