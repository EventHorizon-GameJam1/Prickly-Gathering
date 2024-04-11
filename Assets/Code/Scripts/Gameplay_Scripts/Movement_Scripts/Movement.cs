using UnityEngine;

[System.Serializable]
public abstract class Movement
{
    [Header("Movement")]
    [SerializeField] public float MovementSpeed = 5f;
    [SerializeField] public float SpeedMultiplier = 2f;
    [HideInInspector] public Rigidbody Rigidbody;

    protected float SprintValue => MovementSpeed * SpeedMultiplier;
    private float ActualSprint = 1f;
    private bool IsSprinting = false;

    protected bool Disabled = false;

    public void Move(Vector3 direction)
    {
        if(Disabled) return;

        Rigidbody.velocity = direction.normalized * ActualSprint;
    }

    public void Sprint()
    {
        if (Disabled) return;

        if (IsSprinting)
            return;

        ActualSprint = SprintValue;
        Move(Rigidbody.velocity.normalized);
        IsSprinting = true;
    }

    public void CancelSprint()
    {
        if (Disabled) return;

        ActualSprint = MovementSpeed;
        Move(Rigidbody.velocity.normalized);
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
