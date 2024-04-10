using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    [Header("Game Manager Settings")]
    [SerializeField] private int MaxDays = 3;
    [SerializeField] public FamilySettings GameFamilySetting;

    public static bool OnPause { private set; get; } = false;
    public static float Score { private set; get; } = 0f;
    public static float SecuredScore { private set; get; } = 0f;

    private int DayCounter = 0;
    private List<int> FamilyScore = new List<int>();
    private int FamilyMemberSatisfied = 0;

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
            FamilyScore.Add(0);

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
        Score = Score * percentage;
    }

    private void SecureScore(float percentage)
    {
        //Update Score
        SecuredScore += (Score*percentage);
        Score -= Score*percentage;
        OnSecuredScoreChanged();
        OnScoreChanged();
        //Update Family Necessities
        //TODO: check family necessities
        /*
        float checkedScore = SecuredScore;
        for (int i = 0; i < GameFamilySetting.FamilyNecessities.Count; i++)
        {
            if (GameFamilySetting.FamilyNecessities[i].ScoreRequested >= SecuredScore)
            {
                checkedScore -= GameFamilySetting.FamilyNecessities[i].ScoreRequested;
                FamilyMemberSatisfied++;
            }
        }
        */
        if (FamilyMemberSatisfied >= GameFamilySetting.FamilyNecessities.Count)
        {
            GameWon();
            return;
        }
        EndDay();
    }
    #endregion

    private void GameOver()
    {
        //TODO: GAME OVER
        Debug.Log("game Over");
    }

    private void GameWon()
    {
        //TODO: GAME WON
        Debug.Log("game Won!");
    }

    private void EndDay()
    {
        DayCounter++;
        if(DayCounter > MaxDays)
        {
            GameOver();
            return;
        }
        OnEndDay();
    }

    private void NewDay()
    {
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
}
