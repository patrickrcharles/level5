using System.Collections.Generic;
using Level5.Core.Match;
using UnityEngine;

/// <summary>
/// Where the game gets its mode and level catalogs.
///
/// Authored <see cref="GameModeDefinition"/> / <see cref="LevelDefinition"/> assets under
/// <c>Resources/Match</c> win when they exist. Until the editor migration has written them, the
/// catalogs are built from fallback definitions instead, so the new code path is live from the first
/// commit and cannot drift from the shipping data while it waits for assets. Converting the legacy
/// start-menu prefab components into those fallback definitions is <c>LegacyMatchCatalogBootstrap</c>'s
/// job (<c>Assembly-CSharp</c>, <c>Assets/Scripts/menu_start/</c>) - this type only decides authored
/// vs. fallback and owns the resulting catalogs, so it can compile without any legacy menu/loading type.
///
/// Everything is cached per source so a menu that asks on every highlighted button does not rebuild
/// definitions each frame.
/// </summary>
public static class MatchCatalogs
{
    public const string ModeResourcesPath = "Match/Modes";
    public const string LevelResourcesPath = "Match/Levels";

    private static GameModeCatalog cachedModes;
    private static LevelDefinitionCatalog cachedLevels;
    private static GameModeCompatibility cachedCompatibility;
    private static MatchConfigurationBuilder cachedBuilder;
    private static object modeSourceKey;
    private static object levelSourceKey;
    private static readonly List<string> conversionAnomalies = new List<string>();

    /// <summary>Anomalies found while converting legacy authored data. Empty after a clean load.</summary>
    public static IReadOnlyList<string> ConversionAnomalies => conversionAnomalies;

    /// <summary>True once both catalogs hold at least one entry.</summary>
    public static bool IsReady => cachedModes != null
        && cachedModes.Count > 0
        && cachedLevels != null
        && cachedLevels.Count > 0;

    public static GameModeCatalog Modes => cachedModes ?? GameModeCatalog.Empty();

    public static LevelDefinitionCatalog Levels => cachedLevels ?? LevelDefinitionCatalog.Empty();

    public static GameModeCompatibility Compatibility =>
        cachedCompatibility ??= new GameModeCompatibility(Modes, Levels);

    public static MatchConfigurationBuilder Builder =>
        cachedBuilder ??= new MatchConfigurationBuilder(Modes, Levels, Compatibility);

    /// <summary>
    /// Builds the catalogs from authored Resources, falling back to <paramref name="fallbackModes"/>/
    /// <paramref name="fallbackLevels"/> when no authored asset exists yet. Safe to call repeatedly: it
    /// rebuilds only when the fallback lists change identity, so a caller that keeps handing back the
    /// same converted definitions (see <c>LegacyMatchCatalogBootstrap</c>) does not force a rebuild.
    /// </summary>
    public static void EnsureBuilt(
        IReadOnlyList<GameModeDefinition> fallbackModes,
        IReadOnlyList<LevelDefinition> fallbackLevels,
        IReadOnlyList<string> fallbackModeAnomalies = null)
    {
        EnsureModes(fallbackModes, fallbackModeAnomalies);
        EnsureLevels(fallbackLevels);
    }

    /// <summary>Replaces the catalogs outright. For the editor migration and for tests.</summary>
    public static void Override(GameModeCatalog modes, LevelDefinitionCatalog levels)
    {
        cachedModes = modes;
        cachedLevels = levels;
        modeSourceKey = null;
        levelSourceKey = null;
        cachedCompatibility = null;
        cachedBuilder = null;
    }

    public static void Reset()
    {
        Override(null, null);
        conversionAnomalies.Clear();
    }

    private static void EnsureModes(IReadOnlyList<GameModeDefinition> fallbackModes, IReadOnlyList<string> fallbackModeAnomalies)
    {
        if (cachedModes != null && ReferenceEquals(modeSourceKey, fallbackModes))
        {
            return;
        }

        List<GameModeDefinition> definitions = LoadAuthoredModes();
        if (definitions.Count == 0)
        {
            conversionAnomalies.Clear();
            if (fallbackModeAnomalies != null)
            {
                conversionAnomalies.AddRange(fallbackModeAnomalies);
            }

            definitions = fallbackModes != null ? new List<GameModeDefinition>(fallbackModes) : new List<GameModeDefinition>();
        }

        cachedModes = new GameModeCatalog(definitions);
        modeSourceKey = fallbackModes;
        cachedCompatibility = null;
        cachedBuilder = null;
        ReportProblems("game mode", cachedModes.Problems);
        ReportProblems("game mode", conversionAnomalies);
    }

    private static void EnsureLevels(IReadOnlyList<LevelDefinition> fallbackLevels)
    {
        if (cachedLevels != null && ReferenceEquals(levelSourceKey, fallbackLevels))
        {
            return;
        }

        List<LevelDefinition> definitions = LoadAuthoredLevels();
        if (definitions.Count == 0)
        {
            definitions = fallbackLevels != null ? new List<LevelDefinition>(fallbackLevels) : new List<LevelDefinition>();
        }

        cachedLevels = new LevelDefinitionCatalog(definitions);
        levelSourceKey = fallbackLevels;
        cachedCompatibility = null;
        cachedBuilder = null;
        ReportProblems("level", cachedLevels.Problems);
    }

    private static List<GameModeDefinition> LoadAuthoredModes()
    {
        GameModeDefinition[] assets = Resources.LoadAll<GameModeDefinition>(ModeResourcesPath);
        return assets == null ? new List<GameModeDefinition>() : new List<GameModeDefinition>(assets);
    }

    private static List<LevelDefinition> LoadAuthoredLevels()
    {
        LevelDefinition[] assets = Resources.LoadAll<LevelDefinition>(LevelResourcesPath);
        return assets == null ? new List<LevelDefinition>() : new List<LevelDefinition>(assets);
    }

    private static void ReportProblems(string what, IReadOnlyList<string> problems)
    {
        if (problems == null)
        {
            return;
        }

        foreach (string problem in problems)
        {
            Debug.LogError($"Level 5 {what} catalog: {problem}");
        }
    }
}
