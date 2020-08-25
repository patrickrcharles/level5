
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
//using UnityEngine.XR.WSA.Input;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputStartScreenController : MonoBehaviour
{

    private Vector2 startTouchPosition, endTouchPosition;

    float swipeUpTolerance;
    float swipeDownTolerance;
    float swipeDistance;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;

    Touch touch;
    bool buttonPressed;
    [SerializeField]
    GameObject joystickGameObject;
    [SerializeField]
    GameObject prevSelectedGameObject;

    public static TouchInputStartScreenController instance;


    void Awake()
    {
        initializeStartScreenTouchControls();
    }

    private void Start()
    {
        StartManager.instance.disableButtonsNotUsedForTouchInput();
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
            //Debug.Log("touch.tapCount : " + touch.tapCount);
            //Debug.Log("touch.phase : " + touch.phase);
            //Debug.Log("Mathf.Abs(swipeDistance) > swipeDownTolerance : " + (Mathf.Abs(swipeDistance) > swipeDownTolerance));
            //Debug.Log("swipeDistance < 0 : " + (swipeDistance < 0));
            //Debug.Log("(startTouchPosition.x > (Screen.width / 2)) : " + (startTouchPosition.x > (Screen.width / 2)));
            //Debug.Log("current button : " + EventSystem.current.currentSelectedGameObject.name);


            // swipe down on changeable options
            if (/*touch.tapCount == 1 &&*/ touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
            {
                //Debug.Log("swipe down");
                //change option
                swipeDownOnOption();
                // reset previous button to active button
                if (EventSystem.current.currentSelectedGameObject != prevSelectedGameObject)
                {
                    EventSystem.current.SetSelectedGameObject(prevSelectedGameObject);
                }
            }
            //swipe up on changeable options
            if (/*touch.tapCount == 1 &&*/ touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance > 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
            {
                //Debug.Log("swipe down");
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
                //Debug.Log(" double tap");
                activateDoubleTappedButton();
            }
            //buttonPressed = false;
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

        //check if startmanager is empty and find correct GraphicRaycaster and EventSystem
        if (GameObject.FindObjectOfType<StartManager>() != null)
        {
            //Fetch the Raycaster from the GameObject (the Canvas)
            m_Raycaster = StartManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            m_EventSystem = StartManager.instance.gameObject.GetComponentInChildren<EventSystem>();
        }
        // else, this is not the startscreen and disable object
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private void activateDoubleTappedButton()
    {
        //level select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.LevelSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedLevelDown();
            StartManager.instance.initializeLevelDisplay();
        }
        // traffic select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.TrafficSelectOptionName))
        {
            StartManager.instance.changeSelectedTrafficOption();
            StartManager.instance.initializeTrafficOptionDisplay();
        }
        // player select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.PlayerSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedPlayerDown();
            StartManager.instance.initializePlayerDisplay();
        }
        // friend select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.CheerleaderSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedCheerleaderDown();
            StartManager.instance.initializeCheerleaderDisplay();
        }
        // mode select
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.ModeSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedModeDown();
            StartManager.instance.intializeModeDisplay();
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.StatsMenuButtonName))
        {
            StartManager.instance.loadScene(StartManager.StatsMenuSceneName);
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.StartButtonName))
        {
            StartManager.instance.loadScene();
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.QuitButtonName))
        {
            Application.Quit();
        }
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
        buttonPressed = true;
        //level select
        if (prevSelectedGameObject.name.Equals(StartManager.LevelSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedLevelUp();
            StartManager.instance.initializeLevelDisplay();
        }
        // traffic select
        if (prevSelectedGameObject.name.Equals(StartManager.TrafficSelectOptionName))
        {
            StartManager.instance.changeSelectedTrafficOption();
            StartManager.instance.initializeTrafficOptionDisplay();
        }
        // player select
        if (prevSelectedGameObject.name.Equals(StartManager.PlayerSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedPlayerUp();
            StartManager.instance.initializePlayerDisplay();
        }
        // friend select
        if (prevSelectedGameObject.name.Equals(StartManager.CheerleaderSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedCheerleaderUp();
            StartManager.instance.initializeCheerleaderDisplay();
        }
        // mode select
        if (prevSelectedGameObject.name.Equals(StartManager.ModeSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedModeUp();
            StartManager.instance.intializeModeDisplay();
        }
        buttonPressed = false;
    }
    private void swipeDownOnOption()
    {
        buttonPressed = true;
        //level select
        if (prevSelectedGameObject.name.Equals(StartManager.LevelSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedLevelDown();
            StartManager.instance.initializeLevelDisplay();
        }
        // traffic select
        if (prevSelectedGameObject.name.Equals(StartManager.TrafficSelectOptionName))
        {
            StartManager.instance.changeSelectedTrafficOption();
            StartManager.instance.initializeTrafficOptionDisplay();
        }
        // player select
        if (prevSelectedGameObject.name.Equals(StartManager.PlayerSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedPlayerDown();
            StartManager.instance.initializePlayerDisplay();
        }
        // friend select
        if (prevSelectedGameObject.name.Equals(StartManager.CheerleaderSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedCheerleaderDown();
            StartManager.instance.initializeCheerleaderDisplay();
        }
        // mode select
        if (prevSelectedGameObject.name.Equals(StartManager.ModeSelectOptionButtonName))
        {
            StartManager.instance.changeSelectedModeDown();
            StartManager.instance.intializeModeDisplay();
        }
        buttonPressed = false;
    }
}
