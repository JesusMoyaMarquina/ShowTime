using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public static bool isPaused;

    private GameObject previousMenu;

    // Start is called before the first frame update
    void Start()
    {
        if (mainMenu != null)
        {
            mainMenu?.SetActive(true);
        }
        if (pauseMenu != null)
        {
            pauseMenu?.SetActive(false);
        }
        settingsMenu?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandlePauseMenuInputs();
    }

    #region Pause settings options
    public void OpenSettings(GameObject menu)
    {
        previousMenu = menu;
        previousMenu?.SetActive(false);
        settingsMenu?.SetActive(true);
    }
    public void Return()
    {
        previousMenu?.SetActive(true);
        settingsMenu?.SetActive(false);
    }
    #endregion

    #region Main menu options
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Pause menu options

    public void PauseGame()
    {
        pauseMenu?.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenu?.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }


    private void HandlePauseMenuInputs()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    #endregion

}
