using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    int health;
    [SerializeField]
    int maxEnemyHealth;
    [SerializeField]
    EnemyController enemyController;
    //bool isDead;
    //bool isLocked = false;

    public int Health { get => health; set => health = value; }
    public int MaxEnemyHealth { get => maxEnemyHealth; set => maxEnemyHealth = value; }

    private void Start()
    {
        // default
        maxEnemyHealth = 100;
        health = maxEnemyHealth;

        enemyController = transform.parent.GetComponent<EnemyController>();
        //if (health > 0)
        //{
        //    isDead = false;
        //}
    }

    //private void Update()
    //{
    //    if (health <= 0 && !isDead )
    //    {
    //        isDead = true;
    //    }
    //    if (isDead && !isLocked)
    //    {
    //        isLocked = true;
    //        StartCoroutine(enemyController.disintegrated());
    //    }
    //}
}
