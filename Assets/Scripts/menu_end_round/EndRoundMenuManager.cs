using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndRoundMenuManager : MonoBehaviour
{

    int currentWinnerScore;
    int currentLoserScore;
    bool currentWinnerisCpu;
    bool currentLoserisCpu;
    int numOfContiues;
    string nextLevelName;
    List<LevelSelected> levelsList = new List<LevelSelected>();

    private void Awake()
    {

    }
    // Update is called once per frame
    void Start()
    {        
        if (EndRoundData.numberOfContinues <= 0)
        {
            EndRoundUIObjects.instance.continueOptionObject.SetActive(false);
            EndRoundUIObjects.instance.continueOptionObject.SetActive(false);
        }
        LoadData();
    }

    private void Update()
    {

    }

    private void LoadData()
    {
        EndRoundData.levelsList = GameOptions.levelsList;
        EndRoundData.currentLevelIndex = GameOptions.levelSelectedIndex;

        currentWinnerScore = EndRoundData.currentRoundWinnerScore;
        currentLoserScore = EndRoundData.currentRoundLoserScore;

        currentWinnerisCpu = EndRoundData.currentRoundWinnerIsCpu;
        currentLoserisCpu = EndRoundData.currentRoundLoserIsCpu;

        if (currentWinnerisCpu)
        {
            GameOptions.levelSelectedIndex--;
            EndRoundUIObjects.instance.nextRoundText.text = "Try Again";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex-1].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex-1].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            EndRoundUIObjects.instance.continueNumber.text = EndRoundData.numberOfContinues.ToString();
            nextLevelName =
            EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDescription;

            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].cpuPlayerWinImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.currentRoundPlayerLoserImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "CPU";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "Player 1";
        }
        else
        {
            EndRoundUIObjects.instance.nextRoundText.text = "Start";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            nextLevelName =
            EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelDescription;

            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.currentRoundPlayerWinnerImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].cpuPlayerLoseImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "Player 1";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "CPU";
        }

        EndRoundUIObjects.instance.continueNumber.text = EndRoundData.numberOfContinues.ToString();
        EndRoundUIObjects.instance.currentRoundWinnerScore.text = EndRoundData.currentRoundWinnerScore.ToString();
        EndRoundUIObjects.instance.currentRoundLoserScore.text = EndRoundData.currentRoundLoserScore.ToString();
    }

    public void pressNext()
    {
        if (EndRoundData.numberOfContinues > 0 && currentWinnerisCpu)
        {
            EndRoundData.numberOfContinues--;
        }
        SceneManager.LoadScene(nextLevelName);
    }
    //public void pressContinue()
    //{
    //    if(EndRoundData.numberOfContinues > 0)
    //    {
    //        EndRoundData.numberOfContinues--;
    //        SceneManager.LoadScene(nextLevelName);
    //    }
    //}
    public void pressStartMenu()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
    }
    public void pressQuit()
    {
        Application.Quit();
    }
}
