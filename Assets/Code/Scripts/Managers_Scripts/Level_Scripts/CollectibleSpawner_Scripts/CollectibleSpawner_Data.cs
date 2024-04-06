using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CollectibleSpawner_Data
{
    [SerializeField] public Collectible Collectible;
    [SerializeField][Range(0f, 1f)] public float Probability;
}
