using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevFunctions : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] CharacterProfile player;

    private void Start()
    {
        player = GameLevelManager.instance.PlayerShooterProfile;
    }


    private void Update()
    {
        if(GameLevelManager.instance.Controls.Other.change.enabled 
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
    }
}
