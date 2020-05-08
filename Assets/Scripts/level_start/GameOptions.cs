using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public  class GameOptions : MonoBehaviour
{
    static public String playerSelected;
    [SerializeField]
    static public String levelSelected;
    static public int levelSelectedRootCount;
    static public int levelSelectedBuildIndex;
    [SerializeField]
    static public String levelSelectedName;
    [SerializeField]
    static public String gameModeSelectedName;
    [SerializeField]
    static public int gameModeSelected;

    //player stats
    static public float accuracy2pt;
    static public float accuracy3pt;
    static public float accuracy4pt;
    static public float accuracy7pt;

    static public float jumpForce;
    static public float speed;
    static public float runSpeed;
    static public float runSpeedHasBall;
    //static public float hangTime;
   // static public float range;
   // static public float release;
    static public float criticalPercent;

    static public float shootAngle;

    static public bool gameModeHasBeenSelected;

    //static public int level;
    //static public int experience;
    //static public decimal money;

    void Awake()
    {
        levelSelectedRootCount = SceneManager.GetActiveScene().rootCount;
        levelSelectedName = SceneManager.GetActiveScene().name;
        levelSelectedBuildIndex = SceneManager.GetActiveScene().buildIndex;

       //Debug.Log("levelSected : " + levelSelected);
       //Debug.Log("levelSectedName : " + levelSelectedName);
       //Debug.Log("levelSelectedBuildIndex : " + levelSelectedBuildIndex);
    }

    static public void printCurrentValues()
    {
        Debug.Log("playerSelected : " + playerSelected);
        Debug.Log("levelSected : " + levelSelectedName);
        Debug.Log("gameModeSelected : " + gameModeSelected);
        Debug.Log("gameModeSelected : " + gameModeSelectedName);
        Debug.Log("gameModeSelected : " + gameModeHasBeenSelected);
    }
}
