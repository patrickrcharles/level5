
using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using UnityEngine;


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
    }
}


