using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICollectible
{
    public float CollectibleScore { get; set; }
    public float CollectibleRegeneration { get; set; }
    public void Collect();
}
