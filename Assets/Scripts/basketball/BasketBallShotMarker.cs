﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class BasketBallShotMarker : MonoBehaviour
{
    //* note if var starts with underscore, it will have a publicly accessible property at bottom of file
    // get/set. sometimes get only

    // main state bool
    [SerializeField]
    private bool _playerOnMarker;
    private bool markerEnabled; // flag used to indicate max shots have not been achieved

    private GameObject basketBallTarget;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private BasketBallState basketBallState;

    [SerializeField] private int positionMarkerId; // identitfy specific marker
    // spcific marker's stats
    [SerializeField] private int _shotMade;
    [SerializeField] private int _shotAttempt;
    [SerializeField] private int maxShotAttempt;
    [SerializeField] private int maxShotMade;

    // flags used to idenify marker
    // true value determines whether or not marker is active in Gamerules.cs, aprox. line 250
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

        // used to control opacity of marker image 
        // todo: maybe just disable object. might require more work than it's worth
        spriteRenderer = GetComponent<SpriteRenderer>();

        // initial text display
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

        // failsafe check. data is serialzed and can be set manually but automatic is better. trust the code
        if (GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeAllPointContest)
        {
            maxShotAttempt = 5;
        }

        // if script disabled, disable collisions flag.
        // collisions/colliders still detected if script disabled
        detectCollisions = this.enabled;
    }

    // Update is called once per frame
    void Update()
    {
        // in theory, disables all update checking unless required by game mode

        // if time's up
        if (Time.timeScale <= 0)
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

        // if game mode IS 3,4, all point contest
        if (GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeAllPointContest)
        {
            // max shot attempst reached
            // player NOT in air, player does NOT have ball, ball ! in air
            if (ShotAttempt >= maxShotAttempt & markerEnabled
                && !GameLevelManager.Instance.PlayerState.hasBasketball
                && !GameLevelManager.Instance.PlayerState.inAir
                && !basketBallState.InAir)
            {
                markerEnabled = false;
                // decrease markers remaining
                GameRules.instance.MarkersRemaining--;
                spriteRenderer.color = new Color(1f, 1f, 1f, 0f); // opacity to 0
                setDisplayText();

                //check if last remaining shot marker
                if (GameRules.instance.isGameOver())
                {
                    //GameRules.instance.CounterTime = Timer.instance.CurrentTime;
                    GameRules.instance.GameOver = true;
                }
            }
        }
        // game mode is NOT 3,4 , All point contest
        if (!GameRules.instance.GameModeThreePointContest
            || !GameRules.instance.GameModeFourPointContest
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
                if (GameRules.instance.isGameOver())
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
    }

    private void setDisplayText()
    {
        // if player on marker and markers necessary for game mode and IS 3,4,All point contest
        if (PlayerOnMarker && markerEnabled 
            && (GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeAllPointContest))
        {
            displayCurrentMarkerStats.text = "total points : " + BasketBall.instance.BasketBallStats.TotalPoints + "\n"
                                             // + "current marker : " + positionMarkerId + "\n"
                                             + "made : " + ShotMade + " / " + ShotAttempt + "\n"
                                             + "remaining : " + (maxShotAttempt - ShotAttempt);
        }
        // if player on marker and markers necessary for game mode and NOT 3,4,All point contest
        if (PlayerOnMarker && markerEnabled 
            && !(GameRules.instance.GameModeThreePointContest
            || GameRules.instance.GameModeFourPointContest
            || GameRules.instance.GameModeAllPointContest))
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
    }

    // the shot type is set manually but this is a failsafe check that sets it automatically based 
    // on distance from the rim
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
}


