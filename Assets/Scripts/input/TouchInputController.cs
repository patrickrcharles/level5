
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
        //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
        if (!Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                //Debug.Log("shoot or jump");
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
                //Debug.Log("swipe up");
                touch.phase = TouchPhase.Ended;
                GameLevelManager.instance.PlayerState.TouchControlJump();
            }

            if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
            {
                //Debug.Log("swipe down");
                Pause.instance.TogglePause();
            }
        }

        // if paused
        if (Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            //Debug.Log("touch.tapCount : " + touch.tapCount + " || touch.phase : " + touch.phase);

            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                //Debug.Log("paused tap");
                selectPressedButton();
            }

            // on double tap, perform actions
            if (touch.tapCount == 2  && touch.phase == TouchPhase.Began && !buttonPressed)
            {
                activateDoubleTappedButton();
            }

            //// on double tap, perform actions
            //if (touch.tapCount == 3 && touch.phase == TouchPhase.Began && !buttonPressed)
            //{
            //    activateTripleTappedButton();
            //}
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

    // action button (turn on moneyball)
    private void activateTripleTappedButton()
    {
        buttonPressed = true;
        Debug.Log("triple tap");
        GameRules.instance.toggleMoneyBall();
        buttonPressed = false;
    }

    private void selectPressedButton()
    {
        //Debug.Log("selectPressedButton()");
        //Set up the new Pointer Event
        m_PointerEventData = new PointerEventData(m_EventSystem);
        //Set the Pointer Event Position to that of the mouse position
        m_PointerEventData.position = Input.mousePosition;

        //Create a list of Raycast Results
        List<RaycastResult> results = new List<RaycastResult>();

        //Raycast using the Graphics Raycaster and mouse click position
        m_Raycaster.Raycast(m_PointerEventData, results);

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        foreach (RaycastResult result in results)
        {
            Debug.Log("Hit " + result.gameObject.name);
        }
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
        //if(GameObject.FindObjectOfType<EventSystem>() != null)
        //{
        //    EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        //}
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;
    }

}

