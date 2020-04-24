using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooterProfile : MonoBehaviour
{


    public float accuracy2pt;
    public float accuracy3pt;
    public float accuracy4pt;

    public float jumpForce;
    public float speed;
    public float runSpeed;
    public float hangTime;
    public float range;
    public float release;

    public float criticalPercent;

    public float shootAngle;
    public float shootXVariance;
    public float shootYVariance;
    public float shootZVariance;

    public float Accuracy2pt { get => accuracy2pt; set => accuracy2pt = value; }
    public float Accuracy3pt { get => accuracy3pt; set => accuracy3pt = value; }
    public float Accuracy4pt { get => accuracy4pt; set => accuracy4pt = value; }
    public float JumpForce { get => jumpForce; set => jumpForce = value; }
    public float Speed { get => speed; set => speed = value; }
    public float RunSpeed { get => runSpeed; set => runSpeed = value; }
    public float ShootAngle { get => shootAngle; set => shootAngle = value; }
    public float ShootXVariance { get => shootXVariance; set => shootXVariance = value; }
    public float ShootYVariance { get => shootYVariance; set => shootYVariance = value; }
    public float ShootZVariance { get => shootZVariance; set => shootZVariance = value; }

    // need to eventually hard code stats for each shooter and load stats from file

}
