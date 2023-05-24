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

    public AudioSource battleMusic, menuMusic, bossBattleMusic, battleFinishMusic, loseMusic, victoryMusic, FXSource;

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
                menuMusic.Stop();
                bossBattleMusic.Stop();
                victoryMusic.Stop();
                loseMusic.Stop();
                battleFinishMusic.Stop();
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
                menuMusic.Stop();
                break;
            case GameState.Combat:
                battleMusic.Play();
                menuMusic.Stop();
                bossBattleMusic.Stop();
                victoryMusic.Stop();
                loseMusic.Stop();
                break;
            case GameState.BossCombat:
                battleMusic.Stop();
                menuMusic.Pause();
                bossBattleMusic.Play();
                break;
            case GameState.CombatFinished:
                bossBattleMusic.Stop();
                menuMusic.Pause();
                battleFinishMusic.Play();
                break;
            case GameState.Pause:
                if (GameManager.Instance.previousGameState == GameState.Combat)
                {
                    battleMusic.Pause();
                }
                else if (GameManager.Instance.previousGameState == GameState.BossCombat)
                {
                    bossBattleMusic.Pause();
                }
                menuMusic.Play();
                break;
            case GameState.Vicory:
                bossBattleMusic.Stop();
                battleMusic.Stop();
                battleFinishMusic.Stop();
                menuMusic.Pause();
                victoryMusic.Play();
                break;
            case GameState.Lose:
                bossBattleMusic.Stop();
                battleFinishMusic.Stop();
                battleMusic.Stop();
                menuMusic.Pause();
                loseMusic.Play();
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
