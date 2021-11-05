using System;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;
using Ping = System.Net.NetworkInformation.Ping;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Utility
{
    public static class UtilityFunctions
    {
        public static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public static string RemoveWhitespace(string str)
        {
            return string.Join("", str.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));
        }
        public static bool ContainsWhiteSpace(String s)
        {
            return s.Contains(" ");
        }
        public static bool IsConnectedToInternet()
        {
            string host = "www.google.com";
            Ping p = new Ping();
            try
            {
                PingReply reply = p.Send(host, 500);

                if (reply.Status == IPStatus.Success)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return false;
            }
        }

        public static string RandomString(int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[length];
            var random = new System.Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var finalString = new String(stringChars);

            return finalString;
        }
        public static string GetExternalIpAdress()
        {
            string pubIp = new WebClient().DownloadString("https://api.ipify.org");
            return pubIp;
        }
        public static float GetRandomFloat(float min, float max)
        {
            float randNum = Random.Range(min, max);
            return randNum;
        }
        public static int GetRandomInteger(int min, int max)
        {
            int randNum = Random.Range(min, max);
            return randNum;
        }
        public static float getPercentageFloat(int made, int attempt)
        {
            if (attempt > 0)
            {
                float accuracy = (float)made / (float)attempt;
                return (accuracy * 100);
            }
            else
            {
                return 0;
            }
        }
        public static bool rollForCriticalInt(int max)
        {
            int percent = Random.Range(0, 100);
            //float percent = randNum.Next(1, 100);
            if (percent <= max)
            {
                //Debug.Log("roll for critical : " + percent + "  max chance : " + max);
                return true;
            }
            return false;
        }

        public static Transform FindDeepChild(this Transform aParent, string aName)
        {
            Queue<Transform> queue = new Queue<Transform>();
            queue.Enqueue(aParent);
            while (queue.Count > 0)
            {
                var c = queue.Dequeue();
                if (c.name == aName)
                    return c;
                foreach (Transform t in c)
                    queue.Enqueue(t);
            }
            return null;
        }
    }
}

