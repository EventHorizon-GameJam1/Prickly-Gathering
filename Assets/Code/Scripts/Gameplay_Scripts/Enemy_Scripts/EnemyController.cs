using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    [Header("Enemy Settings")]
    [SerializeField] private EnemySettings EnemySettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;
    [Space]
    [Header("Debug Settings")]
    [SerializeField] private bool DrawGizmos = true;
    //Patrolling
    public PatrollingData EnemyPatrollingPath = new PatrollingData();
    [HideInInspector] public Transform PlayerTransform => LevelManager.PlayerTransform;
    private Transform TargetTransform => transform;
    //Components
    private EnemyMovement EnemyMovement = new EnemyMovement();
    private AnimationController EnemyAnimation = new AnimationController();
    private Rigidbody Body;
    private Animator Animator;
    private NavMeshAgent NavMeshAgent;
    
    private delegate void EnemyAction();
    private EnemyAction EnemyActions;

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
    }

    private void Start()
    {
        ChangeState(State.PATROLLING);
    }

    private void FixedUpdate()
    {
        EnemyActions();
        //TODO: Move
        //TODO: Animate
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
                EnemyActions += PatrolState;
                SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.Patrolling_SFX);
                break;
            }
            case State.FLEE:
            {
                EnemyActions -= EnemyActions;
                EnemyActions += FleeState;
                SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.Fleeing_SFX);
                break;
            }
            case State.ATTACK:
            {
                EnemyActions -= EnemyActions;
                EnemyActions += AttackState;
                SFX_Manager.Request2DSFX?.Invoke(transform.position, EnemySettings.Attack_SFX);
                break;
            }

            default:break;
        }
    }

    private void IdleState()
    {

    }

    private void AttackState()
    {
        EnemyMovement.SetTargetTransform(PlayerTransform);
        EnemyAnimation.UpdateDirection(EnemyMovement.GetDirection());
    }

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
        //Move to next patrolling point
        if(dist <= EnemyMovement.StoppingDistance)
            UpdatePatrollingTarget();

        //Animations
        //Update animation direction
        EnemyAnimation.UpdateDirection(EnemyMovement.GetDirection());
    }

    private void UpdatePatrollingTarget()
    {
        EnemyMovement.SetTargetTransform(EnemyPatrollingPath.GetNextPoint());
    }
    private void FleeState()
    { 

    }

    private void OnDisable()
    {
        EnemyMovement.Disable();
        OnEnemyReady -= OnEnemyReady;
        if (EnemyPatrollingPath != null)
            EnemyPatrollingPath = null;
    }

    private void OnEnable()
    {
        EnemyMovement.SetTargetTransform(EnemyPatrollingPath.GetClosestPatrollingPos(transform.position));
        EnemyActions += PatrolState;
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
