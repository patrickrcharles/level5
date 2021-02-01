using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.database
{
    [Serializable]
    public class DBHighScoreModel 
    {
        public int Id;
        public int Userid;
        public int Modeid;
        public int Characterid ;
        public int Levelid ;
        public string Character ;
        public string Level ;
        public string Os ;
        public string Version ;
        public string Date ;
        public float Time ;
        public int TotalPoints ;
        public float LongestShot ;
        public float TotalDistance ;
        public int MaxShotMade ;
        public int MaxShotAtt ;
        public int ConsecutiveShots ;
        public int TrafficEnabled ;
        public int HardcoreEnabled ;
        public int EnemiesKilled ;
        public string Platform ;
        public string Device ;
        public string Ipaddress ;
        public string Scoreid ;
        public int TwoMade ;
        public int TwoAtt ;
        public int ThreeMade ;
        public int ThreeAtt ;
        public int FourMade ;
        public int FourAtt ;
        public int SevenMade ;
        public int SevenAtt ;

        public DBHighScoreModel convertBasketBallStatsToModel(BasketBallStats basketBallStats)
        {
            DBHighScoreModel highscore = new DBHighScoreModel();

            //highscore.Modeid = reader.GetInt32(2);
            //highscore.Characterid = reader.GetInt32(3);
            //highscore.Character = reader.GetString(4);
            //highscore.Levelid = reader.GetInt32(5);
            //highscore.Level = reader.GetString(6);
            //highscore.Os = reader.GetString(7);
            //highscore.Version = reader.GetString(8);
            //highscore.Date = reader.GetString(9);
            //highscore.Time = reader.GetFloat(10);
            //highscore.TotalPoints = reader.GetInt32(11);
            //highscore.LongestShot = reader.GetFloat(12);
            //highscore.TotalDistance = reader.GetFloat(13);
            //highscore.MaxShotMade = reader.GetInt32(14);
            //highscore.MaxShotAtt = reader.GetInt32(15);
            //highscore.ConsecutiveShots = reader.GetInt32(16);
            //highscore.TrafficEnabled = reader.GetInt32(17);
            //highscore.HardcoreEnabled = reader.GetInt32(18);
            //highscore.EnemiesKilled = reader.GetInt32(19);
            //highscore.Scoreid = reader.GetString(20);
            //highscore.Platform = reader.GetString(21);
            //highscore.Device = reader.GetString(22);
            //highscore.Ipaddress = reader.GetString(23);
            //highscore.TwoMade = reader.GetInt32(24);
            //highscore.TwoAtt = reader.GetInt32(25);
            //highscore.ThreeMade = reader.GetInt32(26);
            //highscore.ThreeAtt = reader.GetInt32(27);
            //highscore.FourMade = reader.GetInt32(28);
            //highscore.FourAtt = reader.GetInt32(29);
            //highscore.SevenMade = reader.GetInt32(30);
            //highscore.SevenAtt = reader.GetInt32(31);

            return highscore;
        }
    }

}
