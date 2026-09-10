using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 35: the narrow slice of a human player controller's state and actions that
/// <c>PlayerDunk</c> needs to run its dunk-decision, ballistic-launch and dunk-sequence logic,
/// expressed as a contract so <c>PlayerDunk</c> no longer names the concrete <c>PlayerController</c>.
/// Mirrors the dependency-cut shape of <see cref="IPlayerDamageReactionHost"/>/
/// <see cref="IPlayerIdleSniperRuntime"/>/<see cref="IPlayerControllerParticipantState"/>: a live view
/// over existing controller state, not a new owner of it.
///
/// <c>PlayerController</c> implements this. Deliberately not a general actor/host abstraction - it
/// exists only to name what <c>PlayerDunk</c> reads and writes. <c>PlayerDunk</c> itself stays in
/// <c>Assembly-CSharp</c> this slice; only its dependency edges are cut (AUD-012 Phase 2b Slice 35 -
/// dependency-preparation only, no ownership move).
/// </summary>
public interface IPlayerDunkHost
{
    Rigidbody RigidBody { get; }

    Vector3 BasketballRimVector { get; }

    int CurrentState { get; }

    int DunkStateHash { get; }

    bool Locked { get; set; }

    bool HasBasketball { get; set; }

    void SetCallBallLocked(bool locked);

    void FaceBasketballGoal();

    void PlayAnimation(string animationName);

    void SetAnimationBool(string parameterName, bool value);

    void FreezePosition();

    void UnfreezePosition();
}
