
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
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

    public static TouchInputController instance;


    void Awake()
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
        // set distance required for swipe up to be regeistered by device
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            // highlight pressed button
            if (touch.phase == TouchPhase.Began)
            {
                selectPressedButton();
            }

            // on double tap, perform actions
            if (touch.tapCount == 2 && touch.phase == TouchPhase.Began && !buttonPressed)
            {
                buttonPressed = true;
                activateDoubleTappedButton();
            }
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
            StartManager.instance.loadStatsMenu(StartManager.StatsMenuSceneName);
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.StartButtonName))
        {
            StartManager.instance.loadScene();
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.QuitButtonName))
        {
            Application.Quit();
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
}
