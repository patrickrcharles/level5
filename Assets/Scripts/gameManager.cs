using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TeamUtility.IO;

public class gameManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts
    private GameObject _player;
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



    public static gameManager instance;

    private void Awake()
    {
        //Debug.Log("gm : awake()");
        // initialize game manger player references
        instance = this;

        //allAudioSources = FindObjectsOfType<AudioSource>();
        //Application.targetFrameRate = 60;


        if (!getCurrentSceneName().StartsWith("start")) { InitializePlayer(); }
        // player + ball spawn locations
        playerSpawnLocation = GameObject.Find("player_spawn_location");
        basketballSpawnLocation = GameObject.Find("ball_spawn_location");

        //Debug.Log("initialize player");
        //initializePlayer();
        //_player = Resources.Load("Prefabs/Player_aba") as GameObject;
        //Instantiate(_player, playerSpawnLocation.transform.position, Quaternion.identity);
        //_playerState = player.GetComponent<playercontrollerscript

        //load and spawn basketbll prefab
        basketball = Resources.Load("Prefabs/objects/basketball_nba") as GameObject;
        Instantiate(basketball, basketballSpawnLocation.transform.position, Quaternion.identity);
        basketballState = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
    }

    private void Start()
    {
        //Debug.Log("gm : start()");

        locked = false;
        InitializePlayer();

        basketballState = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
    }

    private void Update()
    {
        //close app
        if ((InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift)) 
            && InputManager.GetKeyDown(KeyCode.P))
        {
            Quit();
        }
        //pause
        if (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Cancel")
            || InputManager.GetKeyDown(KeyCode.Escape))
        {
            paused = TogglePause();
        }
        // reload scene
        if (InputManager.GetKey(KeyCode.Alpha4)
            && InputManager.GetKey(KeyCode.Alpha2)
            && InputManager.GetKey(KeyCode.Alpha0)
            && !locked)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        //turn off accuracy modifer
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
        //_player = Resources.Load("Prefabs/Player_aba") as GameObject;
        //Instantiate(_player, playerSpawnLocation.transform.position, Quaternion.identity);

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = player.GetComponent<playercontrollerscript>();
        _anim = player.GetComponentInChildren<Animator>();
    }

    //public void resetSceneVariables()
    //{
    //    gameOver = false;
    //    showScore = false;
    //}
    //public void disableEnemyUI()
    //{
    //    GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemyHealthUI");

    //    foreach (GameObject enemy in enemySpawner.instance.enemies)
    //    {
    //        enemy.SetActive(false);
    //    }
    //}

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
