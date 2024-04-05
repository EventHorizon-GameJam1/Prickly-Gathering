using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible Spawner Settings", menuName = "Settings/Collectible/Spawner")]
public class CollectibleSpawnerSettings : ScriptableObject
{
    [Header("Spawner Settings")]
    [SerializeField] public int MaxCollectibleAmount = 10;
    [SerializeField] public int MinCollectibleAmount = 5;
    [SerializeField] public List<CollectibleSpawner_Data> CollectibleToSpawn = new List<CollectibleSpawner_Data>();
    [Header("Debug")]
    [SerializeField] public Color SpawnAreaColor = Color.cyan;
}
