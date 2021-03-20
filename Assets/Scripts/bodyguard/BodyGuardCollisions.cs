﻿using UnityEngine;

public class BodyGuardCollisions : MonoBehaviour
{
    [SerializeField]
    BodyGuardController bodyGuardController;
    [SerializeField]
    BodyGuardHealthBar bodyGuardHealthBar;

    [SerializeField]
    BodyGuardHealth bodyGuardHealth;
    int maxEnemyHealth;

    private void Start()
    {
        bodyGuardController = gameObject.transform.root.GetComponent<BodyGuardController>();
        bodyGuardHealth = GetComponent<BodyGuardHealth>();
        bodyGuardHealthBar = transform.parent.GetComponentInChildren<BodyGuardHealthBar>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if this object is enemy hitbox and (player attack box or enemy attack box)
        if (gameObject.CompareTag("playerHitbox")
            && (other.CompareTag("playerAttackBox") || other.CompareTag("enemyAttackBox"))
            && bodyGuardHealth != null
            && bodyGuardHealthBar != null)
        {
            PlayerAttackBox playerAttackBox = null;
            EnemyAttackBox enemyAttackBox = null;

            if (other.GetComponent<PlayerAttackBox>() != null)
            {
                playerAttackBox = other.GetComponent<PlayerAttackBox>();
                bodyGuardHealth.Health -= playerAttackBox.attackDamage;
            }
            if (other.GetComponent<EnemyAttackBox>() != null
                && bodyGuardHealth != null)
            {
                enemyAttackBox = other.GetComponent<EnemyAttackBox>();
                bodyGuardHealth.Health -= (enemyAttackBox.attackDamage / 2);
            }
            //update health slider
            bodyGuardHealthBar.setHealthSliderValue();
            // check if enemy dead
            if (bodyGuardHealth.Health >= 0)
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
            if (bodyGuardHealth.Health <= 0 && !bodyGuardHealth.IsDead)
            {
                bodyGuardHealth.IsDead = true;
                // killed by player attack box and NOT enemy friendly fire
                if (playerAttackBox != null && bodyGuardHealth.IsDead)
                {
                    if (!GameOptions.EnemiesOnlyEnabled)
                    {
                        // if not enemies only game mode, player can receive health per kill
                        GameLevelManager.instance.PlayerHealth.Health += (bodyGuardHealth.MaxEnemyHealth / 10);
                        //Debug.Log("add to player health : " + (enemyHealth.MaxEnemyHealth / 10));
                    }
                    PlayerHealthBar.instance.setHealthSliderValue();
                    BasketBall.instance.GameStats.EnemiesKilled++;
                    if (BehaviorNpcCritical.instance != null)
                    {
                        BehaviorNpcCritical.instance.playAnimationCriticalSuccesful();
                    }
                }

                StartCoroutine(bodyGuardController.killEnemy());
            }
        }
    }

    private void enemyDisintegrated()
    {
        StartCoroutine(bodyGuardController.disintegrated());
    }

    void enemyTakeDamage()
    {
        //Debug.Log("enemyTakeDamage()");
        StartCoroutine(bodyGuardController.takeDamage());
    }

    void enemyStepOnRake(Collider other)
    {
        other.transform.parent.GetComponentInChildren<Animator>().Play("attack");
        StartCoroutine(bodyGuardController.takeDamage());
    }

    void enemyKnockedDown()
    {
        StartCoroutine(bodyGuardController.knockedDown());
    }
}
