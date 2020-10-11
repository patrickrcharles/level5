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
            && !playerState.KnockedDown_Alternate)
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
            && (other.CompareTag("knock_down_attack") || other.CompareTag("enemyAttackBox") 
            && !playerState.KnockedDown))
        {
            //Debug.Log("this : " + gameObject.name + "  other : " + other.name);
            //Debug.Log("playerState.KnockedDown : " + playerState.KnockedDown );

            // player can be knocked down and other
            if (playerCanBeKnockedDown)
            {
                playerKnockedDown(other.gameObject);
            }
            else
            {
                // avoid knockdown scenario
                playerAvoidKnockDown(other.gameObject);
            }
        }
        if (gameObject.CompareTag("attack_box")
            && other.CompareTag("enemy"))
        {
            Debug.Log("******************************************************** Player attacks enemy");
            // turn off enemy sight for 2 seconds
            other.GetComponent<EnemyDetection>().TurnOffEnemySight(2);
        }
    }

    void playerKnockedDown_Alternate(GameObject playerKnockedDown)
    {
        playerState.KnockedDown_Alternate = true;
    }

    void playerKnockedDown(GameObject playerKnockedDown)
    {
        playerState.KnockedDown = true;
        playerState.hasBasketball = false;
        playerState.setPlayerAnim("hasBasketball", false);
    }
    void playerAvoidKnockDown(GameObject playerAvoidKnocked)
    {
        playerState.AvoidedKnockDown = true;
    }
}
