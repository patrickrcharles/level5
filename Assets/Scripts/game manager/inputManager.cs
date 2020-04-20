using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class inputManager : MonoBehaviour
{

    [SerializeField] KeyCode submit;
    [SerializeField] KeyCode cancel;

    [SerializeField] List<string> buttonNames = new List<string>();
    [SerializeField] List<KeyCode> buttons = new List<KeyCode>();
    [SerializeField] List<GameObject> displayCurrentButtonSettings = new List<GameObject>();

    private int e;
    Button currentButton;

    KeyCode currentKey;
    int i=0;

    float buttonPressedTime=0, endTime=0;

    string currentSelectedObjectName;

    bool changeButtonPressed;

    Navigation navigation;

    // Use this for initialization
    void Start()
    {

        //button = GetComponent<Button>();
        

        for(int i = 0; i < displayCurrentButtonSettings.Capacity; i++)
        {
            if (displayCurrentButtonSettings[i] != null)
            {
                displayCurrentButtonSettings[i].GetComponent<Text>().text = buttons[i].ToString();
            }
        }

        //currentKey = gameObject.name;
        submit = KeyCode.Return;
        cancel = KeyCode.Escape;
    }

    // Update is called once per frame
    void Update()
    {
        // press enter to change button for selected button
        if (Input.GetButtonDown("Submit") && !changeButtonPressed)
        {
            buttonPressedTime = Time.time;
            endTime = buttonPressedTime + 10;
            changeButtonPressed = true;

            // current button selected/highlighted
            currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

            // custom navigation to disable movement
            navigation.mode = Navigation.Mode.None;

            //disable moving from highlighted button so we can change mapping
            currentButton.navigation = navigation;

        }
        // still have 10 seconds, any key pressed, change button already selected and NOT enter or escape
        if (Time.time < endTime && Input.anyKeyDown && changeButtonPressed 
            && !Input.GetButtonDown("Cancel") && !Input.GetButtonDown("Submit"))
        {

            //Debug.Log("FetchKey() : " + FetchKey().ToString());
            //Debug.Log(" currentSelectedGameObject : " + EventSystem.current.currentSelectedGameObject.name);
            /*
            //last key pressed
            currentKey = FetchKey();
            // current button selected/highlighted
            currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            // custom navigation to disable movement
            navigation.mode = Navigation.Mode.None;
            //disable moving from highlighted button so we can change mapping
            currentButton.navigation = navigation;
            */

            currentKey = FetchKey();
            currentSelectedObjectName = EventSystem.current.currentSelectedGameObject.name;
            int indexToChange = currentSelectedObjectIndex(currentSelectedObjectName);
            //Debug.Log("currentSelectedObjectIndex(KeyCode passedKey) : " + currentSelectedObjectIndex(currentSelectedObjectName));

            buttons[indexToChange] = currentKey;
            updateButtonDisplayText();

            changeButtonPressed = false;
            // custom navigation to re-enable movement
            navigation.mode = Navigation.Mode.Automatic;

            //re-enable moving from highlighted button so we can change mapping
            currentButton.navigation = navigation;


            //get pressed key
            Debug.Log("time : " + Time.time );

        }
        if ((Time.time > endTime && changeButtonPressed) || Input.GetKey(cancel))
        {
            changeButtonPressed = false;
            // current button selected/highlighted
            currentButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

            // custom navigation to re-enable movement
            navigation.mode = Navigation.Mode.Automatic;

            //re-enable moving from highlighted button so we can change mapping
            currentButton.navigation = navigation;
        }

    }

    KeyCode FetchKey()
    {
        e = System.Enum.GetNames(typeof(KeyCode)).Length;
        for (int i = 0; i < e; i++)
        {
            if (Input.GetKey((KeyCode)i))
            {
                return (KeyCode)i;
            }
        }
        return KeyCode.None;
    }

    int currentSelectedObjectIndex(string currentObject)
    {
        Debug.Log("currentSelectedObjectIndex(KeyCode passedKey)");
        Debug.Log("currentObject : " + currentObject);
        for (int i = 0; i < buttonNames.Capacity; i++)
        {
            Debug.Log("     for(int i = 0; i < buttonNames.Capacity; i++)");
            if (currentObject == buttonNames[i] && buttonNames[i] !=null)
            {
                Debug.Log("buttonNames[i] : " + buttonNames[i]);
                Debug.Log("button pressed index : " + i);
                changeButtonPressed = true;
                return i;
            }
        }
        return 404;
    }

    void updateButtonDisplayText()
    {
        for (int i = 0; i < displayCurrentButtonSettings.Capacity; i++)
        {
            if (displayCurrentButtonSettings[i] != null)
            {
                displayCurrentButtonSettings[i].GetComponent<Text>().text = buttons[i].ToString();
            }
        }
    }
}
