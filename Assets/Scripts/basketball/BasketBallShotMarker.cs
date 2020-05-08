using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketBallShotMarker : MonoBehaviour
{
    //* note if var starts with underscore, it will have a publicly accessible property at bottom of file
    // get/set. sometimes get only

    // main state bool
    private bool _playerOnMarker;
    private bool markerEnabled; // flag used to indicate max shots have not been achieved

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BasketBallState basketBallState;
    private GameObject basketBallTarget;

    [SerializeField] private int positionMarkerId;
    [SerializeField] private int _shotMade;
    [SerializeField] private int _shotAttempt;
    [SerializeField] private int maxShotAttempt;
    [SerializeField] private int maxShotMade;

    [SerializeField] private bool shotTypeThree;
    [SerializeField] private bool shotTypeFour;
    [SerializeField] private bool shotTypeSeven;

    private float distanceFromRim;

    // text stuff todo: move to game rules
    private Text displayCurrentMarkerStats;
    private const string displayStatsTextObject = "shot_marker_stats";

    // Start is called before the first frame update
    void Start()
    {
        // get reference for accessing basketball state
        basketBallState = GameLevelManager.Instance.Basketball.BasketBallState;
        displayCurrentMarkerStats = GameObject.Find(displayStatsTextObject).GetComponent<Text>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        displayCurrentMarkerStats.text = " current marker : " + positionMarkerId + "\n"
                                         + " made : " + ShotMade + " / " + ShotAttempt + "\n"
                                         + " remaining : " + (maxShotMade - ShotMade);
        markerEnabled = true;

        // set what type of shot marker is based on distance from rim
        // using basketball state
        setMarkerShotType();
    }

    // Update is called once per frame
    void Update()
    {
        // this needs to be turned off if ball hits ground
        if (PlayerOnMarker)
        {
            BasketBall.instance.BasketBallState.CurrentShotMarkerId = positionMarkerId;

            if (markerEnabled)
            {
                displayCurrentMarkerStats.text = " current marker : " + positionMarkerId + "\n"
                                                 + " made : " + ShotMade + " / " + ShotAttempt + "\n"
                                                 + " remaining : " + (maxShotMade - ShotMade);
            }
        }

        if (_shotMade >= maxShotMade && markerEnabled)
        {
            markerEnabled = false;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // is about 100 % transparent
            displayCurrentMarkerStats.text = " current marker : \n"
                                             + " made : \n"
                                             + " remaining : ";
        }
    }

    void OnTriggerStay(Collider other)
    {
        //Debug.Log("on trigger : " + other.gameObject.tag + "  this : "+ gameObject.tag);
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker"))
        {
            _playerOnMarker = true;
        }

        //else
        //{
        //    _playerOnMarker = false;
        //}
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker"))
        {
            _playerOnMarker = false;
            displayCurrentMarkerStats.text = " current marker : \n"
                                             + " made : \n"
                                             + " remaining : ";
        }

        _playerOnMarker = false;
    }

    void setMarkerShotType()
    {
        // get distance from rim
        basketBallTarget = basketBallState.BasketBallTarget;
        distanceFromRim = Vector3.Distance(transform.position, basketBallTarget.transform.position);

        if (distanceFromRim > basketBallState.ThreePointDistance)
        {
            shotTypeThree = true;
            shotTypeFour = false;
            shotTypeSeven = false;
        }

        if (distanceFromRim > basketBallState.FourPointDistance)
        {
            shotTypeThree = false;
            shotTypeFour = true;
            shotTypeSeven = false;
        }

        if (distanceFromRim > basketBallState.SevenPointDistance)
        {
            shotTypeThree = false;
            shotTypeFour = false;
            shotTypeSeven = true;
        }

    }

    public int ShotMade
    {
        get => _shotMade;
        set => _shotMade = value;
    }

    public int ShotAttempt
    {
        get => _shotAttempt;
        set => _shotAttempt = value;
    }

    public bool PlayerOnMarker => _playerOnMarker;
}


