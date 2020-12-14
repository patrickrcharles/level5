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
    [SerializeField]
    bool isMinion;
    [SerializeField]
    bool isBoss;

    public int Health { get => health; set => health = value; }
    public int MaxEnemyHealth { get => maxEnemyHealth; set => maxEnemyHealth = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsBoss { get => isBoss; set => isBoss = value; }

    private void Start()
    {
        // default
        if (isMinion)
        {
            maxEnemyHealth = 50;
        }
        else if (isBoss)
        {
            maxEnemyHealth = 150;
        }
        else
        {
            maxEnemyHealth = 50;
        }
        if (GameOptions.hardcoreModeEnabled)
        {
            maxEnemyHealth += (maxEnemyHealth/4);
        }
        health = maxEnemyHealth;

        enemyController = transform.parent.GetComponent<EnemyController>();
    }
}
