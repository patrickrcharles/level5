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
    private GameObject _cheerleaderPrefab;

    //spawn locations
    private GameObject _basketballSpawnLocation;
    private GameObject _playerSpawnLocation;
    private GameObject _cheerleaderSpawnLocation;

    public BasketBall Basketball;
    public GameObject BasketballObject;

    private GameObject _playerClone;
    private GameObject _cheerleaderClone;
    [SerializeField]
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

        _playerSpawnLocation = GameObject.Find("player_spawn_location");
        _basketballSpawnLocation = GameObject.Find("ball_spawn_location");
        _cheerleaderSpawnLocation = GameObject.Find("cheerleader_spawn_location");

        if (GameObject.FindWithTag("Player") == null)
        {
            Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
        }
        if(GameObject.FindWithTag("basketball") == null)
        {
            _basketballPrefab = Resources.Load(basketBallPrefabPath) as GameObject;
            Instantiate(_basketballPrefab, _basketballSpawnLocation.transform.position, Quaternion.identity);
        }
        if (GameObject.FindWithTag("cheerleader") == null)
        {
            string cheerleaderPrefabPath = "Prefabs/characters/auto_players/cheerleader_" + GameOptions.cheerleaderObjectName;
            _cheerleaderClone = Resources.Load(cheerleaderPrefabPath) as GameObject;

            Instantiate(_cheerleaderClone, _cheerleaderSpawnLocation.transform.position, Quaternion.identity);
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
        GameOptions.printCurrentValues();

        // if an npc is in scene, disable the npc if it is the player selected
        //* this is specific to flash right now
        _npcObjects = GameObject.FindGameObjectsWithTag("auto_npc");
        string playerName = GameOptions.playerObjectName;
        foreach (var npc in _npcObjects)
        {
            if ( !string.IsNullOrEmpty(playerName) && npc.name.Contains(playerName) )
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

        //turn off stats, shift + 2
        if (GetKey(KeyCode.LeftShift)
            && GetKeyDown(KeyCode.Alpha2)
            && !_locked)
        {
            _locked = true;
            BasketBall.instance.toggleUiStats(); 
            _locked = false;
        }
    }

    private string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    private void Quit()
    {
        Application.Quit();
    }

    public GameObject Player => _player;

    public PlayerController PlayerState => _playerState;

    public Animator Anim { get; private set; }

    public bool GameOver { get; set; }
}
