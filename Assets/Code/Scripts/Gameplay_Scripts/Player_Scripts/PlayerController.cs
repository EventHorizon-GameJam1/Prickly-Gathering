using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public delegate void PlayerReady(PlayerController player);
    public static event PlayerReady OnPlayerReady = (PlayerController controller) => { };

    public delegate void PlayerDefeated();
    public static event PlayerDefeated OnPlayerDefeated = () => { };

    public delegate void PlayerDamaged();
    public static event PlayerDamaged OnPlayerDamaged = () => { };

    [SerializeField] private PlayerSettings PlayerSettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;

    public bool IsInvulnerable { private set; get; } = false;

    private Rigidbody Body;
    private Animator Animator;
    //TODO: Consume Stamina
    public float Stamina { private set; get; }
    public float PlayerHP { private set; get; }

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
        PlayerHP = PlayerSettings.PlayerHP;
        Stamina = PlayerSettings.PlayerStartStamina;
    }

    private void EnablePlayerController()
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

    private void TakeDamage(float damage, float percentageLost)
    {
        PlayerHP -= damage;
        OnPlayerDamaged();
        if (PlayerHP <= 0)
            OnPlayerDefeated();
    }

    private void OnCollisionEnter(Collision collision)
    {
        Body.velocity = collision.contacts[0].normal;
    }

    private void OnEnable()
    {
        //Controller Connections
        InputManager.OnParry += Parry;
        //Movement Connections
        InputManager.OnDirectionChanged += PlayerSettings.Movement.Move;
        InputManager.OnSprint += PlayerSettings.Movement.Sprint;
        InputManager.OnSprintCancelled += PlayerSettings.Movement.CancelSprint;
        InputManager.OnJump += PlayerSettings.Movement.Jump;
        //Animation Connections
        InputManager.OnDirectionChanged += PlayerSettings.AnimationController.UpdateDirection;
        InputManager.OnSprint += PlayerSettings.AnimationController.PlaySprint;
        InputManager.OnSprintCancelled += PlayerSettings.AnimationController.StopSprint;
        InputManager.OnParry += PlayerSettings.AnimationController.PlaySpecial;
        //Enemy Connections
        EnemyController.OnPlayerDamaged += TakeDamage;
        //Level Manager Connection
        LevelManager.OnLevelReady += EnablePlayerController;
    }

    private void OnDisable()
    {
        OnPlayerReady -= OnPlayerReady;
        OnPlayerDamaged -= OnPlayerDamaged;
        //Controller Connections
        InputManager.OnParry -= Parry;
        //Movement Disconnections
        InputManager.OnDirectionChanged -= PlayerSettings.Movement.Move;
        InputManager.OnSprint -= PlayerSettings.Movement.Sprint;
        InputManager.OnSprintCancelled -= PlayerSettings.Movement.CancelSprint;
        InputManager.OnJump -= PlayerSettings.Movement.Jump;
        //Animation Disconnections
        InputManager.OnDirectionChanged -= PlayerSettings.AnimationController.UpdateDirection;
        InputManager.OnSprint -= PlayerSettings.AnimationController.PlaySprint;
        InputManager.OnSprintCancelled -= PlayerSettings.AnimationController.StopSprint;
        //Enemy Connections
        EnemyController.OnPlayerDamaged -= TakeDamage;
        //Level Manager Connection
        LevelManager.OnLevelReady -= EnablePlayerController;
    }
}