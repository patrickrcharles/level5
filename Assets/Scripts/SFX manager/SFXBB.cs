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
    public AudioClip lightningStrike;
    public AudioClip rimShot;
    public AudioClip knockedDown;
    public AudioClip skateGrind;
    public AudioClip glitch;

    public AudioClip unlockAchievement;
    

    //public List<AudioClip> vanMusicPlayList;

    public static SFXBB instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    public void playSFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }
}
