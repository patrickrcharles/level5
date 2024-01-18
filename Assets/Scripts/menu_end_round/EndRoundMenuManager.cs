using Assets.Scripts.database;
using Assets.Scripts.restapi;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EndRoundMenuManager : MonoBehaviour
{
    [SerializeField]
    private string currentHighlightedButton;
    [SerializeField]
    private string previousHighlightedButton;

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

    const string nextButton = "next_button";
    const string startMenuButton = "start_menu_button";
    const string quitButton = "quit_button";

    PlayerControls controls;

    public static EndRoundMenuManager instance;
    private bool buttonPressed;

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
    }

    private void Awake()
    {
        instance = this;
        controls = new PlayerControls();
    }
    void Start()
    {
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

        // test trip tie
        //currentWinnerScore = currentLoserScore;
        
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

#if UNITY_STANDALONE || UNITY_EDITOR
    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject != null)
        {
            currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name;
        }

        // ================================== navigation =====================================================================

        //#if UNITY_STANDALONE || UNITY_EDITOR
        // high scores table button selected
        if (currentHighlightedButton.Equals(nextButton) 
            && !buttonPressed
            && controls.UINavigation.Submit.triggered)
        {
            buttonPressed = true;
            pressNext();
                
        }
        if (currentHighlightedButton.Equals(startMenuButton)
            && !buttonPressed
            && controls.UINavigation.Submit.triggered)
        {
            buttonPressed = true;
            pressStartMenu();

        }
        if (currentHighlightedButton.Equals(quitButton)
            && !buttonPressed
            && controls.UINavigation.Submit.triggered)
        {
            buttonPressed = true;
            pressQuit();

        }
        buttonPressed = false;
        //#endif
        // save at end of frame
        previousHighlightedButton = currentHighlightedButton;
    }
#endif
    public void saveGame()
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

     void LoadData()
    {
        EndRoundData.levelsList = GameOptions.levelsList;
        EndRoundData.currentLevelIndex = GameOptions.levelSelectedIndex;
        // cpu wins
        if (currentWinnerisCpu)
        {
            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayerWinImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.currentRoundPlayerLoserImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "CPU";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "Player 1";
        }
        // player wins or tie game
        if (!currentWinnerisCpu || tieGame)
        {
            EndRoundUIObjects.instance.currentRoundWinnerImage.sprite = EndRoundData.currentRoundPlayerWinnerImage;
            EndRoundUIObjects.instance.currentRoundLoserImage.sprite = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayerLoseImage;
            EndRoundUIObjects.instance.currentRoundWinnerIsCpu.text = "Player 1";
            EndRoundUIObjects.instance.currentRoundLoserIsCpu.text = "CPU";
        }
        // tie | text
        if(tieGame)
        {
            GameOptions.levelSelectedIndex--;
            EndRoundUIObjects.instance.winnerText.text = "tie";
            EndRoundUIObjects.instance.loserText.text = "tie";
            EndRoundUIObjects.instance.nextRoundLevel.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDisplayName;
            EndRoundUIObjects.instance.nextRoundOpponent.text = EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].CpuPlayer.GetComponent<CharacterProfile>().PlayerDisplayName;
            nextLevelName =
            EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelObjectName + "_" + EndRoundData.levelsList[EndRoundData.currentLevelIndex - 1].LevelDescription;

        }
        // cpu win | no continues | no tie
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
        // player win | no tie
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

        //Debug.Log("games : " + PlayerData.instance.CampaignGameStats.campaignGamesPlayed);
        //Debug.Log("standings : " + PlayerData.instance.CampaignGameStats.campaignWins + " - "+ PlayerData.instance.CampaignGameStats.campaignLosses+ " - "+ PlayerData.instance.CampaignGameStats.campaignTies);
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
        if (EndRoundData.numberOfContinues > 0 && currentWinnerisCpu && !tieGame)
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
