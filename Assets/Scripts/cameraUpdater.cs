
 using UnityEngine;
 using System.Collections;
 using UnityEngine.Experimental.PlayerLoop;

 public class cameraUpdater : MonoBehaviour
{

    public Transform player;

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

    private bool camera128px;
    private bool camera192px;

    private float addToCameraPosY;



    void Start()
    {

        Debug.Log(("camera updater Start()"));
        basketBallRim = GameObject.Find("rim");

        cam = GetComponent<Camera>();
        cam.depth = -5;

        // this is for the sorting layers. when using perspective camera like i am,
        // sometimes the rendering isnt always done by z values because perspective 
        // uses a value that closest to center of the camera or something
        // this should finally fix all the rendering problems i've been having

        cam.transparencySortMode = TransparencySortMode.Orthographic;

        player = GameLevelManager.instance.player.transform;
        //relCameraPos = player.position - transform.position;

        if (cam.name.Contains("192"))
        {
            camera128px = false;
            camera192px = true;
            addToCameraPosY = 1.835f;
        }
        else
        {
            camera128px = true;
            camera192px = false;
            addToCameraPosY = 1.2f;
        }

    }
    

    void Update()
    {
        //Debug.Log(" zoom amount: " + ZoomAmount);

        playerPos = new Vector3(player.position.x,
            0, player.position.z);
        //camPos = new Vector3(cam.transform.position.x,
        //    0, cam.transform.root.position.z);

        rimPos = new Vector3(basketBallRim.transform.position.x,
            0, basketBallRim.transform.root.position.z);

        //distanceCamFromPlayer = Vector3.Distance(playerPos, camPos);
        distanceRimFromPlayer = rimPos.z - playerPos.z;

        //if ((player != null))
        //{
        //    transform.position = new Vector3(Mathf.Clamp(player.position.x, xMin, xMax),
        //                                   //cam.transform.position.y,
        //                                   player.transform.position.y + 1.2f,
        //                                    cam.transform.position.z);
        //}

        //if (distanceRimFromPlayer > startZoomDistance 
        //    && !cameraZoomedOut)
        //    //&& cam.transform.position.z > zMin)
        //{
        //    zoomOut();
        //}
        //if (distanceRimFromPlayer < startZoomDistance && cameraZoomedOut)
        //{
        //    zoomIn();
        //}

    }

    void FixedUpdate()
    {
        if ((player != null))
        {
            transform.position = new Vector3(Mathf.Clamp(player.position.x, xMin, xMax),
                //cam.transform.position.y,
                player.transform.position.y + addToCameraPosY,
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
    }

    private void zoomOut()
    {
        if( ZoomAmount == -20)
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
        if(ZoomAmount == 0.5f)
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
}