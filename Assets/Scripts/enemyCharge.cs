using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyCharge : MonoBehaviour {


    public GameObject animatorParent;
    public GameObject animatorObject;
    float XchangeInPosition;

    // Use this for initialization
    void Start()
    {
        // Animator anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("------------------------BEFORE ASSIGNMENT---------------------------------------------");
        Debug.Log("----------PARENT transform : " + animatorParent.transform.position);
        Debug.Log("==========ANIMATOR transform : " + animatorObject.transform.position);
        Debug.Log("==========XchangeInPosition : " + XchangeInPosition);

        //XchangeInPosition = animatorObject.transform.position.x - animatorParent.transform.position.x;

        //animatorParent.transform.position = animatorObject.transform.position;
        /*
        animatorParent.transform.position = 
            new Vector3((animatorParent.transform.position.x + XchangeInPosition),
                animatorParent.transform.position.y,
                animatorParent.transform.position.z);
        */

        Debug.Log("------------------------AFTER ASSIGNMENT---------------------------------------------");
        Debug.Log("----------PARENT transform : " + animatorParent.transform.position);
        Debug.Log("==========ANIMATOR transform : " + animatorObject.transform.position);
        Debug.Log("==========XchangeInPosition : " + XchangeInPosition);
    }

    void updateParentTransform()
    {
        XchangeInPosition = animatorObject.transform.position.x - animatorParent.transform.position.x;
        animatorParent.transform.position = animatorObject.transform.position;
    }
}
