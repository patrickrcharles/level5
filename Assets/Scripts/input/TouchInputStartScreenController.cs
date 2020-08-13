
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
                //Set up the new Pointer Event
                m_PointerEventData = new PointerEventData(m_EventSystem);
                //Set the Pointer Event Position to that of the mouse position
                m_PointerEventData.position = Input.mousePosition;

                //Create a list of Raycast Results
                List<RaycastResult> results = new List<RaycastResult>();

                //Raycast using the Graphics Raycaster and mouse click position
                m_Raycaster.Raycast(m_PointerEventData, results);
            }

            // on double tap, perform actions
            if (touch.tapCount == 2 && touch.phase == TouchPhase.Began && !buttonPressed)
            {
                buttonPressed = true;
                //level select
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.LevelSelectOptionButtonName))
                {
                    Debug.Log("level");
                    StartManager.instance.changeSelectedLevelDown();
                    StartManager.instance.initializeLevelDisplay();
                }
                // traffic select
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.TrafficSelectOptionName))
                {
                    Debug.Log("traffic");
                    StartManager.instance.changeSelectedTrafficOption();
                    StartManager.instance.initializeTrafficOptionDisplay();
                }
                // player select
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.PlayerSelectOptionButtonName))
                {
                    Debug.Log("player");
                    StartManager.instance.changeSelectedPlayerDown();
                    StartManager.instance.initializePlayerDisplay();
                }
                // friend select
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.CheerleaderSelectOptionButtonName))
                {
                    Debug.Log("friend");
                    StartManager.instance.changeSelectedCheerleaderDown();
                    StartManager.instance.initializeCheerleaderDisplay();
                }
                // mode select
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.ModeSelectOptionButtonName))
                {
                    Debug.Log("mode");
                    StartManager.instance.changeSelectedModeDown();
                    StartManager.instance.intializeModeDisplay();
                }
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.StatsMenuButtonName))
                {
                    Debug.Log("stats");
                    StartManager.instance.loadStatsMenu(StartManager.StatsMenuSceneName);
                }
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.StartButtonName))
                {
                    Debug.Log("start");
                    StartManager.instance.loadScene();
                }
                if (EventSystem.current.currentSelectedGameObject.name.Equals(StartManager.QuitButtonName))
                {
                    Debug.Log("quit");
                    Application.Quit();
                }
                buttonPressed = false;
            }
        }
    }
}
