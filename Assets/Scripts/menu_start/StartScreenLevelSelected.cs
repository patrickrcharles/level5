using UnityEngine;

public class StartScreenLevelSelected : MonoBehaviour
{

    [SerializeField] private int levelId;
    [SerializeField] private string levelDisplayName;
    [SerializeField] private string levelObjectName;
    [SerializeField] private string levelDescription;
    [SerializeField] private bool levelRequiresTimeOfDay;
    [SerializeField] private bool levelHasTraffic;
    [SerializeField] private bool levelHasWeather;
    [SerializeField] private bool isFightingLevel;
    [SerializeField] private bool isShootingLevel;
    [SerializeField] private bool isBattleRoyalLevel;
    [SerializeField] private bool isCageMatchLevel;

    public string LevelDescription
    {
        get => levelDescription;
        set => levelDescription = value;
    }

    public int LevelId
    {
        get => levelId;
        set => levelId = value;
    }

    public string LevelDisplayName
    {
        get => levelDisplayName;
        set => levelDisplayName = value;
    }

    public string LevelObjectName
    {
        get => levelObjectName;
        set => levelObjectName = value;
    }
    public bool LevelRequiresTimeOfDay { get => levelRequiresTimeOfDay; set => levelRequiresTimeOfDay = value; }
    public bool LevelHasTraffic { get => levelHasTraffic; set => levelHasTraffic = value; }
    public bool LevelHasWeather { get => levelHasWeather; set => levelHasWeather = value; }
    public bool IsFightingLevel { get => isFightingLevel; }
    public bool IsShootingLevel { get => isShootingLevel;  }
    public bool IsBattleRoyalLevel { get => isBattleRoyalLevel; }
    public bool IsCageMatchLevel { get => isCageMatchLevel;  }
}
