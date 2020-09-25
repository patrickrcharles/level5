﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float timeRemaining = 0;
    private float currentTime;
    public float CurrentTime => currentTime;
    [SerializeField]
    private float timeStart;
    int minutes = 0;
    float seconds = 0;

    [SerializeField]
    bool displayTimer = false;
    [SerializeField]
    private bool timerEnabled = false;
    private Text timerText;
    [SerializeField]
    private bool modeRequiresCountDown;
    [SerializeField]
    private bool modeRequiresCounter;

    bool timerTextLocked;
    public static Timer instance;

    private void Awake()
    {
        instance = this;
        timerText = GetComponent<Text>();
        timerText.text = "";
    }

    void Start()
    {

        // if requires custom timer
        if (GameOptions.gameModeThreePointContest 
            || GameOptions.gameModeFourPointContest 
            || GameOptions.gameModeAllPointContest)
        {
            timeStart = GameOptions.customTimer;
        }
        //default 2 minute timer
        else
        {
            // timer is 2 minutes
            timeStart = 180;
        }

        modeRequiresCounter = GameOptions.gameModeRequiresCounter;
        modeRequiresCountDown = GameOptions.gameModeRequiresCountDown;

        if (modeRequiresCounter || modeRequiresCountDown)
        {
            timerEnabled = true;
            displayTimer = true;
            //Debug.Log("display timer true");
        }
        else
        {
            timerEnabled = false;
        }
    }

    void Update()
    {
        // countdown timer
        currentTime += Time.deltaTime;

        if (modeRequiresCountDown)
        {
            timeRemaining = timeStart - currentTime;
            minutes = Mathf.FloorToInt(timeRemaining / 60);
            //seconds = Mathf.FloorToInt(timeRemaining - (minutes * 60));
            seconds = (timeRemaining - (minutes * 60));
        }

        if (modeRequiresCounter)
        {
            //minutes = Mathf.FloorToInt(currentTime / 60);
            //seconds = Mathf.FloorToInt(currentTime - (minutes * 60));
            minutes = Mathf.FloorToInt(currentTime / 60);
            seconds = (currentTime - (minutes * 60));
        }
        // gameover, disable timer display and set text to empty
        if (GameRules.instance.GameOver || timeRemaining < 0)
        {
            //Debug.Log("display timer false");
            displayTimer = false;
            timerText.text = "";
        }
        // time's up, pause and reset timer text
        if (timeRemaining <= 0
            && !GameRules.instance.GameOver
            && !modeRequiresCounter
            && timerEnabled)
        {
            // ball is in the air, let the shot go before pausing 
            // or player in air and has basketball
            // not consecutive game mode
            if (!BasketBall.instance.BasketBallState.InAir 
                // player in air, has ball
                && !(GameLevelManager.instance.PlayerState.hasBasketball && GameLevelManager.instance.PlayerState.inAir)
                // not consecutive shots game mode
                && !GameRules.instance.GameModeRequiresConsecutiveShots)
            {
                //Debug.Log("game over");
                GameRules.instance.GameOver = true;
            }
            // if consecutive shots mode and streak is less than 2
            if ((GameRules.instance.GameModeRequiresConsecutiveShots
                && BasketBallShotMade.instance.ConsecutiveShotsMade < 3))
            {
                //Debug.Log("game over");
                GameRules.instance.GameOver = true;
            }
        }

        if (displayTimer && timerEnabled && modeRequiresCountDown && timeRemaining > 0)
        {
            timerText.text = minutes.ToString("00") + " : " + seconds.ToString("00.000");
        }

        if (displayTimer && timerEnabled && modeRequiresCounter && !GameRules.instance.GameOver)
        {
            //timerTextLocked = true;
            //setCustomTimerText("OVERTIME");
            timerText.text = minutes.ToString("00") + " : " + seconds.ToString("00.000");
        }
    }

    void setCustomTimerText(string text)
    {
        timerText.text = text;
    }

    public float TimeStart
    {
        get => timeStart;
        set => timeStart = value;
    }

    public bool DisplayTimer
    {
        get => displayTimer;
        set => displayTimer = value;
    }
    public float Seconds { get => seconds; set => seconds = value; }
}
