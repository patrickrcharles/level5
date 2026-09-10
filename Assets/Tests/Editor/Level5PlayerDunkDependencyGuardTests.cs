using System.IO;
using NUnit.Framework;

/// <summary>
/// AUD-012 Phase 2b Slice 35 permanent guard: <c>PlayerDunk</c> must carry no <c>PlayerController</c>,
/// <c>PlayerIdentifier</c> or <c>GameLevelManager</c> dependency. It used to resolve
/// <c>GetComponent&lt;PlayerController&gt;()</c> (via a cached <c>PlayerIdentifier.playerController</c>
/// reference), <c>GetComponent&lt;PlayerIdentifier&gt;()</c> for the human basketball association, and
/// <c>GameLevelManager.instance.BasketballRimVector</c> for the rim-relative dunk-side decision. All
/// three now arrive through <see cref="IPlayerDunkHost"/> and
/// <see cref="IPlayerControllerParticipantState"/>, resolved from the same participant GameObject in
/// <c>Start()</c>.
///
/// AUD-012 Phase 2b Slice 36 moved <c>PlayerDunk</c> itself into <c>Level5.Player</c> (a pure
/// ownership move, once this dependency cut made it possible) - this source-level guard remains the
/// permanent invariant check, the same shape as
/// <see cref="Level5PlayerAttackQueueDependencyGuardTests"/> and
/// <see cref="Level5PlayerControllerDependencyGuardTests"/>. Comments and string literals are stripped
/// first, so the doc comments that still explain what was removed do not pass as live references.
/// </summary>
public class Level5PlayerDunkDependencyGuardTests
{
    private static readonly string PlayerDunkPath = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "Level5Player", "PlayerDunk.cs");

    [TestCase("PlayerController")]
    [TestCase("PlayerIdentifier")]
    [TestCase("GameLevelManager")]
    public void PlayerDunkHasNoAssemblyCSharpHostReference(string type)
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerDunkPath));

        Assert.That(
            text,
            Does.Not.Match($@"\b{type}\b"),
            $"PlayerDunk must have zero {type} references - controller state/actions must arrive "
            + "through IPlayerDunkHost, the human basketball association through "
            + "IPlayerControllerParticipantState, and the finalized arena rim through the host's "
            + "BasketballRimVector, not by reading PlayerController, PlayerIdentifier or "
            + "GameLevelManager directly.");
    }
}
