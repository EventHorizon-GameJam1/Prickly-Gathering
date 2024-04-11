using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameScreen : UI_Screen
{
    [Header("Bar Indicators")]
    [SerializeField] private Image Hp_Bar;
    [SerializeField] private Image Stamina_Bar;
    [Header("Score Indicator")]
    [SerializeField] private TMP_Text ScoreText;
    [SerializeField] private TMP_Text SecuredScoreText;
    [Header("Time Indicator")]
    [SerializeField] private Image TimerImage;
    [SerializeField] private Vector3 FinalRotation;
    [SerializeField] private TMP_Text TimerText;

    public PlayerController Player;
    private Quaternion TimerImageStartRotation;
    private Quaternion TimerImageFinalRotation;

    private float PlayerStartHP;
    private float PlayerStartStamina;
    private float PlayerHP;

    private Coroutine TimerCoroutine;

    private void Awake()
    {
        TimerImageStartRotation = TimerImage.rectTransform.rotation;
        TimerImageFinalRotation = Quaternion.Euler(FinalRotation);
    }

    private void UpdatePlayer_HP()
    {
        PlayerHP = Player.PlayerHP;
        Hp_Bar.fillAmount = PlayerHP / PlayerStartHP;        
    }

    private void UpdatePlayer_Stamina()
    {
        Stamina_Bar.fillAmount = Player.Stamina / PlayerStartStamina;
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

    private void OnEnable()
    {
        GameManager.OnScoreChanged += UpdateScore;
        GameManager.OnSecuredScoreChanged += UpdateSecuredScore;
        GameManager.OnNewDay += StartTimer;
        PlayerController.OnHPChanged += UpdatePlayer_HP;
        PlayerController.OnStaminaChanged += UpdatePlayer_Stamina;
    }

    private void OnDisable()
    {
        GameManager.OnScoreChanged -= UpdateScore;
        GameManager.OnSecuredScoreChanged -= UpdateSecuredScore;
        GameManager.OnNewDay -= StartTimer;
        PlayerController.OnHPChanged -= UpdatePlayer_HP;
        PlayerController.OnStaminaChanged -= UpdatePlayer_Stamina;


        if (TimerCoroutine != null)
            StopCoroutine(TimerCoroutine);
    }
}
