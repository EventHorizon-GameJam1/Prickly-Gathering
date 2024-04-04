using UnityEngine;

[System.Serializable]
public abstract class Movement
{
    [Header("Movement")]
    [SerializeField] public float MovementSpeed = 5f;
    [SerializeField] public float SprintMultiplier = 2f;
    [HideInInspector] public Rigidbody Rigidbody;

    protected float SprintValue => MovementSpeed * SprintMultiplier;
    private float ActualSprint = 1f;
    private bool IsSprinting = false;

    protected bool Disabled = false;

    public void ApplySpeed(Vector3 direction)
    {
        if(Disabled) return;

        Rigidbody.velocity = direction.normalized * (MovementSpeed + ActualSprint);
    }

    public void Sprint()
    {
        if (Disabled) return;

        if (IsSprinting)
            return;
        ActualSprint = SprintValue;
        ApplySpeed(Rigidbody.velocity.normalized);
        IsSprinting = true;
    }

    public void CancelSprint()
    {
        if (Disabled) return;

        ActualSprint = 1f;
        ApplySpeed(Rigidbody.velocity.normalized);
        IsSprinting = false;
    }

    public void Disable()
    {
        Rigidbody.velocity = Vector3.zero;
        Disabled = true;
    }

    public void Enable()
    {
        Disabled = false;
    }
}
