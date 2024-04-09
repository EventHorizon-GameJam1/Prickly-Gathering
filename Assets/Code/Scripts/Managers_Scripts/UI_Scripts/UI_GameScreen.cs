using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScreen : UI_Screen
{
    [Header("Bar Indicators")]
    [SerializeField] private Image Hp_Bar;
    [SerializeField] private Image Sprint_Bar;
    [Header("Score Indicator")]
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text SecuredScoreText;
    [Header("Time Indicator")]
    [SerializeField] private Image TimerImage;
    [SerializeField] private Vector3 FinalRotation;
    [SerializeField] private TMP_Text TimerText;

    private PlayerController Player;
    private Quaternion TimerImageStartRotation;
    private Quaternion TimerImageFinalRotation;

    private Coroutine TimerCoroutine;

    private void Awake()
    {
        TimerImageStartRotation = TimerImage.rectTransform.rotation;
        TimerImageFinalRotation = Quaternion.Euler(FinalRotation);
    }

    private void Start()
    {
        StartTimer();
    }

    private void UpdatePlayer_HP()
    {

    }

    private void UpdatePlayer_Sprint()
    {

    }

    private void StartTimer()
    {
        TimerCoroutine = StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        int min, sec;

        while (true)
        {
            sec = (int)LevelManager.TimerProgress;
            min = (int)(sec / 60f);
            if (sec >= 60)
                sec -= min * 60;

            TimerImage.rectTransform.rotation = Quaternion.Lerp(TimerImageStartRotation, TimerImageFinalRotation, LevelManager.TimerProgress/LevelManager.Instance.DayDuration);

            TimerText.text = min + " : " + sec;
            yield return null;
        }
    }
    
    private void UpdateScore()
    {
        ScoreText.text = GameManager.Score.ToString();
    }

    private void UpdateSecuredScore()
    {
        SecuredScoreText.text = GameManager.SecuredScore.ToString();
    }

    private void GetPlayer(PlayerController player)
    {
        Player = player;
    }

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnSecuredScoreChanged += UpdateSecuredScore;
        PlayerController.OnPlayerReady += GetPlayer;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnSecuredScoreChanged -= UpdateSecuredScore;
        PlayerController.OnPlayerReady -= GetPlayer;

        if (TimerCoroutine != null)
            StopCoroutine(TimerCoroutine);
    }
}