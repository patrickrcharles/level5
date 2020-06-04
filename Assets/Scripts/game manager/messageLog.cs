﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class messageLog : MonoBehaviour
{
    Text log;

    public static messageLog instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        log = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void toggleMessageDisplay(String message)
    {

        StartCoroutine(ToggleMessageDisplayLog(5, message));
    }

    IEnumerator ToggleMessageDisplayLog(float seconds, String message)
    {
        log.text = message;
        yield return new WaitForSeconds(seconds);
        log.text = "";
    }
}
