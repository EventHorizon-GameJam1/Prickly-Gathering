using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[System.Serializable]
public class PlayerMovement : Movement
{
    public delegate void JumpAction();
    public event JumpAction OnJumpStarted = () => { };
    public event JumpAction OnJumpFinished = () => { };

    [Header("Jump")]
    [SerializeField] private float JumpHeight = 1.5f;
    [SerializeField] private float JumpDistance = 3f;
    [SerializeField] private float JumpAnimationSpeed = 0.5f;
    [SerializeField] private AnimationCurve JumpAnimationCurve;
    
    public void Jump()
    {
        if(Disabled) return;

        EnumeratorManager.AddEnumerator(JumpAnimation());
    }

    private IEnumerator JumpAnimation()
    {
        //Call start event
        OnJumpStarted();

        //Animation Values
        float progress = 0f;
        float curveValue = 0f;
        Vector3 startPosition = Rigidbody.position;
        //Final position -> Postion + (direction * distance)
        Vector3 finalPosition = Rigidbody.position + (Rigidbody.velocity.normalized * JumpDistance);

        //Disable movement change
        Disable();

        while (progress < 1f)
        {
            //Height
            curveValue = JumpAnimationCurve.Evaluate(progress);
            Vector3 height = curveValue * JumpHeight * Vector3.up;
            Rigidbody.transform.position += height;
            //Position
            Rigidbody.transform.position = Vector3.Lerp(startPosition + height, finalPosition + height, progress);
            progress += Time.deltaTime * JumpAnimationSpeed;
            yield return null;
        }
        
        //Enable movement change
        Enable();

        //Call finish event
        OnJumpFinished();
    }

    new public void Disable()
    {
        base.Disable();
    }

    new public void Enable()
    {
        base.Enable();
    }
}
