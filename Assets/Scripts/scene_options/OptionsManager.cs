
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
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
    private const string textureQualityButtonName = "texture_quality";    

    // option select button name
    private const string resolutionSelectOptionButtonName = "resolution_selected_option";
    private const string vsyncSelectOptionButtonName = "vsync_selected_option";
    private const string dpiSelectOptionButtonName = "dpi_selected_option";
    private const string frameRateOptionButtonName = "frame_rate_selected_option";
    private const string textureQualityOptionButtonName = "texture_quality_selected_option";

    //current settings name
    private const string resolutionCurrentTextName = "current_resolution";
    private const string vsyncCurrentTextName = "current_vsync";
    private const string dpiCurrentTextName = "current_dpi";
    private const string frameRateCurrentTextName = "current_frame_rate";
    private const string TextureQualityCurrentTextName = "current_texture_quality";


    private int resolutionSelectedIndex;
    private int dpiSelectedIndex;
    private int frameRateSelectedIndex;
    private int refreshRateSelectedIndex;

    Resolution[] resolutions;

    PlayerControls controls;

    public static OptionsManager instance;

    bool buttonPressed;

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
        buttonPressed = false;

        resolutions = Screen.resolutions;

        ////float scale;
        //for(int i = 0; i < 10; i++)
        //{
        //    float scale = 1 - (i * 0.1f);
        //    Debug.Log("dpi current x " + scale + " : " + Mathf.Ceil(240 * scale));
        //}

        //Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);

        //int w = Screen.width;
        //int h = Screen.height;
        //float num = (float)w / h;
        //Debug.Log("screen ratio : " + num);

        //current
        GameObject.Find(resolutionCurrentTextName).GetComponent<Text>().text = Screen.currentResolution.ToString();
        GameObject.Find(resolutionSelectOptionButtonName).GetComponent<Text>().text = Screen.currentResolution.ToString();
        //GameObject.Find(textureQualityButtonName).GetComponent<Text>().text = textureQuality.Full.ToString();

        GameObject.Find(textureQualityOptionButtonName).GetComponent<Text>().text 
            = GetEnumValue<textureQuality>(QualitySettings.masterTextureLimit).ToString();

        GameObject.Find(dpiCurrentTextName).GetComponent<Text>().text = Screen.dpi.ToString();

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

        GameObject.Find(TextureQualityCurrentTextName).GetComponent<Text>().text
            = GetEnumValue<textureQuality>(QualitySettings.masterTextureLimit).ToString();

        //// buttons to disable for touch input
        //levelSelectButton = GameObject.Find(levelSelectButtonName).GetComponent<Button>();
        //trafficSelectButton = GameObject.Find(trafficSelectButtonName).GetComponent<Button>();
        //playerSelectButton = GameObject.Find(playerSelectButtonName).GetComponent<Button>();
        //CheerleaderSelectButton = GameObject.Find(cheerleaderSelectButtonName).GetComponent<Button>();
        //modeSelectButton = GameObject.Find(modeSelectButtonName).GetComponent<Button>();

        //// player object with lock texture and unlock text
        //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
        //playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        //playerSelectOptionStatsText = GameObject.Find(playerSelectStatsObjectName).GetComponent<Text>();
        //playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();
        //playerSelectUnlockText = GameObject.Find(playerSelectUnlockObjectName).GetComponent<Text>();
        //playerSelectCategoryStatsText = GameObject.Find(playerSelectStatsCategoryName).GetComponent<Text>();

        //// friend object with lock texture and unlock text
        //cheerleaderSelectedIsLockedObject = GameObject.Find(cheerleaderSelectIsLockedObjectName);
        //cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
        //cheerleaderSelectOptionImage = GameObject.Find(cheerleaderSelectImageObjectName).GetComponent<Image>();
        //cheerleaderSelectUnlockText = GameObject.Find(cheerleaderSelectUnlockObjectName).GetComponent<Text>();

        //// traffic option selection text
        //trafficSelectOptionText = GameObject.Find(trafficSelectOptionName).GetComponent<Text>();

        ////default index for player selected
        //playerSelectedIndex = GameOptions.playerSelectedIndex;
        //cheerleaderSelectedIndex = GameOptions.cheerleaderSelectedIndex;
        //levelSelectedIndex = GameOptions.levelSelectedIndex;
        //modeSelectedIndex = GameOptions.modeSelectedIndex;
        //trafficEnabled = GameOptions.trafficEnabled;


    }


    public static T GetEnumValue<T>(int intValue) where T : struct, IConvertible
    {
        Type enumType = typeof(T);
        if (!enumType.IsEnum)
        {
            throw new Exception("T must be an Enumeration type.");
        }

        return (T)Enum.ToObject(enumType, intValue);
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutionSelectedIndex = 0;
        dpiSelectedIndex = 0;
        frameRateSelectedIndex = 0;

        var resolutions = Screen.resolutions;
        var refreshRates = new Int32[resolutions.Length];

        for (var c = 0; c < refreshRates.Length; c++)
        {
            refreshRates[c] = resolutions[c].refreshRate;
            //Debug.Log(resolutions[c].refreshRate);
        }
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
    }

    private void changeResolution()
    {
        resolutionSelectedIndex++;
        //Debug.Log("resolutionSelectedIndex : " + resolutionSelectedIndex);
        //Debug.Log("resolutions.Length : " + resolutions.Length);

        if(resolutionSelectedIndex >= resolutions.Length)
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
                (Screen.dpi * QualitySettings.resolutionScalingFixedDPIFactor ).ToString("####");

            Debug.Log("dpi current x " + scale + " : " + (Screen.dpi *scale).ToString("000.00"));

        }
        //Debug.Log("screen dpi * scale : " + Screen.dpi * scale );
        Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);
        //Debug.Log("dpiSelectedIndex : " + dpiSelectedIndex);
    }

    private void changeTargetFps()
    {
        //float scale = 1;
        frameRateSelectedIndex++;
        if (frameRateSelectedIndex >= 10)
        {
            frameRateSelectedIndex = 0;
            Application.targetFrameRate = 30;
            //QualitySettings.resolutionScalingFixedDPIFactor = scale;
            Application.targetFrameRate += 30;
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                Application.targetFrameRate.ToString("###");

            Debug.Log("targetFrameRate : "+ Application.targetFrameRate.ToString("###"));
            //return percent.ToString("00.00") + " miles";
        }
        else
        {
            if(Application.targetFrameRate < 0)
            {
                Application.targetFrameRate = 0;
            }
            //scale = 1 - (dpiSelectedIndex * 0.1f);
            //QualitySettings.resolutionScalingFixedDPIFactor = scale;
            Application.targetFrameRate += 30;
            GameObject.Find(frameRateOptionButtonName).GetComponent<Text>().text =
                Application.targetFrameRate.ToString("###");

            Debug.Log("targetFrameRate : " + Application.targetFrameRate.ToString("###"));
        }
        Debug.Log("frameRateSelectedIndex : "+ frameRateSelectedIndex);
        //Debug.Log("rez scale : " + QualitySettings.resolutionScalingFixedDPIFactor);
        //Debug.Log("dpiSelectedIndex : " + dpiSelectedIndex);
    }

    //private void setInitialGameOptions()
    //{
    //    GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    //    GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    //    GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModeObjectName;
    //    GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;

    //    GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;
    //    GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
    //}

    //// Update is called once per frame
    //void Update()
    //{
    //    // check for some button not selected
    //    if (EventSystem.current != null)
    //    {
    //        if (EventSystem.current.currentSelectedGameObject == null)
    //        {
    //            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
    //        }
    //        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
    //    }

    //    // if player highlighted, display player
    //    if (currentHighlightedButton.Equals(playerSelectButtonName) || currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //    {
    //        try
    //        {
    //            initializePlayerDisplay();
    //        }
    //        catch
    //        {
    //            return;
    //        }
    //    }
    //    // if cheerleader highlighted, display cheerleader
    //    if (currentHighlightedButton.Equals(cheerleaderSelectButtonName) || currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
    //    {
    //        try
    //        {
    //            initializeCheerleaderDisplay();
    //        }
    //        catch
    //        {
    //            return;
    //        }
    //    }

    //    // ================================== footer buttons =====================================================================
    //    // start button | start game
    //    if ((controls.Player.submit.triggered
    //         || controls.Player.jump.triggered
    //         || controls.Player.shoot.triggered)
    //        && currentHighlightedButton.Equals(startButtonName))
    //    {
    //        loadScene();
    //    }
    //    // quit button | quit game
    //    if ((controls.Player.submit.triggered
    //         || controls.Player.jump.triggered
    //         || controls.Player.shoot.triggered)
    //        && currentHighlightedButton.Equals(quitButtonName))
    //    {
    //        Application.Quit();
    //    }
    //    // stats menu button | load stats menu
    //    if ((controls.Player.submit.triggered
    //         || controls.Player.jump.triggered
    //         || controls.Player.shoot.triggered)
    //        && currentHighlightedButton.Equals(statsMenuButtonName))
    //    {
    //        loadScene(statsMenuSceneName);
    //    }

    //    if ((controls.Player.submit.triggered
    //        || controls.Player.jump.triggered
    //        || controls.Player.shoot.triggered)
    //        && currentHighlightedButton.Equals(optionsButtonName))
    //    {
    //        loadScene(optionsMenuSceneName);
    //    }

    //    // ================================== navigation =====================================================================
    //    // up, option select
    //    if (controls.UINavigation.Up.triggered && !buttonPressed
    //        && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(trafficSelectOptionName)
    //        && !currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
    //    {
    //        buttonPressed = true;
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnUp().gameObject);
    //        buttonPressed = false;
    //    }

    //    // down, option select
    //    if (controls.UINavigation.Down.triggered && !buttonPressed
    //        && !currentHighlightedButton.Equals(playerSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(levelSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(modeSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName)
    //        && !currentHighlightedButton.Equals(trafficSelectOptionName))
    //    {
    //        buttonPressed = true;
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnDown().gameObject);
    //        buttonPressed = false;
    //    }

    //    // right, go to change options
    //    if (controls.UINavigation.Right.triggered
    //        && EventSystem.current.currentSelectedGameObject.GetComponent<Button>()
    //        .FindSelectableOnRight() != null)
    //    {
    //        EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //            .GetComponent<Button>().FindSelectableOnRight().gameObject);
    //    }

    //    // left, return to option select
    //    if (controls.UINavigation.Left.triggered)
    //    {
    //        // check if button exists. if no selectable on left, throws null object exception
    //        if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnLeft() != null)
    //        {
    //            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
    //                .GetComponent<Button>().FindSelectableOnLeft().gameObject);
    //        }
    //    }

    //    // ================================== change options =============================================================
    //    // up, change options
    //    if (controls.UINavigation.Up.triggered && !buttonPressed)
    //    {
    //        //Debug.Log(" change option up");
    //        buttonPressed = true;
    //        try
    //        {
    //            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //            {
    //                changeSelectedPlayerUp();
    //                initializePlayerDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
    //            {
    //                changeSelectedLevelUp();
    //                initializeLevelDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
    //            {
    //                changeSelectedModeUp();
    //                intializeModeDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
    //            {
    //                changeSelectedCheerleaderUp();
    //                initializeCheerleaderDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(trafficSelectOptionName))
    //            {
    //                changeSelectedTrafficOption();
    //                initializeTrafficOptionDisplay();
    //            }
    //        }
    //        catch
    //        {
    //            return;
    //        }
    //        buttonPressed = false;
    //    }
    //    // down, change option
    //    if (controls.UINavigation.Down.triggered && !buttonPressed)
    //    {
    //        //Debug.Log(" change option down");
    //        buttonPressed = true;
    //        try
    //        {
    //            if (currentHighlightedButton.Equals(playerSelectOptionButtonName))
    //            {
    //                changeSelectedPlayerDown();
    //                initializePlayerDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(levelSelectOptionButtonName))
    //            {
    //                changeSelectedLevelDown();
    //                initializeLevelDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(modeSelectOptionButtonName))
    //            {
    //                changeSelectedModeDown();
    //                intializeModeDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(cheerleaderSelectOptionButtonName))
    //            {
    //                changeSelectedCheerleaderDown();
    //                initializeCheerleaderDisplay();
    //            }
    //            if (currentHighlightedButton.Equals(trafficSelectOptionName))
    //            {
    //                changeSelectedTrafficOption();
    //                initializeTrafficOptionDisplay();
    //            }
    //        }
    //        catch
    //        {
    //            return;
    //        }
    //        buttonPressed = false;
    //    }
    //}


    //public void disableButtonsNotUsedForTouchInput()
    //{
    //    levelSelectButton.enabled = false;
    //    trafficSelectButton.enabled = false;
    //    playerSelectButton.enabled = false;
    //    CheerleaderSelectButton.enabled = false;
    //    modeSelectButton.enabled = false;
    //}

    //public void initializeTrafficOptionDisplay()
    //{
    //    if (trafficEnabled)
    //    {
    //        trafficSelectOptionText.text = "ON";
    //    }
    //    if (!trafficEnabled)
    //    {
    //        trafficSelectOptionText.text = "OFF";
    //    }
    //}

    //public void changeSelectedTrafficOption()
    //{
    //    trafficEnabled = !trafficEnabled;
    //}

    //public void initializeLevelDisplay()
    //{
    //    levelSelectOptionText = GameObject.Find(levelSelectOptionButtonName).GetComponent<Text>();
    //    levelSelectOptionText.text = levelSelectedData[levelSelectedIndex].LevelDisplayName;
    //    GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    //}

    //public void initializeCheerleaderDisplay()
    //{
    //    try
    //    {
    //        cheerleaderSelectOptionImage.enabled = true;
    //        playerSelectOptionImage.enabled = false;
    //        playerSelectOptionStatsText.enabled = false;
    //        playerSelectedIsLockedObject.SetActive(false);
    //        playerSelectCategoryStatsText.enabled = false;
    //        playerSelectedIsLockedObject.SetActive(false);

    //        // check if players is locked
    //        foreach (StartScreenCheerleaderSelected cl in cheerleaderSelectedData)
    //        {
    //            if (AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cl.CheerleaderId) != null)
    //            {
    //                Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cl.CheerleaderId);
    //                cl.IsLocked = tempAchieve.IsLocked;
    //                cl.UnlockCharacterText = tempAchieve.AchievementDescription;
    //            }
    //            // none selected
    //            if (cl.CheerleaderId == 0)
    //            {
    //                cl.IsLocked = false;
    //                cl.UnlockCharacterText = "";
    //            }
    //        }

    //        if (cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
    //        {
    //            Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.CheerleaderId == cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderId);
    //            // disable text and unlock text
    //            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
    //            cheerleaderSelectedIsLockedObject.SetActive(true);
    //            cheerleaderSelectUnlockText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].UnlockCharacterText
    //                + "\nprogress " + tempAchieve.ActivationValueProgressionInt
    //                + " / " + tempAchieve.ActivationValueInt;
    //        }
    //        else
    //        {
    //            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
    //            cheerleaderSelectedIsLockedObject.SetActive(false);
    //            cheerleaderSelectUnlockText.text = "";
    //        }

    //        cheerleaderSelectOptionText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
    //        cheerleaderSelectOptionImage.sprite = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderPortrait;


    //        cheerleaderSelectOptionText = GameObject.Find(cheerleaderSelectOptionButtonName).GetComponent<Text>();
    //        cheerleaderSelectOptionText.text = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
    //        GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
    //    }
    //    catch
    //    {
    //        return;
    //    }
    //}

    //public void intializeModeDisplay()
    //{
    //    modeSelectOptionText = GameObject.Find(modeSelectOptionButtonName).GetComponent<Text>();
    //    modeSelectOptionText.text = modeSelectedData[modeSelectedIndex].ModelDisplayName;

    //    ModeSelectOptionDescriptionText = GameObject.Find(modeSelectDescriptionObjectName).GetComponent<Text>();
    //    ModeSelectOptionDescriptionText.text = modeSelectedData[modeSelectedIndex].ModeDescription;
    //}

    //public void initializePlayerDisplay()
    //{
    //    try
    //    {
    //        cheerleaderSelectOptionImage.enabled = false;
    //        cheerleaderSelectedIsLockedObject.SetActive(false);

    //        playerSelectOptionImage.enabled = true;
    //        playerSelectOptionStatsText.enabled = true;
    //        playerSelectedIsLockedObject.SetActive(true);
    //        playerSelectCategoryStatsText.enabled = true;

    //        // check if players is locked
    //        foreach (ShooterProfile sp in playerSelectedData)
    //        {
    //            if (AchievementManager.instance.AchievementList.Find(x => x.PlayerId == sp.PlayerId) != null)
    //            {
    //                Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.PlayerId == sp.PlayerId);
    //                sp.IsLocked = tempAchieve.IsLocked;
    //                sp.UnlockCharacterText = tempAchieve.AchievementDescription;
    //            }
    //        }

    //        if (playerSelectedData[playerSelectedIndex].IsLocked)
    //        {
    //            // find achievement that unlocks player
    //            //Achievement tempAchieve = AchievementManager.instance.AchievementList.Find(x => x.PlayerId == playerSelectedData[playerSelectedIndex].PlayerId);
    //            // disable text and unlock text
    //            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
    //            playerSelectedIsLockedObject.SetActive(true);
    //            playerSelectUnlockText.text = playerSelectedData[playerSelectedIndex].UnlockCharacterText;
    //            // find achievement by player id
    //            // + "\nprogress : " + tempAchieve.ActivationValueProgressionInt;
    //        }
    //        // if player is locked or free play mode selected
    //        if (!playerSelectedData[playerSelectedIndex].IsLocked
    //            || modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
    //        {
    //            //playerSelectedIsLockedObject = GameObject.Find(playerSelectIsLockedObjectName);
    //            playerSelectedIsLockedObject.SetActive(false);
    //            playerSelectUnlockText.text = "";
    //        }

    //        playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
    //        playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;

    //        playerSelectOptionStatsText.text = playerSelectedData[playerSelectedIndex].Accuracy2Pt.ToString("F0") + "\n"
    //            + playerSelectedData[playerSelectedIndex].Accuracy3Pt.ToString("F0") + "\n"
    //            + playerSelectedData[playerSelectedIndex].Accuracy4Pt.ToString("F0") + "\n"
    //            + playerSelectedData[playerSelectedIndex].Accuracy7Pt.ToString("F0") + "\n"
    //            + playerSelectedData[playerSelectedIndex].calculateSpeedToPercent().ToString("F0") + "\n"
    //            + playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0") + "\n"
    //            //+ playerSelectedData[playerSelectedIndex].Range.ToString("F0") + "\n"
    //            + playerSelectedData[playerSelectedIndex].CriticalPercent.ToString("F0");

    //        GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    //    }
    //    catch
    //    {
    //        return;
    //    }
    //}

    //private void loadPlayerSelectDataList()
    //{
    //    string path = "Prefabs/start_menu/player_selected_objects";
    //    GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

    //    foreach (GameObject obj in objects)
    //    {
    //        ShooterProfile temp = obj.GetComponent<ShooterProfile>();
    //        //Debug.Log(" temp : " + temp.PlayerDisplayName);
    //        playerSelectedData.Add(temp);
    //    }
    //    // sort list by  character id
    //    playerSelectedData.Sort(sortByPlayerId);
    //}

    //private void loadCheerleaderSelectDataList()
    //{

    //    string path = "Prefabs/start_menu/cheerleader_selected_object";
    //    GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

    //    //Debug.Log("objects : " + objects.Length);
    //    foreach (GameObject obj in objects)
    //    {
    //        StartScreenCheerleaderSelected temp = obj.GetComponentInChildren<StartScreenCheerleaderSelected>();
    //        //Debug.Log(" temp : " + temp.UnlockCharacterText);
    //        cheerleaderSelectedData.Add(temp);
    //    }
    //    // sort list by  character id
    //    cheerleaderSelectedData.Sort(sortByCheerleaderId);
    //}

    //static int sortByPlayerId(ShooterProfile p1, ShooterProfile p2)
    //{
    //    return p1.PlayerId.CompareTo(p2.PlayerId);
    //}

    //static int sortByCheerleaderId(StartScreenCheerleaderSelected p1, StartScreenCheerleaderSelected p2)
    //{
    //    return p1.CheerleaderId.CompareTo(p2.CheerleaderId);
    //}

    //static int sortByLevelId(StartScreenLevelSelected l1, StartScreenLevelSelected l2)
    //{
    //    return l1.LevelId.CompareTo(l2.LevelId);
    //}

    //static int sortByModeId(StartScreenModeSelected m1, StartScreenModeSelected m2)
    //{
    //    return m1.ModeId.CompareTo(m2.ModeId);
    //}

    //private void loadLevelSelectDataList()
    //{
    //    //Debug.Log("loadPlayerSelectDataList()");

    //    string path = "Prefabs/start_menu/level_selected_objects";
    //    GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

    //    foreach (GameObject obj in objects)
    //    {
    //        StartScreenLevelSelected temp = obj.GetComponent<StartScreenLevelSelected>();
    //        levelSelectedData.Add(temp);
    //    }
    //    // sort list by  level id
    //    levelSelectedData.Sort(sortByLevelId);
    //}

    //private void loadModeSelectDataList()
    //{
    //    //Debug.Log("loadModeSelectDataList()");

    //    string path = "Prefabs/start_menu/mode_selected_objects";
    //    GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

    //    foreach (GameObject obj in objects)
    //    {
    //        StartScreenModeSelected temp = obj.GetComponent<StartScreenModeSelected>();
    //        modeSelectedData.Add(temp);
    //    }
    //    // sort list by  mode id
    //    modeSelectedData.Sort(sortByModeId);

    //    //foreach (StartScreenModeSelected s in modeSelectedData)
    //    //{
    //    //    Debug.Log(" mode id : " + s.ModeId);
    //    //}
    //}

    //public void changeSelectedPlayerUp()
    //{
    //    //Debug.Log("changeSelectedPlayerUp");
    //    // if default index (first in list), go to end of list
    //    if (playerSelectedIndex == 0)
    //    {
    //        playerSelectedIndex = playerSelectedData.Count - 1;

    //    }
    //    else
    //    {
    //        // if not first index, decrement
    //        playerSelectedIndex--;
    //    }
    //    GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    //    //Debug.Log("player selected : " + GameOptions.playerSelected);
    //}
    //public void changeSelectedPlayerDown()
    //{
    //    //Debug.Log("changeSelectedPlayerDown");
    //    // if default index (first in list
    //    if (playerSelectedIndex == playerSelectedData.Count - 1)
    //    {
    //        playerSelectedIndex = 0;
    //    }
    //    else
    //    {
    //        //if not first index, increment
    //        playerSelectedIndex++;
    //    }
    //    GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    //    //Debug.Log("player selected : " + GameOptions.playerSelected);
    //}

    //public void changeSelectedCheerleaderUp()
    //{
    //    // if default index (first in list
    //    if (cheerleaderSelectedIndex == 0)
    //    {
    //        cheerleaderSelectedIndex = cheerleaderSelectedData.Count - 1;
    //    }
    //    else
    //    {
    //        //if not first index, increment
    //        cheerleaderSelectedIndex--;
    //    }
    //    GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
    //    //Debug.Log("player selected : " + GameOptions.playerSelected);
    //}

    //public void changeSelectedCheerleaderDown()
    //{
    //    // if default index (first in list
    //    if (cheerleaderSelectedIndex == cheerleaderSelectedData.Count - 1)
    //    {
    //        cheerleaderSelectedIndex = 0;
    //    }
    //    else
    //    {
    //        //if not first index, increment
    //        cheerleaderSelectedIndex++;
    //    }
    //    GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;
    //    //Debug.Log("player selected : " + GameOptions.playerSelected);
    //}

    //public void changeSelectedLevelUp()
    //{
    //    // if default index (first in list), go to end of list
    //    if (levelSelectedIndex == 0)
    //    {
    //        levelSelectedIndex = levelSelectedData.Count - 1;
    //    }
    //    else
    //    {
    //        // if not first index, decrement
    //        levelSelectedIndex--;
    //    }
    //    GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    //}
    //public void changeSelectedLevelDown()
    //{
    //    // if default index (first in list
    //    if (levelSelectedIndex == levelSelectedData.Count - 1)
    //    {
    //        levelSelectedIndex = 0;
    //    }
    //    else
    //    {
    //        //if not first index, increment
    //        levelSelectedIndex++;
    //    }
    //    GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    //}
    //public void changeSelectedModeUp()
    //{
    //    // if default index (first in list), go to end of list
    //    if (modeSelectedIndex == 0)
    //    {
    //        modeSelectedIndex = modeSelectedData.Count - 1;
    //    }
    //    else
    //    {
    //        // if not first index, decrement
    //        modeSelectedIndex--;
    //    }
    //    GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
    //    GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;
    //}

    //public void changeSelectedModeDown()
    //{
    //    // if default index (first in list
    //    if (modeSelectedIndex == modeSelectedData.Count - 1)
    //    {
    //        modeSelectedIndex = 0;
    //    }
    //    else
    //    {
    //        //if not first index, increment
    //        modeSelectedIndex++;
    //    }

    //    GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
    //    GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;
    //}


    //public void loadScene()
    //{
    //    // for testing sceneswihtout loading from start screen
    //    GameOptions.gameModeHasBeenSelected = true;

    //    // apply selected player stats to game options, which will be loaded into Player on spawn
    //    setPlayerProfileStats();

    //    // update game options for game mode
    //    setGameOptions();

    //    // i create the string this way so that i can have a description of the level so i know what im opening
    //    string sceneName = GameOptions.levelSelected + "_" + levelSelectedData[levelSelectedIndex].LevelDescription;
    //    //Debug.Log("scene name : " + sceneName);

    //    // check if Player selected is locked
    //    if ((playerSelectedData[playerSelectedIndex].IsLocked || cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
    //        && !modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
    //    {
    //        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
    //        messageText.text = " Bruh, it's locked. pick something else";
    //        // turn off text display after 5 seconds
    //        StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
    //    }
    //    if ((!playerSelectedData[playerSelectedIndex].IsLocked && !cheerleaderSelectedData[cheerleaderSelectedIndex].IsLocked)
    //        || modeSelectedData[modeSelectedIndex].ModelDisplayName.ToLower().Contains("free"))
    //    {
    //        // load highscores before loading scene
    //        if (PlayerData.instance != null)
    //        {
    //            try
    //            {
    //                PlayerData.instance.loadStatsFromDatabase();
    //            }
    //            catch
    //            {
    //                return;
    //            }
    //        }
    //        SceneManager.LoadScene(sceneName);
    //    }
    //}

    //private void setPlayerProfileStats()
    //{
    //    StartScreenModeSelected temp = modeSelectedData[modeSelectedIndex];
    //    // need object name and playerid
    //    GameOptions.playerDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
    //    GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
    //    GameOptions.playerId = playerSelectedData[playerSelectedIndex].PlayerId;

    //    GameOptions.accuracy2pt = playerSelectedData[playerSelectedIndex].Accuracy2Pt;
    //    GameOptions.accuracy3pt = playerSelectedData[playerSelectedIndex].Accuracy3Pt;
    //    GameOptions.accuracy4pt = playerSelectedData[playerSelectedIndex].Accuracy4Pt;
    //    GameOptions.accuracy7pt = playerSelectedData[playerSelectedIndex].Accuracy7Pt;

    //    // if 3/4/All point contest, disable Luck/citical %
    //    if (temp.GameModeThreePointContest
    //        || temp.GameModeFourPointContest
    //        || temp.GameModeAllPointContest)
    //    {
    //        GameOptions.criticalPercent = 0;
    //    }
    //    else
    //    {
    //        GameOptions.criticalPercent = playerSelectedData[playerSelectedIndex].CriticalPercent;
    //    }
    //    GameOptions.jumpForce = playerSelectedData[playerSelectedIndex].JumpForce;
    //    GameOptions.speed = playerSelectedData[playerSelectedIndex].Speed;
    //    GameOptions.runSpeed = playerSelectedData[playerSelectedIndex].RunSpeed;
    //    GameOptions.runSpeedHasBall = playerSelectedData[playerSelectedIndex].RunSpeedHasBall;
    //    GameOptions.shootAngle = playerSelectedData[playerSelectedIndex].ShootAngle;
    //}

    //private void setGameOptions()
    //{
    //    GameOptions.playerId = playerSelectedData[playerSelectedIndex].PlayerId;
    //    GameOptions.playerDisplayName = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
    //    GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;

    //    GameOptions.levelSelected = levelSelectedData[levelSelectedIndex].LevelObjectName;
    //    GameOptions.levelId = levelSelectedData[levelSelectedIndex].LevelId;
    //    GameOptions.levelDisplayName = levelSelectedData[levelSelectedIndex].LevelDisplayName;


    //    GameOptions.gameModeSelectedId = modeSelectedData[modeSelectedIndex].ModeId;
    //    GameOptions.gameModeSelectedName = modeSelectedData[modeSelectedIndex].ModelDisplayName;

    //    GameOptions.gameModeRequiresCountDown = modeSelectedData[modeSelectedIndex].ModeRequiresCountDown;
    //    GameOptions.gameModeRequiresCounter = modeSelectedData[modeSelectedIndex].ModeRequiresCounter;

    //    GameOptions.gameModeRequiresShotMarkers3s = modeSelectedData[modeSelectedIndex].ModeRequiresShotMarkers3S;
    //    GameOptions.gameModeRequiresShotMarkers4s = modeSelectedData[modeSelectedIndex].ModeRequiresShotMarkers4S;

    //    GameOptions.gameModeThreePointContest = modeSelectedData[modeSelectedIndex].GameModeThreePointContest;
    //    GameOptions.gameModeFourPointContest = modeSelectedData[modeSelectedIndex].GameModeFourPointContest;
    //    GameOptions.gameModeAllPointContest = modeSelectedData[modeSelectedIndex].GameModeAllPointContest;

    //    if (modeSelectedData[modeSelectedIndex].CustomTimer > 0)
    //    {
    //        Debug.Log("modeSelectedData[modeSelectedIndex].CustomTimer : " + modeSelectedData[modeSelectedIndex].CustomTimer);
    //        GameOptions.customTimer = modeSelectedData[modeSelectedIndex].CustomTimer;
    //    }
    //    else
    //    {
    //        GameOptions.customTimer = 0;
    //    }

    //    GameOptions.gameModeRequiresMoneyBall = modeSelectedData[modeSelectedIndex].ModeRequiresMoneyBall;
    //    GameOptions.gameModeRequiresConsecutiveShot = modeSelectedData[modeSelectedIndex].ModeRequiresConsecutiveShots;

    //    GameOptions.cheerleaderDisplayName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderDisplayName;
    //    GameOptions.cheerleaderId = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderId;
    //    GameOptions.cheerleaderObjectName = cheerleaderSelectedData[cheerleaderSelectedIndex].CheerleaderObjectName;

    //    GameOptions.trafficEnabled = trafficEnabled;

    //    GameOptions.applicationVersion = Application.version;
    //    GameOptions.operatingSystemVersion = SystemInfo.operatingSystem;


    //    // send current selected options to game options for next load on start manager
    //    GameOptions.playerSelectedIndex = playerSelectedIndex;
    //    GameOptions.cheerleaderSelectedIndex = cheerleaderSelectedIndex;
    //    GameOptions.levelSelectedIndex = levelSelectedIndex;
    //    GameOptions.modeSelectedIndex = modeSelectedIndex;
    //    GameOptions.trafficEnabled = trafficEnabled;
    //}

    //public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    //{
    //    yield return new WaitForSecondsRealtime(seconds);
    //    Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
    //    messageText.text = "";
    //}

    //public void loadScene(string sceneName)
    //{
    //    SceneManager.LoadScene(sceneName);
    //}

    //public static string PlayerSelectOptionButtonName => playerSelectOptionButtonName;

    //public static string CheerleaderSelectOptionButtonName => cheerleaderSelectOptionButtonName;

    //public static string LevelSelectOptionButtonName => levelSelectOptionButtonName;

    //public static string ModeSelectOptionButtonName => modeSelectOptionButtonName;

    //public static string TrafficSelectOptionName => trafficSelectOptionName;

    //public static string StartButtonName => startButtonName;

    //public static string StatsMenuButtonName => statsMenuButtonName;

    //public static string QuitButtonName => quitButtonName;

    //public static string StatsMenuSceneName => statsMenuSceneName;

    //public Button LevelSelectButton { get => levelSelectButton; set => levelSelectButton = value; }
    //public Button TrafficSelectButton { get => trafficSelectButton; set => trafficSelectButton = value; }
    //public Button PlayerSelectButton1 { get => playerSelectButton; set => playerSelectButton = value; }
    //public Button CheerleaderSelectButton1 { get => CheerleaderSelectButton; set => CheerleaderSelectButton = value; }
    //public Button ModeSelectButton { get => modeSelectButton; set => modeSelectButton = value; }
}
