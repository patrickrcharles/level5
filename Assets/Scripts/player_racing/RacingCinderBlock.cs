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

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        target = RacingGameManager.instance.Player.transform.position;
        Debug.Log("RacingGameManager.instance.CharacterProfile.MaxSpeed : " + RacingGameManager.instance.CharacterProfile.MaxSpeed);
        maxSpeed = RacingGameManager.instance.CharacterProfile.MaxSpeed * 2f;
        movementSpeed = defaultMovementSpeed;

        InvokeRepeating("pursuePlayer", 0, 0.4f);
    }

    // Update is called once per frame
    void FixedUpdate()
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

    public void pursuePlayer()
    {
        target = (RacingGameManager.instance.Player.transform.position - transform.position).normalized;

        //transform.Translate(movement);

        //Debug.Log(gameObject.transform.root.name + " -- currentSpeed : " + currentSpeed);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && gameObject.CompareTag("obstacle"))
        {
            movementSpeed = defaultMovementSpeed;
        }
    }
}
