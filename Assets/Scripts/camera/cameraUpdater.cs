
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



    void Start()
    {

        //Debug.Log(("camera updater Start()"));
        basketBallRim = GameObject.Find("rim");

        cam = GetComponent<Camera>();
        cam.depth = -5;

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

        playerDistanceFromRimX = Math.Abs( player.transform.position.x );
        playerDistanceFromRimZ = Math.Abs( player.transform.position.z );
    }

    void FixedUpdate()
    {
        /* todo
         * if( player.x > 6 || player.x < -6)
         *  clamp camera.x to position
         *  ranf is roughly -1 --> 5.7
         */

        if ((player != null) && mainPerspectiveCamActive)
        {
            transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, xMin, xMax),
                //cam.transform.position.y,
                player.transform.position.y + addToCameraPosY,
                cam.transform.position.z);
        }
        if ((player != null) && isOrthoGraphic)
        {
            transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, xMin, xMax),
                //cam.transform.position.y,
                addToCameraPosY,
                cam.transform.position.z);
        }

        if (distanceRimFromPlayer > startZoomDistance
            && !cameraZoomedOut)
        //&& cam.transform.position.z > zMin)
        {
            zoomOut();
        }
        if (distanceRimFromPlayer < startZoomDistance && cameraZoomedOut)
        {
            zoomIn();
        }

        //if (playerDistanceFromRimX > 6f && playerDistanceFromRimZ < 2 && cameraZoomedIn)
        //{
        //    Debug.Log(" zoom out side");
        //    zoomOut();
        //}
        //if (playerDistanceFromRimX < 6f && playerDistanceFromRimZ > 2 && cameraZoomedOut)
        //{
        //    Debug.Log(" zoom in side");
        //    zoomIn();
        //}
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
    }
}