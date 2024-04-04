using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Settings", menuName = "ScriptableSettings/Entity/Enemy")]
public class EnemySettings : ScriptableObject
{
    [Header("Enemy Settings")]
    [SerializeField] public float AttackDistance = 1.5f;
    [SerializeField] public float SprintDistance = 10f;
    [SerializeField] public float TriggerDistance = 5f;
    [SerializeField] public float EnemyDetermination = 1f;
    [SerializeField] public float Damage = 1f;
    [Space]
    [Header("Movement Settings")]
    [SerializeField] public EnemyMovement Movement;
    [Header("Animation Settings")]
    [SerializeField] public AnimationController AnimationController;
    [Space]
    [Header("SFX Settigs")]
    [SerializeField] public AudioClip Idling_SFX;
    [SerializeField] public AudioClip Patrolling_SFX;
    [SerializeField] public AudioClip Attack_SFX;
    [SerializeField] public AudioClip Fleeing_SFX;
    [Header("Debug Gizsmo")]
    [SerializeField] public Color TriggerRadiousColor = new Color32(128, 55, 171, 255);
    [SerializeField] public Color AttackRadiousColor = new Color32(202, 48, 48, 255);
    [SerializeField] public Color SprintRadiousColor = new Color32(219, 106, 0, 127);
}