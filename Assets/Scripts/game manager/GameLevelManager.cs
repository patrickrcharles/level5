using UnityEngine;
using UnityEngine.SceneManagement;
using static TeamUtility.IO.InputManager;
#if UNITY_EDITOR
#endif

public class GameLevelManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts
    [SerializeField] private GameObject _player;
    private PlayerController _playerState;

    //private AudioSource[] allAudioSources;

    private Vector3 _previousPlayerPosition;
    private Quaternion _previousPlayerRotation;

    private bool _startGame;
    private bool _locked;
    private bool _paused;

    private string _currentSceneName;
    //private AudioSource[] allAudioSources;

    //BasketBall objects
    private GameObject _basketballPrefab;

    //spawn locations
    private GameObject _basketballSpawnLocation;
    private GameObject _playerSpawnLocation;

    [SerializeField]
    public BasketBall Basketball;
    [SerializeField]
    public GameObject BasketballObject;

    //[SerializeField]
    //GameObject player_spawn;

    [SerializeField]
    private int _playerCount;

    [SerializeField] private int _basketballCount;

    public static GameLevelManager Instance;

    private GameObject _playerClone;

    //List<string> scenes = new List<string>();

    [SerializeField]
    private GameObject[] _npcObjects;

    const string basketBallPrefabPath = "Prefabs/basketball/basketball_nba";


    private void Awake()
    {
        // initialize game manger player references
        Instance = this;

        // if player selected is not null / player not selected
        if(!string.IsNullOrEmpty( GameOptions.playerObjectName)){
            string playerPrefabPath = "Prefabs/characters/players/player_" + GameOptions.playerObjectName;
            _playerClone = Resources.Load(playerPrefabPath) as GameObject;
        }
        //allAudioSources = FindObjectsOfType<AudioSource>();
        //Application.targetFrameRate = 60;

        if (!GetCurrentSceneName().StartsWith("start")) { InitializePlayer(); }
        // player + ball spawn locations

        _playerSpawnLocation = GameObject.Find("player_spawn_location");
        _basketballSpawnLocation = GameObject.Find("ball_spawn_location");

        if (GameObject.FindWithTag("Player") == null)
        {
            Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
        }
        if(GameObject.FindWithTag("basketball") == null)
        {
            _basketballPrefab = Resources.Load(basketBallPrefabPath) as GameObject;
            Instantiate(_basketballPrefab, _basketballSpawnLocation.transform.position, Quaternion.identity);
        }

        BasketballObject = GameObject.FindWithTag("basketball");
        Basketball = BasketballObject.GetComponent<BasketBall>();
    }

    private void Start()
    {
        _locked = false;

        //set up player/basketball read only references for use in other classes

        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = Player.GetComponent<PlayerController>();
        Anim = Player.GetComponentInChildren<Animator>();

        InitializePlayer();
        GameOptions.printCurrentValues();
        // if an npc is in scene, disable the npc if it is the player selected
        //* this is specific to flash right now

        _npcObjects = GameObject.FindGameObjectsWithTag("auto_npc");
        string playerName = GameObject.FindWithTag("Player").name;
        foreach (var npc in _npcObjects)
        {
            //if (!string.IsNullOrEmpty(GameOptions.playerObjectName)
            //    && GameOptions.playerObjectName.Contains("flash")
            //    && npc != null)
            //{
            //    npc.SetActive(false);
            //}
            Debug.Log("player : "+ playerName + "   npc : "+ npc.name);
            if (npc.name.Contains(playerName))
            {
                npc.SetActive(false);
            }
        }
    }


    private void Update()
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
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha1)
            && !_locked)
        {
            _locked = true;
            PlayerState.toggleRun();
            _locked = false;
        }
        //toggle pull ball to player, shift + 4
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha2)
            && !_locked)
        {
            _locked = true;
            CallBallToPlayer.instance.toggleCallBallToPlayer(); 
            _locked = false;
        }
        //turn off accuracy shift +2
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha3)
            && !_locked)
        {
            _locked = true;
            Basketball.toggleAddAccuracyModifier();
            _locked = false;
        }
        
        //turn off stats, shift +3
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha4)
            && !_locked)
        {
            _locked = true;
            BasketBall.instance.toggleUiStats(); 
            _locked = false;
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

    private string GetCurrentSceneName()
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

    public GameObject Player => _player;

    public PlayerController PlayerState => _playerState;

    public Animator Anim { get; private set; }


    public bool GameOver { get; set; }
}
