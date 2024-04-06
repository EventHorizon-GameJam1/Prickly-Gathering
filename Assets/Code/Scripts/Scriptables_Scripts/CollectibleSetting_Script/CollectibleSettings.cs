using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Collectible Settings", menuName = "Settings/Collectible/Collectible")]
public class CollectibleSetting : ScriptableObject
{
    [Header("Collectible Settings")]
    [SerializeField] public float ScoreOnCollect = 0f;
}
