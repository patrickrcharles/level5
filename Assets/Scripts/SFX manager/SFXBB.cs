using UnityEngine;
using Random = System.Random;

public class SFXBB : MonoBehaviour
{

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] musicList;
    private int currentSongIndex;

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
    public AudioClip blocked;
    public AudioClip skateGrind;
    public AudioClip glitch;
    public AudioClip turnIntoBat;
    public AudioClip airGuitar;
    public AudioClip chainRattle;
    public AudioClip deathRay;
    public AudioClip probeCritical;
    public AudioClip metalBang;
    public AudioClip stoneCold;
    public AudioClip chopWood;
    public AudioClip shootGun;
    public AudioClip takeDamage;
    public AudioClip shotgunRack;
    public AudioClip vampireHiss;
    public AudioClip projectileRocket;
    public AudioClip whipCrack;
    public AudioClip shootAutomaticAK47;

    public static SFXBB instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        playRandomSong();
    }

    public void playSFX(AudioClip audioClip)
    {
        audioSource.PlayOneShot(audioClip);
    }

    private void Update()
    {
        if (!audioSource.isPlaying || (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Alpha0)))  
        {
            playNextSong();
        }
    }

    void playRandomSong()
    {
        Random random = new Random();
        int randNum = random.Next(0, musicList.Length);
        currentSongIndex = randNum;
        audioSource.clip = musicList[currentSongIndex];
        audioSource.Play();
    }

    void playNextSong()
    {
        //int newIndex=0;
        if(currentSongIndex == (musicList.Length-1))
        {
            currentSongIndex = 0;
        }
        else
        {
            currentSongIndex++;
        }
        audioSource.clip = musicList[currentSongIndex];
        audioSource.Play();
    }
}
