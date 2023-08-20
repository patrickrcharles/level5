using UnityEngine;

public class CallBallToPlayer : MonoBehaviour
{
    [SerializeField]
    internal float pullSpeed;
    [SerializeField]
    //private Rigidbody basketballRigidBody;
    private Vector3 pullDirection;

    //private AutoPlayerController autoPlayerState;
    //private PlayerController playerState;
    [SerializeField]
    private BasketBallState _basketBallState;
    [SerializeField]
    private bool locked;
    [SerializeField]
    public bool CallEnabled = true;

    //public static CallBallToPlayer instance;

    public bool Locked { get => locked; set => locked = value; }

    private void Start()
    {
        //_basketBallState = BasketBallState.instance;
        //basketballRigidBody = GameObject.FindGameObjectWithTag("basketball").GetComponent<Rigidbody>();
        Locked = false;
        pullSpeed = 2.3f;

        if (GameOptions.hardcoreModeEnabled && GameOptions.EnemiesOnlyEnabled)
        {
            CallEnabled = false;
            if (GameOptions.gameModeThreePointContest || GameOptions.gameModeFourPointContest || GameOptions.gameModeAllPointContest)
            {
                CallEnabled = true;
            }
        }
    }



    public void pullBallToPlayer(GameObject basketBall)
    {
        Rigidbody basketballRigidBody = basketBall.GetComponent<Rigidbody>();

        Vector3 tempDirection = basketballRigidBody.transform.position;
        pullDirection = transform.position - tempDirection;
        basketballRigidBody.velocity = pullDirection * pullSpeed;
    }
    public void pullBallToPlayerAuto(GameObject basketBallAuto)
    {
        Rigidbody basketballRigidBody = basketBallAuto.GetComponent<Rigidbody>();

        Vector3 tempDirection = basketBallAuto.transform.position;
        pullDirection = transform.position - tempDirection;
        basketballRigidBody.velocity = pullDirection * pullSpeed;
    }
}
