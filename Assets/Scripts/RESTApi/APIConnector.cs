
using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using UnityEngine;
using Random = System.Random;


public class APIConnector : MonoBehaviour
{

    public void CreateNewUser(UserModel user)
    {
        if (!UtilityFunctions.IsValidEmail(user.Email))
        {
            // halt, fix email
            Debug.Log("invalid email address format ");
        }
        // check if user already exists
        if (APIHelper.UserNameExists(user.UserName))
        {
            Debug.Log("username already exists...try again");
        }
        else
        {
            Debug.Log("username does not exist");
            StartCoroutine(APIHelper.PostUser(user));
        }

        // if not, create user
        // else return 'username already exists
    }
}


