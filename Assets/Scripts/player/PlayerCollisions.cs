using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField]
    PlayerController playerState;
    [SerializeField]
    bool playerCanBeKnockedDown;

    private void Start()
    {
        //Debug.Log(" player collsions : awake")
        //playerState = gameObject.transform.parent.GetComponent<PlayerController>();
        playerState = GameLevelManager.Instance.PlayerState;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(" this : " + gameObject.tag + "       other : " + other.tag);
        // if collsion between hitboc and vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox") && other.CompareTag("knock_down_attack") && !playerState.KnockedDown && playerCanBeKnockedDown)
        {
            playerKnockedDown(other.gameObject);
        }
    }

    void playerKnockedDown( GameObject playerKnockedDown)
    {
        playerState.KnockedDown = true;
        Debug.Log("KnockedDown = true;");
    }
}
