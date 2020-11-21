using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    GameObject basketBallGoalPosition;
    public List<GameObject> spawnPositions;
    public List<GameObject> enemyPrefabs;

    private void Start()
    {
        if (GameOptions.enemiesEnabled)
        {
            // position transform relative to basketball goal
            basketBallGoalPosition = GameObject.Find("rim");
            transform.position = new Vector3(basketBallGoalPosition.transform.position.x, 0, basketBallGoalPosition.transform.position.z);
            //Debug.Log(spawnPositions.Capacity);
            int numEnemiesToSpawn = 0;
#if UNITY_STANDALONE || UNITY_EDITOR
            numEnemiesToSpawn = spawnPositions.Capacity;
#endif
#if UNITY_ANDROID
            numEnemiesToSpawn = 3;
#endif
            for (int i = 0; i < numEnemiesToSpawn; i++)
            {
                Instantiate(enemyPrefabs[i], spawnPositions[i].transform.position, Quaternion.identity);
                //enemyPrefabs[i].transform.position = spawnPositions[i].transform.position;
            }
        }
        // this needs to second option or enabling it will spawn enemies
        else if (GameObject.FindGameObjectWithTag("enemy") != null)
        {
            GameOptions.enemiesEnabled = true;
        }
    }
}
