using System.Collections;
using UnityEngine;

public class RacingAnimationEvents : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
  

    Animator animOnCamera;


    private void Start()
    {
      
        //if (GameObject.Find("camera_flash").GetComponent<Animator>() != null)
        //{
        //    animOnCamera = GameObject.Find("camera_flash").GetComponent<Animator>();
        //}
        //else
        //{
        //    animOnCamera = null;
        //}
        // check if attack box is active and should not be

        //InvokeRepeating("checkCollidersDisabledProperly", 0, 1);
    }

    // function - Invoke Repeating





    //public void enableCapsuleCollider()
    //{
    //    capsuleCollider.enabled = true;
    //    //capsuleColliderEnabled = true;
    //}

    //public void disableCapsuleCollider()
    //{
    //    capsuleCollider.enabled = false;
    //    //capsuleColliderEnabled = false;
    //}

    public void enableRigidBodyIsKinematic()
    {
        GameLevelManager.instance.Player1.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void disableRigidBodyIsKinematic()
    {
        GameLevelManager.instance.Player1.GetComponent<Rigidbody>().isKinematic = false;
    }

    public void playSfxBasketballHitRim()
    {
        audioSource.PlayOneShot(SFXBB.instance.basketballHitRim);
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
    public void playSfxTakeDamage()
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

    public void playSfxCloudOfSmoke()
    {
        audioSource.PlayOneShot(SFXBB.instance.turnIntoBat);
    }

    public void playSfxAirGuitar()
    {
        audioSource.PlayOneShot(SFXBB.instance.airGuitar);
    }

    public void playSfxChainRattle()
    {
        audioSource.PlayOneShot(SFXBB.instance.chainRattle);
    }

    public void playSfxDeathRay()
    {
        audioSource.PlayOneShot(SFXBB.instance.deathRay);
    }

    public void playSfxProbeDroidCritical()
    {
        audioSource.PlayOneShot(SFXBB.instance.probeCritical);
    }
    public void playSfxVampireHiss()
    {
        audioSource.PlayOneShot(SFXBB.instance.vampireHiss);
    }
    public void playSfxHitMetalBang()
    {
        audioSource.PlayOneShot(SFXBB.instance.metalBang);
    }

    public void playSfxStoneCold()
    {
        audioSource.PlayOneShot(SFXBB.instance.stoneCold);
    }

    public void playSfxChopWood()
    {
        audioSource.PlayOneShot(SFXBB.instance.chopWood);
    }

    public void playSfxShootGun()
    {
        audioSource.PlayOneShot(SFXBB.instance.shootGun);
    }
    public void playSfxShotgunRack()
    {
        audioSource.PlayOneShot(SFXBB.instance.shotgunRack);
    }
    public void playSfxProjectileRocket()
    {
        audioSource.PlayOneShot(SFXBB.instance.projectileRocket);
    }
    public void playSfxWhipCrack()
    {
        audioSource.PlayOneShot(SFXBB.instance.whipCrack);
    }
    public void playSfxAK47()
    {
        audioSource.PlayOneShot(SFXBB.instance.shootAutomaticAK47);
    }


    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("hanger") && other.gameObject.CompareTag("basketball"))
        {
            //Debug.Log(" game obejct : " + gameObject.tag + "  other : " + other.tag);
            gameObject.GetComponent<Animator>().SetTrigger("hit");
        }

        if (gameObject.transform.parent.name.Contains("mega_robot") && other.gameObject.CompareTag("playerHitbox"))
        {
            //Debug.Log(" game obejct : " + gameObject.name + "  other : " + other.tag);
            gameObject.GetComponent<Animator>().SetTrigger("attack");
        }
    }

    //private void playAnimationCameraFlash()
    //{
    //    animOnCamera.Play("camera_flash");
    //}
}
