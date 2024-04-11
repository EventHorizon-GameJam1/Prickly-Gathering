using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Player Settings", menuName = "Settings/Entity/Player")]
public class PlayerSettings : ScriptableObject
{
    [Header("Player Settings")]
    //DOTO: decrease enemrgy when run
    [SerializeField] private float m_PlayerStamina = 10f;
    [SerializeField] private int m_PlayerHP = 10;
    [SerializeField] private float m_ParrySustainTime = 0.5f;
    [SerializeField] private float m_ParryInvulnerabilityTime = 0.25f;
    [SerializeField, Range(0f, 1f)] private float m_EnergyRecoveredOnParry = 0.1f;
    [Tooltip("Energy usage per sprint frame")]
    [SerializeField, Min(0.0001f)] public float EnergyUsage = 0.1f;
    public float ParrySustainTime { private set; get; }
    public float ParryInvulnerabilityTime { private set; get; }
    public float PlayerStartStamina { private set; get; }
    public int PlayerHP { private set; get; }
    public float EnergyRecoveredOnParry { private set; get; }

    [Header("Movement Settings")]
    [SerializeField] public PlayerMovement Movement;
    [Space]
    [HideInInspector] public AnimationController AnimationController;
    [Space]
    [Header("SFX Settigs")]
    [SerializeField] public AudioClip Walking_SFX;
    [SerializeField] public AudioClip Running_SFX;
    [SerializeField] public AudioClip Parry_SFX;

    private void OnEnable()
    {
        ParrySustainTime = m_ParrySustainTime;
        ParryInvulnerabilityTime = m_ParryInvulnerabilityTime;
        PlayerStartStamina = m_PlayerStamina;
        PlayerHP = m_PlayerHP;
        EnergyRecoveredOnParry = m_EnergyRecoveredOnParry;
    }
}