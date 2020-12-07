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

        //debugText.text = GameLevelManager.instance.PlayerState.RigidBody.velocity.magnitude.ToString();
        //debugText.text = GameLevelManager.instance.PlayerState.MovementSpeed.ToString();
        //+"\n"+ GameLevelManager.instance.PlayerState.CurrentStateInfo;
    }

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
