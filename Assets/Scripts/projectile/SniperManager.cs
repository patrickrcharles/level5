using Assets.Scripts.Utility;
using System.Collections;
using UnityEngine;

public class SniperManager : MonoBehaviour
{
    private GameObject playerHitbox;
    private AudioSource audioSource;
    private Vector3 playerPosAtShoot;
    private PlayerController playerController;

    [SerializeField]
    GameObject projectileLaserPrefab;
    [SerializeField]
    GameObject projectileBulletPrefab;
    [SerializeField]
    float bulletDelay;

    bool locked = false;

    public static SniperManager instance;

    private void Awake()
    {
        StartCoroutine(LoadVariables());
    }
    private void Start()
    {
        //GameOptions.sniperEnabled = true; //test flag
        //// auto start autonomous sniper system
        if (GameOptions.sniperEnabled)
        {
            instance = this;
            InvokeRepeating("startSniper", 0, 0.5f);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator LoadVariables()
    {
        yield return new WaitUntil(() => GameLevelManager.instance.Player1 != null);
        playerController = GameLevelManager.instance.PlayerController1;
        playerHitbox = GameLevelManager.instance.Player1.transform.Find("hitbox").gameObject;
        audioSource = GetComponent<AudioSource>();
    }

    void startSniper()
    {
        if (!locked && playerHitbox != null)
        {
            locked = true;
            float random = UtilityFunctions.GetRandomFloat(0, 4);

            //// test flag to enable
            GameOptions.sniperEnabledBullet = true;
            if (GameOptions.sniperEnabledBullet && GameOptions.sniperEnabled)
            {
                StartCoroutine(StartSniperBullet(random));
            }
            if (GameOptions.sniperEnabledLaser && GameOptions.sniperEnabled)
            {
                StartCoroutine(StartSniperLaser(random));
            }
        }
    }

    IEnumerator StartSniperBullet(float shootdelay)
    {
        // wait until player is not knocked down
        yield return new WaitUntil( ()=> playerController.currentState != playerController.knockedDownState);
        // add shoot delay
        yield return new WaitForSeconds(shootdelay);
        // update stats
        BasketBall.instance.GameStats.SniperShots++;

        // get player position to attack
        PlayerPosAtShoot = playerHitbox.transform.position;
        //PlayerPosAtShoot = GameLevelManager.instance.Player.transform.Find("hitbox").gameObject.transform.position;
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
        //audioSource.PlayOneShot(SFXBB.instance.deathRay);
        StartCoroutine(InstantiateBullet());
    }

    IEnumerator StartSniperLaser(float shootdelay)
    {
        yield return new WaitForSeconds(shootdelay);

        BasketBall.instance.GameStats.SniperShots++;

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
        audioSource.PlayOneShot(SFXBB.instance.shootGun);
        //audioSource.PlayOneShot(SFXBB.instance.deathRay);
        StartCoroutine(InstantiateLaser());
    }

    IEnumerator InstantiateBullet()
    {
        yield return new WaitForSeconds(bulletDelay);
        // instantiate bullet
        Instantiate(projectileBulletPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }

    IEnumerator InstantiateLaser()
    {
        yield return new WaitForSeconds(bulletDelay);
        // instantiate laser
        Instantiate(projectileLaserPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }
    public Vector3 PlayerPosAtShoot { get => playerPosAtShoot; set => playerPosAtShoot = value; }
}
