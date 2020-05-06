using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class callBallToPlayer : MonoBehaviour
{
    [SerializeField]
    Rigidbody basketballRigidBody;
    [SerializeField]
    float pullSpeed;
    [SerializeField]
    Vector3 pullDirection;

    PlayerController playerState;
    BasketBall basketBall;
    BasketBallState _basketBallState;
    bool locked;

    private bool canBallToPlayerEnabled;

    static public callBallToPlayer instance;

    private void Start()
    {
        instance = this;
        playerState = GameLevelManager.instance.PlayerState;
        basketBall = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
        _basketBallState = GameObject.FindWithTag("basketball").GetComponent<BasketBallState>();
        basketballRigidBody = basketBall.GetComponent<Rigidbody>();
        locked = false;
        //if (SceneManager.GetActiveScene().name.Equals("level_03_practice"))
        //{
        //    canBallToPlayerEnabled = true;
        //    PlayerData.instance.IsCheating = true;
        //}
        //else
        //{
        //    canBallToPlayerEnabled = false;
        //}
    }

    void pullBallToPlayer()
    {
        Vector3 tempDirection = basketballRigidBody.transform.position;
        pullDirection = transform.position - tempDirection;
        basketballRigidBody.velocity = pullDirection * pullSpeed;
    }

    private void Update()
    {
        if (InputManager.GetButtonDown("Fire1")
            && !playerState.hasBasketball
            && _basketBallState.CanPullBall
            && canBallToPlayerEnabled
            && !locked)
        {
            ////Debug.Log("callBallToPlayer : pullBallToPlayer");
            locked = true;
            pullBallToPlayer();
            locked = false;
        }
    }

    public void toggleCallBallToPlayer()
    {

        canBallToPlayerEnabled = !canBallToPlayerEnabled;
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "call ball to player = " + canBallToPlayerEnabled + "\nhigh score saving disabled";

        // turn off text display after 5 seconds
        StartCoroutine(BasketBall.instance.turnOffMessageLogDisplayAfterSeconds(5));
    }
}
