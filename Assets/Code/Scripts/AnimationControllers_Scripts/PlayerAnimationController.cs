using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerAnimationController : AnimationController
{
    public void JumpAnimation()
    {
        if (Disabled) return;

        AnimationAnimator.SetBool("IsWalking", false);
        AnimationAnimator.SetBool("IsRunning", false);
        AnimationAnimator.SetBool("IsSpecial", false);
    }
}
