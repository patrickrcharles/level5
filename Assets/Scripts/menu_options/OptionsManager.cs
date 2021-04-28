
using System;
using UnityEngine;
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

    public static OptionsManager instance;

    //bool buttonPressed;

    enum textureQuality
    {
        Full = 0,
        Half = 1,
        Quarter = 2,
        Eighth = 3
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

    //private Text gameModeSelectText;
    void Awake()
    {
        instance = this;
        controls = new PlayerControls();
        //buttonPressed = false;

        resolutions = Screen.resolutions;

        //current
        GameObject.Find(resolutionCurrentTextName).GetComponent<Text>().text = Screen.currentResolution.ToString();
        GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text = Screen.currentResolution.ToString();
        GameObject.Find(dpiCurrentTextName).GetComponent<Text>().text = Screen.dpi.ToString();
        GameObject.Find(refreshRateCurrentTextName).GetComponent<Text>().text = Screen.currentResolution.refreshRate.ToString() + " Hz";

        if (QualitySettings.vSyncCount == 0)
        {
            GameObject.Find(vsyncCurrentTextName).GetComponent<Text>().text = "Off";
        }
        else
        {
            GameObject.Find(vsyncCurrentTextName).GetComponent<Text>().text = "On";
        }

        if (Application.targetFrameRate < 0)
        {
            GameObject.Find(frameRateCurrentTextName).GetComponent<Text>().text = "unlimited";
        }
        else
        {
            GameObject.Find(frameRateCurrentTextName).GetComponent<Text>().text = Application.targetFrameRate.ToString();
        }
        GameObject.Find(refreshRateOptionButtonName).GetComponent<Text>().text = Screen.currentResolution.refreshRate.ToString();

    }


    //public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
    //{
    //    Type enumType = typeof(T);
    //    if (!enumType.IsEnum)
    //    {
    //        throw new Exception("T must be an Enumeration type.");
    //    }

    //    return (T)Enum.ToObject(enumType, intValue);
    //}

    // Start is called before the first frame update
    void Start()
    {
        resolutionSelectedIndex = 0;
        dpiSelectedIndex = 0;
        frameRateSelectedIndex = 0;

        //var resolutions = Screen.resolutions;
        //var refreshRates = new Int32[resolutions.Length];

        //for (var c = 0; c < refreshRates.Length; c++)
        //{
        //    refreshRates[c] = resolutions[c].refreshRate;
        //    Debug.Log("refresh rate : " + resolutions[c].refreshRate);
        //}
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Debug.Log("change rez");
            changeResolution();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Debug.Log("change dpi");
            changeScreenDpi();
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            //Debug.Log("change dpi");
            changeTargetFps();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            //Debug.Log("change dpi");
            changeVSync();
        }
    }

    private void changeVSync()
    {
        if (QualitySettings.vSyncCount == 0)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        if (QualitySettings.vSyncCount == 0)
        {
            GameObject.Find(vsyncSelectOptionButtonName).GetComponent<Text>().text = "Off";
        }
        else
        {
            GameObject.Find(vsyncSelectOptionButtonName).GetComponent<Text>().text = "On";
        }
        Debug.Log("vsync : " + QualitySettings.vSyncCount);
    }

    private void changeResolution()
    {
        resolutionSelectedIndex++;
        //Debug.Log("resolutionSelectedIndex : " + resolutionSelectedIndex);
        //Debug.Log("resolutions.Length : " + resolutions.Length);

        if (resolutionSelectedIndex >= resolutions.Length)
        {
            resolutionSelectedIndex = 0;
            GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text =
                resolutions[resolutionSelectedIndex].ToString();
        }
        else
        {
            GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text =
                resolutions[resolutionSelectedIndex].ToString();
        }
    }

    private void changeScreenDpi()
    {
        float scale = 1;
        dpiSelectedIndex++;
        if (dpiSelectedIndex >= 5)
        {
            dpiSelectedIndex = 0;
            scale = 1 - (dpiSelectedIndex * 0.1f);
            QualitySettings.resolutionScalingFixedDPIFactor = scale;

            GameObject.Find(dpiSelectOptionButtonName).GetComponent<Text>().text =
                (Screen.dpi * QualitySettings.resolutionScalingFixedDPIFactor).ToString("####");

            Debug.Log("dpi current x " + scale + " : " + (Screen.dpi * scale).ToString("000.00"));
            //return percent.ToString("00.00") + " miles";
        }
        else
        {
            scale = 1 - (dpiSelectedIndex * 0.1f);
            QualitySettings.resolutionScalingFixedDPIFactor = scale;
            GameObject.Find(dpiSelectOptionButtonName).GetComponent<Text>().text =
                (Screen.dpi * QualitySettings.resolutionScalingFixedDPIFactor).ToString("####");

            Debug.Log("dpi current x " + scale + " : " + (Screen.dpi * scale).ToString("000.00"));

        }
        //Debug.Log("screen dpi * scale : " + Screen.dpi * scale );
        //Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);
        //Debug.Log("dpiSelectedIndex : " + dpiSelectedIndex);
    }

    private void changeTargetFps()
    {
        if (QualitySettings.vSyncCount == 1)
        {
            frameRateSelectedIndex++;
            if (frameRateSelectedIndex >= 10)
            {
                frameRateSelectedIndex = 0;
                Application.targetFrameRate = 30;
                //QualitySettings.resolutionScalingFixedDPIFactor = scale;
                Application.targetFrameRate += 30;
                GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                    Application.targetFrameRate.ToString("###") + " fps";

                Debug.Log("targetFrameRate : " + Application.targetFrameRate.ToString("###"));
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

                Debug.Log("targetFrameRate : " + Application.targetFrameRate.ToString("###"));
            }
        }
        else
        {
            Application.targetFrameRate = -1;
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                "unlimited";
        }
        Debug.Log("frameRateSelectedIndex : " + frameRateSelectedIndex);
        //Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);
        //Debug.Log("dpiSelectedIndex : " + dpiSelectedIndex);
    }

}