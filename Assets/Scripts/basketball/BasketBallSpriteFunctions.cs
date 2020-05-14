﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallSpriteFunctions : MonoBehaviour
{
    private AudioSource audioSource;

   void Start()
   {
       audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
   }

    public void playSfxBasketballDribbling()
    {
//       //Debug.Log("play bounce sound");
        audioSource.PlayOneShot(SFXBB.Instance.basketballBounce);
    }

    public void playSfxAlienWalking()
    {
//       //Debug.Log("play bounce sound");
        audioSource.PlayOneShot(SFXBB.Instance.alien_walk);
    }

    public void playSfxGameChanger()
    {
        //       //Debug.Log("play bounce sound");
        audioSource.PlayOneShot(SFXBB.Instance.gamechanger);
    }

    public void playSfxCameraFlash()
    {
        //       //Debug.Log("play bounce sound");
        audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
    }
}
