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
///
/// AUD-012 Phase 2b Slice 32 adds a second, narrower guard: <c>PlayerController</c> must also carry no
/// live <c>CameraManager</c> reference. Migrating <c>PlayerDamageReactions</c> into <c>Level5.Player</c>
/// removed that helper's direct <c>CameraManager.instance.Cameras[0]</c> read; the fix would be
/// undone in spirit (a blocker swapped for a blocker) if <c>PlayerController</c> picked up the same
/// read as a replacement instead of keeping it on the <c>Assembly-CSharp</c> composition side
/// (<c>GameLevelManager.ReadPlayerDamageReactionCamera</c> -&gt;
/// <c>SpawnCoordinator.BindHumanDamageReactionCamera</c> -&gt;
/// <c>PlayerController.BindDamageReactionCameraReader</c>).
///
/// AUD-012 Phase 2b Slice 33 adds a third guard: <c>PlayerController</c> must also carry no live
/// <c>SniperManager</c> reference. The idle-sniper check's former direct
/// <c>SniperManager.instance</c> read is now supplied by explicit composition
/// (<c>GameLevelManager.ReadPlayerIdleSniperRuntime</c> -&gt;
/// <c>SpawnCoordinator.BindHumanIdleSniperRuntime</c> -&gt;
/// <c>PlayerController.BindIdleSniperRuntimeReader</c> -&gt; <c>IPlayerIdleSniperRuntime</c>). Scoped
/// to this one file, same caveat as the other two guards above - <c>SniperManager</c> itself is
/// untouched and stays in <c>Assembly-CSharp</c>.
/// </summary>
public class Level5PlayerControllerDependencyGuardTests
{
    private static readonly string PlayerControllerPath = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "PlayerController.cs");

    [Test]
    public void PlayerControllerHasNoGameLevelManagerReference()
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerControllerPath));

        Assert.That(
            text,
            Does.Not.Match(@"\bGameLevelManager\b"),
            "PlayerController must have zero GameLevelManager references - arena spatial context "
            + "(basketball rim vector, ground-height provider) must arrive through "
            + "BindArenaContext(Vector3, IGroundHeightProvider), bound once by "
            + "SpawnCoordinator.BindHumanArenaContext after GameLevelManager.Start() resolves the "
            + "final rim, not by reading GameLevelManager.instance directly.");
    }

    [Test]
    public void PlayerControllerHasNoCameraManagerReference()
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerControllerPath));

        Assert.That(
            text,
            Does.Not.Match(@"\bCameraManager\b"),
            "PlayerController must have zero CameraManager references - the shrink reaction's camera "
            + "must arrive through BindDamageReactionCameraReader(Func<Camera>), bound once by "
            + "SpawnCoordinator.BindHumanDamageReactionCamera from GameLevelManager.Awake, not by "
            + "reading CameraManager.instance directly on this controller.");
    }

    [Test]
    public void PlayerControllerHasNoSniperManagerReference()
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerControllerPath));

        Assert.That(
            text,
            Does.Not.Match(@"\bSniperManager\b"),
            "PlayerController must have zero SniperManager references - the idle-sniper runtime must "
            + "arrive through BindIdleSniperRuntimeReader(Func<IPlayerIdleSniperRuntime>), bound once "
            + "by SpawnCoordinator.BindHumanIdleSniperRuntime from GameLevelManager.Awake, not by "
            + "reading SniperManager.instance directly on this controller.");
    }
}
