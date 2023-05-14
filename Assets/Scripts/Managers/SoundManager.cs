using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    //Keys
    public const string MAIN_KEY = "mainVolume";
    public const string MUSIC_KEY = "musicVolume";
    public const string FX_KEY = "fxVolume";

    public static SoundManager instance;

    public AudioSource battleMusic, menuMusic, FXSource;
    public AudioClip gameThemeMusic;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } 
        else
        {
            Destroy(gameObject);
        }

        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        SceneManager.sceneLoaded += SceneManagerOnSceneLoaded;
    }

    private void SceneManagerOnSceneLoaded(Scene scene, LoadSceneMode loadMode)
    {
        if (battleMusic == null || menuMusic == null || battleMusic.enabled == false || menuMusic.enabled == false)
        {
            return;
        }

            switch (scene.buildIndex)
        {
            case 0:
                battleMusic.Stop();
                menuMusic.Play();
                break;
            case 1:
                menuMusic.Stop();
                break;
        }
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        if (battleMusic == null || menuMusic == null || battleMusic.enabled == false || menuMusic.enabled == false)
        {
            return;
        }

        switch (state)
        {
            case GameState.Cinematics:
                break;
            case GameState.Combat:
                battleMusic.Play();
                menuMusic.Pause();
                break;
            case GameState.Pause:
                battleMusic.Pause();
                menuMusic.Play();
                break;
            case GameState.Vicory:
                battleMusic.Stop();
                menuMusic.Play();
                break;
            case GameState.Lose:
                battleMusic.Stop();
                menuMusic.Play();
                break;
        }
    }

    public void PlayOneShot(AudioClip audioClip)
    {
        FXSource.PlayOneShot(audioClip);
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }
}
