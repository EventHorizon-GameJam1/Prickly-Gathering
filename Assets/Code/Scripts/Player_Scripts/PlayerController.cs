using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    private Rigidbody Body;

    [SerializeField] private PlayerSettings PlayerSettings;

    private void Awake()
    {
        Body = GetComponent<Rigidbody>();
        PlayerSettings.Movement.Rigidbody = Body;
    }

    private void OnEnable()
    {
        InputManager.OnDirectionChanged += PlayerSettings.Movement.ApplySpeed;
        InputManager.OnSprint += PlayerSettings.Movement.Sprint;
        InputManager.OnSprintCancelled += PlayerSettings.Movement.CancelSprint;
        InputManager.OnJump += PlayerSettings.Movement.Jump;
    }

    private void OnDisable()
    {
        InputManager.OnDirectionChanged -= PlayerSettings.Movement.ApplySpeed;
        InputManager.OnSprint -= PlayerSettings.Movement.Sprint;
        InputManager.OnSprintCancelled -= PlayerSettings.Movement.CancelSprint;
        InputManager.OnJump -= PlayerSettings.Movement.Jump;
    }
}