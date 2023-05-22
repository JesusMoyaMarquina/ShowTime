using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool isInCombat;
    public GameState previousGameState;
    private GameState state;

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Cinematics);
    }

    public void UpdateGameState(GameState newState)
    {
        previousGameState = state;
        state = newState;

        switch (newState)
        {
            case GameState.Cinematics:
                HandleCinematic();
                break;
            case GameState.Combat:
                HandleCombat();
                break;
            case GameState.BossCombat:
                HandleBossCombat();
                break;
            case GameState.CombatFinished:
                HandleCombatFinished();
                break;
            case GameState.Pause:
                HandlePause();
                break;
            case GameState.Vicory:
                HandleVictory();
                break;
            case GameState.Lose:
                HandleLose();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChange?.Invoke(newState);
    }

    private void HandleCinematic()
    {
        print("HandleCinematic");

        isInCombat = false;

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandleCombat()
    {
        print("HandleCombat");

        isInCombat = true;

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandleBossCombat()
    {
        print("HandleBossCombat");

        isInCombat = true;

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandleCombatFinished()
    {
        print("HandleCombatFinished");

        isInCombat = true;
        FindObjectOfType<WinZoneScript>(true).gameObject.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandlePause()
    {
        print("HandlePause");

        isInCombat = false;

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    private void HandleVictory()
    {
        print("HandleVictory");

        isInCombat = false;

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    private void HandleLose()
    {
        print("HandleLose");

        isInCombat = false;

        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }
}

public enum GameState
{
    Cinematics,
    Combat,
    BossCombat,
    CombatFinished,
    Pause,
    Vicory,
    Lose
}
