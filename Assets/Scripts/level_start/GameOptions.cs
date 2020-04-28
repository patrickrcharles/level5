using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GameOptions : MonoBehaviour
{
    [SerializeField]
    static public String playerSelected;
    [SerializeField]
    static public String levelSected;
    static public int gameModeSelected;

    static public void printCurrentValues()
    {
        Debug.Log("playerSelected : "+ playerSelected );
        Debug.Log("levelSected : " + levelSected);
        Debug.Log("gameModeSelected : " + gameModeSelected);
    }
}
