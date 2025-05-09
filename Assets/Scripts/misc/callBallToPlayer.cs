using UnityEngine;

public class CallBallToPlayer : MonoBehaviour
{
    [SerializeField]
    internal float pullSpeed;
    [SerializeField]
    private Vector3 pullDirection;
    [SerializeField]
    private BasketBallState _basketBallState;
    [SerializeField]
    private bool locked;
    [SerializeField]
    public bool CallEnabled = true;

    public bool Locked { get => locked; set => locked = value; }

    private void Start()
    {
        Locked = false;
        pullSpeed = 2.3f;

        if (GameOptions.hardcoreModeEnabled && GameOptions.EnemiesOnlyEnabled)
        {
            CallEnabled = false;
            if (GameOptions.gameModeThreePointContest || GameOptions.gameModeFourPointContest || GameOptions.gameModeSevenPointContest || GameOptions.gameModeAllPointContest)
            {
                CallEnabled = true;
            }
        }
    }


    public void pullBallToPlayer(GameObject basketBall)
    {
        //if (!GameOptions.hardcoreModeEnabled)
        //{
            Rigidbody basketballRigidBody = basketBall.GetComponent<Rigidbody>();

            Vector3 tempDirection = basketballRigidBody.transform.position;
            pullDirection = transform.position - tempDirection;
            basketballRigidBody.linearVelocity = pullDirection * pullSpeed;
        //}
    }
    public void pullBallToPlayerAuto(GameObject basketBallAuto)
    {
        //if (!GameOptions.hardcoreModeEnabled)
        //{
            Rigidbody basketballRigidBody = basketBallAuto.GetComponent<Rigidbody>();

            Vector3 tempDirection = basketBallAuto.transform.position;
            pullDirection = transform.position - tempDirection;
            basketballRigidBody.linearVelocity = pullDirection * pullSpeed;
        //}
    }
}
