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

    [SerializeField] private EnemySettings EnemySettings;
    [SerializeField] private SpriteRenderer SpriteRenderer;
    [SerializeField] private bool DrawGizmos = true;

    private Rigidbody Body;
    private Animator Animator;
    private NavMeshAgent NavMeshAgent;

    private EnemyMovement EnemyMovement = new EnemyMovement();
    private AnimationController EnemyAnimation = new AnimationController();
    //TODO: Fix this with patrolling transform
    private Transform TargetTransform => transform;
    private Transform PlayerTransform = null;

    private delegate void EnemyState();
    private EnemyState EnemyActions;

    public PatrollingData EnemyPatrollingPath;

    private void Awake()
    {
        Animator = GetComponent<Animator>();
        Body = GetComponent<Rigidbody>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        //Movement Set Up
        EnemyMovement.Rigidbody = Body;
        NavMeshAgent.stoppingDistance = EnemySettings.Movement.StoppingDistance;
        EnemyMovement.Agent = NavMeshAgent;
        EnemyMovement.SprintMultiplier = EnemySettings.Movement.SprintMultiplier;
        EnemyMovement.MovementSpeed = EnemySettings.Movement.MovementSpeed;
        //Animation Set Up
        EnemyAnimation.Renderer = SpriteRenderer;
        EnemyAnimation.Animator = Animator;
    }

    private void Start()
    {
        EnemyActions += PatrolState;
        EnemyMovement.SetTargetTransform(EnemyPatrollingPath.GetClosestPatrollingPos(transform.position));
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
        if (EnemyMovement.GetDistance() < EnemySettings.TriggerDistance)
        {
            EnemyActions -= PatrolState;
            EnemyActions += AttackState;
        }
        if (EnemyMovement.GetDistance() > EnemySettings.SprintDistance)
        {
            EnemyMovement.Sprint();
            EnemyAnimation.PlaySprint();
        }
        else
        {
            EnemyMovement.CancelSprint();
            EnemyAnimation.StopSprint();
        }

        EnemyAnimation.UpdateDirection(EnemyMovement.GetDirection());
    }

    private void UpdatePatrollingTarget()
    {
        EnemyMovement.SetTargetTransform(EnemyPatrollingPath.GetNextPoint());
    }

    private void OnEnable()
    {
        OnEnemyReady(this);
        EnemyMovement.Enable();
        PlayerController.OnPlayerReady += AcquireTarget;
        EnemyMovement.OnPatrollingPositionReached += UpdatePatrollingTarget;
    }

    private void OnDisable()
    {
        EnemyMovement.Disable();
        OnEnemyReady -= OnEnemyReady;
        if (EnemyPatrollingPath != null)
            EnemyPatrollingPath = null;
    }

    #region Setting up enemy
    private void AcquireTarget(PlayerController playerController)
    {
        PlayerTransform = playerController.transform;
    }
    #endregion

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
