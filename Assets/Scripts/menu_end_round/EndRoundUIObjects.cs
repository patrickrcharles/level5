using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndRoundUIObjects : MonoBehaviour
{
    [SerializeField]
    Button nextRoundButton;
    [SerializeField]
    public Text nextRoundText;
    [SerializeField]
    Button continueButton;
    [SerializeField]
    Button startMenuButton;
    [SerializeField]
    Button QuitButton;
    [SerializeField]
    public Text nextRoundOpponent;
    [SerializeField]
    public Text nextRoundLevel;
    [SerializeField]
    public Text currentRoundWinnerIsCpu;
    [SerializeField]
    public Text currentRoundLoserIsCpu;
    [SerializeField]
    public Text currentRoundWinnerScore;
    [SerializeField]
    public Text currentRoundLoserScore;
    [SerializeField]
    public Text continueTimer;
    [SerializeField]
    public Text continueNumber;
    [SerializeField]
    public Image currentRoundWinnerImage;
    [SerializeField]
    public Image currentRoundLoserImage;
    [SerializeField]
    public GameObject continueOptionObject;

    public static EndRoundUIObjects instance;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }


}
