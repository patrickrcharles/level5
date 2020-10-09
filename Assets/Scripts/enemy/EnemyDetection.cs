using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDetection : MonoBehaviour
{
    EnemyController enemyController;

    private void Start()
    {
        enemyController = GetComponent<EnemyController>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("player detected");
            Debug.Log("go to player");
            enemyController.StatePursue = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("pursuit ended");
            enemyController.StatePursue = false;
        }
    }
}
