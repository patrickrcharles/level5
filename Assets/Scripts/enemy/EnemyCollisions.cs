﻿using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    [SerializeField]
    EnemyController enemyController;
    [SerializeField]
    EnemyHealthBar enemyHealthBar;

    [SerializeField]
    EnemyHealth enemyHealth;
    int maxEnemyHealth;

    private void Start()
    {
        enemyController = gameObject.transform.root.GetComponent<EnemyController>();
        enemyHealth = GetComponent<EnemyHealth>();
        enemyHealthBar = transform.parent.GetComponentInChildren<EnemyHealthBar>();
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

            if (other.CompareTag("playerAttackBox"))
            {
                playerAttackBox = other.GetComponent<PlayerAttackBox>();
            }
            if (other.CompareTag("enemyAttackBox") || other.CompareTag("obstacleAttackBox"))
            {
                enemyAttackBox = other.GetComponent<EnemyAttackBox>();
            }

            bool isRake = false;

            if (playerAttackBox != null
                && !enemyController.stateKnockDown)
            {
                enemyHealth.Health -= playerAttackBox.attackDamage;
            }
            if (enemyAttackBox != null
                && enemyHealth != null
                && !enemyController.stateKnockDown)
            {
                isRake = enemyAttackBox.isRake;
                //enemyHealth.Health -= (enemyAttackBox.attackDamage / 2);
                enemyHealth.Health -= enemyAttackBox.attackDamage;
            }
            //update health slider
            enemyHealthBar.setHealthSliderValue();
            // check if enemy dead
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
                    if (!GameOptions.EnemiesOnlyEnabled)
                    {
                        // if not enemies only game mode, player can receive health per kill
                        GameLevelManager.instance.PlayerHealth.Health += (enemyHealth.MaxEnemyHealth / 10);
                        //Debug.Log("add to player health : " + (enemyHealth.MaxEnemyHealth / 10));
                    }
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
