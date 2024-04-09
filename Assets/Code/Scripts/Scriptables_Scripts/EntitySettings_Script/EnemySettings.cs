using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Settings", menuName = "Settings/Entity/Enemy")]
public class EnemySettings : ScriptableObject
{
    [Header("Enemy Settings")]
    [SerializeField] public int EnemyDetermination = 3;
    [SerializeField] public float Damage = 1f;
    [SerializeField][Range(0f, 1f)] public float PercentageLost = 0.25f;
    [Space]
    [Header("Movement Settings")]
    [SerializeField] public float AttackDistance = 1.5f;
    [SerializeField] public float SprintDistance = 10f;
    [SerializeField] public float TriggerDistance = 5f;
    [SerializeField] public EnemyMovement Movement;
    [Header("Idle Settings")]
    [SerializeField][Range(0f, 1f)] public float IdleProbability = 0.25f;
    [SerializeField][Min(0)] public float IdleTime_Min = 1f;
    [SerializeField][Min(0)] public float IdleTime_Max = 2f;
    [Header("Animation Settings")]
    [SerializeField] public AnimationController AnimationController;
    [Space]
    [Header("SFX Settigs")]
    [SerializeField] public AudioClip Idling_SFX;
    [SerializeField] public AudioClip Patrolling_SFX;
    [SerializeField] public AudioClip Attack_SFX;
    [SerializeField] public AudioClip Fleeing_SFX;
    [SerializeField] public AudioClip EnemySpotted_SFX;
    [Header("Debug Gizsmo")]
    [SerializeField] public Color TriggerRadiousColor = new Color32(128, 55, 171, 255);
    [SerializeField] public Color AttackRadiousColor = new Color32(202, 48, 48, 255);
    [SerializeField] public Color SprintRadiousColor = new Color32(219, 106, 0, 127);
}