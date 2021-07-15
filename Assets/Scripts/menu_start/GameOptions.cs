using System;

public static class GameOptions
{
    static public string applicationVersion;
    static public string operatingSystemVersion;

    // selected options
    static public String characterDisplayName;
    static public String cheerleaderDisplayName;
    static public int characterId;
    static public String levelSelected;
    static public int gameModeSelectedId;
    static public int levelId;
    static public int cheerleaderId;
    // object names
    static public String characterObjectName;
    static public String cheerleaderObjectName;
    static public String levelSelectedName;
    static public String levelDisplayName;
    static public String gameModeSelectedName;
    static public String cheerleaderSelectedName;
    //player stats
    static public float accuracy2pt;
    static public float accuracy3pt;
    static public float accuracy4pt;
    static public float accuracy7pt;
    static public float jumpForce;
    static public float speed;
    static public float runSpeed;
    static public float runSpeedHasBall;
    // static public float range;
    // static public float release;
    static public int luck;
    static public int shootAngle;

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

    static public string previousSceneName;
    static public bool arcadeModeEnabled;

    static public bool architectureIs64bit;
    static public bool architectureIs32bit;
    static public bool architectureIsAndroid;

    static public bool architectureInfoLoaded = false;
    static public bool hardcoreModeEnabled = false;
    static public bool EnemiesOnlyEnabled = false;


    static public bool levelRequiresTimeOfDay = true;
    static public bool levelRequiresWeather = false;

    static public string userName;
    static public int userid;
    static public string bearerToken;
    static public int numOfLocalUsers;

    static public bool tipDialogueLoadedOnStart;
}
