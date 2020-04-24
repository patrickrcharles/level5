using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundcheckAutoPlay : MonoBehaviour {

    [SerializeField]
    GameObject player;
    playercontrollerscriptAutoPlay playerState;
    public float initialHeight, finalHeight;
    Rigidbody rigidBody;

    void Start()
    {
        //Debug.Log("groundcheck.cs :: Awake( )");
        player = gameManagerAutoPlay.instance.player;

        //Debug.Log("player = "+ player.name);
        playerState = gameManagerAutoPlay.instance.playerState;
        //rigidBody = player.GetComponent<Rigidbody>();

        //Debug.Log("playerState = " + playerState);
    }
    
    void Update()
    {
        /*
        // if player is falling
        if (rigidBody.velocity.y > 0)
        {
            //updates "highest point" as long at player still moving upwards ( velcoity > 0)
            finalHeight = player.transform.position.y;
            //Debug.Log("intialHeight : " + initialHeight);  
            //Debug.Log("finalHeight : " + finalHeight);
            
        }

        //if (rigidBody.velocity.y < 0.1f && rigidBody.velocity.y > -0.1f)
        //{
        //    //updates "highest point" as long at player still moving upwards ( velcoity > 0)
        //    //finalHeight = basketball.transform.position.y;
        //    //Debug.Log("============ player  peak : rigidBody.velocity.y == 0)");
        //   //Debug.Log("============ player  jump peak : velocity y " + rigidBody.velocity.y);
        //    //Debug.Log("finalHeight : " + finalHeight);
        //}

        if (rigidBody.velocity.y == 0)
        {
            //updates "highest point" as long at player still moving upwards ( velcoity > 0)
            //finalHeight = basketball.transform.position.y;
            //Debug.Log("============ player  peak : rigidBody.velocity.y == 0)");
           //Debug.Log(" ******************** PLAYER PEAK JUMP ***************************");
           //Debug.Log("final height:: " + finalHeight);
            //Debug.Log("finalHeight : " + finalHeight);
        }
        */
    }

    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if ( other.gameObject.layer == 11 )
        {
            //Debug.Log("----------------- other.gameobject: " + other.gameObject );
            //Debug.Log("----------------- if (other.gameObject.layer == 11)");
            //finalHeight = player.transform.position.y;
            initialHeight = player.transform.position.y;
            if ((finalHeight - initialHeight) > 1)
            {
                //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            }

            playerState.grounded = true;
            playerState.inAir = false;
            //playerState.jump = false;
            playerState.setPlayerAnim("jump", false);

            //Debug.Log("::::: groundcheck.cs :: OnTriggerStay");
            //Debug.Log("     gameobject : " + gameObject.transform.name + "   other : " + other.transform.name);
            //Debug.Log("----------------- grounded : " + playerState.grounded);
            //Debug.Log("intialHeight : " + initialHeight);

            //Debug.Log("----------------- if (!playerState.dead) grounded : " + playerState.grounded);
        }
        
    }
    
    public void OnTriggerExit(Collider other)
    {
        //Debug.Log("     gameobject: " + gameObject + " layer : " + other.gameObject.layer);

        if ( other.gameObject.layer == 11) //ground layer
        {

            // height when player exits ground (fall/jump etc.)
            //initialHeight = player.transform.position.y;
            playerState.grounded = false;
            playerState.inAir = true;
            playerState.setPlayerAnim("jump", true);
            //Debug.Log("intialHeight : " + initialHeight);
            //Debug.Log("::::: groundcheck.cs :: OnTriggerExit");
            //Debug.Log("     gameobject : "+ gameObject.transform.name+"   other : "+other.transform.name);
            //Debug.Log("----------------- grounded : " + playerState.grounded);
            
        }
        
    }
}
