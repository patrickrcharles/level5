using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TeamUtility.IO;

public class gameManager : MonoBehaviour
{
    public int playerLives;
    public Vector3 previousPlayerPosition;
    public Quaternion previousPlayerRotation;
    public bool gameOver;
    public bool reloadLevel;
    public bool showScore;
    public bool startGame;

    string currentSceneName;
    [SerializeField]
    GameObject startMenuMusicObject;

    [SerializeField]
    float totalMoney;

    public bool playerIsEnemy;
    public Text displayPlayerLives;
    public highScoreManager highScoreManger;
    public GameObject backgroundFade;
    public GameObject pauseObject;

    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts
    [SerializeField]
    private GameObject _player;
    public GameObject player
    {
        get { return _player; }
    }
    [SerializeField]
    private playercontrollerscript _playerState;
    public playercontrollerscript playerState
    {
        get { return _playerState; }
    }

    [SerializeField]
    private Animator _anim;
    public Animator anim
    {
        get { return _anim; }
    }

    //[SerializeField]
    //private playerAttacks _playerAttacks;
    //public playerAttacks playerAttacks
    //{
    //    get { return _playerAttacks; }
    //}

    // player ammo, guns, etc.
    public static gameManager instance;

    private void Awake()
    {
        // initialize game manger player references
        instance = this;
        //// get player lives
        //if (gameOptions.instance != null)
        //{
        //    playerLives = gameOptions.instance.getOptionNumberOfLives();
        //}
        //else
        //{
        //    playerLives = 3;
        //}

        Application.targetFrameRate = 60;
        if (!getCurrentSceneName().StartsWith("start")) { initializePlayer(); }
    }

    private void Start()
    {
        //pauseObject.SetActive(false);
        pauseMenu.instance.enabled = false;
    }

    private void Update()
    {
        // this can be moved from update() and left on after each update of the text
        //displayPlayerLives.text = "x " + (playerLives).ToString();

        //// 'press chooseOpponent' screen first load for level
        //if ((InputManager.GetButtonDown("Submit") || InputManager.GetButtonDown("Cancel")))
        //    {
        //    Debug.Log("   if ((InputManager.GetButtonDown(Submit))");
        //}

        if ((InputManager.GetButtonDown("Submit") || InputManager.GetButtonDown("Jump")) 
            && !startGame)
        {
            Debug.Log("gameManager.cs :::::");
            Debug.Log("if (InputManager.GetButtonDown(Submit) && !startGame)");
            backgroundFade.SetActive(false);
            startGame = true;
            Time.timeScale = 1;
            pauseMenu.instance.enabled = true;
        }
        
    }

    public void initializePlayer()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = player.GetComponent<playercontrollerscript>();
        _anim = _player.GetComponent<Animator>();
        //_playerAttacks = _player.GetComponent<playerAttacks>();
        /*
        startMenuMusicObject = GameObject.Find("start_menu_music");
        currentSceneName = getCurrentSceneName().ToLower();
        
        // if not a start screen scene, disable music
        if (!currentSceneName.StartsWith("start")
            && GameObject.Find("start_menu_music") != null)
        {
            Destroy(GameObject.Find("start_menu_music"));
        }
        */
    }

    public void resetSceneVariables()
    {
        //Debug.Log("============ reset gamemanger vars");
        gameOver = false;
        showScore = false;
        //GetComponent<enemyAttackQueue>().setEnemiesCount(0);
    }
    public void disableEnemyUI()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemyHealthUI");

        foreach (GameObject enemy in enemySpawner.instance.enemies)
        {
            enemy.SetActive(false);
        }
    }

    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    public void updatePlayerLives()
    {
        displayPlayerLives.text = "x " + (playerLives).ToString();
    }
}
