
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;

    // option select display
    private Text resolutionSelectOptionText;
    private Text vsyncSelectOptionText;
    private Text dpiSelectOptionText;
    private Text frameRateSelectOptionText;
    private Text refreshRateSelectOptionText;

    //footer object names
    private const string startButtonName = "press_start";
    private const string statsMenuButtonName = "stats_menu";
    private const string optionsButtonName = "options";
    private const string quitButtonName = "quit_game";

    // scene name
    private const string statsMenuSceneName = "level_00_stats";
    private const string optionsMenuSceneName = "level_00_options";

    //button names
    private const string resolutionSelectButtonName = "resolution";
    private const string vsyncSelectButtonName = "vsync";
    private const string dpiSelectButtonName = "dpi";
    private const string frameRateButtonName = "frame_rate";
    private const string refreshButtonName = "refresh_rate";
    private const string textureQualityButtonName = "texture_quality";

    // option select button name
    private const string resolutionSelectOptionButtonName = "resolution_selected_option";
    private const string vsyncSelectOptionButtonName = "vsync_selected_option";
    private const string dpiSelectOptionButtonName = "dpi_selected_option";
    private const string frameRateOptionButtonName = "frame_rate_selected_option";
    private const string refreshRateOptionButtonName = "refresh_rate_selected_option";
    private const string textureQualityOptionButtonName = "texture_quality_selected_option";

    //current settings name
    private const string resolutionCurrentTextName = "current_resolution";
    private const string vsyncCurrentTextName = "current_vsync";
    private const string dpiCurrentTextName = "current_dpi";
    private const string frameRateCurrentTextName = "current_frame_rate";
    private const string TextureQualityCurrentTextName = "current_texture_quality";
    private const string refreshRateCurrentTextName = "current_refresh_rate";


    private int resolutionSelectedIndex;
    private int dpiSelectedIndex;
    private int frameRateSelectedIndex;
    private int refreshRateSelectedIndex;

    Resolution[] resolutions;

    PlayerControls controls;

    //public static OptionsManager instance;

    //#if UNITY_ANDROID && !UNITY_EDITOR
    //private Vector2 startTouchPosition, endTouchPosition;
    //float swipeUpTolerance;
    //float swipeDownTolerance;
    //float swipeDistance;

    //GameObject prevSelectedGameObject;
    //[SerializeField]
    //GraphicRaycaster m_Raycaster;
    //[SerializeField]
    //PointerEventData m_PointerEventData;
    //[SerializeField]
    //EventSystem m_EventSystem;

    //Touch touch;
    //[SerializeField]
    //bool buttonPressed;

    [SerializeField]
    ChangeOptionType optionType;

    enum textureQuality
    {
        Full = 0,
        Half = 1,
        Quarter = 2,
        Eighth = 3
    }
    public enum ChangeOptionType
    {
        NEXT = 0,
        PREVIOUS = 1,
        NONE = 2
    }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
    }

    void Awake()
    {
        //instance = this;
        controls = new PlayerControls();
        //initializeTouchControls();
        //buttonPressed = false;

        resolutions = Screen.resolutions;

        foreach(Resolution r in resolutions)
        {
            Debug.Log("resolution : " + r.ToString());
        }

        //current
        GameObject.Find(resolutionCurrentTextName).GetComponent<Text>().text = Screen.currentResolution.ToString();
        GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text = Screen.currentResolution.ToString();
        GameObject.Find(dpiCurrentTextName).GetComponent<Text>().text = Screen.dpi.ToString();
        GameObject.Find(dpiSelectOptionButtonName).GetComponent<Text>().text = Screen.dpi.ToString();
        //GameObject.Find(refreshRateCurrentTextName).GetComponent<Text>().text = Screen.currentResolution.refreshRate.ToString() + " Hz";

        if (QualitySettings.vSyncCount == 0)
        {
            GameObject.Find(vsyncSelectOptionButtonName).GetComponent<Text>().text = "Off";
            GameObject.Find(vsyncCurrentTextName).GetComponent<Text>().text = "Off";
        }
        else
        {
            GameObject.Find(vsyncSelectOptionButtonName).GetComponent<Text>().text = "On";
            GameObject.Find(vsyncCurrentTextName).GetComponent<Text>().text = "On";
        }

        if (Application.targetFrameRate < 0 && QualitySettings.vSyncCount == 0)
        {
            GameObject.Find(frameRateCurrentTextName).GetComponent<Text>().text = "unlimited";
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text = "unlimited";
        }
        else
        {
            GameObject.Find(frameRateCurrentTextName).GetComponent<Text>().text = Application.targetFrameRate.ToString();
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text = Application.targetFrameRate.ToString();
        }

        GameObject.Find(dpiSelectOptionButtonName).GetComponent<Text>().text = Screen.dpi.ToString();

        //GameObject.Find(refreshRateOptionButtonName).GetComponent<Text>().text = Screen.currentResolution.refreshRate.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutionSelectedIndex = 0;
        dpiSelectedIndex = 0;
        frameRateSelectedIndex = 0;
        optionType = ChangeOptionType.NONE;

        //prevSelectedGameObject = EventSystem.current.firstSelectedGameObject;
        //swipeUpTolerance = Screen.height / 7;
        //swipeDownTolerance = Screen.height / 5;
        //prevSelectedGameObject = EventSystem.current.firstSelectedGameObject;
        ////Debug.Log("screen width : " + Screen.width);

        //buttonPressed = false;
        if (EventSystem.current == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }
        //var resolutions = Screen.resolutions;
        //var refreshRates = new Int32[resolutions.Length];

        //for (var c = 0; c < refreshRates.Length; c++)
        //{
        //    refreshRates[c] = resolutions[c].refreshRate;
        //   //Debug.Log("refresh rate : " + resolutions[c].refreshRate);
        //}
    }


    public void ChangeVSync()
    {
        Debug.Log("change vsync");
        Debug.Log("QualitySettings.vSyncCount 1 : " + QualitySettings.vSyncCount);
        // check current setting and change
        // vsync on, turn off
        if (QualitySettings.vSyncCount == 1)
        {
            QualitySettings.vSyncCount = 0;
            GameObject.Find(vsyncSelectOptionButtonName).GetComponent<Text>().text = "Off";
            // turn off target frame rate
            Application.targetFrameRate = -1;
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                "unlimited";
        }
        // vsync off, turn on
        else
        {
            QualitySettings.vSyncCount = 1;
            GameObject.Find(vsyncSelectOptionButtonName).GetComponent<Text>().text = "On";
            // turn target frame rate to default
            frameRateSelectedIndex = 0;
            Application.targetFrameRate = 30;
            //Application.targetFrameRate += 30;
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                Application.targetFrameRate.ToString("###") + " fps";

            //Debug.Log("targetFrameRate : " + Application.targetFrameRate.ToString("###"));
        }
        //Debug.Log("vsync : " + QualitySettings.vSyncCount);
        Debug.Log("QualitySettings.vSyncCount 1 : " + QualitySettings.vSyncCount);
        PlayerPrefs.Save();
    }

    public void changeResolution()
    {

        //if (optionType == ChangeOptionType.NEXT || optionType == ChangeOptionType.NONE)
        //{
        //    resolutionSelectedIndex++;
        //}
        //else
        //{
        //    resolutionSelectedIndex--;
        //}
        ////Debug.Log("resolutionSelectedIndex : " + resolutionSelectedIndex);
        ////Debug.Log("resolutions.Length : " + resolutions.Length);

        //if (resolutionSelectedIndex >= resolutions.Length)
        //{
        //    resolutionSelectedIndex = 0;
        //    GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text =
        //        resolutions[resolutionSelectedIndex].ToString();
        //}
        //else
        //{
        //    GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text =
        //        resolutions[resolutionSelectedIndex].ToString();
        //}

        //// apply changes
        //int width = resolutions[resolutionSelectedIndex].width;
        //int height = resolutions[resolutionSelectedIndex].height;
        //Screen.SetResolution(height, width, true);
        //PlayerPrefs.Save();
    }

    public void changeScreenDpi()
    {
        if (optionType == ChangeOptionType.NEXT || optionType == ChangeOptionType.NONE)
        {
            dpiSelectedIndex++;
        }
        else
        {
            dpiSelectedIndex--;
        }

        float scale = 1;
        if (dpiSelectedIndex >= 5)
        {
            dpiSelectedIndex = 0;
            scale = 1 - (dpiSelectedIndex * 0.1f);
            QualitySettings.resolutionScalingFixedDPIFactor = scale;

            GameObject.Find(dpiSelectOptionButtonName).GetComponent<Text>().text =
                (Screen.dpi * QualitySettings.resolutionScalingFixedDPIFactor).ToString("####");

           //Debug.Log("dpi current x " + scale + " : " + (Screen.dpi * scale).ToString("000.00"));
            //return percent.ToString("00.00") + " miles";
        }
        else
        {
            scale = 1 - (dpiSelectedIndex * 0.1f);
            QualitySettings.resolutionScalingFixedDPIFactor = scale;
            GameObject.Find(dpiSelectOptionButtonName).GetComponent<Text>().text =
                (Screen.dpi * QualitySettings.resolutionScalingFixedDPIFactor).ToString("####");

           //Debug.Log("dpi current x " + scale + " : " + (Screen.dpi * scale).ToString("000.00"));

        }
        PlayerPrefs.Save();
        //Debug.Log("screen dpi * scale : " + Screen.dpi * scale );
        //Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);
        //Debug.Log("dpiSelectedIndex : " + dpiSelectedIndex);
    }

    public void changeTargetFps()
    {
        if (QualitySettings.vSyncCount == 1)
        {
            if (optionType == ChangeOptionType.NEXT || optionType == ChangeOptionType.NONE)
            {
                frameRateSelectedIndex++;
            }
            else
            {
                frameRateSelectedIndex--;
            }

            if (frameRateSelectedIndex >= 10)
            {
                frameRateSelectedIndex = 0;
                Application.targetFrameRate = 30;
                //QualitySettings.resolutionScalingFixedDPIFactor = scale;
                //Application.targetFrameRate += 30;
                GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                    Application.targetFrameRate.ToString("###") + " fps";

               //Debug.Log("targetFrameRate : " + Application.targetFrameRate.ToString("###"));
                //return percent.ToString("00.00") + " miles";
            }
            else
            {
                if (Application.targetFrameRate < 0)
                {
                    Application.targetFrameRate = 0;
                    //GameObject.Find(frameRateCurrentTextName).GetComponent<Text>().text = "unlimited";
                }
                //scale = 1 - (dpiSelectedIndex * 0.1f);
                //QualitySettings.resolutionScalingFixedDPIFactor = scale;
                Application.targetFrameRate += 30;
                GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                    Application.targetFrameRate.ToString("###") + " fps";

               //Debug.Log("targetFrameRate : " + Application.targetFrameRate.ToString("###"));
            }
        }
        else
        {
            Application.targetFrameRate = -1;
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                "unlimited";
        }
        //Debug.Log("frameRateSelectedIndex : " + frameRateSelectedIndex);
        //Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);
        //Debug.Log("dpiSelectedIndex : " + dpiSelectedIndex);
        PlayerPrefs.Save();
    }

    public void loadStartScreen()
    {
        PlayerPrefs.Save();
        SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
    }

    //private void Update()
    //{
    //    SwipeCheck();
    //}

    //private void SwipeCheck()
    //{
    //    // if no button selected, return to previous
    //    if (EventSystem.current.currentSelectedGameObject == null)
    //    {
    //        EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
    //    }
    //    // save previous button until a touch is made
    //    if (!buttonPressed && Input.touchCount == 0)
    //    {
    //        prevSelectedGameObject = EventSystem.current.currentSelectedGameObject;
    //    }
    //    // if touch
    //    if (Input.touchCount > 0 && !buttonPressed)
    //    {
    //       //Debug.Log("        if (Input.touchCount > 0 && !buttonPressed)");

    //        Touch touch = Input.touches[0];
    //        if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
    //        {
    //            startTouchPosition = touch.position;
    //        }
    //        endTouchPosition = touch.position;
    //        swipeDistance = endTouchPosition.y - startTouchPosition.y;

    //        // swipe down on changeable options
    //        if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
    //            && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
    //            && swipeDistance < 0 // swipe down
    //            && (startTouchPosition.x > (Screen.width / 4))) // if swipe on right 1/2 of screen)) 
    //        {
    //            //change option
    //            swipeDownOnOption();
    //            // reset previous button to active button
    //            if (EventSystem.current.currentSelectedGameObject != prevSelectedGameObject)
    //            {
    //                EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
    //            }
    //        }
    //        //swipe up on changeable options
    //        if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
    //            && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
    //            && swipeDistance > 0 // swipe down
    //            && (startTouchPosition.x > (Screen.width / 4))) // if swipe on right 1/2 of screen)) 
    //        {
    //            //change option
    //            swipeUpOnOption();
    //            // reset previous button to active button
    //            if (EventSystem.current.currentSelectedGameObject != prevSelectedGameObject)
    //            {
    //                EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
    //            }
    //        }
    //        // on double tap, perform actions
    //        if (touch.tapCount == 2 && touch.phase == TouchPhase.Began && !buttonPressed)
    //        {
    //            activateDoubleTappedButton();
    //        }
    //    }
    //}

    //private void initializeTouchControls()
    //{

    //    //check if startmanager is empty and find correct GraphicRaycaster and EventSystem
    //    if (GameObject.FindObjectOfType<OptionsManager>() != null)
    //    {
    //        //Fetch the Raycaster from the GameObject (the Canvas)
    //        //m_Raycaster = StartManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
    //        m_Raycaster = GameObject.Find("OptionManager").GetComponentInChildren<GraphicRaycaster>();
    //        //Fetch the Event System from the Scene
    //        m_EventSystem = GameObject.Find("OptionManager").GetComponentInChildren<EventSystem>();
    //    }
    //    // else, this is not the startscreen and disable object
    //    else
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}

    //private void activateDoubleTappedButton()
    //{
    //    swipeDownOnOption();
    //}



    //private void swipeUpOnOption()
    //{
    //   //Debug.Log("swipe up");

    //    EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
    //    //check highlighted button
    //    if (prevSelectedGameObject.name.Equals(resolutionSelectOptionButtonName))
    //    {
    //        changeResolution(ChangeOptionType.PREVIOUS);
    //        buttonPressed = true;
    //    }
    //    if (prevSelectedGameObject.name.Equals(vsyncSelectOptionButtonName))
    //    {
    //        ChangeVSync();
    //        buttonPressed = true;
    //    }
    //    if (prevSelectedGameObject.name.Equals(dpiSelectOptionButtonName))
    //    {
    //        changeScreenDpi(ChangeOptionType.PREVIOUS);
    //        buttonPressed = true;
    //    }
    //    if (prevSelectedGameObject.name.Equals(frameRateOptionButtonName))
    //    {
    //        changeTargetFps(ChangeOptionType.PREVIOUS);
    //        buttonPressed = true;
    //    }

    //    buttonPressed = false;
    //}

    //private void swipeDownOnOption()
    //{
    //   //Debug.Log("swipe down");

    //    EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
    //    //check highlighted button
    //    if (prevSelectedGameObject.name.Equals(resolutionSelectOptionButtonName))
    //    {
    //        changeResolution(ChangeOptionType.NEXT);
    //        buttonPressed = true;
    //    }
    //    if (prevSelectedGameObject.name.Equals(vsyncSelectOptionButtonName))
    //    {
    //        ChangeVSync();
    //        buttonPressed = true;
    //    }
    //    if (prevSelectedGameObject.name.Equals(dpiSelectOptionButtonName))
    //    {
    //        changeScreenDpi(ChangeOptionType.NEXT);
    //        buttonPressed = true;
    //    }
    //    if (prevSelectedGameObject.name.Equals(frameRateOptionButtonName))
    //    {
    //        changeTargetFps(ChangeOptionType.NEXT);
    //        buttonPressed = true;
    //    }

    //    buttonPressed = false;
    //}
}