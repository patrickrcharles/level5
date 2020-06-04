using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketBallShotMarker : MonoBehaviour
{
    //* note if var starts with underscore, it will have a publicly accessible property at bottom of file
    // get/set. sometimes get only

    // main state bool
    [SerializeField]
    private bool _playerOnMarker;
    private bool markerEnabled; // flag used to indicate max shots have not been achieved

    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private BasketBallState basketBallState;
    private GameObject basketBallTarget;

    [SerializeField] private int positionMarkerId;

    public int PositionMarkerId
    {
        get => positionMarkerId;
        set => positionMarkerId = value;
    }

    [SerializeField] private int _shotMade;
    [SerializeField] private int _shotAttempt;
    [SerializeField] private int maxShotAttempt;
    [SerializeField] private int maxShotMade;

    public int MaxShotMade => maxShotMade;

    [SerializeField] private bool shotTypeThree;
    [SerializeField] private bool shotTypeFour;
    [SerializeField] private bool shotTypeSeven;

    private bool detectCollisions;

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
        displayCurrentMarkerStats.text = "";

        spriteRenderer = GetComponent<SpriteRenderer>();

        setDisplayText();
        // set what type of shot marker is based on distance from rim
        // using basketball state
        setMarkerShotType();

        if (GameOptions.gameModeRequiresShotMarkers3s || GameOptions.gameModeRequiresShotMarkers4s)
        {
            markerEnabled = true;
            setDisplayText();
            // set what type of shot marker is based on distance from rim
            // using basketball state
            setMarkerShotType();
        }
        else // marker is not needed
        {
            // disable text and disable script
            displayCurrentMarkerStats.text = "";
            this.enabled = false;
        }
        // if script disabled, disable collisions flag.
        // collisions/colliders still detected if script disabled
        detectCollisions = this.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            displayCurrentMarkerStats.text = "";
        }
        // this needs to be turned off if ball hits ground
        if (PlayerOnMarker)
        {
            BasketBall.instance.BasketBallState.CurrentShotMarkerId = positionMarkerId;
            // if marker not completed yet
            if (markerEnabled)
            {
                setDisplayText();
            }
        }

        // if made # of shots required at shot marker
        if (_shotMade >= maxShotMade && markerEnabled)
        {
            markerEnabled = false;
            // decrease markers remaining
            GameRules.instance.MarkersRemaining--;
            spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // opacity to 0
            setDisplayText();

            // check if last remaining shot marker
            if (GameRules.instance.isGameOver())
            {
                //GameRules.instance.CounterTime = Timer.instance.CurrentTime;
                GameRules.instance.GameOver = true;
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("ontrigger : this.enabled" + this.enabled);

        //Debug.Log("on trigger : " + other.gameObject.tag + "  this : "+ gameObject.tag + " detect collsions : "+ detectCollisions);
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker")
            && detectCollisions)
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
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker")
                && detectCollisions)
        {
           _playerOnMarker = false;
            setDisplayText();
        }
        //_playerOnMarker = false;
    }

    private void setDisplayText()
    {
        if (PlayerOnMarker && markerEnabled)
        {
            displayCurrentMarkerStats.text = "markers remaining : " + GameRules.instance.MarkersRemaining + "\n"
                                            // + "current marker : " + positionMarkerId + "\n"
                                             + "made : " + ShotMade + " / " + ShotAttempt + "\n"
                                             + "remaining : " + (maxShotMade - ShotMade);
        }
        // if player not on marker or marker disabled (max shots made)
        if (!PlayerOnMarker || !markerEnabled)//&& markerEnabled)
        {
            displayCurrentMarkerStats.text = "markers remaining : " + GameRules.instance.MarkersRemaining + "\n"
                                          //   + "current marker : \n"
                                             + "made : \n"
                                             + "remaining : ";
        }
        //else
        //{
        //    displayCurrentMarkerStats.text = "markers remaining : " + basketBallState.MarkersRemaining + "\n"
        //                                     + "current marker : " + positionMarkerId + "\n"
        //                                     + "made : " + ShotMade + " / " + ShotAttempt + "\n"
        //                                     + "remaining : " + (maxShotMade - ShotMade);
        //}
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

    public bool ShotTypeThree => shotTypeThree;

    public bool ShotTypeFour => shotTypeFour;

    public bool ShotTypeSeven => shotTypeSeven;
}


