using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class uiFollowTarget : MonoBehaviour {


    public Transform target;
    Vector3 pos;
    public bool dropShadow;
    //RectTransform rect;
    //ublic GameObject gameObject;



    void Update()
    {
        //Camera tracks to target position


        pos = Camera.main.WorldToScreenPoint(target.position);
        transform.position = pos;
    }
}
