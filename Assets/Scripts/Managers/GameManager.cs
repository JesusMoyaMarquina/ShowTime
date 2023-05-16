using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameState state;

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
        Debug.Log("Handle Dialogs");
    }

    private void HandleCombat()
    {
        Time.timeScale = 1f;
    }

    private void HandlePause()
    {
        Time.timeScale = 0f;
    }

    private void HandleVictory()
    {
        Time.timeScale = 1f;
    }

    private void HandleLose()
    {
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
