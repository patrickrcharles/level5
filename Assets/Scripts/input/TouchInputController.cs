
using System.Collections;
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
    private bool tapDetected;

    void Awake()
    {
        initializeTouchInputController();
    }

    // changing from Update -> FixedUpdate allows input to be detected at 1/100 as opposed to 1/60 for mobile
    //void FixedUpdate()
    //{
    //    //Debug.Log(EventSystem.current.currentSelectedGameObject.name);
    //    //if (!Pause.instance.Paused && Input.touchCount > 0)
    //    //{
    //    //    Debug.Log("----------------------------------FIXED UPDATE - not paused + touch");
    //    //    touch = Input.touches[0];
    //    //    if (touch.tapCount == 1
    //    //        && touch.phase == TouchPhase.Began
    //    //        && !buttonPressed)
    //    //    {
    //    //        Debug.Log("----------------------------------Input.touchCount > 0");
    //    //        Debug.Log("     buttonPressed : " + buttonPressed);
    //    //        Debug.Log("     touch.tapCount : " + touch.tapCount);
    //    //        Debug.Log("     touch.phase : " + touch.phase);
    //    //        tapDetected = true;
    //    //        startTouchPosition = touch.position;
    //    //        Debug.Log("tapDetected : " + tapDetected);
    //    //    }

    //    //    endTouchPosition = touch.position;
    //    //    swipeDistance = endTouchPosition.y - startTouchPosition.y;

    //    //    //if (touch.tapCount == 1 && touch.phase == TouchPhase.Moved // finger moving
    //    //    //    && Mathf.Abs(swipeDistance) > swipeUpTolerance // swipe is long enough
    //    //    //    && swipeDistance > 0 // swipe up
    //    //    //    && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/3 of screen
    //    //    //{
    //    //    //    //Debug.Log("swipe up");
    //    //    //    touch.phase = TouchPhase.Ended;
    //    //    //    GameLevelManager.instance.PlayerState.TouchControlJump();
    //    //    //}

    //    //    if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
    //    //        && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
    //    //        && swipeDistance < 0 // swipe down
    //    //        && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/2 of screen)) 
    //    //    {
    //    //        //Debug.Log("swipe down");
    //    //        Pause.instance.TogglePause();
    //    //    }
    //    //}
    //}


    void Update()
    {
        if (!Pause.instance.Paused && Input.touchCount > 0)
        {
            //Debug.Log("----------------------------------FIXED UPDATE - not paused + touch");
            touch = Input.touches[0];
            if (touch.tapCount == 1
                && touch.phase == TouchPhase.Began
                && !buttonPressed)
            {
                //Debug.Log("----------------------------------Input.touchCount > 0");
                //Debug.Log("     buttonPressed : " + buttonPressed);
                //Debug.Log("     touch.tapCount : " + touch.tapCount);
                //Debug.Log("     touch.phase : " + touch.phase);
                tapDetected = true;
                startTouchPosition = touch.position;
                //Debug.Log("tapDetected : " + tapDetected);
            }

            endTouchPosition = touch.position;
            swipeDistance = endTouchPosition.y - startTouchPosition.y;

            //if (touch.tapCount == 1 && touch.phase == TouchPhase.Moved // finger moving
            //    && Mathf.Abs(swipeDistance) > swipeUpTolerance // swipe is long enough
            //    && swipeDistance > 0 // swipe up
            //    && (startTouchPosition.x > (Screen.width / 2))) // if swipe on right 1/3 of screen
            //{
            //    //Debug.Log("swipe up");
            //    touch.phase = TouchPhase.Ended;
            //    GameLevelManager.instance.PlayerState.TouchControlJump();
            //}

            if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && startTouchPosition.x > (Screen.width / 2) // if swipe on right 1/2 of screen)) 
                && startTouchPosition.x > (Screen.height / 2)) // if swipe on right 1/2 of screen)) 
            {
                //Debug.Log("swipe down");
                Pause.instance.TogglePause();
            }

            //if (touch.tapCount == 1 && touch.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
            //    && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
            //    && swipeDistance < 0 // swipe down
            //    && startTouchPosition.x < (Screen.width / 2) // if swipe on right 1/2 of screen)) 
            //    && startTouchPosition.x < (Screen.height / 2)) // if swipe on right 1/2 of screen)) 
            //{
            //    //Debug.Log("current camera : " + Camera.current.name);
            //    //Debug.Log("Camera.main : " + Camera.main);
            //    //foreach (Camera c in Camera.allCameras)
            //    //{
            //    //    Debug.Log("     camera : " + c.name);
            //    //}
            //    ////Debug.Log("swipe down");
            //    //foreach (GameObject c in CameraManager.instance.Cameras)
            //    //{
            //    //    Debug.Log("    " + c.name + "active : " + c.activeInHierarchy);
            //    //}
            //    Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
            //    messageText.text = "graphics api : " + SystemInfo.graphicsDeviceType;
            //    StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));


            //    //Debug.Log("graphics api : " + SystemInfo.graphicsDeviceName);
            //    //Debug.Log("graphics api : " + SystemInfo.graphicsDeviceType);
            //    //Debug.Log("graphics api : " + SystemInfo.graphicsMemorySize);
            //    //Debug.Log("graphics api : " + SystemInfo.processorType);
            //    //Debug.Log("graphics api : " + SystemInfo.systemMemorySize);

            //    //ColorHelper.ColorText(SystemInfo.graphicsDeviceName, m_GPUFieldsColor),
            //    //ColorHelper.ColorText("[" + SystemInfo.graphicsDeviceType.ToString() + "]", m_GPUDetailFieldsColor),
            //    //ColorHelper.ColorText(SystemInfo.graphicsMemorySize.ToString(), m_GPUFieldsColor),
            //    //ColorHelper.ColorText(SystemInfo.processorType, m_CPUFieldsColor),
            //    //ColorHelper.ColorText(SystemInfo.systemMemorySize.ToString(), m_CPUFieldsColor),
            //    //ColorHelper.ColorText(SystemInfo.operatingSystem, m_SysFieldsColor)
            //}
        }
        if (tapDetected)
        {
            Debug.Log("shoot/jump");
            //startTouchPosition = touch.position;
            GameLevelManager.instance.PlayerState.touchControlJumpOrShoot(touch.position);
            tapDetected = false;
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
            if (touch.tapCount == 2 && touch.phase == TouchPhase.Began && !buttonPressed)
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
        //Debug.Log("double tap");
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.LoadSceneButton.name)
            && !buttonPressed)
        {
            //Debug.Log("reload");
            Pause.instance.reloadScene();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.LoadStartScreenButton.name)
            && !buttonPressed)
        {
            //Debug.Log("start screen");
            Pause.instance.loadstartScreen();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.CancelMenuButton.name)
            && !buttonPressed)
        {
            //Debug.Log("cancel/un pause");
            Pause.instance.TogglePause();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.QuitGameButton.name)
            && !buttonPressed)
        {
            //Debug.Log("quit");
            Pause.instance.quit();
            buttonPressed = true;
        }
        // new stuff
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleCameraName)
            && !buttonPressed)
        {
            //Debug.Log("toggle camera");
            CameraManager.instance.switchCamera();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleFpsName)
            && !buttonPressed)
        {
            //Debug.Log("toggle fps");
            DevFunctions.instance.ToggleFpsCounter();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleMaxStatsName)
            && !buttonPressed)
        {
            //Debug.Log("set max stats");
            DevFunctions.instance.setMaxPlayerStats();
            buttonPressed = true;
        }
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleUiStatsName)
            && !buttonPressed)
        {
            //Debug.Log("toggle stats");
            BasketBall.instance.toggleUiStats();
            buttonPressed = true;
        }
        buttonPressed = false;
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

    //// action button (turn on moneyball)
    //private void activateTripleTappedButton()
    //{
    //    buttonPressed = true;
    //    Debug.Log("triple tap");
    //    GameRules.instance.toggleMoneyBall();
    //    buttonPressed = false;
    //}

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

