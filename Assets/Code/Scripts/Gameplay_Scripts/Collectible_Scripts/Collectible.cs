using UnityEngine;

[RequireComponent(typeof(CapsuleCollider))]

public class Collectible : MonoBehaviour, ICollectible
{
    public delegate void CollectCollectible(float value);
    public static CollectCollectible OnCollect = (float score) => { };
    public static CollectCollectible OnHpCollect = (float hpRegen) => { };

    [SerializeField] CollectibleSetting ColectibleSetting;

    public float CollectibleScore { get; set ; }
    public float CollectibleRegeneration { get; set; }



    private void Awake()
    {
        CollectibleScore = ColectibleSetting.ScoreOnCollect;
        CollectibleRegeneration = ColectibleSetting.HpOnCollect;
    }

    public void Collect()
    {
        this.gameObject.SetActive(false);
        OnCollect(CollectibleScore);
        OnHpCollect(CollectibleRegeneration);
    }
}