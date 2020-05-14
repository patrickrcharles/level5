using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorNpcCritical : MonoBehaviour
{
    Animator anim;
    AudioSource audioSource;
    bool shotMade;
    public float percentChanceOfCritical;
    public Animator animOnCamera;
    PlayerController playerState;

    private string npcName;
    public static BehaviorNpcCritical instance;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        audioSource = GetComponent<AudioSource>();
        anim = GetComponent<Animator>();
        animOnCamera = GameObject.Find("camera_flash").GetComponent<Animator>();
        playerState = GameLevelManager.Instance.PlayerState;
        npcName = gameObject.transform.root.name;
        Debug.Log("npc name  " + npcName);
    }


    public void rollForCritical()
    {
        if (rollForPhotoChance(percentChanceOfCritical))// && playerState.playerDistanceFromRim < 10)
        {
           //Debug.Log("percentChanceOfTakingPhoto : " + percentChanceOfTakingPhoto);
            //StartCoroutine(wait(1));
            playCriticalSuccessfulAnim();
        }
    }

    public void playAnimationCriticalSuccesful()
    {
        playCriticalSuccessfulAnim();
    }

    IEnumerator wait(float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
       //Debug.Log("jessica take photo");
        anim.Play("critical_success");
        audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
    }

    private void playCriticalSuccessfulAnim()
    {
       //Debug.Log("jessica take photo");
        anim.Play("critical_success");

        //audioSource.PlayOneShot(SFXBB.Instance.cameraFlash);
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
