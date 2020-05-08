using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ShooterProfile : MonoBehaviour
{
    [SerializeField] private int playerId;

    [SerializeField] private string playerDisplayName;
    [SerializeField] private string playerObjectName;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private GameObject shooterProfileObject;

    [SerializeField] private float jumpStatFloor;
    [SerializeField] private float jumpStatCeiling;

    [SerializeField] private float speedStatFloor;
    [SerializeField] private float speedStatCeiling;

    [SerializeField] private float accuracy2pt;
    [SerializeField] private float accuracy3pt;
    [SerializeField] private float accuracy4pt;
    [SerializeField] private float accuracy7pt;

    [SerializeField] private string shooterProfilePrefabName;

    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float runSpeedHasBall;

    [SerializeField] private float runSpeed;
    [SerializeField] private float hangTime;
    [SerializeField] private float range;
    [SerializeField] private float release;
    [SerializeField] private float criticalPercent;

    [SerializeField] private float shootAngle;

    [SerializeField] private int level;
    [SerializeField] private int experience;
    [SerializeField] private decimal money;


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

        // only init if level 1 or 2. other levels still for testing
        if (GameOptions.levelSelectedBuildIndex == 1
            || GameOptions.levelSelectedBuildIndex ==2
            || GameOptions.levelSelectedBuildIndex ==3
             && GameOptions.gameModeHasBeenSelected)
        {
            intializeShooterStatsFromProfile();
        }
    }

    private void intializeShooterStatsFromProfile()
    {
        //Debug.Log("initializeStats()");

        playerObjectName = GameOptions.playerObjectName;
        playerDisplayName = GameOptions.playerDisplayName;
        playerId = GameOptions.playerId;

        runSpeedHasBall = GameOptions.runSpeedHasBall;

        Accuracy2Pt = GameOptions.accuracy2pt;
        Accuracy3Pt = GameOptions.accuracy3pt;
        Accuracy4Pt = GameOptions.accuracy4pt;
        Accuracy7Pt = GameOptions.accuracy7pt;

        JumpForce = GameOptions.jumpForce;
        CriticalPercent = GameOptions.criticalPercent;
        RunSpeed = GameOptions.runSpeed;
        Speed = GameOptions.speed;
    }

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

    public float calculateSpeedToPercent()
    {

        //modifier
        float modifier = 100 / ((speedStatCeiling - speedStatFloor) * 10);
        // percent
        float percent = (runSpeed - speedStatFloor) * modifier * 10;

        //Debug.Log(jumpStatCeiling - jumpStatFloor);
        //Debug.Log(" modifier : "+ modifier + "      percent : "+ percent);


        return percent;
    }

    public float RunSpeedHasBall
    {
        get => runSpeedHasBall;
        set => runSpeedHasBall = value;
    }

    public int PlayerId
    {
        get => playerId;
        set => playerId = value;
    }

    public int Level
    {
        get => level;
        set => level = value;
    }

    public int Experience
    {
        get => experience;
        set => experience = value;
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

    public float Accuracy7Pt
    {
        get => accuracy7pt;
        set => accuracy7pt = value;
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
