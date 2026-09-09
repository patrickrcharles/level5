using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;
using Level5.Core.Match;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

/// <summary>
/// AUD-012 Phase 2b Slice 30: <see cref="PlayerAttackQueue"/>'s queue-capacity/battle-royal policy
/// and its participant anchor now read an explicitly bound <see cref="ResolvedMatchRules"/> and
/// <see cref="Transform"/> (<see cref="PlayerAttackQueue.BindMatchContext"/>, called by
/// <see cref="SpawnCoordinator"/> from both the human and the CPU registration path) instead of
/// reading <c>MatchRuntime.Rules</c> and resolving its own <c>PlayerIdentifier</c>.
///
/// The policy itself is unchanged and is asserted here as behaviour: queue capacity and the
/// battle-royal shared-slot decision follow the bound rules, slot positioning follows the bound
/// anchor, and an unbound queue still runs on safe standard-capacity/own-transform defaults.
/// Mirrors <see cref="Level5PlayerHealthMatchRulesTests"/>'s and
/// <see cref="Level5CallBallToPlayerMatchRulesTests"/>'s shape - the SpawnCoordinator_* tests drive
/// the real private RegisterHuman/RegisterCpu composition path rather than a stand-in.
/// </summary>
public class Level5PlayerAttackQueueMatchContextTests
{
    /// <summary>Minimal ICombatAgent double - a real Component so CanAct/activeInHierarchy behave exactly like production.</summary>
    private sealed class FakeCombatAgent : MonoBehaviour, ICombatAgent
    {
        public GameObject CombatObject => gameObject;
        public Transform CombatTransform => transform;
        public bool CanAct => true;
    }

    private readonly List<GameObject> spawned = new List<GameObject>();
    private PlayerRegistry registry;
    private SpawnCoordinator coordinator;
    private ResolvedMatchRules coordinatorRules;
    private MethodInfo registerHuman;
    private MethodInfo registerCpu;

    [SetUp]
    public void SetUp()
    {
        // PlayerAttackQueue no longer reads MatchRuntime at all; clearing still keeps the rest of the
        // composition path (InitializeHumanProfile, PrepareCpuMatchContext) deterministic regardless
        // of what ran before this test.
        ActiveMatch.Clear();

        registry = new PlayerRegistry();

        // Hardcore + EnemiesOnly, deliberately not the default: a participant that ends up with a
        // fresh default ResolvedMatchRules instead of this exact match's rules would compute the
        // wrong capacity (4 instead of 8) and fail the composition tests below.
        coordinatorRules = Rules(hardcore: true, enemiesOnly: true);
        coordinator = new SpawnCoordinator(
            new SpawnCoordinator.SpawnLocations(),
            registry,
            coordinatorRules,
            new PlayerRoster(new PlayerSlot[0]),
            GameModeId.None);

        registerHuman = typeof(SpawnCoordinator).GetMethod("RegisterHuman", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(registerHuman, "SpawnCoordinator.RegisterHuman must exist");
        registerCpu = typeof(SpawnCoordinator).GetMethod("RegisterCpu", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(registerCpu, "SpawnCoordinator.RegisterCpu must exist");
    }

    [TearDown]
    public void TearDown()
    {
        ActiveMatch.Clear();

        foreach (GameObject go in spawned)
        {
            if (go != null)
            {
                Object.DestroyImmediate(go);
            }
        }

        spawned.Clear();
    }

    private GameObject Spawn(string name)
    {
        GameObject go = new GameObject(name);
        spawned.Add(go);
        return go;
    }

    private static ResolvedMatchRules Rules(
        bool hardcore = false,
        bool enemiesOnly = false,
        CombatMode combatMode = CombatMode.None)
    {
        return new ResolvedMatchRules(combatMode: combatMode, hardcore: hardcore, enemiesOnly: enemiesOnly);
    }

    private static void Invoke(PlayerAttackQueue queue, string method)
    {
        MethodInfo info = typeof(PlayerAttackQueue).GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(info, $"PlayerAttackQueue must declare {method}()");
        info.Invoke(queue, null);
    }

    private static object GetPrivateField(PlayerAttackQueue queue, string field)
    {
        FieldInfo info = typeof(PlayerAttackQueue).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(info, $"PlayerAttackQueue must declare a {field} field");
        return info.GetValue(queue);
    }

    private static void SetPrivateField(PlayerAttackQueue queue, string field, object value)
    {
        FieldInfo info = typeof(PlayerAttackQueue).GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(info, $"PlayerAttackQueue must declare a {field} field");
        info.SetValue(queue, value);
    }

    private static int InvokeGetMaxEnemiesQueued(PlayerAttackQueue queue)
    {
        MethodInfo info = typeof(PlayerAttackQueue).GetMethod("GetMaxEnemiesQueued", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(info, "PlayerAttackQueue must declare GetMaxEnemiesQueued()");
        return (int)info.Invoke(queue, null);
    }

    private static PlayerAttackPosition InvokeSelectAttackSlot(PlayerAttackQueue queue, GameObject attacker)
    {
        MethodInfo info = typeof(PlayerAttackQueue).GetMethod("SelectAttackSlot", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(info, "PlayerAttackQueue must declare SelectAttackSlot(GameObject)");
        return (PlayerAttackPosition)info.Invoke(queue, new object[] { attacker });
    }

    /// <summary>A queue with one real PlayerAttackPosition child, so CacheAttackPositions finds it.</summary>
    private PlayerAttackQueue QueueWithOnePosition(string name)
    {
        GameObject queueGo = Spawn(name);
        PlayerAttackQueue queue = queueGo.AddComponent<PlayerAttackQueue>();

        GameObject slotGo = new GameObject("slot0");
        spawned.Add(slotGo);
        slotGo.transform.SetParent(queueGo.transform);
        slotGo.AddComponent<PlayerAttackPosition>();

        return queue;
    }

    private GameObject SpawnAgent(string name)
    {
        GameObject go = Spawn(name);
        go.AddComponent<FakeCombatAgent>();
        return go;
    }

    // ==================== binding contract ====================

    [Test]
    public void BindMatchContext_Null_IsRejected()
    {
        PlayerAttackQueue queue = Spawn("queue").AddComponent<PlayerAttackQueue>();

        LogAssert.Expect(LogType.Error, new Regex("null match rules"));
        queue.BindMatchContext(null, queue.transform);

        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match context"));
        Invoke(queue, "Start");
    }

    [Test]
    public void BindMatchContext_SecondCall_KeepsTheOriginalContext()
    {
        PlayerAttackQueue queue = Spawn("queue").AddComponent<PlayerAttackQueue>();
        queue.BindMatchContext(Rules(hardcore: true, enemiesOnly: true), queue.transform);

        // Including a null second call: it must report "already bound", not "remaining unbound".
        LogAssert.Expect(LogType.Error, new Regex("already has bound match context"));
        queue.BindMatchContext(null, queue.transform);
        LogAssert.Expect(LogType.Error, new Regex("already has bound match context"));
        queue.BindMatchContext(Rules(), queue.transform);

        Assert.That(InvokeGetMaxEnemiesQueued(queue), Is.EqualTo(8), "the first, Hardcore + EnemiesOnly binding must still be the one in force");
    }

    // ==================== missing composition ====================

    [Test]
    public void Start_WithNoBoundContext_ReportsTheCompositionErrorOnceAndUsesSafeDefaults()
    {
        GameObject queueGo = Spawn("queue");
        queueGo.transform.position = new Vector3(3f, 0f, 4f);
        PlayerAttackQueue queue = queueGo.AddComponent<PlayerAttackQueue>();

        LogAssert.Expect(LogType.Error, new Regex("reached Start\\(\\) with no bound match context"));
        Invoke(queue, "Start");

        // LateUpdate must not repeat the report every frame - an unexpected Debug.LogError from
        // either call would fail this test on its own.
        Invoke(queue, "LateUpdate");
        Invoke(queue, "LateUpdate");

        // Deliberately no assertion on AttackSlotOpen: this queue has no child slots, so
        // CacheAttackPositions falls through to a scene-wide FindGameObjectsWithTag search, and any
        // tagged object in whatever scene the editor happens to have open would decide that flag.
        // The missing-context contract is the capacity fallback, the single diagnostic, and staying
        // enabled - none of which depend on ambient scene state.
        Assert.That(InvokeGetMaxEnemiesQueued(queue), Is.EqualTo(4), "unbound must fall back to EnemyPopulationRules.MaxQueued(null)'s standard capacity");
        Assert.IsTrue(queue.enabled, "missing context must use safe defaults, not disable the component");
        Assert.IsTrue(queue.gameObject.activeSelf, "missing context must use safe defaults, not disable the GameObject");
    }

    // ==================== queue capacity follows bound rules ====================

    [Test]
    public void GetMaxEnemiesQueued_UsesBoundRules_NotTheUnboundDefault()
    {
        PlayerAttackQueue bound = Spawn("bound-queue").AddComponent<PlayerAttackQueue>();
        bound.BindMatchContext(Rules(hardcore: true, enemiesOnly: true), bound.transform);

        PlayerAttackQueue unbound = Spawn("unbound-queue").AddComponent<PlayerAttackQueue>();

        Assert.That(InvokeGetMaxEnemiesQueued(bound), Is.EqualTo(8));
        Assert.That(InvokeGetMaxEnemiesQueued(unbound), Is.EqualTo(4), "an unbound queue must use EnemyPopulationRules.MaxQueued(null), not invent or reach for a different default");
    }

    // ==================== battle-royal shared-slot decision follows bound rules ====================

    /// <summary>
    /// One position, already occupied, with capacity forced down to exactly the position count (1) -
    /// so the "more capacity than positions" half of SelectAttackSlot's allowSharedSlots OR is false,
    /// isolating whichever rules.IsBattleRoyal was bound as the only thing that could still permit a
    /// second attacker onto the same slot.
    /// </summary>
    private PlayerAttackQueue QueueWithOneOccupiedSlotAtCapacity(ResolvedMatchRules rules)
    {
        PlayerAttackQueue queue = QueueWithOnePosition("queue");
        queue.BindMatchContext(rules, queue.transform);
        Invoke(queue, "Start");

        GameObject occupant = SpawnAgent("occupant");
        Assert.IsTrue(queue.TryReserve(occupant, out _), "setup: the first attacker must take the only slot");

        SetPrivateField(queue, "maxEnemiesQueued", 1);
        return queue;
    }

    [Test]
    public void SelectAttackSlot_BattleRoyalRules_SharesAnAlreadyOccupiedSlotEvenAtCapacity()
    {
        PlayerAttackQueue queue = QueueWithOneOccupiedSlotAtCapacity(Rules(combatMode: CombatMode.BattleRoyal));
        GameObject challenger = SpawnAgent("challenger");

        PlayerAttackPosition slot = InvokeSelectAttackSlot(queue, challenger);

        Assert.IsNotNull(slot, "battle royal rules must allow sharing the only slot even though capacity (1) does not exceed the position count (1)");
    }

    [Test]
    public void SelectAttackSlot_StandardRules_DoesNotShareAnAlreadyOccupiedSlotAtCapacity()
    {
        PlayerAttackQueue queue = QueueWithOneOccupiedSlotAtCapacity(Rules());
        GameObject challenger = SpawnAgent("challenger");

        PlayerAttackPosition slot = InvokeSelectAttackSlot(queue, challenger);

        Assert.IsNull(slot, "non-battle-royal rules with capacity equal to the position count must not share an occupied slot");
    }

    // ==================== anchor positioning follows the bound anchor ====================

    [Test]
    public void UpdateAttackPositionTransforms_FollowsTheBoundAnchor_NotTheQueuesOwnTransform()
    {
        GameObject queueGo = Spawn("queue");
        queueGo.transform.position = Vector3.zero;
        PlayerAttackQueue queue = queueGo.AddComponent<PlayerAttackQueue>();

        GameObject slotGo = new GameObject("slot0");
        spawned.Add(slotGo);
        slotGo.transform.SetParent(queueGo.transform);
        PlayerAttackPosition slot = slotGo.AddComponent<PlayerAttackPosition>();

        GameObject anchorGo = Spawn("anchor");
        anchorGo.transform.position = new Vector3(5f, 0f, 5f);

        queue.BindMatchContext(Rules(), anchorGo.transform);
        Invoke(queue, "Start");
        Invoke(queue, "UpdateAttackPositionTransforms");

        // Slot 0's authored offset (PlayerAttackPosition.GetOffsetForSlot) is (-0.6, 0, -0.25).
        Vector3 expected = anchorGo.transform.position + new Vector3(-0.6f, 0f, -0.25f);
        Assert.That(Vector3.Distance(slot.transform.position, expected), Is.LessThan(0.0001f),
            "attack slots must follow the bound anchor, not the queue component's own transform");
    }

    // ==================== SpawnCoordinator composition ====================

    private GameObject SpawnHumanParticipantWithQueue(int pid)
    {
        GameObject actorGo = Spawn($"human-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<PlayerController>();
        actorGo.AddComponent<PlayerIdentifier>();
        actorGo.AddComponent<PlayerAttackQueue>();
        return actorGo;
    }

    private GameObject SpawnCpuParticipantWithQueue(int pid)
    {
        GameObject actorGo = Spawn($"cpu-actor-{pid}");
        actorGo.AddComponent<CharacterProfile>();
        actorGo.AddComponent<AutoPlayerController>();
        actorGo.AddComponent<PlayerIdentifier>();
        actorGo.AddComponent<PlayerAttackQueue>();
        return actorGo;
    }

    /// <summary>
    /// The coordinator's rules are this exact instance (reference equality) and the anchor is the
    /// spawned participant root itself - both read straight back out of the bound private fields
    /// rather than through observable queue behaviour, mirroring how the dependency-guard tests
    /// verify composition without needing a full reservation flow.
    /// </summary>
    private void AssertComposedWithThisMatchsRulesAndOwnTransformAsAnchor(GameObject participant)
    {
        PlayerAttackQueue queue = participant.GetComponent<PlayerAttackQueue>();
        Assert.That(GetPrivateField(queue, "matchRules"), Is.SameAs(coordinatorRules));
        Assert.That(GetPrivateField(queue, "participantAnchor"), Is.SameAs(participant.transform));
    }

    [Test]
    public void SpawnCoordinator_RegisterHuman_BindsThisMatchsRulesAndParticipantAnchorToPlayerAttackQueue()
    {
        GameObject actorGo = SpawnHumanParticipantWithQueue(pid: 0);

        registerHuman.Invoke(coordinator, new object[] { actorGo, 0, null });

        AssertComposedWithThisMatchsRulesAndOwnTransformAsAnchor(actorGo);
    }

    [Test]
    public void SpawnCoordinator_RegisterCpu_BindsThisMatchsRulesAndParticipantAnchorToPlayerAttackQueue()
    {
        GameObject actorGo = SpawnCpuParticipantWithQueue(pid: 1);

        registerCpu.Invoke(coordinator, new object[] { actorGo, 1 });

        AssertComposedWithThisMatchsRulesAndOwnTransformAsAnchor(actorGo);
    }

    [Test]
    public void SpawnCoordinator_ParticipantWithoutPlayerAttackQueue_IsSkippedSilently()
    {
        // Modelled on Lockdown's defender prefab (cpu_player_defense_oldreal), which carries
        // AutoPlayerDefense rather than AutoPlayerController and no PlayerAttackQueue at all: that is
        // authored composition, not a defect, and must not log or throw.
        GameObject defender = Spawn("cpu-defender");
        defender.AddComponent<CharacterProfile>();
        defender.AddComponent<AutoPlayerDefense>();
        defender.AddComponent<PlayerIdentifier>();

        Assert.DoesNotThrow(() => registerCpu.Invoke(coordinator, new object[] { defender, 1 }));
        Assert.IsNull(defender.GetComponent<PlayerAttackQueue>());
    }
}
