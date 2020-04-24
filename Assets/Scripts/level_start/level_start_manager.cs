using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class level_start_manager : MonoBehaviour
{
    [SerializeField] private Button mode1Button;
    [SerializeField] private Button mode2Button;
    [SerializeField] private Button mode3Button;
    [SerializeField] private Button mode4Button;
    [SerializeField] private Button mode5Button;
    [SerializeField] private Button mode6Button;
    [SerializeField] private Button mode7Button;
    [SerializeField] private Button mode8Button;
    [SerializeField] private Button mode9Button;
    [SerializeField] private Button mode10Button;
    [SerializeField] private Button mode11Button;

    [SerializeField]
    private GameObject gameModeDescriptionsObject;
    private Vector3 gameModeDescriptionSpawnPoint;
    [SerializeField]
    private string currentHighlightedButton;
    [SerializeField]
    private GameObject clone;

    // Start is called before the first frame update
    void Start()
    {
        mode1Button.onClick.AddListener(delegate
        {
            loadScene("basketball test");
        });

        //temp = GameObject.Find("mode_5_description");
        gameModeDescriptionsObject = GameObject.Find("game_mode_descriptions");
        gameModeDescriptionSpawnPoint = gameModeDescriptionsObject.transform.position;

    }

    // Update is called once per frame
    void Update()
    {
        // detect if highlighted object changes
        if (EventSystem.current.currentSelectedGameObject.name != currentHighlightedButton)
        {
            if (clone != null)
            {
                GameObject tempClone = GameObject.FindWithTag("mode_description");
                Destroy(tempClone);
                //Debug.Log("highlight changed");
                string tempString = EventSystem.current.currentSelectedGameObject.name + "_description";

                //Debug.Log("tempString : " + tempString);
                string tempPath = "Prefabs/start_menu/mode_descriptions/" + tempString;
                //string tempPath = "Prefabs/start_menu/mode_descriptions/mode_10_description";

                clone = Resources.Load(tempPath) as GameObject;
                Instantiate(clone, gameModeDescriptionSpawnPoint, Quaternion.identity, gameModeDescriptionsObject.transform);
            }
            else
            {
                //Debug.Log("description created");
                string tempString = EventSystem.current.currentSelectedGameObject.name + "_description";

                //Debug.Log("tempString : " + tempString);
                string tempPath = "Prefabs/start_menu/mode_descriptions/" + tempString;
                //string tempPath = "Prefabs/start_menu/mode_descriptions/mode_10_description";

                clone = Resources.Load(tempPath) as GameObject;
                Instantiate(clone, gameModeDescriptionSpawnPoint, Quaternion.identity, gameModeDescriptionsObject.transform);
            }
        }
        currentHighlightedButton = EventSystem.current.currentSelectedGameObject.name; // + "_description";
    }

    public void loadScene(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }


}
