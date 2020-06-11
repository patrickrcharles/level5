﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisions : MonoBehaviour
{
    [SerializeField]
    PlayerController playerState;
    [SerializeField]
    bool playerCanBeKnockedDown;

    private void Awake()
    {
        //Debug.Log(" player collsions : awake")
        playerState = gameObject.GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(" this : " + gameObject.tag + "       other : " + other.tag);
        // if collsion between hitboc and vehicle, knocked down
        if (gameObject.CompareTag("Player") && other.CompareTag("knock_down_attack") && !playerState.KnockedDown && playerCanBeKnockedDown)
        {
            playerState.KnockedDown = true;
            Debug.Log("KnockedDown = true;");
        }
    }
}
