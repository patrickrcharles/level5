using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DevFunctions : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] CharacterProfile player;
    //[SerializeField] Text debugText;
    [SerializeField] GameObject fpsCounter;
    [SerializeField] GameObject[] enemies;
    [SerializeField] Text messageText;
    [SerializeField] float smoothSpeed;

    public static DevFunctions instance;

    bool fpsActive = false;

    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        player = GameLevelManager.instance.PlayerShooterProfile;
        //debugText = GameObject.Find("debug_text").GetComponent<Text>();
        messageText = GameObject.Find("messageDisplay").GetComponent<Text>();

        if (GameLevelManager.instance != null)
        {
            //fpsCounter = GameObject.Find("fps_counter");
            fpsCounter = GameObject.Find("LiteFPSCounter");
            fpsCounter.SetActive(false);
        }
    }
    private void Update()
    {
        if (GameLevelManager.instance.Controls.Other.change.enabled
            && GameLevelManager.instance.Controls.Other.toggle_character_max_stats.triggered)
        {
            setMaxPlayerStats();
        }
        if (GameLevelManager.instance.Controls.Other.change.enabled
            && GameLevelManager.instance.Controls.Other.toggle_fps_counter.triggered)
        {
            ToggleFpsCounter();
        }
        if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha8))
        {
            InstantiateRob();
        }
        if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha9)
            && GameLevelManager.instance.PlayerState.hasBasketball)
        {
            //StartCoroutine(PlayerDunk());
            //PlayerDunk();
            GameLevelManager.instance.PlayerState.PlayerDunk();
        }

        //debugText.text = GameLevelManager.instance.PlayerState.RigidBody.velocity.magnitude.ToString();
        //debugText.text = GameLevelManager.instance.PlayerState.MovementSpeed.ToString();
        //+"\n"+ GameLevelManager.instance.PlayerState.CurrentStateInfo;
    }

    //void PlayerDunk()
    //{
    //    float bballRelativePositioning = GameLevelManager.instance.BasketballRimVector.x - GameLevelManager.instance.PlayerState.transform.position.x;
    //    Vector3 dunkPosition = new Vector3(0, 0, 0);

    //    //*NOTE on dunk anim, open attackbox and make knockdown to poster enemies

    //    // determine which side to dunk on
    //    if (bballRelativePositioning > 0)
    //    {
    //        dunkPosition = GameObject.Find("dunk_position_left").transform.position;
    //        //Vector3 dunkPosition = GameObject.Find("dunk_position_right").transform.position;
    //    }
    //    if (bballRelativePositioning < 0)
    //    {
    //        dunkPosition = GameObject.Find("dunk_position_right").transform.position;
    //    }

    //    Launch(dunkPosition);
    //}

    //public IEnumerator TriggerDunkSequence()
    //{
    //    GameLevelManager.instance.PlayerState.FreezePlayerPosition();
    //    GameLevelManager.instance.PlayerState.playAnim("dunk");
    //    // wait for anim to start + finish
    //    yield return new WaitUntil(() => GameLevelManager.instance.PlayerState.currentState == GameLevelManager.instance.PlayerState.dunkState);
    //    yield return new WaitUntil(() => GameLevelManager.instance.PlayerState.currentState != GameLevelManager.instance.PlayerState.dunkState);

    //    BasketBall.instance.BasketBallState.Thrown = true;
    //    GameLevelManager.instance.PlayerState.UnFreezePlayerPosition();

    //    // move ball above rim
    //    Vector3 temp = BasketBall.instance.BasketBallState.BasketBallTarget.transform.position;
    //    BasketBall.instance.Rigidbody.velocity = Vector3.zero;
    //    BasketBall.instance.transform.position = new Vector3(temp.x, temp.y, temp.z);
    //    //reset
    //    GameLevelManager.instance.PlayerState.hasBasketball = false;
    //    GameLevelManager.instance.PlayerState.setPlayerAnim("hasBasketball", false);
    //}

    //// =================================== Launch ball function =======================================
    //void Launch(Vector3 Target)
    //{
    //    //Vector3 projectileXZPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    //    Vector3 projectileXZPos = GameLevelManager.instance.Player.transform.position;
    //    Vector3 targetXZPos = Target;

    //    // rotate the object to face the target
    //    GameLevelManager.instance.Player.transform.LookAt(targetXZPos);

    //    // shorthands for the formula
    //    float R = Vector3.Distance(projectileXZPos, targetXZPos);

    //    float G = Physics.gravity.y;
    //    float tanAlpha = Mathf.Tan(40 * Mathf.Deg2Rad);
    //    float H = targetXZPos.y - projectileXZPos.y;
    //    float Vz = Mathf.Sqrt(G * R * R / (2.0f * (H - R * tanAlpha)));
    //    float Vy = tanAlpha * Vz;

    //    float xVector = 0;
    //    float yVector = Vy; // + (accuracyModifier * shooterProfile.shootYVariance);
    //    float zVector = Vz; //+ accuracyModifierZ; // + (accuracyModifier * shooterProfile.shootZVariance);

    //    // create the velocity vector in local space and get it in global space
    //    Vector3 localVelocity = new Vector3(xVector, yVector, zVector);
    //    Vector3 globalVelocity = GameLevelManager.instance.Player.transform.TransformDirection(localVelocity);

    //    // launch the object by setting its initial velocity and flipping its state
    //    GameLevelManager.instance.PlayerState.RigidBody.velocity = globalVelocity;
    //    GameLevelManager.instance.PlayerState.playAnim("inair_dunk");
    //    GameLevelManager.instance.Player.transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 0f));
    //}

    private void InstantiateRob()
    {
        Debug.Log("InstantiateRob()");

        GameObject _playerSpawnLocation = GameObject.Find("player_spawn_location");
        string playerPrefabPath = "Prefabs/characters/players/npc_specific/npc_rob";
        GameObject _playerClone = Resources.Load(playerPrefabPath) as GameObject;

        Instantiate(_playerClone, _playerSpawnLocation.transform.position, Quaternion.identity);
    }

    private void testLightningStrike()
    {
        enemies = GameObject.FindGameObjectsWithTag("enemy");
        //Debug.Log("tets lighting animation");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();
            //enemyController.playAnimation("lightning");
            //enemyController.playAnimation("lightning");
            //enemyController.playAnimation("enemy_generic_lightning");
            if (enemyController.SpriteRenderer.isVisible)
            {
                //StartCoroutine(enemyController.struckByLighning());
                StartCoroutine(enemyController.struckByLighning());
            }
            //enemyController.GetComponentInChildren<Animator>().Play("enemy_generic_lightning");
            //Debug.Log("test lighting animation");
        }
    }

    public void setMaxPlayerStats()
    {

        player.Accuracy2Pt = 100;
        player.Accuracy3Pt = 100;
        player.Accuracy4Pt = 100;
        player.Accuracy7Pt = 100;
        player.Release = 100;
        player.Range = 100;
        player.Luck = 10;

        //Text messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "max player stats enabled";
        StartCoroutine(turnOffMessageLogDisplayAfterSeconds(3));
    }

    public void ToggleFpsCounter()
    {
        fpsActive = !fpsActive;

        if (fpsActive)
        {
            fpsCounter.SetActive(true);
        }
        else
        {
            fpsCounter.SetActive(false);
        }
    }

    public IEnumerator turnOffMessageLogDisplayAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        messageText = GameObject.Find("messageDisplay").GetComponent<Text>();
        messageText.text = "";
    }
}
