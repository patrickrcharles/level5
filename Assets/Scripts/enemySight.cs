using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemySight : MonoBehaviour
{

    // if enemy's target is a player. if (false) can change enemy's target manually to move to a desitnation
    bool targetisPlayer;
    GameObject player;
    playercontrollerscript playerState;
    [SerializeField]
    Vector3 playerRelativePosition;
    public GameObject target, requestedTarget, queuedTarget;
    public GameObject frontTarget, backTarget, groundTarget;

    public bool hasAssignedTarget; // temp for testing. convert to get/set
    [SerializeField]
    float queueDistance;
    [SerializeField]
    float frontTargetDistance;
    [SerializeField]
    float backTargetDistance;
    // Use this for initialization

    [SerializeField]
    private bool _facingRight;
    public bool facingRight
    {
        get { return _facingRight; }
        set { _facingRight = value; }
    }
    [SerializeField]

    void Start()
    {

        player = gameManager.instance.player;
        playerState = gameManager.instance.playerState;
        facingRight = true;

        targetisPlayer = true;
        target = player;
        //lineOfFireRange = levelManager.instance.lineOfFireRange;

    }

    // Update is called once per frame
    void Update()
    {
        ////Debug.Log(" playerinsight : " + playerInSight);

        //player positon - enemy position
        if (targetisPlayer) // if enemy  target = player.  target could be prescripted position to go to
        {
            playerRelativePosition = player.transform.position - gameObject.transform.position;
        }
        else // if enemy  target IS NOT player. aka can manually move enemy to predetrmined destination
             // just set target manually to a gameobject and switch off "targetisPlayer"
        {
            playerRelativePosition = target.transform.position - gameObject.transform.position;
        }

        // find player's positioning relative to enemy
        if (playerRelativePosition.x < 0)
        {
            ////Debug.Log("enemySight.cs :: if (playerRelativePosition.x < 0)");
            playerOnRight = false;
            if (facingRight)
            {
                //Debug.Log("enemySight.cs :: if (facingRight)");
                facingPlayer = false;
            }
            if(!facingRight)
            {
                //Debug.Log("enemySight.cs :: else");
                facingPlayer = true;
            }
        }

        if (playerRelativePosition.x > 0)
        {
            playerOnRight = true;
            if (facingRight)
            {
                //Debug.Log("enemySight.cs :: if (enemyWalk.facingRight)");
                facingPlayer = true;
            }
            else
            {
                //Debug.Log("enemySight.cs :: else");
                facingPlayer = false;
            }
        }

        // ------------- make sure Enemy is facing player
        if (playerOnRight && !facingRight)
        {
            //Debug.Log("if (enemySight.playerOnRight && !facingRight)");
            //flip enemy sprite and facing player
            Flip();
        }
        //if player on left of enemy and enemy facing right
        if (!playerOnRight && facingRight)
        {
            //Debug.Log("if (!enemySight.playerOnRight && facingRight)");
            //flip enemy sprite and facing player
            Flip();
        }

    }
    

    // flip sprites
    public void Flip()
    {
        //Debug.Log("enemysight.cs ::: public void Flip()");
        facingRight = !facingRight;
        Vector3 thisScale = transform.localScale;
        thisScale.x *= -1;
        transform.localScale = thisScale;
        //facingPlayer = true;
    }


    int generateRandomInt(int min, int max)
    {
        int random = Random.Range(min, max);
        return random;
    }
    [SerializeField]
    private bool _playerInSight;
    public bool playerInSight
    {
        get { return _playerInSight; }
        set { _playerInSight = value; }
    }
    [SerializeField]
    private bool _facingPlayer;
    public bool facingPlayer
    {
        get { return _facingPlayer; }
        set { _facingPlayer = value; }
    }
    private float _targetDistance;
    public float targetDistance
    {
        get { return _targetDistance; }
        set { _targetDistance = value; }
    }
    private float _playerLineOfFire;
    public float playerLineOfFire
    {
        get { return _playerLineOfFire; }
        set { _playerLineOfFire = value; }
    }
    private float _lineOfFireRange;
    public float lineOfFireRange
    {
        get { return _lineOfFireRange; }
        set { _lineOfFireRange = value; }
    }
    [SerializeField]
    private bool _playerOnRight;
    public bool playerOnRight
    {
        get { return _playerOnRight; }
        set { _playerOnRight = value; }
    }
    [SerializeField]
    private bool _attackingFrontTarget;
    public bool attackingFrontTarget
    {
        get { return _attackingFrontTarget; }
        set { _attackingFrontTarget = value; }
    }
    [SerializeField]
    private bool _attackingBackTarget;
    public bool attackingBackTarget
    {
        get { return _attackingBackTarget; }
        set { _attackingBackTarget = value; }
    }

}


