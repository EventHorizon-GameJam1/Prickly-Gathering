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

    public delegate void EnemyParried();
    public EnemyParried OnEnemyParried = () => { };

    [Header("Enemy Settings")]
    [SerializeField] private EnemySettings EnemySettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;
    [Space]
    [Header("Debug Settings")]
    [SerializeField] private bool DrawGizmos = true;
    //Patrolling
    [HideInInspector] public PatrollingData EnemyPatrollingPath = new PatrollingData();
    [HideInInspector] public Transform PlayerTransform => LevelManager.PlayerTransform;

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
                GetClosestPatrollingPoint();
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
        EnemyMovement.SetTargetTransform(transform);

        //Check if player is in trigger distance
        if (Vector3.Distance(transform.position, PlayerTransform.position) < EnemySettings.TriggerDistance)
            ChangeState(State.ATTACK);
    }
    #endregion

    #region ATTACK
    private void AttackState()
    {
        EnemyMovement.SetTargetTransform(PlayerTransform);
        float dist = EnemyMovement.GetDistance();

        EnemyMovement.Sprint();
        EnemyAnimation.PlaySprint();

        //Play Attack Animation
        if (dist < EnemySettings.AttackDistance)
            EnemyAnimation.PlaySpecial();
        else
            EnemyAnimation.StopSpecial();
    }

    //called by animation
    private void ApplyDamage()
    {
        if(LevelManager.Player.IsInvulnerable)
        {
            //TODO: Parry
            Debug.Log("I've been parried");
            OnEnemyParried();
        }
        else
        {
            //TODO: Damage Player
            Debug.Log("Player have been damaged");
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
        //Move to next patrolling point
        if(dist <= EnemyMovement.StoppingDistance)
            UpdatePatrollingTarget();
        
        //Check if player is in trigger distance
        if(Vector3.Distance(transform.position, PlayerTransform.position) < EnemySettings.TriggerDistance)
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
