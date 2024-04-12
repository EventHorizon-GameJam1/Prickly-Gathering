using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public delegate void PlayerHit();
    public event PlayerHit OnPlayerHit = () => { };

    [HideInInspector] public float LifeTime = 1.5f;
    [HideInInspector] public Movement ProjectileMovement;
    [SerializeField] Rigidbody Rigidbody;

    private void Awake()
    {
        ProjectileMovement.Rigidbody = Rigidbody;
    }

    private IEnumerator Exist()
    {
        yield return new WaitForSeconds(LifeTime);
        this.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        ProjectileMovement.Enable();
        StartCoroutine(Exist());
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.layer == 8)
        {
            OnPlayerHit();
        }
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        OnPlayerHit -= OnPlayerHit;
    }
}
