
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputLinksScreenController : MonoBehaviour
{

    private Vector2 startTouchPosition, endTouchPosition;

    float swipeUpTolerance;
    float swipeDownTolerance;
    float swipeDistance;

    [SerializeField]
    GraphicRaycaster m_Raycaster;
    [SerializeField]
    PointerEventData m_PointerEventData;
    [SerializeField]
    EventSystem m_EventSystem;
    [SerializeField]
    GameObject CreditsManagerObject;

    Touch touch;
    bool buttonPressed;
    [SerializeField]
    GameObject joystickGameObject;

    //public static TouchInputController instance;
    private GameObject prevSelectedGameObject;

    void Awake()
    {
        initializeOptionScreenTouchControls();
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

            // on double tap, perform actions
            if (touch.tapCount == 2 && touch.phase == TouchPhase.Began 
                && !buttonPressed)
            {
                buttonPressed = true;
                activateDoubleTappedButton();
            }
        }
    }

    private void initializeOptionScreenTouchControls()
    {
        // find onscreen stick and disable
        if (GameObject.Find("floating_joystick") != null)
        {
            joystickGameObject = GameObject.Find("floating_joystick");
            joystickGameObject.SetActive(false);
        }

        //check if startmanager is empty and find correct GraphicRaycaster and EventSystem
        if (GameObject.FindObjectOfType<CreditsManager>() != null)
        {
            CreditsManagerObject = FindObjectOfType<CreditsManager>().gameObject;
            //Fetch the Raycaster from the GameObject (the Canvas)
            m_Raycaster = FindObjectOfType<CreditsManager>().gameObject.GetComponentInChildren<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            m_EventSystem = FindObjectOfType<CreditsManager>().gameObject.GetComponentInChildren<EventSystem>();
        }
        // else, this is not the startscreen and disable object
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void activateDoubleTappedButton()
    {
        //buttonPressed = true;

        //if (EventSystem.current.currentSelectedGameObject.name.Equals(StatsManager.ModeSelectButtonName))
        //{
        //    StatsManager.instance.changeSelectedMode("right");
        //    //StatsManager.instance.changeHighScoreModeNameDisplay();
        //    StatsManager.instance.changeHighScoreDataDisplay();
        //}

        // footer
        // main menu
        if (EventSystem.current.currentSelectedGameObject.name.Equals(CreditsManager.MainMenuButtonName))
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_NAME_level_00_start);
        }
        //stats
        if (EventSystem.current.currentSelectedGameObject.name.Equals(CreditsManager.StatsMenuButtonName))
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_NAME_level_00_stats);
        }
        //progression
        if (EventSystem.current.currentSelectedGameObject.name.Equals(CreditsManager.ProgressionMenuButtonName))
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_NAME_level_00_progression);
        }
        // account
        if (EventSystem.current.currentSelectedGameObject.name.Equals(CreditsManager.AccountMenuButtonName))
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_NAME_level_00_account);
        }
        //credits
        if (EventSystem.current.currentSelectedGameObject.name.Equals(CreditsManager.CreditsMenuButtonName))
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_NAME_level_00_credits);
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(CreditsManager.CreditsMenuButtonName))
        {
            SceneManager.LoadSceneAsync(Constants.SCENE_NAME_level_00_credits);
        }

        // account
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
