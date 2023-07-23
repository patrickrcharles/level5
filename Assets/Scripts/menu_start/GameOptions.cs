using System;
using System.Collections.Generic;

public static class GameOptions
{
    static public string applicationVersion;
    static public string operatingSystemVersion;

    static public int numPlayers;
    static public int numCpuPlayers;
    static public List<int> playerIds;

    // selected options
    static public string characterDisplayName;
    static public string cheerleaderDisplayName;
    static public int characterId;
    static public string levelSelected;
    static public int gameModeSelectedId;
    static public int levelId;
    static public int cheerleaderId;
    // object names
    static public string characterObjectName;
    public static List<String> characterObjectNames;
    static public string cheerleaderObjectName;
    static public string levelSelectedName;
    static public string levelDisplayName;
    static public string gameModeSelectedName;
    static public string cheerleaderSelectedName;

    // game mode flags for game rules
    static public bool gameModeHasBeenSelected;
    static public bool gameModeRequiresCounter;
    static public bool gameModeRequiresCountDown;
    // if game requires markers be active
    // 3 / 4 point contest / moneyball / etc
    static public bool gameModeRequiresShotMarkers3s;
    static public bool gameModeRequiresShotMarkers4s;
    // 3 / 4 point contest + trequires timer
    static public bool gameModeThreePointContest;
    static public bool gameModeFourPointContest;
    static public bool gameModeAllPointContest;
    // moneyball required
    static public bool gameModeRequiresMoneyBall;
    // requires consecutive shots
    static public bool gameModeRequiresConsecutiveShot;
    // custom timer used for 3 / 4 / all point contest, change from default of 120 to 80 / 160
    static public float customTimer;

    // start manager selected option indices
    // set default values = 0 (first element in list)
    // using values from game options, will load previous values on next load of start manager
    static public int playerSelectedIndex = 0;
    static public int levelSelectedIndex = 0;
    static public int modeSelectedIndex = 0;
    static public int cheerleaderSelectedIndex = 0;
    static public bool trafficEnabled = false;
    static public bool enemiesEnabled = false;

    static public bool sniperEnabled = false;
    static public bool sniperEnabledBullet = false;
    static public bool sniperEnabledBulletAuto = false;
    static public bool sniperEnabledLaser = false;

    static public int difficultySelected = 1;

    static public string previousSceneName;
    static public bool arcadeModeEnabled;

    static public bool hardcoreModeEnabled = false;
    static public bool EnemiesOnlyEnabled = false;

    static public bool levelRequiresTimeOfDay = true;
    static public bool levelRequiresWeather = false;

    static public string userName;
    static public int userid;
    static public string bearerToken;
    static public int numOfLocalUsers;

    static public bool tipDialogueLoadedOnStart;
    static public bool obstaclesEnabled;
    static public bool battleRoyalEnabled;
    static public bool gameModeRequiresPlayerSurvive;
    static public bool cageMatchEnabled;

    static public bool customCamera;
}
