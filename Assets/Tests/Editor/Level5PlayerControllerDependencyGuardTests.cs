using System.IO;
using NUnit.Framework;

/// <summary>
/// AUD-012 Phase 2b Slice 21 permanent guard: <c>PlayerController</c> must carry no
/// <c>GameLevelManager</c> dependency. Its two former direct reads -
/// <c>GameLevelManager.instance.BasketballRimVector</c> and
/// <c>GameLevelManager.instance.TerrainHeight</c> - are now supplied by explicit composition
/// (<c>SpawnCoordinator.BindHumanArenaContext</c> -&gt; <c>PlayerController.BindArenaContext</c>),
/// called once from <c>GameLevelManager.Start()</c> after arena bootstrap resolves the final rim.
/// This fails the build if a future change reintroduces a direct <c>GameLevelManager</c> read on this
/// type instead of using the bound rim vector / <c>IGroundHeightProvider</c>. Mirrors
/// <see cref="Level5BasketBallDependencyGuardTests"/> and
/// <see cref="Level5BasketballGameManagerEdgeTests"/>.
///
/// Scoped to this one file, deliberately - do not read a pass here as evidence that the player
/// <i>prefab</i> no longer depends on <c>GameLevelManager</c>. Sibling components on the same player
/// hierarchy still read that singleton for the same arena values (<c>PlayerDunk</c> for the rim
/// vector; <c>AutoPlayerDefense</c> and <c>AutoPlayerController</c> for both the rim vector and
/// <c>TerrainHeight</c>), and are out of scope for this slice. Same caveat shape
/// <see cref="Level5BasketballGameManagerEdgeTests"/> already records for the unresolved
/// basketball -&gt; <c>GameRules</c> edge.
/// </summary>
public class Level5PlayerControllerDependencyGuardTests
{
    private static readonly string PlayerControllerPath = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "PlayerController.cs");

    [Test]
    public void PlayerControllerHasNoGameLevelManagerReference()
    {
        string text = Level5TestSourceText.StripComments(File.ReadAllText(PlayerControllerPath));

        Assert.That(
            text,
            Does.Not.Match(@"\bGameLevelManager\b"),
            "PlayerController must have zero GameLevelManager references - arena spatial context "
            + "(basketball rim vector, ground-height provider) must arrive through "
            + "BindArenaContext(Vector3, IGroundHeightProvider), bound once by "
            + "SpawnCoordinator.BindHumanArenaContext after GameLevelManager.Start() resolves the "
            + "final rim, not by reading GameLevelManager.instance directly.");
    }
}
