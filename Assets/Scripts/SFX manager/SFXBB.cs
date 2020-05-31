using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXBB : MonoBehaviour {

    private AudioSource audioSource;

    public AudioClip basketballBounce;
    public AudioClip basketballHitRim;
    public AudioClip basketballHitFence;
    public AudioClip basketballNetSwish;
    public AudioClip cameraFlash;
    public AudioClip alien_walk;
    public AudioClip gamechanger;
    public AudioClip werewolfHowl;
    public AudioClip worker_parasite;
    public AudioClip airhorn;

    //public List<AudioClip> vanMusicPlayList;

    public static SFXBB Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

}
