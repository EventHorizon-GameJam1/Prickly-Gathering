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
    [SerializeField] private List<CollectibleSpawner> CollectibleSpawners = new List<CollectibleSpawner>();
    
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("WARNING: MULTIPLE LEVEL MANAGER INSTANCES FOUND REMOVING GAME OBJECT");
            Destroy(this.gameObject);
        }
    }

    public void ResetLevel()
    {
        //Reset Enemy Spawner
        for(int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].ResetSpawner();
        //Reset Collectible Spawner
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].ResetSpawner();
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
