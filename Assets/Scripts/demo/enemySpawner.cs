using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class enemySpawner : MonoBehaviour
{
    public GameObject[] enemies;
    public int numEnemies;
    public int spawnNum;
    public int maxEnemiesDemo1, maxEnemiesDemo2;
    [SerializeField]
    int maxEnemiesLifetime, enemiesLifetimeCounter = 0;

    public int maxEnemies;
    public bool gameMode1, demoMode2;
    public bool startGame = false;
    // Use this for initialization

    public static enemySpawner instance;
    public bool notLocked;

    void Start()
    {
        // get max enemies
        if (gameOptions.instance != null)
        {
            maxEnemies = gameOptions.instance.getOptionMaxEnemies();
        }
        else
        {
            maxEnemies = 3;
        }
        //enemies = GameObject.FindGameObjectsWithTag("Enemy");
        //numEnemies = enemies.Length;
        notLocked = true;
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {

        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        numEnemies = enemies.Length;

        // if current numEnemies  < max allowable // max numEnemies limit < current amount that have existed
        if (numEnemies < maxEnemies && notLocked && enemiesLifetimeCounter < maxEnemiesLifetime)
        {
            // max allowable - current amount of enemies
            spawnNum = maxEnemies - numEnemies;
            // spawn max number of enemies allowed by reach limit
            for (int i = 0; i < spawnNum; i++)
            {
                int randomSpawnPos = Random.Range(0, 6); // random spawn point
                int randomEnemy = Random.Range(0, enemyList.instance.enemyName.Capacity); // random enemy from list of all enemies
                //Debug.Log("randomSpawnPos : " + randomSpawnPos);
                //Debug.Log("randomEnemy : " + randomEnemy);

                Instantiate(enemyList.instance.enemyName[randomEnemy],
                        //enemyList.instance.enemyName[20],
                        levelManager.instance.spawnPoints[randomSpawnPos].position,
                        levelManager.instance.spawnPoints[randomSpawnPos].rotation);
                //disableEnemyHealthUI();
                numEnemies++;
                enemiesLifetimeCounter++;

            }
            disableEnemyHealthUI();
        }

    }

    private void disableEnemyHealthUI()
    {

        //Debug.Log("---------------------   private void disableEnemyHealthUI()");
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemyHealthUI");
        foreach (GameObject enemy in enemies)
        {
            if (!enemy.transform.root.CompareTag("companion")) // if not companion
            {
                enemy.SetActive(false);
            }
        }
    }
}
