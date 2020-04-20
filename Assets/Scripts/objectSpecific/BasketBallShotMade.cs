using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallShotMade : MonoBehaviour {

    [SerializeField]
    basketBall basketBall;
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
    void Start () {

        basketBall = GameObject.Find("basketball").GetComponent<basketBall>();
        audioSource = GetComponent<AudioSource>();
        anim =  rimSprite.GetComponent<Animator>();
        playerState = gameManager.instance.playerState;
    }
	
	// Update is called once per frame
	void Update ()
    {
        isColliding = false;
	}

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("========================== BasketBall: " + transform.name + " and " + other.gameObject.name);
        if (other.gameObject.name == "basketball" && !playerState.hasBasketball && !isColliding  )
        {
            if (isColliding) return;
            else { isColliding = true; } 

            audioSource.PlayOneShot(SFXBB.Instance.basketballNetSwish);
            if(basketBall.lastShotDistance > basketBall.longestShot)
            {
                basketBall.longestShot = basketBall.lastShotDistance;
            }
            anim.Play("madeshot");
            Debug.Log(" made a shot!");
            Debug.Log("two: "+ basketBall.TwoAttempt 
                + " three: " + basketBall.ThreeAttempt 
                + " four: " + basketBall.FourAttempt  );
            /* trigger net swish animation + sfx
             *  update score
             */

            if (basketBall.TwoAttempt)
            {
                Debug.Log("2 pointer made");
                basketBall.totalPoints += 2;
                basketBall.TwoPointerMade++;
                basketBall.addToShotMade(1);

            }
            if (basketBall.ThreeAttempt)
            {
                Debug.Log("3 pointer made");
                basketBall.totalPoints += 3;
                basketBall.ThreePointerMade++;
                basketBall.addToShotMade(1);
            }
            if (basketBall.FourAttempt)
            {
                Debug.Log("4 pointer made");
                basketBall.totalPoints += 4;
                basketBall.FourPointerMade++;
                basketBall.addToShotMade(1);
            }
            basketBall.TwoAttempt = false;
            basketBall.ThreeAttempt = false;
            basketBall.FourAttempt = false;
            basketBall.updateScoreText();
        }
    }
}
