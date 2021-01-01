using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DevFunctions : MonoBehaviour
{

    [SerializeField] CharacterProfile player;
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
        //if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha9)
        //    && GameLevelManager.instance.PlayerState.hasBasketball)
        //{
        //    //StartCoroutine(PlayerDunk());
        //    //PlayerDunk();
        //    GameLevelManager.instance.PlayerState.PlayerDunk();
        //}
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

        foreach (GameObject enemy in enemies)
        {
            EnemyController enemyController = enemy.GetComponent<EnemyController>();

            if (enemyController.SpriteRenderer.isVisible)
            {
                StartCoroutine(enemyController.struckByLighning());
            }
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
