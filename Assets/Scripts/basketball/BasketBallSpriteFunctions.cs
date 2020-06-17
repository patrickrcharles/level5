using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketBallSpriteFunctions : MonoBehaviour
{
    private AudioSource audioSource;
    const string  attackBoxText = "attack_box";

    [SerializeField]
    GameObject attackBox;

    private void Awake()
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

        if(attackBox != null)
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

    public void enableAttackBox()
    {
        attackBox.SetActive(true);
    }

    public void disableAttackBox()
    {
        attackBox.SetActive(false);
    }
}
