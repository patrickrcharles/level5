using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TeamUtility.IO;
using UnityEditor;

public class GameLevelManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts

    [SerializeField]
    GameObject _player;
    private playercontrollerscript _playerState;
    private Animator _anim;

    Vector3 previousPlayerPosition;
    Quaternion previousPlayerRotation;

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
    BasketBall basketballState;
    GameObject basketball;

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

    List<string> scenes = new List<string>();

    [SerializeField]
    private GameObject flashObject;

    void Awake()
    {
        Debug.Log("gm : awake()");
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
            basketball = Resources.Load("Prefabs/objects/basketball_nba") as GameObject;
            Instantiate(basketball, basketballSpawnLocation.transform.position, Quaternion.identity);
        }


    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = player.GetComponent<playercontrollerscript>();
        _anim = player.GetComponentInChildren<Animator>();

        basketballState = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
        locked = false;

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (scene.enabled)
            {
                scenes.Add(scene.path);
            }
        }

        foreach (var scene in scenes)
        {
            Debug.Log(scene);
        }

        InitializePlayer();

        flashObject = GameObject.Find("flash_auto_play");
        Debug.Log("=================================== selcted player : " + GameOptions.playerSelected );
        Debug.Log("=================================== flashObject : " + flashObject);
        if ( !string.IsNullOrEmpty(GameOptions.playerSelected) 
            &&GameOptions.playerSelected.Contains("flash") 
            && flashObject != null )
        {
            flashObject.SetActive(false);
        }

    }


    void Update()
    {
        //close app
        if ((InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift)) 
            && InputManager.GetKeyDown(KeyCode.Q))
        {
            Quit();
        }

        //reload start
        if ((InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift))
            && InputManager.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("level_start");
        }

        //pause ESC, submit, cancel
        if (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Cancel")
            || InputManager.GetKeyDown(KeyCode.Escape))
        {
            paused = TogglePause();
        }
        // reload scene 4+2+0
        if (InputManager.GetKey(KeyCode.Alpha4)
            && InputManager.GetKey(KeyCode.Alpha2)
            && InputManager.GetKey(KeyCode.Alpha0)
            && !locked)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        //turn off accuracy modifer 6+9
        if (InputManager.GetKey(KeyCode.Alpha6)
            && InputManager.GetKeyDown(KeyCode.Alpha9)
            && !locked)
        {
            locked = true;
            basketballState.toggleAddAccuracyModifier();
            locked = false;
        }
    }

    // set up player references that other scripts use
    // game manager provides read only links to player object and player states
    public void InitializePlayer()
    {
        Debug.Log("initialize player");
        //_player = GameObject.FindGameObjectWithTag("Player");
        //_playerState = player.GetComponent<playercontrollerscript>();
        //_anim = player.GetComponentInChildren<Animator>();
    }


    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    bool TogglePause()
    {
        if (Time.timeScale == 0f)
        {
            //gameManager.instance.backgroundFade.SetActive(false);
            Time.timeScale = 1f;
            //resumeAllAudio();
            return (false);
        }
        else
        {
            //gameManager.instance.backgroundFade.SetActive(true);
            Time.timeScale = 0f;
            //pauseAllAudio();
            return (true);
        }
    }

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

    public GameObject player
    {
        get { return _player; }
    }
    public playercontrollerscript playerState
    {
        get { return _playerState; }
    }
    public Animator anim
    {
        get { return _anim; }
    }
}
