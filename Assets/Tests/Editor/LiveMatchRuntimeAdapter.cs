using Level5.Core.Match;

/// <summary>
/// A live-forwarding <see cref="IPlayerMatchRuntime"/>, for tests that need to bind a
/// <c>PlayerController</c>'s match runtime without constructing <c>GameLevelManager</c> itself (whose
/// own <c>Awake()</c> does scene-dependent setup no EditMode fixture wants to trigger just to satisfy
/// this one seam).
///
/// Forwards every member straight to <see cref="MatchRuntime"/>, mirroring
/// <c>GameLevelManager</c>'s own explicit <see cref="IPlayerMatchRuntime"/> implementation
/// (AUD-012 Phase 2b Slice 37) - so a test using this sees exactly the answers production composition
/// would have supplied for whatever <c>ActiveMatch</c>/legacy-globals state is current.
/// </summary>
public sealed class LiveMatchRuntimeAdapter : IPlayerMatchRuntime
{
    public ResolvedMatchRules Rules => MatchRuntime.Rules;

    public bool CustomCamera => MatchRuntime.CustomCamera;

    public bool LevelHasSevenPointers => MatchRuntime.LevelHasSevenPointers;

    public int LocalInputSlotFor(int playerId) => MatchRuntime.LocalInputSlotFor(playerId);
}
