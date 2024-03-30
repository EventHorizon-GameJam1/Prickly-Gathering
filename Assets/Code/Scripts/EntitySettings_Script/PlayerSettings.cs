using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Settings", menuName = "ScriptableSettings/Entity/Player")]
public class PlayerSettings : ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] public PlayerMovement Movement;
}