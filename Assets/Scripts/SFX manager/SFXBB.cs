using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBB : MonoBehaviour {

    private AudioSource audioSource;

    public AudioClip basketballBounce;
    public AudioClip basketballHitRim;
    public AudioClip basketballHitFence;
    public AudioClip basketballNetSwish;

    //public List<AudioClip> vanMusicPlayList;

    public static SFXBB Instance;

    private void Awake()
    {
        Instance = this;
        //Debug.Log("flamethrower_idle.length" + flamethrower_idle.length);
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        //audioSource.volume = 0;
    }

}
