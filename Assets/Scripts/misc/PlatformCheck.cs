
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;

public class PlatformCheck : MonoBehaviour
{
    [SerializeField] InputSystemUIInputModule inputSystemUIInputModule;
    [SerializeField] StandaloneInputModule standaloneInputModule;

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
        //QualitySettings.vSyncCount = 1;
        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        ////inputSystemUIInputModule.DeactivateModule();
        inputSystemUIInputModule.enabled = false;
        standaloneInputModule.enabled = true;
        //standaloneInputModule.ActivateModule(); 
        Debug.Log("android");

#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        //#if UNITY_STANDALONE
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;
        //standaloneInputModule.DeactivateModule();
        standaloneInputModule.enabled = false;
        inputSystemUIInputModule.enabled = true;
        //inputSystemUIInputModule.ActivateModule();
        //Debug.Log("standalone");
#endif

        //Debug.Log("QualitySettings.vSyncCount : " + QualitySettings.vSyncCount);
        //Debug.Log("Application.targetFrameRate : " + Application.targetFrameRate);
    }
}
