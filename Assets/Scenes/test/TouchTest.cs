using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.EnhancedTouch;
//using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;
using Touch = UnityEngine.Touch;

public class TouchTest : MonoBehaviour
{

    //protected void OnEnable()
    //{
    //    EnhancedTouchSupport.Enable();
    //}

    //protected void OnDisable()
    //{
    //    EnhancedTouchSupport.Disable();
    //}

    //protected void Update()
    //{
    //    var activeTouches = Touch.activeTouches;
    //    for (var i = 0; i < activeTouches.Count; ++i)
    //        Debug.Log("Active touch: " + activeTouches[i]);
    //}

    Touch touch;

    public void Update()
    {
        //foreach (var touch in Touch.activeTouches)
        //    Debug.Log($"{touch.touchId}: {touch.screenPosition},{touch.phase}");

        if (Input.touchCount > 0)
        {
            touch = Input.GetTouch(0);
            //Debug.Log($"{touch.touchId}: {touch.screenPosition},{touch.phase}");
            if (touch.phase == TouchPhase.Moved)
            {
                Debug.Log(touch.phase);
                Debug.Log(touch.deltaPosition.x);
                Debug.Log(touch.deltaPosition.y);
                Debug.Log(touch.position);
                Debug.Log(touch.radius);
            }
            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log(touch.phase);
                Debug.Log(touch.deltaPosition.x);
                Debug.Log(touch.deltaPosition.y);
                Debug.Log(touch.position);
                Debug.Log(touch.radius);
            }
        }
    }
    }
  



