
using System.Collections;
using UnityEngine;
using Level5.Core.Match;

public class AutoPlayerCollisions : MonoBehaviour
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

    // AUD-012 Phase 2b: match rules and the fall-respawn destination, composed by
    // SpawnCoordinator.BindAutoPlayerCollisionsContext instead of this component reading
    // MatchRuntime.Rules / GameLevelManager.instance directly.
    private ResolvedMatchRules matchRules;
    private Transform fallRespawnDestination;

    /// <summary>
    /// Binds the rules this match is being played under. Bind-once, the same shape
    /// <c>PlayerCollisions.BindMatchRules</c> uses.
    /// </summary>
    public void BindMatchRules(ResolvedMatchRules rules)
    {
        if (matchRules != null)
        {
            Debug.LogError($"AutoPlayerCollisions on '{gameObject.name}' already has bound match rules; ignoring a second BindMatchRules call.", this);
            return;
        }

        if (rules == null)
        {
            Debug.LogError($"AutoPlayerCollisions on '{gameObject.name}' was bound with null match rules; remaining unbound.", this);
            return;
        }

        matchRules = rules;
    }

    /// <summary>
    /// Explicit binding of the human spawn point, from
    /// <c>SpawnCoordinator.BindAutoPlayerCollisionsContext</c> - replaces this component's former
    /// direct <c>GameLevelManager.instance.PlayerSpawnLocation</c> read.
    /// </summary>
    public void BindFallRespawnDestination(Transform destination)
    {
        fallRespawnDestination = destination;
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

        //if (gameObject.CompareTag("playerHitbox")
        //    && (!MatchRuntime.Rules.IsBattleRoyal || !MatchRuntime.Rules.IsCageMatch || !MatchRuntime.Rules.EnemiesOnly)
        //    && autoPlayerController.InAir
        //    && autoPlayerController.currentState != autoPlayerController.dunkState
        //    && (other.name.Equals("dunk_position_left") || other.name.Equals("dunk_position_right")))
        //{
        //    StartCoroutine(GameLevelManager.instance.autoPlayerController1.PlayerDunk.TriggerDunkSequence());
        //}

        // player sometimes gets stuck in inair dunk state
        //if (gameObject.CompareTag("autoPlayerHitbox")
        //    && other.CompareTag("ground")
        //    && autoPlayerController.currentState == autoPlayerController.inAirDunkState)
        //{
        //    autoPlayerController.SetPlayerAnim("jump", false);
        //}

        // if collsion between hitbox, vehicle, knocked down
        if (matchRules != null
        && gameObject.CompareTag("autoPlayerHitbox")
        && (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox") || other.CompareTag("playerAttackBox"))
        && !autoPlayerController.KnockedDown
        && !autoPlayerController.TakeDamage
        && (matchRules.EnemiesEnabled
        || matchRules.TrafficEnabled
        || matchRules.ObstaclesEnabled
        || other.transform.root.name.Contains("snake")
        || matchRules.SniperEnabled)
        // roll for evade attack chance
        && !rollForPlayerEvadeAttackChance(autoPlayerController.CharacterProfile.Luck)
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
            if (autoPlayerController.currentState != autoPlayerController.blockState && !isDisintegrate)
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
            if (autoPlayerController.currentState == autoPlayerController.blockState)
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
        autoPlayerController.TakeDamage = false;
        autoPlayerController.KnockedDown = false;
        autoPlayerController.hasBasketball = false;
        autoPlayerController.Disintegrated = true;
        autoPlayerController.SetPlayerAnim("hasBasketball", false);
    }

    void playerTakeDamage()
    {
        autoPlayerController.TakeDamage = true;
        autoPlayerController.KnockedDown = false;
        autoPlayerController.hasBasketball = false;
        autoPlayerController.SetPlayerAnim("hasBasketball", false);
    }
    void playerKnockedDown()
    {
        autoPlayerController.TakeDamage = false;
        autoPlayerController.KnockedDown = true;
        autoPlayerController.hasBasketball = false;
        autoPlayerController.SetPlayerAnim("hasBasketball", false);
    }

    void playerStepOnRake(Collider other)
    {
        other.transform.parent.GetComponentInChildren<Animator>().Play("attack");
        autoPlayerController.TakeDamage = true;
        autoPlayerController.KnockedDown = false;
        autoPlayerController.hasBasketball = false;
        //StartCoroutine(playerState.PlayerFreezeForXSeconds(2f));             
        autoPlayerController.SetPlayerAnim("hasBasketball", false);
    }
}
