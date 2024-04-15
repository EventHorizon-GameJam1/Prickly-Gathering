using System;
using System.Collections;
using System.Security.Cryptography;
using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
public class PlayerController : MonoBehaviour
{
    public delegate void PlayerReady(PlayerController player);
    public static event PlayerReady OnPlayerReady = (PlayerController controller) => { };

    public delegate void PlayerDefeated();
    public static event PlayerDefeated OnPlayerDefeated = () => { };

    public delegate void PlayerDamaged();
    public static event PlayerDamaged OnHPChanged = () => { };

    public delegate void PlayerStaminaChanged();
    public static event PlayerStaminaChanged OnStaminaChanged = () => { };

    [SerializeField] private PlayerSettings PlayerSettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;
    [SerializeField] private ParticleSystem HitParticles;

    public bool IsInvulnerable { private set; get; } = false;

    private Rigidbody Body;
    private Animator Animator;
    
    public float Stamina { private set; get; }
    public float PlayerHP { private set; get; }

    //Parry var
    private Coroutine ParryCorout = null;
    private Coroutine SprintCoroutine = null;
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
    }

    private void Parry()
    {
        if(ParryCorout == null)
        {
            PlayerSettings.Movement.Disable();
            ParryCorout = StartCoroutine(ParryCoroutine());
            SFX_Manager.Request2DSFX?.Invoke(transform.position, PlayerSettings.Parry_SFX);
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

    private void TakeDamage(float damage, float percentageLost)
    {
        PlayerHP -= damage;
        OnHPChanged();

        if (GameManager.Score > 0f)
            HitParticles.Play();

        if (PlayerHP <= 0)
            OnPlayerDefeated();
    }

    private void RecoverHP(float percetage)
    {
        PlayerHP += (PlayerHP*percetage);
        Mathf.Clamp(PlayerHP, 0, PlayerSettings.PlayerHP);
        OnHPChanged();
    }

    private void RecoverStamina()
    {
        Stamina += PlayerSettings.PlayerStartStamina*PlayerSettings.EnergyRecoveredOnParry;
        Stamina = Mathf.Clamp(Stamina, 0f, PlayerSettings.PlayerStartStamina);
        OnStaminaChanged();
    }

    private void Sprint()
    {
        if (Stamina <= 0f) return;
        if(Body.velocity == Vector3.zero) return;

        SprintCoroutine = StartCoroutine(SprintEnum());
        PlayerSettings.Movement.Sprint();
        PlayerSettings.AnimationController.PlaySprint();
    }

    private IEnumerator SprintEnum()
    {
        while (Stamina>0f)
        {
            if (Body.velocity != Vector3.zero)
            {
                Stamina -= PlayerSettings.EnergyUsage*Time.deltaTime;
                Stamina = Mathf.Clamp(Stamina, 0f, PlayerSettings.PlayerStartStamina);
                OnStaminaChanged();
            }
            yield return null;
        }
        CancelSprint();
        SprintCoroutine = null;
    }

    private void CancelSprint()
    {
        if(SprintCoroutine != null) 
            StopCoroutine(SprintCoroutine);
        PlayerSettings.Movement.CancelSprint();
        PlayerSettings.AnimationController.StopSprint();
    }

    private void ResetStamina()
    {
        Stamina = PlayerSettings.PlayerStartStamina;
        OnStaminaChanged();
    }

    private void ResetHP()
    {
        PlayerHP = PlayerSettings.PlayerHP;
        OnHPChanged();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<ICollectible>(out ICollectible collectible))
        {
            collectible.Collect();
        }
    }

    private void SendPlayerRef()
    {
        OnPlayerReady(this);
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
        InputManager.OnJump += PlayerSettings.Movement.Jump;
        //Animation Connections
        InputManager.OnDirectionChanged += PlayerSettings.AnimationController.UpdateDirection;
        InputManager.OnSprint += Sprint;
        InputManager.OnSprintCancelled += CancelSprint;
        InputManager.OnParry += PlayerSettings.AnimationController.PlaySpecial;
        //Enemy Connections
        EnemyController.OnPlayerDamaged += TakeDamage;
        EnemyController.OnEnemyParried += RecoverStamina;
        //Level Manager Connection
        LevelManager.OnLevelReady += EnablePlayerController;
        //Game Manager Connection
        GameManager.OnNewDay += ResetStamina;
        GameManager.OnNewDay += ResetHP;
        //Collectible
        Collectible.OnHpCollect += RecoverHP;
        //Level
        LevelManager.OnLevelReady += SendPlayerRef;
    }

    private void OnDisable()
    {
        OnPlayerReady -= OnPlayerReady;
        OnHPChanged -= OnHPChanged;
        OnPlayerDefeated -= OnPlayerDefeated;
        OnStaminaChanged -= OnStaminaChanged;

        //Controller Disconnections
        InputManager.OnParry -= Parry;
        //Movement Disconnections
        InputManager.OnDirectionChanged -= PlayerSettings.Movement.Move;
        InputManager.OnJump -= PlayerSettings.Movement.Jump;
        //Animation Disconnections
        InputManager.OnDirectionChanged -= PlayerSettings.AnimationController.UpdateDirection;
        InputManager.OnSprint -= Sprint;    
        InputManager.OnSprintCancelled -= CancelSprint;
        InputManager.OnParry -= PlayerSettings.AnimationController.PlaySpecial;
        //Enemy Disconnections
        EnemyController.OnPlayerDamaged -= TakeDamage;
        EnemyController.OnEnemyParried -= RecoverStamina;
        //Level Manager Disconnections
        LevelManager.OnLevelReady -= EnablePlayerController;
        //Game Manager Disconnections
        GameManager.OnNewDay -= ResetStamina;
        GameManager.OnNewDay -= ResetHP;
        //Collectible
        Collectible.OnHpCollect -= RecoverHP;
        //Level
        LevelManager.OnLevelReady -= SendPlayerRef;
    }
}