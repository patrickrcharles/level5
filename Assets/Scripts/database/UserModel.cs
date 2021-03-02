using System;
using System.Collections;
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
    public class UserModel 
    {
        //public int Id;
        public int Userid;
        public string UserName;
        public string FirstName;
        public string LastName;
        public string Email;
        public string Password;
        public string IpAddress;
        public string SignUpDate;
        public string LastLogin;
        public string BearerToken;
    }
}
