using System;
using System.Collections;
using UnityEngine;
using Level5.Core.Match;

/// <summary>
/// The player's health, block and special, and the regeneration that tops them back up.
///
/// AUD-012 Phase 2b Slice 25: the regeneration gate now reads an explicitly bound
/// <see cref="ResolvedMatchRules"/> (<see cref="BindMatchRules"/>, called by <c>SpawnCoordinator</c>
/// for both human and CPU participants) instead of reading <c>MatchRuntime.Rules</c> in
/// <see cref="Update"/>. <c>MatchRuntime</c> lives in <c>Assets/Scripts/game manager/</c>, outside
/// <c>Level5Match/</c>, so it is still <c>Assembly-CSharp</c> and was the single edge keeping this
/// component out of a production assembly. The gate itself is unchanged and still lives here -
/// composition supplies the rules, it does not decide when regeneration runs.
///
/// This stays deliberately separate from <c>ActorHealth</c>: it also owns player-only block,
/// special and regeneration state while sharing the <see cref="IDamageable"/> contract.
///
/// Follow-up to that slice: the four serialized fields that could never take effect
/// (<c>regenerateTimeDelay</c>, read nowhere, and the three regeneration rates, overwritten in
/// <see cref="Start"/> on every instance) are gone, and the redundant sniper terms in the
/// regeneration gate with them. Runtime behaviour is unchanged in all three cases.
/// </summary>
public class PlayerHealth : MonoBehaviour, IDamageable
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
    bool isDead = false;

    // How long each regeneration tick waits, in seconds; the tick itself is always +1.
    // A rate of 0.5 is +1 every half second, or +20 in 10 seconds.
    // A rate of 2 is +1 every two seconds, or +50 in 100 seconds (1 min 40 secs).
    // A rate of 0.04 is +1 every 0.04 seconds, or +100 in 4 seconds.
    //
    // Constants rather than [SerializeField]s: Start() used to overwrite the authored values with
    // exactly these numbers on every instance, so what the prefabs stored was dead data that could
    // never take effect. Honouring the authored values instead was not an option: across the 71
    // authored components, regenerateHealthRate and regenerateSpecialRate are 0 in all 71, and
    // regenerateBlockRate is 0 in 27 of them (0.5 in the other 44). A rate of 0 makes WaitForSeconds
    // return immediately, so that would turn regeneration into an instant refill on every prefab.
    // These are the numbers the game has always run on.
    private const float RegenerateBlockRate = 0.5f;
    private const float RegenerateHealthRate = 2f;
    private const float RegenerateSpecialRate = 0.04f;

    bool regenerateBlock = false;
    bool regenerateSpecial = false;
    bool regenerateHealth = false;

    /// <summary>
    /// The rules this match is being played under, bound once by composition. Not serialized: it is
    /// runtime-only, set after the component already exists, and <see cref="ResolvedMatchRules"/> is
    /// not itself <c>[Serializable]</c>.
    /// </summary>
    private ResolvedMatchRules matchRules;

    public event Action OnHealthChanged;
    public event Action OnBlockChanged;
    public event Action OnSpecialChanged;
    public event Action OnDied;

    /// <summary>
    /// Binds the rules this match is being played under. Bind-once, in the shape
    /// <c>CallBallToPlayer</c>, <c>BasketBall</c> and <c>BasketBallState</c> already use, including
    /// its guard ordering: the already-bound branch is checked before the null-argument branch, so a
    /// null second call after a real bind reports "already bound" rather than "remaining unbound"
    /// and cannot obscure the original valid reference.
    /// </summary>
    public void BindMatchRules(ResolvedMatchRules rules)
    {
        if (matchRules != null)
        {
            Debug.LogError($"PlayerHealth on '{gameObject.name}' already has bound match rules; ignoring a second BindMatchRules call.", this);
            return;
        }

        if (rules == null)
        {
            Debug.LogError($"PlayerHealth on '{gameObject.name}' was bound with null match rules; remaining unbound.", this);
            return;
        }

        matchRules = rules;
    }

    private void Awake()
    {
        Health = maxHealth;
        Block = maxBlock;
        Special = maxSpecial;
    }

    private void Start()
    {
        // A participant composed through SpawnCoordinator always has rules by now - both registration
        // paths bind during GameLevelManager.Awake, before any Start runs. Reaching here unbound is a
        // composition defect, reported once here rather than every frame from Update(). Damage, death,
        // clamping and the events all keep working; only regeneration is skipped, and this neither
        // reaches back into MatchRuntime nor invents default rules to stand in for the real ones.
        if (matchRules == null)
        {
            Debug.LogError($"PlayerHealth on '{gameObject.name}' reached Start() with no bound match rules; regeneration is disabled for this participant.", this);
        }
    }

    private void Update()
    {
        if (health <= 0 && !IsDead)
        {
            IsDead = true;
        }

        if (health > maxHealth)
        {
            Health = maxHealth;
        }

        if (matchRules == null)
        {
            return;
        }

        // SniperEnabled is `Sniper != SniperMode.None`, so it already covers every sniper variant.
        // This used to spell out `|| Sniper == Bullet || Sniper == Laser` alongside it, which could
        // not change the result and left MachineGun looking deliberately excluded when it never was.
        if (matchRules.EnemiesEnabled
            || matchRules.SniperEnabled
            || matchRules.ObstaclesEnabled)
        {
            if (block < MaxBlock && !regenerateBlock)
            {
                StartCoroutine(RegenerateBlock());
            }

            if (health < maxHealth && !IsDead && !regenerateHealth)
            {
                StartCoroutine(RegenerateHealth());
            }

            if (special < maxSpecial && !regenerateSpecial)
            {
                StartCoroutine(RegenerateSpecial());
            }
        }
    }

    IEnumerator RegenerateSpecial()
    {
        regenerateSpecial = true;
        yield return new WaitForSeconds(RegenerateSpecialRate);
        Special += 1f;
        regenerateSpecial = false;
    }

    IEnumerator RegenerateBlock()
    {
        regenerateBlock = true;
        yield return new WaitForSeconds(RegenerateBlockRate);
        Block += 1f;
        regenerateBlock = false;
    }

    IEnumerator RegenerateHealth()
    {
        regenerateHealth = true;
        yield return new WaitForSeconds(RegenerateHealthRate);
        Health += 1f;
        regenerateHealth = false;
    }

    public bool TakeDamage(float damage)
    {
        return ApplyDamage(new DamageInfo(damage));
    }

    public bool ApplyDamage(DamageInfo damageInfo)
    {
        if (damageInfo.Amount <= 0 || IsDead)
        {
            return IsDead;
        }

        Health -= damageInfo.Amount;
        return IsDead;
    }

    public void Heal(float amount)
    {
        if (amount <= 0 || IsDead)
        {
            return;
        }

        Health += amount;
    }

    public void SpendBlock(float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Block -= amount;
    }

    public void SpendSpecial(float amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Special -= amount;
    }

    public float Health
    {
        get => health;
        set
        {
            float clampedHealth = Mathf.Clamp(value, 0, maxHealth);
            if (Mathf.Approximately(health, clampedHealth))
            {
                return;
            }

            health = clampedHealth;
            OnHealthChanged?.Invoke();

            if (health <= 0 && !IsDead)
            {
                IsDead = true;
            }
        }
    }

    public float Block
    {
        get => block;
        set
        {
            float clampedBlock = Mathf.Clamp(value, 0, maxBlock);
            if (Mathf.Approximately(block, clampedBlock))
            {
                return;
            }

            block = clampedBlock;
            OnBlockChanged?.Invoke();
        }
    }

    public bool IsDead
    {
        get => isDead;
        set
        {
            if (isDead == value)
            {
                return;
            }

            isDead = value;
            if (isDead && health > 0)
            {
                health = 0;
                OnHealthChanged?.Invoke();
            }

            if (isDead)
            {
                OnDied?.Invoke();
            }
        }
    }

    public int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public int MaxBlock { get => maxBlock; set => maxBlock = value; }

    public float Special
    {
        get => special;
        set
        {
            float clampedSpecial = Mathf.Clamp(value, 0, maxSpecial);
            if (Mathf.Approximately(special, clampedSpecial))
            {
                return;
            }

            special = clampedSpecial;
            OnSpecialChanged?.Invoke();
        }
    }

    public int MaxSpecial { get => maxSpecial; set => maxSpecial = value; }
    public float CurrentHealth => Health;
    public float CurrentMaxHealth => MaxHealth;
}
