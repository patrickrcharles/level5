using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TouchInputStartScreenController : MonoBehaviour
{
    //#if UNITY_ANDROID && !UNITY_EDITOR
    private Vector2 startTouchPosition, endTouchPosition;

    float swipeUpTolerance;
    float swipeDownTolerance;
    float swipeDistance;

    GameObject prevSelectedGameObject;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    Touch touch;
    bool buttonPressed;

    GameObject joystickGameObject;

    public static TouchInputStartScreenController instance;


    void Awake()
    {
        initializeStartScreenTouchControls();
    }

    private void Start()
    {
        //StartManager.instance.disableButtonsNotUsedForTouchInput();
        // set distance required for swipe up to be regeistered by device
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;
        prevSelectedGameObject = EventSystem.current.firstSelectedGameObject;
        //Debug.Log("screen width : " + Screen.width);

        if (EventSystem.current == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }
    }

    void Update()
    {
        // if no button selected, return to previous
        if (EventSystem.current.currentSelectedGameObject == null)
        {
            EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
        }
        // save previous button until a touch is made
        if (!buttonPressed && Input.touchCount == 0)
        {
            prevSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        }
        // if touch
        if (Input.touchCount > 0 && !buttonPressed)
        {
            Touch touch = Input.touches[0];
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
            }
            endTouchPosition = touch.position;
            swipeDistance = endTouchPosition.y - startTouchPosition.y;

            // swipe down on changeable options
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
            {
                //change option
                swipeDownOnOption();
                // reset previous button to active button
                if (EventSystem.current.currentSelectedGameObject != prevSelectedGameObject)
                {
                    EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
                }
            }
            //swipe up on changeable options
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance > 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
            {
                //change option
                swipeUpOnOption();
                // reset previous button to active button
                if (EventSystem.current.currentSelectedGameObject != prevSelectedGameObject)
                {
                    EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
                }
            }
            // on double tap, perform actions
            if (touch.tapCount == 2 && touch.phase == TouchPhase.Began && !buttonPressed)
            {
                activateDoubleTappedButton();
            }
        }
    }

    private void initializeStartScreenTouchControls()
    {
        // find onscreen stick and disable
        if (GameObject.Find("floating_joystick") != null)
        {
            joystickGameObject = GameObject.Find("floating_joystick");
            joystickGameObject.SetActive(false);
        }

        //check if StartManager is empty and find correct GraphicRaycaster and EventSystem
        if (GameObject.FindObjectOfType<StartManager>() != null)
        {
            //Fetch the Raycaster from the GameObject (the Canvas)
            //m_Raycaster = StartManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
            m_Raycaster = GameObject.Find("startScreen").GetComponentInChildren<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            m_EventSystem = GameObject.Find("startScreen").GetComponentInChildren<EventSystem>();
        }
        // else, this is not the startscreen and disable object
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void activateDoubleTappedButton()
    {
        buttonPressed = true;
        EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
        //level select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.levelSelectButtonName))
        {
            StartManager.instance.changeSelectedLevelDown();
            StartManager.instance.initializeLevelDisplay();
            buttonPressed = true;
        }
        // traffic select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.trafficSelectOptionName))
        {
            StartManager.instance.changeSelectedTrafficOption();
            StartManager.instance.initializeTrafficOptionDisplay();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.enemySelectOptionName))
        {
            StartManager.instance.changeSelectedEnemiesOption();
            StartManager.instance.initializeEnemyOptionDisplay();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.hardcoreSelectOptionName))
        {
            StartManager.instance.changeSelectedHardcoreOption();
            StartManager.instance.initializeHardcoreOptionDisplay();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.SniperSelectOptionName))
        {
            StartManager.instance.changeSelectedSniperOption();
            StartManager.instance.initializeSniperOptionDisplay();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.difficultySelectOptionName))
        {
            StartManager.instance.changeSelectedDifficultyOption(GameOptions.difficultySelected);
            StartManager.instance.initializeDifficultyOptionDisplay();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.ObstacleSelectOptionName))
        {
            StartManager.instance.changeSelectedObstacleOption();
            StartManager.instance.initializeObstacleOptionDisplay();
            buttonPressed = true;
        }

        // player select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.playerSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedPlayerDown();
            StartManager.instance.initializePlayerDisplay();
            buttonPressed = true;
        }
        // friend select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.friendSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedfriendDown();
            StartManager.instance.initializefriendDisplay();
            buttonPressed = true;
        }
        // mode select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.modeSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedModeDown();
            StartManager.instance.initializeModeDisplay();
            buttonPressed = true;
        }
        //stats
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.statsMenuButtonName))
        {
            StartManager.instance.loadMenu(Constants.SCENE_NAME_level_00_stats);
            buttonPressed = true;
        }
        // start
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.startButtonName))
        {
            StartManager.instance.loadGame();
            buttonPressed = true;
        }
        // update /progression
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.updateMenuButtonName))
        {
            //Debug.Log("load prgression screen");
            GameOptions.playerSelectedIndex = StartManager.instance.playerSelectedIndex;
            StartManager.instance.loadMenu(Constants.SCENE_NAME_level_00_progression);
            buttonPressed = true;
        }
        // options
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.optionsMenuButtonName))
        {
            //Debug.Log("load prgression screen");
            StartManager.instance.loadMenu(Constants.SCENE_NAME_level_00_options);
            buttonPressed = true;
        }
        // credits
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.creditsMenuButtonName))
        {
            //Debug.Log("load prgression screen");
            StartManager.instance.loadMenu(Constants.SCENE_NAME_level_00_credits);
            buttonPressed = true;
        }
        // quit
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.quitButtonName))
        {
            Application.Quit();
            buttonPressed = true;
        }
        //account
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.accountMenuButtonName))
        {
            StartManager.instance.loadMenu(Constants.SCENE_NAME_level_00_account);
            buttonPressed = true;
        }
        //options
        //if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.OptionsMenuButtonName))
        //{
        //    StartManager.instance.loadMenu(Constants.SCENE_NAME_level_00_options);
        //    buttonPressed = true;
        //}
        buttonPressed = false;
    }

    //private void selectPressedButton()
    //{
    //    //Set up the new Pointer Event
    //    m_PointerEventData = new PointerEventData(m_EventSystem);
    //    //Set the Pointer Event Position to that of the mouse position
    //    m_PointerEventData.position = Input.mousePosition;

    //    //Create a list of Raycast Results
    //    List<RaycastResult> results = new List<RaycastResult>();

    //    //Raycast using the Graphics Raycaster and mouse click position
    //    m_Raycaster.Raycast(m_PointerEventData, results);
    //}

    private void swipeUpOnOption()
    {
        EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
        //level select
        if (prevSelectedGameObject.name.Equals(StartManager.levelSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedLevelUp();
            StartManager.instance.initializeLevelDisplay();
            buttonPressed = true;
        }
        // traffic select
        if (prevSelectedGameObject.name.Equals(StartManager.trafficSelectOptionName))
        {
            StartManager.instance.changeSelectedTrafficOption();
            StartManager.instance.initializeTrafficOptionDisplay();
            buttonPressed = true;
        }
        if (prevSelectedGameObject.name.Equals(StartManager.hardcoreSelectOptionName))
        {
            StartManager.instance.changeSelectedHardcoreOption();
            StartManager.instance.initializeHardcoreOptionDisplay();
            buttonPressed = true;
        }
        if (prevSelectedGameObject.name.Equals(StartManager.SniperSelectOptionName))
        {
            StartManager.instance.changeSelectedSniperOption();
            StartManager.instance.initializeSniperOptionDisplay();
            buttonPressed = true;
        }
        if (prevSelectedGameObject.name.Equals(StartManager.enemySelectOptionName))
        {
            StartManager.instance.changeSelectedEnemiesOption();
            StartManager.instance.initializeEnemyOptionDisplay();
        }
        if (prevSelectedGameObject.name.Equals(StartManager.difficultySelectOptionName))
        {
            StartManager.instance.changeSelectedDifficultyOption(GameOptions.difficultySelected);
            StartManager.instance.initializeDifficultyOptionDisplay();
        }
        if (prevSelectedGameObject.name.Equals(StartManager.ObstacleSelectOptionName))
        {
            StartManager.instance.changeSelectedObstacleOption();
            StartManager.instance.initializeObstacleOptionDisplay();
        }
        //// num players  select
        //if (prevSelectedGameObject.name.Equals(StartManager.num))
        //{
        //    StartManager.instance.changeSelectedPlayerUp();
        //    StartManager.instance.initializePlayerDisplay();
        //    buttonPressed = true;
        //}
        // player select
        if (prevSelectedGameObject.name.Equals(StartManager.playerSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedPlayerUp();
            StartManager.instance.initializePlayerDisplay();
            buttonPressed = true;
        }
        // friend select
        if (prevSelectedGameObject.name.Equals(StartManager.friendSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedfriendUp();
            StartManager.instance.initializefriendDisplay();
            buttonPressed = true;
        }
        // mode select
        if (prevSelectedGameObject.name.Equals(StartManager.modeSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedModeUp();
            StartManager.instance.initializeModeDisplay();
            buttonPressed = true;
        }
        buttonPressed = false;
    }
    private void swipeDownOnOption()
    {
        EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
        //level select
        if (prevSelectedGameObject.name.Equals(StartManager.levelSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedLevelDown();
            StartManager.instance.initializeLevelDisplay();
            buttonPressed = true;
        }
        // traffic select
        if (prevSelectedGameObject.name.Equals(StartManager.trafficSelectOptionName))
        {
            StartManager.instance.changeSelectedTrafficOption();
            StartManager.instance.initializeTrafficOptionDisplay();
            buttonPressed = true;
        }
        if (prevSelectedGameObject.name.Equals(StartManager.hardcoreSelectOptionName))
        {
            StartManager.instance.changeSelectedHardcoreOption();
            StartManager.instance.initializeHardcoreOptionDisplay();
            buttonPressed = true;
        }
        // sniper select
        if (prevSelectedGameObject.name.Equals(StartManager.SniperSelectOptionName))
        {
            StartManager.instance.changeSelectedSniperOption();
            StartManager.instance.initializeSniperOptionDisplay();
            buttonPressed = true;
        }
        if (prevSelectedGameObject.name.Equals(StartManager.enemySelectOptionName))
        {
            StartManager.instance.changeSelectedEnemiesOption();
            StartManager.instance.initializeEnemyOptionDisplay();
        }
        if (prevSelectedGameObject.name.Equals(StartManager.difficultySelectOptionName))
        {
            StartManager.instance.changeSelectedDifficultyOption(GameOptions.difficultySelected);
            StartManager.instance.initializeDifficultyOptionDisplay();
        }
        if (prevSelectedGameObject.name.Equals(StartManager.ObstacleSelectOptionName))
        {
            StartManager.instance.changeSelectedObstacleOption();
            StartManager.instance.initializeObstacleOptionDisplay();
        }
        // player select
        if (prevSelectedGameObject.name.Equals(StartManager.playerSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedPlayerDown();
            StartManager.instance.initializePlayerDisplay();
            buttonPressed = true;
        }
        // friend select
        if (prevSelectedGameObject.name.Equals(StartManager.friendSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedfriendDown();
            StartManager.instance.initializefriendDisplay();
            buttonPressed = true;
        }
        // mode select
        if (prevSelectedGameObject.name.Equals(StartManager.modeSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedModeDown();
            StartManager.instance.initializeModeDisplay();
            buttonPressed = true;
        }
        buttonPressed = false;
    }
    //#endif
}
