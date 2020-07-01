﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallSpriteFunctions : MonoBehaviour
{
    private AudioSource audioSource;
    const string  attackBoxText = "attack_box";
    const string  hitboxBoxText = "playerHitbox";

    [SerializeField]
    GameObject attackBox;

    [SerializeField]
    GameObject hitBox;

    private void Start()
    {
        audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
        // get attack box reference
        if (gameObject.transform.parent.Find(attackBoxText) != null)
        {
            attackBox = gameObject.transform.parent.Find(attackBoxText).gameObject;
        }
        else
        {
            attackBox = null;
        }
        // find hitbox
        if (gameObject.transform.parent.Find(hitboxBoxText) != null)
        {
            hitBox = gameObject.transform.parent.Find(attackBoxText).gameObject;
        }
        else
        {
            hitBox = null;
        }

        if (attackBox != null)
        {
            disableAttackBox();
        }

    }

    public void playSfxBasketballDribbling()
    {
        audioSource.PlayOneShot(SFXBB.Instance.basketballBounce);
    }

    public void playSfxAlienWalking()
    {
        audioSource.PlayOneShot(SFXBB.Instance.alien_walk);
    }

    public void playSfxGameChanger()
    {
        audioSource.PlayOneShot(SFXBB.Instance.gamechanger);
    }

    public void playSfxCameraFlash()
    {
        audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
    }

    public void playSfxWerewolfHowl()
    {
        audioSource.PlayOneShot(SFXBB.Instance.werewolfHowl);
    }

    public void playSfxWorkerParasite()
    {
        audioSource.PlayOneShot(SFXBB.Instance.worker_parasite);
    }

    public void playSfxAirHorn() 
    {
        audioSource.PlayOneShot(SFXBB.Instance.airhorn);
    }
    public void playSfxLightningStrike()
    {
        audioSource.PlayOneShot(SFXBB.Instance.lightningStrike);
    }

    public void playSfxRimShot()
    {
        audioSource.PlayOneShot(SFXBB.Instance.rimShot);
    }
    public void playSfxKnockedDown()
    {
        audioSource.PlayOneShot(SFXBB.Instance.knockedDown);
    }

    public void playSfxSkateGrind()
    {
        audioSource.PlayOneShot(SFXBB.Instance.skateGrind);
    }

    public void playSfxGlitch()
    {
        audioSource.PlayOneShot(SFXBB.Instance.glitch);
    }

    public void enableAttackBox()
    {
        attackBox.SetActive(true);
    }

    public void disableAttackBox()
    {
        attackBox.SetActive(false);
    }

    public void enableHitBox()
    {
        hitBox.SetActive(true);
    }

    public void disableHitBox()
    {
        hitBox.SetActive(false);
    }
}
