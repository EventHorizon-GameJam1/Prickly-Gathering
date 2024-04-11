using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GameManager;

public class GameManager : MonoBehaviour
{
    //Sigleton
    public static GameManager Instance;

    //Evenets
    public delegate void GameStateChanged();
    public static GameStateChanged OnGameStateChanged = () => { };

    public delegate void ScoreChanged();
    public static ScoreChanged OnScoreChanged = () => { };
    public static ScoreChanged OnSecuredScoreChanged = () => { };

    public delegate void GameDay();
    public static GameDay OnNewDay = () => { };
    public static GameDay OnEndDay = () => { };

    public delegate void EndGame();
    public static EndGame OnGameWon = () => { };
    public static EndGame OnGameOver = () => { };

    [Header("Game Manager Settings")]
    [SerializeField] private int MaxDays = 3;
    [SerializeField] public FamilySettings GameFamilySetting;
    [SerializeField] private string Win_Scene = "none";
    [SerializeField] private string Lose_Scene = "none";

    public static bool OnPause { private set; get; } = false;
    public static float Score { private set; get; } = 0f;
    public static float SecuredScore { private set; get; } = 0f;

    private int DayCounter = 0;
    private float ScoreToReach = 0;

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

        for (int i = 0; i < GameFamilySetting.FamilyNecessities.Count; i++)
            ScoreToReach += GameFamilySetting.FamilyNecessities[i].ScoreRequested;
    }

    #region MENU
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
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        OnPause = true;
    }
    #endregion

    #region SCORE
    private void IncraseScore(float score)
    {
        Score += score;
        OnScoreChanged();
    }

    private void LoseScore(float damage, float percentage)
    {
        Score = (int)(Score * percentage);
        OnScoreChanged();
    }

    private void SecureScore(float percentage)
    {
        //Update Score
        SecuredScore += (Score*percentage);
        Score -= Score*percentage;
        OnSecuredScoreChanged();
        OnScoreChanged();
        
        EndDay();
    }
    #endregion

    private void GameOver()
    {
        if(Lose_Scene != "none")
            SceneManager.LoadScene(Lose_Scene);
    }

    private void GameWon()
    {
        if (Win_Scene != "none")
            SceneManager.LoadScene(Win_Scene);
    }

    private void EndDay()
    {
        OnEndDay();
    }

    private void NewDay()
    {
        DayCounter++;

        if (SecuredScore >= ScoreToReach)
        {
            GameWon();
            return;
        }

        if (DayCounter > MaxDays)
        {
            GameOver();
            return;
        }

        OnNewDay();
    }
    
    private void OnEnable()
    {
        //Collectible events
        Collectible.OnCollect += IncraseScore;
        //Level manager event
        LevelManager.OnLevelReady += UnPauseGame;
        LevelManager.OnTimerEnded += EndDay;
        //Pause Game
        InputManager.OnMenuCalled += MenuCalled;
        //Enemy Connection
        EnemyController.OnPlayerDamaged += LoseScore;
        //Player connection
        PlayerController.OnPlayerDefeated += GameOver;
        //Dem connection
        Dem.OnPlayerSecured += SecureScore;
        //Continue to new level event
        UI_FamilyNecessities.OnContinueToNewDay += NewDay;
    }

    private void OnDisable()
    {
        OnGameStateChanged -= OnGameStateChanged;
        OnScoreChanged -= OnScoreChanged;
        OnSecuredScoreChanged -= OnSecuredScoreChanged;
        OnGameOver -= OnGameOver;
        OnGameWon -= OnGameWon;
        OnNewDay -= OnNewDay;
        OnEndDay -= OnEndDay;

        //Collectible events
        Collectible.OnCollect -= IncraseScore;
        //Level manager event
        LevelManager.OnLevelReady -= UnPauseGame;
        LevelManager.OnTimerEnded -= EndDay;
        //Pause Game
        InputManager.OnMenuCalled -= MenuCalled;
        //Enemy Connection
        EnemyController.OnPlayerDamaged -= LoseScore;
        //Player connection
        PlayerController.OnPlayerDefeated -= GameOver;
        //Dem connection
        Dem.OnPlayerSecured -= SecureScore; 
        //Continue to new level event
        UI_FamilyNecessities.OnContinueToNewDay -= NewDay;
    }
}
