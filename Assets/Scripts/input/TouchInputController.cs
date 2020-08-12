
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
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

    public static TouchInputController instance;
    public  bool locked;

    void Awake()
    {
        instance = this;
        // set distance required for swipe up to be regeistered by device
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 7;

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GameLevelManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameLevelManager.instance.gameObject.GetComponentInChildren<EventSystem>();
    }

    // Use this for initialization
    // Update is called once per frame
    void FixedUpdate()
    {
            SwipeCheck();
            tapCheck();
    }

    void Update()
    {
        // if paused check for swipes
        if (Pause.instance.Paused)
        {
            UISwipeCheck();
        }
    }

    private void SwipeCheck()
    {
        //Debug.Log("SwipeCheck");
        // get initial touch position
        if (Input.touchCount > 0
            && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            startTouchPosition = Input.GetTouch(0).position;
            //Debug.Log("initial touch");
        }

        // if swipe moves
        if (Input.touchCount > 0)
        {
            endTouchPosition = Input.GetTouch(0).position;
            swipeDistance = endTouchPosition.y - startTouchPosition.y;

            if (Input.GetTouch(0).phase == TouchPhase.Moved
                && Mathf.Abs(swipeDistance) > swipeUpTolerance
                && swipeDistance > 0
                && GameLevelManager.instance.PlayerState.grounded)
            {
                //Debug.Log("swipe up");
                Debug.Log(swipeDistance);
                GameLevelManager.instance.PlayerState.TouchControlJump();
                jumpAllowed = false;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Ended
                && Mathf.Abs(swipeDistance) > swipeDownTolerance
                && swipeDistance < 0 )
            {
                Debug.Log("pause pleez");
                Pause.instance.TogglePause();
            }
        }
    }

    private void UISwipeCheck()
    {

        //// get initial touch position
        //if (Input.touchCount > 0
        //    && Input.GetTouch(0).phase == TouchPhase.Began)
        //{
        //    startTouchPosition = Input.GetTouch(0).position;
        //    //Debug.Log("initial touch");
        //}

        //// if swipe moves
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
        //{
        //    endTouchPosition = Input.GetTouch(0).position;
        //    swipeDistance = endTouchPosition.y - startTouchPosition.y;
        //    if (Mathf.Abs(swipeDistance) > swipeDownTolerance
        //        && swipeDistance < 0)
                
        //    {
        //        Pause.instance.TogglePause();
        //    }
        //}

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            //Set up the new Pointer Event
            m_PointerEventData = new PointerEventData(m_EventSystem);
            //Set the Pointer Event Position to that of the mouse position
            m_PointerEventData.position = Input.mousePosition;

            //Create a list of Raycast Results
            List<RaycastResult> results = new List<RaycastResult>();

            //Raycast using the Graphics Raycaster and mouse click position
            m_Raycaster.Raycast(m_PointerEventData, results);

            //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
            int i = 0;
            foreach (RaycastResult result in results)
            {
                if (result.gameObject.activeInHierarchy)
                {
                    Debug.Log("Hit" + i + " : " + result.gameObject.name);
                    i++;
                }
            }
        }
    }

    private void tapCheck()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                //shootAllowed = true;
                GameLevelManager.instance.PlayerState.touchControlShoot();
                shootAllowed = false;
            }
        }
    }
}
