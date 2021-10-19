﻿using Assets.Scripts.Utility;
using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    [SerializeField]
    EnemyController enemyController;
    [SerializeField]
    EnemyHealthBar enemyHealthBar;

    [SerializeField]
    EnemyHealth enemyHealth;
    int maxEnemyHealth;
    [SerializeField]
    int luck;

    private void Start()
    {
        enemyController = gameObject.transform.root.GetComponent<EnemyController>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealthBar = transform.parent.GetComponentInChildren<EnemyHealthBar>();
        if (luck == 0)
        {
            if (enemyController.IsBoss) { luck = 10; };
            if (enemyController.IsMinion) { luck = 5; };
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // if this object is enemy hitbox and (player attack box or enemy attack box)
        if (gameObject.CompareTag("enemyHitbox")
            && (other.CompareTag("playerAttackBox") || other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox"))
            && enemyHealth != null
            && enemyHealthBar != null)
        {
            PlayerAttackBox playerAttackBox = null;
            EnemyAttackBox enemyAttackBox = null; ;

            // check for enemy dodge
            bool enemyDodge = false;
            if (UtilityFunctions.rollForCriticalInt(luck))
            {
                enemyDodge = true;
                StartCoroutine(enemyHealthBar.DisplayCustomMessageOnDamageDisplay("dodged"));
            }
            if (!enemyDodge)
            {
                if (other.CompareTag("playerAttackBox"))
                {
                    playerAttackBox = other.GetComponent<PlayerAttackBox>();
                }
                if (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox"))
                {
                    enemyAttackBox = other.GetComponent<EnemyAttackBox>();
                }

                bool isRake = false;
                string damageDisplayMessage;
                // ------------------ player attacks enemy -----------------------
                // player attack. reduce health
                if (playerAttackBox != null
                    && !enemyController.stateKnockDown)
                {
                    damageDisplayMessage = "-" + playerAttackBox.attackDamage;
                    //if (UtilityFunctions.rollForCriticalInt(luck))
                    //{
                    //    enemyHealth.Health -= playerAttackBox.attackDamage * 2;
                    //    damageDisplayMessage = "2x damage -" + playerAttackBox.attackDamage;
                    //}
                    //else
                    //{
                    //    enemyHealth.Health -= playerAttackBox.attackDamage;
                    //}
                    enemyHealth.Health -= playerAttackBox.attackDamage;
                    StartCoroutine(enemyHealthBar.DisplayCustomMessageOnDamageDisplay(damageDisplayMessage));
                }
                // ------------------ enemy attacks enemy -----------------------
                // enemy attack. reduce damage to %50
                if (enemyAttackBox != null
                    && enemyHealth != null
                    && !enemyController.stateKnockDown)
                {
                    isRake = enemyAttackBox.isRake;
                    // if rake/obstacle 100% damage
                    if (isRake)
                    {
                        damageDisplayMessage = "-" + enemyAttackBox.attackDamage;
                        enemyHealth.Health -= enemyAttackBox.attackDamage;
                    }
                    // if enemy 50% damage
                    else
                    {
                        damageDisplayMessage = "-" + enemyAttackBox.attackDamage * 0.5f;
                        enemyHealth.Health -= enemyAttackBox.attackDamage / 2;
                    }
                    StartCoroutine(enemyHealthBar.DisplayCustomMessageOnDamageDisplay(damageDisplayMessage));
                }
                //update health slider
                enemyHealthBar.setHealthSliderValue();

                // check if enemy dead + enemy fails to roll critical to dodge
                if (enemyHealth.Health > 0)
                {
                    // player knock down attack
                    if (playerAttackBox != null
                        && playerAttackBox.knockDownAttack
                        && !playerAttackBox.disintegrateAttack)
                    {
                        enemyKnockedDown();
                    }
                    // if !knock down + is disintegrate
                    else if (playerAttackBox != null
                        && !playerAttackBox.knockDownAttack
                        && playerAttackBox.disintegrateAttack)
                    {
                        enemyDisintegrated();
                    }
                    // enemy attack / friendly fire /vehicle
                    else if (enemyAttackBox != null
                        && enemyAttackBox.knockDownAttack
                        && !enemyAttackBox.disintegrateAttack)
                    {
                        enemyKnockedDown();
                    }
                    // if !knock down + is disintegrate
                    else if (enemyAttackBox != null
                        && !enemyAttackBox.knockDownAttack
                        && enemyAttackBox.disintegrateAttack)
                    {
                        enemyDisintegrated();
                    }
                    else
                    {
                        if (!isRake)
                        {
                            enemyTakeDamage();
                        }
                        if (isRake)
                        {
                            enemyStepOnRake(other);
                        }
                    }
                }

                // else enemy is dead
                if (enemyHealth.Health <= 0 && !enemyHealth.IsDead)
                {
                    enemyHealth.IsDead = true;
                    // killed by player attack box and NOT enemy friendly fire
                    if (playerAttackBox != null && enemyHealth.IsDead)
                    {
                        if (GameLevelManager.instance.PlayerHealth.Health < GameLevelManager.instance.PlayerHealth.MaxHealth)
                        {
                            if (enemyController.IsBoss)
                            {
                                //Debug.Log("ADD HEALTH : 5");
                                GameLevelManager.instance.PlayerHealth.Health += 5;
                            }
                            if (enemyController.IsMinion)
                            {
                                GameLevelManager.instance.PlayerHealth.Health += 2;
                                //Debug.Log("ADD HEALTH : 2");
                            }
                            if (GameLevelManager.instance.PlayerHealth.Health > GameLevelManager.instance.PlayerHealth.MaxHealth)
                            {
                                GameLevelManager.instance.PlayerHealth.Health = GameLevelManager.instance.PlayerHealth.MaxHealth;
                            }
                        }
                        //if (!GameOptions.EnemiesOnlyEnabled)
                        //{
                        //    // if not enemies only game mode, player can receive health per kill
                        //    GameLevelManager.instance.PlayerHealth.Health += (enemyHealth.MaxEnemyHealth / 10);
                        //    //Debug.Log("add to player health : " + (enemyHealth.MaxEnemyHealth / 10));
                        //}
                        PlayerHealthBar.instance.setHealthSliderValue();
                        BasketBall.instance.GameStats.EnemiesKilled++;
                        if (enemyController.IsBoss)
                        {

                            BasketBall.instance.GameStats.BossKilled++;
                        }
                        else
                        {
                            BasketBall.instance.GameStats.MinionsKilled++;
                        }
                        if (BehaviorNpcCritical.instance != null)
                        {
                            BehaviorNpcCritical.instance.playAnimationCriticalSuccesful();
                        }
                    }
                    StartCoroutine(enemyController.killEnemy());
                }
            }
        }
    }

    private void enemyDisintegrated()
    {
        StartCoroutine(enemyController.disintegrated());
    }

    void enemyTakeDamage()
    {
        StartCoroutine(enemyController.takeDamage());
    }

    void enemyStepOnRake(Collider other)
    {
        other.transform.parent.GetComponentInChildren<Animator>().Play("attack");
        StartCoroutine(enemyController.takeDamage());
    }

    void enemyKnockedDown()
    {
        StartCoroutine(enemyController.knockedDown());
    }
}
