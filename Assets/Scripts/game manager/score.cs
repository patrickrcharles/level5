using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TeamUtility.IO;

public class score : MonoBehaviour
{
    public GameObject timer;

    public List<int> enemiesKilled;
    public List<int> enemypoints;

    public bool showScore;
    public bool restartLevel;

    GUIStyle guiStyle = new GUIStyle();
    public static score instance;
    public int totalkills;

    string playerName;
    string level;
    int numEnemies;
    string time;
    int kills;
    int currentScore = 0;
    string date;
    int livesUsed;
    bool insertScoreToDB = false;
    public bool rakesDisabled = false;
    bool notlocked = true;

    // Use this for initialization
    void Start()
    {
        livesUsed = gameManager.instance.playerLives;
        //Debug.Log("score. cs - Start() ");

    }
    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        // 'press chooseOpponent to display/update score
        if (showScore && notlocked)
        {
            notlocked = false;
            continueGame.instance.setGuiOn(false);

            gameManager.instance.gameOver = true;
            gameManager.instance.backgroundFade.SetActive(true);
            gameManager.instance.disableEnemyUI();

            insertScoreToDB = true;
            timer.GetComponent<countdown>().setScoreTime();
            playerName = "PRC";
            updateDBHighScore();
            gameManager.instance.highScoreManger.insertScore
                (playerName, level, numEnemies, time, totalkills, currentScore, date, livesUsed);

            //StartCoroutine( Wait(3f));
        }
        // press chooseOpponent to restart level
        if (restartLevel)
        {

            // reset current score
            currentScore = 0;
            //reset scene vars. gameover and showScore in gamemanager. will disable score overlay.
            //could destroy levelmanager object but i dont think i need to since it's the same level

            //Debug.Log("reload scene");
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            gameManager.instance.resetSceneVariables();
        }
    }

    private void OnGUI()
    {
        if (showScore)
        {
            gameManager.instance.backgroundFade.SetActive(true);
            //gameManager.instance.disableEnemyUI();
            //disables enemy healthUI because it was on top the fade background and makes score unreadable

            guiStyle.fontSize = 20;
            guiStyle.normal.textColor = Color.white;
            float xPos = Screen.width * .4f;
            float yPos = Screen.height * .17f;
            int xSize = 500;
            int ySize = 300;

            GUILayout.BeginArea(new Rect(xPos, yPos, xSize, ySize));
            GUILayout.Label("enemies killed", guiStyle);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(xPos + 200, yPos, xSize, ySize));
            GUILayout.Label("times killed", guiStyle);
            GUILayout.EndArea();

            for (int i = 0; i < enemyList.instance.enemyName.Capacity - 1; i++)
            {
                //Debug.Log(" enemylist.nemyname.capacity : " + enemyList.instance.enemyName.Capacity);
                guiStyle.fontSize = 15;
                guiStyle.normal.textColor = Color.white;


                if (enemiesKilled[i] > 0)
                {
                    yPos += 20;
                    GUILayout.BeginArea(new Rect(xPos, yPos, xSize, ySize));
                    GUILayout.Label(enemyList.instance.enemyName[i].name, guiStyle);
                    GUILayout.EndArea();


                    GUILayout.BeginArea(new Rect(xPos + 250, yPos, xSize, ySize));
                    GUILayout.Label(enemiesKilled[i].ToString(), guiStyle);
                    GUILayout.EndArea();
                }
            }

            yPos += 20;
            GUILayout.BeginArea(new Rect(xPos, yPos, xSize, ySize));
            GUILayout.Label("total kills", guiStyle);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(xPos + 250, yPos, xSize, ySize));
            GUILayout.Label(totalkills.ToString(), guiStyle);
            GUILayout.EndArea();

            yPos += 20;
            GUILayout.BeginArea(new Rect(xPos, yPos, xSize, ySize));
            GUILayout.Label("total score", guiStyle);
            GUILayout.EndArea();

            GUILayout.BeginArea(new Rect(xPos + 250, yPos, xSize, ySize));
            GUILayout.Label(currentScore.ToString(), guiStyle);
            GUILayout.EndArea();

            yPos += 40;
            GUILayout.BeginArea(new Rect(xPos, yPos, xSize, ySize));
            GUILayout.Label("press enter to restart level", guiStyle);
            GUILayout.EndArea();
        }
    }

    void updateDBHighScore()
    {
        getlevel();
        getnumEnemies();
        getDate();
        calculateTotalScore();
    }

    public void setTime(string tim)
    {
        time = tim;
    }

    void getname()
    {
        // input player name
    }
    public void getlevel()
    {
        level = SceneManager.GetActiveScene().name;
        //Debug.Log("current level : "+ SceneManager.GetActiveScene().name);
    }
    void getnumEnemies()
    {
        if (enemySpawner.instance.gameMode1)
        {
            numEnemies = enemySpawner.instance.maxEnemiesDemo1;
        }
        else
        {
            numEnemies = enemySpawner.instance.maxEnemiesDemo2;
        }
    }

    public string getTime()
    {
        return time;
    }

    int getKills()
    {
        return totalkills;
    }

    public void getDate()
    {
        PlayerPrefs.SetString("date time", System.DateTime.Now.ToString("MM/dd/yyyy"));
        date = PlayerPrefs.GetString("date time");
    }


    public void calculateCurrentScore(int id)
    {
        currentScore += enemypoints[id];
    }
    public void calculateTotalScore()
    {

        currentScore = Mathf.FloorToInt(currentScore * (numEnemies * .35f) * (totalkills * .15f));
        currentScore -= calculatePointDeduction(currentScore);
    }

    public void setCurrentScore(int score)
    {
        currentScore = score;
    }

    int calculatePointDeduction(int currScore)
    {

        if (livesUsed == 2)
        {
            return Mathf.FloorToInt(0.25f * currScore);
        }
        if (livesUsed == 3)
        {
            return Mathf.FloorToInt(0.35f * currScore);
        }
        if (livesUsed > 3)
        {
            return Mathf.FloorToInt(livesUsed * 0.11f * currScore);
        }
        else
            return 0;
    }
}
