using UnityEngine;

public class BasketBallSpriteFunctions : MonoBehaviour
{
    private AudioSource audioSource;
    const string attackBoxText = "attack_box";
    const string hitboxBoxText = "playerHitbox";

    [SerializeField]
    GameObject attackBox;

    [SerializeField]
    GameObject hitBox;

    private void Start()
    {
        if (GameLevelManager.instance.Basketball != null)
        {
            audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
        }

        // get attack box reference
        if (gameObject.transform.Find(attackBoxText) != null)
        {
            attackBox = gameObject.transform.Find(attackBoxText).gameObject;
            disableAttackBox();
        }
        else
        {
            attackBox = null;
        }
        // find hitbox
        if (gameObject.transform.parent.Find(hitboxBoxText) != null)
        {
            hitBox = gameObject.transform.Find(hitboxBoxText).gameObject;
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
    public void playSfxYell1()
    {
        audioSource.PlayOneShot(SFXBB.instance.yellYeah1, 0.3f);
    }
    public void playSfxYell2()
    {
        audioSource.PlayOneShot(SFXBB.instance.yellYeah2, 0.3f);
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

    public void enableRigidBodyIsKinematic()
    {
        GameLevelManager.instance.Player.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void disableRigidBodyIsKinematic()
    {
        GameLevelManager.instance.Player.GetComponent<Rigidbody>().isKinematic = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.CompareTag("hanger") && other.gameObject.CompareTag("basketball"))
        {
            //Debug.Log(" game obejct : " + gameObject.tag + "  other : " + other.tag);
            gameObject.GetComponentInChildren<Animator>().SetTrigger("hit");
        }

        if (gameObject.transform.parent.name.Contains("mega_robot") && other.gameObject.CompareTag(hitboxBoxText))
        {
            //Debug.Log(" game obejct : " + gameObject.name + "  other : " + other.tag);
            gameObject.GetComponent<Animator>().SetTrigger("attack");
        }
    }

}
