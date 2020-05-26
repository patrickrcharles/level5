using System.Collections;
using System.Collections.Generic;
using static TeamUtility.IO.InputManager;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    GameObject[] cameras;

    cameraUpdater cameraUpdater;
    Camera cam;

    int currentCameraIndex;
    int defaultCameraIndex = 0;

    bool locked;

    public static CameraManager instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        defaultCameraIndex = 0;
        currentCameraIndex = defaultCameraIndex;
        cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        setDefaultCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if(GetKey(KeyCode.LeftShift) && GetKeyDown(KeyCode.Alpha3) && !locked)
        {
            locked = true;
            switchCamera();
        }
    }

    void setDefaultCamera()
    {
        for(int i = 0; i < cameras.Length; i++)
        {
            if(i == defaultCameraIndex)
            {
                cameras[i].SetActive(true);
            }
            if(i != defaultCameraIndex)
            {
                cameras[i].SetActive(false);
            }
        }
    }

    void switchCamera()
    {
        // if not last camera, go to next
        if(currentCameraIndex < cameras.Length)
        {
            currentCameraIndex++;
        }

        // current camera at end of array, go to default/first camera
        if (currentCameraIndex >= cameras.Length)
        {
            currentCameraIndex = 0;
        }

        // set next camera active based on incremented index
        // set other cameras inactive
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i == currentCameraIndex)
            {
                cameras[i].SetActive(true);
            }
            if ( i != currentCameraIndex)
            {
                cameras[i].SetActive(false);
            }
        }
        locked = false;
    }
}
