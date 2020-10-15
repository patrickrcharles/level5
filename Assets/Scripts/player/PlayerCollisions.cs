using System;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField]
    PlayerController playerState;
    [SerializeField]
    bool playerCanBeKnockedDown;
    bool locked = false;

    private void Start()
    {
        playerState = GameLevelManager.instance.PlayerState;
    }

    private void OnTriggerEnter(Collider other)
    {

        // if collsion between hitbox and vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox")
            && (other.CompareTag("knock_down_attack") || other.CompareTag("enemyAttackBox")) 
            && !playerState.KnockedDown
            && !playerState.TakeDamage
            && !locked)
        {
            if (playerState.CurrentState != playerState.BlockState)
            {
                //Debug.Log("this : " + gameObject.name + "  other : " + other.name);
                Debug.Log("     enemy attacked player");
                //Debug.Log("playerState.KnockedDown : " + playerState.KnockedDown );
                locked = true;
                // player can be knocked down and other
                if (playerCanBeKnockedDown && other.GetComponent<EnemyAttackBox>().knockDownAttack)
                {
                    //Debug.Log("     enemy used knockdown attack");
                    playerKnockedDown();
                }
                else
                {
                    // avoid knockdown scenario
                    //playerAvoidKnockDown(other.gameObject);
                    playerTakeDamage();
                }
                locked = false;
            }
            else
            {
                // blocking play sound
                // block meter goes down
                SFXBB.instance.playSFX(SFXBB.instance.blocked);
            }
        }
        if (gameObject.CompareTag("playerAttackBox")
            && other.CompareTag("enemyHitbox"))
        {
            // turn off enemy sight for 2 seconds
            other.transform.parent.GetComponent<EnemyDetection>().TurnOffEnemySight(2);
        }
    }

    void playerTakeDamage()
    {
        Debug.Log("player take damage");
        playerState.TakeDamage = true;
        playerState.KnockedDown = false;
        playerState.hasBasketball = false;
        playerState.setPlayerAnim("hasBasketball", false);
    }

    void playerKnockedDown()
    {
        Debug.Log("     playerCollsions ::  playerKnockedDown()");
        playerState.TakeDamage = false;
        playerState.KnockedDown = true;
        playerState.hasBasketball = false;
        playerState.setPlayerAnim("hasBasketball", false);
    }
    void playerAvoidKnockDown(GameObject playerAvoidKnocked)
    {
        playerState.AvoidedKnockDown = true;
    }
}
