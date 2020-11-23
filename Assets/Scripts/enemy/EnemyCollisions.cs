using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCollisions : MonoBehaviour
{
    [SerializeField]
    EnemyController enemyController;

    private void Start()
    {
        enemyController = gameObject.transform.root.GetComponent<EnemyController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if this object is enemy hitbox and (player attack box or enemy attack box)
        if (gameObject.CompareTag("enemyHitbox")
            && (other.CompareTag("playerAttackBox") || other.CompareTag("enemyAttackBox")) )
        {
            // player attack
            if (other.GetComponent<PlayerAttackBox>() != null && other.GetComponent<PlayerAttackBox>().knockDownAttack)
            {
                enemyKnockedDown();
            }
            // enemy attack / friendly fire /vehicle
            else if (other.GetComponent<EnemyAttackBox>() != null && other.GetComponent<EnemyAttackBox>().knockDownAttack)
            {
                enemyKnockedDown();
            }
            else
            {
                enemyTakeDamage();
            }
        }
    }

    void enemyTakeDamage()
    {
        //Debug.Log("enemyTakeDamage()");
        StartCoroutine( enemyController.takeDamage());
    }

    void enemyKnockedDown()
    {
        //Debug.Log("enemyKnockedDown()");
        StartCoroutine(enemyController.knockedDown());
    }
}
