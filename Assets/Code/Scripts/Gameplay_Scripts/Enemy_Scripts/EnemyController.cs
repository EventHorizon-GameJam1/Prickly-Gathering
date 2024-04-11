using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMesh))]
public class EnemyController : MonoBehaviour
{
    private enum State
    {
        IDLE,
        PATROLLING,
        ATTACK,
        FLEE
    }

    public delegate void EnemyReady(EnemyController enemy);
    public EnemyReady OnEnemyReady = (EnemyController controller) => {};

    public delegate void EnemyParried();
    public static EnemyParried OnEnemyParried = () => { };

    public delegate void PlayerDamaged(float damage, float suppliesLost);
    public static PlayerDamaged OnPlayerDamaged = (float damage, float suppliesLost) => { };

    [Header("Enemy Settings")]
    [SerializeField] private EnemySettings EnemySettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;
    [SerializeField] private State StartingState = State.PATROLLING;
    [Space]
    [Header("Health Indicator settings")]
    [SerializeField] private Sprite FullHealth;
    [SerializeField] private Sprite EmptyHealth;
    [SerializeField] private List<SpriteRenderer> HealthIndicators = new List<SpriteRenderer>(); 
    [Space]
    [Header("Debug Settings")]
    [SerializeField] private bool DrawGizmos = true;
    //Patrolling
    [HideInInspector] public PatrollingData EnemyPatrollingPath;
    [HideInInspector] private Transform PlayerTransform => LevelManager.PlayerTransform;

    //Components
    private EnemyMovement EnemyMovement = new EnemyMovement();
    private AnimationController EnemyAnimation = new AnimationController();
    private Rigidbody Body;
    private Animator Animator;
    private NavMeshAgent NavMeshAgent;
    
    private int EnemyDetermination = 0;

    private delegate void EnemyAction();
    private EnemyAction EnemyActions;

    private Coroutine IdleCoroutine;

    private bool IsFleeing = false;
    private bool CanDamage = true;
    private bool ClosestAlreadyGot = false;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Body = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        //Movement Set Up
        EnemyMovement.Rigidbody = Body;
        EnemyMovement.StoppingDistance = EnemySettings.Movement.StoppingDistance;
        NavMeshAgent.stoppingDistance = EnemyMovement.StoppingDistance;
        EnemyMovement.Agent = NavMeshAgent;
        EnemyMovement.SprintMultiplier = EnemySettings.Movement.SprintMultiplier;
        EnemyMovement.MovementSpeed = EnemySettings.Movement.MovementSpeed;
        //Animation Set Up
        EnemyAnimation.Renderer = SpriteRenderer;
        EnemyAnimation.Animator = Animator;
        EnemyAnimation.ParticleTransform = ParticleHolder;
        //Enemy set up
        EnemyDetermination = EnemySettings.EnemyDetermination;
    }

    private void Start()
    {
        ChangeState(StartingState);

        for (int i = 0; i < HealthIndicators.Count; i++)
            HealthIndicators[i].sprite = FullHealth;
    }

    private void FixedUpdate()
    {
        EnemyActions();
        //Update animation direction
        EnemyAnimation.UpdateDirection(EnemyMovement.GetDirection());
    }

    private void ChangeState(State newState)
    {
        switch(newState)
        {
            case State.IDLE:
            {
                EnemyActions -= EnemyActions;
                EnemyActions += IdleState;
                SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.Idling_SFX);
                break;
            }
            case State.PATROLLING:
            {
                EnemyActions -= EnemyActions;
                NavMeshAgent.stoppingDistance = EnemyMovement.StoppingDistance;
                NavMeshAgent.speed = EnemyMovement.MovementSpeed;

                if (!ClosestAlreadyGot)
                {
                    ClosestAlreadyGot = true;
                    GetClosestPatrollingPoint();
                }

                EnemyActions += PatrolState;
                if(EnemySettings.Patrolling_SFX!=null)
                    SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.Patrolling_SFX);
                break;
            }
            case State.FLEE:
            {
                EnemyActions -= EnemyActions;
                IsFleeing = true;
                EnemyAnimation.StopSpecial();
                EnemyMovement.SetTargetTransform(EnemyPatrollingPath.EscapePoint);
                EnemyActions += FleeState;
                SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.Fleeing_SFX);
                break;
            }
            case State.ATTACK:
            {
                CanDamage = true;

                if (IdleCoroutine != null)
                    StopCoroutine(IdleCoroutine);

                NavMeshAgent.stoppingDistance = EnemySettings.AttackDistance;

                for (int i = 0; i < HealthIndicators.Count; i++)
                    HealthIndicators[i].gameObject.SetActive(true);

                EnemyActions -= EnemyActions;
                EnemyActions += AttackState;
                EnemyMovement.SetTargetTransform(PlayerTransform);
                SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.EnemySpotted_SFX);
                break;
            }
            default:break;
        }
    }

    #region IDLE
    private void IdleState()
    {
        EnemyMovement.Disable();

        if(IdleCoroutine ==  null)
            IdleCoroutine = StartCoroutine(IdleEumerator());

        //Check if player is in trigger distance
        if (Vector3.Distance(transform.position, PlayerTransform.position) < EnemySettings.TriggerDistance)
            ChangeState(State.ATTACK);
    }

    private IEnumerator IdleEumerator()
    {
        float randomWait = UnityEngine.Random.Range(EnemySettings.IdleTime_Min, EnemySettings.IdleTime_Max);
        yield return new WaitForSeconds(randomWait);
        EnemyMovement.Enable();
        UpdatePatrollingTarget();
        ChangeState(State.PATROLLING);
        IdleCoroutine = null;
    }
    #endregion

    #region ATTACK
    private void AttackState()
    {
        EnemyMovement.SetTargetTransform(PlayerTransform);
        float dist = EnemyMovement.GetDistance();

        EnemyMovement.Sprint();
        EnemyAnimation.PlaySprint();

        if(CanDamage)
        {
            //Play Attack Animation
            if (dist < EnemySettings.AttackDistance)
                EnemyAnimation.PlaySpecial();
            else
                EnemyAnimation.StopSpecial();
        }
    }

    private void StopAttack()
    {
        CanDamage = false;
    }

    //Called by animation
    private void ApplyDamage()
    {
        if(LevelManager.Player.IsInvulnerable) //Parryed
            TakeDamage();
        else
            OnPlayerDamaged(EnemySettings.Damage, EnemySettings.PercentageLost);
    }

    private void TakeDamage()
    {
        EnemyDetermination--;
        OnEnemyParried();
        HealthIndicators[EnemyDetermination].sprite = EmptyHealth;

        if (EnemyDetermination <= 0)
        {
            for (int i = 0; i < HealthIndicators.Count; i++)
                HealthIndicators[i].gameObject.SetActive(false);
            ChangeState(State.FLEE);
        }
    }
    #endregion

    #region PATROLLING
    private void PatrolState()
    {
        float dist = EnemyMovement.GetDistance();

        //Movement
        //Run or Walk
        if (dist > EnemySettings.SprintDistance)
        {
            EnemyMovement.Sprint();
            EnemyAnimation.PlaySprint();
        }
        else
        {
            EnemyMovement.CancelSprint();
            EnemyAnimation.StopSprint();
        }
        //Move to next patrolling point after idle
        if (dist <= EnemyMovement.StoppingDistance)
        {
            if (UnityEngine.Random.value > EnemySettings.IdleProbability)
            {
                ChangeState(State.IDLE);
                return;
            }
            UpdatePatrollingTarget();
        }

        //Check if player is in trigger distance
        if (Vector3.Distance(transform.position, PlayerTransform.position) < EnemySettings.TriggerDistance)
            ChangeState(State.ATTACK);
    }

    private void UpdatePatrollingTarget()
    {
        EnemyMovement.SetTargetTransform(EnemyPatrollingPath.GetNextPoint());
    }

    private void GetClosestPatrollingPoint()
    {
        EnemyMovement.SetTargetTransform(EnemyPatrollingPath.GetClosestPatrollingPos(transform.position));
    }
    #endregion

    #region FLEEING
    private void FleeState()
    {
        EnemyMovement.Sprint();
        EnemyAnimation.PlaySprint();
    }
    #endregion

    private void OnEnable()
    {
        IsFleeing = false;
        ClosestAlreadyGot = false;

        ChangeState(State.PATROLLING);

        for (int i = 0; i < HealthIndicators.Count; i++)
            HealthIndicators[i].gameObject.SetActive(false);

        LevelManager.OnPlayerSecured += StopAttack;
        PlayerController.OnPlayerDefeated += StopAttack;
    }

    private void OnDisable()
    {
        EnemyMovement.Disable();
        NavMeshAgent.speed = 0;
        OnEnemyReady -= OnEnemyReady;

        LevelManager.OnPlayerSecured -= StopAttack;
        PlayerController.OnPlayerDefeated -= StopAttack;
    }


    //Gizsmos
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (!DrawGizmos) return;

        //Trigger Zone
        Handles.color = EnemySettings.TriggerRadiousColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, EnemySettings.TriggerDistance);
        //Running Radious
        Handles.color = EnemySettings.SprintRadiousColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, EnemySettings.SprintDistance);
        //Attack Radious
        Handles.color = EnemySettings.AttackRadiousColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, EnemySettings.AttackDistance);
    }
#endif
}
