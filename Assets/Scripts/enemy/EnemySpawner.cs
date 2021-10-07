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
            // ***** DISABLE FOR TESTING
            //GameOptions.enemiesEnabled = true;
            GameObject[] enemyHealthList = GameObject.FindGameObjectsWithTag("enemy");
            foreach (GameObject go in enemyHealthList)
            {
                if (go.GetComponentInChildren<EnemyController>().IsBoss)
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
        //// this needs to second option or enabling it will spawn enemies
        //if (GameObject.FindGameObjectWithTag("enemy") != null)
        //{
        //    GameOptions.enemiesEnabled = true;
        //}
        // if enemies in scene, spawn max
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            if (GameOptions.hardcoreModeEnabled)
            {
                maxNumberOfEnemies = 8;
            }
            else if (GameOptions.battleRoyalEnabled)
            {
                maxNumberOfEnemies = 4;
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
        }
    }

    void spawnDefaultMinions()
    {
        int numberToSpawn = maxNumberOfEnemies - numberOfMinions - numberOfBoss;

        Random random = new Random();
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                //Random random = new Random();
                int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);
                //Debug.Log("randomIndex : " + randomIndex + "  max : "+ (enemyMinionPrefabs.Count - 1));
                Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
                // if spawn more enemies than enemy list has, spawn random enemy
                //if (i >= enemyMinionPrefabs.Count)
                //{
                //    Random random = new Random();
                //    int randomIndex = random.Next(0, maxNumberOfMinions - 1);
                //    Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
                //}
                //else
                //{
                //    Instantiate(enemyMinionPrefabs[i], spawnPositions[i].transform.position, Quaternion.identity);
                //}
            }
        }
    }

    void spawnDefaultBoss()
    {
        //Debug.Break();
        int numberToSpawn = maxNumberOfBoss - numberOfBoss;
        Random random = new Random();
        int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);
        if (numberToSpawn > 0)
        {
            for (int i = 0; i < numberToSpawn; i++)
            {
                // if spawn more enemies than enemy list has, spawn random enemy
                if (i >= enemyBossPrefabs.Count)
                {
                    //Random random = new Random();
                    //int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);
                    Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
                }
                else
                {
                    Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[i].transform.position, Quaternion.identity);
                }
            }
        }
    }

    void spawnSingleMinion()
    {

        Random random = new Random();
        int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);

        int spawnIndex = 0;
        if(randomIndex >= spawnPositions.Count-1)
        {
            spawnIndex = spawnPositions.Count - 1;
        }
        else
        {
            spawnIndex = randomIndex;
        }
        // if spawn more enemies than enemy list has, spawn random enemy
        if (randomIndex >= enemyMinionPrefabs.Count)
        {
            Instantiate(enemyMinionPrefabs[0], spawnPositions[spawnIndex].transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(enemyMinionPrefabs[randomIndex], spawnPositions[spawnIndex].transform.position, Quaternion.identity);
        }

        //Random random = new Random();
        //int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);
        //GameObject[] enemyList = GameObject.FindGameObjectsWithTag("enemy");
        //GameObject enemy = null;

        ////int index = enemyList.Where(x => x.name)
        //foreach (GameObject go in enemyList)
        //{
        //    enemy = enemyMinionPrefabs.Where(x => !x.name.Contains(go.name)).First();
        //    Debug.Log("enemy to spawn : " + go.name);
        //}
        //Instantiate(enemy, spawnPositions[randomIndex].transform.position, Quaternion.identity);
    }

    void spawnBoss()
    {
        Random random = new Random();
        int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);

        // if spawn more enemies than enemy list has, spawn random enemy
        if (randomIndex >= enemyBossPrefabs.Count - 1)
        {
            Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
        }
        else
        {
            Instantiate(enemyBossPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
        }
    }

    void getNumberOfCurrentEnemiesInScene()
    {
        // *note : dont need to check for boss. if boss killed, doesnt respawn

        numberOfMinions = GameObject.FindGameObjectsWithTag("enemy").Length;
        numberOfBoss = getNumberOfBoss();

        if (numberOfMinions < maxNumberOfMinions)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, enemyMinionPrefabs.Count - 1);

            // update spawner location so spawn locations is near player
            spawnSingleMinion();
        }
        if (numberOfBoss < maxNumberOfBoss)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, enemyBossPrefabs.Count - 1);

            // update spawner location so spawn locations is near player
            spawnBoss();
        }
    }

    int getNumberOfBoss()
    {
        int value = 0;
        GameObject[] enemyHealthList = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject go in enemyHealthList)
        {
            if (go.GetComponentInChildren<EnemyController>().IsBoss)
            {
                value++;
            }
        }
        return value;
    }
}
