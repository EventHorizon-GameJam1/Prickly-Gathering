using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class AnimationController
{ 
    protected Animator AnimationAnimator;
    [HideInInspector] public SpriteRenderer Renderer { set; private get; }

    protected bool Disabled = false;

    public void SetAnimator(Animator animator)
    {
        AnimationAnimator = animator;
    }

    public void UpdateDirection(Vector3 direction)
    {
        if(Disabled) return;

        if (direction.x != 0f)
            FlipAnimation(direction.x);

        if (direction == Vector3.zero)
        {
            AnimationAnimator.SetBool("IsWalking", false);
        }
        else
        {
            AnimationAnimator.SetBool("IsWalking", true);
        }
    }

    public void PlaySprint()
    {
        if (Disabled) return;
        AnimationAnimator.SetBool("IsRunning", true);
    }

    public void StopSprint()
    {
        if (Disabled) return;
        AnimationAnimator.SetBool("IsRunning", false);
    }
    public void PlaySpecial()
    {
        if (Disabled) return;

        AnimationAnimator.SetBool("IsWalking", false);
        AnimationAnimator.SetBool("IsRunning", false);
        AnimationAnimator.SetBool("IsSpecial", true);
    }

    public void StopSpecial()
    {
        if (Disabled) return;

        AnimationAnimator.SetBool("IsWalking", false);
        AnimationAnimator.SetBool("IsRunning", false);
        AnimationAnimator.SetBool("IsSpecial", false);
    }

    private void FlipAnimation(float dir)
    {
        if (Disabled) return;

        if (dir > 0 && Renderer.flipX)
            Renderer.flipX = false;
        else if(dir < 0 && !Renderer.flipX)
            Renderer.flipX = true;
    }

    public void Disable()
    {
        Disabled = true;
    }

    public void Enable()
    {
        Disabled = false;
    }
}
