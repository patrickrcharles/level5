﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackPosition : MonoBehaviour
{
    public bool engaged;
    public GameObject enemyEngaged;
    public int attackPositionId;
    public Vector3 position;

    private void Start()
    {
        InvokeRepeating("updateAttackPositionTransform", 0, 1);
    }

    void updateAttackPositionTransform()
    {
        if(enemyEngaged == null)
        {
            engaged = false;
        }
        Vector3 playerTransform = GameLevelManager.instance.Player.transform.position;
        if (attackPositionId == 1)
        {
            transform.position = new Vector3(playerTransform.x - 0.6f, playerTransform.y, playerTransform.z - 0.25f);
        }
        if (attackPositionId == 2)
        {
            transform.position = new Vector3(playerTransform.x - 0.6f, playerTransform.y, playerTransform.z + 0.25f);
        }
        if (attackPositionId == 3)
        {
            transform.position = new Vector3(playerTransform.x + 0.6f, playerTransform.y, playerTransform.z - 0.25f);
        }
        if (attackPositionId == 4)
        {
            transform.position = new Vector3(playerTransform.x + 0.6f, playerTransform.y, playerTransform.z + 0.25f);
        }
        position = transform.position;
    }
}
