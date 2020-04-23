using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TeamUtility.IO;

public class gameManager : MonoBehaviour
{
    //public int playerLives;
    Vector3 previousPlayerPosition;
    Quaternion previousPlayerRotation;
    bool gameOver;
    bool reloadLevel;
    bool showScore;
    bool startGame;

    //public bool playerMadeShot;

    string currentSceneName;
    //[SerializeField]
    //GameObject startMenuMusicObject;

    //[SerializeField]
    //float totalMoney;

    //public bool playerIsEnemy;
    //public Text displayPlayerLives;
    //public GameObject backgroundFade;
    //public GameObject pauseObject;

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

    public static gameManager instance;
    bool paused = false;

    //private AudioSource[] allAudioSources;

    //BasketBall objects
    GameObject basketball;
    BasketBall basketballState;
    [SerializeField]
    GameObject basketballSpawnLocation;

    //player spawn location
    [SerializeField]
    GameObject playerSpawnLocation;
    //[SerializeField]
    //GameObject player_spawn;


    bool locked;

    private void Awake()
    {
        Debug.Log("gm : awake()");
        // initialize game manger player references
        instance = this;
        //allAudioSources = FindObjectsOfType<AudioSource>();

        //Application.targetFrameRate = 60;
        if (!getCurrentSceneName().StartsWith("start")) { initializePlayer(); }

                // player + ball spawn locations
        playerSpawnLocation = GameObject.Find("player_spawn_location");
        basketballSpawnLocation = GameObject.Find("ball_spawn_location");

        //Debug.Log("initialize player");
        //initializePlayer();
        //_player = Resources.Load("Prefabs/Player_aba") as GameObject;
        //Instantiate(_player, playerSpawnLocation.transform.position, Quaternion.identity);
        //_playerState = player.GetComponent<playercontrollerscript

        //load and spawn basketbll prefab
        basketball = Resources.Load("Prefabs/objects/basketball 1") as GameObject;
        Instantiate(basketball, basketballSpawnLocation.transform.position, Quaternion.identity);
        basketballState = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
    }

    private void Start()
    {
        Debug.Log("gm : start()");
        //pauseObject.SetActive(false);
        //pauseMenu.instance.enabled = false;
        locked = false;
        _playerState = player.GetComponent<playercontrollerscript>();
        _anim = player.GetComponentInChildren<Animator>();
        basketballState = GameObject.FindWithTag("basketball").GetComponent<BasketBall>();
    }

    private void Update()
    {
        if ((InputManager.GetKey(KeyCode.LeftShift) || InputManager.GetKey(KeyCode.RightShift)) 
            && InputManager.GetKeyDown(KeyCode.P))
        {
            Quit();
        }
        if (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Cancel")
            || InputManager.GetKeyDown(KeyCode.Escape))
        {
            paused = togglePause();
        }
        if (InputManager.GetKey(KeyCode.Alpha4)
            && InputManager.GetKey(KeyCode.Alpha2)
            && InputManager.GetKey(KeyCode.Alpha0)
            && !locked)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        if (InputManager.GetKey(KeyCode.Alpha6)
            && InputManager.GetKeyDown(KeyCode.Alpha9)
            && !locked)
        {
            locked = true;
            basketballState.toggleAddAccuracyModifier();
            locked = false;
        }
    }

    public void initializePlayer()
    {
        Debug.Log("initialize player");
        //_player = Resources.Load("Prefabs/Player_aba") as GameObject;
        //Instantiate(_player, playerSpawnLocation.transform.position, Quaternion.identity);

        _player = GameObject.FindGameObjectWithTag("Player");
        _anim = _player.GetComponent<Animator>();
    }

    public void resetSceneVariables()
    {
        gameOver = false;
        showScore = false;
    }
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
    //public void updatePlayerLives()
    //{
    //    displayPlayerLives.text = "x " + (playerLives).ToString();
    //}


    bool togglePause()
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
}
