using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TeamUtility.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartManager : MonoBehaviour
{
    //private GameObject gameModeNameObject;
    //private Vector3 gameModeNameSpawnPoint;

    //[SerializeField]
    //private GameObject[] gameModeNamesList;

    //private GameObject gameModeDescriptionsObject;
    //private Vector3 gameModeDescriptionSpawnPoint;

    [SerializeField]
    private string currentHighlightedButton;

    [SerializeField]
    private List<StartScreenPlayerSelected> playerSelectedData;

    private Text playerSelectText;
    private Image playerSelectImage;
    private Text playerSelectStatsText;

    private Text levelSelectText;

    //private Image levelSelectImage;

    private const string startObjectName = "press_start";

    //private Text gameModeSelectText;
    void Awake()
    {
        loadPlayerSelectDataList();

    }

    // Start is called before the first frame update
    void Start()
    {
        initiazePlayerText();
        GameOptions.playerSelected = playerSelectedData[0].PlayerObjectName;
        GameOptions.levelSected = levelSelectText.text;

        //string gamesModeNamesPath = "Prefabs/start_menu/mode_names";
        //gameModeNamesList =Resources.LoadAll<GameObject>(gamesModeNamesPath);

        ////temp = GameObject.Find("mode_5_description");
        //gameModeDescriptionsObject = GameObject.Find("game_mode_descriptions");
        //gameModeDescriptionSpawnPoint = gameModeDescriptionsObject.transform.position;
        //gameModeNameObject = GameObject.Find("game_mode_name");        //gameModeNamesList =Resources.LoadAll<GameObject>(gamesModeNamesPath);

        ////temp = GameObject.Find("mode_5_description");
        //gameModeDescriptionsObject = GameObject.Find("game_mode_descriptions");
        //gameModeDescriptionSpawnPoint = gameModeDescriptionsObject.transform.position;
        //gameModeNameObject = GameObject.Find("game_mode_name");
    }

    private void initiazePlayerText()
    {
        playerSelectText = GameObject.Find("player_selected_name").GetComponent<Text>();
        playerSelectStatsText = GameObject.Find("player_selected_stats_numbers").GetComponent<Text>();
        playerSelectImage = GameObject.Find("player_selected_image").GetComponent<Image>();

        levelSelectText = GameObject.Find("level_selected_name").GetComponent<Text>();

        playerSelectText.text = playerSelectedData[0].PlayerName;
        playerSelectImage.sprite = playerSelectedData[0].PlayerPortrait;
        playerSelectStatsText.text = playerSelectedData[0].Accuracy2Pt.ToString() + "\n"
            + playerSelectedData[0].Accuracy3Pt.ToString() + "\n"
            + playerSelectedData[0].Accuracy4Pt.ToString() + "\n"
            + playerSelectedData[0].calculateJumpValueToPercent().ToString() + "\n"
            + playerSelectedData[0].Range.ToString() + "\n"
            + playerSelectedData[0].CriticalPercent.ToString();

        //levelSelectText.text 
    }

    private void loadPlayerSelectDataList()
    {
        Debug.Log("loadPlayerSelectDataList()");

        string path = "Prefabs/start_menu/player_selected_objects";
        GameObject[] objects = Resources.LoadAll<GameObject>(path) as GameObject[];

        foreach (GameObject obj in objects)
        {
            StartScreenPlayerSelected temp = obj.GetComponent<StartScreenPlayerSelected>();
            playerSelectedData.Add(temp);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //// detect if highlighted object changes
        //if (EventSystem.current.currentSelectedGameObject.name != currentHighlightedButton)
        //{
        //    if (cloneDescription != null)
        //    {
        //        getNextModeDecription();
        //    }
        //    else
        //    {
        //        createFirstModeDescription();
        //    }
        //}
        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
        //Debug.Log("current : "+ currentHighlightedButton);

        if ( (InputManager.GetKeyDown(KeyCode.Return) || InputManager.GetKeyDown(KeyCode.Space) ) 
            && currentHighlightedButton.Equals(startObjectName) )
        {
            Debug.Log("pressed enter");
            loadScene();
        }
    }



    //private void createFirstModeDescription()
    //{

    //    //Debug.Log("description created");
    //    string tempString = EventSystem.current.currentSelectedGameObject.name + "_description";

    //    //Debug.Log("tempString : " + tempString);
    //    string tempPath = "Prefabs/start_menu/mode_descriptions/" + tempString;
    //    //string tempPath = "Prefabs/start_menu/mode_descriptions/mode_10_description";

    //    cloneDescription = Resources.Load(tempPath) as GameObject;
    //    Instantiate(cloneDescription, gameModeDescriptionSpawnPoint, Quaternion.identity, gameModeDescriptionsObject.transform);
    //}

    //private void getNextModeDecription()
    //{
    //    GameObject tempClone = GameObject.FindWithTag("mode_description");
    //    Destroy(tempClone);
    //    //Debug.Log("highlight changed");
    //    string tempString = EventSystem.current.currentSelectedGameObject.name + "_description";

    //    //Debug.Log("tempString : " + tempString);
    //    string tempPath = "Prefabs/start_menu/mode_descriptions/" + tempString;
    //    //string tempPath = "Prefabs/start_menu/mode_descriptions/mode_10_description";

    //    cloneDescription = Resources.Load(tempPath) as GameObject;
    //    Instantiate(cloneDescription, gameModeDescriptionSpawnPoint, Quaternion.identity, gameModeDescriptionsObject.transform);
    //}

    public void loadScene()
    {
        SceneManager.LoadScene("basketball test");
    }


}
