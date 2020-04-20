using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerAnimations : MonoBehaviour {


    public AnimationClip attack1, attack2, attack3, attack4, death, disintegrated, climbLedge, climbRoof,
    smokeBomb, groundShockwave, flamethrower, rocketLauncher, stepOnRake, throwMolotov, swingWhip,
    shocked;

    public static playerAnimations Instance;

    private void Awake()
    {
        Instance = this;
    }
}
