using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField]
    int health;
    bool isDead;

    public int Health { get => health; set => health = value; }

    private void Start()
    {
        // default
        health = 100;
        if (health > 0)
        {
            isDead = false;
        }
    }

    private void Update()
    {
        if (health <= 0 && !isDead)
        {
            Debug.Log("enemy is dead");
            isDead = true;
        }
    }
}
