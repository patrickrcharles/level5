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
    bool isDead;
    //bool isLocked = false;

    public int Health { get => health; set => health = value; }
    public int MaxEnemyHealth { get => maxEnemyHealth; set => maxEnemyHealth = value; }
    public bool IsDead { get => isDead; set => isDead = value; }

    private void Start()
    {
        // default
        maxEnemyHealth = 100;
        health = maxEnemyHealth;

        enemyController = transform.parent.GetComponent<EnemyController>();
    }
}
