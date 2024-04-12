using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Settings", menuName = "Settings/Entity/Enemy shoot that can may or may not shoot")]
public class EnemySettings_Shooter : ScriptableObject
{
    [Header("Shooter Settings")]
    [SerializeField] public Projectile ObjectToShoot;
    [SerializeField] public int PoolSize = 5;
    [SerializeField] public float ProjectileLifeTime = 1.5f;
    [SerializeField] public Movement ProjectileMovement;
    [SerializeField] public Vector3 ProjectileOffset = Vector3.up;
}
