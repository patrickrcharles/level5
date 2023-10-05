using UnityEngine;

[CreateAssetMenu]
public class CpuPlayer : ScriptableObject
{
    [SerializeField] private int playerId;

    [SerializeField] private string playerDisplayName;
    [SerializeField] private string playerObjectName;
    [SerializeField] private Sprite playerPortrait;

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
    [SerializeField] private int clutch;

    [SerializeField] private int shootAngle;

    [SerializeField] private int level;
    [SerializeField] private int experience;
    [SerializeField] private int pointsAvailable;
    [SerializeField] private int pointsUsed;
    [SerializeField] private decimal money;
    [SerializeField] private bool isFighter;
    [SerializeField] private bool isShooter;
}
