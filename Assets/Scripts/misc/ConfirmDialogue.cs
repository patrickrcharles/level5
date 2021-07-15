using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ConfirmDialogue : MonoBehaviour
{
    [HideInInspector]
    public int NONE = 0, YES = 1, CANCEL = 2;
    [HideInInspector]
    public int result = 0;

    public Button confirmButton;
    public Button cancelButton;
    //public Button nextButton;

    public Button ConfirmButton { get => confirmButton; set => confirmButton = value; }
    public Button CancelButton { get => cancelButton; set => cancelButton = value; }
    //public Button NextButton { get => nextButton; set => nextButton = value; }

    private void Awake()
    {
        confirmButton = GameObject.Find("confirm_button").GetComponent<Button>();
        confirmButton.onClick.AddListener(ConfirmButtonOnClick);

        if (GameObject.Find("cancel_button") != null)
        {
            cancelButton = GameObject.Find("cancel_button").GetComponent<Button>();
            cancelButton.onClick.AddListener(CancelButtonOnClick);
        }
        //if (GameObject.Find("next_button") != null)
        //{
        //    cancelButton = GameObject.Find("next_button").GetComponent<Button>();
        //    cancelButton.onClick.AddListener(NextButtonOnClick);
        //}

        EventSystem.current.SetSelectedGameObject(confirmButton.gameObject);
    }

    public void ConfirmButtonOnClick()
    {
        result = YES;
    }

    public void CancelButtonOnClick()
    {
        result = CANCEL;
    }
}
