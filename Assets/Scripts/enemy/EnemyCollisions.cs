using System;
using System.Collections;
using System.Collections.Generic;
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
            && (other.CompareTag("playerAttackBox") || other.CompareTag("enemyAttackBox"))
            && enemyHealth != null
            && enemyHealthBar != null)
        {
            PlayerAttackBox playerAttackBox = null;
            EnemyAttackBox enemyAttackBox = null;

            Debug.Log(gameObject.transform.root.name + " attacked by " + other.transform.root.name);
            if (other.GetComponent<PlayerAttackBox>() != null)
            {
                playerAttackBox = other.GetComponent<PlayerAttackBox>();
                enemyHealth.Health -= playerAttackBox.attackDamage;
                Debug.Log("--------- took + " + playerAttackBox.attackDamage + " damage");
            }
            if (other.GetComponent<EnemyAttackBox>() != null
                && enemyHealth !=null )
            {
                enemyAttackBox = other.GetComponent<EnemyAttackBox>();
                enemyHealth.Health -= (enemyAttackBox.attackDamage /2);
                Debug.Log("--------- took + " + (enemyAttackBox.attackDamage / 2) + " damage");
            }
            //update health slider
            enemyHealthBar.setHealthSliderValue();
            // check if enemy dead
            if (enemyHealth.Health >= 0 )
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
                    enemyTakeDamage();
                    if (other.transform.parent.name.Contains("rake"))
                    {
                        enemyStepOnRake(other);
                    }
                }
            }
            // else enemy is dead
            if(enemyHealth.Health <= 0 && !enemyHealth.IsDead)
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
                    BasketBall.instance.BasketBallStats.EnemiesKilled++;
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
        //Debug.Log("enemyTakeDamage()");
        StartCoroutine( enemyController.takeDamage());
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
