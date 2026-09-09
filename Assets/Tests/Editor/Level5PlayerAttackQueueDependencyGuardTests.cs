using System.IO;
using NUnit.Framework;

/// <summary>
/// AUD-012 Phase 2b Slice 30 permanent guard: <c>PlayerAttackQueue</c> must carry no
/// <c>MatchRuntime</c> and no <c>PlayerIdentifier</c> dependency. It used to read
/// <c>MatchRuntime.Rules</c> for its queue-capacity/battle-royal policy and its own
/// <c>GetComponent&lt;PlayerIdentifier&gt;()</c> for the participant anchor position. Both now
/// arrive through <c>BindMatchContext</c>, composed by <c>SpawnCoordinator</c>.
///
/// <c>PlayerAttackQueue</c> itself still compiles into <c>Assembly-CSharp</c> (it also depends on
/// its same-assembly sibling <c>PlayerAttackPosition</c>), so it is not yet reached by
/// <see cref="Level5ProductionAssemblyBoundaryTests"/>'s migrated-assembly scan. This source-level
/// guard is the same shape as <see cref="Level5CharacterProfileDependencyGuardTests"/> and
/// <see cref="Level5PlayerControllerDependencyGuardTests"/>. Comments and string literals are
/// stripped first, so the doc comments that still explain what was removed do not pass as live
/// references.
/// </summary>
public class Level5PlayerAttackQueueDependencyGuardTests
{
    private static readonly string PlayerAttackQueuePath = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "PlayerAttackQueue.cs");

    [TestCase("MatchRuntime")]
    [TestCase("PlayerIdentifier")]
    public void PlayerAttackQueueHasNoGlobalMatchOrIdentifierReference(string type)
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerAttackQueuePath));

        Assert.That(
            text,
            Does.Not.Match($@"\b{type}\b"),
            $"PlayerAttackQueue must have zero {type} references - the resolved match rules and the "
            + "participant anchor must arrive through BindMatchContext, composed once by "
            + "SpawnCoordinator, not by reading MatchRuntime or resolving PlayerIdentifier here.");
    }
}
