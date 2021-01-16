
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class APIHighScores 
{
    public List<HighScore> HighScores;

    [Serializable]
    public class HighScore
    {
        [JsonProperty("Id")]
        public int Id;
        [JsonProperty("UserId")]
        public int UserId;
        [JsonProperty("ModeId")]
        public int ModeId;
        [JsonProperty("CharacterId")]
        public int CharacterId;
        [JsonProperty("LevelId")]
        public int LevelId;
        [JsonProperty("Character")]
        public string Character;
        [JsonProperty("Level")]
        public string Level;
        [JsonProperty("Os")]
        public string Os;
        [JsonProperty("Version")]
        public string Version;
        [JsonProperty("Date")]
        public string Date;
        [JsonProperty("Time")]
        public float Time;
        [JsonProperty("TotalPoints")]
        public int TotalPoints;
        [JsonProperty("LongestShot")]
        public float LongestShot;
        [JsonProperty("TotalDistance")]
        public float TotalDistance;
        [JsonProperty("MaxShotMade")]
        public int MaxShotMade;
        [JsonProperty("MaxShotAtt")]
        public int MaxShotAtt;
        [JsonProperty("ConsecutiveShots")]
        public int ConsecutiveShots;
        [JsonProperty("TrafficEnabled")]
        public int TrafficEnabled;
        [JsonProperty("HardcoreEnabled")]
        public int HardcoreEnabled;
        [JsonProperty("EnemiesKilled")]
        public int EnemiesKilled;
        [JsonProperty("Platform")]
        public string Platform;
        [JsonProperty("Device")]
        public string Device;
    }
}
