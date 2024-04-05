using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]

public class Collectible : MonoBehaviour, ICollectible
{
    public delegate void CollectCollectible(float score);
    public static CollectCollectible OnCollect = (float score) => { };

    [SerializeField] CollectibleSetting ColectibleSetting;

    public float CollectibleScore { get; set ; }

    private void Awake()
    {
        CollectibleScore = ColectibleSetting.ScoreOnCollect;
    }

    public void Collect()
    {
        this.gameObject.SetActive(false);
        OnCollect(CollectibleScore);
    }
}