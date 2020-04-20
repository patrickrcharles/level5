//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class playerAttacks : MonoBehaviour {


//    // components 
//    Animator anim;
//    AnimatorStateInfo currentStateInfo;
//    Stats stats;
//    GameObject dropShadow;
//    AudioSource moonwalkAudio;
//    [SerializeField]
//    SpriteRenderer spriteRenderer;
//    private Rigidbody rigidBody;
//    uiInventory uiInventory;
//    playercontrollerscript player;
//    gameManager gameManager;
//    float weaponCoolDown;
//    playerWeapons weapons;

//    public bool notLocked;

//    // attack boxes
//    public GameObject attack1Box, attack2Box, attackElbowBox,
//        attackFlyingElbowBox, attackKickBox, attackShoveBox,
//        counterAttackBox, sweatThisBox, deathRayBox, flamethrowerHitbox;
//    //hitbox (take damage)
//    public GameObject playerHitbox;
//    // spawn psotions for projectiles
//    public Transform projectileSpawnPoint, rocketSpawnPoint, throwSpawnPoint;
//    // projectiles
//    public GameObject projectile, rocket, molotov;

//    // Use this for initialization
//    public void  Start () {

//        // sets player limits from level manager. eventually remove by setting up
//        // collider boundaries
//        //setPlayerBounds(); // can remove once physical colliders finished
//        notLocked = true; // default needs to be true
        
//        gameManager = transform.root.GetComponent<gameManager>();
//        player = gameManager.playerState;
//        anim = gameManager.anim;

//        projectile = Resources.Load("Prefabs/player_bullet") as GameObject;
//        rocket = Resources.Load("Prefabs/player_rocket") as GameObject;
//        molotov = Resources.Load("Prefabs/pickup_items/molotov") as GameObject;

//        stats = GetComponent<Stats>();
//        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
//        uiInventory = GetComponent<uiInventory>();
//        playerHitbox.SetActive(true);
//        player.canAttack = true;
//        rigidBody = GetComponent<Rigidbody>();
//        moonwalkAudio = GetComponent<AudioSource>();
//        anim = GetComponentInChildren<Animator>();

//    }
	
//	// Update is called once per frame
//	public void  Update () {
		


//	}


//    // ================= START FUNCTION/ IENUMERATORS  ============================================================================
//    IEnumerator Smoking()
//    {
//        //Debug.Log("IEnumerator Smoking()");
//        yield return new WaitForSeconds(.3f);
//        //gameObject.GetComponent<Stats>().health -= Time.deltaTime * player.chargeSpeed;
//        Debug.Log("player stats 1 :: " + player.getStats().health);
//        player.getStats().health -= Time.deltaTime * player.chargeSpeed;
//        Debug.Log("player stats 2 :: " + player.getStats().health);
//        player.smokingEnabled = false;
//    }

//    //prototyping pretty much done. need to move attacks, throws, special moves, etc to separate "playerAttacks" class
//    public void  Attack1()
//    {
//        anim.SetBool("attack1", true);
//    }
//    public void  Attack2()
//    {
//        anim.SetBool("attack2", true);
//    }
//    public void  Attack3()
//    {
//        anim.SetBool("attack1", false);
//        anim.SetBool("attack2", false);
//        anim.SetBool("elbow", true);
//    }
//    public void  Attack3Dropkick()
//    {
//        anim.SetBool("attack1", false);
//        anim.SetBool("attack2", false);
//        anim.SetBool("kick", false);
//        anim.SetBool("dropkick", true);
//    }
//    public void  Attack3Cranekick()
//    {
//        anim.SetBool("attack1", false);
//        anim.SetBool("attack2", false);
//        anim.SetBool("kick", false);
//        anim.SetBool("cranekick", true);
//    }
//    public void  Attack4()
//    {
//        anim.SetBool("kick", true);
//    }
//    public void  Attack5Melee()
//    {
//        Debug.Log("Attack5Melee()");
//        if (player.currentMelee == "rightPunch")
//        {
//            anim.SetBool("attack2", true);
//        }
//        if (player.currentMelee == "bat")
//        {
//            anim.Play("swingBat");
//        }
//        if (player.currentMelee == "spikedBat")
//        {
//            anim.Play("swingSpikedBat");
//        }
//        if (player.currentMelee == "whip")
//        {
//            //todo: investigate weird bug where sfx plays too many times
//            anim.Play("swingWhip");
//            if (player.soundPlayed)
//            {
//                player.soundPlayed = false;
//                AudioSource.PlayClipAtPoint(SFX.Instance.whipCrack, this.transform.position);
//                StartCoroutine(Wait(playerAnimations.Instance.swingWhip.length));
//            }
//        }
//        else
//        {
//            Debug.Log("else ::::: notLocked = true;");
//            notLocked = true;
//        }
//    }


//    // -------------------------Knocked down - shockwave attack
//    IEnumerator KnockedDownShockwaveAttack()
//    {
//        anim.Play("groundShockWave");
//        yield return new WaitForSeconds(playerAnimations.Instance.groundShockwave.length);
//        player.canMove = true;
//        player.knockedDown = false;
//        player.canGroundAttack = false;
//    }


//    //--------------- throw molotov ------------------------------------
//    IEnumerator throwMolotov(float animTime)
//    {
//        anim.SetBool("molotov", true);
//        yield return new WaitForSecondsRealtime(animTime);
//        anim.SetBool("molotov", false);
//        Instantiate(molotov, throwSpawnPoint.position, throwSpawnPoint.rotation);
//        weapons.gunAmmoCapacity -= 1;
//        uiInventory.currentWeaponAmmo.text = "x " + weapons.gunAmmoCapacity.ToString();
//        projectile.GetComponent<projectile>().shooter = "Player";

//        Debug.Log("current weapon = " + player.currentWeapon);
//        Debug.Log("current ammo = " + weapons.gunAmmoCapacity);
//    }

//    //todo: remove all the convoluted weapon stuff and start implementing system of only
//    //using available weapons instead of all. need to implement inventory finally
//    public void  projectileWeapon()
//    {
//        Debug.Log("public void  projectileWeapon()");
//        // check ammo and decrease on fire
//        if (weapons.gunAmmoCapacity > 0 && notLocked)
//        {
//            anim.SetBool("shoot", true);
//            anim.SetBool("walking", false);

//            // cooldown is length of weapon's animation
//            StartCoroutine(WeaponCooldown(weaponCoolDown));

//            if ((weapons.currentWeapon == "magnum" || currentWeapon == "peacemaker") && notLocked)
//            {
//                ShootProjectile();
//                AudioSource.PlayClipAtPoint(SFX.Instance.magnum, this.transform.position);
//            }
//            if ((currentWeapon == "shotgun") && notLocked)
//            {
//                ShootProjectile();
//                AudioSource.PlayClipAtPoint(SFX.Instance.magnum, this.transform.position);
//            }
//            if ((currentWeapon == "ak47") && notLocked)
//            {
//                StartCoroutine(ShootBurstFireProjectile(burstFireRate));
//                AudioSource.PlayClipAtPoint(SFX.Instance.ak47, this.transform.position);
//            }
//            if ((currentWeapon == "deathRay") && notLocked)
//            {
//                //ShootProjectile();
//                weapons.gunAmmoCapacity -= 1;
//                uiInventory.currentWeaponAmmo.text = "x " + weapons.gunAmmoCapacity.ToString();
//                AudioSource.PlayClipAtPoint(SFX.Instance.deathRay, this.transform.position);
//            }
//            if ((currentWeapon == "freezeRay") && notLocked)
//            {
//                //ShootProjectile();
//                weapons.gunAmmoCapacity -= 1;
//                uiInventory.currentWeaponAmmo.text = "x " + weapons.gunAmmoCapacity.ToString();
//                AudioSource.PlayClipAtPoint(SFX.Instance.deathRay, this.transform.position);
//            }
//            if ((currentWeapon == "uzi") && notLocked)
//            {

//                StartCoroutine(ShootBurstFireProjectile(burstFireRate));
//                AudioSource.PlayClipAtPoint(SFX.Instance.ak47, this.transform.position);
//            }
//            if ((currentWeapon == "rocketLauncher") && notLocked)
//            {
//                ShootProjectile();
//                AudioSource.PlayClipAtPoint(SFX.Instance.flamethrower, this.transform.position);
//            }
//        }
//    }

//    public void  ShootProjectile()
//    {
//        Debug.Log("public void  ShootProjectile()");
//        //StartCoroutine(WeaponCooldown(weaponCoolDown));
//        notLocked = false;
//        if (currentWeapon != "rocketLauncher")
//        {
//            Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
//            Debug.Log("Instantiate(" + projectile + ", " + projectileSpawnPoint.position + ", " + projectileSpawnPoint.rotation + ");");
//        }
//        else
//        {
//            Instantiate(rocket, rocketSpawnPoint.position, rocketSpawnPoint.rotation);
//            AudioSource.PlayClipAtPoint(SFX.Instance.rocketLauncher, this.transform.position);
//            Debug.Log("Instantiate(" + rocket + ", " + rocketSpawnPoint.position + ", " + rocketSpawnPoint.rotation + ");");
//        }
//        weapons.gunAmmoCapacity -= 1;
//        uiInventory.currentWeaponAmmo.text = "x " + weapons.gunAmmoCapacity.ToString();
//        projectile.GetComponent<projectile>().shooter = "Player";
//    }

//    // -------------------------shoot burst fire -----------------
//    IEnumerator ShootBurstFireProjectile(float numProjectiles)
//    {
//        //Debug.Log("public void  ShootBurstFireProjectile()");
//        //Debug.Log("ShootBurstFireProjectile : " + currentWeapon + " ammo  = " + gunAmmoCapacity);

//        notLocked = false;
//        for (int i = 0; i < numProjectiles; i++)
//        {
//            Debug.Log("for (int i = 0; i < numProjectiles; i++) : " + currentWeapon + " ammo  = " + gunAmmoCapacity);

//            if (gunAmmoCapacity > 0 && anim.GetBool("shoot"))
//            {
//                Instantiate(projectile, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
//                if (weapons.gunAmmoCapacity > 0)
//                {
//                    weapons.gunAmmoCapacity -= 1;
//                }
//                uiInventory.currentWeaponAmmo.text = "x " + weapons.gunAmmoCapacity.ToString();
//                Debug.Log("if (gunAmmoCapacity > 0) : " + currentWeapon + " ammo  = " + gunAmmoCapacity);
//                projectile.GetComponent<projectile>().shooter = "Player";
//                Debug.Log("shooter = " + projectile.GetComponent<projectile>().shooter);
//                yield return new WaitForSeconds(burstFireWait);
//            }
//        }
//    }


//    // ------------------------ Throw Smokebomb -----------------------------------------------------------------------
//    IEnumerator SmokeBomb()
//    {
//        // disable hitbox first
//        //CURRENTLY DISABLED BY ANIMATION
//        //playerHitbox.SetActive(false);

//        int randomX = Random.Range(-3, 3);
//        int randomZ = Random.Range(-3, 3);
//        bool canSmokeBomb = boundaryCheck(randomX, randomZ);

//        if (canSmokeBomb)
//        {
//            anim.Play("smokeBomb");
//            yield return new WaitForSecondsRealtime(playerAnimations.Instance.smokeBomb.length);
//            transform.position = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);
//            anim.Play("smokeBombSpawn");
//            //playerHitbox.SetActive(true);
//        }
//        else
//        {
//            StartCoroutine(SmokeBomb());
//        }
//    }
//    // -------------- boundary check for Throw smokebomb ---------------------------------------------------------
//    bool boundaryCheck(int x, int z)
//    {
//        if ((transform.position.x + x) < levelManager.instance.xMaxPlayer
//            && (transform.position.x + x) > levelManager.instance.xMinPlayer
//            && (transform.position.z + z) < levelManager.instance.zMaxPlayer
//            && (transform.position.z + z) > levelManager.instance.zMinPlayer)
//        {
//            //Debug.Log("pos + x : " + (transform.position.x+x) + "  pos + z : " + (transform.position.z + z));
//            return true;
//        }
//        else
//            return false;
//    }


//    //todo: this can be renamed and used as a generic damage per second function
//    IEnumerator FlameThrowerDamage()
//    {
//        yield return new WaitForSeconds(0.2f);
//        weapons.gunAmmoCapacity -= 1;
//        notLocked = true;
//    }

//    //--------------------------weapon cooldown -----------------------------------
//    IEnumerator WeaponCooldown(float seconds)
//    {
//        yield return new WaitForSecondsRealtime(seconds);
//        anim.SetBool("shoot", false);
//        //canShoot = true;
//        //re-enable movement
//        notLocked = true;

//        Debug.Log("     anim.SetBool(shoot, true); = " + anim.GetBool("shoot"));
//    }

//    // -----------------generic wait coroutine ----------------------------
//    IEnumerator Wait(float seconds)
//    {
//        Debug.Log("1 - Wait() : " + Time.time);
//        yield return new WaitForSecondsRealtime(seconds);
//        soundPlayed = true;
//        notLocked = true;
//        Debug.Log("2 - Wait() : " + Time.time);
//    }
//}
