using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;

public class callBallToPlayer : MonoBehaviour
{

    Rigidbody basketballRigidBody;

    [SerializeField]
    float pullSpeed;
    [SerializeField]
    Vector3 pullDirection;
    playercontrollerscript playerState;
    [SerializeField]
    basketBall basketBall;

    private void Start()
    {
        playerState = gameManager.instance.playerState;
        basketBall = GameObject.FindWithTag("basketball").GetComponent<basketBall>();
        basketballRigidBody = basketBall.GetComponent<Rigidbody>();
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
        if (InputManager.GetButtonDown("Fire1") && !playerState.hasBasketball && basketBall.canPullBall  )   
        {
            pullBallToPlayer();
        }
    }
}
