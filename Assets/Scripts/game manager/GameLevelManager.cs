
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_EDITOR
#endif

public class GameLevelManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts
    private GameObject _player;
    private PlayerController _playerState;

    //private AudioSource[] allAudioSources;

    private bool _locked;

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
    public Vector3 BasketballRimVector;

    private GameObject _playerClone;
    private GameObject _cheerleaderClone;

    GameObject[] _npcObjects;
    GameObject[] _vehicleObjects;


    PlayerControls controls;
    [SerializeField]
    FloatingJoystick joystick;

    const string basketBallPrefabPath = "Prefabs/basketball/basketball_nba";

    public static GameLevelManager instance;

    private void Awake()
    {

        instance = this;
        // mapped controls
        controls = new PlayerControls();

        //ui touch controls
        if (GameObject.FindGameObjectWithTag("joystick") != null)
        {
            joystick = GameObject.FindGameObjectWithTag("joystick").GetComponentInChildren<FloatingJoystick>();
        }

        // if player selected is not null / player not selected
        if (!string.IsNullOrEmpty( GameOptions.playerObjectName)){
            string playerPrefabPath = "Prefabs/characters/players/player_" + GameOptions.playerObjectName;
            _playerClone = Resources.Load(playerPrefabPath) as GameObject;
        }
        // spawn location
        _playerSpawnLocation = GameObject.Find("player_spawn_location");
        _basketballSpawnLocation = GameObject.Find("ball_spawn_location");
        _cheerleaderSpawnLocation = GameObject.Find("cheerleader_spawn_location");

        // if player doesnt exists
        if (GameObject.FindWithTag("Player") == null)
        {
            if (_playerClone != null)
            {    
                Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
            }
            else
            {
                // spawn default character
                string playerPrefabPath = "Prefabs/characters/players/player_drblood" ;
                _playerClone = Resources.Load(playerPrefabPath) as GameObject;
                Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
            }
        }
        // if basketball doesnt exists
        if (GameObject.FindWithTag("basketball") == null)
        {
            _basketballPrefab = Resources.Load(basketBallPrefabPath) as GameObject;
            Instantiate(_basketballPrefab, _basketballSpawnLocation.transform.position, Quaternion.identity);
        }
        // cheerleader doesnt exists
        if (GameObject.FindWithTag("cheerleader") == null 
            && GameOptions.cheerleaderObjectName != null)
        {
            string cheerleaderPrefabPath = "Prefabs/characters/auto_players/cheerleader_" + GameOptions.cheerleaderObjectName;
            _cheerleaderClone = Resources.Load(cheerleaderPrefabPath) as GameObject;

            if (_cheerleaderClone != null)
            {
                Instantiate(_cheerleaderClone, _cheerleaderSpawnLocation.transform.position, Quaternion.identity);
            }
        }

        BasketballObject = GameObject.FindWithTag("basketball");
        Basketball = BasketballObject.GetComponent<BasketBall>();
        BasketballRimVector = GameObject.Find("rim").transform.position;

    }

    private void Start()
    {

        //unlimited
        //QualitySettings.vSyncCount = 0;

        //Debug.Log("screen.dpi : " + Screen.dpi);
        //Debug.Log("device model : " + SystemInfo.deviceModel);
        //Debug.Log("device  name: " + SystemInfo.deviceName);
        //Debug.Log("device graphics : " + SystemInfo.graphicsDeviceName);

        //QualitySettings.vSyncCount = 1;
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;

        //QualitySettings.resolutionScalingFixedDPIFactor = 0.75f;


        //Debug.Log("quality level : " + QualitySettings.GetQualityLevel());
        //Debug.Log("quality level : " + QualitySettings.names);

        _locked = false;

        //set up player/basketball read only references for use in other classes
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerState = Player.GetComponent<PlayerController>();
        Anim = Player.GetComponentInChildren<Animator>();

        // if an npc is in scene, disable the npc if it is the player selected
        //* this is specific to flash right now
        _npcObjects = GameObject.FindGameObjectsWithTag("auto_npc");

        //string playerName = GameOptions.playerObjectName;
        if (GameOptions.trafficEnabled)
        {
            _vehicleObjects = GameObject.FindGameObjectsWithTag("vehicle");

            // check if player is vehicle in scene
            foreach (var veh in _vehicleObjects)
            {
                if (!string.IsNullOrEmpty(GameOptions.playerObjectName) && veh.name.Contains(GameOptions.playerObjectName))
                {
                    veh.SetActive(false);
                }
            }
        }
        // check if player is npc in scene
        foreach (var npc in _npcObjects)
        {
            if ( !string.IsNullOrEmpty(GameOptions.playerObjectName) && npc.name.Contains(GameOptions.playerObjectName) )
            {
                npc.SetActive(false);
            }
        }
        //// check if player is vehicle in scene
        //foreach (var veh in _vehicleObjects)
        //{
        //    if (!string.IsNullOrEmpty(playerName) && veh.name.Contains(playerName))
        //    {
        //        veh.SetActive(false);
        //    }
        //}
    }

    private void Update()
    {
        //turn on : toggle run
        if (Controls.Other.change.enabled
            && Controls.Other.toggle_run_keyboard.triggered
            && !_locked
            && !Pause.instance.Paused)
        {
            _locked = true;
            PlayerState.toggleRun();
            _locked = false;
        }

        //turn off stats
        if (Controls.Other.change.enabled
            && Controls.Other.toggle_stats_keyboard.triggered
            && !_locked
            && !Pause.instance.Paused)
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
    public PlayerControls Controls { get => controls; set => controls = value; }
    public FloatingJoystick Joystick { get => joystick;  }

    private void OnEnable()
    {
        controls.Player.Enable();
        controls.UINavigation.Enable();
        controls.Other.Enable();
        //controls.PlayerTouch.Enable();
    }
    private void OnDisable()
    {
        controls.Player.Disable();
        controls.UINavigation.Disable();
        controls.Other.Disable();
        //controls.PlayerTouch.Disable();
    }
}
