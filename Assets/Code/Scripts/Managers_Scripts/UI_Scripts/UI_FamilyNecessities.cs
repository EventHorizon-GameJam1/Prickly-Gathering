using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_FamilyNecessities : MonoBehaviour
{
    public delegate void ContinueToNewDay();
    public static event ContinueToNewDay OnContinueToNewDay = () => { };

    [SerializeField] private List<UI_Necessities_Data> FamilyNecessities_UIElements;
    [SerializeField] private Button ContinueButton;
    [Header("Fading Animation Settings")]
    [SerializeField] private Image FadeImage;
    [SerializeField] private Color FadeImageStartColor;
    [SerializeField] private Color FadeImageEndColor;
    [SerializeField][Min(0)] private float FadeTime = 1.5f;
    [Header("Show Animation Settings")]
    [SerializeField][Min(0)] private float FamilyNecessitiesTime = 1.5f;
    [SerializeField] private AnimationCurve FamilyNecessitiesAnimationCurve;
    [SerializeField][Min(0)] private float DelayBetweenShow = 1.5f;

    private List<FamilyNecessities_Data> FamilyNecessities;
    private WaitForSeconds WaitTime;

    private float ScoreBudget;
    private float ScoreAdded = 0f;

    private void Start()
    {
        WaitTime = new WaitForSeconds(DelayBetweenShow);
        FamilyNecessities = GameManager.Instance.GameFamilySetting.FamilyNecessities;
        ContinueButton.interactable = false;
        InitializeSubUI();
    }

    private void OnEnable()
    {
        StartCoroutine(FadeOut());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void InitializeSubUI()
    {
        for(int i= 0; i<FamilyNecessities.Count; i++)
        {
            FamilyNecessities_UIElements[i].Name_Text.text = FamilyNecessities[i].Name;
            FamilyNecessities_UIElements[i].Necessities_Text.text = "0/"+FamilyNecessities[i].ScoreRequested.ToString();
        }
    }

    private IEnumerator FadeIn()
    {
        float progress = 0f;
        float fadeSpeed = 1f / FadeTime;

        while(progress < FadeTime)
        {
            FadeImage.color = Color.Lerp(FadeImageStartColor, FadeImageEndColor, progress);
            progress += Time.deltaTime * fadeSpeed;
            yield return null;
        }
        
        OnContinueToNewDay();
    }

    private IEnumerator FadeOut()
    {
        float progress = 0f;
        float fadeSpeed = 1 / FadeTime;

        while (progress < FadeTime)
        {
            FadeImage.color = Color.Lerp(FadeImageEndColor, FadeImageStartColor, progress);
            progress += Time.deltaTime * fadeSpeed;
            yield return null;
        }

        StartCoroutine(AnimateAll());
    }

    private IEnumerator AnimateAll()
    {
        ScoreBudget = GameManager.SecuredScore;

        for (int i = 0; i< FamilyNecessities.Count; i++)
        {
            StartCoroutine(AnimateFamilyNecessities(i));
            yield return WaitTime;
        }
        ContinueButton.interactable = true;
    }

    private IEnumerator AnimateFamilyNecessities(int i)
    {
        float progress = 0f;
        float animationSpeed = 1f / FamilyNecessitiesTime;
        
        while (progress<1f)
        {
            FamilyNecessities_UIElements[i].SliderImage.fillAmount = FamilyNecessitiesAnimationCurve.Evaluate(progress);
            FamilyNecessities_UIElements[i].Necessities_Text.text = (int)(FamilyNecessities_UIElements[i].SliderImage.fillAmount*FamilyNecessities[i].ScoreRequested) +"/" + FamilyNecessities[i].ScoreRequested.ToString();
            progress += Time.deltaTime * animationSpeed;
            yield return null;
        }
        
    }

    public void Continue()
    {
        StartCoroutine(FadeIn());
        ContinueButton.interactable = false;
    }
}
