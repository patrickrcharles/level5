
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JoyButton : MonoBehaviour, IPointerUpHandler, IPointerDownHandler
{

    [HideInInspector]
    protected bool pressed;

    public void OnPointerDown(PointerEventData eventData)
    {
        pressed = true;
        Debug.Log("pressed");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pressed = false;
        Debug.Log("pressed");
    }

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Button start");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
