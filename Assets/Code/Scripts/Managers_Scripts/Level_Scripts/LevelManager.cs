using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Singleton
    public static LevelManager Instance;

    //Events
    public delegate void LevelReady();
    public static event LevelReady OnLevelReady = () => { };

    public delegate void TimerEnded();
    public static event TimerEnded OnTimerEnded = () => { };

    public delegate void PlayerSecured();
    public static event PlayerSecured OnPlayerSecured = () => { };

    public static Transform PlayerSpawnPosition;
    public static Transform PlayerTransform { get; private set; } = null;
    public static PlayerController Player { get; private set; } = null;
    public static float LevelLoadProgress { get; private set; } = 0f;
    public static float TimerProgress { get; private set; } = 0f;

    [Header("Level Manager Settings")]
    [SerializeField] private List<EnemyZone> EnemyZones = new List<EnemyZone>();
    [SerializeField] private List<CollectibleSpawner> CollectibleSpawners = new List<CollectibleSpawner>();
    [Tooltip("Time is in seconds")]
    [SerializeField] public float DayDuration = 300f;
    [Header("Light Cycle Settings")]
    [SerializeField] private Transform LightTransform;
    [SerializeField] private Vector3 StartRotation;
    [SerializeField] private Vector3 EndRotation;

    private int SpawnerToSetUp = 0;
    private int SpawnerSetUpped = 0;

    private Coroutine LightCycleCoroutine;

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

        PlayerSpawnPosition = transform;
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
    
    private IEnumerator LightCycle()
    {
        TimerProgress = 0f;

        Quaternion initialQuaternion = Quaternion.Euler(StartRotation);
        Quaternion finalQuaternion = Quaternion.Euler(EndRotation);

        while(TimerProgress < DayDuration)
        {
            LightTransform.rotation = Quaternion.Lerp(initialQuaternion, finalQuaternion, TimerProgress/DayDuration);
            TimerProgress += Time.deltaTime;
            yield return null;
        }

        OnTimerEnded();

        LightCycleCoroutine = null;
    }

    private void StartLevel()
    {
        SpawnAll();
        if(LightCycleCoroutine != null)
        {
            StopCoroutine(LightCycleCoroutine);
            LightCycleCoroutine = null;
        }
        LightCycleCoroutine = StartCoroutine(LightCycle());
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

        //Reset Player Pos
        if(Player != null)
            Player.transform.position = PlayerSpawnPosition.position;

        if (LightCycleCoroutine != null)
        {
            StopCoroutine(LightCycleCoroutine);
            LightCycleCoroutine = null;
        }
        LightCycleCoroutine = StartCoroutine(LightCycle());

        SpawnAll();
    }

    private void UpdateLoadProgress()
    {
        //value that go from 0 to 1
        SpawnerSetUpped++;
        LevelLoadProgress = SpawnerSetUpped / (float)SpawnerToSetUp;
        
        if( LevelLoadProgress > 0.99999f )
            OnLevelReady();
    }

    private void StopDayCycle()
    {
        StopCoroutine(LightCycleCoroutine);
    }

    private void SetPlayerTransform(PlayerController controller)
    {
        PlayerTransform = controller.transform;
        Player = controller;
        Player.transform.position = PlayerSpawnPosition.position;
    }

    private void OnEnable()
    {
        //Level Manager Events
        OnLevelReady += StartLevel;

        //Spawner events
        //Set up collectible events
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].OnReady += UpdateLoadProgress;

        //Set up Enemy events
        for (int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].OnReady += UpdateLoadProgress;

        //Player events
        PlayerController.OnPlayerReady += SetPlayerTransform;

        //Game manager
        GameManager.OnNewDay += ResetLevel;
        GameManager.OnEndDay += StopDayCycle;
    }

    private void OnDisable()
    {
        Player = null;
        //Level Manager Events
        OnLevelReady -= StartLevel;

        //Spawner events
        //Set up collectible events
        for (int i = 0; i < CollectibleSpawners.Count; i++)
            CollectibleSpawners[i].OnReady -= UpdateLoadProgress;

        //Set up Enemy events
        for (int i = 0; i < EnemyZones.Count; i++)
            EnemyZones[i].OnReady -= UpdateLoadProgress;

        //Player events
        PlayerController.OnPlayerReady -= SetPlayerTransform;

        //Game manager
        GameManager.OnNewDay -= ResetLevel;
        GameManager.OnEndDay -= StopDayCycle;
    }
}
