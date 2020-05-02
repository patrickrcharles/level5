using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float timeRemaining = 0;
    private float currentTime;
    float timeStart;
    int minutes = 0;
    int seconds = 0;
    bool displayTimer = true;

    Text timerText;

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

        // time's up, pause and reste timer text
        if (timeRemaining <= 0)
        {
            displayTimer = false;
            timerText.text = "";
            Time.timeScale = 0f;
        }

        if (displayTimer)
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
