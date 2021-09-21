using System.Collections;
using UnityEngine;

public class PlayerAnimationEvents : MonoBehaviour
{
    [SerializeField]
    private AudioSource audioSource;
    const string attackBoxText = "attackBox";
    const string attackBoxSpecialText = "attackBoxSpecial";
    const string hitboxBoxText = "hitbox";

    [SerializeField]
    GameObject attackBox;
    [SerializeField]
    GameObject attackBoxSpecial;
    [SerializeField]
    GameObject hitBox;
    [SerializeField]
    GameObject projectileLaserPrefab;
    [SerializeField]
    GameObject projectileBulletPrefab;
    [SerializeField]
    GameObject projectileAutomaticBulletPrefab;
    [SerializeField]
    GameObject projectileMolotovPrefab;
    [SerializeField]
    GameObject projectileRocketPrefab;
    [SerializeField]
    GameObject projectileCigarettePrefab;
    [SerializeField]
    GameObject projectileSpawn;
    [SerializeField]
    CapsuleCollider capsuleCollider;

    bool attackBoxEnabled;
    bool attackBoxSpecialEnabled;

    Animator animOnCamera;
    PlayerController playerController;

    private bool hitBoxEnabled;

    private void Start()
    {
        if (transform.Find("projectileSpawn") != null)
        {
            projectileLaserPrefab = Resources.Load("Prefabs/projectile/projectile_laser_player") as GameObject;
            projectileBulletPrefab = Resources.Load("Prefabs/projectile/projectile_bullet_player") as GameObject;
            projectileAutomaticBulletPrefab = Resources.Load("Prefabs/projectile/projectile_automatic_bullet") as GameObject;
            projectileMolotovPrefab = Resources.Load("Prefabs/projectile/projectile_molotov") as GameObject;
            projectileRocketPrefab = Resources.Load("Prefabs/projectile/projectile_rocket") as GameObject;
            projectileCigarettePrefab = Resources.Load("Prefabs/projectile/projectile_cigarette") as GameObject;
            projectileSpawn = transform.Find("projectileSpawn").gameObject;
        }
        if (transform.root.GetComponent<CapsuleCollider>() != null)
        {
            capsuleCollider = transform.root.GetComponent<CapsuleCollider>();
        }

        playerController = GameLevelManager.instance.PlayerController;
        audioSource = GetComponent<AudioSource>();

        if (transform.Find(attackBoxText) != null)
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

        InvokeRepeating("checkCollidersDisabledProperly", 0, 1);
    }

    // function - Invoke Repeating
    private void checkCollidersDisabledProperly()
    {
        if (playerController.CurrentState != playerController.AttackState
            && playerController.CurrentState != playerController.SpecialState
            && playerController.CurrentState != playerController.dunkState
            && attackBoxEnabled)
        {
            disableAttackBox();
        }
        if (playerController.CurrentState != playerController.BlockState && hitBoxEnabled)
        {
            disableHitBox();
        }
        if (playerController.CurrentState != playerController.SpecialState
            && playerController.CurrentState != playerController.AttackState
            && attackBoxSpecialEnabled)
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
    public void instantiateProjectileBulletAuto()
    {
        Instantiate(projectileAutomaticBulletPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }

    public void instantiateProjectileAutomaticBullet(int numOfBullets)
    {
        StartCoroutine(ShootAutomaticWeapon(numOfBullets));
        //Instantiate(projectileBulletPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }

    IEnumerator ShootAutomaticWeapon(int numBullets)
    {
        for (int i = 0; i < numBullets; i++)
        {
            instantiateProjectileBulletAuto();
            yield return new WaitForSeconds(0.1f);
        }
    }

    public void instantiateProjectileMolotov()
    {
        Instantiate(projectileMolotovPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }
    public void instantiateProjectileCigarette()
    {
        Instantiate(projectileCigarettePrefab, projectileSpawn.transform.position, Quaternion.identity);
    }
    public void instantiateProjectileRocket()
    {
        Instantiate(projectileRocketPrefab, projectileSpawn.transform.position, Quaternion.identity);
    }


    public void applyForceToDirectionFacingXAndY(float force)
    {
        // get direction facing
        if (playerController.FacingRight)
        {
            //apply to X
            playerController.RigidBody.AddForce(force, force, 0, ForceMode.VelocityChange);
        }
        if (!playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(-force, force, 0, ForceMode.VelocityChange);
        }
        // apply for in x direction

    }

    public void applyForceToDirectionFacingProjectile(float force)
    {
        if (playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(force, 0, 0, ForceMode.VelocityChange);
        }
        if (!playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(-force, 0, 0, ForceMode.VelocityChange);
        }
    }

    public void applyForceToDirectionFacing()
    {
        // get direction facing
        if (playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(2.5f, 1.5f, 0, ForceMode.VelocityChange);
        }
        if (!playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(-2.5f, 1.5f, 0, ForceMode.VelocityChange);
        }
    }
    public void applyForceToXDirectionFacing(float Xforce)
    {
        // get direction facing
        if (playerController.FacingRight)
        {
            //apply to X
            playerController.RigidBody.AddForce(Xforce, 0, 0, ForceMode.VelocityChange);
        }
        if (!playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(-Xforce, 0, 0, ForceMode.VelocityChange);
        }
        // apply for in x direction
    }

    public void applyForceToXDirectionNotFacing(float Xforce)
    {
        Debug.Log("force : " + Xforce);
        // get direction facing
        if (playerController.FacingRight)
        {
            //apply to X
            playerController.RigidBody.AddForce(-Xforce, 2, 0, ForceMode.VelocityChange);
        }
        if (!playerController.FacingRight)
        {
            playerController.RigidBody.AddForce(Xforce, 2, 0, ForceMode.VelocityChange);
        }
        // apply for in x direction
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
        GameLevelManager.instance.Player.GetComponent<Rigidbody>().isKinematic = true;
    }

    public void disableRigidBodyIsKinematic()
    {
        GameLevelManager.instance.Player.GetComponent<Rigidbody>().isKinematic = false;
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

    void CheckAttackBoxActiveStatus()
    {
        if (!GameLevelManager.instance.PlayerController.IsSpecialState()
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
