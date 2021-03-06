using Assets.Scripts.database;
using Assets.Scripts.restapi;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalAccount : MonoBehaviour
{
    string userNameSelected;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            userNameSelected = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0).GetComponent<Text>().text;
        }
    }

    public void LoginButton()
    {
        if (UserAccountManager.instance.UsersLoaded)
        {
            GameOptions.userName = userNameSelected;
            UserModel user = UserAccountManager.instance.UserAccountData.Where(x => x.UserName == userNameSelected).Single();
            GameOptions.userid = user.Userid;
            StartCoroutine(APIHelper.PostToken(user));
        }
    }
}
