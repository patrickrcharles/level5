using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TeamUtility.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class GameLevelManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts

    [SerializeField]
    GameObject _player;
    private PlayerController _playerState;
    private Animator _anim;

    Vector3 previousPlayerPosition;
    Quaternion previousPlayerRotation;

    [SerializeField]
    bool gameOver;

    bool startGame;
    bool locked;
    bool paused;
    //bool reloadLevel;
    //bool showScore;

    string currentSceneName;
    //[SerializeField]
    //GameObject startMenuMusicObject;

    //[SerializeField]
    //float totalMoney;

    //public bool playerIsEnemy;
    //public Text displayPlayerLives;
    //public GameObject backgroundFade;
    //public GameObject pauseObject;

    //private AudioSource[] allAudioSources;

    //BasketBall objects
    BasketBall _basketballState;
    GameObject _basketball;
    GameObject basketballPrefab;


    //spawn locations
    GameObject basketballSpawnLocation;
    GameObject playerSpawnLocation;
    //[SerializeField]
    //GameObject player_spawn;

    [SerializeField]
    private int playerCount;

    [SerializeField] private int basketballCount;

    public static GameLevelManager instance;

    private GameObject playerClone;

    //List<string> scenes = new List<string>();

    [SerializeField]
    private GameObject npcObject;


    void Awake()
    {
        // initialize game manger player references
        instance = this;

        // if player selected is not null / player not selecetd
        if(!string.IsNullOrEmpty( GameOptions.playerSelected)){
            string playerPrefabPath = "Prefabs/characters/players/player_" + GameOptions.playerSelected.ToString();
            playerClone = Resources.Load("Prefabs/characters/players/player_" + GameOptions.playerSelected.ToString()) as GameObject;
        }
        //allAudioSources = FindObjectsOfType<AudioSource>();
        //Application.targetFrameRate = 60;

        if (!getCurrentSceneName().StartsWith("start")) { InitializePlayer(); }
        // player + ball spawn locations

        playerSpawnLocation = GameObject.Find("player_spawn_location");
        basketballSpawnLocation = GameObject.Find("ball_spawn_location");

        if (GameObject.FindWithTag("Player") == null)
        {
            Instantiate(playerClone, playerSpawnLocation.transform.position, Quaternion.identity);
        }
        if(GameObject.FindWithTag("basketball") == null)
        {
            basketballPrefab = Resources.Load("Prefabs/objects/basketball_nba") as GameObject;
            Instantiate(basketballPrefab, basketballSpawnLocation.transform.position, Quaternion.identity);
        }
    }

    void Start()
    {
        locked = false;

        //set up player/basketball read only references for use in other classes

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = Player.GetComponent<PlayerController>();
        _anim = Player.GetComponentInChildren<Animator>();
        _basketball = GameObject.FindWithTag("basketball");
        _basketballState = _basketball.GetComponent<BasketBall>();

        InitializePlayer();

        // if an npc is in scene, disable the npc if it is the player selected
        //* this is specific to flash right now
        npcObject = GameObject.FindGameObjectWithTag("auto_npc");

        if ( !string.IsNullOrEmpty(GameOptions.playerSelected) 
             && GameOptions.playerSelected.Contains("flash") 
             && npcObject != null )
        {
            npcObject.SetActive(false);
        }
        GameOptions.printCurrentValues();
    }


    void Update()
    {
        ////close app
        //if ((InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift)) 
        //    && InputManager.GetKeyDown(KeyCode.Q))
        //{
        //    Quit();
        //}

        ////reload start
        //if ((InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift))
        //    && InputManager.GetKeyDown(KeyCode.P))
        //{
        //    SceneManager.LoadScene("level_00_start");
        //}

        ////pause ESC, submit, cancel
        //if (InputManager.GetButtonDown("Submit")
        //    || InputManager.GetButtonDown("Cancel")
        //    || InputManager.GetKeyDown(KeyCode.Escape))
        //{
        //    paused = TogglePause();
        //}
        // reload scene 4+2+0

        //turn on : toggle run, shift +1
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha1)
            && !locked)
        {
            locked = true;
            PlayerState.toggleRun();
            locked = false;
        }

        //turn off accuracy shift +2
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha2)
            && !locked)
        {
            locked = true;
            _basketballState.toggleAddAccuracyModifier();
            locked = false;
        }
        
        //turn off stats, shift +3
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha3)
            && !locked)
        {
            locked = true;
            BasketBall.instance.toggleUiStats(); 
            locked = false;
        }       
        
        //toggle pull ball to player, shift + 4
        if (InputManager.GetKey(KeyCode.LeftShift)
            && InputManager.GetKeyDown(KeyCode.Alpha4)
            && !locked)
        {
            locked = true;
            callBallToPlayer.instance.toggleCallBallToPlayer(); 
            locked = false;
        }

        ////run stat analysis
        //if (InputManager.GetKey(KeyCode.LeftShift)
        //    && InputManager.GetKeyDown(KeyCode.Alpha0)
        //    && !locked)
        //{
        //    locked = true;
        //    _basketballState.testConclusions.getDataFromList();
        //    _basketballState.testConclusions.printConclusions();
        //    locked = false;
        //}
    }

    // set up player references that other scripts use
    // game manager provides read only links to player object and player states
    public void InitializePlayer()
    {
        //Debug.Log("initialize player");
        //_player = GameObject.FindGameObjectWithTag("Player");
        //_playerState = player.GetComponent<playercontrollerscript>();
        //_anim = player.GetComponentInChildren<Animator>();
    }


    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    //bool TogglePause()
    //{
    //    if (Time.timeScale == 0f)
    //    {
    //        //gameManager.instance.backgroundFade.SetActive(false);
    //        Time.timeScale = 1f;
    //        //resumeAllAudio();
    //        return (false);
    //    }
    //    else
    //    {
    //        //gameManager.instance.backgroundFade.SetActive(true);
    //        Time.timeScale = 0f;
    //        //pauseAllAudio();
    //        return (true);
    //    }
    //}

    //void pauseAllAudio()
    //{
    //    foreach (AudioSource audioS in allAudioSources)
    //    {
    //        //audioS.Stop();
    //        audioS.Pause();
    //    }
    //}

    //void resumeAllAudio()
    //{
    //    foreach (AudioSource audioS in allAudioSources)
    //    {
    //        //audioS.Stop();
    //        audioS.UnPause();
    //    }
    //}

    private void Quit()
    {
        Application.Quit();
    }

    public GameObject Player
    {
        get { return _player; }
    }
    public PlayerController PlayerState
    {
        get { return _playerState; }
    }
    public Animator Anim
    {
        get { return _anim; }
    }

    public BasketBall BasketballState
    {
        get => _basketballState;
    }

    public GameObject Basketball
    {
        get => _basketball;
    }


    public bool GameOver
    {
        get => gameOver;
        set => gameOver = value;
    }
}
