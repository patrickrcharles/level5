using System.Collections;
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
        audioSource.PlayOneShot(SFXBB.instance.basketballBounce);
    }

    public void playSfxAlienWalking()
    {
        audioSource.PlayOneShot(SFXBB.instance.alien_walk);
    }

    public void playSfxGameChanger()
    {
        audioSource.PlayOneShot(SFXBB.instance.gamechanger);
    }

    public void playSfxCameraFlash()
    {
        audioSource.PlayOneShot(SFXBB.instance.cameraFlash);
    }

    public void playSfxWerewolfHowl()
    {
        audioSource.PlayOneShot(SFXBB.instance.werewolfHowl);
    }

    public void playSfxWorkerParasite()
    {
        audioSource.PlayOneShot(SFXBB.instance.worker_parasite);
    }

    public void playSfxAirHorn() 
    {
        audioSource.PlayOneShot(SFXBB.instance.airhorn);
    }
    public void playSfxLightningStrike()
    {
        audioSource.PlayOneShot(SFXBB.instance.lightningStrike);
    }

    public void playSfxRimShot()
    {
        audioSource.PlayOneShot(SFXBB.instance.rimShot);
    }
    public void playSfxKnockedDown()
    {
        audioSource.PlayOneShot(SFXBB.instance.knockedDown);
    }

    public void playSfxSkateGrind()
    {
        audioSource.PlayOneShot(SFXBB.instance.skateGrind);
    }

    public void playSfxGlitch()
    {
        audioSource.PlayOneShot(SFXBB.instance.glitch);
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
