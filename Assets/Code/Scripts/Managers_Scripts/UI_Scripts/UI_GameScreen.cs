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

    [HideInInspector] public PlayerController Player;
    private Quaternion TimerImageStartRotation;
    private Quaternion TimerImageFinalRotation;

    public float PlayerStartHP;
    public float PlayerStartStamina;
    private float PlayerHP;

    private Coroutine TimerCoroutine;

    private void Awake()
    {
        TimerImageStartRotation = TimerImage.rectTransform.rotation;
        TimerImageFinalRotation = Quaternion.Euler(FinalRotation);
    }

    private void UpdatePlayer_HP()
    {
        if (Player == null)
            return;

        PlayerHP = Player.PlayerHP;
        Hp_Bar.fillAmount = PlayerHP / PlayerStartHP;        
    }

    private void UpdatePlayer_Stamina()
    {
        if (Player == null)
            return;

        Stamina_Bar.fillAmount = Player.Stamina / PlayerStartStamina;
    }

    private void StartTimer()
    {
        TimerCoroutine = StartCoroutine(UpdateTimer());
    }

    private IEnumerator UpdateTimer()
    {
        int min, sec;

        TimerImage.rectTransform.rotation = TimerImageStartRotation;

        Vector3 startRotEuler = TimerImage.rectTransform.rotation.eulerAngles;

        Vector3 middleRot = (FinalRotation - startRotEuler)/2;
        middleRot += Vector3.forward * 0.05f;

        Quaternion targetRot = Quaternion.Euler(middleRot);


        while (true)
        {
            sec = (int)LevelManager.TimerProgress;
            min = (int)(sec / 60f);
            if (sec >= 60)
                sec -= min * 60;

            float progress = LevelManager.TimerProgress / LevelManager.Instance.DayDuration;

            if (progress <= 0.5)
                TimerImage.rectTransform.rotation = Quaternion.Lerp(TimerImageStartRotation, targetRot, progress/0.5f);
            else
                TimerImage.rectTransform.rotation = Quaternion.Lerp(targetRot, TimerImageFinalRotation, (progress-0.5f)/0.5f);


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
