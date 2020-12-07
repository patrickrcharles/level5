using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

public class EnemySpawner : MonoBehaviour
{
    GameObject basketBallGoalPosition;
    public List<GameObject> spawnPositions;
    public List<GameObject> enemyPrefabs;

    [SerializeField]
    int numberOfEnemies;

    [SerializeField]
    int maxNumberOfEnemies;

    private void Awake()
    {
        if (GameOptions.enemiesEnabled || GameOptions.EnemiesOnlyEnabled)
        {
            // position transform relative to basketball goal
            //basketBallGoalPosition = GameObject.Find("rim");
            //transform.position = new Vector3(basketBallGoalPosition.transform.position.x, 0, basketBallGoalPosition.transform.position.z);
            //Debug.Log(spawnPositions.Capacity);
            maxNumberOfEnemies = enemyPrefabs.Count;
            int numEnemiesToSpawn = 0;

#if UNITY_STANDALONE || UNITY_EDITOR
            numEnemiesToSpawn = spawnPositions.Capacity;
            
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
            numEnemiesToSpawn = spawnPositions.Capacity/2;
#endif
            //Debug.Log(numEnemiesToSpawn);
            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                Instantiate(enemyPrefabs[i], spawnPositions[i].transform.position, Quaternion.identity);
                //enemyPrefabs[i].transform.position = spawnPositions[i].transform.position;
            }
        }
        // this needs to second option or enabling it will spawn enemies
        if (GameObject.FindGameObjectWithTag("enemy") != null)
        {
            GameOptions.enemiesEnabled = true;
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
        numberOfEnemies = GameObject.FindGameObjectsWithTag("enemy").Length;
        if (numberOfEnemies < maxNumberOfEnemies)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, enemyPrefabs.Count - 1);

            // update spawner location so spawn locations is near player
            gameObject.transform.position = GameLevelManager.instance.Player.transform.position;
            Instantiate(enemyPrefabs[randomIndex], spawnPositions[randomIndex].transform.position, Quaternion.identity);
        }
    }
}
