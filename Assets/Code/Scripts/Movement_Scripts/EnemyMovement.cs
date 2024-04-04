using UnityEngine;
using UnityEngine.AI;

[System.Serializable]
public class EnemyMovement : Movement
{
    public delegate void AttackDistanceReached();
    public event AttackDistanceReached OnAttackDistanceReached = () => { };

    public delegate void PatrollingPositionReached();
    public event PatrollingPositionReached OnPatrollingPositionReached = () => { };

    [SerializeField] public float StoppingDistance = 1.25f;

    [HideInInspector] public NavMeshAgent Agent;
    [HideInInspector] public Vector3 FleeingPosition = Vector3.zero;
    private Vector3 FleeingPos => FleeingPosition;
    private float DistanceFromTarget;

    private Transform TargetTransform;

    public void SetTargetTransform(Transform target)
    {
        TargetTransform = target;
        Agent.updateRotation = false;
        Agent.destination = TargetTransform.position;
    }

    public float GetDistance()
    {
        UpdateDistance();
        return DistanceFromTarget;
    }

    public Vector3 GetDirection()
    {
        UpdateDistance();
        if(!OnTarget())
            return Agent.velocity.normalized;
        else
            return Vector3.zero;
    }

    private bool OnTarget()
    {
        if (DistanceFromTarget < StoppingDistance)
        {
            OnPatrollingPositionReached();
            Stop();
            return true;
        }
        return false;
    }

    public void Stop()
    {
        Agent.velocity = Vector3.zero;
    }

    private void UpdateDistance()
    {
        DistanceFromTarget = Vector3.Distance(TargetTransform.position, Agent.transform.position);
    }

    new public void Sprint()
    {
        Agent.speed = SprintValue;
    }

    new public void CancelSprint()
    {
        Agent.speed = MovementSpeed;
    }

    new public void Enable()
    {
        base.Enable();
        Agent.speed = base.MovementSpeed;
    }

    new public void Disable()
    {
        base.Disable();
        Agent.speed = 0f;
        OnAttackDistanceReached -= OnAttackDistanceReached;
        OnPatrollingPositionReached -= OnPatrollingPositionReached;
    }
}
