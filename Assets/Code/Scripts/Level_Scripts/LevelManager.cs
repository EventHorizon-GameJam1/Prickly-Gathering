using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Singleton
    public static LevelManager Instance;

    public static Transform PlayerTransform { get; private set; } = null;

    [Header("Level Manager Settings")]
    //TODO: Add sky cycle
    [SerializeField] private List<EnemyZone> EnemyZones = new List<EnemyZone>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }

    private void SetPlayerTransform(PlayerController controller)
    {
        PlayerTransform = controller.transform;
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerReady += SetPlayerTransform;
    }
}
