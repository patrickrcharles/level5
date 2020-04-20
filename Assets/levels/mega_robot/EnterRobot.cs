using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnterRobot : MonoBehaviour {


    playercontrollerscript playerState;

	// Use this for initialization
	void Start ()
    {
        playerState = gameManager.instance.playerState;
	}
	


    void OnTriggerEnter(Collider other)
    {
        if( gameObject.name.Contains("playerEnterRobot") && other.CompareTag("playerHitbox"))
        {
            SceneManager.LoadScene("level1_robot");
        }

    }
}
