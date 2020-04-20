using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class animatorForward : MonoBehaviour
{

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
        //Debug.Log("------------------------BEFORE ASSIGNMENT---------------------------------------------");
        //Debug.Log("----------PARENT transform : " + animatorParent.transform.position);
        //Debug.Log("==========ANIMATOR transform : " + animatorObject.transform.position);
        //Debug.Log("==========XchangeInPosition : " + XchangeInPosition);
        
        XchangeInPosition = animatorObject.transform.position.x - animatorParent.transform.position.x ;
        /*
        animatorParent.transform.position =
            new Vector3((animatorParent.transform.position.x + XchangeInPosition),
                animatorParent.transform.position.y,
                animatorParent.transform.position.z);


        //Debug.Log("------------------------AFTER ASSIGNMENT---------------------------------------------");
        //Debug.Log("----------PARENT transform : " + animatorParent.transform.position);
        //Debug.Log("==========ANIMATOR transform : " + animatorObject.transform.position);
        //Debug.Log("==========XchangeInPosition : " + XchangeInPosition);

        */
    }


    /*
    public void OnAnimatorMove()
    {
        //Animator anim = GetComponent<Animator>();
        //transform.parent.rotation = anim.rootRotation;

        //animatorParent.transform.position = anim.rootPosition;

        //animatorParent.transform.position = animatorObject.transform.position;

        //Debug.Log("----------PARENT transform : " + animatorParent.transform.position);
        //Debug.Log("==========ANIMATOR transform : " + animatorObject.transform.position);
        Debug.Log("++++++++++anim.rootPosition : " + anim.rootPosition);
        Debug.Log("^^^^^^^^^^anim.deltaPosition : " + anim.deltaPosition);

        Debug.Log("anim.deltaPosition : " + anim.deltaPosition);

        /*
            //transform.parent.rotation = anim.rootRotation;
            animatorParent.transform.position = anim.transform.position;

            
          
    */
    // //Debug.Log("anim.transform.position : " + anim.transform.position);
}
   

