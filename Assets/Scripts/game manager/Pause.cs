using System;
using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [SerializeField]
    private bool paused;
    public static Pause instance;
    private Image fadeTexture;

    void Awake()
    {
        instance = this;
        fadeTexture = GameObject.Find("fade_texture").GetComponent<Image>();
        if (Time.timeScale == 1f)
        {
            setBackgroundFade(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //pause ESC, submit, cancel
        if (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Cancel")
            || InputManager.GetKeyDown(KeyCode.Escape))
        {
            paused = TogglePause();
        }
    }


    public bool TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            //gameManager.instance.backgroundFade.SetActive(false);
            Time.timeScale = 1f;
            setBackgroundFade(false);
            //resumeAllAudio();
            return (false);
        }
        else
        {
            //gameManager.instance.backgroundFade.SetActive(true);
            Time.timeScale = 0f;
            //pauseAllAudio();
            setBackgroundFade(true);
            return (true);
        }
    }

    public void setBackgroundFade(bool value)
    {
        fadeTexture.enabled = value;
    }

    public object Paused
    {
        get => paused;
    }
}
