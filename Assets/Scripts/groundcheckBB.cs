using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class groundcheckBB : MonoBehaviour {

    GameObject player;
    playercontrollerscript playerState;
    public float initialHeight, finalHeight;

    [SerializeField]
    basketBall basketball;
    //Rigidbody rigidBody;

    void Awake()
    {
        //Debug.Log("groundcheck.cs :: Awake( )");
        basketball = transform.root.gameObject.GetComponent<basketBall>();
        //rigidBody = basketball.GetComponent<Rigidbody>();
        playerState = gameManager.instance.playerState;
        //Debug.Log("playerState = " + playerState);
    }
    
    void Update()
    {
        //// if player is falling
        //if (rigidBody.velocity.y > 0)
        //{
        //    //updates "highest point" as long at player still moving upwards ( velcoity > 0)
        //    finalHeight = basketball.transform.position.y;
        //    //Debug.Log("intialHeight : " + initialHeight);  
        //    //Debug.Log("finalHeight : " + finalHeight);
        //}
        //// if player is falling
        //if (rigidBody.velocity.y < 0.1f && rigidBody.velocity.y > -0.1f)
        //{
        //    //updates "highest point" as long at player still moving upwards ( velcoity > 0)
        //    //finalHeight = basketball.transform.position.y;
        //    //Debug.Log("============ basketball peak : velocity y"+ rigidBody.velocity.y);  
        //    //Debug.Log("finalHeight : " + finalHeight);
        //}
    }

    public void OnTriggerStay(Collider other)
    {
        // later 11 is ground/terrain
        if ( other.gameObject.layer == 11 || other.gameObject.name.Contains("TERRAIN")) //ground layer
        {
            //Debug.Log("----------------- other.gameobject: " + other.gameObject );
            //Debug.Log("----------------- if (other.gameObject.layer == 11)");
            //finalHeight = player.transform.position.y;
            initialHeight = basketball.transform.position.y;
            if ((finalHeight - initialHeight) > 1)
            {
                //Debug.Log("fall distance : " + (finalHeight - initialHeight));
            }
            basketball.grounded = true;
            basketball.inAir = false;
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

        if (other.gameObject.layer == 11 || other.gameObject.name.Contains("TERRAIN"))//ground layer
        {
            basketball.grounded = false;
            basketball.inAir = true;
            //basketball.grounded = false;
            //if (playerState.hasBasketball)
            //{
            //    basketball.inAir = false;
            //}
            //if (!playerState.hasBasketball)
            //{
            //    basketball.inAir = true;
            //}
            // height when player exits ground (fall/jump etc.)
            //initialHeight = player.transform.position.y;

            //Debug.Log("intialHeight : " + initialHeight);
            //Debug.Log("::::: groundcheck.cs :: OnTriggerExit");
            //Debug.Log("     gameobject : "+ gameObject.transform.name+"   other : "+other.transform.name);
            //Debug.Log("----------------- grounded : " + playerState.grounded);

        }
        
    }
}
