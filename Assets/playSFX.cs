using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playSFX : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip fart;
    public AudioClip attack;

    //public List<AudioClip> vanMusicPlayList;

    public static playSFX Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void playFartSound()
    {
        audioSource.PlayOneShot(fart);
    }

    void playAttackSound()
    {
        audioSource.PlayOneShot(attack);
    }

}
