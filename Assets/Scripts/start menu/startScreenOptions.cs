using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class startScreenOptions : MonoBehaviour
{
    // UI text
    public Text number_of_enemies_text;
    public Text game_mode_text;
    public Text weapons_text;
    public Text lives_text;

    // current slected object/button/selectable
    [SerializeField]
    GameObject currentObject;

    // is left/right being pressed
    private bool _XaxisInUse;
    public bool XaxisInUse
    {
        get { return _XaxisInUse; }
        set { _XaxisInUse = value; }
    }

    // small delay after input pressed to prevent multiple inputs per press
    [SerializeField]
    const float buttonDelayTime = 0.2f;

    // to determine which option to change on user input 
    string optionNumEnemies = "number_of_enemies";
    string optionGameMode = "game_mode";
    string optionWeaponList = "weapons";
    string optionLives = "lives";
    string optionArrowLeft = "left_arrow";
    string optionArrowRight = "right_arrow";
    string optionHeader = "header";

    string currentSceneName;
    // buttons in scene
    Selectable headerSelectable, leftArrowSelectable, rightArrowSelectable;

    int index; //var to get index
    [SerializeField]
    // holds values for game options that can be changed
    gameOptions gameOptions;
    // counter needed for button navigation
    int countGameModes;
    int countWeaponLists;

    void awake()
    {
    }

    void Start()
    {
        gameOptions = Instantiate(Resources.Load("Prefabs/start_prefabs/game_options") as GameObject).GetComponent<gameOptions>();
        index = 0;
        countGameModes = gameOptions.getOptionGameModes().Count;
        countWeaponLists = gameOptions.getOptionWeaponLists().Count;
        SetInitialOptions();
    }

    // Update is called once per frame
    void Update()
    {

        currentObject = EventSystem.current.currentSelectedGameObject;
        // menu is active, right analog stick event
        if ((InputManager.GetAxisRaw("Horizontal") > 0.9f || InputManager.GetAxisRaw("Horizontal") < -0.9f)
            && !XaxisInUse)
        {
            //Debug.Log(":::::::::::::::::::::::::::::   INPUT  currentObject.name : " + currentObject.name);
            XaxisInUse = true;
            string direction;
            //set whether input selection is L/R to cycle L/R through list
            if ((InputManager.GetAxisRaw("Horizontal") > 0))
            { direction = "right"; }
            else
            { direction = "left"; }
            changeOptionMenuSelection(direction);
        }
        // if not option that can be changed (navigation error or header ), disable locking boolean
        if (currentObject.name.ToLower() == optionArrowLeft
            || currentObject.name.ToLower() == optionArrowRight
            || currentObject.name.ToLower() == optionHeader)
        {
            XaxisInUse = false;
        }
        if (currentObject.name.ToLower() == optionArrowLeft
            && (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Jump")))
        {
            LoadSceneLeft();
        }
        if (currentObject.name.ToLower() == optionArrowRight
            && (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Jump")))
        {
            LoadSceneRight();
        }
    }

    private void changeOptionMenuSelection(string direction)
    {
        if (currentObject.name.ToLower() == optionNumEnemies)
        {
            Debug.Log("----- change currentObject.name : " + currentObject.name);
            changeOptions(direction, optionNumEnemies);
            StartCoroutine(buttonPressDelay(buttonDelayTime));
        }
        if (currentObject.name.ToLower() == optionGameMode)
        {
            Debug.Log("----- change currentObject.name : " + currentObject.name);
            changeOptions(direction, optionGameMode);
            StartCoroutine(buttonPressDelay(buttonDelayTime));
        }
        if (currentObject.name.ToLower() == optionWeaponList)
        {
            Debug.Log("----- change currentObject.name : " + currentObject.name);
            changeOptions(direction, optionWeaponList);
            StartCoroutine(buttonPressDelay(buttonDelayTime));
        }
        if (currentObject.name.ToLower() == optionLives)
        {
            Debug.Log("----- change currentObject.name : " + currentObject.name);
            changeOptions(direction, optionLives);
            StartCoroutine(buttonPressDelay(buttonDelayTime));
        }
    }

    //adds delay allowing right trigger to be used as a button
    IEnumerator buttonPressDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        XaxisInUse = false;
        Debug.Log(" IEnumerator enableRightStickPress(float delay) ::::: XaxisInUse = false;");
    }

    public void changeOptions(string direction, string optionToChange)
    {
        Debug.Log("---------- void changeOptions() --------------------------");
        Debug.Log("index :: " + index);
        // get current index of value to change if in a list
        // if not in list, index value not needed
        if (optionToChange == optionGameMode)
        {
            Debug.Log("     optionGameMode");
            string currentSelection = game_mode_text.text;
            index = gameOptions.getOptionGameModes().IndexOf(currentSelection);
        }
        if (optionToChange == optionWeaponList)
        {
            Debug.Log("     optionWeaponList");
            string currentSelection = weapons_text.text;
            index = gameOptions.getOptionWeaponLists().IndexOf(currentSelection);
        }

        Debug.Log("direction :: " + direction);
        Debug.Log("index :: " + index);
        Debug.Log("optionToChange :: " + optionToChange);

        //update index to new value based on input
        if (direction == "right") { index += 1; }
        if (direction == "left") { index += -1; }

        Debug.Log("direction :: " + direction);
        Debug.Log("index :: " + index);

        switch (optionToChange)
        {
            case "number_of_enemies":
                ChangeNumberOfEnemies(direction);
                break;
            case "game_mode":
                ChangeGameMode(index);
                break;
            case "weapons":
                ChangeWeaponList(index);
                break;
            case "lives":
                ChangeLivesUsed(direction);
                break;
        }
    }

    public void SetInitialOptions()
    {
        game_mode_text.text = gameOptions.getCurrentGameMode();
        number_of_enemies_text.text = gameOptions.getOptionMaxEnemies().ToString();
        weapons_text.text = gameOptions.getCurrentWeaponList();
        lives_text.text = gameOptions.getOptionNumberOfLives().ToString();
        currentSceneName = getCurrentSceneName();
    }

    void ChangeWeaponList(int index)
    {
        Debug.Log("ChangeWeaponList() :: " + index);
        // first item
        if (index < 0)
        {
            Debug.Log("if (index < 0) :: " + index);
            gameOptions.setCurrentWeaponList(countWeaponLists - 1);
            Debug.Log("gameOptions.getCurrentWeaponList(countWeaponLists - 1) :: " + gameOptions.getOptionWeaponLists()[countWeaponLists - 1]);
        }
        // last item
        if (index > countWeaponLists - 1)
        {
            Debug.Log("if (index > countWeaponLists) :: " + index);
            gameOptions.setCurrentWeaponList(0);
            Debug.Log("gameOptions.getCurrentWeaponList(0) :: " + gameOptions.getOptionWeaponLists()[0]);
            // i suppose i could add a function in weapon class to do all of the above changes
        }
        // else NOT last item in list
        if (index >= 0 && index <= countWeaponLists - 1)
        {
            Debug.Log("if (index >= 0 && index <= countWeaponLists - 1) :: " + index);
            gameOptions.setCurrentWeaponList(index);
            Debug.Log("gameOptions.getCurrentWeaponList(0) :: " + gameOptions.getOptionWeaponLists()[index]);
        }
        weapons_text.text = gameOptions.getCurrentWeaponList();
    }
    void ChangeGameMode(int index)
    {
        Debug.Log("countGameModes : " + countGameModes);
        // first item
        if (index < 0)
        {
            gameOptions.setCurrentGameMode(countGameModes - 1);
        }
        // last item
        if (index > countGameModes - 1)
        {
            gameOptions.setCurrentGameMode(0);
        }
        // else NOT last item in list
        if (index >= 0 && index <= countGameModes - 1)
        {
            gameOptions.setCurrentGameMode(index);
        }
        Debug.Log("index : " + index);
        Debug.Log("gameOptions.getCurrentGameMode() : " + gameOptions.getCurrentGameMode());

        game_mode_text.text = gameOptions.getCurrentGameMode();
    }

    //todo: remove hardcoded values, add vars
    void ChangeNumberOfEnemies(string direction)
    {
        Debug.Log("--------------------------  ChangeNumberOfPlayers(string direction)");
        if (direction == "left")
        {
            if (gameOptions.getOptionMaxEnemies() > 1)
            {
                gameOptions.setCurrentNumberOfEnemies(-1);
            }
        }
        if (direction == "right")
        {
            if (gameOptions.getOptionMaxEnemies() < 20)
            {
                gameOptions.setCurrentNumberOfEnemies(1);
            }
        }
        number_of_enemies_text.text = gameOptions.getOptionMaxEnemies().ToString();
    }

    //todo: remove hardcoded values, add vars
    void ChangeLivesUsed(string direction)
    {
        Debug.Log("--------------------------  ChangeLivesUsed(string direction)");
        if (direction == "left")
        {
            if (gameOptions.getOptionNumberOfLives() > 1)
            {
                gameOptions.setCurrentNumberOfLives(-1);
            }
        }
        if (direction == "right")
        {
            if (gameOptions.getOptionNumberOfLives() < 5)
            {
                gameOptions.setCurrentNumberOfLives(1);
            }
        }
        lives_text.text = gameOptions.getOptionNumberOfLives().ToString();
    }

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

    void LoadSceneLeft()
    {
        loadStartScene();
    }
    void LoadSceneRight()
    {
        loadHighScoresScene();
    }

    void findNavigationButtons()
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
    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
}
