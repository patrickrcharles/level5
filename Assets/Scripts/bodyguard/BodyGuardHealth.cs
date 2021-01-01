using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyGuardHealth : MonoBehaviour
{
    [SerializeField]
    int health;
    [SerializeField]
    int maxBodyGuardHealth;
    [SerializeField]
    BodyGuardController bodyGuardController;
    bool isDead;
    [SerializeField]
    bool isMinion;
    [SerializeField]
    bool isBoss;

    public int Health { get => health; set => health = value; }
    public int MaxEnemyHealth { get => maxBodyGuardHealth; set => maxBodyGuardHealth = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public bool IsBoss { get => isBoss; set => isBoss = value; }

    private void Start()
    {
        // default
        //if (isMinion)
        //{
        //    maxBodyGuardHealth = 50;
        //}
        //else if (isBoss)
        //{
        //    maxBodyGuardHealth = 150;
        //}
        //else
        //{
        //    maxBodyGuardHealth = 50;
        //}
        maxBodyGuardHealth = 100;

        if (GameOptions.hardcoreModeEnabled)
        {
            maxBodyGuardHealth += (maxBodyGuardHealth/4);
        }
        health = maxBodyGuardHealth;

        bodyGuardController = transform.parent.GetComponent<BodyGuardController>();
    }
}
