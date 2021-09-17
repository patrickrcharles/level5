﻿using Assets.Scripts.Models;
using Assets.Scripts.restapi;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CreditsManager : MonoBehaviour
{
    [SerializeField]
    public string currentHighlightedButton;

    //version text
    private Text versionText;

    private const string startButtonName = "press_start";
    private const string inputFieldButtonName = "ReportInputField";
    private const string submitReportButtonName = "submit_report";

    private const string webLinkMusic = "https://www.instagram.com/stustumaru/";
    private const string webLinkDevProgress = "https://www.instagram.com/patrickcharlez/";
    private const string webLinkDevWebSite = "http://www.skeletondistrict.com/highscores/";
    private const string webLinkGooglePLay = "https://play.google.com/store/apps/details?id=com.level5.level5";
    private const string webLinkItchIo = "https://skeleton-district.itch.io/level-5";
    private const string webLinkBugReportEmail = "mailto:levelfivegames@gmail.com?subject=BugReport";

    [SerializeField]
    private GameObject submitReportButtonObject;
    [SerializeField]
    string reportInput;
    [SerializeField]
    InputField reportInputField;

    public PlayerControls controls;

    public static CreditsManager instance;

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
    }

    void Awake()
    {
        instance = this;

        controls = new PlayerControls();
        // find all button / text / etc and assign to variables
        //getUiObjectReferences();
        reportInputField = GameObject.Find(inputFieldButtonName).GetComponent<InputField>();
    }

    // Update is called once per frame
    void Update()
    {
        // check for some button not selected
        if (EventSystem.current != null)
        {
            if (EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(EventSystem.current.firstSelectedGameObject); // + "_description";
            }
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
        }

        // ================================== footer buttons =========================
        // start button | start game
        if ((controls.UINavigation.Submit.triggered
            || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(startButtonName))
        {
            loadStartMenu();
        }

        // ================================== submit report ====================
        // on submit/enter if (input field) highlight submit button
        if (EventSystem.current.currentSelectedGameObject.name.Equals(inputFieldButtonName)
            && controls.UINavigation.Submit.triggered)
        {
            EventSystem.current.SetSelectedGameObject(submitReportButtonObject);
        }
        if ((controls.UINavigation.Submit.triggered
            || controls.Player.shoot.triggered)
            && currentHighlightedButton.Equals(submitReportButtonName))
        {
            UserReportModel userReportModel = new UserReportModel();
            userReportModel.Report = reportInput;
            StartCoroutine( APIHelper.PostReport(userReportModel));
        }
    }


    // ============================  footer options activate - load scene/stats/quit/etc ==============================

    public void loadStartMenu()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
    }


    // ============================  message display ==============================
    // used in this context to display if item is locked

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }

    public void readReportInput(string s)
    {
        reportInput = reportInputField.text;
    }

    public void OpenMusicSite()
    {
        Application.OpenURL(webLinkMusic);
    }
    public void OpenDevProgressSite()
    {
        Application.OpenURL(webLinkDevProgress);
    }
    public void OpenDevWebSite()
    {
        Application.OpenURL(webLinkDevWebSite);
    }
    public void OpenGooglePlaySite()
    {
        Application.OpenURL(webLinkGooglePLay);
    }
    public void OpenItchIoSite()
    {
        Application.OpenURL(webLinkItchIo);
    }
    public void OpenBugReportEmail()
    {
        Application.OpenURL(webLinkBugReportEmail);
    }
}
