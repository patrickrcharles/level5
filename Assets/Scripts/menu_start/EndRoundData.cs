using System.Collections.Generic;
using UnityEngine;

public static class EndRoundData 
{
    static public List<LevelSelected> levelsList;

    static public bool currentRoundWinnerIsCpu;
    static public bool currentRoundLoserIsCpu;

    static public int numberOfContinues = 3;
    static public int currentRoundWinnerScore;
    static public int currentRoundLoserScore;

    static public int currentLevelIndex;
    static public int nextLevelIndex;

    static public Sprite currentRoundPlayerWinnerImage;
    static public Sprite currentRoundPlayerLoserImage;
    static public Sprite currentRoundCpuWinnerImage;
    static public Sprite currentRoundCpuLoserImage;

    static public string nextRoundLevelName;
    static public string nextRoundOpponentName;
    static public string currentRoundLevelName;
    static public string currentRoundOpponentName;

}
