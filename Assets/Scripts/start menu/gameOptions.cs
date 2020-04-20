using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameOptions : MonoBehaviour
{

    [SerializeField]
    List<string> optionGameModes;
    [SerializeField]
    string optionCurrentGameMode;
    [SerializeField]
    int optionMaxEnemies;
    [SerializeField]
    List<string> optionWeaponList;
    [SerializeField]
    string optionCurrentWeaponList;
    [SerializeField]
    int optionNumberOfLives;

    public static gameOptions instance;

    // Use this for initialization
    void Awake()
    {
        instance = this;

        optionMaxEnemies = 1;
        optionNumberOfLives = 1;
        setGameModeList();
        setWeaponList();
        //if gameoptions IS empty, dont destroy. hopefully this will save one instance of them in memory
        // todo: write option to gamemanager maybe, so i can destroy these objects
        DontDestroyOnLoad(instance.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    void setGameModeList()
    {
        optionGameModes = new List<string>();
        optionGameModes.Add("timed kills");
        optionGameModes.Add("free play");
        optionCurrentGameMode = optionGameModes[0];
    }
    void setWeaponList()
    {
        optionWeaponList = new List<string>();
        optionWeaponList.Add("no weapons");
        optionWeaponList.Add("pistols only");
        optionWeaponList.Add("melee only");
        optionWeaponList.Add("special only");
        optionWeaponList.Add("throw only");
        optionWeaponList.Add("all weapons");
        optionCurrentWeaponList = optionWeaponList[0];
    }
    // get functions
    public List<string> getOptionGameModes()
    {
        return optionGameModes;
    }
    public List<string> getOptionWeaponLists()
    {
        return optionWeaponList;
    }
    public int getOptionMaxEnemies()
    {
        return optionMaxEnemies;
    }
    public int getOptionNumberOfLives()
    {
        return optionNumberOfLives;
    }
    public string getCurrentWeaponList()
    {
        return optionCurrentWeaponList;
    }
    public string getCurrentGameMode()
    {
        return optionCurrentGameMode;
    }

    // set functions
    public void setCurrentGameMode(int index)
    {
        optionCurrentGameMode = optionGameModes[index];
    }
    public void setCurrentWeaponList(int index)
    {
        optionCurrentWeaponList = optionWeaponList[index];
    }
    public void setCurrentNumberOfEnemies(int numIncrease)
    {
        optionMaxEnemies += numIncrease;
    }
    public void setCurrentNumberOfLives(int numIncrease)
    {
        optionNumberOfLives += numIncrease;
    }
}
