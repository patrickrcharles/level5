using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CallBallToPlayer : MonoBehaviour
{
    [SerializeField] internal float pullSpeed;
    private Rigidbody basketballRigidBody;
    private Vector3 pullDirection;

    private PlayerController playerState;
    private BasketBall basketBall;
    private BasketBallState _basketBallState;

    private bool locked;
    private bool canBallToPlayerEnabled;

    public static CallBallToPlayer instance;

    private void Start()
    {
        instance = this;
        playerState = GameLevelManager.Instance.PlayerState;
        basketBall = GameLevelManager.Instance.Basketball;
        _basketBallState = basketBall.GetComponent<BasketBallState>();
        basketballRigidBody = basketBall.GetComponent<Rigidbody>();
        locked = false;
        canBallToPlayerEnabled = true;
    }

    private void pullBallToPlayer()
    {
        Vector3 tempDirection = basketballRigidBody.transform.position;
        pullDirection = transform.position - tempDirection;
        basketballRigidBody.velocity = pullDirection * pullSpeed;
    }

    private void Update()
    {
        if (InputManager.GetButtonDown("Fire1")
            && !playerState.hasBasketball
            && !playerState.inAir
            && _basketBallState.CanPullBall
            && canBallToPlayerEnabled
            && !locked)
        {
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
