using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour
{
    public ConfirmDialogue confirmationDialog;
    public Canvas canvas;

    private Coroutine coroutine;

    private void Start()
    {
        coroutine = null;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha9) == true)
        {
            if (coroutine == null)
            {
                coroutine = StartCoroutine(ShowConfirmationDialog());
            }
        }
    }

    IEnumerator ShowConfirmationDialog()
    {
        // whatever you're doing now with the temporary / placement preview building
        ConfirmDialogue dialog = Instantiate(confirmationDialog, canvas.transform); // instantiate the UI dialog box

        while (dialog.result == dialog.NONE)
        {
            //Debug.Log("Builder.ShowConfirmationDialog() - Yielding");

            yield return null; // wait
        }

        if (dialog.result == dialog.YES)
        {
            // place the real building
            Debug.Log("Builder.ShowConfirmationDialog(): Yes");
            //isDialogueConfirmed(dialog);
        }
        else if (dialog.result == dialog.CANCEL)
        {
            // remove the temporary / preview building
            Debug.Log("Builder.ShowConfirmationDialog(): Cancel");
            //isDialogueConfirmed(dialog);
        }

        Destroy(dialog.gameObject);

        coroutine = null;
    }

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
