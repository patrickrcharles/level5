using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    [SerializeField]
    float timeRemaining = 0;
    [SerializeField]
    private float currentTime;

    [SerializeField]
    float timeStart;
    [SerializeField]
    int minutes = 0;
    [SerializeField]
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
}
