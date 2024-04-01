using UnityEditor.Animations;
using UnityEngine;

[System.Serializable]
public class AnimationController
{
    [HideInInspector] public Animator Animator { set; private get; }
    [HideInInspector] public SpriteRenderer Renderer { set; private get; }

    public void UpdateDirection(Vector3 direction)
    {
        if (direction.x != 0f)
            FlipAnimation(direction.x);

        if (direction == Vector3.zero)
        {
            Animator.SetBool("IsWalking", false);
        }
        else
        {
            Animator.SetBool("IsWalking", true);
        }
    }

    public void PlaySprint() => Animator.SetBool("IsRunning", true);
    public void StopSprint() => Animator.SetBool("IsRunning", false);
    public void PlaySpecial()
    {
        Animator.SetBool("IsWalking", false);
        Animator.SetBool("IsRunning", false);
        Animator.SetBool("IsSpecial", true);
    }

    public void StopSpecial()
    {
        Animator.SetBool("IsWalking", false);
        Animator.SetBool("IsRunning", false);
        Animator.SetBool("IsSpecial", false);
    }

    private void FlipAnimation(float dir)
    {
        if (dir > 0 && Renderer.flipX)
            Renderer.flipX = false;
        else if(dir < 0 && !Renderer.flipX)
            Renderer.flipX = true;
    }
}
