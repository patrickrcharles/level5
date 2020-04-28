﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooterProfile : MonoBehaviour
{
    [SerializeField] private string playerDisplayName;
    [SerializeField] private string playerObjectName;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private GameObject shooterProfileObject;

    [SerializeField] private float jumpStatFloor;
    [SerializeField] private float jumpStatCeiling;
    [SerializeField] private float accuracy2pt;
    [SerializeField] private float accuracy3pt;
    [SerializeField] private float accuracy4pt;

    [SerializeField] private string shooterProfilePrefabName;

    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float runSpeed;
    [SerializeField] private float hangTime;
    [SerializeField] private float range;
    [SerializeField] private float release;
    [SerializeField] private float criticalPercent;

    [SerializeField] private float shootAngle;


    //private float shootXVariance;
    //private float shootYVariance;
    //private float shootZVariance;

    void Awake()
    {
        //Debug.Log("StartScreenPlayerSelected");
        //shooterProfilePrefabName = "player" + playerDisplayName;

        //Debug.Log(shooterProfilePrefabName);
        //shooterProfileObject = Resources.Load("Prefabs /characters/players/" + shooterProfilePrefabName) as GameObject;
        //Instantiate(shooterProfileObject);

        ////basketball = Resources.Load("Prefabs/objects/basketball_nba") as GameObject;
       // intializeShooterStatsFromProfile();
    }

    //private void intializeShooterStatsFromProfile()
    //{
    //    Debug.Log("initializeStats()");
    //    Accuracy2Pt = shooterProfile.Accuracy2pt;
    //    Accuracy3Pt = shooterProfile.Accuracy3pt;
    //    Accuracy4Pt = shooterProfile.Accuracy4pt;
    //    JumpForce = shooterProfile.JumpForce;
    //    Debug.Log(shooterProfile.JumpForce);
    //    CriticalPercent = shooterProfile.criticalPercent;

    //}

    public float calculateJumpValueToPercent()
    {

        //modifier
        float modifier = 100 / ((jumpStatCeiling - jumpStatFloor) * 10);
        // percent
        float percent = (JumpForce - jumpStatFloor) * modifier * 10;

        //Debug.Log(jumpStatCeiling - jumpStatFloor);
        //Debug.Log(" modifier : "+ modifier + "      percent : "+ percent);

        return percent;
    }

    public string PlayerDisplayName
    {
        get => playerDisplayName;
        set => playerDisplayName = value;
    }

    public string PlayerObjectName
    {
        get => playerObjectName;
        set => playerObjectName = value;
    }

    public Sprite PlayerPortrait
    {
        get => playerPortrait;
        set => playerPortrait = value;
    }

    public GameObject ShooterProfileObject
    {
        get => shooterProfileObject;
        set => shooterProfileObject = value;
    }

    public float JumpStatFloor
    {
        get => jumpStatFloor;
        set => jumpStatFloor = value;
    }

    public float JumpStatCeiling
    {
        get => jumpStatCeiling;
        set => jumpStatCeiling = value;
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

    public string ShooterProfilePrefabName
    {
        get => shooterProfilePrefabName;
        set => shooterProfilePrefabName = value;
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

    public float ShootAngle
    {
        get => shootAngle;
        set => shootAngle = value;
    }
}
