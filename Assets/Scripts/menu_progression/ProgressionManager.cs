﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ProgressionManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;
    // option select buttons, this will be disabled with touch input
    [SerializeField] Button playerSelectButton;

    //list of all shooter profiles with player data
    private List<CharacterProfile> playerSelectedData;

    // list off cheerleader profile data
    private List<CheerleaderProfile> cheerleaderSelectedData;

    //player selected display
    private Text playerSelectOptionText;
    private Image playerSelectOptionImage;
    private Text playerProgressionStatsText;
    private Text playerProgressionUpdatePointsText;
    private Text progression3Accuracy;
    private Text progression4Accuracy;
    private Text progression7Accuracy;
    private Text progressionRange;
    private Text progressionRelease;
    private Text progressionSpeed;
    private Text progressionJump;
    private Text progressionLuck;

    private Text bonusReleaseText;
    private Text bonusRangeText;
    private Text bonusLuckText;

    private Text addTo3Text;
    private Text addTo4Text;
    private Text addTo7Text;

    //const object names
    private const string startButtonName = "press_start";
    private const string statsMenuButtonName = "stats_menu";
    private const string quitButtonName = "quit_game";
    //private const string optionsMenuButtonName = "options_menu";

    // scene name
    private const string sceneStatsName = "level_00_stats";
    private const string sceneLoadingName = "level_00_loading";
    private const string sceneStartName = "level_00_start";

    // button names
    private const string playerSelectButtonName = "player_select_button";
    private const string playerSelectOptionButtonName = "player_selected_name";
    //private const string playerSelectStatsObjectName = "player_selected_stats_numbers";
    private const string playerSelectImageObjectName = "player_selected_image";
    //private const string playerSelectStatsCategoryName = "player_selected_stats_category";
    //private const string playerBonusName = "current_player_bonus";

    //private const string playerProgressionName = "player_progression";
    private const string playerProgressionStatsName = "player_progression_stats";
    private const string playerProgressionPointsAvailableName = "player_points_available";

    private const string progression3AccuracyName = "3accuracyButton";
    private const string progression4AccuracyName = "4accuracyButton";
    private const string progression7AccuracyName = "7accuracyButton";

    private const string releaseBonusName = "release_bonus";
    private const string rangeBonusName = "range_bonus";
    private const string luckBonusName = "luck_bonus";

    private const string confirmButtonName = "confirm_button";
    private const string cancelButtonName = "cancel_button";
    private const string saveButtonName = "save_button";
    private const string resetButtonName = "reset_button";


    private int playerSelectedIndex;
    [SerializeField] int experienceRequiredForNextLevel;

    public PlayerControls controls;
    public static ProgressionManager instance;

    ProgressionState progressionState;
    // flags
    bool buttonPressed = false;
    bool dataLoaded = false;

    // confirm save dialogue
    bool confirmationDialogueBoxEnabled = false;
    GameObject confirmationDialogueBox;

    GameObject prevSelectedObject;

    public int ExperienceRequiredForNextLevel { get => experienceRequiredForNextLevel; set => experienceRequiredForNextLevel = value; }
    public static string PlayerSelectOptionButtonName => playerSelectOptionButtonName;
    public static string StartButtonName => startButtonName;
    public static string StatsMenuButtonName => statsMenuButtonName;
    public static string QuitButtonName => quitButtonName;
    public static string Progression3AccuracyName => progression3AccuracyName;
    public static string Progression4AccuracyName => progression4AccuracyName;
    public static string Progression7AccuracyName => progression7AccuracyName;
    public static string ConfirmButtonName => confirmButtonName;
    public static string CancelButtonName => cancelButtonName;
    public static string SaveButtonName => saveButtonName;
    public static string ResetButtonName => resetButtonName;
    public ProgressionState ProgressionState { get => progressionState; set => progressionState = value; }
    public bool DataLoaded { get => dataLoaded; }
    public static string SceneStatsName => sceneStatsName;
    public static string SceneStartName => sceneStartName;

    public enum UpdateType
    {
        Add,
        Subtract,
        Reset
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
        // disable confirmation dialogue
        confirmationDialogueBox = GameObject.Find("confirm_update");
        confirmationDialogueBox.SetActive(confirmationDialogueBoxEnabled);

        progressionState = GetComponent<ProgressionState>();
        // dont destroy on load / check for duplicate instance
        //destroyInstanceIfAlreadyExists();
        StartCoroutine(getLoadedData());

        controls = new PlayerControls();
        // find all button / text / etc and assign to variables
        getUiObjectReferences();

        //default index for player selected
        playerSelectedIndex = GameOptions.playerSelectedIndex;
    }

    // Start is called before the first frame update
    void Start()
    {
        AnaylticsManager.MenuProgressionLoaded();

        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        // default display
        StartCoroutine(InitializeDisplay());
    }

    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }
        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";

        // ================================== footer buttons =====================================================================
        // start button | start game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(startButtonName))
        {
            confirmChanges();
            loadScene(sceneStartName);
        }
        // quit button | quit game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(quitButtonName))
        {
            confirmChanges();
            Application.Quit();
        }
        // stats menu button | load stats menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(statsMenuButtonName))
        {
            confirmChanges();
            loadScene(sceneStatsName);
        }

        // ================================== navigation =====================================================================

        // right, go to change options
        if (controls.UINavigation.Right.triggered
            && EventSystem.current.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnRight() != null
            && currentHighlightedButton.Equals(playerSelectButtonName))
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                .GetComponent<Button>().FindSelectableOnRight().gameObject);
        }

        // left, return to option select
        if (controls.UINavigation.Left.triggered
            && currentHighlightedButton.Equals(playerSelectButtonName))
        {
            // check if button exists. if no selectable on left, throws null object exception
            if (EventSystem.current.currentSelectedGameObject.GetComponent<Button>().FindSelectableOnLeft() != null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.currentSelectedGameObject
                    .GetComponent<Button>().FindSelectableOnLeft().gameObject);
            }
        }
        // ================================== confirmation dialogue / save / reset ===================================================

        // save button triggered
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(saveButtonName))
        {
            saveChanges();
        }
        // reset button triggered
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(resetButtonName))
        {
            // enable confirmation object
            // set selected object to confirm button
            //confirmationDialogueBox.SetActive(true);
            resetChanges();

            // reset stats
            //EventSystem.current.SetSelectedGameObject(GameObject.Find(progression3AccuracyName).gameObject);
        }
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(confirmButtonName))
        {
            confirmChanges();
        }
        // cancel popup
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(cancelButtonName))
        {
            // do nothing, continue state

            cancelChanges();
        }
        // ================================== change options =============================================================
        // up, change options
        if (controls.UINavigation.Up.triggered && !buttonPressed
            && currentHighlightedButton.Equals(playerSelectOptionButtonName))
        {
            changePlayerUp();
        }
        // down, change option
        if (controls.UINavigation.Down.triggered && !buttonPressed
            && currentHighlightedButton.Equals(playerSelectOptionButtonName))
        {
            changePlayerDown();

        }
        // add a point to selected category
        if (!buttonPressed && dataLoaded
            && progressionState.PointsAvailable > 0
            && controls.UINavigation.Submit.triggered)
        {
            addPoint();
        }
        // subtract a point
        if (!buttonPressed && dataLoaded
            && controls.UINavigation.Cancel.triggered)
        {
            subtractPoint();
        }
    }

    public void changePlayerUp()
    {
        buttonPressed = true;
        try
        {
            changeSelectedPlayerUp();
            initializePlayerDisplay();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
        buttonPressed = false;
    }

    public void changePlayerDown()
    {
        buttonPressed = true;
        try
        {
            changeSelectedPlayerDown();
            initializePlayerDisplay();
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
        buttonPressed = false;
    }

    public void addPoint()
    {
        buttonPressed = true;
        if (currentHighlightedButton.Equals(progression3AccuracyName))
        {
            updateThreeAccuracy(UpdateType.Add);
        }
        if (currentHighlightedButton.Equals(progression4AccuracyName))
        {
            updateFourAccuracy(UpdateType.Add);
        }
        if (currentHighlightedButton.Equals(progression7AccuracyName))
        {
            updateSevenAccuracy(UpdateType.Add);
        }
        initializePlayerDisplay();
        buttonPressed = false;
    }

    public void subtractPoint()
    {
        buttonPressed = true;
        if (currentHighlightedButton.Equals(progression3AccuracyName) && progressionState.AddTo3 > 0)
        {
            updateThreeAccuracy(UpdateType.Subtract);
        }
        if (currentHighlightedButton.Equals(progression4AccuracyName) && progressionState.AddTo4 > 0)
        {
            updateFourAccuracy(UpdateType.Subtract);
        }
        if (currentHighlightedButton.Equals(progression7AccuracyName) && progressionState.AddTo7 > 0)
        {
            updateSevenAccuracy(UpdateType.Subtract);
        }
        initializePlayerDisplay();
        buttonPressed = false;
    }

    public void saveChanges()
    {
        confirmationDialogueBox.SetActive(true);
        EventSystem.current.SetSelectedGameObject(GameObject.Find(confirmButtonName).gameObject);
    }

    public void cancelChanges()
    {
        confirmationDialogueBox.SetActive(false);
        EventSystem.current.SetSelectedGameObject(GameObject.Find(progression3AccuracyName).gameObject);
    }

    public void resetChanges()
    {
        resetUpdatePoints();
        // reset player state
        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
        // display
        initializePlayerDisplay();

        EventSystem.current.SetSelectedGameObject(GameObject.Find(progression3AccuracyName).gameObject);
    }

    public void confirmChanges()
    {
        playerSelectedData[playerSelectedIndex].Accuracy3Pt = progressionState.Accuracy3;
        playerSelectedData[playerSelectedIndex].Accuracy4Pt = progressionState.Accuracy4;
        playerSelectedData[playerSelectedIndex].Accuracy7Pt = progressionState.Accuracy7;
        playerSelectedData[playerSelectedIndex].Range = progressionState.Range;
        playerSelectedData[playerSelectedIndex].Release = progressionState.Release;
        playerSelectedData[playerSelectedIndex].Luck = progressionState.Luck;
        playerSelectedData[playerSelectedIndex].PointsAvailable = progressionState.PointsAvailable;

        progressionState.PointsUsedThisSession =
            progressionState.AddTo3
            + progressionState.AddTo4
            + progressionState.AddTo7;

        playerSelectedData[playerSelectedIndex].PointsUsed += progressionState.PointsUsedThisSession;

        // save to DB
        DBHelper.instance.UpdateCharacterProfile(playerSelectedData[playerSelectedIndex]);
        // disable pop up
        confirmationDialogueBox.SetActive(false);
        // reset points
        resetUpdatePoints();
        // reset player state
        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
        // display
        initializePlayerDisplay();
        // reset stats
        EventSystem.current.SetSelectedGameObject(GameObject.Find(progression3AccuracyName).gameObject);
    }

    public void resetUpdatePoints()
    {
        //int originalPointsAvailable = progressionState.AddTo3 + progressionState.AddTo4 + progressionState.AddTo7;
        progressionState.AddTo3 = 0;
        progressionState.AddTo4 = 0;
        progressionState.AddTo7 = 0;

        progressionState.Accuracy3 = 0;
        progressionState.Accuracy4 = 0;
        progressionState.Accuracy7 = 0;

        progressionState.Luck = 0;
        progressionState.Range = 0;
        progressionState.Release = 0;

        addTo3Text.text = "--";
        addTo4Text.text = "--";
        addTo7Text.text = "--";

        progressionState.PointsAvailable = 0;
        progressionState.PointsUsedThisSession = 0;
    }

    public void updateThreeAccuracy(UpdateType updateType)
    {
        if (progressionState.Accuracy3 < progressionState.MaxThreeAccuraccy)
        {
            switch (updateType)
            {
                case UpdateType.Add:
                    {
                        progressionState.AddTo3++;
                        addTo3Text.text = (progressionState.AddTo3) > 0 ? ("+" + progressionState.AddTo3.ToString()) : ("--");
                        progressionState.PointsAvailable--;
                        progressionState.PointsUsedThisSession++;
                        break;
                    }
                case UpdateType.Subtract:
                    {
                        progressionState.AddTo3--;
                        addTo3Text.text = (progressionState.AddTo3) > 0 ? ("+" + progressionState.AddTo3.ToString()) : ("--");
                        progressionState.PointsAvailable++;
                        progressionState.PointsUsedThisSession--;
                        break;
                    }
                default: break;
            }
            updateStaticCharacterStatistics(playerSelectedData[playerSelectedIndex]);
            initializePlayerDisplay();
        }
    }
    public void updateFourAccuracy(UpdateType updateType)
    {
        if (progressionState.Accuracy4 < progressionState.MaxFourAccuraccy)
        {
            switch (updateType)
            {
                case UpdateType.Add:
                    {
                        progressionState.AddTo4++;
                        addTo4Text.text = (progressionState.AddTo4) > 0 ? ("+" + progressionState.AddTo4.ToString()) : ("--");
                        progressionState.PointsAvailable--;
                        progressionState.PointsUsedThisSession++;
                        break;
                    }
                case UpdateType.Subtract:
                    {
                        progressionState.AddTo4--;
                        addTo4Text.text = (progressionState.AddTo4) > 0 ? ("+" + progressionState.AddTo4.ToString()) : ("--");
                        progressionState.PointsAvailable++;
                        progressionState.PointsUsedThisSession--;
                        break;
                    }
                default: break;
            }
            updateStaticCharacterStatistics(playerSelectedData[playerSelectedIndex]);
            initializePlayerDisplay();
        }
    }
    public void updateSevenAccuracy(UpdateType updateType)
    {
        if (progressionState.Accuracy7 < progressionState.MaxSevenAccuraccy)
        {
            switch (updateType)
            {
                case UpdateType.Add:
                    {
                        progressionState.AddTo7++;
                        addTo7Text.text = (progressionState.AddTo7) > 0 ? ("+" + progressionState.AddTo7.ToString()) : ("--");
                        progressionState.PointsAvailable--;
                        progressionState.PointsUsedThisSession++;

                        break;
                    }
                case UpdateType.Subtract:
                    {
                        progressionState.AddTo7--;
                        addTo7Text.text = (progressionState.AddTo7) > 0 ? ("+" + progressionState.AddTo7.ToString()) : ("--");
                        progressionState.PointsAvailable++;
                        progressionState.PointsUsedThisSession--;
                        break;
                    }
                default: break;
            }
            updateStaticCharacterStatistics(playerSelectedData[playerSelectedIndex]);
            initializePlayerDisplay();
        }
    }


    private void loadScene()
    {
        throw new NotImplementedException();
    }

    IEnumerator UpdateLevelAndExperienceFromDatabase()
    {
        yield return new WaitUntil(() => dataLoaded);

        foreach (CharacterProfile s in playerSelectedData)
        {
            s.Experience = DBHelper.instance.getIntValueFromTableByFieldAndCharId("CharacterProfile", "experience", s.PlayerId);
            s.Level = DBHelper.instance.getIntValueFromTableByFieldAndCharId("CharacterProfile", "level", s.PlayerId);
        }
    }

    IEnumerator getLoadedData()
    {
        if (LoadedData.instance != null)
        {
            yield return new WaitUntil(() => LoadedData.instance.PlayerSelectedData != null);

            playerSelectedData = LoadedData.instance.PlayerSelectedData;

            yield return new WaitUntil(() => LoadedData.instance.CheerleaderSelectedData != null);
            cheerleaderSelectedData = LoadedData.instance.CheerleaderSelectedData;

            if (playerSelectedData != null
                && cheerleaderSelectedData != null)
            {
                dataLoaded = true;
            }
        }
        else
        {
            if (String.IsNullOrEmpty(GameOptions.previousSceneName))
            {
                GameOptions.previousSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(sceneLoadingName);
            }
            else
            {
                GameOptions.previousSceneName = SceneManager.GetActiveScene().name;
                SceneManager.LoadScene(GameOptions.previousSceneName);
            }
        }
    }

    IEnumerator InitializeDisplay()
    {
        //Debug.Log("------------------------------- start manager InitializeDisplay");
        yield return new WaitUntil(() => dataLoaded);

        // display default data
        progressionState.clearState();
        // get default character loaded to progression state
        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
        // load static upgrade statistics
        updateStaticCharacterStatistics(playerSelectedData[playerSelectedIndex]);
        // init display
        initializePlayerDisplay();

    }
    // ============================  get UI buttons / text references ==============================
    private void getUiObjectReferences()
    {

        //Debug.Log("getUiObjectReferences()");
        // buttons to disable for touch input
        playerSelectButton = GameObject.Find(playerSelectButtonName).GetComponent<Button>();

        // player object with lock texture and unlock text
        playerSelectOptionText = GameObject.Find(playerSelectOptionButtonName).GetComponent<Text>();
        playerSelectOptionImage = GameObject.Find(playerSelectImageObjectName).GetComponent<Image>();
        playerProgressionStatsText = GameObject.Find(playerProgressionStatsName).GetComponent<Text>();
        playerProgressionUpdatePointsText = GameObject.Find(playerProgressionPointsAvailableName).GetComponent<Text>();

        progression3Accuracy = GameObject.Find("3accuracy").GetComponent<Text>();
        progression4Accuracy = GameObject.Find("4accuracy").GetComponent<Text>();
        progression7Accuracy = GameObject.Find("7accuracy").GetComponent<Text>();
        progressionRange = GameObject.Find("range").GetComponent<Text>();
        progressionRelease = GameObject.Find("release").GetComponent<Text>();
        progressionSpeed = GameObject.Find("speed").GetComponent<Text>();
        progressionJump = GameObject.Find("jump").GetComponent<Text>();
        progressionLuck = GameObject.Find("luck").GetComponent<Text>();

        bonusReleaseText = GameObject.Find(releaseBonusName).GetComponent<Text>();
        bonusRangeText = GameObject.Find(rangeBonusName).GetComponent<Text>();
        bonusLuckText = GameObject.Find(luckBonusName).GetComponent<Text>();

        addTo3Text = GameObject.Find(progression3AccuracyName).GetComponent<Text>();
        addTo4Text = GameObject.Find(progression4AccuracyName).GetComponent<Text>();
        addTo7Text = GameObject.Find(progression7AccuracyName).GetComponent<Text>();
    }
    public void disableButtonsNotUsedForTouchInput()
    {
        //Debug.Log("disable buttons for touch");
        //levelSelectButton.enabled = false;
        //trafficSelectButton.enabled = false;
        playerSelectButton.enabled = false;
        //CheerleaderSelectButton.enabled = false;
        //modeSelectButton.enabled = false;
    }


    public void initializePlayerDisplay()
    {
        //Debug.Log("initializePlayerDisplay()");
        try
        {
            // name and portrait
            playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
            playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;

            // update text display static update stats (range, release, luck)
            //if (playerSelectedData[playerSelectedIndex].PointsAvailable > 0)  

            if (progressionState.PointsUsedThisSession > 0)
            {
                if (progressionState.Release < progressionState.MaxReleaseAccuraccy)
                {
                    bonusReleaseText.text = "+" + progressionState.AddToRelease;
                }
                else
                {
                    bonusReleaseText.text = "MAX";
                }
                if (progressionState.Luck < progressionState.MaxLuck)
                {
                    bonusLuckText.text = "+" + progressionState.AddToLuck;
                }
                else
                {
                    bonusLuckText.text = "MAX";
                }
                bonusRangeText.text = "+" + progressionState.AddToRange;
            }
            else
            {
                bonusReleaseText.text = "";
                bonusRangeText.text = "";
                bonusLuckText.text = "";
            }
            //// luck point only available every 3rd level
            //bonusLuckText.text = progressionState.AddToLuck == 0
            //    ? bonusLuckText.text = ""
            //    : "+" + progressionState.AddToLuck.ToString();

            // set text displays

            // these DO NOT have max limits
            progressionRange.text = progressionState.Range.ToString("F0") + " ft";
            progressionSpeed.text = playerSelectedData[playerSelectedIndex].calculateSpeedToPercent().ToString("F0");
            progressionJump.text = playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0");

            // these DO have max limits
            //release
            if (progressionState.Release < progressionState.MaxReleaseAccuraccy)
            {
                progressionRelease.text = progressionState.Release.ToString("F0");
            }
            else
            {
                progressionRelease.text = progressionState.Release.ToString("F0") + " MAX";
            }
            // luck
            if (progressionState.Luck < progressionState.MaxLuck)
            {
                progressionLuck.text = progressionState.Luck.ToString("F0");
            }
            else
            {
                progressionRelease.text = progressionState.Luck.ToString("F0") + " MAX";
            }
            // 3 accuracy
            if (progressionState.Accuracy3 < progressionState.MaxThreeAccuraccy)
            {
                progression3Accuracy.text = progressionState.Accuracy3.ToString("F0");
            }
            else
            {
                progression3Accuracy.text = progressionState.Accuracy3.ToString("F0") + " MAX";
            }
            // 4 accuracy
            if (progressionState.Accuracy4 < progressionState.MaxFourAccuraccy)
            {
                progression4Accuracy.text = progressionState.Accuracy4.ToString("F0");
            }
            // 7 accuracy
            else
            {
                progression4Accuracy.text = progressionState.Accuracy4.ToString("F0") + " MAX";
            }
            if (progressionState.Accuracy7 < progressionState.MaxSevenAccuraccy)
            {
                progression7Accuracy.text = progressionState.Accuracy7.ToString("F0");
            }
            else
            {
                progression7Accuracy.text = progressionState.Accuracy7.ToString("F0") + " MAX";
            }

            // get level by experience
            progressionState.Level = progressionState.Experience / experienceRequiredForNextLevel;
            //                      next level      x    exp to gain a level (ex 3000)  -   current amount of exp
            int nextlvl = ((progressionState.Level + 1) * experienceRequiredForNextLevel) - progressionState.Experience;
            // display lvl, exp, exp for next lvl
            playerProgressionStatsText.text = progressionState.Level.ToString("F0") + "\n"
                + progressionState.Experience.ToString("F0") + "\n"
                + nextlvl.ToString("F0") + "\n";
            playerProgressionUpdatePointsText.text = "points available : " + progressionState.PointsAvailable.ToString();
            // not sure what this is for but im not gonna touch it yet
            GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        }
        catch (Exception e)
        {
            Debug.Log("ERROR : " + e);
            return;
        }
    }

    // ============================  footer options activate - load scene/stats/quit/etc ==============================

    private void updateStaticCharacterStatistics(CharacterProfile originalCharState)
    {
        //Debug.Log("updateStaticCharacterStatistics");

        int lastUpdate = originalCharState.Level - originalCharState.PointsAvailable;
        int luckPointsAvailable = (originalCharState.Level / 3) - (lastUpdate / 3);

        // auto add to luck if available
        if (progressionState.PointsUsedThisSession <= luckPointsAvailable
            && progressionState.Luck < progressionState.MaxLuck)
        {
            progressionState.AddToLuck = progressionState.PointsUsedThisSession;
        }
        else
        {
            progressionState.AddToLuck = 0;
        }
        // if luck less than max luck, can add. else. luck is max
        if (progressionState.Luck < progressionState.MaxLuck)
        {
            progressionState.Luck = originalCharState.Luck + progressionState.AddToLuck;
        }
        else
        {
            progressionState.Luck = progressionState.MaxLuck;
        }
        // if release > max
        if (progressionState.Release < progressionState.MaxReleaseAccuraccy)
        {
            progressionState.AddToRelease = progressionState.PointsUsedThisSession;
            progressionState.Release = (int)(originalCharState.Release + progressionState.AddToRelease);
        }
        else
        {
            progressionState.Release = progressionState.MaxReleaseAccuraccy;
        }
        // if 3 acc > max
        if (progressionState.Accuracy3 < progressionState.MaxThreeAccuraccy)
        {
            progressionState.Accuracy3 = (int)(originalCharState.Accuracy3Pt + progressionState.AddTo3);
        }
        else
        {
            progressionState.Accuracy3 = progressionState.MaxThreeAccuraccy;
        }
        // if 4 acc > max
        if (progressionState.Accuracy4 < progressionState.MaxFourAccuraccy)
        {
            progressionState.Accuracy4 = (int)(originalCharState.Accuracy4Pt + progressionState.AddTo4);
        }
        else
        {
            progressionState.Accuracy4 = progressionState.MaxFourAccuraccy;
        }
        // if 7 acc > max
        if (progressionState.Accuracy7 < progressionState.MaxSevenAccuraccy)
        {
            progressionState.Accuracy7 = (int)(originalCharState.Accuracy7Pt + progressionState.AddTo7);
        }
        else
        {
            progressionState.Accuracy7 = progressionState.MaxSevenAccuraccy;
        }
        progressionState.AddToRange = progressionState.PointsUsedThisSession * 5;
        progressionState.Range = (int)(originalCharState.Range + progressionState.AddToRange);
    }

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    //// ============================  message display ==============================
    //// used in this context to display if item is locked

    //public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    //{
    //    yield return new WaitForSecondsRealtime(seconds);
    //    Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
    //    messageText.text = "";
    //}

    // ============================  navigation functions ==============================
    private void changeSelectedPlayerUp()
    {
        //progressionState.clearState();
        resetUpdatePoints();

        playerSelectedIndex =
            (playerSelectedIndex == 0
            ? playerSelectedData.Count - 1
            : playerSelectedIndex -= 1);

        // init update state
        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
        // update static statistsics
        updateStaticCharacterStatistics(playerSelectedData[playerSelectedIndex]);
    }
    private void changeSelectedPlayerDown()
    {
        //progressionState.clearState();
        resetUpdatePoints();

        addTo3Text.text = "--";
        addTo4Text.text = "--";
        addTo7Text.text = "--";

        playerSelectedIndex =
            ((playerSelectedIndex == playerSelectedData.Count - 1)
            ? playerSelectedIndex = 0
            : playerSelectedIndex += 1);

        // init update state
        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
        //update static statistsics
        updateStaticCharacterStatistics(playerSelectedData[playerSelectedIndex]);

    }
}
