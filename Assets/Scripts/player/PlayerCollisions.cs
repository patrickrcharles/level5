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
        playerState = GameLevelManager.Instance.PlayerState;
    }

    private void OnTriggerEnter(Collider other)
    {

        // if collsion between hitboc and vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox") && other.CompareTag("knock_down_attack") && !playerState.KnockedDown)
        {
            Debug.Log(" this : " + gameObject.tag + "       other : " + other.tag);
            if (playerCanBeKnockedDown) 
            {
                playerKnockedDown(other.gameObject);
                VehicleController vehicleController = other.gameObject.transform.parent.GetComponent<VehicleController>();
                if (PlayerData.instance != null)
                {
                    PlayerData.instance.AddHitByCarInstanceToList(vehicleController.VehicleId, GameOptions.playerDisplayName, GameOptions.levelDisplayName);
                }
            }
            else
            {
                // avoid knockdown scenario
                playerAvoidKnockDown(other.gameObject);
            }
        }
    }

    void playerKnockedDown( GameObject playerKnockedDown)
    {
        playerState.KnockedDown = true;
    }
    void playerAvoidKnockDown(GameObject playerAvoidKnocked)
    {
        playerState.AvoidedKnockDown = true;
    }
}
