using Assets.Scripts.database;
using Assets.Scripts.restapi;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocalAccount : MonoBehaviour
{
    [SerializeField]
    string userNameSelected;

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            userNameSelected = EventSystem.current.currentSelectedGameObject.transform.parent.GetChild(0).GetComponent<Text>().text;
        }
    }
    // OnClick UI
    public void LoginButton()
    {
        if (UserAccountManager.instance.UsersLoaded)
        {
            GameOptions.userName = userNameSelected;
            UserModel user = UserAccountManager.instance.UserAccountData.Where(x => x.UserName == userNameSelected).Single();
            GameOptions.userid = user.Userid;
            StartCoroutine(APIHelper.PostToken(user));
        }
        else
        {
            UserModel user = new UserModel();
            GameOptions.userName = "Guest";

            user.Userid = UserAccountManager.GuestUserid;
            user.UserName = UserAccountManager.GuestPassword;
            user.Password = "guest";

            StartCoroutine(APIHelper.PostToken(user));

            //// if connected to internet
            //if (UtilityFunctions.IsConnectedToInternet())
            //{
            //    StartCoroutine(APIHelper.PostToken(user));
            //}
            //else
            //{
            //    SceneManager.LoadScene(Constants.SCENE_NAME_level_00_loading);
            //}
        }
    }
    // OnClick UI
    public void RemoveUserButton()
    {
        // bring up dialogue
        if (DialogueManager.instance.Coroutine == null)
        {
            DialogueManager.instance.Coroutine = StartCoroutine(DialogueManager.instance.ShowConfirmationDialog());
        }
        // start coroutine to remove locally
        StartCoroutine(UserAccountManager.instance.RemoveUserButton(userNameSelected));
    }
}
