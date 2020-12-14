﻿using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using Random = System.Random;

public class PlayerSwapAttack : MonoBehaviour
{

    public AnimationClip[] closeAttacks;
    [SerializeField]
    protected Animator anim;
    [SerializeField]
    AnimatorOverrideController animatorOverrideController;

    //public AnimationClip closeAttack1;
    //public AnimationClip closeAttack2;
    //public AnimationClip closeAttack3;
    public AnimationClip longRangeAttack;
    protected int index;

    public void Start()
    {
        //anim = GameLevelManager.instance.Anim;
        anim = GetComponentInChildren<Animator>();
        animatorOverrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
        index = 0;
    }

    public void setCloseAttack()
    {
        // if enemy has more than one close attack, chose random one
        if (closeAttacks.Length > 1)
        {
            Random random = new Random();
            int randomIndex = random.Next(0, closeAttacks.Length);
            animatorOverrideController["attack"] = closeAttacks[randomIndex];
            anim.runtimeAnimatorController = animatorOverrideController;
        }
        // else use default
        else
        {
            animatorOverrideController["attack"] = closeAttacks[0];
            anim.runtimeAnimatorController = animatorOverrideController;
        }
    }

    public void setLongRangeAttack()
    {
        animatorOverrideController["attack"] = longRangeAttack;
        anim.runtimeAnimatorController = animatorOverrideController;
    }
}
