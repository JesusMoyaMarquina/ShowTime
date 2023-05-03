using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class MenuManager : MonoBehaviour
{
    public GameObject victoryMenu, loseMenu, mainMenu, pauseMenu, settingsMenu, keybindsMenu, gameUI;

    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public TextMeshProUGUI volumePercentageText;
    public Slider volumeSlider;

    public static bool isPaused;

    public static string settedResolution = "";

    [SerializeField] private InputActionAsset inputActions;

    private GameObject previousMenu;
    private Resolution[] resolutions;
    private List<Resolution> realResolutions;

    private bool closeDelay;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        HandleGamePause(state == GameState.Pause);
        victoryMenu.SetActive(state == GameState.Vicory);
        loseMenu.SetActive(state == GameState.Lose);
    }

    private void Start()
    {
        HandleMenuVisibility();
        GetResolutionList();
        fullscreenToggle.isOn = Screen.fullScreen;
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
        settingsMenu?.SetActive(false);
        keybindsMenu?.SetActive(false);
    }

    void Update()
    {
        HandlePauseMenuInputs();
        HandleSettingsMenuInputs();
        HandleKeybindingMenuInputs();
    }

    #region General menu options
    public void BackToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void Return(GameObject actualMenu)
    {
        previousMenu?.SetActive(true);
        actualMenu?.SetActive(false);
    }
    #endregion

    #region Settings menu options
    public void OpenSettings(GameObject menu)
    {
        previousMenu = menu;
        previousMenu?.SetActive(false);
        settingsMenu?.SetActive(true);
    }

    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
        volumePercentageText.text = (int) ((volumeSlider.value * 100 / 80) + 100) + "%";
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen (bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = realResolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        settedResolution = $"{realResolutions[resolutionIndex].width} x {realResolutions[resolutionIndex].height}";
    }

    private void GetResolutionList()
    {
        resolutions = Screen.resolutions;

        resolutionDropdown.ClearOptions();

        realResolutions = new List<Resolution>();
        List<string> options = new List<string>();

        //Crear of resolution duplicates
        string actualResolution = "";
        for (int i = 0; i < resolutions.Length; i++)
        {
            if(!actualResolution.Contains($"{resolutions[i].width} x {resolutions[i].height}"))
            {
                actualResolution = $"{resolutions[i].width} x {resolutions[i].height}";
                realResolutions.Add(resolutions[i]);
            }
        }

        int currentResolutionIndex = 0;
        for (int i = 0; i < realResolutions.Count; i++)
        {
            string option = $"{realResolutions[i].width} x {realResolutions[i].height}";
            options.Add(option);

            if (realResolutions[i].width == Screen.currentResolution.width && realResolutions[i].height == Screen.currentResolution.height && settedResolution.Equals(""))
            {
                currentResolutionIndex = i;
            } else if (settedResolution.Contains($"{realResolutions[i].width} x {realResolutions[i].height}"))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;

        resolutionDropdown.RefreshShownValue();
    }

    private void HandleSettingsMenuInputs()
    {
        if (settingsMenu != null && Input.GetButtonDown("Cancel") && settingsMenu.activeSelf)
        {
            Return(settingsMenu);
        }
    }
    #endregion

    #region Main menu options
    public void StartGame()
    {
        isPaused = false;
        SceneManager.LoadScene("Game");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    #endregion

    #region Pause menu options

    private void HandleGamePause(bool paused)
    {
        if(paused)
        {
            StartCoroutine(CloseMenuDelay());
        }
        if (pauseMenu != null)
        {
            gameUI?.SetActive(!paused);
            pauseMenu?.SetActive(paused);
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.UpdateGameState(GameState.Combat);
    }

    private void HandlePauseMenuInputs()
    {
        if(pauseMenu != null && pauseMenu.activeSelf && Input.GetButtonDown("Cancel") && !closeDelay) 
        {
            GameManager.Instance.UpdateGameState(GameState.Combat);
        }
    }

    IEnumerator CloseMenuDelay()
    {
        closeDelay = true;
        yield return new WaitForEndOfFrame();
        closeDelay = false;

    }
    #endregion

    #region Keybinds menu options
    public void OpenKeybinds()
    {
        settingsMenu?.SetActive(false);
        keybindsMenu?.SetActive(true);
    }

    public void KeybindsReturn()
    {
        settingsMenu?.SetActive(true);
        keybindsMenu?.SetActive(false);
    }

    public void ResetAllBindings()
    {
        foreach(InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    private void HandleKeybindingMenuInputs()
    {
        if (keybindsMenu != null && Input.GetButtonDown("Cancel") && keybindsMenu.activeSelf)
        {
            KeybindsReturn();
        }
    }
    #endregion

}
