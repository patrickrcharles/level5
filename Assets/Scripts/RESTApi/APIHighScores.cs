
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

[Serializable]
public class APIHighScores 
{
    [JsonProperty("Id")]
    public int Id;
    [JsonProperty("UserId")]
    public int UserId;
    [JsonProperty("ModeId")]
    public int ModeId;

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
    }
}
