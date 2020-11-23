
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
            && other.CompareTag("enemyAttackBox")
            && !playerState.KnockedDown
            && !playerState.TakeDamage
            && !locked)
        {
            locked = true;
            // player is not blocking
            if (playerState.CurrentState != playerState.BlockState)
            {
                locked = true;
                // player can be knocked down and other
                if (playerCanBeKnockedDown && other.GetComponent<EnemyAttackBox>().knockDownAttack)
                {
                    playerKnockedDown();
                }
                else
                {
                    playerTakeDamage();
                }
            }
            // player is blocking
            if (playerState.CurrentState == playerState.BlockState)
            {
                // blocking play sound
                // block meter goes down
                SFXBB.instance.playSFX(SFXBB.instance.blocked);
                locked = false;
            }
            locked = false;
        }
    }

    void playerTakeDamage()
    {
        playerState.TakeDamage = true;
        playerState.KnockedDown = false;
        playerState.hasBasketball = false;
        playerState.setPlayerAnim("hasBasketball", false);
    }
    void playerKnockedDown()
    {
        playerState.TakeDamage = false;
        playerState.KnockedDown = true;
        playerState.hasBasketball = false;
        playerState.setPlayerAnim("hasBasketball", false);
    }
}
