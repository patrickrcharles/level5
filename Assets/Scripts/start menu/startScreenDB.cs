using System.Collections;
using System.Collections.Generic;
using TeamUtility.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class startScreenDB : MonoBehaviour
{


    [SerializeField]
    public List<string> sceneNames;
    [SerializeField]
    int sceneIndex = 0;

    [SerializeField]
    bool YaxisInUse = false;
    [SerializeField]
    bool noInputDetected = false;
    [SerializeField]
    bool XaxisInUse = false;

    // to determine which option to change on user input 
    string optionNumEnemies = "number_of_enemies";
    string optionGameMode = "game_mode";
    string optionWeaponList = "weapons";
    string optionLives = "lives";

    string optionArrowLeft = "left_arrow";
    string optionArrowRight = "right_arrow";
    string optionHeader = "header";
    [SerializeField]
    GameObject currentObject;

    [SerializeField]
    Selectable leftArrowButton;
    [SerializeField]
    Selectable rightArrowButton;
    [SerializeField]
    Button defaultButton;
    [SerializeField]
    Button currentButton;
    [SerializeField]
    string currentSceneName;
    [SerializeField]
    sceneList scenesList; // list of scene names

    private void Awake()
    {

        // initialize scene name, list of scenes, find buttons and set button flags
        initializeScene();
    }

    // Use this for initialization
    void Start()
    {
        // if 'selectlevel' (disaplys curent level selection) is enabled, display currently
        // selected level
        // as memus are navigated, this object is disabled because the text isnt used. 
        // this will check if it is enabled indicating it is needed
        //selectLevel.GetComponent<text>().text = sceneNames[sceneIndex];
    }

    // Update is called once per frame
    void Update()
    {

        currentObject = EventSystem.current.currentSelectedGameObject;
        /*
        // if left/right input detected
        if ((InputManager.GetAxisRaw("Horizontal") == 1 || InputManager.GetAxisRaw("Horizontal") == -1)
                && !XaxisInUse)
        // if left/right button is pressed
        //if (InputManager.GetAxisRaw("Horizontal") != 0)
        {
            Debug.Log("Left/Right direction pressed");

            // if 'left' is pressed, cycle through selectable levels list
            if (InputManager.GetAxis("Horizontal") < 0)
            {
                XaxisInUse = true;
                //LoadSceneLeft();
            }
            else
            {
                // if 'right' is pressed, cycle through selectable levels list
                if (InputManager.GetAxis("Horizontal") > 0)
                {
                    XaxisInUse = true;
                    //LoadSceneRight();
                }
            }
        }
        */
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

    void loadHighScoresScene()
    {
        SceneManager.LoadScene("startScreen_highscores");
    }
    void loadDemo1DB()
    {
        SceneManager.LoadScene("start_demo1DB");
    }
    void loadDemo2DB()
    {
        SceneManager.LoadScene("start_demo2DB");
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

        loadOptionsScene();
    }
    void LoadSceneRight()
    {

        loadStartScene();
    }

    void findNavigationButtons()
    {
        if (GameObject.Find("left_arrow") != null)
        {
            leftArrowButton = GameObject.Find("left_arrow").GetComponent<Selectable>();
        }
        if (GameObject.Find("right_arrow") != null)
        {
            rightArrowButton = GameObject.Find("right_arrow").GetComponent<Selectable>();
        }
    }
    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    private void initializeScene()
    {
        // get current scene name
        currentSceneName = getCurrentSceneName();
        //finds and sets buttons for scene navigation
        findNavigationButtons();
    }
}
