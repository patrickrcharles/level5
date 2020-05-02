using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFX : MonoBehaviour {

    private AudioSource audioSource;
    public AudioClip basketballBounce;
    public AudioClip basketballHitRim;

    public static SFX Instance;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
}
