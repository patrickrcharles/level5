using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TeamUtility.IO;

public class pauseMenu : MonoBehaviour
{
    public bool paused = false;
    private AudioSource[] allAudioSources;
    private GUIStyle guiStyle = new GUIStyle();  //create a new variable
    public continueGame continueGame;

    public static pauseMenu instance;
    public GameObject controlsText;
    //GameObject[] rakes;

    private void Start()
    {
        controlsText.SetActive(false);
        //rakes = GameObject.FindGameObjectsWithTag("rake");
    }

    void Awake()
    {    
        //allAudioSources = FindObjectsOfType(AudioSource) as AudioSource[];
        allAudioSources = FindObjectsOfType<AudioSource>();
        //continueGame = GameObject.Find("continue").GetComponent<continueGame>();
        instance = this;
    }

    void Update()
    {

        if ((InputManager.GetButtonDown("Submit") 
            || InputManager.GetButtonDown("Cancel") 
            || InputManager.GetKeyDown(KeyCode.Escape)))
        {
           //Debug.Log("pauseMenu.cs :: if (submit, cancel, esc");
            paused = togglePause();
        }

        //if (InputManager.GetButton("Jump") && InputManager.GetButton("Fire3") && paused)
        //{
        //    //Debug.Log("pauseMenu.cs :::::::: if (InputManager.GetButton(Jump) && InputManager.GetButton(Fire3) && paused)");
        //    //Quit();
        //    // reset current score
        //    //score.instance.setCurrentScore(0);

        //    //destroy levelmanger because going to chooseOpponent screen
        //    GameObject[] lvlmanger = GameObject.FindGameObjectsWithTag("levelManager");
        //    foreach (GameObject lvl in lvlmanger)
        //    {
        //        Destroy(lvl);
        //       //Debug.Log("lvlmanger " + lvl.name);
        //    }


        //    // destroy player because player bounds needs to be reset for each level
        //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //    foreach (GameObject player in players)
        //    {
        //        Destroy(player);
        //       //Debug.Log("player " + player.name);
        //    }

        //    SceneManager.LoadScene("startScreen");

        //    // reset level manager instance

        //}
        //if (InputManager.GetButton("Fire1") && InputManager.GetButton("Fire2") && paused)
        //{
        //    //Debug.Log("pauseMenu.cs :::::::: if (InputManager.GetButton(Fire1) && InputManager.GetButton(Fire2) && paused)");
        //    // reset current score
        //    //score.instance.setCurrentScore(0);

        //    //destroy levelmanger because going to chooseOpponent screen
        //    GameObject[] lvlmanger = GameObject.FindGameObjectsWithTag("levelManager");
        //    foreach (GameObject lvl in lvlmanger)
        //    {
        //        Destroy(lvl);
        //       //Debug.Log("lvlmanger " + lvl.name);
        //    }
            

        //    // destroy player because player bounds needs to be reset for each level
        //    GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        //    foreach (GameObject player in players)
        //    {
        //        Destroy(player);
        //       //Debug.Log("player " + player.name);
        //    }

        //    SceneManager.LoadScene("startScreen");
        //    // reset level manager instance   
        //    // add score to DB before restart
        //    //timer.GetComponent<countdown>()

        //    // reset current score

        //   // score.instance.setCurrentScore(0);

        //    //reset scene vars. gameover and showScore in gamemanager. will disable score overlay.
        //    gameManager.instance.resetSceneVariables();

        //    //could destroy levelmanager object but i dont think i need to since it's the same level
        //    //StartCoroutine(Wait(1));

        //   //Debug.Log("reload scene");
        //    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //}

    
        /*
        if (InputManager.GetKey(KeyCode.F) && InputManager.GetKeyDown(KeyCode.U))
        {
            
            foreach(GameObject rke in rakes)
            {
                if(rke.activeSelf)
                {
                    rke.SetActive(false);
                    score.instance.rakesDisabled = true;
                }
                else if(!rke.activeSelf)
                {
                    rke.SetActive(true);
                }
            }
        }
        */
        
    }

    bool togglePause()
    {
        if (Time.timeScale == 0f)
        {
            gameManager.instance.backgroundFade.SetActive(false);

            //controlsText.SetActive(false);
            Time.timeScale = 1f;
            resumeAllAudio();
            return (false);
        }
        else
        {
            gameManager.instance.backgroundFade.SetActive(true);
            //controlsText.SetActive(true);
            Time.timeScale = 0f;
            pauseAllAudio();
            return (true);
        }
    }

   

    void pauseAllAudio()
    {
        foreach (AudioSource audioS  in allAudioSources)
        {
            //audioS.Stop();
            audioS.Pause();
        }
    }

    void resumeAllAudio()
    {
        foreach (AudioSource audioS in allAudioSources)
        {
            //audioS.Stop();
            audioS.UnPause();
        }
    }

    private void Quit()
    {
        Application.Quit();
    }
}