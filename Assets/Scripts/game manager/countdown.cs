using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class countdown : MonoBehaviour
{
    float timeRemaining = 0;
    //GUIStyle guiStyle = new GUIStyle();
    int minutes = 0;
    int seconds = 0;
    bool displayTimer = true;
    Text timerText;

    private void Awake()
    {
        timerText = GetComponent<Text>();
    }

    void Update()
    {
        timeRemaining +=  Time.deltaTime;
        seconds = Mathf.FloorToInt(timeRemaining);


        if (seconds > 60 && displayTimer )
        {
            minutes = Mathf.FloorToInt(seconds / 60); 
            seconds = seconds - (minutes*60);
        }

        if (displayTimer)
        {
            if (timeRemaining > 0 && timeRemaining < 600)
            {
               
                if (minutes > 10 && seconds > 10)
                {
                   timerText.text = minutes + " : " + seconds;
                }
                if (minutes < 10 && seconds < 10)
                {
                    timerText.text = "0" + minutes + " : 0" + seconds;
                }
                if (minutes < 10 && seconds > 9)
                {
                    timerText.text = "0" + minutes + " : " + seconds;
                }
                if (minutes > 9 && seconds < 10)
                {
                    timerText.text = minutes + " : 0" + seconds;
                }
            }
        }

        else if (timeRemaining >= 600)
        {
            //GUI.Label(new Rect(100, 100, 200, 100), "Time's Up");

            gameManager.instance.gameOver = true;
            gameManager.instance.showScore = true;
            displayTimer = false;
//            setScoreTime();
            Time.timeScale = 0.01f;
        }
        // Debug.Log(" Mathf.Abs(timeRemaining)" + Mathf.FloorToInt(timeRemaining));
    }

    void OnGUI()
    {
        /*
        if (displayTimer)
        {
            if (timeRemaining > 0 && timeRemaining < 600 )
            {
                guiStyle.fontSize = 25;
                guiStyle.normal.textColor = Color.white;

                if (minutes > 10 && seconds > 10)
                {
                    GUI.Label(new Rect(Screen.width * .5f, 50, 200, 100),
                                 "Timer : " + minutes + " : " + seconds, guiStyle);
                }
                if (minutes < 10 && seconds < 10)
                {
                    GUI.Label(new Rect(Screen.width * .5f, 50, 200, 100),
                                 "Timer : 0" + minutes + " : 0" + seconds, guiStyle);
                }
                if (minutes < 10 && seconds > 9)
                {
                    GUI.Label(new Rect(Screen.width * .5f, 50, 200, 100),
                                 "Timer : 0" + minutes + " : " + seconds, guiStyle);
                }
                if (minutes > 9 && seconds < 10)
                {
                    GUI.Label(new Rect(Screen.width * .5f, 50, 200, 100),
                                 "Timer : " + minutes + " : 0" + seconds, guiStyle);
                }
            }
        }
        
        else if (!gameManager.instance.gameOver && timeRemaining >= 600)
        {
            //GUI.Label(new Rect(100, 100, 200, 100), "Time's Up");

            gameManager.instance.gameOver = true;
            gameManager.instance.showScore = true;
            displayTimer = false;
            setScoreTime();
            Time.timeScale = 0.01f;   
        }
        */
    }

    //public void setScoreTime()
    //{

    //    if (minutes > 10 && seconds > 9)
    //    {
    //        score.instance.setTime( minutes + " : " + seconds);
            
    //    }
    //    if (minutes < 10 && seconds < 10)
    //    {

    //        score.instance.setTime("0" + minutes + " : 0" + seconds);
    //    }
    //    if (minutes < 10 && seconds > 9)
    //    {

    //        score.instance.setTime("0" + minutes + " : " + seconds);
    //    }
    //    if (minutes > 9 && seconds < 10)
    //    {
    //        score.instance.setTime(minutes + " : 0" + seconds);
    //    }

    //    Debug.Log("score.instance.getTime :" + score.instance.getTime());
    //}
}