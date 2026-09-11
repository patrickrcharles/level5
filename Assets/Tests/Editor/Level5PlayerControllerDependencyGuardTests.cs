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
/// hierarchy still read that singleton for the same arena values (<c>AutoPlayerDefense</c> and
/// <c>AutoPlayerController</c> for both the rim vector and <c>TerrainHeight</c>), and are out of scope
/// for this slice - <c>PlayerDunk</c> itself was dependency-closed of this edge by Slice 35's
/// <c>IPlayerDunkHost</c> contract and no longer reads <c>GameLevelManager</c>. Same caveat shape
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
///
/// AUD-012 Phase 2b Slice 34 adds a fourth guard: <c>PlayerController</c> must also carry no live
/// <c>PlayerIdentifier</c> reference. Its two former direct reads -
/// <c>GetComponent&lt;PlayerIdentifier&gt;().pid</c>/<c>.isCpu</c> in <c>InitializeInput()</c> and
/// <c>GetComponent&lt;PlayerIdentifier&gt;().basketball</c> in <c>Start()</c> - are now supplied by
/// <c>GetComponent&lt;IPlayerControllerParticipantState&gt;()</c>, which <c>PlayerIdentifier</c>
/// implements explicitly. Same caveat as the other guards above, sharpened for this one:
/// <c>PlayerIdentifier</c> itself is untouched, stays in <c>Assembly-CSharp</c>, and remains the
/// authoritative owner of participant id, CPU status, and human basketball association - this guard
/// only proves <c>PlayerController</c> no longer names the concrete type.
///
/// AUD-012 Phase 2b Slice 37 adds a fifth, final guard: <c>PlayerController</c> must also carry no live
/// <c>MatchRuntime</c> reference - its last direct <c>Assembly-CSharp</c> dependency. Its three former
/// direct reads - <c>MatchRuntime.Rules</c>, <c>MatchRuntime.CustomCamera</c> and
/// <c>MatchRuntime.LocalInputSlotFor(playerId)</c> - are now supplied by the explicitly bound
/// <c>IPlayerMatchRuntime</c> (<c>SpawnCoordinator.BindHumanMatchRuntime</c> -&gt;
/// <c>PlayerController.BindMatchRuntime</c>), which <c>GameLevelManager</c> implements as a live
/// forwarding boundary over <c>MatchRuntime</c>. Same caveat as the other guards above:
/// <c>MatchRuntime</c> itself is untouched and stays in <c>Assembly-CSharp</c>; this guard only proves
/// <c>PlayerController</c> no longer names it directly. With this guard passing, a fresh dependency
/// closure scan finds zero live <c>Assembly-CSharp</c> types on <c>PlayerController</c>.
///
/// AUD-012 Phase 2b Slice 38 moved <c>PlayerController</c> itself into <c>Level5.Player</c> (a pure
/// ownership move, once Slice 37 made it dependency-closed) - this source-level guard remains the
/// permanent invariant check, the same shape as <see cref="Level5PlayerDunkDependencyGuardTests"/>.
/// </summary>
public class Level5PlayerControllerDependencyGuardTests
{
    private static readonly string PlayerControllerPath = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "Level5Player", "PlayerController.cs");

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

    [Test]
    public void PlayerControllerHasNoPlayerIdentifierReference()
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerControllerPath));

        Assert.That(
            text,
            Does.Not.Match(@"\bPlayerIdentifier\b"),
            "PlayerController must have zero PlayerIdentifier references - participant id, CPU status "
            + "and the human basketball association must arrive through "
            + "GetComponent<IPlayerControllerParticipantState>(), which PlayerIdentifier implements "
            + "explicitly, not by reading PlayerIdentifier directly on this controller.");
    }

    [Test]
    public void PlayerControllerHasNoMatchRuntimeReference()
    {
        string text = Level5TestSourceText.StripCommentsAndLiterals(File.ReadAllText(PlayerControllerPath));

        Assert.That(
            text,
            Does.Not.Match(@"\bMatchRuntime\b"),
            "PlayerController must have zero MatchRuntime references - rules, custom-camera and local "
            + "input-slot resolution must arrive through the bound IPlayerMatchRuntime "
            + "(BindMatchRuntime), supplied by SpawnCoordinator.BindHumanMatchRuntime from "
            + "GameLevelManager.Awake, not by reading MatchRuntime directly on this controller.");
    }
}
