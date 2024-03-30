using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy Settings", menuName = "ScriptableSettings/Entity/Enemy")]
public class EnemySettings : ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] public Movement Movement;
}