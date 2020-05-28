
using System;
using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.PlayerLoop;

public class cameraUpdater : MonoBehaviour
{

    public GameObject player;

    public Camera cam;
    public float xMin, xMax, zMin, zMax, yMin, yMax;

    public float distanceCamFromPlayer, distanceRimFromPlayer;

    [SerializeField]
    GameObject basketBallRim;

    public Vector3 playerPos, camPos, rimPos;
    public float floatcameraDistanceFromGoal;

    public float ZoomAmount = 0; //With Positive and negative values
    public float MaxToClamp = 10;
    public float ROTSpeed = .1f;

    [SerializeField]
    bool cameraZoomedIn, cameraZoomedOut;

    public float startZoomDistance;

    [SerializeField]
    private float addToCameraPosY;

    [SerializeField]
    float playerDistanceFromRimX;
    [SerializeField]
    float playerDistanceFromRimZ;

    [SerializeField]
    bool isOrthoGraphic;

    bool mainPerspectiveCamActive;
    bool orthoCam1Active;
    bool orthoCam2Active;
    bool isFollowBallCamera;

    [SerializeField]
    public float smoothSpeed = 0.125f;
    [SerializeField]
    public Vector3 offset;

    bool cameraLockToGoal;
    private bool locked;

    void Start()
    {

        //Debug.Log(("camera updater Start()"));
        basketBallRim = GameObject.Find("rim");

        cam = GetComponent<Camera>();
        //cam.depth = -5;

        // this is for the sorting layers. when using perspective camera like i am,
        // sometimes the rendering isnt always done by z values because perspective 
        // uses a value that closest to center of the camera or something
        // this should finally fix all the rendering problems i've been having
        cam.transparencySortMode = TransparencySortMode.Orthographic;

        // will check settings and set intial camera
        setCamera();

        player = GameLevelManager.Instance.Player;
        //relCameraPos = player.position - transform.position;

    }


    void Update()
    {
        //Debug.Log(" zoom amount: " + ZoomAmount);

        playerPos = new Vector3(player.transform.position.x,
            0, player.transform.position.z);
        //camPos = new Vector3(cam.transform.position.x,
        //    0, cam.transform.root.position.z);

        rimPos = new Vector3(basketBallRim.transform.position.x,
            0, basketBallRim.transform.root.position.z);

        //distanceCamFromPlayer = Vector3.Distance(playerPos, camPos);
        distanceRimFromPlayer = rimPos.z - playerPos.z;

        playerDistanceFromRimX = player.transform.position.x;
        playerDistanceFromRimZ = Math.Abs(player.transform.position.z);
    }

    void FixedUpdate()
    {
        /* todo
         * if( player.x > 6 || player.x < -6)
         *  clamp camera.x to position
         *  ranf is roughly -1 --> 5.7
         */

        //Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + addToCameraPosY, cam.transform.position.z);
        //Vector3 desiredPosition = targetPosition + offset;
        //Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        //transform.position = smoothedPosition;

        if ((player != null) && mainPerspectiveCamActive && !isFollowBallCamera)
        {
            // * note change var to player distance because each camera is in a different spot
            if ((playerDistanceFromRimX < -5.5 || playerDistanceFromRimX > 5.5)
                && !((playerDistanceFromRimX < -6.8 || playerDistanceFromRimX > 6.8)))
            {
                updatePositionNearGoal();
            }
            else
            {
                updatePositionOnPlayer();
            }
        }

        if ((player != null) && isOrthoGraphic && !isFollowBallCamera)
        {
            transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, xMin, xMax),
                //cam.transform.position.y,
                addToCameraPosY,
                cam.transform.position.z);
        }
        if ((player != null) && isFollowBallCamera)
        {
            transform.position = new Vector3(BasketBall.instance.transform.position.x,
                 BasketBall.instance.transform.position.y + 0.5f,
                 BasketBall.instance.transform.position.z - 2);
        }

        if (distanceRimFromPlayer > startZoomDistance
            && !cameraZoomedOut && !isFollowBallCamera)
        //&& cam.transform.position.z > zMin)
        {
            zoomOut();
        }
        if (distanceRimFromPlayer < startZoomDistance && cameraZoomedOut)
        {
            zoomIn();
        }
    }

    private void updatePositionOnPlayer()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x, player.transform.position.y + addToCameraPosY, cam.transform.position.z);
        Vector3 desiredPosition = targetPosition + offset;
        Vector3 smoothedPosition = Vector3.Lerp(gameObject.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }

    private void updatePositionNearGoal()
    {
        Vector3 targetPosition = new Vector3(cam.transform.position.x, player.transform.position.y + addToCameraPosY, cam.transform.position.z);
        Vector3 desiredPosition = targetPosition + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    private void zoomOut()
    {
        if (ZoomAmount == -20)
        {
            cameraZoomedOut = true;
        }
        else
        {
            cameraZoomedOut = false;
        }

        //Debug.Log("zoom out camera");
        ZoomAmount -= .5f;
        //Debug.Log("zoomAmount : " + ZoomAmount + "Input.GetAxis(mouse_axis_2) : " + Input.GetAxis("mouse_axis_2"));
        ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
        var translate = Mathf.Min(Mathf.Abs(-1), MaxToClamp - Mathf.Abs(ZoomAmount));
        gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(-1));
    }


    private void zoomIn()
    {
        if (ZoomAmount == 0.5f)
        {
            cameraZoomedOut = false;
        }
        //Debug.Log("zoom in camera");
        ZoomAmount += .5f;
        //Debug.Log("zoomAmount : " + ZoomAmount + "Input.GetAxis(mouse_axis_2) : " + Input.GetAxis("mouse_axis_2"));
        ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
        var translate = Mathf.Min(Mathf.Abs(1), MaxToClamp - Mathf.Abs(ZoomAmount));
        gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(1));
    }

    void setCamera()
    {

        // if perspective camera
        if (!isOrthoGraphic)
        {
            addToCameraPosY = 1.835f;
            mainPerspectiveCamActive = true;
            orthoCam1Active = false;
            orthoCam2Active = false;
        }

        // 2 orthographic cameras
        if (isOrthoGraphic)
        {
            if (cam.name.Contains("camera_orthographic_1"))
            {
                addToCameraPosY = 2.5f;
                mainPerspectiveCamActive = false;
                orthoCam1Active = true;
            }
            if (cam.name.Contains("camera_orthographic_2"))
            {
                addToCameraPosY = 3.3f;
                mainPerspectiveCamActive = false;
                orthoCam1Active = false;
                orthoCam2Active = true;
            }
        }
        if (cam.name.Contains("follow_ball"))
        {
            isFollowBallCamera = true;
        }
        else
        {
            isFollowBallCamera = false;
        }
    }
}