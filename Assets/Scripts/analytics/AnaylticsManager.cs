using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public static class AnaylticsManager
{

    public static void MenuStartLoaded()
    {
        string eventName = "Start SCreen Loaded";
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName);
    }

    public static void MenuStatsLoaded()
    {
        string eventName = "Stats Menu Loaded";
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName);
    }

    public static void MenuProgressionLoaded()
    {
        string eventName = "Progression Menu Loaded"; ;
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName,
            new Dictionary<string, object>
            {
                {"game mode", GameOptions.gameModeSelectedName },
                {"player", GameOptions.playerDisplayName },
                {"cheerleader", GameOptions.cheerleaderDisplayName },
                {"traffic", GameOptions.trafficEnabled },
                {"deviceType", SystemInfo.deviceType },
                {"deviceModel", SystemInfo.deviceModel },
                {"OperatingSystem", SystemInfo.operatingSystem }
            }
            );
    }

    public static void LevelLoaded(string levelName)
    {
        string eventName = levelName;
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName,
            new Dictionary<string, object>
            {
                {"game mode", GameOptions.gameModeSelectedName },
                {"player", GameOptions.playerDisplayName },
                {"cheerleader", GameOptions.cheerleaderDisplayName },
                {"traffic", GameOptions.trafficEnabled },
                {"deviceType", SystemInfo.deviceType },
                {"deviceModel", SystemInfo.deviceModel },
                {"OperatingSystem", SystemInfo.operatingSystem }
            }
            );
    }

    public static void PlayerShoot(float sliderValue)
    {
        string eventName = "player shoot";
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName,
            new Dictionary<string, object>
            {
                {"player", GameOptions.playerDisplayName },
                {"slider value", sliderValue }
            }
            );
    }
}
