using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class startMenuMusic : MonoBehaviour {

    private AudioSource audioSource;
    private static startMenuMusic instance;
    private bool startScreenMusicOn;

    // Use this for initialization
    void Awake () {
        
        instance = this;
        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;

        //check if current scene is start screen and enable music and play it
        if  (!startScreenMusicOn)
        {
            startScreenMusicOn = true;
            audioSource.enabled = true;
            audioSource.Play();
        }
        // dont destroy music for startscreen, options, etc
        //DontDestroyOnLoad(instance.gameObject);
    }

    void Start()
    { 
        /*
        // find music object and disable
        // this has somethng to do with if music has already started or not
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        */
    }
}
