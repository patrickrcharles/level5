using System.Collections;
using System.Collections.Generic;
using static TeamUtility.IO.InputManager;
using UnityEngine;
using UnityEngine.UI;

public class CameraManager : MonoBehaviour
{
    GameObject[] cameras;

    cameraUpdater cameraUpdater;
    Camera cam;

    int currentCameraIndex;
    int defaultCameraIndex = 0;

    bool locked;
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

        CameraOnGoalAllowed = false;

        defaultCameraIndex = 0;
        currentCameraIndex = defaultCameraIndex;
        Cameras = GameObject.FindGameObjectsWithTag("MainCamera");
        setDefaultCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (GetKey(KeyCode.LeftShift) && GetKeyDown(KeyCode.Alpha3) && !locked)
        {
            locked = true;
            switchCamera();
        }
        if (GetKey(KeyCode.LeftShift) && GetKeyDown(KeyCode.Alpha4) && !locked)
        {
            CameraOnGoalAllowed = !CameraOnGoalAllowed;

            Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
            messageText.text = "camera view on goal = " + CameraOnGoalAllowed;
            // turn off text display after 5 seconds
            StartCoroutine(turnOffMessageLogDisplayAfterSeconds(5));
        }
    }

    void setDefaultCamera()
    {
        for (int i = 0; i < Cameras.Length; i++)
        {
            if (i == defaultCameraIndex)
            {
                Cameras[i].SetActive(true);
            }
            if (i != defaultCameraIndex)
            {
                Cameras[i].SetActive(false);
            }
            if (Cameras[i].name.Contains("follow_ball"))
            {
                cameraFollowBallIndex = i;
            }
            if (Cameras[i].name.Contains("goal"))
            {
                cameraOnGoalIndex = i;
            }
            Debug.Log("cam[i].name : " + Cameras[i].name);
        }
    }

    void switchCamera()
    {
        // if not last camera, go to next
        if (currentCameraIndex < Cameras.Length)
        {
            currentCameraIndex++;
        }

        // current camera at end of array, go to default/first camera
        if (currentCameraIndex >= Cameras.Length)
        {
            currentCameraIndex = 0;
        }

        // set next camera active based on incremented index
        // set other cameras inactive
        for (int i = 0; i < Cameras.Length; i++)
        {
            if (i == currentCameraIndex)
            {
                Cameras[i].SetActive(true);
            }
            if (i != currentCameraIndex)
            {
                Cameras[i].SetActive(false);
            }
        }
        locked = false;
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }
}
