using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class GameOptions : MonoBehaviour
{
    [SerializeField]
    static public String playerSelected;
    [SerializeField]
    static public String levelSelected;
    [SerializeField]
    static public String levelSelectedName;
    static public int gameModeSelected;

    static public void printCurrentValues()
    {
        Debug.Log("playerSelected : "+ playerSelected );
        Debug.Log("levelSected : " + levelSelected);
        Debug.Log("gameModeSelected : " + gameModeSelected);
    }
}
