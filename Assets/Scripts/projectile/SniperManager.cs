using Assets.Scripts.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SniperManager : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController;
    [SerializeField]
    GameObject player;
    [SerializeField]
    GameObject projectileLaserPrefab;
    [SerializeField]
    GameObject projectileBulletPrefab;
    private AudioSource audioSource;
    [SerializeField]
    float bulletDelay;

    Vector3 playerPosAtShoot;
    bool locked = false;

    public static SniperManager instance;

    private void Awake()
    {
        StartCoroutine(LoadVariables());
    }
    private void Start()
    {
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
        yield return new WaitUntil(() => GameLevelManager.instance.Player != null);

        player = GameLevelManager.instance.Player.transform.Find("hitbox").gameObject;
        GameOptions.sniperEnabled = true;
        audioSource = GetComponent<AudioSource>();
    }

    void startSniper()
    {
        if (!locked && player != null)
        {
            locked = true;
            float random = UtilityFunctions.GetRandomFloat(0, 4);
            StartCoroutine( ShootSniper(random) );
        }
    }


    IEnumerator ShootSniper(float shootdelay)
    {
        //Debug.Log("startSniper()");
        yield return new WaitForSeconds(shootdelay);
        //Debug.Log("startSniper() after yield");

        BasketBall.instance.BasketBallStats.SniperShots += 1;

        // get player position to attack
        PlayerPosAtShoot = player.transform.position;
        // edit prefab
        EnemyProjectile enemyProjectile = projectileBulletPrefab.GetComponentInChildren<EnemyProjectile>();
        //EnemyProjectile enemyProjectile = projectileLaserPrefab.GetComponentInChildren<EnemyProjectile>();
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

    IEnumerator InstantiateBullet()
    {
        yield return new WaitForSeconds(bulletDelay);
        // instantiate bullet
        //Instantiate(projectileLaserPrefab, gameObject.transform.position, Quaternion.identity);
        Instantiate(projectileBulletPrefab, gameObject.transform.position, Quaternion.identity);
        locked = false;
    }
    public Vector3 PlayerPosAtShoot { get => playerPosAtShoot; set => playerPosAtShoot = value; }
}
