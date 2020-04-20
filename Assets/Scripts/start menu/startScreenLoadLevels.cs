using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class startScreenLoadLevels : MonoBehaviour {

    sceneList sceneList;

    // to determine which option to change on user input 
    string optionNumEnemies = "number_of_enemies";
    string optionGameMode = "game_mode";
    string optionWeaponList = "weapons";
    string optionLives = "lives";

    string optionArrowLeft = "left_arrow";
    string optionArrowRight = "right_arrow";
    string optionHeader = "header";

    [SerializeField]
    public Selectable leftArrowSelectable;
    [SerializeField]
    public Selectable rightArrowSelectable;
    [SerializeField]
    public Selectable headerSelectable;
    [SerializeField]
    public Selectable defaultButton;
    [SerializeField]
    public Selectable currentButton;
    [SerializeField]
    string currentSceneName;


    void loadHighScoresScene()
    {
        SceneManager.LoadScene("startScreen_highscores");
    }
    void loadStartScene()
    {
        SceneManager.LoadScene("startScreen");
    }
    void loadOptionsScene()
    {
        SceneManager.LoadScene("startScreen_options");
    }

    public void LoadSceneLeft()
    {
        if (currentSceneName == "startScreen")
        {
            loadOptionsScene();
        }
        if (currentSceneName == "startScreen_highscores")
        {
            loadStartScene();
        }
        if (currentSceneName == "startScreen_options")
        {
            loadHighScoresScene();
        }
    }
    public void LoadSceneRight()
    {
        if (currentSceneName == "startScreen")
        {
            loadHighScoresScene();
        }
        if (currentSceneName == "startScreen_highscores")
        {
            loadOptionsScene();
        }
        if (currentSceneName == "startScreen_options")
        {
            loadStartScene();
        }
    }

    public void findNavigationButtons()
    {
        if (GameObject.Find(optionArrowLeft) != null)
        {
            leftArrowSelectable = GameObject.Find(optionArrowLeft).GetComponent<Selectable>();
        }
        if (GameObject.Find(optionArrowRight) != null)
        {
            rightArrowSelectable = GameObject.Find(optionArrowRight).GetComponent<Selectable>();
        }
        if (GameObject.Find(optionHeader) != null)
        {
            headerSelectable = GameObject.Find(optionHeader).GetComponent<Selectable>();
        }
    }
    public string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
