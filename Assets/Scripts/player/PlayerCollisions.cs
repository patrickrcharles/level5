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
        playerState = GameLevelManager.instance.PlayerState;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("playerHitbox")
            && other.CompareTag("knock_down_attack2")
            && !playerState.KnockedDown_Alternate1)
        {
            if (playerCanBeKnockedDown)
            {
                playerKnockedDown_Alternate(other.gameObject);
            }
            else
            {
                // avoid knockdown scenario
                playerAvoidKnockDown(other.gameObject);
            }
        }
        // if collsion between hitbox and vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox")
            && other.CompareTag("knock_down_attack")
            && !playerState.KnockedDown)
        {
            // player can be knocked down and other
            if (playerCanBeKnockedDown )
            {
                playerKnockedDown(other.gameObject);
                // if vehicle, update hit by vehicle stats
                if (other.transform.parent.parent.CompareTag("vehicle"))
                {
                    VehicleController vehicleController = other.gameObject.transform.parent.GetComponent<VehicleController>();
                    // if playerData object exists
                    if (PlayerData.instance != null && playerCanBeKnockedDown)
                    {
                        // add hit by car reference
                        PlayerData.instance.AddHitByCarInstanceToList(vehicleController.VehicleId);
                    }
                }
            }
            else
            {
                // avoid knockdown scenario
                playerAvoidKnockDown(other.gameObject);
            }
        }
    }

    void playerKnockedDown_Alternate(GameObject playerKnockedDown)
    {
        playerState.KnockedDown_Alternate1 = true;
    }

    void playerKnockedDown(GameObject playerKnockedDown)
    {
        playerState.KnockedDown = true;
    }
    void playerAvoidKnockDown(GameObject playerAvoidKnocked)
    {
        playerState.AvoidedKnockDown = true;
    }
}
