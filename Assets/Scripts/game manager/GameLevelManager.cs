using UnityEngine;
using UnityEngine.SceneManagement;
using static TeamUtility.IO.InputManager;
#if UNITY_EDITOR
#endif

public class GameLevelManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts
    private GameObject _player;
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

    public BasketBall Basketball;
    public GameObject BasketballObject;

    //[SerializeField]
    //GameObject player_spawn;

    //[SerializeField]
    //private int _playerCount;

    //[SerializeField] private int _basketballCount;

    private GameObject _playerClone;
    private GameObject[] _npcObjects;
    const string basketBallPrefabPath = "Prefabs/basketball/basketball_nba";

    public static GameLevelManager Instance;

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

        //if (!GetCurrentSceneName().StartsWith("start")) { InitializePlayer(); }
        //// player + ball spawn locations

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

        //InitializePlayer();
        //GameOptions.printCurrentValues();

        // if an npc is in scene, disable the npc if it is the player selected
        //* this is specific to flash right now
        _npcObjects = GameObject.FindGameObjectsWithTag("auto_npc");
        string playerName = GameObject.FindWithTag("Player").name;
        foreach (var npc in _npcObjects)
        {
            if (npc.name.Contains(playerName))
            {
                npc.SetActive(false);
            }
        }
    }

    private void Update()
    {
        //turn on : toggle run, shift +1
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha1)
            && !_locked)
        {
            _locked = true;
            PlayerState.toggleRun();
            _locked = false;
        }
        ////toggle pull ball to player, shift + 2
        //if (GetKey(KeyCode.LeftShift)
        //    && GetKeyDown(KeyCode.Alpha2)
        //    && !_locked)
        //{
        //    _locked = true;
        //    CallBallToPlayer.instance.toggleCallBallToPlayer(); 
        //    _locked = false;
        //}

        //turn off stats, shift + 2
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha2)
            && !_locked)
        {
            _locked = true;
            BasketBall.instance.toggleUiStats(); 
            _locked = false;
        }

        ////turn off accuracy shift +2
        //if (GetKey(KeyCode.LeftShift)
        //    && GetKeyDown(KeyCode.Alpha3)
        //    && !_locked)
        //{
        //    _locked = true;
        //    Basketball.toggleAddAccuracyModifier();
        //    _locked = false;
        //}
    }

    // set up player references that other scripts use
    // game manager provides read only links to player object and player states
    //public void InitializePlayer()
    //{
    //    //Debug.Log("initialize player");
    //    //_player = GameObject.FindGameObjectWithTag("Player");
    //    //_playerState = player.GetComponent<playercontrollerscript>();
    //    //_anim = player.GetComponentInChildren<Animator>();
    //

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

    [SerializeField]
    public bool GameOver { get; set; }
}
