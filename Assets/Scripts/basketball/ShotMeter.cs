using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotMeter : MonoBehaviour
{
    private const string sliderValueOnPressName = "slider_value_text";
    [SerializeField]
    private Text sliderValueOnPress;

    float sliderValueOnButtonPress;
    public float SliderValueOnButtonPress => sliderValueOnButtonPress;

    ShooterProfile shooterProfile;
    Slider slider;
    public Slider Slider => slider;

    float meterTime;
    float meterStartTime;
    float meterEndTime;
    float meterIncrement;

    bool meterStarted;
    bool meterEnded;
    bool sliderMaxReached;
    bool sliderMinReached;

    private float currentTime;

    public float meterFillTime;
    bool locked;

    // Start is called before the first frame update
    void Start()
    {
        shooterProfile = GameLevelManager.Instance.Player.GetComponent<ShooterProfile>();
        slider = GetComponentInChildren<Slider>();
        meterFillTime = calculateSliderFillTime(); // time for shot meter active, based on player jump/time until jump peak
        sliderValueOnPress = GameObject.Find(sliderValueOnPressName).GetComponent<Text>();
    }


    // Update is called once per frame
    void Update()
    {
        // if player grounded reset slider
        if (GameLevelManager.Instance.PlayerState.grounded)
        {
            slider.value = 0;
        }
        // if meter hits max
        if (slider.value >= 100)
        {
            sliderMaxReached = true;
        }
        //
        if (meterStarted && !locked)
        {
            locked = true;
        }

        if (meterEnded)
        {
            locked = false;
        }

        if (meterStarted && locked)
        {
            //ShotEnded = false;
            if (!sliderMaxReached)
            {
                currentTime = Time.time;
                meterEndTime = meterStartTime + meterFillTime;
                slider.value = (((currentTime - meterStartTime) / (meterFillTime)) * 100);
                // in case this is where it hits 100, it can carry over to next next if statement and get overwritten
                if (slider.value >= 100)
                {
                    sliderMaxReached = true;
                }
            }

            if (sliderMaxReached)
            {
                currentTime = Time.time;
                slider.value = 90 - Math.Abs(100 - (((currentTime - meterStartTime) / (meterFillTime)) * 100));

            }
        }

        if (meterEnded)
        {
            sliderValueOnButtonPress = ((((Time.time - meterStartTime) / (meterFillTime)) * 100));
            // display number
            displaySliderValueOnPressText(slider.value.ToString("###"));
            meterStarted = false;
            meterEnded = false;
            sliderMaxReached = false;
        }
    }

    public float MeterStartTime
    {
        get => meterStartTime;
        set => meterStartTime = value;
    }

    public float MeterEndTime
    {
        get => meterEndTime;
        set => meterEndTime = value;
    }

    float calculateSliderFillTime()
    {
        float time = shooterProfile.JumpForce / Physics.gravity.y;
        return Math.Abs(time);
    }
    public bool MeterStarted
    {
        get => meterStarted;
        set => meterStarted = value;
    }

    public bool MeterEnded
    {
        get => meterEnded;
        set => meterEnded = value;
    }

    public void displaySliderValueOnPressText(String message)
    {

        StartCoroutine(toggleSliderValueOnPressText(2, message));
    }

    IEnumerator toggleSliderValueOnPressText(float seconds, String message)
    {
        sliderValueOnPress.text = message;
        yield return new WaitForSeconds(seconds);
        sliderValueOnPress.text = "";
    }
}
