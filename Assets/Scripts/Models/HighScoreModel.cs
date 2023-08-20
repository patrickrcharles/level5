using System;
using System.Collections.Generic;
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
        public int Difficulty;
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
        // add versus mode stats to save
        public int p1TotalPoints;
        public int p2TotalPoints;
        public int p3TotalPoints;
        public int p4TotalPoints;
        public string firstPlace;
        public string secondPlace;
        public string thirdPlace;
        public string fourthPlace;
        public int p1IsCpu;
        public int p2IsCpu;
        public int p3IsCpu;
        public int p4IsCpu;
        public int numPlayers;

        public HighScoreModel convertBasketBallStatsToModel(List<PlayerIdentifier> pi)
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
            model.Time = pi[0].gameStats.TimePlayed;
            model.Difficulty = GameOptions.difficultySelected;
            model.TotalPoints = pi[0].gameStats.TotalPoints;
            model.LongestShot = pi[0].gameStats.LongestShotMade;
            model.TotalDistance = pi[0].gameStats.TotalDistance;
            model.MaxShotMade = pi[0].gameStats.ShotMade;
            model.MaxShotAtt = pi[0].gameStats.ShotAttempt;
            model.ConsecutiveShots = pi[0].gameStats.MostConsecutiveShots;
            model.TrafficEnabled = trafficEnabled;
            model.HardcoreEnabled = hardcoreEnabled;
            model.EnemiesKilled = pi[0].gameStats.EnemiesKilled;
            model.Device = SystemInfo.deviceModel;
            model.Platform = SystemInfo.deviceType.ToString();
            //if (IsConnectedToInternet())
            //{
            //    model.Ipaddress = GetExternalIpAdress();
            //}
            //else
            //{
            //    model.Ipaddress = "noConnectivity" + RandomString(8);
            //}
            model.Ipaddress = GetExternalIpAdress();
            model.TwoMade = pi[0].gameStats.TwoPointerMade;
            model.TwoAtt = pi[0].gameStats.TwoPointerAttempts;
            model.ThreeMade = pi[0].gameStats.ThreePointerMade;
            model.ThreeAtt = pi[0].gameStats.ThreePointerAttempts;
            model.FourMade = pi[0].gameStats.FourPointerMade;
            model.FourAtt = pi[0].gameStats.FourPointerAttempts;
            model.SevenMade = pi[0].gameStats.SevenPointerMade;
            model.SevenAtt = pi[0].gameStats.SevenPointerAttempts;
            model.BonusPoints = pi[0].gameStats.BonusPoints;
            model.MoneyBallMade = pi[0].gameStats.MoneyBallMade;
            model.MoneyBallAtt = pi[0].gameStats.MoneyBallAttempts;
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
            model.SniperShots = pi[0].gameStats.SniperShots;
            model.Sniperhits = pi[0].gameStats.SniperHits;
            Debug.Log("GameOptions.numPlayers : " + GameOptions.numPlayers);
            Debug.Log("pi[0].isCpu : " + pi[0].isCpu);
            Debug.Log("pi[0]. : " + pi[0].characterProfile.PlayerDisplayName);
            Debug.Log("pi[1]. : " + pi[1].characterProfile.PlayerDisplayName);

            if (GameOptions.numPlayers > 0)
            {
                model.p1TotalPoints = pi[0].gameStats.TotalPoints;
                model.firstPlace = pi[0].characterProfile.PlayerDisplayName;
                if (pi[0].isCpu) { model.p1IsCpu = 1; }
                else { model.p1IsCpu = 0; }
            }
            else
            {
                model.p1TotalPoints = 0;
                model.firstPlace = "";
                model.p1IsCpu = 99;
            }
            //player2
            if (GameOptions.numPlayers > 1)
            {
                model.p2TotalPoints = pi[1].gameStats.TotalPoints;
                model.secondPlace = pi[1].characterProfile.PlayerDisplayName;
                if (pi[1].isCpu) { model.p2IsCpu = 1; }
                else { model.p2IsCpu = 0; }
            }
            else
            {
                model.p2TotalPoints = 0;
                model.secondPlace = "";
                model.p2IsCpu = 99;
            }
            //player 3
            if (GameOptions.numPlayers > 2)
            {
                model.p3TotalPoints = pi[2].gameStats.TotalPoints;
                model.thirdPlace = pi[0].characterProfile.PlayerDisplayName;
                if (pi[2].isCpu) { model.p3IsCpu = 1; }
                else { model.p3IsCpu = 0; }
            }
            else
            {
                model.p3TotalPoints = 0;
                model.thirdPlace = "";
                model.p3IsCpu = 99;
            }
            //player 4
            if (GameOptions.numPlayers > 3)
            {
                model.p4TotalPoints = pi[3].gameStats.TotalPoints;
                model.fourthPlace = pi[3].characterProfile.PlayerDisplayName;
                if (pi[3].isCpu) { model.p4IsCpu = 1; }
                else { model.p4IsCpu = 0; }
            }
            else
            {
                model.p4TotalPoints = 0;
                model.fourthPlace = "";
                model.p4IsCpu = 99;
            }
            model.numPlayers = GameOptions.numPlayers;

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
