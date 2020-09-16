using System;
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

    //list of all shooter profiles with player data
    private List<CharacterProfile> playerSelectedData;

    // list off cheerleader profile data
    private List<CheerleaderProfile> cheerleaderSelectedData;

    // option select buttons, this will be disabled with touch input
    Button playerSelectButton;

    //player selected display
    private Text playerSelectOptionText;
    private Image playerSelectOptionImage;
    //private Text playerSelectOptionStatsText;
    //private Text playerSelectCategoryStatsText;
    //private Text playerProgressionCategoryText;
    private Text playerProgressionStatsText;

    private Text playerProgressionUpdatePointsText;
    //private Text playerProgressionUpdateBonusText;

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

    [SerializeField] private Text addTo3Text;
    [SerializeField] private Text addTo4Text;
    [SerializeField] private Text addTo7Text;


    //const object names
    private const string startButtonName = "press_start";
    private const string statsMenuButtonName = "stats_menu";
    private const string quitButtonName = "quit_game";
    private const string optionsMenuButtonName = "options_menu";

    // scene name
    private const string statsMenuSceneName = "level_00_stats";

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

    private const string loadScreenSceneName = "level_00_loading";

    private const string confirmButtonName = "confirm_button";
    private const string cancelButtonName = "cancel_button";
    private const string saveButtonName = "save_button";
    private const string resetButtonName = "reset_button";

    private int playerSelectedIndex;

    public PlayerControls controls;

    //public CharacterUpdateState characterUpdateState;

    public static ProgressionManager instance;

    ProgressionState progressionState = new ProgressionState();

    bool buttonPressed = false;
    bool dataLoaded = false;

    bool confirmationDialogueBoxEnabled = false;
    GameObject confirmationDialogueBox;

    enum UpdateType
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
        // disable confirmation dialogue
        confirmationDialogueBox = GameObject.Find("confirm_update");
        confirmationDialogueBox.SetActive(confirmationDialogueBoxEnabled);

        // dont destroy on load / check for duplicate instance
        //destroyInstanceIfAlreadyExists();
        StartCoroutine(getLoadedData());

        controls = new PlayerControls();
        // find all button / text / etc and assign to variables
        getUiObjectReferences();

        //default index for player selected
        playerSelectedIndex = GameOptions.playerSelectedIndex;

        // update experience and levels
        // recommended here because experience will be gained after every game played
        StartCoroutine(UpdateLevelAndExperienceFromDatabase());
    }

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(InitializeDisplay());
    }

    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
            }
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
        }

        // ================================== footer buttons =====================================================================
        // start button | start game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(startButtonName))
        {
            loadScene();
        }
        // quit button | quit game
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(quitButtonName))
        {
            Application.Quit();
        }
        // stats menu button | load stats menu
        if ((controls.UINavigation.Submit.triggered
             || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(statsMenuButtonName))
        {
            loadScene(statsMenuSceneName);
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
            confirmationDialogueBox.SetActive(true);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(confirmButtonName).gameObject);
        }
        // reset button triggered
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(resetButtonName))
        {
            // enable confirmation object
            // set selected object to confirm button
            confirmationDialogueBox.SetActive(true);

            // reset stats
            EventSystem.current.SetSelectedGameObject(GameObject.Find(saveButtonName).gameObject);
        }
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(confirmButtonName))
        {

            // new progressionState --> playerselecetdata[playerSelectedIndex]
            // dbhelper save characterprofile
            // reload data from database
            // initialize display
            playerSelectedData[playerSelectedIndex].Accuracy3Pt = progressionState.Accuracy3;
            playerSelectedData[playerSelectedIndex].Accuracy4Pt = progressionState.Accuracy4;
            playerSelectedData[playerSelectedIndex].Accuracy7Pt = progressionState.Accuracy7;
            playerSelectedData[playerSelectedIndex].Range = progressionState.Range;
            playerSelectedData[playerSelectedIndex].Release = progressionState.Release;
            playerSelectedData[playerSelectedIndex].Luck = progressionState.Luck;

            confirmationDialogueBox.SetActive(false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(saveButtonName).gameObject);
        }
        if (controls.UINavigation.Submit.triggered && currentHighlightedButton.Equals(cancelButtonName))
        {
            
            // do nothing, continue state

            confirmationDialogueBox.SetActive(false);
            EventSystem.current.SetSelectedGameObject(GameObject.Find(saveButtonName).gameObject);
        }

        // ================================== change options =============================================================
        // up, change options
        if (controls.UINavigation.Up.triggered && !buttonPressed
            && currentHighlightedButton.Equals(playerSelectOptionButtonName))
        {
            buttonPressed = true;
            try
            {
                changeSelectedPlayerUp();
                initializePlayerDisplay();
            }
            catch
            {
                return;
            }
            buttonPressed = false;
        }
        // down, change option
        if (controls.UINavigation.Down.triggered && !buttonPressed
            && currentHighlightedButton.Equals(playerSelectOptionButtonName))
        {
            buttonPressed = true;
            try
            {
                changeSelectedPlayerDown();
                initializePlayerDisplay();
            }
            catch
            {
                return;
            }
            buttonPressed = false;
        }
        // add a point to selected category
        if (!buttonPressed && dataLoaded
            && progressionState.PointsAvailable > 0
            && controls.UINavigation.Submit.triggered)
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
            buttonPressed = false;
        }
        // subtract a point
        if (!buttonPressed && dataLoaded
            && controls.UINavigation.Cancel.triggered)
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
            buttonPressed = false;
        }
    }
    private void resetUpdatePoints()
    {
        //int originalPointsAvailable = progressionState.AddTo3 + progressionState.AddTo4 + progressionState.AddTo7;
        progressionState.AddTo3 = 0;
        progressionState.AddTo4 = 0;
        progressionState.AddTo4 = 0;

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
        //progressionState.PointsAvailable = originalPointsAvailable;
    }

    private void updateThreeAccuracy(UpdateType updateType)
    {
        switch (updateType)
        {
            case UpdateType.Add:
                {
                    progressionState.AddTo3++;
                    addTo3Text.text = (progressionState.AddTo3) > 0 ? ("+" + progressionState.AddTo3.ToString()) : ("--");
                    progressionState.PointsAvailable--;

                    break;
                }
            case UpdateType.Subtract:
                {
                    progressionState.AddTo3--;
                    addTo3Text.text = (progressionState.AddTo3) > 0 ? ("+" + progressionState.AddTo3.ToString()) : ("--");
                    progressionState.PointsAvailable++;
                    break;
                }
            default: break;
        }
        initializePlayerDisplay();
    }
    private void updateFourAccuracy(UpdateType updateType)
    {
        switch (updateType)
        {
            case UpdateType.Add:
                {
                    progressionState.AddTo4++;
                    addTo4Text.text = (progressionState.AddTo4) > 0 ? ("+" + progressionState.AddTo4.ToString()) : ("--");
                    progressionState.PointsAvailable--;
                    break;
                }
            case UpdateType.Subtract:
                {
                    progressionState.AddTo4--;
                    addTo4Text.text = (progressionState.AddTo4) > 0 ? ("+" + progressionState.AddTo4.ToString()) : ("--");
                    progressionState.PointsAvailable++;
                    break;
                }
            default: break;
        }
        initializePlayerDisplay();
    }
    private void updateSevenAccuracy(UpdateType updateType)
    {
        switch (updateType)
        {
            case UpdateType.Add:
                {
                    progressionState.AddTo7++;
                    addTo7Text.text = (progressionState.AddTo7) > 0 ? ("+" + progressionState.AddTo7.ToString()) : ("--");
                    progressionState.PointsAvailable--;

                    break;
                }
            case UpdateType.Subtract:
                {
                    progressionState.AddTo7--;
                    addTo7Text.text = (progressionState.AddTo7) > 0 ? ("+" + progressionState.AddTo7.ToString()) : ("--");
                    progressionState.PointsAvailable++;
                    break;
                }
            default: break;
        }
        initializePlayerDisplay();
    }


    private void loadScene()
    {
        throw new NotImplementedException();
    }

    IEnumerator UpdateLevelAndExperienceFromDatabase()
    {
        yield return new WaitUntil(() => dataLoaded);

        Debug.Log("updateLevelAndExperienceFromDatabase");
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
                SceneManager.LoadScene(loadScreenSceneName);
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
        Debug.Log("------------------------------- start manager InitializeDisplay");
        yield return new WaitUntil(() => dataLoaded);
        // display default data
        progressionState.clearState();
        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
        initializePlayerDisplay();

    }
    // ============================  get UI buttons / text references ==============================
    private void getUiObjectReferences()
    {
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
        //levelSelectButton.enabled = false;
        //trafficSelectButton.enabled = false;
        playerSelectButton.enabled = false;
        //CheerleaderSelectButton.enabled = false;
        //modeSelectButton.enabled = false;
    }


    public void initializePlayerDisplay()
    {
        try
        {

            // use original player data. static data
            int lastUpdate = playerSelectedData[playerSelectedIndex].Level - playerSelectedData[playerSelectedIndex].PointsAvailable;
            int luckPointsAvailable = (playerSelectedData[playerSelectedIndex].Level / 3) - (lastUpdate / 3);

            if (playerSelectedData[playerSelectedIndex].PointsAvailable > 0)
            {
                bonusReleaseText.text = "+" + playerSelectedData[playerSelectedIndex].PointsAvailable.ToString();
                bonusRangeText.text = "+" + (playerSelectedData[playerSelectedIndex].PointsAvailable * 5).ToString();
            }
            else
            {
                bonusReleaseText.text = "";
                bonusRangeText.text = "";
            }
            // luck point only available every 3rd level
            bonusLuckText.text = luckPointsAvailable == 0
                ? bonusLuckText.text = ""
                : "+" + luckPointsAvailable.ToString();

            progressionRange.text = (playerSelectedData[playerSelectedIndex].Range
                + playerSelectedData[playerSelectedIndex].PointsAvailable * 5).ToString("F0") + " ft";
            progressionRelease.text = (playerSelectedData[playerSelectedIndex].Release
                + playerSelectedData[playerSelectedIndex].PointsAvailable).ToString("F0");

            progressionSpeed.text = playerSelectedData[playerSelectedIndex].calculateSpeedToPercent().ToString("F0");
            progressionJump.text = playerSelectedData[playerSelectedIndex].calculateJumpValueToPercent().ToString("F0");
            progressionLuck.text = (playerSelectedData[playerSelectedIndex].Luck + luckPointsAvailable).ToString("F0");
            playerSelectOptionText.text = playerSelectedData[playerSelectedIndex].PlayerDisplayName;
            playerSelectOptionImage.sprite = playerSelectedData[playerSelectedIndex].PlayerPortrait;

            //bonusReleaseText.text = playerSelectedData[playerSelectedIndex].PointsAvailable.ToString();
            //bonusRangeText.text = (playerSelectedData[playerSelectedIndex].PointsAvailable * 5).ToString();

            //use progression state data. dynamic data
            progression3Accuracy.text = (progressionState.Accuracy3 + progressionState.AddTo3).ToString("F0");
            progression4Accuracy.text = (progressionState.Accuracy4 + progressionState.AddTo4).ToString("F0");
            progression7Accuracy.text = (progressionState.Accuracy7 + progressionState.AddTo7).ToString("F0");

            progressionState.Level = progressionState.Experience / 2000;
            int nextlvl = ((progressionState.Level + 1) * 2000) - progressionState.Experience;

            playerProgressionStatsText.text = progressionState.Level.ToString("F0") + "\n"
                + progressionState.Experience.ToString("F0") + "\n"
                + nextlvl.ToString("F0") + "\n";

            playerProgressionUpdatePointsText.text = "points available : " + progressionState.PointsAvailable.ToString();

            //// player points avaiable for upgrade
            //if (progressionState.PointsAvailable > 0)
            //{
            //    playerProgressionUpdatePointsText.text = "points available : " + progressionState.PointsAvailable.ToString();

            //}
            //else
            //{
            //    playerProgressionUpdatePointsText.text = "points available : " + progressionState.PointsAvailable.ToString();
            //}

            GameOptions.playerObjectName = playerSelectedData[playerSelectedIndex].PlayerObjectName;
        }
        catch
        {
            return;
        }
    }

    // ============================  footer options activate - load scene/stats/quit/etc ==============================

    public void loadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    // ============================  message display ==============================
    // used in this context to display if item is locked

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

    // ============================  navigation functions ==============================
    public void changeSelectedPlayerUp()
    {
        //progressionState.clearState();
        resetUpdatePoints();

        playerSelectedIndex =
            (playerSelectedIndex == 0
            ? playerSelectedData.Count - 1
            : playerSelectedIndex -= 1);

        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
    }
    public void changeSelectedPlayerDown()
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

        progressionState.initializeState(playerSelectedData[playerSelectedIndex]);
    }
}
