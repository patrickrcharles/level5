using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    private AudioSource audioSource;
    const string attackBoxText = "attackBox";
    const string attackBoxSpecialText = "attackBoxSpecial";
    const string hitboxBoxText = "hitbox";

    GameObject attackBox;
    GameObject attackBoxSpecial;
    GameObject hitBox;
    [SerializeField]
    GameObject projectileLaserPrefab;
    GameObject projectileBulletPrefab;
    GameObject projectileMolotovPrefab;
    [SerializeField]
    GameObject projectileSpawn;

    bool attackBoxEnabled;
    bool attackBoxSpecialEnabled;

    Animator animOnCamera;
    PlayerController playerController;

    private bool hitBoxEnabled;

    private void Start()
    {
        if (transform.Find("projectileSpawn") != null)
        {
            projectileLaserPrefab = Resources.Load("Prefabs/projectile/projectile_laser") as GameObject;
            projectileBulletPrefab = Resources.Load("Prefabs/projectile/projectile_bullet_magnum") as GameObject;
            projectileMolotovPrefab = Resources.Load("Prefabs/projectile/projectile_molotov") as GameObject;
            projectileSpawn = transform.Find("projectileSpawn").gameObject;
        }

        playerController = GameLevelManager.instance.PlayerState;
        if (GameLevelManager.instance.Basketball != null)
        {
            audioSource = GameObject.FindWithTag("basketball").GetComponent<AudioSource>();
        }
        if (transform.Find(attackBoxText)!= null)
        {
            attackBox = transform.Find(attackBoxText).gameObject;        
            disableAttackBox();
        }
        else
        {
            attackBox = null;
        }
        if (transform.Find(attackBoxSpecialText) != null)
        {
            attackBoxSpecial = transform.Find(attackBoxSpecialText).gameObject;
            disableAttackBoxSpecial();
        }
        else
        {
            attackBoxSpecial = null;
        }
        if (gameObject.transform.parent.Find(hitboxBoxText) != null)
        {
            hitBox = gameObject.transform.parent.Find(hitboxBoxText).gameObject;
        }
        else
        {
            hitBox = null;
        }
        if (GameObject.Find("camera_flash").GetComponent<Animator>() != null)
        {
            animOnCamera = GameObject.Find("camera_flash").GetComponent<Animator>();
        }
        else
        {
            animOnCamera = null;
        }



        // check if attack box is active and should not be

        //InvokeRepeating("CheckAttackBoxActiveStatus", 0, 3);
    }
    private void Update()
    {
        if (playerController.CurrentState != playerController.AttackState && attackBoxEnabled)
        {
            disableAttackBox();
        }
        if (playerController.CurrentState != playerController.BlockState && hitBoxEnabled)
        {
            disableHitBox();
        }
        if (playerController.CurrentState != playerController.SpecialState && attackBoxSpecialEnabled)
        {
            disableAttackBoxSpecial();
        }
    }

    public void instantiateProjectileLazer()
    {
        Instantiate(projectileLaserPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }
    public void instantiateProjectileBullet()
    {
        Instantiate(projectileBulletPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }
    public void instantiateProjectileMolotov()
    {
        Instantiate(projectileMolotovPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }

    public void applyForceToDirectionFacingKickFlip(float force)
    {
        // get direction facing
        if (playerController.facingRight)
        {
            //apply to X
            playerController.RigidBody.AddForce(force, force, 0, ForceMode.VelocityChange);
        }
        if (!playerController.facingRight)
        {
            playerController.RigidBody.AddForce(-force, force, 0, ForceMode.VelocityChange);
        }
        // apply for in x direction

    }

    public void applyForceToDirectionFacingProjectile(float force)
    {
        // get direction facing
        if (playerController.facingRight)
        {
            //apply to X
            playerController.RigidBody.AddForce(force, 0, 0, ForceMode.VelocityChange);
        }
        if (!playerController.facingRight)
        {
            playerController.RigidBody.AddForce(-force, 0, 0, ForceMode.VelocityChange);
        }
        // apply for in x direction
    }

    public void applyForceToDirectionFacing()
    {
        // get direction facing
        if (playerController.facingRight)
        {
            //apply to X
            playerController.RigidBody.AddForce(2.5f, 1.5f, 0, ForceMode.VelocityChange);
        }
        if(!playerController.facingRight)
        {
            playerController.RigidBody.AddForce(-2.5f, 1.5f, 0, ForceMode.VelocityChange);
        }
        // apply for in x direction
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
        attackBoxEnabled = true;
    }

    public void disableAttackBox()
    {
        attackBox.SetActive(false);
        attackBoxEnabled = false;
    }
    public void enableAttackBoxSpecial()
    {
        attackBoxSpecial.SetActive(true);
        attackBoxSpecialEnabled = true;
    }

    public void disableAttackBoxSpecial()
    {
        attackBoxSpecial.SetActive(false);
        attackBoxSpecialEnabled = false;
    }

    public void enableHitBox()
    {
        hitBox.SetActive(true);
        hitBoxEnabled = true;
    }

    public void disableHitBox()
    {
        hitBox.SetActive(false);
        hitBoxEnabled = false;
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
            Debug.Log(" game obejct : " + gameObject.tag + "  other : " + other.tag);
            gameObject.GetComponent<Animator>().SetTrigger("hit");
        }

        if (gameObject.transform.parent.name.Contains("mega_robot") && other.gameObject.CompareTag("playerHitbox"))
        {
            //Debug.Log(" game obejct : " + gameObject.name + "  other : " + other.tag);
            gameObject.GetComponent<Animator>().SetTrigger("attack");
        }
    }

    void CheckAttackBoxActiveStatus()
    {
        if (!GameLevelManager.instance.PlayerState.IsSpecialState()
            && attackBox.activeSelf)
        {
            attackBox.SetActive(false);
        }
    }
    //private void playAnimationCameraFlash()
    //{
    //    animOnCamera.Play("camera_flash");
    //}
}
