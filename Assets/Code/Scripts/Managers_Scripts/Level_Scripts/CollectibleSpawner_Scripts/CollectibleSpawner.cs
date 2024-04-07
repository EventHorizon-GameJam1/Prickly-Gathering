using System.Collections.Generic;
using UnityEngine;

public class CollectibleSpawner : MonoBehaviour
{
    public delegate void CollectibleSpawnerReady();
    public event CollectibleSpawnerReady OnReady = () => {};

    [Header("Collectible spawner Settings")]
    [SerializeField] private CollectibleSpawnerSettings SpawnSettings;
    [SerializeField] private Vector2 SpawnArea = Vector2.one;
    //Collectible list
    private List<Collectible>[] SpawnedCollectible;

    private int SpawnAmount = 0;

    public void SetUp()
    {
        //Set up collectible list
        SpawnedCollectible = new List<Collectible>[SpawnSettings.CollectibleToSpawn.Count];
        for (int i = 0; i < SpawnedCollectible.Length; i++)
            SpawnedCollectible[i] = new List<Collectible>();
        //Instanciate all collectible
        for (int i = 0; i < SpawnSettings.CollectibleToSpawn.Count; i++)
        {
            for(int j = 0; j < SpawnSettings.MinCollectibleAmount; j++)
            {
                Collectible collectibleToAdd = Instantiate(SpawnSettings.CollectibleToSpawn[i].Collectible, transform.position, Quaternion.identity);
                SpawnedCollectible[i].Add(collectibleToAdd);
                SpawnedCollectible[i][j].gameObject.SetActive(false);
            }
        }
        //Spawner is now ready
        OnReady();
    }

    public void ResetSpawner()
    {
        //Deactivate all collectibles
        for(int i = 0; i < SpawnedCollectible.Length; i++)
        {
            for(int j=0;  j < SpawnedCollectible[i].Count; j++)
                SpawnedCollectible[i][j].gameObject.SetActive(false);
        }
        Spawn();
    }

    public void Spawn()
    {
        //New Spawn amount
        SpawnAmount = UnityEngine.Random.Range(SpawnSettings.MinCollectibleAmount, SpawnSettings.MaxCollectibleAmount);

        //Weighted random
        //Probability array
        float[] probabilityArr = new float[SpawnSettings.CollectibleToSpawn.Count];
        probabilityArr[0] = SpawnSettings.CollectibleToSpawn[0].Probability;
        for (int i = 1; i < SpawnSettings.CollectibleToSpawn.Count; i++)
            probabilityArr[i] = probabilityArr[i - 1] + SpawnSettings.CollectibleToSpawn[i].Probability;

        //Activate correct collectibles -> for Spawn amount
        for(int i=0; i<SpawnAmount; i++)
        {
            //Get random value
            float randomValue = UnityEngine.Random.value;
            //Select collectible
            for (int j = 0; j < SpawnSettings.CollectibleToSpawn.Count; j++)
            {
                if (randomValue < probabilityArr[j])
                {
                    //new pos
                    Vector2 newObjectPos = Vector2.zero;
                    newObjectPos.x = UnityEngine.Random.Range(-SpawnArea.x/2, SpawnArea.x/2);
                    newObjectPos.y = UnityEngine.Random.Range(-SpawnArea.y/2, SpawnArea.y/2);

                    bool dirtySpawn = false;
                    //Find first free object of j index
                    for(int k = 0; k < SpawnedCollectible[j].Count; k++)
                    {
                        if (SpawnedCollectible[j][k].gameObject.activeInHierarchy == false)
                        {
                            SpawnedCollectible[j][k].transform.position = transform.position + new Vector3(newObjectPos.x, 0f, newObjectPos.y);
                            //Activate the object
                            SpawnedCollectible[j][k].gameObject.SetActive(true);
                            dirtySpawn = true;
                            break;
                        }
                    }
                    //if no object are found -> instanciate new object
                    if(!dirtySpawn)
                    {
                        Collectible collectibleToAdd = Instantiate(SpawnSettings.CollectibleToSpawn[j].Collectible, transform.position + new Vector3(newObjectPos.x, 0f, newObjectPos.y), Quaternion.identity);
                        SpawnedCollectible[j].Add(collectibleToAdd);
                    }
                    //Stop and select next collectible
                    break;
                }
            }
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
