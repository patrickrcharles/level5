using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using UnityEngine;
using Random = System.Random;

namespace Assets.Scripts.database
{
    [Serializable]
    public class HighScoreModel
    {
        //public int Id;
        public int Userid;
        public string UserName;
        public int Modeid;
        public int Characterid;
        public int Levelid;
        public string Character;
        public string Level;
        public string Os;
        public string Version;
        public string Date;
        public float Time;
        public int TotalPoints;
        public float LongestShot;
        public float TotalDistance;
        public int MaxShotMade;
        public int MaxShotAtt;
        public int ConsecutiveShots;
        public int TrafficEnabled;
        public int HardcoreEnabled;
        public int EnemiesEnabled;
        public int EnemiesKilled;
        public string Platform;
        public string Device;
        public string Ipaddress;
        public string Scoreid;
        public int TwoMade;
        public int TwoAtt;
        public int ThreeMade;
        public int ThreeAtt;
        public int FourMade;
        public int FourAtt;
        public int SevenMade;
        public int SevenAtt;
        public int BonusPoints;
        public int MoneyBallMade;
        public int MoneyBallAtt;
        public int SniperEnabled;
        public int SniperMode;
        public string SniperModeName;
        public int SniperShots;
        public int Sniperhits;

        public HighScoreModel convertBasketBallStatsToModel(GameStats stats)
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
            int sniperEnabled = 0;
            if (GameOptions.sniperEnabled)
            {
                sniperEnabled = 1;
            }

            HighScoreModel model = new HighScoreModel();

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
            if (IsConnectedToInternet())
            {
                model.Ipaddress = GetExternalIpAdress();
            }
            else
            {
                model.Ipaddress = "noConnectivity" + RandomString(8);
            }
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
            model.SniperEnabled = sniperEnabled;
            if (!GameOptions.sniperEnabled)
            {
                model.SniperMode = 0;
                model.SniperModeName = "none";
            }
            if (GameOptions.sniperEnabledBullet && GameOptions.sniperEnabled)
            {
                model.SniperMode = 1;
                model.SniperModeName = "single bullet";
            }
            if (GameOptions.sniperEnabledBulletAuto && GameOptions.sniperEnabled)
            {
                model.SniperMode = 2;
                model.SniperModeName = "machine gun ";
            }
            if (GameOptions.sniperEnabledLaser && GameOptions.sniperEnabled)
            {
                model.SniperMode = 3;
                model.SniperModeName = "disintegration ray";
            }
            model.SniperShots = stats.SniperShots;
            model.Sniperhits = stats.SniperHits;

            return model;
        }

        public string GetExternalIpAdress()
        {
            //string pubIp = new WebClient().DownloadString("https://api.ipify.org");
            //return pubIp;

            // External IP Address (get your external IP locally)  
            //UTF8Encoding utf8 = new UTF8Encoding();
            //WebClient webClient = new WebClient();
            //String externalIp = utf8.GetString(webClient.DownloadData(
            //"http://whatismyip.com/automation/n09230945.asp"));

            try
            {
                string externalIP;
                externalIP = (new WebClient()).DownloadString("http://checkip.dyndns.org/");
                externalIP = (new Regex(@"\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}"))
                             .Matches(externalIP)[0].ToString();
                return externalIP;
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return null;
            }

            //return externalIp;
        }

        public bool IsConnectedToInternet()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return false;
            }
        }

        private string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
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
                + DateTime.Now.Minute.ToString()
                + Utility.UtilityFunctions.RandomString(8);

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
