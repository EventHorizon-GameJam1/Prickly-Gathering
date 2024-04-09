using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Family Settings", menuName = "Settings/Family")]
public class FamilySettings : ScriptableObject
{
    [Header("Family Settings")]
    [SerializeField] public List<FamilyNecessities_Data> FamilyNecessities;
}
