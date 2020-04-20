using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using TeamUtility.IO;
using UnityEngine.EventSystems;

public class startScreenManager : MonoBehaviour {

    public bool levelSelect;
    public bool pressStart;
    //public bool highScoreScreen;
    //public bool optionScreen;
    //public bool demo1HighScore, demo2HighScore;
    //public bool textIsBlinking;
    [SerializeField]
    GameObject pressStartObject;
    [SerializeField]
    GameObject selectLevelObject;

    //GameObject startScreenObject;

    // start screen music component and script to start it and keep it playing
    [SerializeField]
    private GameObject[] startMenuMusic;
    [SerializeField]
    private GameObject startMenuMusicObject;
    [SerializeField]
    public List<string> sceneNames;
    [SerializeField]
    int sceneIndex = 0;

    //[SerializeField]
    //float blinkTextTime;
    [SerializeField]
    bool YaxisInUse = false;
    [SerializeField]
    bool noInputDetected = false;
    [SerializeField]
    bool XaxisInUse = false;

    [SerializeField]
    Selectable leftArrowButton;
    [SerializeField]
    Selectable rightArrowButton;
    [SerializeField]
    Selectable defaultButton;
    [SerializeField]
    Selectable currentButton;
    [SerializeField]
    Selectable startButton;
    [SerializeField]
    Selectable levelSelectButton;

    [SerializeField]
    string currentSceneName;
    [SerializeField]
    sceneList scenesList; // list of scene names

    [SerializeField]
    GameObject currentObject;

    // to determine which option to change on user input 
    string startArrowLeft = "left_arrow";
    string startArrowRight = "right_arrow";
    //string optionHeader = "header";

    private void Awake()
    {

        // initialize scene name, list of scenes, find buttons and set button flags
        initializeScene();
    }

    // Use this for initialization
    void Start ()
    {
        
        // if current is start screen, need to load music
        if (currentSceneName.Equals(scenesList.startScreen))
        {
            Debug.Log("if (currentSceneName.Equals(scenesList.startScreen))");

            startMenuMusic = GameObject.FindGameObjectsWithTag("startScreenMusic");
            Debug.Log("startMenuMusic : "+ startMenuMusic.Length);

            if (startMenuMusic.Length == 0)
            {
                startMenuMusicObject = Instantiate(Resources.Load("Prefabs/start_prefabs/start_menu_music") as GameObject);
                DontDestroyOnLoad(startMenuMusicObject);
            }
        }

        // if level select button is active
        if (levelSelect)
        {
            selectLevelObject.GetComponent<Text>().text = scenesList.levelNames[sceneIndex];
        }
        // if press start button is active
        if (pressStart)
        {
            pressStartObject.GetComponent<Text>().text = "Press Start";
        }
        
        // if 'press start' is pressed, and select level enabled, get list scenes indexes
        if (selectLevelObject != null && selectLevelObject.activeSelf)
        {
            selectLevelObject.GetComponent<Text>().text = sceneNames[sceneIndex];
        }

        //todo: no idea what this does
        //check if current scene is not startscreen enable music and play it
        if (SceneManager.GetActiveScene().name != "startScreen")
        {
            Debug.Log("if (SceneManager.GetActiveScene().name != startScreen)");
            sceneIndex = 0;
            //startMenuMusic.enabled = true;
            //startMenuMusic.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        currentObject = EventSystem.current.currentSelectedGameObject;

        // if 'press start' is pressed, starts displaying of selectable levels
        if (InputManager.GetButtonDown("Submit") && pressStart)
        {
            pressStart = false;
            levelSelect = true;
            //disable start object
            pressStartObject.SetActive(false);
            //enable level select
            selectLevelObject.SetActive(true);
            selectLevelObject.GetComponent<Text>().text = sceneNames[sceneIndex];
            // set selected object to level select
            EventSystem.current.SetSelectedGameObject(selectLevelObject);
            //Destroy(startMenuMusicObject);
        }

        // if level selected and submit pressed, load selected scene
        if (InputManager.GetButtonDown("Submit") && levelSelect & noInputDetected)
        {
            // if no input and levelselect enabled, load scene
            loadScene();
        }
        // if up or down button is pressed, and level select enabled
        if ((InputManager.GetAxisRaw("Vertical")== 1 || InputManager.GetAxisRaw("Vertical")  == -1)
                && !YaxisInUse && levelSelect)
        {
            YaxisInUse = true;
            // switch to disable more than one input detection

                //Debug.Log("if (!YaxisInUse)");
                //noInputDetected = false;
                // if 'up' is pressed and not last level option in list cycle through selectable levels list
                changeCurrentUISelectedLevel();

        }
        // if not option that can be changed (navigation error or header ), disable locking boolean
        if (currentObject.name.ToLower() == startArrowLeft
            || currentObject.name.ToLower() == startArrowRight)
            //|| currentObject.name.ToLower() == optionHeader)
        {
            XaxisInUse = false;
        }
        // if current selecetd left arrow and input
        if (currentObject.name.ToLower() == startArrowLeft
            && (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Jump")))
        {
            LoadSceneLeft();
            Debug.Log("LoadSceneLeft()");
        }
        // if current selecetd right arrow and input
        if (currentObject.name.ToLower() == startArrowRight
            && (InputManager.GetButtonDown("Submit")
            || InputManager.GetButtonDown("Jump")))
        {
            LoadSceneRight();
            Debug.Log("LoadSceneRight()");
        }
        // if no input, can load level
        if (levelSelect && 
            (InputManager.GetAxisRaw("Vertical") < 0.1 &&
            InputManager.GetAxisRaw("Vertical") > -0.1) ) 
        {
            //Debug.Log("InputManager.GetAxisRaw(Vertical) :: "+ InputManager.GetAxisRaw("Vertical"));
            YaxisInUse = false;
            noInputDetected = true;
        }
    }

    void loadScene()
    {
        /* change level name to level"X"
         * ex level1, level2, etc
         * string levelToLoad = "level"+sceneindex
         * then do something like SceneManager.LoadScene(levelToLoad);
        */
        DestroyImmediate(startMenuMusicObject);

        // this can be rewritten to eliminate if statements
        Debug.Log("loadscene()  :: aduio should be destoyed");
        if (sceneIndex ==0)
        {
            //DestroyImmediate(startMenuMusicObject);
            SceneManager.LoadScene("level1");
        }
        if (sceneIndex == 1)
        {
            //Destroy(startMenuMusicObject);
            SceneManager.LoadScene("level2");
        }
        if (sceneIndex == 2)
        {
            //Destroy(startMenuMusicObject);
            SceneManager.LoadScene("level8");
        }
        if (sceneIndex == 3)
        {
            //Destroy(startMenuMusicObject);
            SceneManager.LoadScene("level7");
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
        if (currentSceneName == "startScreen")
        {
            loadHighScoresScene();
        }
    }
    void LoadSceneRight()
    {
        if (currentSceneName == "startScreen")
        {
            loadOptionsScene();
        }
    }

    void findNavigationButtons()
    {
        if (GameObject.Find(startArrowLeft) != null)
        {
            leftArrowButton = GameObject.Find(startArrowLeft).GetComponent<Selectable>();
        }
        if (GameObject.Find(startArrowRight) != null)
        {
            rightArrowButton = GameObject.Find(startArrowRight).GetComponent<Selectable>();
        }
        if (GameObject.Find("default") != null)
        {
            defaultButton = GameObject.Find("default").GetComponent<Selectable>();
        }
        if (GameObject.Find("press_start") != null)
        {
            startButton = GameObject.Find("press_start").GetComponent<Selectable>();
        }
        if (GameObject.Find("level_select") != null)
        {
            levelSelectButton = GameObject.Find("level_select").GetComponent<Selectable>();
        }
    }
    private string getCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }
    private void initializeScene()
    {
        pressStartObject = GameObject.Find("press_start");
        selectLevelObject = GameObject.Find("level_select");

        // set scene list names/values
        scenesList = gameObject.GetComponent<sceneList>();
        Debug.Log("scenesList.startScreen : " + scenesList.startScreen);
        // get current scene name
        currentSceneName = getCurrentSceneName();
        //finds and sets buttons for scene navigation
        findNavigationButtons();   

        // set flags for active buttons
        if (startButton != null
            && startButton.IsInteractable() 
            && currentSceneName == scenesList.startScreen)
        {
            pressStart = true;
        }
        else
        {
            pressStart = false;
        }
        if (!levelSelect && selectLevelObject !=null)
        {
            // if start button should be active, disable level select
            selectLevelObject.SetActive(false);
        }
        if (pressStart)
        {
            EventSystem.current.firstSelectedGameObject = pressStartObject;
            currentButton = startButton;
        }
    }
    private void changeCurrentUISelectedLevel()
    {
        Debug.Log("changeCurrentUISelectedLevel()");
        Debug.Log("         sceneIndex :: "+ sceneIndex);
        //Debug.Log("if (!YaxisInUse)");
        //noInputDetected = false;
        // if 'up' is pressed and not last level option in list cycle through selectable levels list
        if (InputManager.GetAxis("Vertical") > 0 && sceneIndex < sceneNames.Count - 1)
        {
            sceneIndex++;
            selectLevelObject.GetComponent<Text>().text = sceneNames[sceneIndex];
        }
        else
        {
            //Debug.Log("     else");
            // if 'down' is pressed, cycle through selectable levels list
            if (InputManager.GetAxis("Vertical") < 0 && sceneIndex > 0)
            {
                //Debug.Log("if (InputManager.GetAxis(Vertical) < 0 && sceneIndex > 0)");
                sceneIndex--;
                selectLevelObject.GetComponent<Text>().text = sceneNames[sceneIndex];
            }
        }
        //YaxisInUse = false;
        //noInputDetected = true;
    }
}
