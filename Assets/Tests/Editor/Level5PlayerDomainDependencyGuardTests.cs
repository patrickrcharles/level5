using System.IO;
using NUnit.Framework;

/// <summary>
/// AUD-012 Phase 2b, Slice 50 permanent guards: the CPU actor/defense cluster, the collision
/// cluster, and the HUD/animation/cheerleader presentation cluster must carry no
/// <c>GameLevelManager</c>/<c>GameOptions</c>/<c>GameRules</c>/<c>MatchRuntime</c>/<c>ProjectilePool</c>/
/// <c>EnemyAttackBox</c> dependency. Each of these had a direct read replaced by explicit composition
/// (see <c>SpawnCoordinator</c>'s <c>BindCpu*</c>/<c>BindPlayerCollisionsContext</c>/
/// <c>BindAutoPlayerCollisionsContext</c>/<c>BindPlayerAnimationEventsContext</c> and
/// <c>GameLevelManager.BindPlayerHealthBarContext</c>) or a neutral contract
/// (<c>IAttackBoxHitInfo</c>, <c>Level5.Combat</c>). Mirrors the shape of
/// <see cref="Level5PlayerControllerDependencyGuardTests"/>, one file per fixture rather than one
/// combined regex, so a future regression names the exact file and the exact type it reintroduced.
///
/// Scoped to these files, deliberately - do not read a pass here as evidence that the wider player
/// prefab hierarchy is free of these types. <see cref="Level5ProductionAssemblyBoundaryTests.NoMigratedProductionAssemblyReachesIntoAssemblyCSharp"/>
/// is the comprehensive version of this same check, covering every file in every migrated assembly.
/// </summary>
public class Level5PlayerDomainDependencyGuardTests
{
    private static readonly string PlayerFolder = Path.Combine(
        Directory.GetCurrentDirectory(), "Assets", "Scripts", "player", "Level5Player");

    private static string Read(string fileName)
    {
        return Level5TestSourceText.StripCommentsAndLiterals(
            File.ReadAllText(Path.Combine(PlayerFolder, fileName)));
    }

    [Test]
    public void AutoPlayerControllerHasNoLegacyGlobalReferences()
    {
        string text = Read("AutoPlayerController.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "AutoPlayerController must have zero GameLevelManager references - arena context and the "
            + "participant registry must arrive through BindArenaContext/BindParticipantRegistry, bound "
            + "by SpawnCoordinator.BindCpuArenaContext/BindCpuParticipantRegistry.");
        Assert.That(text, Does.Not.Match(@"\bMatchRuntime\b"),
            "AutoPlayerController must have zero MatchRuntime references - match rules must arrive "
            + "through the bound IPlayerMatchRuntime (BindMatchRuntime), not a direct static read.");
    }

    [Test]
    public void AutoPlayerDefenseHasNoLegacyGlobalReferences()
    {
        string text = Read("AutoPlayerDefense.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "AutoPlayerDefense must have zero GameLevelManager references - the guarded-player "
            + "fallback, rim vector and ground-height fallback must arrive through "
            + "BindParticipantRegistry/BindArenaContext, bound by SpawnCoordinator.");
    }

    [Test]
    public void PlayerCollisionsHasNoLegacyGlobalReferences()
    {
        string text = Read("PlayerCollisions.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "PlayerCollisions must have zero GameLevelManager references - the fall-respawn "
            + "destination must arrive through BindFallRespawnDestination, and the dunk sequence must "
            + "run through this component's own resolved playerController, not a global Player1.");
        Assert.That(text, Does.Not.Match(@"\bGameRules\b"),
            "PlayerCollisions must have zero GameRules references - the killed-on-idle write must go "
            + "through the bound BindKilledOnIdleCallback delegate, not a direct GameRules.instance write.");
        Assert.That(text, Does.Not.Match(@"\bMatchRuntime\b"),
            "PlayerCollisions must have zero MatchRuntime references - match rules must arrive through "
            + "the bound ResolvedMatchRules (BindMatchRules), not a direct static read.");
        Assert.That(text, Does.Not.Match(@"\bEnemyAttackBox\b"),
            "PlayerCollisions must have zero EnemyAttackBox references - attack-box metadata must be "
            + "read through the neutral IAttackBoxHitInfo contract, not the concrete Level5.Enemy type.");
    }

    [Test]
    public void AutoPlayerCollisionsHasNoLegacyGlobalReferences()
    {
        string text = Read("AutoPlayerCollisions.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "AutoPlayerCollisions must have zero GameLevelManager references - the fall-respawn "
            + "destination must arrive through BindFallRespawnDestination.");
        Assert.That(text, Does.Not.Match(@"\bMatchRuntime\b"),
            "AutoPlayerCollisions must have zero MatchRuntime references - match rules must arrive "
            + "through the bound ResolvedMatchRules (BindMatchRules), not a direct static read.");
        Assert.That(text, Does.Not.Match(@"\bEnemyAttackBox\b"),
            "AutoPlayerCollisions must have zero EnemyAttackBox references - attack-box metadata must "
            + "be read through the neutral IAttackBoxHitInfo contract, not the concrete Level5.Enemy type.");
    }

    [Test]
    public void PlayerAnimationEventsHasNoLegacyGlobalReferences()
    {
        string text = Read("PlayerAnimationEvents.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "PlayerAnimationEvents must have zero GameLevelManager references - the has-auto-player "
            + "check must arrive through the bound BindHasAutoPlayerReader, and rigidbody-kinematic "
            + "toggling must use this component's own resolved playerController.RigidBody, not a "
            + "global Player1.");
        Assert.That(text, Does.Not.Match(@"\bProjectilePool\b"),
            "PlayerAnimationEvents must have zero ProjectilePool references - ProjectilePool is "
            + "Assembly-CSharp, not Level5.Pooling; projectile spawning must go through the bound "
            + "BindProjectileSpawner delegate.");
    }

    [Test]
    public void CheerleaderSwapAnimationHasNoGameLevelManagerReference()
    {
        string text = Read("CheerleaderSwapAnimation.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "CheerleaderSwapAnimation must have zero GameLevelManager references - the dev/editor "
            + "input-enabled check must read PlayerControlsProvider.Controls directly, the same "
            + "authoritative Level5.Input object GameLevelManager.Controls only ever forwarded.");
    }

    [Test]
    public void PlayerHealthBarHasNoLegacyGlobalReferences()
    {
        string text = Read("PlayerHealthBar.cs");

        Assert.That(text, Does.Not.Match(@"\bGameLevelManager\b"),
            "PlayerHealthBar must have zero GameLevelManager references - the tracked PlayerHealth, "
            + "character display name and damage-display-text reader must arrive through "
            + "BindPrimaryHumanContext, bound by GameLevelManager.BindPlayerHealthBarContext.");
        Assert.That(text, Does.Not.Match(@"\bMatchRuntime\b"),
            "PlayerHealthBar must have zero MatchRuntime references - match rules must arrive through "
            + "the bound ResolvedMatchRules (BindPrimaryHumanContext), not a direct static read.");
    }
}
