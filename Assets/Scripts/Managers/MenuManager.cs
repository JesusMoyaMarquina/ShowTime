using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance;

    //Canvases
    public GameObject principalSceneMenu;
    public GameObject settingsMenu, keybindsMenu, selectDifficultyMenu;
    private GameObject victoryMenu, loseMenu, mainMenu, pauseMenu, gameUI, creditsScreen, trainZoneSettingsMenu, cinematicCanvas;

    //State
    public bool closeDelay;
    public static bool isPaused;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;

        Instance = this;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    private void SceneManagerOnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        SetVariableMenus();
        principalSceneMenu = mainMenu == null ? pauseMenu : mainMenu;
        HandleMenuVisibility();
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        if (CombatManager.instance != null)
        {
            victoryMenu.SetActive(state == GameState.Vicory);
            loseMenu.SetActive(state == GameState.Lose);
            gameUI?.SetActive(GameManager.Instance.isInCombat);
        } 
        else if (TrainManagerScript.Instance != null)
        {
            gameUI?.SetActive(GameManager.Instance.isInCombat);
        }
    }

    private void SetVariableMenus()
    {
        //Main menu menus
        mainMenu = GameObject.Find("MainMenu");

        //Game scene menus
        victoryMenu = GameObject.Find("VictoryMenu");
        loseMenu = GameObject.Find("LoseMenu");
        pauseMenu = GameObject.Find("PauseMenu");
        if(GameManager.Instance != null)
        {
            gameUI = GameObject.Find("GameUI");
        }
        else if (TrainManagerScript.Instance != null)
        {
            gameUI = GameObject.Find("TrainUI");
        }
        cinematicCanvas = GameObject.Find("CinematicCanvas");
        trainZoneSettingsMenu = GameObject.Find("TrainingZoneSettings");
        creditsScreen = GameObject.Find("CreditsScreen");

    }

    private void HandleMenuVisibility()
    {
        if(gameUI != null)
        {
            gameUI?.SetActive(true);
        }
        if (mainMenu != null)
        {
            mainMenu?.SetActive(true);
        }
        if (pauseMenu != null)
        {
            pauseMenu?.SetActive(false);
        }
        if (settingsMenu != null)
        {
            settingsMenu?.SetActive(false);
        }
        if (keybindsMenu != null)
        {
            keybindsMenu?.SetActive(false);
        }
        if(selectDifficultyMenu != null)
        {
            selectDifficultyMenu?.SetActive(false);
        }
        if (trainZoneSettingsMenu != null)
        {
            trainZoneSettingsMenu?.SetActive(false);
        }
        if (creditsScreen != null)
        {
            creditsScreen?.SetActive(false);
        }
        if (cinematicCanvas != null)
        {
            cinematicCanvas?.SetActive(false);
        }
    }

    public IEnumerator CloseMenuDelay()
    {
        closeDelay = true;
        yield return new WaitForEndOfFrame();
        closeDelay = false;
    }

    #region General menu options
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void OpenCredits()
    {
        principalSceneMenu?.SetActive(false);
        FindObjectOfType<CreditScreensScript>(true).gameObject.SetActive(true);
    }

    public void OpenSettings()
    {
        principalSceneMenu?.SetActive(false);
        FindObjectOfType<SettingsMenuScript>(true).gameObject.SetActive(true);
    }

    public void OpenTrainingZoneSettings()
    {
        FindObjectOfType<SelectDifficultyScript>(false).gameObject.SetActive(false);
        FindObjectOfType<TrainingZoneSettingsScript>(true).gameObject.SetActive(true);
    }

    public void OpenDifficulty()
    {
        principalSceneMenu?.SetActive(false);
        FindObjectOfType<SelectDifficultyScript>(true).gameObject.SetActive(true);
    }
    #endregion
}
