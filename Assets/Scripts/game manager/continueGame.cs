using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TeamUtility.IO;

public class continueGame : MonoBehaviour
{

    public bool canContinue = false;
    GameObject player;
    playercontrollerscript playerState;

    GUIStyle guiStyle = new GUIStyle();
    [SerializeField]
    bool guiOn;
    public static continueGame instance;
    public GameObject pauseMenu;
    public GameObject playerPrefab;
    public Transform playerRespawnPoint;

    public float hitboxCooldownAfterContinue;

    private void Awake()
    {
        //pauseMenu = GetComponent<pauseMenu>();
        instance = this;
    }

    // Use this for initialization
    void Start()
    {

        player = gameManager.instance.player;
        playerState = gameManager.instance.playerState;

        //Debug.Log(" continueGame.cs ------------  player : " + player.name);
    }

    // Update is called once per frame
    void Update()
    {

        //// if player can continue
        //if ((InputManager.GetButtonDown("Submit") || InputManager.GetButtonDown("Cancel")) && canContinue)
        //{
        //    //Debug.Log("if (InputManager.GetButtonUp(Submit) && canContinue)");
        //    // disable pause menu. after pressing chooseOpponent to continue game, Pause script will pick up input and pause the game
        //    // need to figure a condition to add or change the button to continue

        //    pauseMenu.SetActive(false);
        //    StartCoroutine(PlayerContinue());
        //    gameManager.instance.backgroundFade.SetActive(false);
        //}

        //// if player cant continue, but game isnt over yet (at 'game over' screen)
        //if (InputManager.GetButtonUp("Submit") && !canContinue && !gameManager.instance.gameOver && gameManager.instance.playerLives == 0)
        //{
        //    score.instance.showScore = true;
        //}
        //// if player cant continue, but game is over (at 'current score' screen)
        //if (InputManager.GetButtonUp("Submit") && !canContinue && gameManager.instance.gameOver && gameManager.instance.playerLives == 0)
        //{
        //    score.instance.restartLevel = true;
        //}
        
    }

    IEnumerator PlayerContinue()
    {
       //Debug.Log("+++++++++++++++++++++++  PRESSED ENTER TO CONTINUE ++++++++++++++++++++++++++++++++++++++");
        //decrement remaingin lives

        guiOn = false;
        gameManager.instance.previousPlayerPosition = player.transform.position;

        // write public function in playerControllerScript to do this. maybe reset all needed variable
        // * maybe resetPlayerStats() that does all this

        //player.GetComponent<Stats>().health = 0;
        //Debug.Log("player health : " + player.GetComponent<Stats>().health);


        playerState.setPlayerAnim("waiting", false);
        playerState.canMove = true;
        player.GetComponent<Rigidbody>().isKinematic = false;
        playerState.notLocked = true;

        yield return new WaitForSecondsRealtime(3);

        //undo diasble movement stuff from death coroutine
        //NOTE: triggers ONTriggerExit in groundcheck --> jump anim
        //player.GetComponent<Rigidbody>().detectCollisions = true;


        //playerState.playerHitbox.SetActive(true);
       //Debug.Log("hitbox enabled in ContinueGame.cs");

    }

    public void setCanContinue(bool continueValue)
    {
        canContinue = continueValue;
    }

    public bool getCanContinue()
    {
        return canContinue;
    }

    private void OnGUI()
    {
        if (guiOn && canContinue)
        {
            //Debug.Log("        if (guiOn && canContinue)");

            gameManager.instance.backgroundFade.SetActive(true);

            guiStyle.fontSize = 40;
            guiStyle.normal.textColor = Color.white;

            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Now you can finally put in your GUI, such as:
            GUILayout.BeginVertical();
            //GUILayout.Button("Hello, world!");
            GUILayout.Label("press chooseOpponent to continue", guiStyle);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();
        }
        if (guiOn && !canContinue)
        {
            //Debug.Log("        if (guiOn && !canContinue)");
            gameManager.instance.backgroundFade.SetActive(true);
            //gameManager.instance.gameOver = true;

            guiStyle.fontSize = 40;
            guiStyle.normal.textColor = Color.white;


            GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            // Now you can finally put in your GUI, such as:
            GUILayout.BeginVertical();
            //GUILayout.Button("Hello, world!");
            GUILayout.Label("game over", guiStyle);
            GUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndArea();

            guiStyle.fontSize = 20;
            GUILayout.BeginArea(new Rect(Screen.width * .32f, Screen.height * .6f, 400, 100));
            GUILayout.Label("press chooseOpponent to show score", guiStyle);
            GUILayout.EndArea();
        }

    }

    public void setGuiOn(bool isON)
    {
        guiOn = isON;
    }
    public bool getGuiOn()
    {
        return guiOn;
    }
}
