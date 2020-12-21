using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour
{
    //GameObject basketBallGoalPosition;
    public List<GameObject> spawnPositions;
    [SerializeField]
    public List<GameObject> enemyMinionPrefabs;
    [SerializeField]
    public List<GameObject> enemyBossPrefabs;
    [SerializeField]
    int numberOfMinions;
    [SerializeField]
    int numberOfBoss;
    [SerializeField]
    int maxNumberOfEnemies;
    [SerializeField]
    int maxNumberOfBoss = 1;
    [SerializeField]
    int maxNumberOfMinions;

    private void Awake()
    {
        // get number of enemies already in scene
        if (GameObject.FindGameObjectWithTag("enemy") != null)
        {
            // this needs to second option or enabling it will spawn enemies
            GameOptions.enemiesEnabled = true;
            GameObject[] enemyHealthList = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject go in enemyHealthList)
            {
                Debug.Log("enemy : " + go.name);
                if (go.GetComponentInChildren<EnemyHealth>().IsBoss)
                {
                    numberOfBoss++;
                }
                else
                {
                    numberOfMinions++;
                }
            }
        }
    }

    private void Start()
    {
        //Debug.Log("GameObject.FindGameObjectWithTag(enemy) != null : " + (GameObject.FindGameObjectWithTag("enemy") != null));
        //// this needs to second option or enabling it will spawn enemies
        //if (GameObject.FindGameObjectWithTag("enemy") != null)
        //{
        //    GameOptions.enemiesEnabled = true;
        //}
        // if enemies in scene, spawn max
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            Debug.Log("GameOptions.hardcoreModeEnabled : " + GameOptions.hardcoreModeEnabled);
            if (GameOptions.hardcoreModeEnabled)
            {
                maxNumberOfEnemies = 8;
            }
            else
            {
                maxNumberOfEnemies = 6;
            }


#if UNITY_ANDROID && !UNITY_EDITOR
            maxNumberOfEnemies = maxNumberOfEnemies/2;
#endif
            maxNumberOfMinions = maxNumberOfEnemies - maxNumberOfBoss;

            // spawn enemies if necessary
            spawnDefaultMinions();
            spawnDefaultBoss();

            // start function to check status of current enemies
            InvokeRepeating("getNumberOfCurrentEnemiesInScene", 0, 2f);

            Debug.Log("GameOptions.enemiesEnabled" + GameOptions.enemiesEnabled);
        }
    }



    void spawnDefaultMinions()
    {
        Debug.Log("---------------------------spawnDefaultMinions ");
        Debug.Log("maxNumberOfEnemies : " + maxNumberOfEnemies);
        Debug.Log("maxNumberOfBoss : " + maxNumberOfBoss);
        Debug.Log("maxNumberOfMinions : " + maxNumberOfMinions);
        Debug.Log("numberOfMinions : " + numberOfMinions);
        Debug.Log("numberOfBoss : " + numberOfBoss);
        Debug.Log("int numberToSpawn = " + maxNumberOfEnemies + " -  " + numberOfMinions + " - " + numberOfBoss);

        int numberToSpawn = maxNumberOfEnemies - numberOfMinions - numberOfBoss;

        Debug.Log("numberToSpawn : " + numberToSpawn);
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                // if spawn more enemies than enemy list has, spawn random enemy
                if (i >= enemyMinionPrefabs.Count)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(0, maxNumberOfMinions - 1);
                    Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
                    Debug.Log("enemyMinionPrefabs[randomIndex] " + enemyMinionPrefabs[randomIndex].name);
                }
                else
                {
                    Instantiate(enemyMinionPrefabs[i], spawnPositions[i].transform.position, Quaternion.identity);
                    Debug.Log("enemyMinionPrefabs[randomIndex] " + enemyMinionPrefabs[i].name);
                }
            }
        }
    }
    void spawnDefaultBoss()
    {
        Debug.Log("-------------------------------spawnDefaultBoss ");
        Debug.Log("spawnDefaultMinions ");
        Debug.Log("maxNumberOfEnemies : " + maxNumberOfEnemies);
        Debug.Log("maxNumberOfBoss : " + maxNumberOfBoss);
        Debug.Log("maxNumberOfMinions : " + maxNumberOfMinions);
        Debug.Log("numberOfMinions : " + numberOfMinions);
        Debug.Log("numberOfBoss : " + numberOfBoss);
        Debug.Log("int numberToSpawn = " + maxNumberOfBoss + " -  " + numberOfBoss);

        int numberToSpawn = maxNumberOfBoss - numberOfBoss;
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                // if spawn more enemies than enemy list has, spawn random enemy
                if (i >= enemyBossPrefabs.Count)
                {
                    Random random = new Random();
                    int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);
                    Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
                    Debug.Log("enemyBossPrefabs[randomIndex] " + enemyBossPrefabs[randomIndex].name);
                }
                else
                {
                    Instantiate(enemyMinionPrefabs[i], spawnPositions[i].transform.position, Quaternion.identity);
                    Debug.Log("enemyBossPrefabs[randomIndex] " + enemyBossPrefabs[i].name);
                }
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
            Instantiate(enemyMinionPrefabs[0], spawnPositions[randomIndex].transform.position, Quaternion.identity);
            Debug.Log("spawn minion : " + enemyMinionPrefabs[randomIndex].name);

        }

        else
        {
            Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
            Debug.Log("spawn minion : " + enemyMinionPrefabs[randomIndex].name);
        }
    }

    void spawnBoss()
    {
        Random random = new Random();
        int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);

        // if spawn more enemies than enemy list has, spawn random enemy
        if (randomIndex >= enemyBossPrefabs.Count - 1)
        {
            Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
            Debug.Log("spawn boss : " + enemyBossPrefabs[randomIndex].name);
        }

        else
        {
            Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
            Debug.Log("spawn boss : " + enemyBossPrefabs[randomIndex].name);
        }
    }


    void getNumberOfCurrentEnemiesInScene()
    {
        // *note : dont need to check for boss. if boss killed, doesnt respawn

        numberOfMinions = GameObject.FindGameObjectsWithTag("enemy").Length;
        numberOfBoss = getNumberOfBoss();
        Debug.Log("minions : " + numberOfMinions + "     boss : " + numberOfBoss);

        if (numberOfMinions < maxNumberOfMinions)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);

            // update spawner location so spawn locations is near player
            //gameObject.transform.position = GameLevelManager.instance.Player.transform.position;
            spawnSingleMinion();
        }
        if (numberOfBoss < maxNumberOfBoss)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);

            // update spawner location so spawn locations is near player
            //gameObject.transform.position = GameLevelManager.instance.Player.transform.position;
            spawnBoss();
        }
    }

    int getNumberOfBoss()
    {
        int value = 0;
        GameObject[] enemyHealthList = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject go in enemyHealthList)
        {
            if (go.GetComponentInChildren<EnemyHealth>().IsBoss)
            {
                value++;
            }
        }
        return value;
    }
}
