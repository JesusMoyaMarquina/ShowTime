using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public void OpenSettings()
    {
        MenuManager.Instance.OpenSettings();
    }

    public void StartGame()
    {
        MenuManager.Instance.OpenDifficulty();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
