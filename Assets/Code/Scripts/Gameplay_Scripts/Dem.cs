using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dem : MonoBehaviour
{
    public delegate void SecurePlayer(float percentage);
    public static event SecurePlayer OnPlayerSecured = (float percentage) => { };

    public delegate void SecureAvailable(float percentage);
    public static event SecureAvailable OnSecureAvailable = (float percentage) => { };
    public static event SecureAvailable OnSecureNotAvailable = (float percentage) => { };

    [Header("Dem Settings")]
    [SerializeField][Range(0f, 1f)] private float StoredPercentage = 1f;
    [SerializeField] Transform PlayerSpawnPos = null;

    private bool Enabled = false;

    private void Interaction()
    {
        if (Enabled)
        {
            LevelManager.PlayerSpawnPosition = PlayerSpawnPos;
            OnPlayerSecured(StoredPercentage);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Enabled = true;
        OnSecureAvailable(StoredPercentage);
    }

    private void OnTriggerExit(Collider other)
    {
        Enabled = false;
        OnSecureNotAvailable(StoredPercentage);
    }

    private void OnEnable()
    {
        InputManager.OnSelect += Interaction;
    }

    private void OnDisable()
    {
        InputManager.OnSelect -= Interaction;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawSphere(PlayerSpawnPos.position, 0.5f);
    }
#endif
}
