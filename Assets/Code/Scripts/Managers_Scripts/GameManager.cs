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
        Time.timeScale = 0f;
    }

    private void StartGame()
    {
        Time.timeScale = 1f;
    }

    private void IncraseScore(float score)
    {
        Score += score;
    }
    
    private void OnEnable()
    {
        //Collectible events
        Collectible.OnCollect += IncraseScore;
        //Level manager event
        LevelManager.OnLevelReady += StartGame;
    }
}
