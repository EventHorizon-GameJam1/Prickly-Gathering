using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UI_Generic : MonoBehaviour
{
    [SerializeField] private string SceneToLoad = "none";
    public void LoadScene()
    {
        if(SceneToLoad != "none")
            SceneManager.LoadScene(SceneToLoad);
    }

    public void CloseApplication()
    {
        Application.Quit();
    }
}
