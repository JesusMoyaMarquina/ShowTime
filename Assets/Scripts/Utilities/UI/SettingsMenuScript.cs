using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsMenuScript : MonoBehaviour
{
    public static SettingsMenuScript instance;

    //Keys
    public const string RESOLUTION_KEY = "resolution";
    public const string GRAPHICS_KEY = "graphics";

    //Audios
    public AudioMixer mainAudioMixer, musicAudioMixer, FXAudioMixer;
    public TextMeshProUGUI mainVolumePercentageText, musicVolumePercentageText, FXVolumePercentageText;
    public Slider mainVolumeSlider, musicVolumeSlider, FXVolumeSlider;

    //Dropdowns
    public TMP_Dropdown resolutionDropdown, graphicsDropdown;
    public static string settedResolution = "";

    //Toggles
    public Toggle fullscreenToggle;

    //Resolutions
    private Resolution[] resolutions;
    private List<Resolution> realResolutions;

    private void Awake()
    {
        instance = this;
        GetResolutionList();
        fullscreenToggle.isOn = Screen.fullScreen;
        LoadParams();
    }

    private void Update()
    {
        if (gameObject != null && Input.GetButtonDown("Cancel") && gameObject.activeSelf)
        {
            Return();
        }
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat(SoundManager.MAIN_KEY, mainVolumeSlider.value);
        PlayerPrefs.SetFloat(SoundManager.MUSIC_KEY, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(SoundManager.FX_KEY, FXVolumeSlider.value);
        PlayerPrefs.SetInt(RESOLUTION_KEY, resolutionDropdown.value);
        PlayerPrefs.SetInt(GRAPHICS_KEY, graphicsDropdown.value);
    }

    public void Return()
    {
        MenuManager.Instance.principalSceneMenu?.SetActive(true);
        gameObject?.SetActive(false);
    }

    public void SetMainVolume(float volume)
    {
        mainAudioMixer.SetFloat("volume", volume);
        mainVolumePercentageText.text = (int)((mainVolumeSlider.value * 100 / 80) + 100) + "%";
    }

    public void SetMusicVolume(float volume)
    {
        musicAudioMixer.SetFloat("volume", volume);
        musicVolumePercentageText.text = (int)((musicVolumeSlider.value * 100 / 80) + 100) + "%";
    }

    public void SetFXVolume(float volume)
    {
        FXAudioMixer.SetFloat("volume", volume);
        FXVolumePercentageText.text = (int)((FXVolumeSlider.value * 100 / 80) + 100) + "%";
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    public void SetFullscreen(bool isFullscreen)
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
            if (!actualResolution.Contains($"{resolutions[i].width} x {resolutions[i].height}"))
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
            }
            else if (settedResolution.Contains($"{realResolutions[i].width} x {realResolutions[i].height}"))
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);

        resolutionDropdown.value = currentResolutionIndex;

        resolutionDropdown.RefreshShownValue();
    }

    void LoadParams()
    {
        mainAudioMixer.SetFloat("volume", PlayerPrefs.GetFloat(SoundManager.MAIN_KEY));
        musicAudioMixer.SetFloat("volume", PlayerPrefs.GetFloat(SoundManager.MUSIC_KEY));
        FXAudioMixer.SetFloat("volume", PlayerPrefs.GetFloat(SoundManager.FX_KEY));
        resolutionDropdown.value = PlayerPrefs.GetInt(RESOLUTION_KEY);
        graphicsDropdown.value = PlayerPrefs.GetInt(GRAPHICS_KEY);
    }
}
