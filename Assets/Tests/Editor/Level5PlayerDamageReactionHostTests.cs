using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 32: <see cref="PlayerController"/> now implements
/// <see cref="IPlayerDamageReactionHost"/>, the narrow contract <c>PlayerDamageReactions</c> (moved
/// into <c>Level5.Player</c>) uses instead of the concrete controller. These tests establish the
/// mapping is real - a representative read and write round-trip through the interface reach the same
/// backing state as the controller's own public surface - rather than testing every member
/// independently.
/// </summary>
public class Level5PlayerDamageReactionHostTests
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
    public void PlayerControllerImplementsIPlayerDamageReactionHost()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();

        Assert.That(controller, Is.InstanceOf<IPlayerDamageReactionHost>());
    }

    [Test]
    public void HostReadsReflectExistingControllerState()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        IPlayerDamageReactionHost host = controller;

        controller.KnockedDown = true;
        controller.TakeDamage = true;
        controller.Locked = true;
        controller.AvoidedKnockDown = true;
        controller.FacingRight = false;
        controller.isShrunk = true;
        controller.CurrentState = 111;
        controller.takeDamageState = 222;
        controller.knockedDownState = 333;
        controller.disintegratedState = 444;
        controller.lightningState = 555;

        Assert.That(host.KnockedDown, Is.True);
        Assert.That(host.TakeDamage, Is.True);
        Assert.That(host.Locked, Is.True);
        Assert.That(host.AvoidedKnockDown, Is.True);
        Assert.That(host.FacingRight, Is.False);
        Assert.That(host.IsShrunk, Is.True);
        Assert.That(host.CurrentState, Is.EqualTo(111));
        Assert.That(host.TakeDamageStateHash, Is.EqualTo(222));
        Assert.That(host.KnockedDownStateHash, Is.EqualTo(333));
        Assert.That(host.DisintegratedStateHash, Is.EqualTo(444));
        Assert.That(host.LightningStateHash, Is.EqualTo(555));
        Assert.That(host.ActorTransform, Is.SameAs(controller.transform));
    }

    [Test]
    public void HostWritesMutateTheSameControllerState()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        IPlayerDamageReactionHost host = controller;

        host.KnockedDown = true;
        host.TakeDamage = true;
        host.Locked = true;
        host.AvoidedKnockDown = true;
        host.FacingRight = false;
        host.IsShrunk = true;

        Assert.That(controller.KnockedDown, Is.True);
        Assert.That(controller.TakeDamage, Is.True);
        Assert.That(controller.Locked, Is.True);
        Assert.That(controller.AvoidedKnockDown, Is.True);
        Assert.That(controller.FacingRight, Is.False);
        Assert.That(controller.isShrunk, Is.True);
    }

    [Test]
    public void MarkDeadUpdatesPlayerHealthDeathStateRatherThanDuplicatingIt()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        PlayerHealth health = Spawn("player-health").AddComponent<PlayerHealth>();
        controller.PlayerHealth = health;
        IPlayerDamageReactionHost host = controller;

        Assert.That(health.IsDead, Is.False);

        host.MarkDead();

        Assert.That(health.IsDead, Is.True, "MarkDead must flip the existing PlayerHealth.IsDead, not invent separate death state");
    }

    // ==================== shrink camera resolver ====================

    [Test]
    public void GetShrinkCameraIsNullWhenNoResolverIsBound()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        IPlayerDamageReactionHost host = controller;

        Assert.That(host.GetShrinkCamera(), Is.Null);
    }

    [Test]
    public void GetShrinkCameraResolvesTheBoundReaderLiveNotSnapshotted()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        IPlayerDamageReactionHost host = controller;
        Camera first = Spawn("camera-1").AddComponent<Camera>();
        Camera second = Spawn("camera-2").AddComponent<Camera>();
        Camera current = first;

        controller.BindDamageReactionCameraReader(() => current);

        Assert.That(host.GetShrinkCamera(), Is.SameAs(first));

        current = second;

        Assert.That(
            host.GetShrinkCamera(),
            Is.SameAs(second),
            "the resolver must be invoked live on every call, not captured once at bind time");
    }

    [Test]
    public void GetShrinkCameraIsNullWhenTheBoundResolverReturnsNull()
    {
        PlayerController controller = Spawn("player").AddComponent<PlayerController>();
        IPlayerDamageReactionHost host = controller;

        controller.BindDamageReactionCameraReader(() => null);

        Assert.That(host.GetShrinkCamera(), Is.Null);
    }
}
