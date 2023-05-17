using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;
    public GameState previousGameState;

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        UpdateGameState(GameState.Combat);
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
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandleCombat()
    {
        print("HandleCombat");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandleCombatFinished()
    {
        print("HandleCombatFinished");
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1f;
    }

    private void HandlePause()
    {
        print("HandlePause");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0f;
    }

    private void HandleVictory()
    {
        print("HandleVictory");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }

    private void HandleLose()
    {
        print("HandleLose");
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 1f;
    }
}

public enum GameState
{
    Cinematics,
    Combat,
    CombatFinished,
    Pause,
    Vicory,
    Lose
}
