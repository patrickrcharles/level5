using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sceneList : MonoBehaviour {

    public string startScreen = "startScreen";
    public string startScreen_db = "startScreen_highscores";
    public string startScreen_options = "startScreen_options";
    public string level1 = "level1";
    public string level1aptExterior = "level1_apt_exterior";
    public string level1InsideRobot = "level1_robot";
    public string level1UnderRobot = "level1_under_robot";
    public string level2 = "level2";
    public string level3 = "level3";
    public string level4 = "level4";
    public string level5 = "level5";
    public string level6 = "level6";
    public string level7 = "level7";
    public string level8 = "level8";
    public string miniGame_moonwalk = "minigame_moonwalk";
    public string miniGame_russianRoulette = "minigame_russian_roulette";

    public List<string> levelNames;

    private void Awake()
    {

        setLevelNamesList();
    }

    void setLevelNamesList()
    {
        levelNames.Add("Lee Ave.");
        levelNames.Add("Circle K");
        levelNames.Add("Alaska");
        levelNames.Add("The Caffe");
    }

}
