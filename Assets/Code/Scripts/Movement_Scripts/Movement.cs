using UnityEngine;

[System.Serializable]
public class Movement
{
    [Header("Movement")]
    [SerializeField] private float MovementSpeed = 1.0f;
    [SerializeField] private float SprintIncreaser = 0.5f;
    [HideInInspector] public Rigidbody Rigidbody;

    private float SprintValue = 0f;
    private bool IsSprinting = false;
    protected bool Disabled = false;

    public void ApplySpeed(Vector3 direction)
    {
        if(Disabled) return;

        Rigidbody.velocity = direction.normalized * (MovementSpeed + SprintValue);
    }

    public void Sprint()
    {
        if (Disabled) return;

        if (IsSprinting)
            return;
        SprintValue += SprintIncreaser;
        ApplySpeed(Rigidbody.velocity.normalized);
        IsSprinting = true;
    }

    public void CancelSprint()
    {
        if (Disabled) return;

        SprintValue -= SprintIncreaser;
        ApplySpeed(Rigidbody.velocity.normalized);
        IsSprinting = false;
    }

    public void Disable()
    {
        Disabled = true;
    }

    public void Enable()
    {
        Disabled = false;
    }
}
