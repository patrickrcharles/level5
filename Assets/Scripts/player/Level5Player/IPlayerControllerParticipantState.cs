using UnityEngine;

/// <summary>
/// AUD-012 Phase 2b Slice 34: the narrow slice of a human participant's identity `PlayerController`
/// reads - id, CPU status, and the human basketball association - expressed as a contract so this
/// assembly does not depend on the concrete `PlayerIdentifier` (still `Assembly-CSharp`). Mirrors the
/// dependency-cut shape of <see cref="IPlayerDamageReactionHost"/>/<see cref="IPlayerIdleSniperRuntime"/>:
/// a read-only live view over existing state, not a new owner of it.
///
/// `PlayerIdentifier` implements this explicitly. `BasketballObject` returns the human `basketball`
/// field specifically - never `autoBasketball` - because that is the only value `PlayerController`'s
/// `Start()` ever consumed.
/// </summary>
public interface IPlayerControllerParticipantState
{
    int PlayerId { get; }

    bool IsCpu { get; }

    GameObject BasketballObject { get; }
}
