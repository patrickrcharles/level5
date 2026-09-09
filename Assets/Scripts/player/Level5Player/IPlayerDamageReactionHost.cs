using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 32: the narrow slice of a human player controller's state and actions that
/// <see cref="PlayerDamageReactions"/> needs to run its damage/knockdown/lightning/shrink reaction
/// coroutines, expressed as a contract so those coroutines can live in <c>Level5.Player</c> without
/// this assembly depending on the concrete <c>PlayerController</c> (still <c>Assembly-CSharp</c>).
///
/// Also removes the helper's other <c>Assembly-CSharp</c> edge, <c>CameraManager</c>: the shrink
/// reaction's camera lookup reaches this contract only through <see cref="GetShrinkCamera"/>, a live
/// call into a <c>Func&lt;Camera&gt;</c> composed on the <c>Assembly-CSharp</c> side
/// (<c>GameLevelManager.ReadPlayerDamageReactionCamera</c> -&gt;
/// <c>SpawnCoordinator.BindHumanDamageReactionCamera</c> -&gt;
/// <c>PlayerController.BindDamageReactionCameraReader</c>) rather than this contract or its
/// implementer ever naming <c>CameraManager</c> directly.
///
/// <c>PlayerController</c> implements this. Deliberately not a general actor-reaction or
/// human/CPU-shared abstraction - it exists only to name what this one helper reads and writes.
/// </summary>
public interface IPlayerDamageReactionHost
{
    Animator Anim { get; }
    Rigidbody RigidBody { get; }
    Transform ActorTransform { get; }

    int CurrentState { get; }
    int TakeDamageStateHash { get; }
    int KnockedDownStateHash { get; }
    int DisintegratedStateHash { get; }
    int LightningStateHash { get; }

    bool TakeDamage { get; set; }
    bool KnockedDown { get; set; }
    bool Locked { get; set; }
    bool AvoidedKnockDown { get; set; }
    bool IsShrunk { get; set; }
    bool FacingRight { get; set; }

    /// <summary>Marks the actor dead. Narrower than exposing the whole <c>PlayerHealth</c> component.</summary>
    void MarkDead();

    /// <summary>
    /// The camera the shrink reaction should halve/restore the field of view of, resolved live at
    /// call time (never cached) so the reaction sees whatever the composed resolver currently answers.
    /// Null when no resolver is bound or the resolver itself has nothing to offer - both are normal,
    /// not an error - the shrink reaction proceeds regardless and simply skips the FOV change.
    /// </summary>
    Camera GetShrinkCamera();
}
