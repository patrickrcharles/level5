﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TouchPhase = UnityEngine.TouchPhase;

public class TouchInputController : MonoBehaviour
{

    private Vector2 startTouchPosition1, endTouchPosition1;
    private Vector2 startTouchPosition2, endTouchPosition2;

    float swipeUpTolerance;
    float swipeDownTolerance;
    float swipeDistance;

    [SerializeField]
    GraphicRaycaster m_Raycaster;
    [SerializeField]
    PointerEventData m_PointerEventData;
    [SerializeField]
    EventSystem m_EventSystem;

    Touch touch1;
    Touch touch2;

    public static TouchInputController instance;

    bool buttonPressed;

    bool pauseExists;
    [SerializeField]
    private bool tap1Detected;
    [SerializeField]
    private bool tap2Detected;
    //[SerializeField]
    //private bool doubleTap1Detected;
    //[SerializeField]
    //private bool doubleTap2Detected;
    [SerializeField]
    private bool hold1Detected;
    //[SerializeField]
    //private bool hold2Detected;

    [SerializeField]
    float tapInterval;
    float touchfirstTap;
    float touchLastTap;

    [SerializeField]
    PlayerController playerController;

    public bool HoldDetected { get => hold1Detected; set => hold1Detected = value; }

    void Awake()
    {
        initializeTouchInputController();

    }
    private void Start()
    {
        instance = this;
        playerController = GameLevelManager.instance.PlayerState;
    }

    //#if UNITY_ANDROID && !UNITY_EDITOR
    void Update()
    {
        // not paused + touch
        if (!Pause.instance.Paused && Input.touchCount > 0)
        {
            // ====================== get touches =====================================
            // detect multi touches
            if (Input.touchCount > 1)
            {
                touch1 = Input.touches[0];
                touch2 = Input.touches[1];
            }
            else
            {
                touch1 = Input.touches[0];
            }

            // ====================== touch 1 : tap  =====================================
            if (touch1.tapCount == 1
                && touch1.phase == TouchPhase.Began
                && !buttonPressed)
            {
                tap1Detected = true;
                startTouchPosition1 = touch1.position;
            }
            else
            { tap1Detected = false; }

            // ====================== touch 1 : hold  =====================================
            //Touch 1 is hold + bottom left screen
            if (touch1.tapCount == 1
                && touch1.phase == TouchPhase.Stationary
                //&& (touch1.phase == TouchPhase.Stationary || touch1.phase == TouchPhase.Moved)
                && !buttonPressed
                && touch1.position.x < (Screen.width / 2)
                && touch1.position.y > (Screen.height / 2)
                //&& startTouchPosition1.x < (Screen.height / 2)
                && playerController.PlayerCanBlock
                && GameOptions.enemiesEnabled) // if swipe on right 1/2 of screen)) )
            {
                hold1Detected = true;
                startTouchPosition1 = touch1.position;
                if (playerController.CanBlock && !playerController.hasBasketball)
                {
                    playerController.playerBlock();
                }
            }
            if (touch1.phase == TouchPhase.Ended || touch1.phase == TouchPhase.Canceled)
            {
                hold1Detected = false;
                playerController.Anim.SetBool("block", false);
            }

            // ====================== touch 2 : tap  =====================================
            if (touch2.tapCount == 1
                && touch2.phase == TouchPhase.Began
                && !buttonPressed)
            {
                tap2Detected = true;
                startTouchPosition2 = touch2.position;
                touchfirstTap = Time.time;
            }
            else
            {
                tap2Detected = false;
            }

            //// ====================== touch 1 : double tap  =====================================
            //if (touch1.tapCount == 2 && touch1.phase == TouchPhase.Began)
            //{
            //    doubleTap1Detected = true;
            //}
            //else
            //{
            //    doubleTap1Detected = false;
            //}

            //// ====================== touch 2 : double tap  =====================================
            //if (touch2.tapCount == 2 && touch2.phase == TouchPhase.Began)
            //{
            //    //Debug.Log("double tap 2");
            //    doubleTap2Detected = true;
            //    //touchLastTap = Time.time;
            //    //Debug.Log("double tap time interval : " + (touchLastTap - touchfirstTap));
            //}
            //if (touch2.tapCount == 2 && touch2.phase == TouchPhase.Ended)
            //{
            //    doubleTap2Detected = false;
            //}
            // ====================== touch 2  =====================================

            //Touch 2 is tap + hold detected + bottom right screen
            if (hold1Detected
                && !buttonPressed
                && startTouchPosition2.x > (Screen.width / 2)
                && GameOptions.enemiesEnabled) // if swipe on right 1/2 of screen)) 
                                               //&& startTouchPosition2.x < (Screen.height / 2)) // if swipe on right 1/2 of screen)) )
            {
                // ====================== touch 2 + bottom right =====================================
                if (tap2Detected && startTouchPosition2.y < (Screen.height / 2))
                {
                    //Debug.Log("tap2Detected && !doubleTap2Detected");
                    buttonPressed = true;
                    if (!playerController.hasBasketball && playerController.CanAttack)
                    {
                        //Debug.Log("player attack ");
                        tap2Detected = false;
                        playerController.playerAttack();
                    }
                    buttonPressed = false;
                }
                // ====================== touch 2 + top right =====================================
                if (tap2Detected && startTouchPosition2.y > (Screen.height / 2))
                {
                    //Debug.Log("!tap2Detected && doubleTap2Detected");
                    buttonPressed = true;
                    if (!playerController.inAir
                        && playerController.Grounded
                        && !playerController.KnockedDown)
                    {
                        //Debug.Log("player special attack ");
                        //doubleTap2Detected = false;
                        hold1Detected = false;
                        playerController.playerSpecial();
                    }
                    buttonPressed = false;
                }
            }
            // ====================== touch 1 swipe distance  =====================================

            endTouchPosition1 = touch1.position;
            swipeDistance = endTouchPosition1.y - startTouchPosition1.y;

            // ====================== touch 1 swipe down : Pause  =====================================
            // not paused + swipe down + top right of screen
            if (touch1.tapCount == 1 && touch1.phase == TouchPhase.Ended // finger stoppped moving | *tapcount = 1 keeps pause from being called twice
                && Mathf.Abs(swipeDistance) > swipeDownTolerance // swipe is long enough
                && swipeDistance < 0 // swipe down
                && startTouchPosition1.x > (Screen.width / 2) // if swipe on right 1/2 of screen)) 
                && startTouchPosition1.x > (Screen.height / 2)) // if swipe on right 1/2 of screen)) 
            {
                Pause.instance.TogglePause();
            }
        }

        if (tap1Detected && !GameOptions.EnemiesOnlyEnabled)
        {
            GameLevelManager.instance.PlayerState.touchControlJumpOrShoot(touch1.position);
            tap1Detected = false;
        }

        // ====================== touch 1 : Paused  =====================================

        // if paused + touch touch
        if (Pause.instance.Paused && Input.touchCount > 0)
        {
            Touch touch = Input.touches[0];
            //tap
            if (touch.tapCount == 1 && touch.phase == TouchPhase.Began)
            {
                selectPressedButton();
            }
            // on double tap, perform actions
            if (touch.tapCount == 2 && touch.phase == TouchPhase.Began && !buttonPressed)
            {
                activateDoubleTappedButton();
            }
        }
    }
    //#endif

    private void activateDoubleTappedButton()
    {
        //Debug.Log("double tap");
        // reload
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.LoadSceneButton.name)
            && !buttonPressed)
        {
            //Debug.Log("reload");
            Pause.instance.reloadScene();
            buttonPressed = true;
        }
        // start screen
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.LoadStartScreenButton.name)
            && !buttonPressed)
        {
            //Debug.Log("start screen");
            Pause.instance.loadstartScreen();
            buttonPressed = true;
        }
        // cancel/unpause
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.CancelMenuButton.name)
            && !buttonPressed)
        {
            //Debug.Log("cancel/un pause");
            Pause.instance.TogglePause();
            buttonPressed = true;
        }
        //quit
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.instance.QuitGameButton.name)
            && !buttonPressed)
        {
            //Debug.Log("quit");
            Pause.instance.quit();
            buttonPressed = true;
        }
        //// new stuff
        //if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleCameraName)
        //    && !buttonPressed)
        //{
        //    //Debug.Log("toggle camera");
        //    CameraManager.instance.switchCamera();
        //    buttonPressed = true;
        //}
        // toggle fps
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleFpsName)
            && !buttonPressed)
        {
            //Debug.Log("toggle fps");
            DevFunctions.instance.ToggleFpsCounter();
            buttonPressed = true;
        }
        // set max stats
        if (EventSystem.current.currentSelectedGameObject.name.Equals(Pause.ToggleMaxStatsName)
            && !buttonPressed)
        {
            //Debug.Log("set max stats");
            DevFunctions.instance.setMaxPlayerStats();
            buttonPressed = true;
        }
        // toggle ui stats
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

        //For every result returned, output the name of the GameObject on the Canvas hit by the Ray
        //foreach (RaycastResult result in results)
        //{
        //    Debug.Log("Hit " + result.gameObject.name);
        //}
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

