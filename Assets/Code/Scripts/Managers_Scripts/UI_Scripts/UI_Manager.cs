using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private UI_Screen Game_UI;
    [SerializeField] private UI_PauseScreen Pause_UI;

    private void Start()
    {
        ChangeState();
    }

    private void ChangeState()
    {
        if(GameManager.OnPause)
        {
            //Show pause menu
            Game_UI.Canvas.gameObject.SetActive(false);
            Pause_UI.Canvas.gameObject.SetActive(true);
        }
        else
        {
            //Show game menu
            Game_UI.Canvas.gameObject.SetActive(true);
            Pause_UI.Canvas.gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += ChangeState;
    }
}
