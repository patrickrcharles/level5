﻿using UnityEngine;
using UnityEngine.UI;

public class BasketBallShotMarker : MonoBehaviour
{
    [SerializeField]
    private bool _playerOnMarker;
    [SerializeField]
    private bool _autoPlayerOnMarker;
    private bool markerEnabled; // flag used to indicate max shots have not been achieved

    private GameObject basketBallTarget;
    private SpriteRenderer spriteRenderer;
    private BasketBallState basketBallState;

    [SerializeField] public int positionMarkerId; // identitfy specific marker
    // spcific marker's stats
    [SerializeField] private int _shotMade;
    [SerializeField] private int _shotAttempt;
    [SerializeField] private int maxShotAttempt;
    [SerializeField] private int maxShotMade;

    // flags used to idenify marker
    // true value determines whether or not marker is active in Gamerules.cs, aprox. line 250
    [SerializeField] public bool shotTypeThree;
    [SerializeField] public bool shotTypeFour;
    [SerializeField] public bool shotTypeSeven;

    private bool detectCollisions;
    private float distanceFromRim;

    // text stuff todo: move to game rules
    private Text displayCurrentMarkerStats;
    private const string displayStatsTextObject = "shot_marker_stats";

    public bool locked = false;

    // Start is called before the first frame update
    void Start()
    {
        _shotMade = 0;
        _shotAttempt = 0;

        // get reference for accessing basketball state
        basketBallState = GameLevelManager.instance.players[0].basketBallState;
        displayCurrentMarkerStats = GameObject.Find(displayStatsTextObject).GetComponent<Text>();
        displayCurrentMarkerStats.text = "";

        // used to control opacity of marker image 
        // todo: maybe just disable object. might require more work than it's worth
        spriteRenderer = GetComponent<SpriteRenderer>();

        // initial text display
        setDisplayText();
        // set what type of shot marker is based on distance from rim
        // using basketball state
        setMarkerShotType();
        //test flag
        //GameOptions.gameModeRequiresShotMarkers4s = true;
        if (GameOptions.gameModeRequiresShotMarkers3s || GameOptions.gameModeRequiresShotMarkers4s || GameOptions.gameModeRequiresShotMarkers7s)
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

        // failsafe check. data is serialzed and can be set manually but automatic is better. trust the code
        //if (GameRules.instance.GameModeThreePointContest
        //    || GameRules.instance.GameModeFourPointContest
        //    || GameRules.instance.GameModeSevenPointContest
        //    || GameRules.instance.GameModeAllPointContest)
        //{
        //    maxShotAttempt = 5;
        //}

        // if script disabled, disable collisions flag.
        // collisions/colliders still detected if script disabled
        detectCollisions = this.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        // if time's up
        if (Time.timeScale <= 0)
        {
            displayCurrentMarkerStats.text = "";
        }
        // this needs to be turned off if ball hits ground
        if (PlayerOnMarker /*|| _autoPlayerOnMarker && GameOptions.numPlayers >= 1*/)
        {
            // if marker not completed yet
            if (markerEnabled)
            {
                setDisplayText();
            }
        }
        // if game mode is 3/4/all point contest
        if (GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeSevenPointContest
            || GameRules.instance.GameModeAllPointContest)
        {
            // max shot attempts reached
            // player NOT in air, player does NOT have ball, ball ! in air
            if (ShotAttempt >= maxShotAttempt & markerEnabled
                && !GameLevelManager.instance.players[0].playerController.hasBasketball
                && !GameLevelManager.instance.players[0].playerController.InAir
                && !basketBallState.InAir)
            {
                markerEnabled = false;
                // decrease markers remaining
                GameRules.instance.MarkersRemaining--;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // opacity to 0
                setDisplayText();

                //check if last remaining shot marker
                if (GameRules.instance.IsGameOver())
                {
                    //GameRules.instance.CounterTime = Timer.instance.CurrentTime;
                    GameRules.instance.GameOver = true;
                }
            }
        }
        // game mode is NOT 3/4/All point contest
        if (!GameRules.instance.GameModeThreePointContest
            || !GameRules.instance.GameModeFourPointContest
            || !GameRules.instance.GameModeSevenPointContest
            || !GameRules.instance.GameModeAllPointContest)
        {
            // if made # of shots required at shot marker
            if (ShotMade >= MaxShotMade && markerEnabled)
            {
                markerEnabled = false;
                // decrease markers remaining
                GameRules.instance.MarkersRemaining--;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // opacity to 0
                setDisplayText();

                // check if last remaining shot marker
                if (GameRules.instance.IsGameOver())
                {
                    //GameRules.instance.CounterTime = Timer.instance.CurrentTime;
                    GameRules.instance.GameOver = true;
                }
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        // if player enters shot marker area
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker")
            && detectCollisions)
        {
            _playerOnMarker = true;
            other.GetComponentInParent<PlayerIdentifier>().basketball.GetComponent<BasketBallState>().CurrentShotMarkerId = positionMarkerId;
        }
        // if player enters shot marker area
        if (other.gameObject.CompareTag("autoPlayerHitbox") && gameObject.CompareTag("shot_marker")
            && detectCollisions)
        {
            _autoPlayerOnMarker = true;
            other.GetComponentInParent<PlayerIdentifier>().autoBasketball.GetComponent<BasketBallState>().CurrentShotMarkerId = positionMarkerId;
        }
    }

    void OnTriggerExit(Collider other)
    {
        // if player exits shot marker area
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker")
                && detectCollisions)
        {
            _playerOnMarker = false;
            setDisplayText(); // update display to empty
        }
        // if player exits shot marker area
        if (other.gameObject.CompareTag("autoPlayerHitbox") && gameObject.CompareTag("shot_marker")
                && detectCollisions)
        {
            _autoPlayerOnMarker = false;
            locked = false;
            setDisplayText(); // update display to empty
        }
    }

    private void setDisplayText()
    {
        // if player on marker and markers necessary for game mode and IS 3,4,All point contest
        if ((PlayerOnMarker || _autoPlayerOnMarker) && markerEnabled
            && (GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeSevenPointContest
            || GameRules.instance.GameModeAllPointContest))
        {
            displayCurrentMarkerStats.text = "total points : " + BasketBall.instance.GameStats.TotalPoints + "\n"
                                             // + "current marker : " + positionMarkerId + "\n"
                                             + "made : " + ShotMade + " / " + ShotAttempt + "\n"
                                             + "remaining : " + (maxShotAttempt - ShotAttempt);
        }
        // if player on marker and markers necessary for game mode and NOT 3,4,All point contest
        if ((PlayerOnMarker || _autoPlayerOnMarker) && markerEnabled
            && !(GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeSevenPointContest
            || GameRules.instance.GameModeAllPointContest))
        {
            displayCurrentMarkerStats.text = "markers remaining : " + GameRules.instance.MarkersRemaining + "\n"
                                             // + "current marker : " + positionMarkerId + "\n"
                                             + "made : " + ShotMade + " / " + ShotAttempt + "\n"
                                             + "remaining : " + (maxShotMade - ShotMade);
        }
        // if player not on marker or marker disabled (max shots made)
        if (!(PlayerOnMarker || _autoPlayerOnMarker) || !markerEnabled)//&& markerEnabled)
        {
            displayCurrentMarkerStats.text = "markers remaining : " + GameRules.instance.MarkersRemaining + "\n"
                                             //   + "current marker : \n"
                                             + "made : \n"
                                             + "remaining : ";
        }
    }

    // the shot type is set manually but this is a failsafe check that sets it automatically based 
    // on distance from the rim
    void setMarkerShotType()
    {
        // get distance from rim
        //basketBallTarget = basketBallState.BasketBallTarget;
        basketBallTarget = GameObject.Find("basketBall_target");
        distanceFromRim = Vector3.Distance(transform.position, basketBallTarget.transform.position);

        if (distanceFromRim > Constants.DISTANCE_3point)
        {
            shotTypeThree = true;
            shotTypeFour = false;
            shotTypeSeven = false;
        }

        if (distanceFromRim > Constants.DISTANCE_4point)
        {
            shotTypeThree = false;
            shotTypeFour = true;
            shotTypeSeven = false;
        }

        if (distanceFromRim > Constants.DISTANCE_7point)
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

    public int PositionMarkerId
    {
        get => positionMarkerId;
        set => positionMarkerId = value;
    }
    public int MaxShotMade => maxShotMade;
    public bool PlayerOnMarker => _playerOnMarker;
    public bool ShotTypeThree => shotTypeThree;
    public bool ShotTypeFour => shotTypeFour;
    public bool ShotTypeSeven => shotTypeSeven;
    public bool MarkerEnabled { get => markerEnabled; set => markerEnabled = value; }
    public bool AutoPlayerOnMarker { get => _autoPlayerOnMarker; set => _autoPlayerOnMarker = value; }
    public int MaxShotAttempt { get => maxShotAttempt; set => maxShotAttempt = value; }
}


