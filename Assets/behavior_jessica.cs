using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class behavior_jessica : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;
    bool shotMade;
    public float percentChanceOfTakingPhoto;

    public static behavior_jessica instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
    }


    public void playAnimationTakePhoto()
    {
        if (rollForPhotoChance(percentChanceOfTakingPhoto))
        {
            StartCoroutine(wait(1));
        }
    }

    IEnumerator wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        Debug.Log("jessica take photo");
        anim.Play("takePhoto");
        audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
    }

    public bool rollForPhotoChance( float maxPercent)
    {
        float percent = Random.Range(1, 100);
        Debug.Log("jessica random percent : " + percent + " maxPercent : " + maxPercent);
        if(percent < maxPercent)
        {
            Debug.Log(" take photo");
            return true;
        }
        return false;
    }
}
