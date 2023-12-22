using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EndRoundMenuManager : MonoBehaviour
{

    int currentWinnerScore;
    int currentLoserScore;
    bool currentWinnerisCpu;
    bool currentLoserisCpu;
    bool tieGame = false;
    int numOfContiues;
    string nextLevelName;
    List<LevelSelected> levelsList = new List<LevelSelected>();

    void Start()
    {     
        if (EndRoundData.numberOfContinues <= 0 && !currentWinnerisCpu)
        Debug.Log("EndRoundData.currentLevelIndex: " + (EndRoundData.currentLevelIndex));
        Debug.Log("GameOptions.levelsList : " + (GameOptions.levelsList.Count-1));
        {
            //EndRoundUIObjects.instance.continueOptionObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(EndRoundUIObjects.instance.startMenuButton.gameObject);
            EndRoundUIObjects.instance.nextInfoObject.SetActive(false);
            EndRoundUIObjects.instance.endMessageObject.SetActive(true);
            EndRoundUIObjects.instance.endMessageText.text = "You suck. go sit on the tire.";
        }
        LoadData();
    }

    private void LoadData()
    {
        EndRoundData.levelsList = GameOptions.levelsList;
        EndRoundData.currentLevelIndex = GameOptions.levelSelectedIndex;

        currentWinnerScore = EndRoundData.currentRoundWinnerScore;
        currentLoserScore = EndRoundData.currentRoundLoserScore;
   
        if(currentWinnerScore == currentLoserScore)
        {
            tieGame = true;
            EndRoundUIObjects.instance.winnerText.text = "tie";
            EndRoundUIObjects.instance.loserText.text = "tie";
        }
        currentWinnerisCpu = tieGame ? true : EndRoundData.currentRoundWinnerIsCpu;
        currentLoserisCpu = EndRoundData.currentRoundLoserIsCpu;

        if (currentWinnerisCpu)
        {
            GameOptions.levelSelectedIndex--;
            EndRoundUIObjects.instance.nextRoundText.text = !tieGame ? "Try Again" : "Tie Game";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            EndRoundUIObjects.instance.continueNumber.text = EndRoundData.numberOfContinues.ToString();
            nextLevelName =
            EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDescription;

            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayerWinImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.currentRoundPlayerLoserImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "CPU";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "Player 1";
        }
        else
        {
            EndRoundUIObjects.instance.nextRoundText.text = !tieGame ? "Start" : "Tie Game";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            nextLevelName =
            EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelDescription;

            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.currentRoundPlayerWinnerImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayerLoseImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "Player 1";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "CPU";
        }

        EndRoundUIObjects.instance.continueNumber.text = EndRoundData.numberOfContinues.ToString();
        EndRoundUIObjects.instance.currentRoundWinnerScore.text = EndRoundData.currentRoundWinnerScore.ToString();
        EndRoundUIObjects.instance.currentRoundLoserScore.text = EndRoundData.currentRoundLoserScore.ToString();
    }

    public void pressNext()
    {
        Debug.Log("pressNext() : "+ EndRoundData.numberOfContinues);
        if (EndRoundData.numberOfContinues > 0 && currentWinnerisCpu)
        {
            if (!tieGame) { EndRoundData.numberOfContinues--;}
            SceneManager.LoadScene(nextLevelName);
        }
        if (EndRoundData.numberOfContinues > 0 && !currentWinnerisCpu)
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void pressStartMenu()
    {
        SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
    }
    public void pressQuit()
    {
        Application.Quit();
    }
}
