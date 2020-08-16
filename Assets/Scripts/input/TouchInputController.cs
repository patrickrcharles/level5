
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputController : MonoBehaviour
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

    Touch touch;

    public static TouchInputController instance;

    bool buttonPressed;

    bool pauseExists;

    void Awake()
    {
        initializeTouchInputController();
    }

    void Update()
    {
        if (!Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                GameLevelManager.instance.PlayerState.touchControlJumpOrShoot(touch.position);
                //Debug.Log("touch pressure : " + touch.pressure);
            }

            endTouchPosition = touch.position;
            swipeDistance = endTouchPosition.y - startTouchPosition.y;

            if (touch.phase == TouchPhase.Moved // finger moving
                && Mathf.Abs(swipeDistance) > swipeUpTolerance // swipe is long enough
                && swipeDistance > 0 // swipe up
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/3 of screen
            {
                touch.phase = TouchPhase.Ended;
                GameLevelManager.instance.PlayerState.TouchControlJump();
            }

            if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
            {
                Pause.instance.TogglePause();
            }
        }

        // if paused
        if (Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                selectPressedButton();
            }

            // on double tap, perform actions
            if (touch.tapCount == 2  && touch.phase == TouchPhase.Began && !buttonPressed)
            {
                activateDoubleTappedButton();
            }
        }
    }

    private void activateDoubleTappedButton()
    {
        buttonPressed = true;
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.LoadSceneButton.name))
        {
            Pause.instance.reloadScene();
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.LoadStartScreenButton.name))
        {
            Pause.instance.loadstartScreen();
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.CancelMenuButton.name))
        {
            Pause.instance.TogglePause();
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.QuitGameButton.name))
        {
            Pause.instance.quit();
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

    private void initializeTouchInputController()
    {
        if (GameObject.FindObjectOfType<GameLevelManager>() != null)
        {
            //Fetch the Raycaster from the GameObject (the Canvas)
            m_Raycaster = GameLevelManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            m_EventSystem = GameLevelManager.instance.gameObject.GetComponentInChildren<EventSystem>();
        }
        else
        {
            this.gameObject.SetActive(false);
        }
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;
    }

}

