using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    int health = 0;
    [SerializeField]
    int block;
    bool isDead = false;

    public int Health { get => health; set => health = value; }
    public int Block { get => block; set => block = value; }
    public bool IsDead { get => isDead; set => isDead = value; }

    private void Awake()
    {
        // default
        health = 0;
        block = 25;
        if(health <= 100)
        {
            IsDead = false;
        }
    }

    private void Update()
    {
        if(health >= 100 && !IsDead)
        {
            Debug.Log("player is dead");
            IsDead = true;
        }
    }
}
