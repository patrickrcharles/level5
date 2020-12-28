using System;
using System.Collections;
using UnityEngine;

public class PlayerDunk : MonoBehaviour
{
    PlayerController playerController;

    private Vector3 dunkPositionLeft;
    private Vector3 dunkPositionRight;
    [SerializeField]
    private float dunkRangeFeet;
    [SerializeField]
    private float jumpAngle;

    public static PlayerDunk instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }
    private void Start()
    {
        dunkPositionLeft = GameObject.Find("dunk_position_left").transform.position;
        dunkPositionRight = GameObject.Find("dunk_position_right").transform.position;
        playerController = GameLevelManager.instance.PlayerState;
        // default dunk values
        jumpAngle = 45;
        dunkRangeFeet = 15;
    }

    //------------------------------------dunk functions ------------------------------------------------------------
    // note - dunk range * 6 will give dunk range in "feet". ex. distance = 2 units is equal to ~ distance = 6 feet (onscreen)
    public void playerDunk()
    {
        CallBallToPlayer.instance.Locked = true;
        BasketBall.instance.BasketBallState.Locked = true;
        playerController.checkIsPlayerFacingGoal(); // turns player facing rim

        float bballRelativePositioning = GameLevelManager.instance.BasketballRimVector.x - transform.position.x;
        // shot type for stats
        BasketBall.instance.updateBasketBallStateShotTypeOnShoot();
        //calculate shot distance 
        Vector3 target = BasketBall.instance.BasketBallState.BasketBallTarget.transform.position;
        Vector3 tempPos = new Vector3(target.x, 0, target.z);
        float tempDist = Vector3.Distance(tempPos, BasketBall.instance.BasketBallPosition.transform.position);
        BasketBall.instance.LastShotDistance = tempDist;

        // determine which side to dunk on
        if (bballRelativePositioning > 0 && !playerController.locked)
        {
            Launch(dunkPositionLeft);
        }
        if (bballRelativePositioning < 0 && !playerController.locked)
        {
            Launch(dunkPositionRight);
        }
    }

    public IEnumerator TriggerDunkSequence()
    {
        playerController.FreezePlayerPosition();
        playerController.playAnim("dunk");

        // wait for anim to start + finish
        yield return new WaitUntil(() => playerController.CurrentState == playerController.dunkState);
        yield return new WaitUntil(() => playerController.currentState != playerController.dunkState);

        BasketBall.instance.BasketBallState.Thrown = true;
        playerController.UnFreezePlayerPosition();

        // move ball above rim
        Vector3 temp = BasketBall.instance.BasketBallState.BasketBallTarget.transform.position;
        BasketBall.instance.Rigidbody.velocity = Vector3.zero;
        BasketBall.instance.transform.position = new Vector3(temp.x, temp.y, temp.z);
        //reset
        playerController.hasBasketball = false;
        playerController.setPlayerAnim("hasBasketball", false);
    }

    // =================================== Launch ball function =======================================
    void Launch(Vector3 Target)
    {
        playerController.locked = true;
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
            playerController.playAnim("inair_dunk");
        }

        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
        playerController.locked = false;
    }

    public float DunkRangeFeet { get => dunkRangeFeet; }
}
