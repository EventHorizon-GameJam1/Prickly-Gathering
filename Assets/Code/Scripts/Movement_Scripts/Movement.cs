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

    public void ApplySpeed(Vector3 direction)
    {
        Rigidbody.velocity = direction.normalized * (MovementSpeed + SprintValue);
    }

    public void Sprint()
    {
        if (IsSprinting)
            return;
        SprintValue += SprintIncreaser;
        IsSprinting = true;
    }

    public void CancelSprint()
    {
        SprintValue -= SprintIncreaser;
        IsSprinting = false;
    }
}
