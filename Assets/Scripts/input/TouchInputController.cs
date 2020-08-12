
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputController : MonoBehaviour
{

    private Vector2 startTouchPosition, endTouchPosition;
    private bool jumpAllowed = false;
    private bool shootAllowed = false;

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
    public bool locked;

    void Awake()
    {
        instance = this;
        // set distance required for swipe up to be regeistered by device
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GameLevelManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameLevelManager.instance.gameObject.GetComponentInChildren<EventSystem>();
    }

    void Update()
    {
        if (!Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                startTouchPosition = touch.position;
                GameLevelManager.instance.PlayerState.touchControlShoot();
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
            if (touch.phase == TouchPhase.Ended // finger stoppped moving
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/3 of screen)
            {
                Pause.instance.TogglePause();
            }
        }

        // if paused
        if (Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
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

                ////For every result returned, output the name of the GameObject on the Canvas hit by the Ray
                //int i = 0;
                //foreach (RaycastResult result in results)
                //{
                //    if (result.gameObject.activeInHierarchy)
                //    {
                //        Debug.Log("Hit" + i + " : " + result.gameObject.name);
                //        i++;
                //    }
                //}
            }
            if (touch.tapCount == 2)
            {
                Debug.Log("EventSystem.current.currentSelectedGameObject.name : " + EventSystem.current.currentSelectedGameObject.name);

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
            }
        }
    }
}
