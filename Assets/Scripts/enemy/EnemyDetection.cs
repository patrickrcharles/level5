﻿using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    EnemyController enemyController;
    [SerializeField]
    bool playerSighted;
    bool enemyDetectionEnabled = true;
    public float enemySightDistance;

    public bool PlayerSighted { get => playerSighted; set => playerSighted = value; }

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
        if(enemySightDistance == 0)
        {
            enemySightDistance = 5;
        }
        //enemyDetectionEnabled = true;
        InvokeRepeating("CheckPlayerDistance", 0, 0.2f);
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        //Debug.Log("player detected");
    //        Debug.Log("go to player");
    //        PlayerSighted = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.CompareTag("playerHitbox"))
    //    {
    //        //Debug.Log("pursuit ended");
    //        PlayerSighted = false;
    //    }
    //}

    void CheckPlayerDistance()
    {
        if(enemyController.DistanceFromPlayer < enemySightDistance 
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

    public void TurnOffEnemySight(float seconds)
    {
        StartCoroutine(DelayEnemmySight(seconds));
    }

    IEnumerator DelayEnemmySight(float seconds)
    {
        enemyDetectionEnabled = false;
        playerSighted = false;
        yield return new WaitForSeconds(seconds);
        enemyDetectionEnabled = true;
    }
}
