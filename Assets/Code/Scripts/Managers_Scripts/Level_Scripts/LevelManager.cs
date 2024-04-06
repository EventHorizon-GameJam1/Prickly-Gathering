using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Singleton
    public static LevelManager Instance;

    //Events
    public delegate void LevelReady();
    public static event LevelReady OnLevelReady = () => { };

    public static Transform PlayerTransform { get; private set; } = null;
    public static float LevelLoadProgress { get; private set; } = 0f;

    [Header("Level Manager Settings")]
    //TODO: Add sky cycle
    [SerializeField] private List<EnemyZone> EnemyZones = new List<EnemyZone>();
    [SerializeField] private List<CollectibleSpawner> CollectibleSpawners = new List<CollectibleSpawner>();

    private int SpawnerToSetUp = 0;
    private int SpawnerSetUpped = 0;

    private void Awake()
    {
        //Set up Singleton
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.Log("WARNING: MULTIPLE LEVEL MANAGER INSTANCES FOUND REMOVING GAME OBJECT");
            Destroy(this.gameObject);
        }

        SpawnerToSetUp = EnemyZones.Count + CollectibleSpawners.Count;
        SpawnerSetUpped = 0;
        LevelLoadProgress = 0f;
    }

    public void Start()
    {
        //Set up collectible zones
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].SetUp();

        //Set up Enemy Zones
        for (int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].SetUp();
    }

    private void SpawnAll()
    {
        //Set up collectible zones
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].Spawn();

        //Set up Enemy Zones
        for (int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].Spawn();
    }

    public void ResetLevel()
    {
        //Reset Enemy Spawner
        for (int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].ResetSpawner();
        //Reset Collectible Spawner
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].ResetSpawner();
    }

    private void UpdateLoadProgress()
    {
        //value that go from 0 to 1
        SpawnerSetUpped++;
        LevelLoadProgress = SpawnerSetUpped / (float)SpawnerToSetUp;
        
        if( LevelLoadProgress > 0.99999f )
            OnLevelReady();
    }

    private void SetPlayerTransform(PlayerController controller)
    {
        PlayerTransform = controller.transform;
    }

    private void OnEnable()
    {
        //Level Manager Events
        OnLevelReady += SpawnAll;

        //Spawner events
        //Set up collectible events
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].OnReady += UpdateLoadProgress;

        //Set up Enemy events
        for (int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].OnReady += UpdateLoadProgress;

        //Player events
        PlayerController.OnPlayerReady += SetPlayerTransform;
    }
}
