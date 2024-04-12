using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Shoot_Component : EnemyController
{
    [Header("Shooter Settings")]
    [SerializeField] private EnemySettings_Shooter Settings;

    private List<Projectile> ObjectToShoot;

    private void Awake()
    {
        for(int i = 0; i < Settings.PoolSize; i++)
        {
            Projectile projectile = new Projectile();
            projectile.ProjectileMovement = Settings.ProjectileMovement;
            projectile.LifeTime = Settings.ProjectileLifeTime;
            projectile.OnPlayerHit += ProjectileHasHitPlayer;
            ObjectToShoot.Add(Instantiate(projectile, transform.position, Quaternion.identity));
        }
    }

    private void ProjectileHasHitPlayer()
    {
        OnPlayerDamaged(base.EnemySettings.Damage, base.EnemySettings.PercentageLost);
    }

    private void ShootProjectile()
    {
        for(int i=0;  i < ObjectToShoot.Count; i++)
        {
            if (!ObjectToShoot[i].gameObject.activeInHierarchy)
            {
                Vector3 PlayerDirection = PlayerTransform.position - transform.position;
                ObjectToShoot[i].ProjectileMovement.Move(PlayerDirection);
                ObjectToShoot[i].gameObject.SetActive(true);
            }
        }
    }

    protected override void AttackState()
    {
        EnemyMovement.SetTargetTransform(PlayerTransform);
        float dist = EnemyMovement.GetDistance();

        EnemyMovement.Sprint();
        EnemyAnimation.PlaySprint();

        if (CanDamage)
        {
            //Play Attack Animation
            if (dist < EnemySettings.AttackDistance)
                EnemyAnimation.PlaySpecial();
            else
                EnemyAnimation.StopSpecial();
        }
    }

    protected override void ApplyDamage()
    {
        ShootProjectile();
    }
}
