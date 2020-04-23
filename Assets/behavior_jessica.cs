using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class behavior_jessica : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;
    bool shotMade;
    public float percentChanceOfTakingPhoto;
    public Animator animOnCamera;
    playercontrollerscript playerState;
    public static behavior_jessica instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        animOnCamera = GameObject.Find("camera_flash").GetComponent<Animator>();
        playerState = gameManager.instance.playerState;
    }


    public void playAnimationTakePhoto()
    {
        if (rollForPhotoChance(percentChanceOfTakingPhoto) && playerState.playerDistanceFromRim < 10)
        {
           //Debug.Log("percentChanceOfTakingPhoto : " + percentChanceOfTakingPhoto);
            //StartCoroutine(wait(1));
            takePhoto();
        }
    }

    IEnumerator wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
       //Debug.Log("jessica take photo");
        anim.Play("takePhoto");
        audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
    }

    private void takePhoto()
    {
       //Debug.Log("jessica take photo");
        anim.Play("takePhoto");
        audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
    }

    public bool rollForPhotoChance( float maxPercent)
    {
        float percent = Random.Range(1, 100);
       //Debug.Log("jessica random percent : " + percent + " maxPercent : " + maxPercent);
        if(percent <= maxPercent)
        {
           //Debug.Log(" jessica takes a photo");
            return true;
        }
        return false;
    }
    private void playAnimationCameraFlash()
    {
        animOnCamera.Play("camera_flash");
    }
}
