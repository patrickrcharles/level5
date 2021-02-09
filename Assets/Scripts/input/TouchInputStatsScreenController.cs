
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.XR.WSA.Input;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputStatsScreenController : MonoBehaviour
{

    private Vector2 startTouchPosition, endTouchPosition;

    float swipeUpTolerance;
    float swipeDownTolerance;
    float swipeDistance;

    [SerializeField]
    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    [SerializeField]
    EventSystem m_EventSystem;

    Touch touch;
    bool buttonPressed;
    [SerializeField]
    GameObject joystickGameObject;

    public static TouchInputController instance;
    private GameObject prevSelectedGameObject;

    void Awake()
    {
        initializeStatScreenTouchControls();
    }
    private void Start()
    {
        // set distance required for swipe up to be regeistered by device
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;
        prevSelectedGameObject = EventSystem.current.firstSelectedGameObject;
        if (EventSystem.current == null)
        {
            EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
        }
    }

    void Update()
    {
        // save previous button until a touch is made
        if (!buttonPressed && Input.touchCount == 0)
        {
            prevSelectedGameObject = EventSystem.current.currentSelectedGameObject;
        }

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

    private void initializeStatScreenTouchControls()
    {
        // find onscreen stick and disable
        if (GameObject.Find("floating_joystick") != null)
        {
            joystickGameObject = GameObject.Find("floating_joystick");
            joystickGameObject.SetActive(false);
        }

        //check if startmanager is empty and find correct GraphicRaycaster and EventSystem
        if (GameObject.FindObjectOfType<StatsManager>() != null)
        {
            //Fetch the Raycaster from the GameObject (the Canvas)
            //m_Raycaster = StatsManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
            m_Raycaster = GameObject.Find("stats_manager").GetComponentInChildren<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            //m_EventSystem = StatsManager.instance.gameObject.GetComponentInChildren<EventSystem>();
            m_EventSystem = GameObject.Find("stats_manager").GetComponentInChildren<EventSystem>();
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
        //high score, mode change
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonName))
        {
            StatsManager.instance.changeSelectedMode("right");
            //StatsManager.instance.changeHighScoreModeNameDisplay();
            StatsManager.instance.changeHighScoreDataDisplay(false);
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonHardcoreName))
        {
            StatsManager.instance.changeSelectedMode("right");
            //StatsManager.instance.changeHighScoreModeNameDisplay();
            StatsManager.instance.changeHighScoreDataDisplay(true);
        }

        // player select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.MainMenuButtonName))
        {
            StatsManager.instance.loadMainMenu(StatsManager.MainMenuSceneName);
        }
        buttonPressed = false;
    }

    private void selectPressedButton()
    {
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);
    }


    private void swipeUpOnOption()
    {
        StatsManager.instance.changeHighScoreDataDisplay(false);
        buttonPressed = true;
        //high score, mode change
        EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonName))
        {
            StatsManager.instance.changeSelectedMode("right");
            //StatsManager.instance.changeHighScoreModeNameDisplay();
            StatsManager.instance.changeHighScoreDataDisplay(false);
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonHardcoreName))
        {
            StatsManager.instance.changeSelectedMode("right");
            //StatsManager.instance.changeHighScoreModeNameDisplay();
            StatsManager.instance.changeHighScoreDataDisplay(true);
            buttonPressed = true;
        }
        buttonPressed = false;
    }
    private void swipeDownOnOption()
    {
        StatsManager.instance.changeHighScoreDataDisplay(false);
        buttonPressed = true;
        //high score, mode change
        EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonName))
        {
            StatsManager.instance.changeSelectedMode("left");
            //StatsManager.instance.changeHighScoreModeNameDisplay();
            StatsManager.instance.changeHighScoreDataDisplay(false);
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonHardcoreName))
        {
            StatsManager.instance.changeSelectedMode("left");
            //StatsManager.instance.changeHighScoreModeNameDisplay();
            StatsManager.instance.changeHighScoreDataDisplay(true);
            buttonPressed = true;
        }
        buttonPressed = false;
    }
}
