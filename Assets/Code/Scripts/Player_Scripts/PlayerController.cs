using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{

    [SerializeField] private PlayerSettings PlayerSettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Animator Animator;
    public bool IsInvulnerable { private set; get; } = false;

    private Rigidbody Body;

    //Parry var
    //Parry Time After Invulnerability (TAI)
    private float Parry_TAI => PlayerSettings.ParrySustainTime - PlayerSettings.ParryInvulnerabilityTime;
    private WaitForSeconds ParryWait;
    private WaitForSeconds InvulnerabilityWait;

    private void Awake()
    {
        //Get Components
        Body = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        //Set Up
        PlayerSettings.Movement.Rigidbody = Body;
        PlayerSettings.AnimationController.SetAnimator(Animator);
        PlayerSettings.AnimationController.Renderer = SpriteRenderer;
        ParryWait = new WaitForSeconds(PlayerSettings.ParryInvulnerabilityTime);
        InvulnerabilityWait = new WaitForSeconds(Parry_TAI);
    }

    private void Start()
    {
        PlayerSettings.Movement.Enable();
    }

    private void Parry()
    {
        PlayerSettings.Movement.Disable();
        StartCoroutine(ParryCoroutine());
    }

    private IEnumerator ParryCoroutine()
    {
        IsInvulnerable = true;
        yield return ParryWait;
        IsInvulnerable = false;
        yield return InvulnerabilityWait;
        PlayerSettings.Movement.Enable();
        PlayerSettings.AnimationController.StopSpecial();
    }

    private void OnEnable()
    {
        //Controller Connections
        InputManager.OnParry += Parry;
        //Movement Connections
        InputManager.OnDirectionChanged += PlayerSettings.Movement.ApplySpeed;
        InputManager.OnSprint += PlayerSettings.Movement.Sprint;
        InputManager.OnSprintCancelled += PlayerSettings.Movement.CancelSprint;
        InputManager.OnJump += PlayerSettings.Movement.Jump;
        //Animation Connections
        InputManager.OnDirectionChanged += PlayerSettings.AnimationController.UpdateDirection;
        InputManager.OnSprint += PlayerSettings.AnimationController.PlaySprint;
        InputManager.OnSprintCancelled += PlayerSettings.AnimationController.StopSprint;
        InputManager.OnParry += PlayerSettings.AnimationController.PlaySpecial;
        //Jump
        InputManager.OnJump += PlayerSettings.AnimationController.JumpAnimation;
        PlayerSettings.Movement.OnJumpFinished += PlayerSettings.AnimationController.Enable;
    }

    private void OnDisable()
    {
        //Controller Connections
        InputManager.OnParry -= Parry;
        //Movement Disconnections
        InputManager.OnDirectionChanged -= PlayerSettings.Movement.ApplySpeed;
        InputManager.OnSprint -= PlayerSettings.Movement.Sprint;
        InputManager.OnSprintCancelled -= PlayerSettings.Movement.CancelSprint;
        InputManager.OnJump -= PlayerSettings.Movement.Jump;
        //Animation Disconnections
        InputManager.OnDirectionChanged -= PlayerSettings.AnimationController.UpdateDirection;
        InputManager.OnSprint -= PlayerSettings.AnimationController.PlaySprint;
        InputManager.OnSprintCancelled -= PlayerSettings.AnimationController.StopSprint;
        InputManager.OnParry -= PlayerSettings.AnimationController.PlaySpecial;
        //Jump
        InputManager.OnJump -= PlayerSettings.AnimationController.JumpAnimation;
        PlayerSettings.Movement.OnJumpFinished -= PlayerSettings.AnimationController.Enable;
    }
}