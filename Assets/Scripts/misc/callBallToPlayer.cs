using UnityEngine;

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

    public bool Locked { get => locked; set => locked = value; }

    private void Start()
    {
        instance = this;
        playerState = GameLevelManager.instance.PlayerState;
        basketBall = GameLevelManager.instance.Basketball;
        _basketBallState = basketBall.GetComponent<BasketBallState>();
        basketballRigidBody = basketBall.GetComponent<Rigidbody>();
        Locked = false;

        canBallToPlayerEnabled = true;
        pullSpeed = 2.0f;
    }

    private void LateUpdate()
    {
        if (GameLevelManager.instance.Controls.Player.shoot.triggered
            && GameLevelManager.instance.Controls.Other.change.ReadValue<float>() == 0
            && GameLevelManager.instance.PlayerState.CurrentState != GameLevelManager.instance.PlayerState.BlockState
            && !playerState.hasBasketball
            && _basketBallState.CanPullBall
            && !_basketBallState.Locked
            && playerState.grounded
            && !Locked)
        {
            //Debug.Log("call ball input read");
            Locked = true;
            pullBallToPlayer();
            Locked = false;
        }
    }


    public void pullBallToPlayer()
    {
        //Debug.Log("pullBallToPlayer()");
        Vector3 tempDirection = basketballRigidBody.transform.position;
        pullDirection = transform.position - tempDirection;
        basketballRigidBody.velocity = pullDirection * pullSpeed;
    }

    //public void toggleCallBallToPlayer()
    //{
    //    canBallToPlayerEnabled = !canBallToPlayerEnabled;
    //    Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
    //    messageText.text = "call ball to player = " + canBallToPlayerEnabled + "\nhigh score saving disabled";

    //    // turn off text display after 5 seconds
    //    StartCoroutine(BasketBall.instance.turnOffMessageLogDisplayAfterSeconds(3));
    //}
}
