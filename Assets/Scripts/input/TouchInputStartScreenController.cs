
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputStartScreenController : MonoBehaviour
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

    bool pauseExists;

    void Awake()
    {
        if (GameObject.FindObjectOfType<StartManager>() != null)
        {
            //Fetch the Raycaster from the GameObject (the Canvas)
            m_Raycaster = StartManager.instance.gameObject.GetComponentInChildren<GraphicRaycaster>();
            //Fetch the Event System from the Scene
            m_EventSystem = StartManager.instance.gameObject.GetComponentInChildren<EventSystem>();
        }
        else
        {
            Debug.Log("disable touch input start");
            this.gameObject.SetActive(false);
        }
        // set distance required for swipe up to be regeistered by device
        swipeUpTolerance = Screen.height / 7;
        swipeDownTolerance = Screen.height / 5;

    }

    void Update()
    {

        if ( Input.touchCount > 0)
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
    }
}
