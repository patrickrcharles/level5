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
            && (other.CompareTag("playerAttackBox") || other.CompareTag("enemyAttackBox")) )
        {
            PlayerAttackBox playerAttackBox = null;
            EnemyAttackBox enemyAttackBox = null;

            if (other.GetComponent<PlayerAttackBox>() != null)
            {
                playerAttackBox = other.GetComponent<PlayerAttackBox>();
                enemyHealth.Health -= playerAttackBox.attackDamage;
            }
            if (other.GetComponent<EnemyAttackBox>() != null)
            {
                enemyAttackBox = other.GetComponent<EnemyAttackBox>();
                enemyHealth.Health -= (enemyAttackBox.attackDamage /2);
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
            else
            {
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
        //Debug.Log("enemyTakeDamage()");
        StartCoroutine(enemyController.takeDamage());
    }

    void enemyKnockedDown()
    {
        //Debug.Log("enemyKnockedDown()");
        StartCoroutine(enemyController.knockedDown());
    }
}
