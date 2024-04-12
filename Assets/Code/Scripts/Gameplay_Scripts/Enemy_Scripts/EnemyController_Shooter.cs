using System.Collections.Generic;
using UnityEngine;

public class EnemyController_Shooter: EnemyController
{
    [Header("Shooter Settings")]
    [SerializeField] private EnemySettings_Shooter Settings;

    private List<Projectile> ObjectToShoot = new List<Projectile>();

    private new void Awake()
    {
        base.Awake();
        for (int i = 0; i < Settings.PoolSize; i++)
        {
            Projectile ProjectileToShoow = Settings.ObjectToShoot;
            ProjectileToShoow.ProjectileMovement = Settings.ProjectileMovement;
            ProjectileToShoow.LifeTime = Settings.ProjectileLifeTime;
            ObjectToShoot.Add(Instantiate(Settings.ObjectToShoot, transform.position, Quaternion.identity));
            ObjectToShoot[i].OnPlayerHit += ProjectileHasHitPlayer;
            ObjectToShoot[i].gameObject.SetActive(false);
        }
    }

    private void ProjectileHasHitPlayer()
    {
        OnPlayerDamaged(base.EnemySettings.Damage, base.EnemySettings.PercentageLost);
    }

    private void ShootProjectile()
    {
        for (int i = 0; i < ObjectToShoot.Count; i++)
        {
            if (!ObjectToShoot[i].gameObject.activeInHierarchy)
            {
                Vector3 PlayerDirection = PlayerTransform.position - transform.position;
                ObjectToShoot[i].transform.position = transform.position + Settings.ProjectileOffset;
                ObjectToShoot[i].gameObject.SetActive(true);
                ObjectToShoot[i].ProjectileMovement.Move(PlayerDirection);
                break;
            }
        }
    }

    protected override void ApplyDamage()
    {
        ShootProjectile();
    }
}
