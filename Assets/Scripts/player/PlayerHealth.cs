﻿using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    float health = 0;
    [SerializeField]
    int maxHealth = 100;
    [SerializeField]
    float block;
    [SerializeField]
    int maxBlock = 30;
    [SerializeField]
    float special;
    [SerializeField]
    int maxSpecial = 100;
    [SerializeField]
    float regenerateBlockRate;
    [SerializeField]
    float regenerateHealthRate;
    [SerializeField]
    float regenerateSpecialRate;
    [SerializeField]
    float regenerateTimeDelay;
    [SerializeField]
    bool isDead = false;
    bool regenerateBlock = false;
    bool regenerateSpecial = false;

    PlayerController playerController;
    private bool regenerateHealth;

    private void Awake()
    {
        // default
        health = maxHealth;
        block = maxBlock;
        special = maxSpecial;
    }

    private void Start()
    {
        playerController = GameLevelManager.instance.PlayerController1;
        // regenerate rate is +1 per interval
        // rate of 0.4f is equal to +1 every 0.5 second or +25 in 10 secs
        // rate of 1f is equal to +1 every 1 second or +100 in 100 seconds (1 min 40 secs)
        // rate of 0.04f is equal to +1 every 0.04 second or +100 in 4 seconds 
        regenerateBlockRate = 0.5f;
        regenerateHealthRate = 2f;
        regenerateSpecialRate = 0.04f;
    }

    private void Update()
    {
        if (GameOptions.enemiesEnabled 
            || GameOptions.sniperEnabled 
            || GameOptions.sniperEnabledBullet 
            || GameOptions.sniperEnabledLaser
            || GameOptions.obstaclesEnabled)
        {
            if (health <= 0 && !IsDead)
            {
                IsDead = true;
            }
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            // regenerate
            if (block < MaxBlock
                && !regenerateBlock)
            {
                StartCoroutine(RegenerateBlock());
            }
            if (health < maxHealth
                && !regenerateHealth)
            {
                StartCoroutine(RegenerateHealth());
            }
            if (special < maxSpecial
                && !regenerateSpecial)
            {
                StartCoroutine(RegenerateSpecial());
            }
        }
    }

    IEnumerator RegenerateSpecial()
    {
        //Debug.Log("RegenerateSpecial : " + special + "  time : " + Time.time);
        regenerateSpecial = true;
        yield return new WaitForSeconds(regenerateSpecialRate);
        special += 1f;
        if (PlayerHealthBar.instance != null)
        {
            PlayerHealthBar.instance.setSpecialSliderValue();
        }
        regenerateSpecial = false;
    }

    IEnumerator RegenerateBlock()
    {
        regenerateBlock = true;
        yield return new WaitForSeconds(regenerateBlockRate);
        block += 1f;
        if (PlayerHealthBar.instance != null)
        {
            PlayerHealthBar.instance.setBlockSliderValue();
        }
        regenerateBlock = false;
    }

    IEnumerator RegenerateHealth()
    {
        regenerateHealth = true;
        yield return new WaitForSeconds(regenerateHealthRate);
        health += 1f;
        if (PlayerHealthBar.instance != null)
        {
            PlayerHealthBar.instance.setHealthSliderValue();
        }
        regenerateHealth = false;
    }
    public float Health { get => health; set => health = value; }
    public float Block { get => block; set => block = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int MaxBlock { get => maxBlock; set => maxBlock = value; }
    public float Special { get => special; set => special = value; }
    public int MaxSpecial { get => maxSpecial; set => maxSpecial = value; }
}
