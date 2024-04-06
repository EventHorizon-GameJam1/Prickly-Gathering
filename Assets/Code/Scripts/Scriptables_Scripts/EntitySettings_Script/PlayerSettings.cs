using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Settings", menuName = "Settings/Entity/Player")]
public class PlayerSettings : ScriptableObject
{
    [Header("Player Settings")]
    //DOTO: decrease enemrgy when run
    [SerializeField] private float EnergyAmount = 10f;
    [SerializeField] private float m_ParrySustainTime = 0.5f;
    [SerializeField] private float m_ParryInvulnerabilityTime = 0.25f;
    public float ParrySustainTime { private set; get; }
    public float ParryInvulnerabilityTime { private set; get; }
    public float PlayerStartStamina { private set; get; }

    [Header("Movement Settings")]
    [SerializeField] public PlayerMovement Movement;
    [Space]
    [HideInInspector] public AnimationController AnimationController;
    [Space]
    [Header("SFX Settigs")]
    [SerializeField] public AudioClip Walking_SFX;
    [SerializeField] public AudioClip Running_SFX;
    [SerializeField] public AudioClip Parry_SFX;

    private void Awake()
    {
        ParrySustainTime = m_ParrySustainTime;
        ParryInvulnerabilityTime = m_ParryInvulnerabilityTime;
        ParrySustainTime = EnergyAmount;
    }
}