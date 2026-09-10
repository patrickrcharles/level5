using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 35: <see cref="PlayerController"/> now implements
/// <see cref="IPlayerDunkHost"/>, the narrow contract <c>PlayerDunk</c> uses instead of the concrete
/// controller. These tests establish the mapping is real - representative members round-trip the same
/// public state every other caller already uses, rather than duplicating it - mirroring the shape
/// <see cref="Level5PlayerControllerParticipantStateTests"/> already established for
/// <c>IPlayerControllerParticipantState</c>. Per the "protect representative mappings" guidance, this
/// does not re-verify every trivial forwarding member independently.
/// </summary>
public class Level5PlayerDunkHostTests
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
    public void PlayerControllerImplementsIPlayerDunkHost()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();

        Assert.That(controller, Is.InstanceOf<IPlayerDunkHost>());
    }

    [Test]
    public void RigidBodyMapsDirectlyToTheExistingRigidBodyProperty()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        Rigidbody rigidBody = controller.gameObject.AddComponent<Rigidbody>();
        controller.RigidBody = rigidBody;
        IPlayerDunkHost host = controller;

        Assert.That(host.RigidBody, Is.SameAs(rigidBody));
    }

    [Test]
    public void BasketballRimVectorMapsToTheRimBoundThroughBindArenaContext()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        Vector3 rim = new Vector3(1f, 2f, 3f);
        controller.BindArenaContext(rim, null);
        IPlayerDunkHost host = controller;

        Assert.That(
            host.BasketballRimVector,
            Is.EqualTo(rim),
            "BasketballRimVector must expose the same finalized rim BindArenaContext stores, not a live GameLevelManager read");
    }

    [Test]
    public void CurrentStateMapsDirectlyToTheExistingCurrentStateProperty()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        controller.CurrentState = 42;
        IPlayerDunkHost host = controller;

        Assert.That(host.CurrentState, Is.EqualTo(42));
    }

    [Test]
    public void DunkStateHashMapsDirectlyToTheExistingDunkStateField()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        controller.dunkState = 7;
        IPlayerDunkHost host = controller;

        Assert.That(host.DunkStateHash, Is.EqualTo(7));
    }

    [Test]
    public void LockedRoundTripsThroughTheExistingLockedProperty()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        IPlayerDunkHost host = controller;

        host.Locked = true;

        Assert.That(controller.Locked, Is.True);

        controller.Locked = false;

        Assert.That(host.Locked, Is.False);
    }

    [Test]
    public void HasBasketballRoundTripsThroughTheExistingHasBasketballField()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        IPlayerDunkHost host = controller;

        host.HasBasketball = true;

        Assert.That(controller.hasBasketball, Is.True);

        controller.hasBasketball = false;

        Assert.That(host.HasBasketball, Is.False);
    }

    [Test]
    public void SetCallBallLockedForwardsToTheExistingCallBallToPlayerComponent()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        CallBallToPlayer callBallToPlayer = controller.gameObject.AddComponent<CallBallToPlayer>();
        controller.CallBallToPlayer = callBallToPlayer;
        IPlayerDunkHost host = controller;

        host.SetCallBallLocked(true);

        Assert.That(callBallToPlayer.Locked, Is.True);
    }

    // Representative action/animation forwarding path: FreezePosition/UnfreezePosition need only a
    // Rigidbody, unlike PlayAnimation/SetAnimationBool/FaceBasketballGoal which need an Animator and
    // (for FaceBasketballGoal) the controller's own movement-tracked relative-positioning state - out
    // of scope for a single representative check.
    [Test]
    public void FreezeAndUnfreezePositionForwardToTheExistingRigidbodyFreezeHelperCalls()
    {
        PlayerController controller = Spawn("controller").AddComponent<PlayerController>();
        Rigidbody rigidBody = controller.gameObject.AddComponent<Rigidbody>();
        controller.RigidBody = rigidBody;
        IPlayerDunkHost host = controller;

        host.FreezePosition();

        Assert.That(rigidBody.constraints, Is.EqualTo(
            RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ
            | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ));

        host.UnfreezePosition();

        Assert.That(rigidBody.constraints, Is.EqualTo(RigidbodyConstraints.FreezeRotation));
    }
}
