
using System;
using UnityEngine;

public class CharacterProfile : MonoBehaviour
{
    [SerializeField] private int playerId;

    [SerializeField] private string playerDisplayName;
    [SerializeField] private string playerObjectName;
    [SerializeField] private Sprite playerPortrait;
    [SerializeField] private GameObject shooterProfileObject;

    private float jumpStatFloor = 3.5f;
    private float jumpStatCeiling = 6;

    private float speedStatFloor = 2.5f;
    private float speedStatCeiling = 6.5f;

    [SerializeField] private float accuracy2pt;
    [SerializeField] private float accuracy3pt;
    [SerializeField] private float accuracy4pt;
    [SerializeField] private float accuracy7pt;

    [SerializeField] private string shooterProfilePrefabName;

    [SerializeField] private float jumpForce;
    [SerializeField] private float speed;
    [SerializeField] private float runSpeedHasBall;

    [SerializeField] private float runSpeed;

    [SerializeField] private int range;
    [SerializeField] private int release;

    [SerializeField] private int luck;

    [SerializeField] private int shootAngle;

    [SerializeField] private int level;
    [SerializeField] private int experience;
    [SerializeField] private int pointsAvailable;
    [SerializeField] private int pointsUsed;
    [SerializeField] private decimal money;
    [SerializeField] private bool isFighter;
    [SerializeField] private bool isShooter;


    void Start()
    {
        // only init if level 1 or 2. other levels still for testing
        if (GameOptions.gameModeHasBeenSelected)
        {
            intializeShooterStatsFromProfile();
        }
        if (GameOptions.arcadeModeEnabled || GameOptions.difficultySelected == 0 )
        {
            Accuracy2Pt = 100;
            Accuracy3Pt = 100;
            Accuracy4Pt = 100;
            Accuracy7Pt = 100;
            Release = 100;
            Range = 150;
            Luck = 10;
        }
    }

    public CharacterProfile() { }

    private void intializeShooterStatsFromProfile()
    {
        CharacterProfile temp = LoadedData.instance.getSelectedCharacterProfile(GameOptions.characterId);

        playerObjectName = temp.playerObjectName;
        playerDisplayName = temp.playerDisplayName;
        playerId = temp.playerId;

        Speed = temp.speed;
        RunSpeed = temp.runSpeed;
        runSpeedHasBall = temp.runSpeedHasBall;

        JumpForce = temp.jumpForce;
        shootAngle = temp.shootAngle;

        Accuracy2Pt = temp.accuracy2pt;
        Accuracy3Pt = temp.accuracy3pt;
        Accuracy4Pt = temp.accuracy4pt;
        Accuracy7Pt = temp.accuracy7pt;

        Range = temp.range;
        Release = temp.release;

        experience = temp.Experience;
        level = temp.Level;
        pointsAvailable = temp.PointsAvailable;
        pointsUsed = temp.PointsUsed;

        // if 3/4/All point contest, disable Luck/citical %
        if (GameOptions.gameModeThreePointContest
            || GameOptions.gameModeFourPointContest
            || GameOptions.gameModeAllPointContest)
        {
            Luck = 0;
        }
        else
        {
            Luck = temp.Luck;
        }
        // destroy loaded data after updating stats 
        if (LoadedData.instance != null)
        {
            try
            {
                Destroy(LoadedData.instance.gameObject);
            }
            catch (Exception e)
            {
                Debug.Log("ERROR : " + e);
                return;
            }
        }
    }

    public float calculateJumpValueToPercent()
    {
        //modifier
        float modifier = 100 / ((jumpStatCeiling - jumpStatFloor) * 10);
        // percent
        float percent = (JumpForce - jumpStatFloor) * modifier * 10;
        return percent;
    }
    public float calculateSpeedToPercent()
    {
        //modifier
        float modifier = 100 / ((speedStatCeiling - speedStatFloor) * 10);
        // percent
        float percent = (runSpeed - speedStatFloor) * modifier * 10;
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
    public int Luck
    {
        get => luck;
        set => luck = value;
    }
    public int ShootAngle
    {
        get => shootAngle;
        set => shootAngle = value;
    }
    public Sprite PlayerPortrait { get => playerPortrait; set => playerPortrait = value; }
    public int PointsAvailable { get => pointsAvailable; set => pointsAvailable = value; }
    public int PointsUsed { get => pointsUsed; set => pointsUsed = value; }
    public int Range { get => range; set => range = value; }
    public int Release { get => release; set => release = value; }
    public bool IsFighter { get => isFighter; set => isFighter = value; }
    public bool IsLocked { get; internal set; }
    public bool IsShooter { get => isShooter; set => isShooter = value; }
}
