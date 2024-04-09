using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PauseScreen: UI_Screen
{
    [Header("Button Canvas")]
    [SerializeField] private Canvas ButtonsCanvas;
    [Header("Audio Menu Screen")]
    [SerializeField] private Canvas AudioMenuCanvas;
    [SerializeField] private UI_AudioSettings AudioSettings;
    [Header("Main Menu")]
    [SerializeField] private string MainMenuScene = "none";

    public void ShowAudioCanvas()
    {
        ButtonsCanvas.gameObject.SetActive(false);
        AudioMenuCanvas.gameObject.SetActive(true);
    }

    public void Back()
    {
        ButtonsCanvas.gameObject.SetActive(true);
        AudioMenuCanvas.gameObject.SetActive(false);
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void MainMenu()
    {
        if(MainMenuScene != "none")
            SceneManager.LoadScene(MainMenuScene);
    }

    private void OnEnable()
    {
        ButtonsCanvas.gameObject.SetActive(true);
        AudioMenuCanvas.gameObject.SetActive(false);
    }
}
