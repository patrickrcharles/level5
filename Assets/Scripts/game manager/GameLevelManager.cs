using System;
using UnityEngine;

public class GameLevelManager : MonoBehaviour
{
    // this is to keep a reference to player in game manager 
    // that can be retrieved across all scripts
    [SerializeField]
    private GameObject _player;
    private PlayerController _playerState;
    private CharacterProfile _playerShooterProfile;
    private PlayerHealth _playerHealth;
    [SerializeField]
    private PlayerAttackQueue _playerAttackQueue;

    //BasketBall objects
    private GameObject _basketballPrefab;
    private GameObject _cheerleaderPrefab;

    //spawn locations
    private GameObject _basketballSpawnLocation;
    private GameObject _playerSpawnLocation;
    private GameObject _cheerleaderSpawnLocation;

    private BasketBall _basketball;
    //private GameObject _basketballObject;
    private Vector3 _basketballRimVector;

    private GameObject _playerClone;
    private GameObject _cheerleaderClone;

    GameObject[] _npcObjects;
    GameObject[] _vehicleObjects;

    PlayerControls controls;
    FloatingJoystick joystick;

    const string basketBallPrefabPath = "Prefabs/basketball/basketball";

    public static GameLevelManager instance;
    private bool _locked;


    //[SerializeField] InputSystemUIInputModule inputSystemUIInputModule;
    //[SerializeField] StandaloneInputModule standaloneInputModule;
    //[SerializeField] EventSystem eventSystem;

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

        // spawn locations
        _playerSpawnLocation = GameObject.Find("player_spawn_location");
        _basketballSpawnLocation = GameObject.Find("ball_spawn_location");
        _cheerleaderSpawnLocation = GameObject.Find("cheerleader_spawn_location");

        // if player doesnt exists, spawn default Player
        checkPlayerPrefabExists();
        // if basketball doesnt exists
        checkBasketballPrefabExists();
        // cheerleader doesnt exists
        checkCheerleaderPrefabExists();
        //// check if player is npc in scene
        checkPlayerIsNpcInScene();
        // get necessary references to objects
        //StartCoroutine( SetMajorObjectReferences() );
        //BasketballObject = GameObject.FindWithTag("basketball");
        Basketball = GameObject.FindGameObjectWithTag("basketball").GetComponent<BasketBall>();
        BasketballRimVector = GameObject.Find("rim").transform.position;
    }

    private void Start()
    {
        // return to this if n
        GameOptions.previousSceneName = "level_00_start";

        // analytic event
        if (!String.IsNullOrEmpty(GameOptions.levelSelectedName))
        {
            AnaylticsManager.LevelLoaded(GameOptions.levelSelectedName);
        }

        //disable lighting if necessary
        // something like if gameoptions.lightingenabled

        //Light[] lights = GameObject.FindObjectsOfType<Light>();
        //if(lights.Length > 0)
        //{
        //    foreach (Light light in lights)
        //    {
        //        //Debug.Log("disable : " + light.name);
        //        light.enabled = false;
        //    }
        //    RenderSettings.ambientLight = Color.white;
        //}


        _locked = false;
        //set up player/basketball read only references for use in other classes
        if (GameObject.FindWithTag("Player") == null)
        {
            Debug.Log("null player");
        }

        _player = GameObject.FindWithTag("Player");
        _playerState = _player.GetComponent<PlayerController>();
        _playerShooterProfile = _player.GetComponent<CharacterProfile>();
        _playerAttackQueue = _player.GetComponent<PlayerAttackQueue>();
        _playerHealth = _player.GetComponentInChildren<PlayerHealth>();

        Anim = Player.GetComponentInChildren<Animator>();
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

    private void checkPlayerIsNpcInScene()
    {
        //Debug.Log("checkPlayerIsNpcInScene() : traffic : "+ GameOptions.trafficEnabled);
        // check if player is 'a vehicle' ex. Sam on skateboard, Rad Tony on horse
        if (GameOptions.trafficEnabled)
        {
            _vehicleObjects = GameObject.FindGameObjectsWithTag("vehicle");

            // check if player is vehicle in scene
            foreach (var veh in _vehicleObjects)
            {
                //Debug.Log("veh  : " + veh.name);
                //Debug.Log("GameOptions.playerObjectName : " + GameOptions.playerObjectName);
                if (!string.IsNullOrEmpty(GameOptions.playerObjectName) && veh.name.Contains(GameOptions.playerObjectName))
                {
                    //Debug.Log("disable veh  : " + veh.name);
                    veh.SetActive(false);
                }
            }
        }
        // check if player is an npc, ex. ??? on slab
        _npcObjects = GameObject.FindGameObjectsWithTag("auto_npc");
        foreach (var npc in _npcObjects)
        {
            if (!string.IsNullOrEmpty(GameOptions.playerObjectName) && npc.name.Contains(GameOptions.playerObjectName))
            {
                //Debug.Log("disable npc  : " + npc.name);
                npc.SetActive(false);
            }
        }
    }

    private void checkCheerleaderPrefabExists()
    {
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
    }

    private void checkBasketballPrefabExists()
    {
        if (GameObject.FindWithTag("basketball") == null)
        {
            _basketballPrefab = Resources.Load(basketBallPrefabPath) as GameObject;
            Instantiate(_basketballPrefab, _basketballSpawnLocation.transform.position, Quaternion.identity);
        }
    }

    private void checkPlayerPrefabExists()
    {
        // if player selected is not null / player not selected
        if (!string.IsNullOrEmpty(GameOptions.playerObjectName))
        {
            string playerPrefabPath = "Prefabs/characters/players/player_" + GameOptions.playerObjectName;
            _playerClone = Resources.Load(playerPrefabPath) as GameObject;
            //Debug.Log("load prefab");y analyticsvalidotr not working

        }

        //Debug.Log("GameObject.FindWithTag(Player) : " + GameObject.FindWithTag("Player"));
        // if no player, spawn player
        if (GameObject.FindWithTag("Player") == null)
        {
            if (_playerClone != null)
            {
                Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
                //Debug.Log("player spawned : " + _playerClone.name);
            }
            else
            {
                // spawn default character
                string playerPrefabPath = "Prefabs/characters/players/player_drblood";
                //Debug.Log("playerPrefabPath : " + playerPrefabPath);
                _playerClone = Resources.Load(playerPrefabPath) as GameObject;
                //Debug.Log("_playerClone : " + _playerClone);
                Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
            }
        }
    }

    //private string GetCurrentSceneName()
    //{
    //    return SceneManager.GetActiveScene().name;
    //}

    //private void Quit()
    //{
    //    Application.Quit();
    //}

    public GameObject Player => _player;

    public PlayerController PlayerState => _playerState;

    public Animator Anim { get; private set; }

    public bool GameOver { get; set; }
    public PlayerControls Controls { get => controls; set => controls = value; }
    public FloatingJoystick Joystick { get => joystick; }
    public BasketBall Basketball { get => _basketball; set => _basketball = value; }
    //public GameObject BasketballObject { get => _basketballObject; set => _basketballObject = value; }
    public Vector3 BasketballRimVector { get => _basketballRimVector; set => _basketballRimVector = value; }
    public CharacterProfile PlayerShooterProfile { get => _playerShooterProfile; set => _playerShooterProfile = value; }
    public PlayerAttackQueue PlayerAttackQueue { get => _playerAttackQueue; set => _playerAttackQueue = value; }
    public PlayerHealth PlayerHealth { get => _playerHealth; set => _playerHealth = value; }
}
