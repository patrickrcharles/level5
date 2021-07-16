using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RacingCinderBlock : MonoBehaviour
{
    [SerializeField]
    Vector3 target;
    [SerializeField]
    Vector3 movement;
    [SerializeField]
    float acceleration;
    [SerializeField]
    float maxSpeed;
    [SerializeField]
    float movementSpeed;
    [SerializeField]
    float defaultMovementSpeed;
    [SerializeField]
    Rigidbody rigidbody;
    [SerializeField]
    bool targetReached;
    [SerializeField]
    bool lockSwitch = false;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        target = RacingGameManager.instance.Player.transform.position;
        //Debug.Log("RacingGameManager.instance.CharacterProfile.MaxSpeed : " + RacingGameManager.instance.CharacterProfile.MaxSpeed);
        maxSpeed = RacingGameManager.instance.CharacterProfile.MaxSpeed * 2f;
        movementSpeed = defaultMovementSpeed;

        //InvokeRepeating("pursuePlayer", 0, 0.4f);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        pursuePlayer();

        if (!targetReached)
        {
            if (movementSpeed < maxSpeed)
            {
                //movementSpeed = 1 + (acceleration / 100);
                movementSpeed = (movementSpeed + (acceleration / 100));
            }

            //pursuePlayer();
            if (!RacingGameManager.instance.PlayerController.KnockedDown)
            {
                movement = target * (movementSpeed * Time.fixedDeltaTime);
                //movement = targetPosition * (movementSpeed * Time.deltaTime);
                rigidbody.MovePosition(transform.position + movement);
            }
            else
            {
                movementSpeed = 0;
            }
        }
        if (targetReached && !lockSwitch)
        {
            //transform.position = new Vector3(RacingGameManager.instance.Player.transform.position.x,
            //    RacingGameManager.instance.Player.transform.position.y + 2,
            //    RacingGameManager.instance.Player.transform.position.z);
            lockSwitch = true;
            Vector3 force = new Vector3(0, -20, 0);
            rigidbody.AddForce(force, ForceMode.VelocityChange);
        }
    }

    public void pursuePlayer()
    {
        if (!targetReached)
        {
            Vector3 newVector = new Vector3(RacingGameManager.instance.Player.transform.position.x+1,
                RacingGameManager.instance.Player.transform.position.y + 2.5f,
                RacingGameManager.instance.Player.transform.position.z);
            target = (newVector - transform.position).normalized;
            if((newVector.x - transform.position.x) < 1)
            {
                targetReached = true;
            }
        }
        //if()
        //if (targetReached)
        //{
        //    //Vector3 newVector = new Vector3(RacingGameManager.instance.Player.transform.position.x,
        //    //    RacingGameManager.instance.Player.transform.position.y + 2,
        //    //    RacingGameManager.instance.Player.transform.position.z);
        //    //transform.position = new Vector3(RacingGameManager.instance.Player.transform.position.x,
        //    //    RacingGameManager.instance.Player.transform.position.y + 2,
        //    //    RacingGameManager.instance.Player.transform.position.z);
        //    Vector3 force = new Vector3(0, -20, 0);
        //    rigidbody.AddForce(force, ForceMode.VelocityChange);
        //    //target = (newVector).normalized;
        //    //Destroy(gameObject, 1);
        //}

        //transform.Translate(movement);

        //Debug.Log(gameObject.transform.root.name + " -- currentSpeed : " + currentSpeed);

    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.CompareTag("Player") && gameObject.CompareTag("obstacle") && !targetReached)
        //{
        //    targetReached = true;
        //    Debug.Log("player tag");
        //    //movementSpeed = defaultMovementSpeed;
        //}
        if ((other.CompareTag("ground") || other.CompareTag("playerHitbox")) && lockSwitch)
        {
            //lockSwitch = false;
            rigidbody.detectCollisions = false;
            Debug.Log("ground/playerHitbox tag");
            //targetReached = true;
            Vector3 instantiateVector = new Vector3(RacingGameManager.instance.Player.transform.position.x-35,
                RacingGameManager.instance.Player.transform.position.y + 2,
                RacingGameManager.instance.Player.transform.position.z);
            Instantiate(RacingGameManager.instance.CinderBlockPrefab, instantiateVector, Quaternion.identity);

            Destroy(this.gameObject);
            //movementSpeed = defaultMovementSpeed;
        }
    }
}
