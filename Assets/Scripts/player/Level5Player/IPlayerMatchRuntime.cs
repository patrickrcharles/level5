using Level5.Core.Match;

/// <summary>
/// AUD-012 Phase 2b Slice 37: the narrow slice of <c>MatchRuntime</c> that <see cref="PlayerController"/>
/// needs, expressed as a contract so that controller can stop naming the static <c>MatchRuntime</c>
/// (still <c>Assembly-CSharp</c>) - its last direct edge into that assembly.
///
/// <c>GameLevelManager</c> implements this explicitly, forwarding every member straight to
/// <c>MatchRuntime</c> on each call - it is not a second owner of match state, not a configuration
/// object, and not a replacement for <c>MatchRuntime</c>. <c>MatchRuntime</c> stays the only place that
/// resolves a validated match configuration or reconstructs rules from the legacy globals for a
/// directly entered gameplay scene; this interface only lets <see cref="PlayerController"/> reach that
/// answer without naming the concrete type.
/// </summary>
public interface IPlayerMatchRuntime
{
    ResolvedMatchRules Rules { get; }

    bool CustomCamera { get; }

    /// <summary>
    /// AUD-012 Phase 2b: added for <c>AutoPlayerController</c>'s former direct
    /// <c>MatchRuntime.LevelHasSevenPointers</c> read (<c>cpuShootSevenpointers</c>). Human-only
    /// consumers of this interface simply never read it - the human path resolves seven-point
    /// eligibility through <see cref="Level5.Core.CpuSevenPointEligibility"/> from CPU-only code.
    /// </summary>
    bool LevelHasSevenPointers { get; }

    int LocalInputSlotFor(int playerId);
}
