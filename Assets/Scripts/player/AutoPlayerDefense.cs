using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.PlayerLoop;

public class AutoPlayerDefense : MonoBehaviour
{
    public PlayerIdentifier playerIdentifier;
    private CharacterProfile cpuCharacterProfile;

    [SerializeField] Vector3 playerPosition;
    [SerializeField] float playerRelativePositioning;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private Vector3 movement;
    [SerializeField] private float playerGuardingDistance;
    private Animator anim;
    private Rigidbody rigidBody;
    public float movementSpeed;
    GameObject dropShadow;

    public int blockedShots;

    private AnimatorStateInfo currentStateInfo;

    public int currentState;
    public int idleState;
    public int walkState;
    public int run;
    public int bWalk;
    public int bIdle;
    public int knockedDownState;
    public int takeDamageState;
    public int specialState;
    public int attackState;
    public int blockState;
    public int inAirDunkState;
    public int inAirHasBasketballFrontState;
    public int inAirHasBasketballSideState;
    public int inAirShootState;
    public int inAirShootFrontState;
    public int jumpState;
    public int inAirHasBasketball;
    public int disintegratedState;
    private float movementHorizontal;
    private float movementVertical;
    private bool FacingRight;
    public float distanceToTarget;
    public float playerDistanceToGoal;

    public bool arrivedAtTarget;
    public bool jumpTrigger;
    public bool grounded;
    public bool inAir;
    public bool isLocked;

    // Start is called before the first frame update
    void Start()
    {
        playerIdentifier = GameLevelManager.instance.players[0];
        cpuCharacterProfile = gameObject.GetComponent<CharacterProfile>();
        rigidBody = GetComponent<Rigidbody>();
        movementSpeed = cpuCharacterProfile.RunSpeed;
        anim = gameObject.GetComponentInChildren<Animator>();
        FacingRight = true;
        dropShadow = transform.Find("drop_shadow").gameObject;
    }
    //void FixedUpdate()
    //{
  
    //}
    // Update is called once per frame
    void Update()
    {
        currentStateInfo = anim.GetCurrentAnimatorStateInfo(0);
        currentState = currentStateInfo.fullPathHash;

        // drop shadow lock to bball transform on the ground
        dropShadow.transform.position = new Vector3(transform.root.position.x, Terrain.activeTerrain.SampleHeight(transform.position) + 0.02f, transform.root.position.z);
        distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        playerDistanceToGoal = Vector3.Distance(playerPosition, GameLevelManager.instance.BasketballRimVector);

        if(distanceToTarget < 0.05)
        {
            arrivedAtTarget = true;
        }
        else
        {
            arrivedAtTarget = false;
        }
        if (!arrivedAtTarget)
        {
            anim.SetBool("walking", true);
        }
        // not moving
        else
        {
            anim.SetBool("walking", false);
            anim.SetBool("moonwalking", false);
        }

        // player moving right, not facing right
        if (playerRelativePositioning > 0 && !FacingRight)//&& canMove)
        {
            Flip();
        }
        // player moving left, and facing right
        if (playerRelativePositioning < 0  && FacingRight)//&& canMove)
        {
            Flip();
        }
        if (playerIdentifier.playerController.currentState == playerIdentifier.playerController.inAirHasBasketball 
            && !inAir
            && !isLocked)
        {
            isLocked = true;
            jumpTrigger = true;
        }      
        if (jumpTrigger)
        {
            jumpTrigger = false;
            StartCoroutine( AutoPlayerJump(playerIdentifier));
        }
        if(inAir) { SetPlayerAnim("jump",true); }
        if(grounded) { SetPlayerAnim("jump",false); }

        playerRelativePositioning = playerIdentifier.player.transform.position.x - transform.position.x;
        playerPosition = playerIdentifier.player.transform.position;

        if (inAir)
        {
            movementSpeed = cpuCharacterProfile.InAirSpeed;
        }
        else
        {
            movementSpeed = cpuCharacterProfile.RunSpeed;
        }
        //if (!arrivedAtTarget) {
        moveToPosition(moveCpuPlayer());
        //}
    }


    public void SetPlayerAnim(string animationName, bool isTrue)
    {
        anim.SetBool(animationName, isTrue);
    }

    Vector3 moveCpuPlayer()
    {
        Vector3 directionOfTravel = (new Vector3(playerPosition.x, 0, playerPosition.z + playerGuardingDistance) - GameLevelManager.instance.BasketballRimVector).normalized;
        targetPosition = LerpByDistance(new Vector3(playerPosition.x,0,playerPosition.z), new Vector3(GameLevelManager.instance.BasketballRimVector.x, 0, GameLevelManager.instance.BasketballRimVector.z), playerGuardingDistance);
        
        return targetPosition;
    }

    public Vector3 LerpByDistance(Vector3 A, Vector3 B, float x)
    {
        Vector3 P = x * Vector3.Normalize(B - A) + A;

        return P;
    }

    public void moveToPosition(Vector3 target)
    {
        Vector3 targetPosition = new();
        targetPosition = (target - transform.position);
        
        movement = targetPosition * (movementSpeed * Time.fixedDeltaTime);
        rigidBody.MovePosition(transform.position + movement);
    }

    private void getAnimatorStateHashes()
    {
        idleState = Animator.StringToHash("base.idle");
        walkState = Animator.StringToHash("base.movement.walk");
        run = Animator.StringToHash("base.movement.run");
        bWalk = Animator.StringToHash("base.movement.basketball_dribbling");
        bIdle = Animator.StringToHash("base.movement.basketball_idle");
        knockedDownState = Animator.StringToHash("base.knockedDown");
        takeDamageState = Animator.StringToHash("base.takeDamage");
        specialState = Animator.StringToHash("base.special");
        attackState = Animator.StringToHash("base.attack.attack");
        blockState = Animator.StringToHash("base.attack.block");
        inAirDunkState = Animator.StringToHash("base.inair.inair_dunk");
        inAirHasBasketballFrontState = Animator.StringToHash("inair.inair_hasBasketball_front");
        inAirHasBasketballSideState = Animator.StringToHash("inair.inair_hasBasketball_side");
        inAirShootState = Animator.StringToHash("base.inair.basketball_shoot");
        inAirShootFrontState = Animator.StringToHash("base.inair.basketball_shoot_front");
        jumpState = Animator.StringToHash("base.inair.jump");
        inAirHasBasketball = Animator.StringToHash("base.inair.inair_hasBasketball");
        disintegratedState = Animator.StringToHash("base.disintegrated");
    }

    //private bool IsPlayerInAirState(PlayerIdentifier player)
    //{
    //    if (player.playerController.currentState == player.playerController.inAirDunkState
    //        || player.playerController.currentState == player.playerController.inAirHasBasketball
    //        || player.playerController.currentState == player.playerController.inAirHasBasketballFrontState
    //        || player.playerController.currentState == player.playerController.inAirHasBasketballSideState
    //        || player.playerController.currentState == player.playerController.inAirShootFrontState
    //        || player.playerController.currentState == player.playerController.inAirShootState)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }

    //}

    //void IsWalking(float horizontal, float vertical)
    //{
    //    Debug.Log(horizontal + ", " + vertical);
    //    // if moving
    //    //if (horizontal > 0f || horizontal < 0f || vertical > 0f || vertical < 0f)
    //    if (horizontal != 0 || vertical != 0f)
    //    {
    //        anim.SetBool("walking", true);
    //    }
    //    // not moving
    //    else
    //    {
    //        anim.SetBool("walking", false);
    //        anim.SetBool("moonwalking", false);
    //    }

    //    // player moving right, not facing right
    //    if (horizontal > 0 && !FacingRight)//&& canMove)
    //    {
    //        Flip();
    //    }
    //    // player moving left, and facing right
    //    if (horizontal < 0f && FacingRight)//&& canMove)
    //    {
    //        Flip();
    //    }
    //}

    void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;

        //if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled || GameOptions.sniperEnabled)
        //{
        //    Vector3 damageScale = damageDisplayObject.transform.localScale;
        //    damageScale.x *= -1;
        //    damageDisplayObject.transform.localScale = damageScale;
        //}
    }

     IEnumerator AutoPlayerJump(PlayerIdentifier player)
    {
        //Debug.Log("AutoPlayerJump");
        rigidBody.velocity = Vector3.up * cpuCharacterProfile.JumpForce; 
        yield return new WaitUntil(() => player.playerController.Grounded);
        isLocked = false;
    }
}
