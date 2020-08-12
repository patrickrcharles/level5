using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class TouchButtonControl : MonoBehaviour, IPointerDownHandler
{
    const string jumpButtonName = "jump";
    const string shootButtonName = "shoot";
    const string nextSceneButtonName = "nextScene";

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("button : " + eventData.button);
        //Debug.Log("gname : " + gameObject.name);

        if (gameObject.name.Equals(jumpButtonName))
        {
            GameLevelManager.instance.PlayerState.TouchControlJump();
        }
        if (gameObject.name.Equals(shootButtonName))
        {
            GameLevelManager.instance.PlayerState.touchControlShoot();
        }
        if (gameObject.name.Equals(nextSceneButtonName))
        {

            if ((SceneManager.GetActiveScene().buildIndex + 1) == SceneManager.sceneCountInBuildSettings)
            {
                SceneManager.LoadScene("level_01_scrapyard");
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
