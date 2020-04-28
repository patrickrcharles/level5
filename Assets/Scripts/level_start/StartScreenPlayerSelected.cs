using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartScreenPlayerSelected : MonoBehaviour
{
    [SerializeField]
    public string playerDisplayName;
    [SerializeField]
    public string playerObjectName;

    [SerializeField]
    private Sprite playerPortrait;

    float accuracy2pt;
    float accuracy3pt;
    float accuracy4pt;

    float jumpForce;
    float speed;
    float runSpeed;
    float hangTime;

    float range;
    float release;

    public float jumpStatFloor;
    public float jumpStatCeiling;

    public float criticalPercent;

    public float  calculateJumpValueToPercent()
    {
        // ex, 4.5  - 3 = 1.5
        float modifier = 100/ (jumpStatCeiling - jumpStatFloor);
        float percent = (jumpStatCeiling - JumpForce) * modifier;
        return percent;
    }

    public string PlayerObjectName
    {
        get => playerObjectName;
        set => playerObjectName = value;
    }

    public string PlayerName
    {
        get => playerDisplayName;
        set => playerDisplayName = value;
    }

    public Sprite PlayerPortrait
    {
        get => playerPortrait;
        set => playerPortrait = value;
    }

    public float Accuracy2Pt
    {
        get => accuracy2pt;
        set => accuracy2pt = value;
    }

    public float Accuracy3Pt
    {
        get => accuracy3pt;
        set => accuracy3pt = value;
    }

    public float Accuracy4Pt
    {
        get => accuracy4pt;
        set => accuracy4pt = value;
    }

    public float JumpForce
    {
        get => jumpForce;
        set => jumpForce = value;
    }

    public float Speed
    {
        get => speed;
        set => speed = value;
    }

    public float RunSpeed
    {
        get => runSpeed;
        set => runSpeed = value;
    }

    public float HangTime
    {
        get => hangTime;
        set => hangTime = value;
    }

    public float Range
    {
        get => range;
        set => range = value;
    }

    public float Release
    {
        get => release;
        set => release = value;
    }

    public float CriticalPercent
    {
        get => criticalPercent;
        set => criticalPercent = value;
    }
}
