using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemySpawner_Data
{
    [SerializeField] public EnemyController EnemyToSpawn = null;
    [SerializeField] public Transform EnemyCollectible = null;
    [SerializeField][Range(0f, 1f)] public float Probability = 0.25f;
}
