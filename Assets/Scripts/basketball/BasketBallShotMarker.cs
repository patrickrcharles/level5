using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BasketBallShotMarker : MonoBehaviour
{
    [SerializeField]
    private int positionMarkerId;
    [SerializeField]
    private int _shotMade;
    [SerializeField]
    private int _shotAttempt;
    [SerializeField]
    private int maxShotAttempt;
    [SerializeField]
    private int maxShotMade;
    private bool _playerOnMarker;
    public bool PlayerOnMarker => _playerOnMarker;

    private BasketBallState basketBallState;

    [SerializeField]
    private bool shotTypeThree;
    [SerializeField]
    private bool shotTypeFour;

    // text stuff
    private Text displayCurrentMarkerStats;
    private const string displayStatsTextObject = "shot_marker_stats";
    [SerializeField]
    private SpriteRenderer spriteRenderer;
    [SerializeField]
    private bool markerEnabled;

    // Start is called before the first frame update
    void Start()
    {
        // get reference for accessing basketball state
        basketBallState = BasketBall.instance.BasketBallState;
        displayCurrentMarkerStats = GameObject.Find(displayStatsTextObject).GetComponent<Text>();

        spriteRenderer = GetComponent<SpriteRenderer>();

        displayCurrentMarkerStats.text = " current marker : " + positionMarkerId + "\n"
                                         + " made : " + ShotMade + " / " + ShotAttempt + "\n"
                                         + " remaining : " + (maxShotMade - ShotMade);
        markerEnabled = true;
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
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("on trigger : " + other.gameObject.tag + "  this : "+ gameObject.tag);
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker"))
        {
            _playerOnMarker = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("playerHitbox") && gameObject.CompareTag("shot_marker"))
        {
            _playerOnMarker = false;
            displayCurrentMarkerStats.text = " current marker : \n"
                                             + " made : \n"
                                             + " remaining : ";
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
}
