using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;

public class callBallToPlayer : MonoBehaviour
{
    [SerializeField]
    Rigidbody basketballRigidBody;
    [SerializeField]
    float pullSpeed;
    [SerializeField]
    Vector3 pullDirection;

    playercontrollerscript playerState;
    BasketBall basketBall;
    BasketballState basketballState;
    bool locked;

    private void Start()
    {
        playerState = gameManager.instance.playerState;
        basketBall = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
        basketballState = GameObject.FindWithTag("basketball").GetComponent<BasketballState>();
        basketballRigidBody = basketBall.GetComponent<Rigidbody>();
        locked = false;
    }

    void pullBallToPlayer()
    {
       //Debug.Log("callBallToPlayer : pullBallToPlayer");
        Vector3 tempDirection = basketballRigidBody.transform.position;
        pullDirection = transform.position - tempDirection;

        //Debug.Log("pullDirection" + pullDirection);
        //Debug.Log("player Direction" + transform.position);
        basketballRigidBody.velocity = pullDirection * pullSpeed;
    }

    private void Update()
    {
        if (InputManager.GetButtonDown("Fire1") 
            && !playerState.hasBasketball 
            && basketballState.CanPullBall 
            && !locked)   
        {
            Debug.Log("callBallToPlayer : pullBallToPlayer");
            locked = true;
            pullBallToPlayer();
            locked = false;
        }
    }
}
