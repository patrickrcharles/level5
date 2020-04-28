using UnityEngine;


public class BasketBallShotMade : MonoBehaviour
{
    BasketBall basketball;
    BasketBallState _basketBallState;
    private BasketBallStats basketBallStats;

    public AudioClip shotMade;
    //;
    AudioSource audioSource;
    //SpriteRenderer spriteRenderer;
    public GameObject rimSprite;
    Animator anim;
    playercontrollerscript playerState;
    bool isColliding;

    //public AudioClip shotMissed;

    // Use this for initialization
    void Start()
    {
        basketball = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
        _basketBallState = basketball.GetComponent<BasketBallState>();
        basketBallStats = basketball.GetComponent<BasketBallStats>();
        audioSource = GetComponent<AudioSource>();
        anim = rimSprite.GetComponent<Animator>();
        playerState = GameLevelManager.instance.playerState;
    }

    // Update is called once per frame
    void Update()
    {
        isColliding = false;
    }

    void OnTriggerEnter(Collider other)
    {
        //Debug.Log("========================== BasketBall: " + transform.name + " and " + other.gameObject.name)
        if (other.gameObject.CompareTag("basketball") && !playerState.hasBasketball && !isColliding)
        {
            if (isColliding) return;
            else { isColliding = true; }

            audioSource.PlayOneShot(SFXBB.Instance.basketballNetSwish);
            if (basketball.lastShotDistance > basketball.longestShot)
            {
                basketball.longestShot = basketball.lastShotDistance;
            }
            anim.Play("madeshot");

            Debug.Log(" made a shot!");
            Debug.Log("two: " + _basketBallState.TwoAttempt
                + " three: " + _basketBallState.ThreeAttempt
                + " four: " + _basketBallState.FourAttempt);
            /* trigger net swish animation + sfx
         *  update score
         */

            if (_basketBallState.TwoAttempt)
            {
                //Debug.Log("2 pointer made");
                basketBallStats.TotalPoints += 2;
                basketBallStats.TwoPointerMade++;
                basketBallStats.ShotMade++;

            }
            if (_basketBallState.ThreeAttempt)
            {
                //Debug.Log("3 pointer made");
                basketBallStats.TotalPoints += 3;
                basketBallStats.ThreePointerMade++;
                basketBallStats.ShotMade++;
            }
            if (_basketBallState.FourAttempt)
            {
                //Debug.Log("4 pointer made");
                basketBallStats.TotalPoints += 4;
                basketBallStats.FourPointerMade++;
                basketBallStats.ShotMade++;

            }
            _basketBallState.TwoAttempt = false;
            _basketBallState.ThreeAttempt = false;
            _basketBallState.FourAttempt = false;
        }
        // update onscreen ui stats
        basketball.updateScoreText();
    }
}

