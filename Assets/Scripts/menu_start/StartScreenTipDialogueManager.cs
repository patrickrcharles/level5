using Assets.Scripts.database;
using Assets.Scripts.restapi;
using Assets.Scripts.Utility;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScreenTipDialogueManager : MonoBehaviour
{
    [HideInInspector]
    public int NONE = 0, YES = 1, CANCEL = 2, NEXT = 3;
    [HideInInspector]
    public int result = 0;

    [SerializeField]
    private Button cancelButton;
    [SerializeField]
    private Button nextButton;

    [SerializeField]
    Text tipText;
    [SerializeField]
    List<PlayerTips> tipsList;

    int randomTipIndex = 0;
    PlayerControls controls;

    public Button CancelButton { get => cancelButton; set => cancelButton = value; }
    public Button NextButton { get => nextButton; set => nextButton = value; }

    private void OnEnable()
    {
        if (!GameOptions.tipDialogueLoadedOnStart)
        {
            controls.Player.Enable();
            controls.UINavigation.Enable();
            controls.Other.Enable();
        }
    }
    private void OnDisable()
    {
        if (!GameOptions.tipDialogueLoadedOnStart)
        {
            controls.Player.Disable();
            controls.UINavigation.Disable();
            controls.Other.Disable();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!GameOptions.tipDialogueLoadedOnStart)
        {
            //instance = this;
            controls = new PlayerControls();
            if (GameObject.Find("cancel_button") != null)
            {
                cancelButton = GameObject.Find("cancel_button").GetComponent<Button>();
                cancelButton.onClick.AddListener(CancelButtonOnClick);
            }
            if (GameObject.Find("next_button") != null)
            {
                nextButton = GameObject.Find("next_button").GetComponent<Button>();
                nextButton.onClick.AddListener(NextButtonOnClick);
            }

            int i = 0;
            foreach (PlayerTips tip in tipsList)
            {
                tip.tipId = i;
                i++;
            }
            randomTipIndex = UtilityFunctions.GetRandomInteger(0, tipsList.Count);
            tipText.text = tipsList[randomTipIndex].tip;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void Start()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
        GameOptions.tipDialogueLoadedOnStart = true;
    }

    private void CloseTipDialogue()
    {
        EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject);
        Destroy(this.gameObject);
    }
    public void NextTipButton()
    {
        //  16 < 17
        if (randomTipIndex < tipsList.Count - 1)
        {
            randomTipIndex++;
            tipText.text = tipsList[randomTipIndex].tip;
        }
        // 17 = 17, 18 < 17
        else
        {
            randomTipIndex = 0;
            tipText.text = tipsList[0].tip;
        }
        EventSystem.current.SetSelectedGameObject(nextButton.gameObject);
    }

    public void CancelButtonOnClick()
    {
        result = CANCEL;
        CloseTipDialogue();
    }

    public void NextButtonOnClick()
    {
        result = NEXT;
        NextTipButton();
    }
}