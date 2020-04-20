﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class interact_jdrac : MonoBehaviour {

    Text text;
    public List<string> phrases = new List<string>();
    Random random;
    int ranNum;
    public GameObject textObject;
    GameObject rootGameObject;

	// Use this for initialization
	void Start ()
    {

        text = textObject.GetComponent<Text>();
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void Awake()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.tag == "enemyBystander" && other.tag == "playerHitbox")
        {
            ranNum = Random.Range(0, 6);
            /*
            //Debug.Log("phrases[].size   : " + phrases.Capacity);
            //Debug.Log("random # : " + ranNum);
            for (int i = 0; i < phrases.Capacity; i++)
            {
                //Debug.Log(" phrases[" + i + "] : " + phrases[i]);
            }
            */
            text.text= phrases[ranNum];
        }

    }

    private void OnTriggerExit(Collider other)
    {
        text.text = null;
    }
}
