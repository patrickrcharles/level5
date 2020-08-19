using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    float deltaTime = 0.0f;
    Text fpsText;
    float msec;
    float fps;

    private void Awake()
    {
        fpsText = GetComponent<Text>();
    }

    void Update()
    {
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        msec = deltaTime * 1000.0f;
        fps = 1.0f / deltaTime;
        fpsText.text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);

    }

    //void OnGUI()
    //{
    //    int w = Screen.width, h = Screen.height;

    //    GUIStyle style = new GUIStyle();

    //    Rect rect = new Rect(0, 0, w, h * 2 / 50);
    //    style.alignment = TextAnchor.UpperLeft;
    //    style.fontSize = h * 2 / 50;
    //    style.normal.textColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    //    float msec = deltaTime * 1000.0f;
    //    float fps = 1.0f / deltaTime;
    //    string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    //    GUI.Label(rect, text, style);
    //}
}