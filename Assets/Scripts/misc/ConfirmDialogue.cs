using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfirmDialogue : MonoBehaviour
{
    [HideInInspector]
    public int NONE = 0, YES = 1, CANCEL = 2;
    [HideInInspector]
    public int result = 0;

    Button confirmButton;
    Button cancelButton;

    private void Start()
    {
        confirmButton = GameObject.Find("confirm_button").GetComponent<Button>();
        confirmButton.onClick.AddListener(ConfirmButtonOnClick);

        cancelButton = GameObject.Find("cancel_button").GetComponent<Button>();
        cancelButton.onClick.AddListener(CancelButtonOnClick);

        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
        }
    }

    public void ConfirmButtonOnClick()
    {
        result = YES;
        //if I debug here it prints
        Debug.Log(result);
    }

    public void CancelButtonOnClick()
    {
        result = CANCEL;
        Debug.Log(result);
    }
}
