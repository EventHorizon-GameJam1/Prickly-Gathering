using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_TitleScreen : MonoBehaviour
{
    [SerializeField] private Canvas TitleScreenCanvas;
    [SerializeField] private Canvas SettingsCanvas;
    [SerializeField] private Canvas CreditsCanvas;
    [SerializeField] private string GameScene = "none";

    public void ShowTitle()
    {
        TitleScreenCanvas.gameObject.SetActive(true);
        SettingsCanvas.gameObject.SetActive(false);
        CreditsCanvas.gameObject.SetActive(false);
    }
    public void ShowCredits()
    {
        TitleScreenCanvas.gameObject.SetActive(false);
        SettingsCanvas.gameObject.SetActive(false);
        CreditsCanvas.gameObject.SetActive(true);
    }
    public void ShowSettings()
    {
        TitleScreenCanvas.gameObject.SetActive(false);
        SettingsCanvas.gameObject.SetActive(true);
        CreditsCanvas.gameObject.SetActive(false);
    }

    public void Play()
    {
        if(GameScene != "none")
            SceneManager.LoadScene(GameScene);
    }
}
