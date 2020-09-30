using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DevFunctions : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] CharacterProfile player;
    [SerializeField] Text debugText;
    [SerializeField] GameObject fpsCounter;

    bool fpsActive = false;

    private void Start()
    {
        player = GameLevelManager.instance.PlayerShooterProfile;
        debugText = GameObject.Find("debug_text").GetComponent<Text>();

        if (GameLevelManager.instance != null)
        {
            fpsCounter = GameObject.Find("fps_counter");
            fpsCounter.SetActive(false);
        }
    }


    private void Update()
    {
        if (GameLevelManager.instance.Controls.Other.change.enabled
            && GameLevelManager.instance.Controls.Other.toggle_character_max_stats.triggered)
        {
            player.Accuracy2Pt = 100;
            player.Accuracy3Pt = 100;
            player.Accuracy4Pt = 100;
            player.Accuracy7Pt = 100;
            player.Release = 100;
            player.Range = 100;
            player.Luck = 10;
        }
        if (GameLevelManager.instance.Controls.Other.change.enabled
            && GameLevelManager.instance.Controls.Other.toggle_fps_counter.triggered)
        {
            ToggleFpsCounter();
        }

        //debugText.text = GameLevelManager.instance.PlayerState.RigidBody.velocity.magnitude.ToString();
        //debugText.text = GameLevelManager.instance.PlayerState.MovementSpeed.ToString();
        //+"\n"+ GameLevelManager.instance.PlayerState.CurrentStateInfo;
    }

    void ToggleFpsCounter()
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
}
