using Assets.Scripts.database;
using Assets.Scripts.restapi;
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
    string nextLevelName;
    List<LevelSelected> levelsList = new List<LevelSelected>();
    [SerializeField]
    GameStats gameStats = new GameStats();
    bool isGameSaved = false;
    bool finalLevel = false;

    void Start()
    {
        Debug.Log("EndRoundData.currentLevelIndex: " + (EndRoundData.currentLevelIndex));
        Debug.Log("GameOptions.levelsList : " + (GameOptions.levelsList.Count-1));

        if (EndRoundData.currentLevelIndex == GameOptions.levelsList.Count-1)
        {
            finalLevel = true;
            EventSystem.current.SetSelectedGameObject(EndRoundUIObjects.instance.startMenuButton.gameObject);
            EndRoundUIObjects.instance.nextInfoObject.SetActive(false);
            EndRoundUIObjects.instance.endMessageObject.SetActive(true);
            EndRoundUIObjects.instance.endMessageText.text = "You beat all the Computahs. Sick.";
            saveGame();
        }
        currentWinnerScore = EndRoundData.currentRoundWinnerScore;
        currentLoserScore = EndRoundData.currentRoundLoserScore;        
        currentWinnerisCpu = EndRoundData.currentRoundWinnerIsCpu;
        currentLoserisCpu = EndRoundData.currentRoundLoserIsCpu;
        
        PlayerData.instance.CampaignGameStats.campaignGamesPlayed++;
        if (currentWinnerScore == currentLoserScore)
        {
            tieGame = true;
            PlayerData.instance.CampaignGameStats.campaignTies++;
        }
        if (currentWinnerisCpu && !tieGame)
        {
            PlayerData.instance.CampaignGameStats.campaignLosses++;
        }
        if (!currentWinnerisCpu && !tieGame)
        {
            PlayerData.instance.CampaignGameStats.campaignWins++;
        }

        // game over
        if (EndRoundData.numberOfContinues <= 0 && currentWinnerisCpu && !tieGame && !finalLevel)
        {
            //PlayerData.instance.CampaignGameStats.campaignLosses++;
            //EndRoundUIObjects.instance.continueOptionObject.SetActive(false);
            EventSystem.current.SetSelectedGameObject(EndRoundUIObjects.instance.startMenuButton.gameObject);
            EndRoundUIObjects.instance.nextInfoObject.SetActive(false);
            EndRoundUIObjects.instance.endMessageObject.SetActive(true);
            EndRoundUIObjects.instance.endMessageText.text = "You suck. go sit on the tire.";
            saveGame();
        }
            LoadData();
    }
    private void saveGame()
    {
        isGameSaved = true;
        HighScoreModel dBHighScoreModel = new();
        HighScoreModel user = dBHighScoreModel.convertCampaignBasketBallStatsToModel(PlayerData.instance.CampaignGameStats);

        if (GameObject.FindGameObjectWithTag("database") != null)//&& basketBallStats.TimePlayed > 60)
        {
            DBConnector.instance.savePlayerGameStats(user);
            // if username is logged in
            if (!string.IsNullOrEmpty(GameOptions.userName) && GameOptions.userid != 0)
            {
                StartCoroutine(APIHelper.PostHighscore(user));
            }
            // if user not logged in, set submitted score to false
            else
            {
                DBHelper.instance.setGameScoreSubmitted(user.Scoreid, false);
            }
        }
        Destroy(PlayerData.instance.GetComponent<GameStats>());
        PlayerData.instance.CampaignGameStats = PlayerData.instance.gameObject.AddComponent<GameStats>();
    }

    private void LoadData()
    {
        EndRoundData.levelsList = GameOptions.levelsList;
        EndRoundData.currentLevelIndex = GameOptions.levelSelectedIndex;
        if (currentWinnerisCpu)
        {
            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayerWinImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.currentRoundPlayerLoserImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "CPU";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "Player 1";
        }
        if (!currentWinnerisCpu || tieGame)
        {
            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.currentRoundPlayerWinnerImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayerLoseImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "Player 1";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "CPU";
        }
        if(tieGame)
        {
            EndRoundUIObjects.instance.winnerText.text = "tie";
            EndRoundUIObjects.instance.loserText.text = "tie";
        }

        if (currentWinnerisCpu && !tieGame && EndRoundData.numberOfContinues > 0)
        {
            GameOptions.levelSelectedIndex--;
            EndRoundUIObjects.instance.nextRoundText.text = !tieGame ? "Try Again" : "Tie Game";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            EndRoundUIObjects.instance.continueNumber.text = EndRoundData.numberOfContinues.ToString();
            nextLevelName =
            EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDescription;
        }
        if (!currentWinnerisCpu && !tieGame)
        {
            EndRoundUIObjects.instance.nextRoundText.text = !tieGame ? "Start" : "Tie Game";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            nextLevelName = EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex].LevelDescription;
        }

        EndRoundUIObjects.instance.continueNumber.text = EndRoundData.numberOfContinues.ToString();
        EndRoundUIObjects.instance.currentRoundWinnerScore.text = EndRoundData.currentRoundWinnerScore.ToString();
        EndRoundUIObjects.instance.currentRoundLoserScore.text = EndRoundData.currentRoundLoserScore.ToString();

        Debug.Log("games : " + PlayerData.instance.CampaignGameStats.campaignGamesPlayed);
        Debug.Log("standings : " + PlayerData.instance.CampaignGameStats.campaignWins + " - "+ PlayerData.instance.CampaignGameStats.campaignLosses+ " - "+ PlayerData.instance.CampaignGameStats.campaignTies);
    }

    public void pressNext()
    {

        if (finalLevel)
        {
            if (!isGameSaved) { saveGame(); }
            SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
        }
        if (tieGame)
        {
            string currentlevelName = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDescription;
            SceneManager.LoadScene(currentlevelName);
        }
        if (EndRoundData.numberOfContinues > 0 && currentWinnerisCpu)
        {
            if (!tieGame) { EndRoundData.numberOfContinues--;}
            SceneManager.LoadScene(nextLevelName);
        }
        if (!currentWinnerisCpu)
        {
            SceneManager.LoadScene(nextLevelName);
        }
    }

    public void pressStartMenu()
    {
        if(!isGameSaved) { saveGame(); }
        SceneManager.LoadScene(Constants.SCENE_NAME_level_00_start);
    }
    public void pressQuit()
    {
        if (!isGameSaved) { saveGame(); }
        Application.Quit();
    }
}
