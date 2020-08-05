using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchButtonControl : MonoBehaviour, IPointerDownHandler
{
    const string jumpButtonName = "jump";
    const string shootpButtonName = "shoot";


    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("button : " + eventData.button);
        //Debug.Log("gname : " + gameObject.name);

        if (gameObject.name.Equals(jumpButtonName))
        {
            GameLevelManager.Instance.PlayerState.TouchControlJump();
        }
        if (gameObject.name.Equals(shootpButtonName))
        {
            GameLevelManager.Instance.PlayerState.touchControlShoot();
        }
    }
}
