using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAttackBoxes : MonoBehaviour {

    public GameObject basketball;
    public GameObject playerHitbox;

    void Start()
    {
        basketball = GameObject.FindGameObjectWithTag("basketball");
    }

    public void playBasketballBounceSound()
    {
        //Debug.Log("    void playBasketballBounceSound()");
        AudioSource.PlayClipAtPoint(SFXBB.Instance.basketballBounce, basketball.transform.position, 1);
    }

}
