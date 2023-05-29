using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectDifficultyScript : MonoBehaviour
{
    public static SelectDifficultyScript Instance;

    private int difficulty;

    private void Start()
    {
        Instance = this;
    }

    public void SetDifficulty(int difficulty)
    {
        this.difficulty = difficulty;
    }

    public void OpenTrainingZoneSettings()
    {
        MenuManager.Instance.OpenTrainingZoneSettings();
    }

    public void StartGame()
    {
        MenuManager.isPaused = false;
        SceneManager.LoadScene("Game");
    }

    public void Return()
    {
        MenuManager.Instance.principalSceneMenu?.SetActive(true);
        gameObject?.SetActive(false);
    }

    public int GetDifficulty()
    {
        return difficulty;
    }
}
