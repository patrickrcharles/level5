using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

public class PlatformCheck : MonoBehaviour
{
    [SerializeField] InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] StandaloneInputModule standaloneInputModule;

    //public static PlatformCheck instance;

    // Start is called before the first frame update
    void Awake()
    {
        if (EventSystem.current.gameObject.GetComponent<StandaloneInputModule>() != null)
        {
            standaloneInputModule = EventSystem.current.gameObject.GetComponent<StandaloneInputModule>();
        }

        if (EventSystem.current.gameObject.GetComponent<InputSystemUIInputModule>() != null)
        {
            inputSystemUIInputModule = EventSystem.current.gameObject.GetComponent<InputSystemUIInputModule>();
        }

#if UNITY_ANDROID && !UNITY_EDITOR
        Debug.Log("if android");
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        ////inputSystemUIInputModule.DeactivateModule();
        inputSystemUIInputModule.enabled = false;
        //standaloneInputModule.ActivateModule();
        
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        Debug.Log("if editor");
        QualitySettings.vSyncCount = 0;
        //standaloneInputModule.DeactivateModule();
        standaloneInputModule.enabled = false;

        //inputSystemUIInputModule.ActivateModule();
        

#endif
    }


}
