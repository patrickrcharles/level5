﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenModeSelected : MonoBehaviour
{

    [SerializeField] private int modeId;
    [SerializeField] private string modelDisplayName;
    [SerializeField] private string modeObjectName;
    [SerializeField] private string modeDescription;
    [SerializeField] private bool modeRequiresCounter;
    [SerializeField] private bool modeRequiresCountDown;

    [SerializeField] private bool modeRequiresShotMarkers3s;
    [SerializeField] private bool modeRequiresShotMarkers4s;

    [SerializeField] private bool gameModeThreePointContest;
    [SerializeField] private bool gameModeFourPointContest;
    [SerializeField] private bool gameModeAllPointContest;
    [SerializeField] private float customTimer;

    [SerializeField] private bool modeRequiresMoneyBall;

    [SerializeField] private bool modeRequiresConsecutiveShots;

    [SerializeField] private string highScoreField;



    public bool ModeRequiresMoneyBall => modeRequiresMoneyBall;

    public bool ModeRequiresShotMarkers3S
    {
        get => modeRequiresShotMarkers3s;
    }

    public bool ModeRequiresShotMarkers4S
    {
        get => modeRequiresShotMarkers4s;
    }


    public bool ModeRequiresCounter
    {
        get => modeRequiresCounter;
    }

    public bool ModeRequiresCountDown
    {
        get => modeRequiresCountDown;

    }

    public int ModeId
    {
        get => modeId;
    }

    public string ModelDisplayName
    {
        get => modelDisplayName;
    }

    public string ModeObjectName
    {
        get => modeObjectName;
    }

    public string ModeDescription
    {
        get => modeDescription;
    }
    public bool ModeRequiresConsecutiveShots { get => modeRequiresConsecutiveShots; set => modeRequiresConsecutiveShots = value; }
    public string HighScoreField { get => highScoreField; set => highScoreField = value; }
    public bool GameModeThreePointContest { get => gameModeThreePointContest; set => gameModeThreePointContest = value; }
    public bool GameModeFourPointContest { get => gameModeFourPointContest; set => gameModeFourPointContest = value; }
    public bool GameModeAllPointContest { get => gameModeAllPointContest; set => gameModeAllPointContest = value; }
    public float CustomTimer { get => customTimer; set => customTimer = value; }
}
