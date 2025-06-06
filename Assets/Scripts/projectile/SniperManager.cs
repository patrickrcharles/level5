using Assets.Scripts.Utility;
using System.Collections;
using UnityEngine;

public class SniperManager : MonoBehaviour
{
    [SerializeField]
    private GameObject playerHitbox;
    [SerializeField]
    private AudioSource audioSource;
    private Vector3 playerPosAtShoot;
    [SerializeField]
    private PlayerController playerController;

    [SerializeField]
    GameObject projectileLaserPrefab;
    [SerializeField]
    GameObject projectileBulletPrefab;    
    [SerializeField]
    GameObject projectileAutomaticBulletPrefab;
    [SerializeField]
    GameObject projectileBulletInstantKillPrefab;
    [SerializeField]
    float bulletDelay;

    public bool locked = false;

    public static SniperManager instance;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        StartCoroutine(LoadVariables());
        //GameOptions.sniperEnabled = true; 
        //test flag
        // auto start autonomous sniper system
        if (GameOptions.sniperEnabled || GameOptions.sniperEnabledLaser || GameOptions.sniperEnabledBullet)
        {
            instance = this;
            InvokeRepeating("startSniper", 0, 0.5f);
        }
        //else
        //{
        //    gameObject.SetActive(false);
        //}
    }

    IEnumerator LoadVariables()
    {
        yield return new WaitUntil(() => GameLevelManager.instance.players[0] != null);
        yield return new WaitUntil(() => GameLevelManager.instance.players[0].playerController != null);
        playerController = GameLevelManager.instance.players[0].playerController;
        playerHitbox = GameLevelManager.instance.players[0].transform.Find("hitbox").gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    void startSniper()
    {
        if (!locked && playerHitbox != null)
        {
            locked = true;
            float random = UtilityFunctions.GetRandomFloat(0, 4);

            //// test flag to enable
            //GameOptions.sniperEnabledBullet = true;
            if (GameOptions.sniperEnabledBullet && !GameLevelManager.instance.Player1.playerController.PlayerHealth.IsDead)
            {
                StartCoroutine(StartSniperBullet(random));
            }
            if (GameOptions.sniperEnabledLaser && !GameLevelManager.instance.Player1.playerController.PlayerHealth.IsDead)
            {
                StartCoroutine(StartSniperLaser(random));
            }
            if (GameOptions.sniperEnabledBulletAuto && !GameLevelManager.instance.Player1.playerController.PlayerHealth.IsDead)
            {
                StartCoroutine(StartSniperBulletAuto(random));
            }
        }
    }

    public IEnumerator StartSniperBullet(float shootdelay)
    {
        // wait until player is not knocked down
        yield return new WaitUntil( ()=> playerController.currentState != playerController.knockedDownState);
        // add shoot delay
        yield return new WaitForSeconds(shootdelay);
        // update stats
        GameLevelManager.instance.players[0].gameStats.SniperShots++;

        // get player position to attack
        PlayerPosAtShoot = playerHitbox.transform.position;
        // edit prefab
        EnemyProjectile enemyProjectile = projectileBulletPrefab.GetComponentInChildren<EnemyProjectile>();
        enemyProjectile.sniperProjectile = true;
        enemyProjectile.impactProjectile = true;

        // get vector to player
        Vector3 direction = PlayerPosAtShoot - (gameObject.transform.position);
        // set vector bullet direction
        enemyProjectile.projectileForceSniper = direction;
        //play sound
        audioSource.PlayOneShot(SFXBB.instance.shootGun);
        InstantiateInstantBullet();
    }

    public IEnumerator StartSniperBulletAuto(float shootdelay)
    {
        // wait until player is not knocked down
        yield return new WaitUntil(() => playerController.currentState != playerController.knockedDownState);
        // add shoot delay
        yield return new WaitForSeconds(shootdelay);

        // get player position to attack
        PlayerPosAtShoot = playerHitbox.transform.position;
        //PlayerPosAtShoot = GameLevelManager.instance.Player.transform.Find("hitbox").gameObject.transform.position;
        // edit prefab
        EnemyProjectile enemyProjectile = projectileAutomaticBulletPrefab.GetComponentInChildren<EnemyProjectile>();
        enemyProjectile.sniperProjectile = true;
        enemyProjectile.impactProjectile = true;

        // get vector to player
        Vector3 direction = PlayerPosAtShoot - (gameObject.transform.position);
        // set vector bullet direction
        enemyProjectile.projectileForceSniper = direction;
        //play sound
        audioSource.PlayOneShot(SFXBB.instance.shootAutomaticAK47);
        StartCoroutine(InstantiateProjectileAutomaticBullet(enemyProjectile, 10));
    }
    IEnumerator StartSniperLaser(float shootdelay)
    {
        yield return new WaitForSeconds(shootdelay);

        GameLevelManager.instance.players[0].gameStats.SniperShots++;

        // get player position to attack
        PlayerPosAtShoot = playerHitbox.transform.position;
        // edit prefab
        EnemyProjectile enemyProjectile = projectileLaserPrefab.GetComponentInChildren<EnemyProjectile>();
        enemyProjectile.sniperProjectile = true;
        enemyProjectile.impactProjectile = true;

        // get vector to player
        Vector3 direction = PlayerPosAtShoot - (gameObject.transform.position);
        // set vector bullet direction
        enemyProjectile.projectileForceSniper = direction;
        //play sound
        audioSource.PlayOneShot(SFXBB.instance.deathRay);
        StartCoroutine(InstantiateLaser());
    }
    public IEnumerator StartSniperBulletInstantKill(float shootdelay)
    {
        yield return new WaitForSeconds(shootdelay);

        GameLevelManager.instance.players[0].gameStats.SniperShots++;

        // get player position to attack
        PlayerPosAtShoot = playerHitbox.transform.position;
        // edit prefab
        EnemyProjectile enemyProjectile = projectileBulletInstantKillPrefab.GetComponentInChildren<EnemyProjectile>();
        enemyProjectile.sniperProjectile = true;
        enemyProjectile.impactProjectile = true;

        // get vector to player
        Vector3 direction = PlayerPosAtShoot - (gameObject.transform.position);
        // set vector bullet direction
        enemyProjectile.projectileForceSniper = direction;
        //play sound
        audioSource.PlayOneShot(SFXBB.instance.shootGun);
        StartCoroutine(InstantiateBulletInstantKill());
    }
    void InstantiateInstantBullet()
    {
        //yield return new WaitForSeconds(bulletDelay);
        // instantiate bullet
        Instantiate(projectileBulletPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }

    IEnumerator InstantiateBullet()
    {
        yield return new WaitForSeconds(bulletDelay);
        // instantiate bullet
        Instantiate(projectileBulletPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }
    IEnumerator InstantiateProjectileAutomaticBullet(EnemyProjectile enemyProjectile,int numBullets)
    {

        yield return new WaitForSeconds(bulletDelay);
        for (int i = 0; i < numBullets; i++)
        {
            instantiateProjectileBulletAuto(enemyProjectile);
            yield return new WaitForSeconds(0.2f);
        }
        locked = false;
    }

    public void instantiateProjectileBulletAuto(EnemyProjectile enemyProjectile)
    {
        float random = UtilityFunctions.GetRandomFloat(-0.35f, 0.35f);
        Vector3 target = new Vector3(enemyProjectile.projectileForceSniper.x + random, enemyProjectile.projectileForceSniper.y, enemyProjectile.projectileForceSniper.z);
        enemyProjectile.projectileForceSniper = target;
        Instantiate(projectileAutomaticBulletPrefab, gameObject.transform.position, Quaternion.identity);
        // update stats
        GameLevelManager.instance.players[0].gameStats.SniperShots++;
    }

    IEnumerator InstantiateLaser()
    {
        yield return new WaitForSeconds(bulletDelay);
        // instantiate laser
        Instantiate(projectileLaserPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }
    IEnumerator InstantiateBulletInstantKill()
    {
        yield return new WaitForSeconds(bulletDelay);
        // instantiate laser
        Instantiate(projectileBulletInstantKillPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }
    public Vector3 PlayerPosAtShoot { get => playerPosAtShoot; set => playerPosAtShoot = value; }
}
