using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 33: <see cref="SniperManager"/> now implements
/// <see cref="IPlayerIdleSniperRuntime"/>, the narrow contract <c>PlayerController</c>'s idle-sniper
/// check uses instead of the concrete manager. These tests establish the mapping is real - the
/// interface's <c>Locked</c> reads/writes the same public <c>locked</c> field every other caller
/// (<c>startSniper</c>, <c>InstantiateConfiguredProjectile</c>, ...) already uses, and
/// <c>GetInstantKillRoutine</c> delegates to the existing <c>StartSniperBulletInstantKill</c> coroutine
/// rather than a separate implementation - without executing the projectile flow itself. Mirrors the
/// shape <see cref="Level5PlayerDamageReactionHostTests"/> already established for
/// <c>IPlayerDamageReactionHost</c>.
/// </summary>
public class Level5PlayerIdleSniperRuntimeTests
{
    private readonly List<GameObject> spawned = new List<GameObject>();

    [TearDown]
    public void TearDown()
    {
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

    [Test]
    public void SniperManagerImplementsIPlayerIdleSniperRuntime()
    {
        SniperManager manager = Spawn("sniper").AddComponent<SniperManager>();

        Assert.That(manager, Is.InstanceOf<IPlayerIdleSniperRuntime>());
    }

    [Test]
    public void InterfaceLockedReadsTheExistingPublicLockedField()
    {
        SniperManager manager = Spawn("sniper").AddComponent<SniperManager>();
        IPlayerIdleSniperRuntime runtime = manager;

        manager.locked = true;

        Assert.That(runtime.Locked, Is.True);
    }

    [Test]
    public void InterfaceLockedWriteMutatesTheSamePublicLockedField()
    {
        SniperManager manager = Spawn("sniper").AddComponent<SniperManager>();
        IPlayerIdleSniperRuntime runtime = manager;
        manager.locked = true;

        runtime.Locked = false;

        Assert.That(manager.locked, Is.False, "IPlayerIdleSniperRuntime.Locked must mutate the same backing field as the public locked, not a duplicate");
    }

    [Test]
    public void GetInstantKillRoutineDelegatesToStartSniperBulletInstantKill()
    {
        // Proves delegation to the existing coroutine method by identity of the compiler-generated
        // iterator state machine, without stepping the coroutine (which would need the full
        // playerHitbox/audioSource/projectile-pool rig StartSniperBulletInstantKill depends on).
        SniperManager manager = Spawn("sniper").AddComponent<SniperManager>();
        IPlayerIdleSniperRuntime runtime = manager;

        IEnumerator routine = runtime.GetInstantKillRoutine(1.5f);

        Assert.That(routine, Is.Not.Null);
        Assert.That(
            routine.GetType().Name,
            Does.Contain("StartSniperBulletInstantKill"),
            "GetInstantKillRoutine must return SniperManager's own StartSniperBulletInstantKill coroutine, not a separate implementation");
    }
}
