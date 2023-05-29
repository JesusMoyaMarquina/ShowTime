using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrainingZoneSettingsScript : MonoBehaviour
{
    public static SelectDifficultyScript Instance;

    private int enemyType;

    public void StartTrain()
    {
        MenuManager.isPaused = false;
        SceneManager.LoadScene("Train");
    }

    public void SetEnemyType(int enemyType)
    {
        this.enemyType = enemyType;
    }

    public void Return()
    {
        FindObjectOfType<SelectDifficultyScript>(true).gameObject.SetActive(true);
        gameObject.SetActive(false);
    }

    public int GetEnemyType()
    {
        return enemyType;
    }
}
