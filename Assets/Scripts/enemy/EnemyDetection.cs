using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    EnemyController enemyController;
    bool playerSighted;
    bool enemyDetectionEnabled = true;
    public float enemySightDistance;

    public bool PlayerSighted { get => playerSighted; set => playerSighted = value; }

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        if (enemySightDistance == 0)
        {
            enemySightDistance = 5;
        }
        // if only enemies, make increase enemy sight
        if (GameOptions.EnemiesOnlyEnabled)
        {
            enemySightDistance = 20;
        }
        //enemyDetectionEnabled = true;
        InvokeRepeating("CheckPlayerDistance", 0, 0.2f);
        InvokeRepeating("CheckReturnToPatrolStatus", 0, 3f);
    }

    void CheckPlayerDistance()
    {
        if (enemyController.DistanceFromPlayer < enemySightDistance
            && enemyDetectionEnabled)
        {

            playerSighted = true;
        }
        if (enemyController.DistanceFromPlayer >= enemySightDistance
            && enemyDetectionEnabled)
        {
            playerSighted = false;
        }
    }

    //public void TurnOffEnemySight(float seconds)
    //{
    //    StartCoroutine(DelayEnemmySight(seconds));
    //}

    IEnumerator DelayEnemySight(float seconds)
    {
        enemyDetectionEnabled = false;
        playerSighted = false;
        yield return new WaitForSeconds(seconds);
        enemyDetectionEnabled = true;
    }

    void CheckReturnToPatrolStatus()
    {
        if (enemyController.stateIdle
            && gameObject.transform.position != enemyController.OriginalPosition
            && !playerSighted
            && enemyDetectionEnabled)
        {
            enemyController.statePatrol = true;
        }
        else
        {
            enemyController.statePatrol = false;
        }
    }
}
