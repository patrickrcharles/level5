using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallSpriteFunctions : MonoBehaviour
{
    private AudioSource audioSource;

   void Start()
   {
       audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
   }

    public void playSFfxBasketballDribbling()
    {
        //Debug.Log("play bounce sound");
        audioSource.PlayOneShot(SFXBB.Instance.basketballBounce);
    }
}
