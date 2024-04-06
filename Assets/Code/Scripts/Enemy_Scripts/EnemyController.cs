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
    public delegate void EnemyReady(EnemyController enemy);
    public EnemyReady OnEnemyReady = (EnemyController controller) => {};

    [Header("Enemy Settings")]
    [SerializeField] private EnemySettings EnemySettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private Transform ParticleHolder;
    [Space]
    [Header("Debug Settings")]
    [SerializeField] private bool DrawGizmos = true;

    private Rigidbody Body;
    private Animator Animator;
    private NavMeshAgent NavMeshAgent;

    private EnemyMovement EnemyMovement = new EnemyMovement();
    private AnimationController EnemyAnimation = new AnimationController();
    //TODO: Fix this with patrolling transform
    private Transform TargetTransform => transform;
    [HideInInspector] public Transform PlayerTransform => LevelManager.PlayerTransform;

    private delegate void EnemyState();
    private EnemyState EnemyActions;

    public PatrollingData EnemyPatrollingPath = new PatrollingData();

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
        //OnEnemyReady(this);
    }

    private void FixedUpdate()
    {
        EnemyActions();
        //TODO: Move
        //TODO: Animate
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
