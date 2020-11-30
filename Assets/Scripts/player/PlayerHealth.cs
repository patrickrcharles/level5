using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    int health = 0;
    [SerializeField]
    int maxHealth = 100;
    [SerializeField]
    float block;
    [SerializeField]
    int maxBlock = 20;
    [SerializeField]
    float regenerateBlockRate;
    [SerializeField]
    float regenerateTimeDelay;

    bool isDead = false;
    bool regenerateBlock = false;

    PlayerController playerController;

    public int Health { get => health; set => health = value; }
    public float Block { get => block; set => block = value; }
    public bool IsDead { get => isDead; set => isDead = value; }
    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int MaxBlock { get => maxBlock; set => maxBlock = value; }

    private void Awake()
    {
        // default
        health = maxHealth;
        block = maxBlock;
    }

    private void Start()
    {
        playerController = GameLevelManager.instance.PlayerState;
    }

    private void Update()
    {
        if (GameOptions.enemiesEnabled)
        {
            if (health <= 0 && !IsDead)
            {
                IsDead = true;
            }
            if (health > maxHealth)
            {
                health = maxHealth;
            }
            if (block < MaxBlock
                && !regenerateBlock
                && (playerController.currentState == playerController.idleState
                || playerController.currentState == playerController.bIdle
                || playerController.currentState == playerController.walkState
                || playerController.currentState == playerController.run
                || playerController.currentState == playerController.bWalk))
            {
                StartCoroutine(RegenerateBlock());
            }
        }
    }

    IEnumerator RegenerateBlock()
    {
        regenerateBlock = true;
        yield return new WaitForSeconds(regenerateBlockRate);
        block += 1f;
        PlayerHealthBar.instance.setBlockSliderValue();
        regenerateBlock = false;
    }
}
