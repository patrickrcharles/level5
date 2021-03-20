using System.Collections;
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
    int maxBlock = 20;
    [SerializeField]
    float regenerateBlockRate;
    [SerializeField]
    float regenerateHealthRate;
    [SerializeField]
    float regenerateTimeDelay;

    bool isDead = false;
    bool regenerateBlock = false;

    PlayerController playerController;
    private bool regenerateHealth;

    public float Health { get => health; set => health = value; }
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
        playerController = GameLevelManager.instance.PlayerController;
        regenerateBlockRate = 0.5f;
        regenerateHealthRate = 1f;
    }

    private void Update()
    {
        if (GameOptions.enemiesEnabled || GameOptions.sniperEnabled)
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
            if (health < maxHealth
                && !regenerateHealth
                && (playerController.currentState == playerController.idleState
                || playerController.currentState == playerController.bIdle
                || playerController.currentState == playerController.walkState
                || playerController.currentState == playerController.run
                || playerController.currentState == playerController.bWalk))
            {
                StartCoroutine(RegenerateHealth());
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
}
