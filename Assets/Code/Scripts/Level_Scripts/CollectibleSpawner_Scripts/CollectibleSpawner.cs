using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    [Header("Collectible spawner Settings")]
    [SerializeField] private CollectibleSpawnerSettings SpawnSettings;
    [SerializeField] private Vector2 SpawnArea = Vector2.one;
    //Collectible 
    private List<Collectible> SpawnedCollectible = new List<Collectible>();

    private void Awake()
    {
        for(int i = 0; i < SpawnedCollectible.Count; i++)
        {
            for(int j = 0; j < SpawnSettings.MaxCollectibleAmount; j++)
            {

            }
        }
    }

    private void Spawn()
    {
        //weighted random
        //probability array
        float[] probabilityArr = new float[SpawnSettings.CollectibleToSpawn.Count];
        probabilityArr[0] = SpawnSettings.CollectibleToSpawn[0].Probability;
        for (int i = 1; i < SpawnSettings.CollectibleToSpawn.Count; i++)
            probabilityArr[i] = probabilityArr[i - 1] + SpawnSettings.CollectibleToSpawn[i].Probability;
        //get random value
        float randomValue = UnityEngine.Random.value;
        //select only one enemy
        for (int i = 0; i < SpawnSettings.CollectibleToSpawn.Count; i++)
        {
            if (randomValue < probabilityArr[i])
            {
                SpawnedCollectible[i].gameObject.SetActive(true);
                return;
            }
        }
        Debug.LogWarning("Error while selecting wich enemy to spawn");
        SpawnedCollectible[0].gameObject.SetActive(true);
    }

    public void ResetSpawner()
    {
        for(int i = 0; i < SpawnedCollectible.Count; i++)
        {
            SpawnedCollectible[i].gameObject.SetActive(false);
        }
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = SpawnSettings.SpawnAreaColor;
        Gizmos.DrawWireCube(transform.position, new Vector3(SpawnArea.x , 0f, SpawnArea.y));
    }
#endif
}
