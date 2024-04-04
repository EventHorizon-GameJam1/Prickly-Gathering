using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    //Singleton
    public static LevelManager Instance;

    [Header("Level Manager Settings")]
    //TODO: Add sky cycle
    [SerializeField] private List<EnemyZone> EnemySpawners = new List<EnemyZone>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this.gameObject);
    }
}
