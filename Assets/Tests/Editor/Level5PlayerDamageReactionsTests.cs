using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 32: <see cref="PlayerDamageReactions"/> now runs entirely against
/// <see cref="IPlayerDamageReactionHost"/>, so these coroutines can be driven directly against a fake
/// host - no <see cref="PlayerController"/>, scene, or Play Mode required. Each coroutine is stepped
/// deterministically by calling <see cref="IEnumerator.MoveNext"/> directly rather than through
/// Unity's coroutine scheduler: a bare <c>MoveNext()</c> call always runs the method body up to its
/// next <c>yield</c> regardless of what was yielded, so a <c>WaitUntil</c>/<c>WaitForSeconds</c> never
/// actually has to wait here - covering the same coroutine bodies in effectively zero wall-clock time.
///
/// Only one ordinary reaction (<see cref="PlayerKnockedDown"/>) and the changed camera seam
/// (<see cref="ShrinkPlayer"/>) are covered, per AUD-012 Phase 2b Slice 32's scope - the other
/// reactions are structurally identical (same freeze/animate/wait/restore shape against the same
/// host) and are not independently re-verified here.
/// </summary>
public class Level5PlayerDamageReactionsTests
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

    private FakeHost MakeHost()
    {
        GameObject actorGo = Spawn("reaction-actor");
        return new FakeHost
        {
            Anim = actorGo.AddComponent<Animator>(),
            RigidBody = actorGo.AddComponent<Rigidbody>(),
            ActorTransform = actorGo.transform,
            TakeDamageStateHash = 1,
            KnockedDownStateHash = 2,
            DisintegratedStateHash = 3,
            LightningStateHash = 4,
        };
    }

    private static void Drain(IEnumerator routine)
    {
        Assert.DoesNotThrow(() =>
        {
            while (routine.MoveNext())
            {
            }
        });
    }

    // ==================== representative ordinary reaction ====================

    [Test]
    public void PlayerKnockedDown_DrivesTheExpectedHostRigidbodyAndStateMutations()
    {
        FakeHost host = MakeHost();
        PlayerDamageReactions reactions = new PlayerDamageReactions(host);

        Drain(reactions.PlayerKnockedDown(1.5f));

        Assert.That(host.RigidBody.constraints, Is.EqualTo(RigidbodyConstraints.FreezeRotation),
            "the coroutine must end by relaxing the constraints it froze to start the reaction");
        Assert.That(host.KnockedDown, Is.False);
        Assert.That(host.TakeDamage, Is.False);
        Assert.That(host.Locked, Is.False);
    }

    // ==================== changed camera seam ====================

    [Test]
    public void ShrinkPlayer_HalvesScaleAndCameraFovThenRestoresExactly()
    {
        FakeHost host = MakeHost();
        host.ActorTransform.localScale = new Vector3(2f, 2f, 2f);
        Camera camera = Spawn("shrink-camera").AddComponent<Camera>();
        camera.fieldOfView = 61f;
        host.ShrinkCamera = camera;

        PlayerDamageReactions reactions = new PlayerDamageReactions(host);
        IEnumerator routine = reactions.ShrinkPlayer();

        Assert.That(routine.MoveNext(), Is.True, "step 1: isShrunk set, freeze, play lightning");
        Assert.That(host.IsShrunk, Is.True);

        Assert.That(routine.MoveNext(), Is.True, "step 2: nothing between the two lightning-state waits");

        Assert.That(routine.MoveNext(), Is.True, "step 3: knockdown/unfreeze, capture+halve scale and FOV");
        Assert.That(host.KnockedDown, Is.True);
        Assert.That(host.ActorTransform.localScale, Is.EqualTo(new Vector3(1f, 1f, 1f)));
        Assert.That(camera.fieldOfView, Is.EqualTo(30.5f).Within(0.001f));

        Assert.That(routine.MoveNext(), Is.False, "step 4: restore original scale/FOV, clear isShrunk");
        Assert.That(host.ActorTransform.localScale, Is.EqualTo(new Vector3(2f, 2f, 2f)),
            "the exact original scale must come back, not a hardcoded default");
        Assert.That(camera.fieldOfView, Is.EqualTo(61f).Within(0.001f),
            "AUD-054: the exact captured FOV must be restored, not a literal");
        Assert.That(host.IsShrunk, Is.False);
        Assert.That(host.FacingRight, Is.True, "restored scale.x > 0 means facing right");
    }

    [Test]
    public void ShrinkPlayer_NullCamera_StillPermitsThePlayerShrinkReactionAndSkipsOnlyTheFovChange()
    {
        FakeHost host = MakeHost();
        host.ActorTransform.localScale = Vector3.one;
        host.ShrinkCamera = null;

        PlayerDamageReactions reactions = new PlayerDamageReactions(host);

        Drain(reactions.ShrinkPlayer());

        Assert.That(host.IsShrunk, Is.False, "a missing camera must not abort the reaction");
        Assert.That(host.ActorTransform.localScale, Is.EqualTo(Vector3.one));
        Assert.That(host.KnockedDown, Is.True);
    }

    // ==================== test double ====================

    private sealed class FakeHost : IPlayerDamageReactionHost
    {
        public Animator Anim { get; set; }
        public Rigidbody RigidBody { get; set; }
        public Transform ActorTransform { get; set; }

        public int CurrentState { get; set; }
        public int TakeDamageStateHash { get; set; }
        public int KnockedDownStateHash { get; set; }
        public int DisintegratedStateHash { get; set; }
        public int LightningStateHash { get; set; }

        public bool TakeDamage { get; set; }
        public bool KnockedDown { get; set; }
        public bool Locked { get; set; }
        public bool AvoidedKnockDown { get; set; }
        public bool IsShrunk { get; set; }
        public bool FacingRight { get; set; }

        public Camera ShrinkCamera { get; set; }

        // MarkDead() itself is exercised by PlayerDisintegrated, which this file does not cover -
        // see the class doc comment. Level5PlayerDamageReactionHostTests covers MarkDead's actual
        // wiring against a real PlayerController/PlayerHealth instead of a fake here.
        public void MarkDead()
        {
        }

        public Camera GetShrinkCamera() => ShrinkCamera;
    }
}
