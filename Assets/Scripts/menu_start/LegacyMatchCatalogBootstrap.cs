using System.Collections.Generic;
using Level5.Core.Match;

/// <summary>
/// The composition seam between the legacy start-menu prefab data and <see cref="MatchCatalogs"/>.
///
/// <see cref="MatchCatalogs"/> moved into <c>Level5.Match</c> (AUD-012 Phase 2b Slice 20) and can no
/// longer reference <see cref="StartScreenModeSelected"/>/<see cref="LevelSelected"/> directly. This
/// type is what still can: it converts the legacy prefab components through the existing
/// <see cref="GameModeDefinitionFactory"/>/<see cref="LevelDefinitionFactory"/> and hands
/// <see cref="MatchCatalogs"/> only the resulting <c>Level5.Core.Match</c> definitions.
///
/// Conversion results are cached by the original source-list reference, mirroring the identity cache
/// <see cref="MatchCatalogs"/> used to keep itself before this split, so a menu that calls this every
/// highlighted frame does not reconvert the same prefab data each time. <see cref="MatchCatalogs"/> is
/// still called on every request - only the conversion work is skipped when the source has not changed.
/// </summary>
public static class LegacyMatchCatalogBootstrap
{
    private static IReadOnlyList<StartScreenModeSelected> lastModeSources;
    private static List<GameModeDefinition> cachedFallbackModes;
    private static List<string> cachedModeAnomalies;

    private static IReadOnlyList<LevelSelected> lastLevelSources;
    private static List<LevelDefinition> cachedFallbackLevels;

    /// <summary>
    /// Converts the legacy prefab data (reusing the previous conversion when the source lists are the
    /// same objects as last time) and builds the runtime catalogs from it.
    /// </summary>
    public static void EnsureBuilt(
        IReadOnlyList<StartScreenModeSelected> modeSources,
        IReadOnlyList<LevelSelected> levelSources)
    {
        if (cachedFallbackModes == null || !ReferenceEquals(lastModeSources, modeSources))
        {
            cachedModeAnomalies = new List<string>();
            cachedFallbackModes = GameModeDefinitionFactory.CreateAll(modeSources, cachedModeAnomalies);
            lastModeSources = modeSources;
        }

        if (cachedFallbackLevels == null || !ReferenceEquals(lastLevelSources, levelSources))
        {
            cachedFallbackLevels = LevelDefinitionFactory.CreateAll(levelSources);
            lastLevelSources = levelSources;
        }

        MatchCatalogs.EnsureBuilt(cachedFallbackModes, cachedFallbackLevels, cachedModeAnomalies);
    }

    /// <summary>
    /// Clears the conversion cache, for tests. Mirrors <see cref="MatchCatalogs.Reset"/> so this
    /// type's own state can be put back to "nothing converted yet" the same way every other static
    /// owner in this migration (<c>MatchCatalogs</c>, <c>ActiveMatch</c>, <c>VersusRuntime</c>,
    /// <c>VersusCatalogs</c>, <c>ActiveVersusAttempt</c>) already can. Does not touch
    /// <see cref="MatchCatalogs"/> itself - callers that also want the runtime catalogs cleared still
    /// call <see cref="MatchCatalogs.Reset"/> separately, as they already do.
    /// </summary>
    public static void Reset()
    {
        lastModeSources = null;
        cachedFallbackModes = null;
        cachedModeAnomalies = null;
        lastLevelSources = null;
        cachedFallbackLevels = null;
    }
}
