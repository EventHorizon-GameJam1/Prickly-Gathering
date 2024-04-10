using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private UI_GameScreen Game_UI;
    [SerializeField] private UI_PauseScreen Pause_UI;
    [SerializeField] private UI_FamilyNecessities FamilyNecessities_UI;
    [SerializeField] private TMP_Text CommunicationText;

    private void Start()
    {
        CommunicationText.gameObject.SetActive(false);
        ChangeState();
    }

    private void ChangeState()
    {
        if(GameManager.OnPause)
        {
            //Show pause menu
            Game_UI.Canvas.gameObject.SetActive(false);
            Pause_UI.Canvas.gameObject.SetActive(true);
            FamilyNecessities_UI.gameObject.SetActive(false);
        }
        else
        {
            //Show game menu
            Game_UI.Canvas.gameObject.SetActive(true);
            Pause_UI.Canvas.gameObject.SetActive(false);
            FamilyNecessities_UI.gameObject.SetActive(false);
        }
    }

    private void CommunicateStoreValue(float percentage)
    {
        CommunicationText.gameObject.SetActive(true);
        CommunicationText.text = "Interact To End The Day And Store "+ percentage*100 + "% of Resources";
    }

    private void HideCommunication(float percentage)
    {
        CommunicationText.gameObject.SetActive(false);
    }

    private void ShowFamilyNecessities()
    {
        FamilyNecessities_UI.gameObject.SetActive(true);
        Game_UI.Canvas.gameObject.SetActive(false);
        Pause_UI.Canvas.gameObject.SetActive(false);
    }

    private void HideFamilyNecessities()
    {
        FamilyNecessities_UI.gameObject.SetActive(false);
        Game_UI.Canvas.gameObject.SetActive(true);
        Pause_UI.Canvas.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        UI_FamilyNecessities.OnContinueToNewDay += HideFamilyNecessities;
        GameManager.OnGameStateChanged += ChangeState;
        GameManager.OnEndDay += ShowFamilyNecessities;
        Dem.OnSecureAvailable += CommunicateStoreValue;
        Dem.OnSecureNotAvailable += HideCommunication;
    }
}
