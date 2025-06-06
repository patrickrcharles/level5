﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public static class AnaylticsManager
{

    public static void MenuStartLoaded()
    {
        string eventName = "Start Screen Loaded";
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
                {"player", GameOptions.characterDisplayName },
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
                {"player", GameOptions.characterDisplayName },
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
                {"player", GameOptions.characterDisplayName },
                {"slider value", sliderValue }
            }
            );
    }

    public static void PointsScoredEnemiesEnabled(GameStats basketBallStats)
    {
        string eventName = "points scored : enemies enabled";

        int accuracy = 0;
        if (basketBallStats.ShotAttempt != 0)
        {
            accuracy = (basketBallStats.ShotMade / basketBallStats.ShotAttempt);
        }
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName,
            new Dictionary<string, object>
            {
                {"game mode", GameOptions.gameModeSelectedName },
                {"player", GameOptions.characterDisplayName },
                {"cheerleader", GameOptions.cheerleaderDisplayName },
                {"points", basketBallStats.TotalPoints },
                {"accuracy", accuracy.ToString("##.####") },
                {"deviceType", SystemInfo.deviceType },
                {"deviceModel", SystemInfo.deviceModel },
                {"OperatingSystem", SystemInfo.operatingSystem }
            }
            );
    }
    public static void PointsScoredEnemiesDisabled(GameStats basketBallStats)
    {
        string eventName = "points scored : enemies disabled";
        AnalyticsResult analyticsResult =
            Analytics.CustomEvent(eventName,
            new Dictionary<string, object>
            {
                {"game mode", GameOptions.gameModeSelectedName },
                {"player", GameOptions.characterDisplayName },
                {"cheerleader", GameOptions.cheerleaderDisplayName },
                {"points", basketBallStats.TotalPoints },
                {"accuracy", (basketBallStats.ShotMade / basketBallStats.ShotAttempt).ToString("##.####") },
                {"deviceType", SystemInfo.deviceType },
                {"deviceModel", SystemInfo.deviceModel },
                {"OperatingSystem", SystemInfo.operatingSystem }
            }
            );
    }
}
