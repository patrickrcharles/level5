﻿
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
        yield return new WaitUntil(() => GameLevelManager.instance.PlayerController != null);
        playerState = GameLevelManager.instance.PlayerController;
        playerHealth = GameLevelManager.instance.Player.GetComponentInChildren<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // check for fall respawner
        if (gameObject.CompareTag("playerHitbox") && other.CompareTag("fallRespawner"))
        {
            GameLevelManager.instance.Player.transform.position 
                = GameLevelManager.instance.PlayerSpawnLocation.transform.position;
                //= new Vector3(GameLevelManager.instance.PlayerSpawnLocation.transform.position.x,
                //GameLevelManager.instance.PlayerSpawnLocation.transform.position.y+ 5,
                //GameLevelManager.instance.PlayerSpawnLocation.transform.position.z);
        }

        if (gameObject.CompareTag("playerHitbox")
            && playerState.InAir
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
            playerState.SetPlayerAnim("jump", false);
        }

        // if collsion between hitbox, vehicle, knocked down
        if (gameObject.CompareTag("playerHitbox")
        && (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox") || other.CompareTag("playerAttackBox"))
        && !playerState.KnockedDown
        && !playerState.TakeDamage
        && (GameOptions.enemiesEnabled
        || GameOptions.trafficEnabled
        || other.transform.parent.name.Contains("rake")
        || other.transform.root.name.Contains("snake")
        || GameOptions.sniperEnabled)
        // roll for evade attack chance
        && !rollForPlayerEvadeAttackChance(GameLevelManager.instance.CharacterProfile.Luck)
        && !locked)
        {

            locked = true;
            EnemyAttackBox enemyAttackBox = null;
            PlayerAttackBox playerAttackBox = null;
            int damage = 0;
            bool isKnockdown = false;
            bool isRake = false;

            if (other.GetComponent<EnemyAttackBox>() != null)
            {
                enemyAttackBox = other.GetComponent<EnemyAttackBox>();
                damage = enemyAttackBox.attackDamage;
                isKnockdown = enemyAttackBox.knockDownAttack;
                if (enemyAttackBox.transform.parent.name.Contains("rake"))
                {
                    isRake = true;
                }
            }
            if (other.GetComponent<PlayerAttackBox>() != null)
            {
                playerAttackBox = other.GetComponent<PlayerAttackBox>();
                damage = playerAttackBox.attackDamage;
                isKnockdown = playerAttackBox.knockDownAttack;
                if (playerAttackBox.transform.parent.name.Contains("rake"))
                {
                    isRake = true;
                }
            }

            // player is not blocking
            if (playerState.CurrentState != playerState.BlockState)
            {
                locked = true;
                // deduct from player health 
                playerHealth.Health -= damage;
                // null check
                if (PlayerHealthBar.instance != null) // null check for health bar
                {
                    PlayerHealthBar.instance.setHealthSliderValue();
                    StartCoroutine(PlayerHealthBar.instance.DisplayDamageTakenValue(damage));
                }

                // player can be knocked down and other
                if (playerCanBeKnockedDown && isKnockdown)
                {
                    playerKnockedDown();
                }
                else
                {
                    playerTakeDamage();
                    // if stepped on rake
                    if (isRake)
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
        playerState.SetPlayerAnim("hasBasketball", false);
    }
    void playerKnockedDown()
    {
        playerState.TakeDamage = false;
        playerState.KnockedDown = true;
        playerState.hasBasketball = false;
        playerState.SetPlayerAnim("hasBasketball", false);
    }

    void playerStepOnRake(Collider other)
    {
        other.transform.parent.GetComponentInChildren<Animator>().Play("attack");
        playerState.TakeDamage = true;
        playerState.KnockedDown = false;
        playerState.hasBasketball = false;
        //StartCoroutine(playerState.PlayerFreezeForXSeconds(2f));             
        playerState.SetPlayerAnim("hasBasketball", false);
    }
}
