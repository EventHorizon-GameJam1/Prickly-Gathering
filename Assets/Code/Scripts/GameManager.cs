using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Sigleton
    public static GameManager Instance;

    private float Score = 0f;

    private void Awake()
    {
        //Set up singleton
        if(Instance == null)
            Instance = this;
        else
        {
            Debug.Log("WARNING: MULTIPLE GAME MANAGER INSTANCES FOUND REMOVING GAME OBJECT");
            Destroy(this.gameObject);
        }
    }

    private void IncraseScore(float score)
    {
        Score += score;
        Debug.Log("Score Increased by "+score+"\nNow is "+ Score);
    }
    
    private void OnEnable()
    {
        Collectible.OnCollect += IncraseScore;
    }
}
