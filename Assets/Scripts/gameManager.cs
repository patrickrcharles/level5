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

    public bool playerMadeShot;

    string currentSceneName;
    [SerializeField]
    GameObject startMenuMusicObject;

    [SerializeField]
    float totalMoney;

    public bool playerIsEnemy;
    public Text displayPlayerLives;
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

    public static gameManager instance;
    public bool paused = false;

    private AudioSource[] allAudioSources;

    public GameObject basketball;
    public basketBall basketballState;
    public GameObject basketballSpawnLocation;

    public bool locked;


    private void Awake()
    {
        // initialize game manger player references
        instance = this;
        allAudioSources = FindObjectsOfType<AudioSource>();

        //Application.targetFrameRate = 60;
        if (!getCurrentSceneName().StartsWith("start")) { initializePlayer(); }
        basketball = Resources.Load("Prefabs/objects/basketball 1") as GameObject;
        Instantiate(basketball, basketballSpawnLocation.transform.position, Quaternion.identity);
    }

    private void Start()
    {
        pauseObject.SetActive(false);
        pauseMenu.instance.enabled = false;
        locked = false;
        basketballState = GameObject.FindWithTag("basketball").GetComponent<basketBall>();
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
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = player.GetComponent<playercontrollerscript>();
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
    public void updatePlayerLives()
    {
        displayPlayerLives.text = "x " + (playerLives).ToString();
    }


    bool togglePause()
    {
        if (Time.timeScale == 0f)
        {
            gameManager.instance.backgroundFade.SetActive(false);
            Time.timeScale = 1f;
            resumeAllAudio();
            return (false);
        }
        else
        {
            gameManager.instance.backgroundFade.SetActive(true);
            Time.timeScale = 0f;
            pauseAllAudio();
            return (true);
        }
    }

    void pauseAllAudio()
    {
        foreach (AudioSource audioS in allAudioSources)
        {
            //audioS.Stop();
            audioS.Pause();
        }
    }

    void resumeAllAudio()
    {
        foreach (AudioSource audioS in allAudioSources)
        {
            //audioS.Stop();
            audioS.UnPause();
        }
    }

    private void Quit()
    {
        Application.Quit();
    }
}
