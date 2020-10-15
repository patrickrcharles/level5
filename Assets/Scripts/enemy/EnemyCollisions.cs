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
        //Debug.Log("this.tag : " + gameObject.tag + "  other.tag : " + other.tag);

        if (gameObject.CompareTag("enemyHitbox")
            && (other.CompareTag("playerAttackBox") || other.CompareTag("knock_down_attack")))
        {
            //Debug.Log("------------------------------- player attacked enemy");
            if (other.CompareTag("knock_down_attack") || other.GetComponent<PlayerAttackBox>().knockDownAttack)
            {
                //Debug.Log("     player used knockdown attack");
                enemyKnockedDown();
            }
            else
            {
                // avoid knockdown scenario
                //playerAvoidKnockDown(other.gameObject);
                enemyTakeDamage();
            }
        }
        if (gameObject.CompareTag("enemyHitbox")
            && (other.CompareTag("playerAttackBox") && other.name == "attackBoxSpecial"))
        {
            Debug.Log("------------------------------- "+  other.name +" hit "+ gameObject.name +" enemy with special enemy");
            //if (other.CompareTag("knock_down_attack") || other.GetComponent<PlayerAttackBox>().knockDownAttack)
            //{
            //    //Debug.Log("     player used knockdown attack");
            //    enemyKnockedDown();
            //}
            //else
            //{
            //    // avoid knockdown scenario
            //    //playerAvoidKnockDown(other.gameObject);
            //    enemyTakeDamage();
            //}
        }
    }


    void enemyTakeDamage()
    {
        //Debug.Log("enemyKnockedDown()");
        StartCoroutine( enemyController.takeDamage());
    }

    void enemyKnockedDown()
    {
        //Debug.Log("enemyKnockedDown()");
        StartCoroutine(enemyController.knockedDown());
    }
}
