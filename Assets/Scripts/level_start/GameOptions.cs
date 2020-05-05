using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GameOptions : MonoBehaviour
{
    static public String playerSelected;
    [SerializeField]
    static public String levelSelected;
    [SerializeField]
    static public String levelSelectedName;
    static public String gameModeSelectedName;
    static public int gameModeSelected;

    //player stats
    static public float accuracy2pt;
    static public float accuracy3pt;
    static public float accuracy4pt;

    static public float jumpForce;
    static public float speed;
    static public float runSpeed;
    //static public float hangTime;
   // static public float range;
   // static public float release;
    static public float criticalPercent;

    static public float shootAngle;

    //static public int level;
    //static public int experience;
    //static public decimal money;


    static public void printCurrentValues()
    {
        Debug.Log("playerSelected : " + playerSelected);
        Debug.Log("levelSected : " + levelSelectedName);
        Debug.Log("gameModeSelected : " + gameModeSelected);
        Debug.Log("gameModeSelected : " + gameModeSelectedName);
    }
}
