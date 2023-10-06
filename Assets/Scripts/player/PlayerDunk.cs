using System;
using System.Collections;
using UnityEngine;

public class PlayerDunk : MonoBehaviour
{
    PlayerController playerController;
    BasketBallState basketBallState;
    BasketBall basketBall;
    private Vector3 dunkPositionLeft;
    private Vector3 dunkPositionRight;
    [SerializeField]
    private float dunkRangeFeet;
    [SerializeField]
    private float jumpAngle;
    [SerializeField]
    private bool playerCanDunk;

    private void Start()
    {
        if (GameObject.Find("basketball_goal") != null)
        {
            dunkPositionLeft = GameObject.Find("dunk_position_left").transform.position;
            dunkPositionRight = GameObject.Find("dunk_position_right").transform.position;
        }
        PlayerIdentifier pi = GetComponent<PlayerIdentifier>();
        playerController = pi.playerController;
        basketBall = pi.basketBallController;
        basketBallState = pi.basketBallState;
        // default dunk values
        jumpAngle = 45;
        dunkRangeFeet = 15;
    }

    //------------------------------------dunk functions ------------------------------------------------------------
    // note - dunk range * 6 will give dunk range in "feet". ex. distance = 2 units is equal to ~ distance = 6 feet (onscreen)
    public void playerDunk()
    {
        playerController.CallBallToPlayer.Locked = true;
        basketBallState.Locked = true;
        playerController.CheckIsPlayerFacingGoal(); // turns player facing rim

        float bballRelativePositioning = GameLevelManager.instance.BasketballRimVector.x - transform.position.x;
        // shot type for stats
        basketBall.updateBasketBallStateShotTypeOnShoot(basketBallState.TwoPoints,
            basketBallState.ThreePoints,
            basketBallState.FourPoints,
            basketBallState.SevenPoints);
        //calculate shot distance 
        Vector3 target = basketBallState.BasketBallTarget.transform.position;
        Vector3 tempPos = new Vector3(target.x, 0, target.z);
        float tempDist = Vector3.Distance(tempPos, basketBall.BasketBallPosition.transform.position);
        basketBall.LastShotDistance = tempDist;

        // determine which side to dunk on
        if (bballRelativePositioning > 0 && !playerController.Locked)
        {
            Launch(dunkPositionLeft);
        }
        if (bballRelativePositioning < 0 && !playerController.Locked)
        {
            Launch(dunkPositionRight);
        }
        //BasketBall.instance.BasketBallState.Locked = false;
    }

    public IEnumerator TriggerDunkSequence()
    {
        playerController.FreezePlayerPosition();
        playerController.PlayAnim("dunk");

        // wait for anim to start + finish
        yield return new WaitUntil(() => playerController.CurrentState == playerController.dunkState);
        yield return new WaitUntil(() => playerController.currentState != playerController.dunkState);

        basketBallState.Thrown = true;
        playerController.UnFreezePlayerPosition();

        // move ball above rim
        Vector3 temp = basketBallState.BasketBallTarget.transform.position;
        basketBall.Rigidbody.velocity = Vector3.zero;
        basketBall.transform.position = new Vector3(temp.x, temp.y, temp.z);
        //reset
        playerController.hasBasketball = false;
        playerController.SetPlayerAnim("hasBasketball", false);
    }

    // =================================== Launch ball function =======================================
    void Launch(Vector3 Target)
    {
        playerController.Locked = true;
        playerController.RigidBody.velocity = Vector3.zero;

        Vector3 projectileXZPos = transform.position;
        Vector3 targetXZPos = Target;

        // rotate the object to face the target
        transform.LookAt(targetXZPos);

        // shorthands for the formula
        float R = Vector3.Distance(projectileXZPos, targetXZPos);
        float G = Physics.gravity.y;
        float tanAlpha = Mathf.Tan(jumpAngle * Mathf.Deg2Rad);
        float H = targetXZPos.y - projectileXZPos.y;
        float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
        float Vy = tanAlpha * Vz;

        float xVector = 0;
        float yVector = Vy; // + (accuracyModifier * shooterProfile.shootYVariance);
        float zVector = Vz; //+ accuracyModifierZ; // + (accuracyModifier * shooterProfile.shootZVariance);

        // create the velocity vector in local space and get it in global space
        Vector3 localVelocity = new Vector3(xVector, yVector, zVector);

        Vector3 globalVelocity = transform.TransformDirection(localVelocity);

        // launch the object by setting its initial velocity and flipping its state
        if (Math.Abs(globalVelocity.y) < 7 && Math.Abs(globalVelocity.z) < 7)
        {
            playerController.RigidBody.velocity = globalVelocity;
            playerController.PlayAnim("inair_dunk");
        }

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        playerController.Locked = false;

    }

    public float DunkRangeFeet { get => dunkRangeFeet; }
    public bool PlayerCanDunk { get => playerCanDunk; }
}
