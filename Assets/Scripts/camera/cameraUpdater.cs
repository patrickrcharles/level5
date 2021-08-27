
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class cameraUpdater : MonoBehaviour
{

    GameObject player;
    Camera cam;
    Vector3 basketBallRim;
    public Vector3 playerPos, camPos, rimPos;

    public float xMin, xMax, zMin, zMax, yMin, yMax;
    public float distanceCamFromPlayer, distanceRimFromPlayer;
    public float floatcameraDistanceFromGoal;

    public float ZoomAmount = 0; //With Positive and negative values
    public float MaxToClamp = 10;
    public float ROTSpeed = 0.1f;

    [SerializeField]
    bool cameraZoomedIn, cameraZoomedOut;
    [SerializeField]
    float startZoomDistance;
    [SerializeField]
    private float addToCameraPosY;
    [SerializeField]
    float playerDistanceFromRimX;
    [SerializeField]
    float playerDistanceFromRimZ;

    [SerializeField]
    bool isOrthoGraphic;

    bool mainPerspectiveCamActive;
    //bool orthoCam1Active;
    //bool orthoCam2Active;
    bool isFollowBallCamera;

    [SerializeField]
    public float smoothSpeed = 0.125f;
    [SerializeField]
    private Vector3 lockOnGoalCameraOffset;

    bool cameraLockToGoal;
    private bool locked;
    [SerializeField]
    private bool isLockOnGoalCamera;

    bool onGoalCameraEnabled;
    [SerializeField]
    bool smoothCameraMotion;

    // flag for activating weather system prefab
    // set this in camera manager because it is based on a specific level
    // GM if( level requires weather ) --> for each cam, requires weather = true;
    // Cam Update if(requires weather) weather.setActive(true)
    GameObject weatherSystemObject;
    [SerializeField]
    bool requiresWeatherSystem;
    [SerializeField]
    private float cameraOffset;

    public bool RequiresWeatherSystem { get => requiresWeatherSystem; set => requiresWeatherSystem = value; }

    void Start()
    {
        requiresWeatherSystem = GameOptions.levelRequiresWeather;

        // get weather system object reference
        foreach (Transform t in gameObject.transform)
        {
            //Debug.Log("transform name : " + t.name + "  transform tage : "+ t.tag);
            //#hack
            if (t.CompareTag("weather_system") && !t.name.Contains("goal"))
            {
                weatherSystemObject = t.gameObject;
                if (requiresWeatherSystem || SceneManager.GetActiveScene().name.Equals(Constants.SCENE_NAME_level_03_snow))
                {
                    //Debug.Log("WEATHER ACTIVE -- \ntransform name : " + t.name + "  transform tage : " + t.tag);
                    weatherSystemObject.SetActive(true);
                }
                else
                {
                    weatherSystemObject.SetActive(false);
                }
            }
        }

        if (GameLevelManager.instance != null)
        {
            basketBallRim = GameLevelManager.instance.BasketballRimVector;
            player = GameLevelManager.instance.Player;
            smoothCameraMotion = true;
        }
        else
        {
            player = GameObject.FindGameObjectWithTag("Player");
            smoothCameraMotion = false;
        }

        cam = GetComponent<Camera>();
        //cam.depth = -5;

        // this is for the sorting layers. when using perspective camera like i am,
        // sometimes the rendering isnt always done by z values because perspective 
        // uses a value that closest to center of the camera or something
        // this should finally fix all the rendering problems i've been having
        cam.transparencySortMode = TransparencySortMode.Orthographic;

        // will check settings and set intial camera
        setCamera();

  
        //relCameraPos = player.position - transform.position;

    }


    void Update()
    {
        if (GameLevelManager.instance != null) 
        {
            playerDistanceFromRimX = basketBallRim.x - player.transform.position.x;
            playerDistanceFromRimZ = Math.Abs(player.transform.position.z);
        }

        if (!CameraManager.instance.CameraOnGoalAllowed && onGoalCameraEnabled)
        {
            CameraManager.instance.Cameras[CameraManager.instance.CameraOnGoalIndex].SetActive(false);
            onGoalCameraEnabled = false;
        }

        // * note change var to player distance because each camera is in a different spot
        if (Math.Abs(playerDistanceFromRimX) > 8 && !onGoalCameraEnabled
            && CameraManager.instance.CameraOnGoalAllowed
            && !GameOptions.EnemiesOnlyEnabled)
        {
            toggleCameraOnGoal();
        }

        if (Math.Abs(playerDistanceFromRimX) < 8 && onGoalCameraEnabled
            && CameraManager.instance.CameraOnGoalAllowed
            && !GameOptions.EnemiesOnlyEnabled)
        {
            toggleCameraOnGoal();
        }
        //if (isLockOnGoalCamera)
        //{
        //    transform.position = basketBallRim + lockOnGoalCameraOffset;
        //}


        //if (distanceRimFromPlayer > startZoomDistance
        //    && !cameraZoomedOut && !isFollowBallCamera && !isLockOnGoalCamera)
        ////&& cam.transform.position.z > zMin)
        //{
        //    zoomOut();
        //}
        //if (distanceRimFromPlayer < startZoomDistance && cameraZoomedOut)
        //{
        //    zoomIn();
        //

    }

    void FixedUpdate()
    {
        if (GameLevelManager.instance != null)
        {
            playerDistanceFromRimX = basketBallRim.x - player.transform.position.x;
            playerDistanceFromRimZ = Math.Abs(player.transform.position.z);
        }

        //if (!CameraManager.instance.CameraOnGoalAllowed && onGoalCameraEnabled)
        //{
        //    CameraManager.instance.Cameras[CameraManager.instance.CameraOnGoalIndex].SetActive(false);
        //    onGoalCameraEnabled = false;
        //}

        //// * note change var to player distance because each camera is in a different spot
        //if (Math.Abs(playerDistanceFromRimX) > 8.5f && !onGoalCameraEnabled
        //    && CameraManager.instance.CameraOnGoalAllowed)
        //{
        //    toggleCameraOnGoal();
        //}

        //if (Math.Abs(playerDistanceFromRimX) < 8.5f && onGoalCameraEnabled
        //    && CameraManager.instance.CameraOnGoalAllowed)
        //{
        //    toggleCameraOnGoal();
        //}
        if (isLockOnGoalCamera)
        {
            transform.position = basketBallRim + lockOnGoalCameraOffset;
        }

        if ((player != null) && mainPerspectiveCamActive && !isFollowBallCamera && !isLockOnGoalCamera)
        {
            // * note change var to player distance because each camera is in a different spot
            if ((playerDistanceFromRimX < -7 || playerDistanceFromRimX > 7)
                && !((playerDistanceFromRimX < -8 || playerDistanceFromRimX > 8)))
            {
                updatePositionNearGoal();
            }
            else
            {
                updatePositionOnPlayer();
            }
        }

        if ((player != null) && isOrthoGraphic && !isFollowBallCamera && !isLockOnGoalCamera)
        {
            transform.position = new Vector3(Mathf.Clamp(player.transform.position.x, xMin, xMax),
                //cam.transform.position.y,
                addToCameraPosY,
                cam.transform.position.z);
        }
        if ((player != null) && isFollowBallCamera && !isLockOnGoalCamera)
        {
            transform.position = new Vector3(BasketBall.instance.transform.position.x,
                 BasketBall.instance.transform.position.y + 0.5f,
                 BasketBall.instance.transform.position.z - 2);
        }
    }

    public void toggleCameraOnGoal()
    {
        onGoalCameraEnabled = !onGoalCameraEnabled;
        if (onGoalCameraEnabled)
        {
            CameraManager.instance.Cameras[CameraManager.instance.CameraOnGoalIndex].SetActive(true);
        }
        if (!onGoalCameraEnabled)
        {
            CameraManager.instance.Cameras[CameraManager.instance.CameraOnGoalIndex].SetActive(false);
        }
    }

    private void updatePositionOnPlayer()
    {
        Vector3 targetPosition = new Vector3(player.transform.position.x + cameraOffset, player.transform.position.y + addToCameraPosY, cam.transform.position.z);
        if (smoothCameraMotion)
        {
            Vector3 desiredPosition = targetPosition;
            Vector3 smoothedPosition = Vector3.Lerp(gameObject.transform.position, targetPosition, smoothSpeed * Time.deltaTime);
            transform.position = smoothedPosition;
        }
        else
        {
            transform.position = targetPosition;
        }
    }

    private void updatePositionNearGoal()
    {
        Vector3 targetPosition = new Vector3(cam.transform.position.x, player.transform.position.y + addToCameraPosY, cam.transform.position.z);
        Vector3 desiredPosition = targetPosition;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }

    //private void zoomOut()
    //{
    //    if (ZoomAmount == -20)
    //    {
    //        cameraZoomedOut = true;
    //    }
    //    else
    //    {
    //        cameraZoomedOut = false;
    //    }

    //    //Debug.Log("zoom out camera");
    //    ZoomAmount -= .5f;
    //    //Debug.Log("zoomAmount : " + ZoomAmount + "Input.GetAxis(mouse_axis_2) : " + Input.GetAxis("mouse_axis_2"));
    //    ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
    //    var translate = Mathf.Min(Mathf.Abs(-1), MaxToClamp - Mathf.Abs(ZoomAmount));
    //    gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(-1));
    //}


    //private void zoomIn()
    //{
    //    if (ZoomAmount == 0.5f)
    //    {
    //        cameraZoomedOut = false;
    //    }
    //    //Debug.Log("zoom in camera");
    //    ZoomAmount += .5f;
    //    //Debug.Log("zoomAmount : " + ZoomAmount + "Input.GetAxis(mouse_axis_2) : " + Input.GetAxis("mouse_axis_2"));
    //    ZoomAmount = Mathf.Clamp(ZoomAmount, -MaxToClamp, MaxToClamp);
    //    var translate = Mathf.Min(Mathf.Abs(1), MaxToClamp - Mathf.Abs(ZoomAmount));
    //    gameObject.transform.Translate(0, 0, translate * ROTSpeed * Mathf.Sign(1));
    //}

    void setCamera()
    {

        // if perspective camera
        if (!isOrthoGraphic)
        {
            addToCameraPosY = 1.835f;
            mainPerspectiveCamActive = true;
            //orthoCam1Active = false;
            //orthoCam2Active = false;
        }

        // 2 orthographic cameras
        if (isOrthoGraphic)
        {
            if (cam.name.Contains("camera_orthographic_1"))
            {
                addToCameraPosY = 2.5f;
                mainPerspectiveCamActive = false;
                //orthoCam1Active = true;
            }
            if (cam.name.Contains("camera_orthographic_2"))
            {
                addToCameraPosY = 3.3f;
                mainPerspectiveCamActive = false;
                //orthoCam1Active = false;
                //orthoCam2Active = true;
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
        if (cam.name.Contains("goal"))
        {
            isLockOnGoalCamera = true;
            transform.position = basketBallRim + lockOnGoalCameraOffset;
        }
        else
        {
            isLockOnGoalCamera = false;
        }
    }
}