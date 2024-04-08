using System.Collections;
using UnityEngine;
using static PlayerController;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public delegate void PlayerReady(PlayerController player);
    public static event PlayerReady OnPlayerReady = (PlayerController controller) => { };

    [SerializeField] private PlayerSettings PlayerSettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;

    public bool IsInvulnerable { private set; get; } = false;

    private Rigidbody Body;
    private Animator Animator;
    //TODO: Consume Stamina
    private float Stamina => PlayerSettings.PlayerStartStamina;

    //Parry var
    private Coroutine ParryCorout = null;
    private WaitForSeconds ParryWait;
    private WaitForSeconds InvulnerabilityWait;
    //Parry Time After Invulnerability (TAI)
    private float Parry_TAI => PlayerSettings.ParrySustainTime - PlayerSettings.ParryInvulnerabilityTime;

    private void Awake()
    {
        //Get Components
        Body = GetComponent<Rigidbody>();
        Animator = GetComponent<Animator>();
        //Set Up
        PlayerSettings.Movement.Rigidbody = Body;
        PlayerSettings.AnimationController.Animator = Animator;
        PlayerSettings.AnimationController.Renderer = SpriteRenderer;
        PlayerSettings.AnimationController.ParticleTransform = ParticleHolder;
        InvulnerabilityWait = new WaitForSeconds(PlayerSettings.ParryInvulnerabilityTime);
        ParryWait = new WaitForSeconds(Parry_TAI);
    }

    private void Start()
    {
        PlayerSettings.Movement.Enable();
        OnPlayerReady(this);
    }

    private void Parry()
    {
        if(ParryCorout == null)
        {
            PlayerSettings.Movement.Disable();
            ParryCorout = StartCoroutine(ParryCoroutine());
        }
    }

    private IEnumerator ParryCoroutine()
    {
        PlayerSettings.AnimationController.PlaySpecial();
        IsInvulnerable = true;
        yield return InvulnerabilityWait;
        IsInvulnerable = false;
        yield return ParryWait;
        PlayerSettings.Movement.Enable();
        PlayerSettings.Movement.CancelSprint();
        PlayerSettings.AnimationController.StopSpecial();
        ParryCorout = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<ICollectible>(out ICollectible collectible))
        {
            collectible.Collect();
        }
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
    }

    private void OnDisable()
    {
        OnPlayerReady += OnPlayerReady;
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
    }
}