using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour
{
    //GameObject basketBallGoalPosition;
    public List<GameObject> spawnPositions;
    public List<GameObject> enemyMinionPrefabs;
    public List<GameObject> enemyBossPrefabs;

    [SerializeField]
    int numberOfEnemies;

    [SerializeField]
    int maxNumberOfEnemies;
    [SerializeField]
    int maxNumberOfBoss = 1;
    int maxNumberOfMinions;

    private void Awake()
    {
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            // position transform relative to basketball goal
            //basketBallGoalPosition = GameObject.Find("rim");
            //transform.position = new Vector3(basketBallGoalPosition.transform.position.x, 0, basketBallGoalPosition.transform.position.z);
            //Debug.Log(spawnPositions.Capacity);
            maxNumberOfEnemies = enemyBossPrefabs.Count + enemyMinionPrefabs.Count;

            // max for hardcore
            if (GameOptions.hardcoreModeEnabled)
            {
                maxNumberOfEnemies = 8;
            }
            // regular
            else
            {
                maxNumberOfEnemies = 6;
            }
            maxNumberOfMinions = maxNumberOfEnemies - maxNumberOfBoss;
            Debug.Log("minions : " + maxNumberOfMinions);
            Debug.Log("boss : " + maxNumberOfBoss);

            //#if UNITY_STANDALONE || UNITY_EDITOR
            //            numEnemiesToSpawn = spawnPositions.Capacity;

            //#endif

            // if mobile cut number in half
#if UNITY_ANDROID && !UNITY_EDITOR
            maxNumberOfEnemies = spawnPositions.Capacity/2;
#endif
            spawnDefaultMinions();
            spawnBoss();

        }
        // this needs to second option or enabling it will spawn enemies
        if (GameObject.FindGameObjectWithTag("enemy") != null)
        {
            GameOptions.enemiesEnabled = true;
        }
    }

    void spawnDefaultMinions()
    {
        for (int i = 0; i < maxNumberOfMinions; i++)
        {
            // if spawn more enemies than enemy list has, spawn random enemy
            if (i >= enemyMinionPrefabs.Count)
            {
                Random random = new Random();
                int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);

                Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
            }
            else
            {
                Instantiate(enemyMinionPrefabs[i], spawnPositions[i].transform.position, Quaternion.identity);
            }
        }
    }
    void spawnSingleMinion()
    {
        Random random = new Random();
        int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);

        // if spawn more enemies than enemy list has, spawn random enemy
        if (randomIndex >= enemyMinionPrefabs.Count)
        {
            Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
        }

        else
        {
            Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
        }
    }

    void spawnBoss()
    {
        for (int i = 0; i < maxNumberOfBoss; i++)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);

            Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
        }
    }

    void Start()
    {
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            InvokeRepeating("getNumberOfCurrentEnemiesInScene", 0, 2f);
        }
    }

    void getNumberOfCurrentEnemiesInScene()
    {
        // *note : dont need to check for boss. if boss killed, doesnt respawn

        numberOfEnemies = GameObject.FindGameObjectsWithTag("enemy").Length;
        if (numberOfEnemies < maxNumberOfEnemies)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, maxNumberOfMinions);

            // update spawner location so spawn locations is near player
            gameObject.transform.position = GameLevelManager.instance.Player.transform.position;
            //Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
            spawnSingleMinion();
        }
    }
}
