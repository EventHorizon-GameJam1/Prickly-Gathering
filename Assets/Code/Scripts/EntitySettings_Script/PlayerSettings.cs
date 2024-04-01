using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Settings", menuName = "ScriptableSettings/Entity/Player")]
public class PlayerSettings : ScriptableObject
{
    [Header("Player Settings")]
    [SerializeField] private float m_ParrySustainTime = 0.5f;
    [SerializeField] private float m_ParryInvulnerabilityTime = 0.25f;
    public float ParrySustainTime { private set; get; }
    public float ParryInvulnerabilityTime { private set; get; }

    [Header("Movement Settings")]
    [SerializeField] public PlayerMovement Movement;
    [Header("Animation Settings")]
    [SerializeField] public PlayerAnimationController AnimationController;

    private void Awake()
    {
        ParrySustainTime = m_ParrySustainTime;
        ParryInvulnerabilityTime = m_ParryInvulnerabilityTime;
    }
}