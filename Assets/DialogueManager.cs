using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    public ConfirmDialogue confirmationDialog;
    public Canvas canvas;
    private Coroutine coroutine;
    ConfirmDialogue previousDialog;

    bool buttonPressed = false;

    public static DialogueManager instance;

    private void Start()
    {
        instance = this;
        Coroutine = null;
        canvas = GameObject.FindObjectOfType<Canvas>();
    }

    public IEnumerator ShowConfirmationDialog()
    {
        ConfirmDialogue dialog = Instantiate(confirmationDialog, canvas.transform); // instantiate the UI dialog box
        PreviousDialog = dialog;
        while (dialog.result == dialog.NONE)
        {
            yield return null; // wait
        }

        if (dialog.result == dialog.YES)
        {
            buttonPressed = true;
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
        if (dialog.result == dialog.CANCEL)
        {
            buttonPressed = true;
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        }
        Destroy(dialog.gameObject);

        coroutine = null;
    }

    public Coroutine Coroutine { get => coroutine; set => coroutine = value; }
    public ConfirmDialogue PreviousDialog { get => previousDialog; set => previousDialog = value; }
    public bool ButtonPressed { get => buttonPressed; }

    //public bool isDialogueConfirmed(ConfirmDialogue dialog)
    //{
    //    if (dialog.result == dialog.YES)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
    //public bool isDialogueCanceled(ConfirmDialogue dialog)
    //{
    //    if (dialog.result == dialog.CANCEL)
    //    {
    //        return true;
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}
}
