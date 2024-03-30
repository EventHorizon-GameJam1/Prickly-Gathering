using System.Collections;
using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody Body;

    [SerializeField] private PlayerMovement Movement;

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        Movement.Rigidbody = Body;
    }

    private void OnEnable()
    {
        InputManager.OnDirectionChanged += Movement.ApplySpeed;
        InputManager.OnSprint += Movement.Sprint;
        InputManager.OnSprintCancelled += Movement.CancelSprint;
        InputManager.OnJump += Movement.Jump;
    }

    private void OnDisable()
    {
        InputManager.OnDirectionChanged -= Movement.ApplySpeed;
        InputManager.OnSprint -= Movement.Sprint;
        InputManager.OnSprintCancelled -= Movement.CancelSprint;
        InputManager.OnJump -= Movement.Jump;
    }
}
