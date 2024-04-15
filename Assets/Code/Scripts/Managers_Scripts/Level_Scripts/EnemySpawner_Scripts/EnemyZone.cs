using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnemyZone : MonoBehaviour
{
    public delegate void CollectibleSpawnerReady();
    public event CollectibleSpawnerReady OnReady = () => { };

    [Header("Spawner Settings")]
    [SerializeField] private float SpawnRadius = 15f;
    [SerializeField] private List<EnemySpawner_Data> SpawnData;
    [SerializeField] private PatrollingData EnemyPath = new PatrollingData();
    [Space]
    [Header("Debug Settings")]
    [SerializeField] private Color SpawnAreaColor = Color.red;
    [SerializeField] private Color PathColor = Color.white;
    [SerializeField] private Color PathPointsColor = Color.blue;
    [SerializeField] private float PathPointsSize = 0.2f;
    [SerializeField] private Color EscapepointColor = Color.magenta;

    private List<EnemyController> EnemiesSpawned = new List<EnemyController>();

    public void SetUp()
    {
        //instanciate all enemies
        for (int i = 0; i < SpawnData.Count; i++)
        {
            //Instanciate Enemies and disable it
            EnemyController EnemyToSpawn = SpawnData[i].EnemyToSpawn;
            EnemyToSpawn.EnemyPatrollingPath = EnemyPath;
            EnemiesSpawned.Add(Instantiate(EnemyToSpawn, transform.position, Quaternion.identity));
            EnemiesSpawned[i].gameObject.SetActive(false);
        }
        //Spawner is now ready
        OnReady();
    }
    
    public void ResetSpawner()
    {
        //Disable all enemies
        for (int i = 0; i < SpawnData.Count; i++)
        {
            //Disale enemy
            EnemiesSpawned[i].gameObject.SetActive(false);
        }
    }

    public void Spawn()
    {
        //weighted random
        //probability array
        float[] probabilityArr = new float[SpawnData.Count];
        probabilityArr[0] = SpawnData[0].Probability;
        for (int i = 1; i < SpawnData.Count; i++)
            probabilityArr[i] = probabilityArr[i - 1] + SpawnData[i].Probability;
        //get random value
        float randomValue = UnityEngine.Random.value;
        //select only one enemy
        for(int i = 0; i < EnemiesSpawned.Count; i++)
        {
            if (randomValue < probabilityArr[i])
            {
                Vector3 randomPos = Random.insideUnitSphere * SpawnRadius;
                randomPos.y = 0f;
                EnemiesSpawned[i].EnemyPatrollingPath = EnemyPath;
                EnemiesSpawned[i].transform.position = randomPos + transform.position;
                EnemiesSpawned[i].gameObject.SetActive(true);
                return;
            }
        }
        Debug.LogWarning("Error while selecting wich enemy to spawn");
        EnemiesSpawned[0].gameObject.SetActive(true);
    }

    public void SetPlayerTransform(Transform PlayerTransform)
    {
        for (int i = 0; i < EnemiesSpawned.Count; i++)
        {
            EnemiesSpawned[i].SetPlayerTransform(PlayerTransform);
        }
    }


    private void OnEnable()
    {
        GameManager.OnEndDay += ResetSpawner;
    }

    private void OnDisable()
    {
        GameManager.OnEndDay -= ResetSpawner;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        //Draw Spawn Zone
        Handles.color = SpawnAreaColor;
        Handles.DrawWireDisc(transform.position, Vector3.up, SpawnRadius);
        //Path
        List<Transform> patrollingPos = EnemyPath.PatrollingPositions;
        if(patrollingPos.Count >= 2)
        {
            Handles.color = PathColor;
            for (int i = 0; i < patrollingPos.Count - 1; i++)
            {
                Handles.DrawLine(patrollingPos[i].position, patrollingPos[i + 1].position);
            }
            Handles.DrawLine(patrollingPos[patrollingPos.Count - 1].position, patrollingPos[0].position);
            //PathPoints
            Gizmos.color = PathPointsColor;
            for (int i = 0; i < patrollingPos.Count - 1; i++)
            {
                Gizmos.DrawWireSphere(patrollingPos[i].position, PathPointsSize);
            }
        }
        //Escape Point
        if(EnemyPath.EscapePoint.position != null)
        {
            Gizmos.color = EscapepointColor;
            Gizmos.DrawWireSphere(EnemyPath.EscapePoint.position, PathPointsSize);
        }
    }
#endif
}
