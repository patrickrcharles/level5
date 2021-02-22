
using System.Collections;
using UnityEngine;
using Random = System.Random;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField]
    PlayerController playerState;
    [SerializeField]
    PlayerHealth playerHealth;
    [SerializeField]
    bool playerCanBeKnockedDown;
    bool locked = false;

    private void Start()
    {
        StartCoroutine(GetPlayerObjects());
    }

    IEnumerator GetPlayerObjects()
    {
        yield return new WaitUntil(() => GameLevelManager.instance.PlayerState != null);
        playerState = GameLevelManager.instance.PlayerState;
        playerHealth = GameLevelManager.instance.Player.GetComponentInChildren<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("playerHitbox")
            && playerState.inAir
            && playerState.currentState != playerState.dunkState
            && (other.name.Equals("dunk_position_left") || other.name.Equals("dunk_position_right")))
        {
            StartCoroutine(PlayerDunk.instance.TriggerDunkSequence());
        }
        // player sometimes gets stuck in inair dunk state
        if (gameObject.CompareTag("playerHitbox")
            && other.CompareTag("ground")
            && playerState.currentState == playerState.inAirDunkState)
        {
            playerState.setPlayerAnim("jump", false);
        }

        // if collsion between hitbox and vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox")
        && (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox"))
        && !playerState.KnockedDown
        && !playerState.TakeDamage
        && (GameOptions.enemiesEnabled || GameOptions.trafficEnabled ||other.transform.parent.name.Contains("rake"))
        // roll for evade attack chance
        && !rollForPlayerEvadeAttackChance(GameLevelManager.instance.PlayerShooterProfile.Luck)
        && !locked)
        {
            //bool critical = rollForCriticalShotChance(GameLevelManager.instance.PlayerShooterProfile.Luck);
            locked = true;
            EnemyAttackBox enemyAttackBox = null;
            if (other.GetComponent<EnemyAttackBox>() != null)
            {
                enemyAttackBox = other.GetComponent<EnemyAttackBox>();
            }
            // player is not blocking
            if (playerState.CurrentState != playerState.BlockState)
            {
                locked = true;
                // deduct from player health 
                playerHealth.Health -= enemyAttackBox.attackDamage;
                if (PlayerHealthBar.instance != null) // null check for health bar
                {
                    PlayerHealthBar.instance.setHealthSliderValue();
                    StartCoroutine(PlayerHealthBar.instance.DisplayDamageTakenValue(enemyAttackBox.attackDamage));
                }

                // player can be knocked down and other
                if (playerCanBeKnockedDown && enemyAttackBox.knockDownAttack)
                {
                    playerKnockedDown();
                }
                else
                {
                    playerTakeDamage();
                    // if stepped on rake
                    if (enemyAttackBox.transform.parent.name.Contains("rake"))
                    {
                        playerStepOnRake(other);
                    }
                }
            }
            // player is blocking
            if (playerState.CurrentState == playerState.BlockState)
            {
                // blocking play sound
                // block meter goes down
                SFXBB.instance.playSFX(SFXBB.instance.blocked);
                playerHealth.Block -= enemyAttackBox.attackDamage;
                PlayerHealthBar.instance.setBlockSliderValue();
                locked = false;
            }
            locked = false;
        }
    }

    // player has a chance to evade attack based on character profile's luck value
    bool rollForPlayerEvadeAttackChance(float maxPercent)
    {
        Random random = new Random();
        float percent = random.Next(1, 100);
        if (percent <= maxPercent)
        {
            StartCoroutine(PlayerHealthBar.instance.DisplayCustomMessageOnDamageDisplay("dodged"));
            return true;
        }

        return false;
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

    void playerStepOnRake(Collider other)
    {
        other.transform.parent.GetComponentInChildren<Animator>().Play("attack");
        playerState.TakeDamage = true;
        playerState.KnockedDown = false;
        playerState.hasBasketball = false;
        //StartCoroutine(playerState.PlayerFreezeForXSeconds(2f));             
        playerState.setPlayerAnim("hasBasketball", false);
    }
}
