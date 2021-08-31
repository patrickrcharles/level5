﻿using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] cameras;

    cameraUpdater cameraUpdater;
    Camera cam;
    [SerializeField]
    int currentCameraIndex;
    //int defaultCameraIndex = 0;
    [SerializeField]
    int numberOfCameras;

    //bool locked;
    private int cameraOnGoalIndex;
    private int cameraFollowBallIndex;

    bool cameraOnGoalAllowed;

    public static CameraManager instance;

    public int CameraOnGoalIndex { get => cameraOnGoalIndex; }
    public int CameraFollowBallIndex { get => cameraFollowBallIndex; }
    public GameObject[] Cameras { get => cameras; set => cameras = value; }
    public bool CameraOnGoalAllowed { get => cameraOnGoalAllowed; set => cameraOnGoalAllowed = value; }

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        CameraOnGoalAllowed = true;

        //defaultCameraIndex = 0;
        //currentCameraIndex = defaultCameraIndex;
        Cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        setDefaultCamera();
    }

    // Update is called once per frame
    //void Update()
    //{
    //    //if (GameLevelManager.instance.Controls.Other.change.enabled
    //    //    &&
    //    //    GameLevelManager.instance.Controls.Other.toggle_camera_keyboard.triggered
    //    //    && !locked
    //    //    && !Pause.instance.Paused)
    //    //{
    //    //    locked = true;
    //    //    switchCamera();
    //    //}
    //}

    void setDefaultCamera()
    {
        for (int i = 0; i < Cameras.Length; i++)
        {
            // #review for better way to make this more generic
            // if level requires weather system. currently only this level so can hardcode
            // doesnt work unless game options display name set
            if (GameOptions.levelDisplayName != null && GameOptions.levelDisplayName.ToLower().Contains("norf"))
            {
                Cameras[i].GetComponent<cameraUpdater>().RequiresWeatherSystem = true;
                //Debug.Log("level requires WEATHER");
            }

            if (Cameras[i].name.Contains("perspective_1"))
            {
                currentCameraIndex = i;
                Cameras[i].SetActive(true);
                //Cameras[i].GetComponent<Camera>().enabled = true;
                numberOfCameras++;
            }
            //if (i == defaultCameraIndex)
            //{
            //    Cameras[i].SetActive(true);
            //    numberOfCameras++;
            //}
            if (!Cameras[i].name.Contains("perspective"))
            {
                Cameras[i].SetActive(false);
                //Cameras[i].GetComponent<Camera>().enabled = false;
                numberOfCameras++;
            }
            //if (Cameras[i].name.Contains("follow_ball"))
            //{
            //    cameraFollowBallIndex = i;
            //    numberOfCameras--;
            //}
            if (Cameras[i].name.Contains("goal"))
            {
                cameraOnGoalIndex = i;
                Cameras[i].SetActive(false);
                //Cameras[i].GetComponent<Camera>().enabled = false;
                //numberOfCameras--;
            }
        }
        //Debug.Log("**************************************************** setDefaultCamera :   current cam : " + cameras[currentCameraIndex].name);
        //foreach (GameObject cam in cameras)
        //{
        //    //Debug.Log(cam.name + " is active : " + cam.activeSelf);
        //}
    }

    public void switchCamera()
    {
        //Debug.Log("**************************************************** switch camera :   current cam : " + cameras[currentCameraIndex].name);
        //Debug.Log(" --------------------------------------------------- currentCameraIndex : " + currentCameraIndex);
        // if not last camera, go to next
        if (currentCameraIndex < numberOfCameras)
        {
            currentCameraIndex++;
            //Debug.Log("     if (currentCameraIndex < numberOfCameras)");
        }
        // current camera at end of array, go to default/first camera
        if (currentCameraIndex >= numberOfCameras)
        {
            currentCameraIndex = 0;
            //Debug.Log("     if (currentCameraIndex >= numberOfCameras)");
        }
        //Debug.Log(" --------------------------------------------------- currentCameraIndex : " + currentCameraIndex);
        // set next camera active based on incremented index
        // set other cameras inactive
        for (int i = 0; i < numberOfCameras; i++)
        {
            if (i == currentCameraIndex)
            {
                //Debug.Log("     if (i == currentCameraIndex)        camera[i] : "+ cameras[i].activeSelf);
                //Cameras[i].SetActive(true);
                Cameras[i].SetActive(true);
                //Cameras[i].GetComponent<Camera>().enabled = true;
            }
            if (i != currentCameraIndex)
            {
                //Debug.Log("     if (i != currentCameraIndex)        camera[i] : " + cameras[i].activeSelf);
                Cameras[i].SetActive(false);
                //Cameras[i].GetComponent<Camera>().enabled = false;
            }
            if (Cameras[i].name.Contains("goal"))
            {
                cameraOnGoalIndex = i;
                Cameras[i].SetActive(false);
                //Cameras[i].GetComponent<Camera>().enabled = false;
                //numberOfCameras--;
            }
        }

        StartCoroutine(turnOffMessageLogDisplayAfterSeconds(3));

        //locked = false;
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }
}
