using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Sigleton
    public static GameManager Instance;

    //Evenets
    public delegate void GameStateChanged();
    public static GameStateChanged OnGameStateChanged = () => {};

    public static bool OnPause { private set; get; } = false;
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

    private void MenuCalled()
    {
        if (OnPause)
            UnPauseGame();
        else
            PauseGame();
        OnGameStateChanged();
    }

    private void UnPauseGame()
    {
        Time.timeScale = 1f;
        OnPause = false;
        Debug.Log(OnPause);
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        OnPause = true;
        Debug.Log(OnPause);
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
        LevelManager.OnLevelReady += UnPauseGame;
        //Pause Game
        InputManager.OnMenuCalled += MenuCalled;
    }
}
