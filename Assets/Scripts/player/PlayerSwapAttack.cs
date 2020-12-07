using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

public class PlayerSwapAttack : MonoBehaviour
{

    public AnimationClip[] attackAnimations;
    protected Animator anim;
    AnimatorOverrideController animatorOverrideController;

    protected int index;

    public void Start()
    {
        anim = GameLevelManager.instance.Anim;
        animatorOverrideController = anim.runtimeAnimatorController as AnimatorOverrideController;
        index = 0;
    }

    public void Update()
    {
        if (GameLevelManager.instance.Controls.Other.change.enabled && Input.GetKeyDown(KeyCode.Alpha7))
        {
            index++;
            if (index > attackAnimations.Length-1)
            {
                index = 0;
            }
            SetCurrentAnimation(anim, index);
        }
    }

    public void SetCurrentAnimation(Animator animator, int index)
    {
        animatorOverrideController["attack"] = attackAnimations[index];
        animator.runtimeAnimatorController = animatorOverrideController;
    }
}
