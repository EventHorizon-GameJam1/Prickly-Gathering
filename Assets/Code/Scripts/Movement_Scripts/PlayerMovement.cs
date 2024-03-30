using System.Collections;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : Movement
{
    [Header("Jump")]
    [SerializeField] private float JumpHeight = 1.5f;
    [SerializeField] private float JumpAnimationSpeed = 0.5f;
    [SerializeField] private AnimationCurve JumpAnimationCurve;

    private bool IsJumping = false;
    
    public void Jump()
    {
        if(!IsJumping)
            EnumeratorManager.AddEnumerator(JumpAnimation());
    }

    private IEnumerator JumpAnimation()
    {
        IsJumping = true;
        float progress = 0f;
        float curveValue = 0f;
        while (progress < 1f)
        {
            curveValue = JumpAnimationCurve.Evaluate(progress);
            Rigidbody.transform.position = new Vector3(Rigidbody.transform.position.x, curveValue * JumpHeight, Rigidbody.transform.position.z);
            progress += Time.deltaTime * JumpAnimationSpeed;
            yield return null;
        }
        IsJumping = false;
    }
}
