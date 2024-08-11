using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using Ping = System.Net.NetworkInformation.Ping;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Utility
{
	public static class LoadGame
	{
		public static void LoadGameMode(StartScreenModeSelected mode, LevelSelected level, PlayerIdentifier player)
		{

		}
        //public static void LoadGameMode(StartScreenModeSelected mode, LevelSelected level, PlayerIdentifier player, PlayerIdentifier cpuPlayer)
        public static IEnumerator LoadDevLevelVersus(int seconds)
        {
            // get level id, mode id
            // get player prefab
            // get cpu prefab
            GameObject player;
            GameObject cpuPlayer;
            string playerPrefabPath1;
            string cpuPrefabPath1;


            playerPrefabPath1 = Constants.PREFAB_PATH_CHARACTER_human + GameOptions.characterObjectNames[0];
            cpuPrefabPath1 = Constants.PREFAB_PATH_CHARACTER_cpu + "pony";

            GameObject go1 = GameObject.FindGameObjectWithTag("Player");
            //GameObject go2 = Resources.Load(cpuPrefabPath1) as GameObject;

            player = go1;
            //cpuPlayer = go2;

            PlayerIdentifier pi = player.GetComponent<PlayerIdentifier>();
            //PlayerIdentifier cpuipi = cpuPlayer.GetComponent<PlayerIdentifier>();

            //mode
            GameOptions.gameModeSelectedId = Modes.VersusCpu;
            GameOptions.gameModeSelectedName = "Versus";
            GameOptions.gameModeRequiresCountDown = true;
            GameOptions.gameModeRequiresBasketball = true;
            GameOptions.gameModeAllowsCpuShooters = true;
            //level
            GameOptions.levelId = Levels.Dev;
            GameOptions.levelHasSevenPointers = true;
            GameOptions.levelDisplayName = "Dev";
            //options
            GameOptions.gameModeHasBeenSelected = true;
            GameOptions.customTimer = 0;
            //character
            GameOptions.characterObjectNames = new List<string>();
            GameOptions.characterObjectNames.Add(pi.characterProfile.PlayerObjectName);
            GameOptions.characterObjectNames.Add("pony");

            GameOptions.numPlayers = GameOptions.gameModeSelectedId != Modes.BeatThaComputahs ? 2 :  GameOptions.characterObjectNames.Count;
            GameOptions.levelsList = PlayerData.instance.LevelsList;

            yield return new WaitForSeconds(seconds);

            string sceneName;
            sceneName = Constants.SCENE_NAME_level_23_dev;
            SceneManager.LoadScene(sceneName);
        }

        //internal static void LoadDevLevelVersus()
        //{
        //    throw new NotImplementedException();
        //}
    }
}