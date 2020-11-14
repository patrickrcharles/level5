using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanManager : MonoBehaviour
{
    GameObject basketBallGoalPosition;

    private void Start()
    {
        // position transform relative to basketball goal
        basketBallGoalPosition = GameObject.Find("rim");
        //Debug.Log("vector : " + basketBallGoalPosition.transform.position);
        transform.position = new Vector3(basketBallGoalPosition.transform.position.x, 0, basketBallGoalPosition.transform.position.z);
        //Debug.Log("vector : " + position);
        //transform.position = position;
    }
}
