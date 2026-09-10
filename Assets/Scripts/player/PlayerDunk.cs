using System;
using System.Collections;
using Assets.Scripts.Utility;
using UnityEngine;

public class PlayerDunk : MonoBehaviour
{
    IPlayerDunkHost playerHost;
    BasketBallState basketBallState;
    BasketBall basketBall;
    [SerializeField]
    private Vector3 dunkPositionLeft;
    [SerializeField]
    private Vector3 dunkPositionRight;
    [SerializeField]
    private float dunkRangeFeet;
    [SerializeField]
    private float jumpAngle;
    [SerializeField]
    private bool playerCanDunk;

    private void Start()
    {
        GameObject dunkPositionLeftObject = SceneObjects.Find("dunk_position_left", this);
        GameObject dunkPositionRightObject = SceneObjects.Find("dunk_position_right", this);

        // Launch() divides by (H - R * tanAlpha) and takes its Sqrt - a degenerate target (the
        // Vector3.zero default that a missing marker would otherwise silently leave in place) can
        // send Vz/Vy to NaN or fling the player toward the world origin instead of the rim. Disabling
        // dunking through the same PlayerCanDunk gate PlayerController already checks before ever
        // triggering a dunk is safer than letting Launch() run against a position that was never real.
        if (dunkPositionLeftObject == null || dunkPositionRightObject == null)
        {
            playerCanDunk = false;
        }
        else
        {
            dunkPositionLeft = dunkPositionLeftObject.transform.position;
            dunkPositionRight = dunkPositionRightObject.transform.position;
        }

        playerHost = GetComponent<IPlayerDunkHost>();
        IPlayerControllerParticipantState participant = GetComponent<IPlayerControllerParticipantState>();
        GameObject basketballObject = participant.BasketballObject;
        basketBall = basketballObject.GetComponent<BasketBall>();
        basketBallState = basketballObject.GetComponent<BasketBallState>();
        // default dunk values
        jumpAngle = 45;
        dunkRangeFeet = 15;
    }

    //------------------------------------dunk functions ------------------------------------------------------------
    // note - dunk range * 6 will give dunk range in "feet". ex. distance = 2 units is equal to ~ distance = 6 feet (onscreen)
    public void playerDunk()
    {
        playerHost.SetCallBallLocked(true);
        basketBallState.Locked = true;
        playerHost.FaceBasketballGoal(); // turns player facing rim

        float bballRelativePositioning = playerHost.BasketballRimVector.x - transform.position.x;
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
        if (bballRelativePositioning > 0 && !playerHost.Locked)
        {
            Launch(dunkPositionLeft);
        }
        if (bballRelativePositioning < 0 && !playerHost.Locked)
        {
            Launch(dunkPositionRight);
        }
        //BasketBall.instance.BasketBallState.Locked = false;
    }

    public IEnumerator TriggerDunkSequence()
    {
        playerHost.FreezePosition();
        playerHost.PlayAnimation("dunk");

        // wait for anim to start + finish
        yield return new WaitUntil(() => playerHost.CurrentState == playerHost.DunkStateHash);
        yield return new WaitUntil(() => playerHost.CurrentState != playerHost.DunkStateHash);

        basketBallState.Thrown = true;
        playerHost.UnfreezePosition();

        // move ball above rim
        Vector3 temp = basketBallState.BasketBallTarget.transform.position;
        basketBall.Rigidbody.linearVelocity = Vector3.zero;
        basketBall.transform.position = new Vector3(temp.x, temp.y, temp.z);
        //reset
        playerHost.HasBasketball = false;
        playerHost.SetAnimationBool("hasBasketball", false);
    }

    // =================================== Launch ball function =======================================
    void Launch(Vector3 Target)
    {
        playerHost.Locked = true;
        playerHost.RigidBody.linearVelocity = Vector3.zero;

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
            playerHost.RigidBody.linearVelocity = globalVelocity;
            playerHost.PlayAnimation("inair_dunk");
        }

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        playerHost.Locked = false;

    }

    public float DunkRangeFeet { get => dunkRangeFeet; }
    public bool PlayerCanDunk { get => playerCanDunk; }
}
