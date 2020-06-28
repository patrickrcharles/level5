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
        

        // if collsion between hitbox and vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox") 
            && other.CompareTag("knock_down_attack") 
            && !playerState.KnockedDown)
        {
            //Debug.Log("this.tag : " + gameObject.tag + "  other.tag : " + other.name);

            if (playerCanBeKnockedDown)
            {
                playerKnockedDown(other.gameObject);
                VehicleController vehicleController = other.gameObject.transform.parent.GetComponent<VehicleController>();
                // if playerData object exists
                if (PlayerData.instance != null && playerCanBeKnockedDown)
                {
                    // add hit by car reference
                    PlayerData.instance.AddHitByCarInstanceToList(vehicleController.VehicleId);
                }
            }
            else
            {
                // avoid knockdown scenario
                playerAvoidKnockDown(other.gameObject);
            }
        }
        // if attack box hits vehicle or npc
        if ((gameObject.CompareTag("attack_box") && other.CompareTag("vehicle") )
            || (gameObject.CompareTag("attack_box") && other.CompareTag("auto_npc")) )
        {
            TestText.instance.setText(" collsion between "+ gameObject.name + "  and "+ other.gameObject.name);
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
