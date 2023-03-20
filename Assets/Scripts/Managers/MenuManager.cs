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
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;
    public GameObject keybindsMenu;
    public AudioMixer audioMixer;
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle;
    public static bool isPaused;

    [SerializeField] private InputActionAsset inputActions;

    private GameObject previousMenu;
    private Resolution[] resolutions;
    private List<Resolution> realResolutions;

    // Start is called before the first frame update
    void Start()
    {
        HandleMenuVisibility();
        GetResolutionList();
        fullscreenToggle.isOn = Screen.fullScreen;
    }

    private void HandleMenuVisibility()
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
        keybindsMenu?.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        HandlePauseMenuInputs();
        HandleSettingsMenuInputs();
        HandleKeybindingMenuInputs();
    }

    #region General menu options
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

            if (realResolutions[i].width == Screen.currentResolution.width && realResolutions[i].height == Screen.currentResolution.height)
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
        if (pauseMenu != null && Input.GetButtonDown("Cancel") && (pauseMenu.activeSelf || !isPaused))
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
