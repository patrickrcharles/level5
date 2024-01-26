
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

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

#if UNITY_ANDROID || UNITY_IOS 

        QualitySettings.vSyncCount = 1;
        Application.targetFrameRate = 60;

        ////inputSystemUIInputModule.DeactivateModule();
        inputSystemUIInputModule.enabled = false;
        standaloneInputModule.enabled = true;
        //standaloneInputModule.ActivateModule(); 
#endif

#if UNITY_STANDALONE || UNITY_EDITOR
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        //standaloneInputModule.DeactivateModule();
        // if win/osx but has touch support
        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
            inputSystemUIInputModule.enabled = false;
            standaloneInputModule.enabled = true;
        }
        // no touch support
        if (!Input.touchSupported || SystemInfo.deviceType != DeviceType.Handheld)
        {
            inputSystemUIInputModule.enabled = true;
            standaloneInputModule.enabled = false;
        }
        inputSystemUIInputModule.enabled = true;
        standaloneInputModule.enabled = false;
        //inputSystemUIInputModule.ActivateModule();
        //Debug.Log("standalone");
#endif
    }
}
