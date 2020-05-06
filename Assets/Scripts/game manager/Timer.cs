using System;
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
    float timeStart;
    int minutes = 0;
    int seconds = 0;

    [SerializeField]
    bool displayTimer = true;

    [SerializeField] private bool timerEnabled = false;
    //Text timerText;
    private Text timerText;

    private void Awake()
    {
        timerText = GetComponent<Text>();
    }

    void Update()
    {
        // countdown timer
        currentTime += Time.deltaTime;
        timeRemaining = timeStart - currentTime;
        minutes = Mathf.FloorToInt(timeRemaining / 60);
        seconds = Mathf.FloorToInt(timeRemaining - (minutes * 60));

        // time's up, pause and reset timer text
        if (timeRemaining <= 0 && !GameRules.instance.GameOver && timerEnabled)
        {
            displayTimer = false;
            timerText.text = "";
            // ball is in the air, let the shot go before pausing 
            if (!BasketBall.instance.BasketBallState.InAir)
            {
                //Debug.Log("timer ended");
                GameRules.instance.GameOver = true;
                //Pause.instance.Paused = true;
                //Time.timeScale = 0f;
            }
        }

        if (displayTimer && timerEnabled)
        {
            timerText.text = minutes.ToString("00") + " : " + seconds.ToString("00");
        }
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
}
