using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.database
{
    [Serializable]
    public class DBHighScoreModel 
    {
        //public int Id;
        public int Userid;
        public string UserName;
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
        public int EnemiesEnabled ;
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
        public int BonusPoints;
        public int MoneyBallMade;
        public int MoneyBallAtt;

        public DBHighScoreModel convertBasketBallStatsToModel(BasketBallStats stats)
        {
            int trafficEnabled = 0;
            if (GameOptions.trafficEnabled)
            {
                trafficEnabled = 1;
            }
            int hardcoreEnabled = 0;
            if (GameOptions.hardcoreModeEnabled)
            {
                hardcoreEnabled = 1;
            }
            int enemiesEnabled = 0;
            if (GameOptions.enemiesEnabled)
            {
                enemiesEnabled = 1;
            }

            DBHighScoreModel model = new DBHighScoreModel();

            model.Scoreid = generateUniqueScoreID();
            model.Modeid = GameOptions.gameModeSelectedId;
            model.Characterid = GameOptions.characterId;
            model.Character = GameOptions.characterDisplayName;
            model.Levelid = GameOptions.levelId;
            model.Level = GameOptions.levelDisplayName;
            model.Os = SystemInfo.operatingSystem;
            model.Version = Application.version;
            model.Date = DateTime.Now.ToString();
            model.Time = stats.TimePlayed;
            model.TotalPoints = stats.TotalPoints;
            model.LongestShot = stats.LongestShotMade;
            model.TotalDistance = stats.TotalDistance;
            model.MaxShotMade = stats.ShotMade;
            model.MaxShotAtt = stats.ShotAttempt;
            model.ConsecutiveShots = stats.MostConsecutiveShots;
            model.TrafficEnabled = trafficEnabled;
            model.HardcoreEnabled = hardcoreEnabled;
            model.EnemiesKilled = stats.EnemiesKilled;
            model.Device = SystemInfo.deviceModel;
            model.Platform = SystemInfo.deviceType.ToString();
            model.Ipaddress = GetExternalIpAdress();
            model.TwoMade = stats.TwoPointerMade;
            model.TwoAtt = stats.TwoPointerAttempts;
            model.ThreeMade = stats.ThreePointerMade;
            model.ThreeAtt = stats.ThreePointerAttempts;
            model.FourMade = stats.FourPointerMade;
            model.FourAtt = stats.FourPointerAttempts;
            model.SevenMade = stats.SevenPointerMade;
            model.SevenAtt = stats.SevenPointerAttempts;
            model.BonusPoints = stats.BonusPoints;
            model.MoneyBallMade = stats.MoneyBallMade;
            model.MoneyBallAtt = stats.MoneyBallAttempts;
            model.EnemiesEnabled = enemiesEnabled;
            model.UserName = GameOptions.userName;
            model.Userid = GameOptions.userid;
            //Debug.Log(" model.UserName : " + model.UserName);

            return model;
        }

        public string GetExternalIpAdress()
        {
            string pubIp = new WebClient().DownloadString("https://api.ipify.org");
            return pubIp;
        }

        string generateUniqueScoreID()
        {
            string macAddress = "";
            string uniqueScoreId = "";
            string uniqueModeDateIdentifier = "";
            //TimeZone localZone = TimeZone.CurrentTimeZone;

            uniqueModeDateIdentifier = 
                  DateTime.Now.Day.ToString()
                + DateTime.Now.Month.ToString()
                + DateTime.Now.Year.ToString()
                + DateTime.Now.Second.ToString()
                + DateTime.Now.Millisecond.ToString();
            //Debug.Log("----- uniqueModeDateIdentifier : " + uniqueModeDateIdentifier);
            //uniqueModeDateIdentifier
            //    = RemoveWhitespace(date)
            //    + RemoveWhitespace(localZone.StandardName)
            //    + RemoveWhitespace(GameOptions.levelDisplayName)
            //    + RemoveWhitespace(GameOptions.playerDisplayName)
            //    + RemoveWhitespace(GameOptions.gameModeSelectedName);

            foreach (NetworkInterface ninf in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ninf.NetworkInterfaceType != NetworkInterfaceType.Ethernet) continue;
                if (ninf.OperationalStatus == OperationalStatus.Up)
                {
                    macAddress += ninf.GetPhysicalAddress().ToString();
                    break;
                }
            }
            uniqueScoreId = macAddress + uniqueModeDateIdentifier + SystemInfo.deviceUniqueIdentifier;

            return uniqueScoreId;
        }
    }

}
