using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    int health;
    [SerializeField]
    int block;

    bool isDead;

    public int Health { get => health; set => health = value; }
    public int Block { get => block; set => block = value; }

    private void Start()
    {
        // default
        health = 0;
        block = 25;
        if(health >= 100)
        {
            isDead = false;
        }
    }

    private void Update()
    {
        if(health <= 0 && !isDead)
        {
            Debug.Log("player is dead");
            isDead = true;
        }
    }
}
