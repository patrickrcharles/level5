using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startChooseOpponentAnimations : MonoBehaviour {

    [SerializeField]
    Animator anim;
	// Use this for initialization
	void Start () {

        anim = gameObject.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		
        if(!anim.isActiveAndEnabled)
        {
            anim.enabled = true;
        }
	}
}
