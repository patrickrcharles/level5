
using System;
using System.Collections;
using UnityEngine;
using Level5.Core.Match;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField]
    PlayerIdentifier playerIdentifier;
    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    AutoPlayerController autoPlayerController;
    [SerializeField]
    PlayerHealth playerHealth;
    [SerializeField]
    bool playerCanBeKnockedDown;
    bool locked = false;

    // AUD-012 Phase 2b: match rules, the fall-respawn destination and the GameRules.killedOnIdle
    // forwarding callback, composed by SpawnCoordinator.BindPlayerCollisionsContext instead of this
    // component reading MatchRuntime.Rules / GameLevelManager.instance / GameRules.instance directly.
    private ResolvedMatchRules matchRules;
    private Transform fallRespawnDestination;
    private Action markKilledOnIdle;

    /// <summary>
    /// Binds the rules this match is being played under. Bind-once, the same shape
    /// <c>CallBallToPlayer</c>/<c>PlayerHealth</c> already use.
    /// </summary>
    public void BindMatchRules(ResolvedMatchRules rules)
    {
        if (matchRules != null)
        {
            Debug.LogError($"PlayerCollisions on '{gameObject.name}' already has bound match rules; ignoring a second BindMatchRules call.", this);
            return;
        }

        if (rules == null)
        {
            Debug.LogError($"PlayerCollisions on '{gameObject.name}' was bound with null match rules; remaining unbound.", this);
            return;
        }

        matchRules = rules;
    }

    /// <summary>
    /// Explicit binding of the human spawn point, from <c>SpawnCoordinator.BindPlayerCollisionsContext</c> -
    /// replaces this component's former direct <c>GameLevelManager.instance.PlayerSpawnLocation</c> read.
    /// </summary>
    public void BindFallRespawnDestination(Transform destination)
    {
        fallRespawnDestination = destination;
    }

    /// <summary>
    /// Explicit binding of the <c>GameRules.killedOnIdle</c> forwarding callback, from
    /// <c>SpawnCoordinator.BindPlayerCollisionsContext</c> - replaces this component's former direct
    /// <c>GameRules.instance.killedOnIdle = true</c> write.
    /// </summary>
    public void BindKilledOnIdleCallback(Action callback)
    {
        markKilledOnIdle = callback;
    }

    private void Start()
    {
        GetPlayerObjects();
    }

    private void GetPlayerObjects()
    {
        playerIdentifier = GetComponentInParent<PlayerIdentifier>();
        if (playerIdentifier.isCpu)
        {
            autoPlayerController = playerIdentifier.autoPlayer.GetComponent<AutoPlayerController>();
        }
        else
        {
            playerController = playerIdentifier.player.GetComponent<PlayerController>();
        }
        
        playerHealth = playerIdentifier.isCpu
            ? playerIdentifier.autoPlayer.GetComponentInChildren<PlayerHealth>() 
            : playerIdentifier.player.GetComponentInChildren<PlayerHealth>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // check for fall respawner
        if (gameObject.CompareTag("playerHitbox") && other.CompareTag("fallRespawner"))
        {
            if (fallRespawnDestination != null)
            {
                playerIdentifier.transform.position = fallRespawnDestination.position;
            }
        }

        if (matchRules != null
            && gameObject.CompareTag("playerHitbox")
            && !matchRules.IsBattleRoyal
            && !matchRules.IsCageMatch
            && !matchRules.EnemiesOnly
            && playerController.InAir
            && playerController.currentState != playerController.dunkState
            && (other.name.Equals("dunk_position_left") || other.name.Equals("dunk_position_right")))
        {
            StartCoroutine(playerController.PlayerDunk.TriggerDunkSequence());
        }
        // player sometimes gets stuck in inair dunk state
        if (gameObject.CompareTag("playerHitbox")
            && other.CompareTag("ground")
            && playerController.currentState == playerController.inAirDunkState)
        {
            playerController.SetPlayerAnim("jump", false);
        }

        // if collsion between hitbox, vehicle, knocked down
        if (matchRules != null
        && gameObject.CompareTag("playerHitbox")
        && (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox") || other.CompareTag("playerAttackBox"))
        && !playerController.KnockedDown
        && !playerController.TakeDamage
        && (matchRules.EnemiesEnabled
        || matchRules.TrafficEnabled
        || matchRules.ObstaclesEnabled
        || other.transform.root.name.Contains("snake")
        || matchRules.SniperEnabled
        || matchRules.Sniper == SniperMode.Bullet
        || matchRules.Sniper == SniperMode.Laser
        || other.transform.root.name.Contains("projectile_bullet_instantkill_enemy"))
        // roll for evade attack chance
        && !rollForPlayerEvadeAttackChance(playerController.CharacterProfile.Luck)
        && !locked)
        {
            locked = true;
            IAttackBoxHitInfo enemyAttackBoxHit = null;
            IAttackBoxHitInfo playerAttackBoxHit = null;
            int damage = 0;
            bool isKnockdown = false;
            bool isRake = false;
            bool isDisintegrate = false;
            // get attack box player/enemy
            if (other.CompareTag("playerAttackBox"))
            {
                playerAttackBoxHit = other.GetComponent<IAttackBoxHitInfo>();
            }
            if (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox"))
            {
                enemyAttackBoxHit = other.GetComponent<IAttackBoxHitInfo>();
            }
            // check if player attack
            if (enemyAttackBoxHit != null)
            {
                isRake = enemyAttackBoxHit.IsRake;
                damage = enemyAttackBoxHit.AttackDamage;
                isKnockdown = enemyAttackBoxHit.KnockDownAttack;
                isDisintegrate = enemyAttackBoxHit.DisintegrateAttack;
                if (enemyAttackBoxHit.IsKilledOnIdle)
                {
                    markKilledOnIdle?.Invoke();
                }
                if (isDisintegrate)
                {
                    locked = true;
                    playerDisintegrated();
                }

            }
            //check if enemy attack
            if (playerAttackBoxHit != null)
            {
                damage = playerAttackBoxHit.AttackDamage;
                isKnockdown = playerAttackBoxHit.KnockDownAttack;
                isDisintegrate = playerAttackBoxHit.DisintegrateAttack;
                if (isDisintegrate)
                {
                    locked = true;
                    playerDisintegrated();
                }
            }

            // player is not blocking
            if (playerController.CurrentState != playerController.BlockState && !isDisintegrate)
            {
                locked = true;
                playerHealth.TakeDamage(damage);
                if (PlayerHealthBar.instance != null && PlayerHealthBar.instance.IsTracking(playerHealth))
                {
                    StartCoroutine(PlayerHealthBar.instance.DisplayDamageTakenValue(damage));
                }

                if (playerHealth.IsDead)
                {
                    locked = false;
                    return;
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
                        Debug.Log("stepped on rake");
                        playerStepOnRake(other);
                    }
                }
            }
            // player is blocking
            if (playerController.CurrentState == playerController.BlockState)
            {
                // blocking play sound
                // block meter goes down
                SFXBB.instance.playSFX(SFXBB.instance.blocked);
                if (enemyAttackBoxHit != null)
                {
                    playerHealth.SpendBlock(enemyAttackBoxHit.AttackDamage);
                }
                locked = false;
            }
            locked = false;
        }
    }

    // player has a chance to evade attack based on character profile's luck value
    bool rollForPlayerEvadeAttackChance(float maxPercent)
    {
        float percent = UnityEngine.Random.Range(0f, 100f);
        if (percent < maxPercent)
        {
            if (PlayerHealthBar.instance != null)
            {
                StartCoroutine(PlayerHealthBar.instance.DisplayCustomMessageOnDamageDisplay("dodged"));
            }
            return true;
        }

        return false;
    }

    void playerDisintegrated()
    {
        playerController.TakeDamage = false;
        playerController.KnockedDown = false;
        playerController.hasBasketball = false;
        playerController.Disintegrated = true;
        playerController.SetPlayerAnim("hasBasketball", false);
    }

    void playerTakeDamage()
    {
        playerController.TakeDamage = true;
        playerController.KnockedDown = false;
        playerController.hasBasketball = false;
        playerController.SetPlayerAnim("hasBasketball", false);
    }
    void playerKnockedDown()
    {
        playerController.TakeDamage = false;
        playerController.KnockedDown = true;
        playerController.hasBasketball = false;
        playerController.SetPlayerAnim("hasBasketball", false);
    }

    void playerStepOnRake(Collider other)
    {
        other.transform.parent.GetComponentInChildren<Animator>().Play("attack");
        playerController.TakeDamage = true;
        playerController.KnockedDown = false;
        playerController.hasBasketball = false;
        //StartCoroutine(playerState.PlayerFreezeForXSeconds(2f));             
        playerController.SetPlayerAnim("hasBasketball", false);
    }
}
