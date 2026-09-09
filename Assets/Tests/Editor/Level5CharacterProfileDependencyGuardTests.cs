using System.IO;
using NUnit.Framework;

/// <summary>
/// AUD-012 Phase 2b Slice 27 permanent guard: <c>CharacterProfile</c> must carry no
/// <c>MatchRuntime</c> and no <c>LoadedData</c> dependency. It used to read
/// <c>MatchRuntime.Rules</c>/<c>MatchRuntime.Cheerleader</c> for its Arcade/easy override, its
/// cheerleader bonuses and its contest Luck/Clutch suppression, and
/// <c>LoadedData.instance.getSelectedCharacterProfile(int)</c> for the saved profile a human is
/// rebuilt from. All four now arrive through <c>PrepareHumanMatchContext</c> /
/// <c>PrepareCpuMatchContext</c>, composed by <c>SpawnCoordinator</c>.
///
/// This type still compiles into <c>Assembly-CSharp</c>, so
/// <see cref="Level5ProductionAssemblyBoundaryTests"/>'s migrated-assembly scan cannot enforce this
/// boundary yet - which is exactly why the source-level guard exists now, in the same shape as
/// <see cref="Level5BasketBallStateDependencyGuardTests"/> and
/// <see cref="Level5PlayerControllerDependencyGuardTests"/>. Comments and string literals are
/// stripped first, so the doc comments that still explain what was removed do not pass as live
/// references.
/// </summary>
public class Level5CharacterProfileDependencyGuardTests
{
    private static readonly string CharacterProfilePath = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "CharacterProfile.cs");

    [TestCase("MatchRuntime")]
    [TestCase("LoadedData")]
    public void CharacterProfileHasNoGlobalMatchOrPersistenceReference(string type)
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(CharacterProfilePath));

        Assert.That(
            text,
            Does.Not.Match($@"\b{type}\b"),
            $"CharacterProfile must have zero {type} references - the saved profile resolver, the "
            + "cheerleader selection and the resolved match rules must arrive through "
            + "PrepareHumanMatchContext / PrepareCpuMatchContext, composed once by SpawnCoordinator, "
            + "not by discovering match or persistence singletons here.");
    }
}
