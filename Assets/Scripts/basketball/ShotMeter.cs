using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotMeter : MonoBehaviour
{
 
    [SerializeField]
    float sliderValueOnButtonPress;

    public float SliderValueOnButtonPress => sliderValueOnButtonPress;

    [SerializeField]
    public float meterTime;
    [SerializeField]
     float meterStartTime;
    [SerializeField]
     float meterEndTime;
    [SerializeField]
     float meterIncrement;
    [SerializeField]
     ShooterProfile shooterProfile;
    [SerializeField]
     public Slider slider;
    [SerializeField]
     bool meterStarted;

     [SerializeField]
     bool meterEnded;
    [SerializeField]
     bool sliderMaxReached;
    [SerializeField]
     bool sliderMinReached;

     [SerializeField] private float currentTime;

     public float meterFillTime;
      bool locked;

     // Start is called before the first frame update
    void Start()
    {
        shooterProfile = GameLevelManager.Instance.Player.GetComponent<ShooterProfile>();
        slider = GetComponentInChildren<Slider>();
        meterFillTime = calculateSliderFillTime();
        Debug.Log("meterfill time : "+meterFillTime);
        //resetShotMeter();
    }


    // Update is called once per frame
    void Update()
    {
        // if player grounded reset slider
        if (GameLevelManager.Instance.PlayerState.grounded)
        {
            slider.value = 0;
        }

        if (slider.value >= 100)
        {
            Debug.Log("if (slider.value >= 100)");
            sliderMaxReached = true;
        }

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

                Debug.Log("     start : " + meterStartTime + "  end : " + meterEndTime);
                Debug.Log("     current time : " + currentTime);
                Debug.Log("     start time : " + meterStartTime);
                Debug.Log("     end time : " + meterEndTime);
                Debug.Log("     MeterEndTime - currentTime : " + ((MeterEndTime - currentTime)));
                Debug.Log("     meterEndTime - startTime : " +  (MeterEndTime - MeterStartTime));
                Debug.Log("     meter fill time 1 : " + meterFillTime + "   %: "+ (((currentTime - meterStartTime) / (meterFillTime)*100)));
                Debug.Log("     meter fill time 2 : " + meterFillTime + "   %: " + (((currentTime - meterStartTime) / (MeterEndTime - MeterStartTime)) * 100));
                slider.value = (((currentTime - meterStartTime) / (meterFillTime)) * 100);
                Debug.Log("slider.value : " + slider.value + "=======================================================================");
            }

            if (sliderMaxReached)
            {
                currentTime = Time.time;
                slider.value = 90 - Math.Abs(100 - (((currentTime - meterStartTime) / (meterFillTime)) * 100));
                Debug.Log("slider.value : " + slider.value + "///////////////////////////////////////////////////////////// >= 100 =====");
            }
        }
        //Debug.Log("slider.value : " + slider.value + "###################################################################################################################");

        // ===========================================
        if (meterEnded)
        {
            sliderValueOnButtonPress = ((((Time.time - meterStartTime) / (meterFillTime)) *100));
            Debug.Log("sliderValueOnButtonPress : "+ sliderValueOnButtonPress);
            Debug.Log("time to press : "+ sliderValueOnButtonPress);
            //Debug.Log("% : " + ((sliderValueOnButtonPress - meterStartTime) / (meterEndTime- meterStartTime)) + "=======================================================================");
            meterStarted = false;
            meterEnded = false;

            sliderMaxReached = false;
            //resetShotMeter();
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

    //private void resetShotMeter()
    //{
    //    Debug.Log("resetShotMeter()");
    //    MeterStartTime = Time.time;
    //    MeterEndTime = Time.time + meterFillTime;
    //}

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
}
