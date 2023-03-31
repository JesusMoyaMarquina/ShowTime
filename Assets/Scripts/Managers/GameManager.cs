using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
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
        if (PhotonNetwork.IsMasterClient)
        {
            print("Master");
        }
        else
        {
            print("Not master");
        }
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Dialogs:
                HandleDialogs();
                break;
            case GameState.Combat:
                HandleCombat();
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

    private void HandleDialogs()
    {
        Debug.Log("Handle Dialogs");
    }

    private void HandleCombat()
    {
        Debug.Log("Handle Combat");
        Time.timeScale = 1f;
    }

    private void HandlePause()
    {
        Debug.Log("Handle Pause");
        Time.timeScale = 0f;
    }

    private void HandleVictory()
    {
        Debug.Log("Handle Victory");
        Time.timeScale = 1f;
    }

    private void HandleLose()
    {
        Debug.Log("Handle Lose");
        Time.timeScale = 1f;
    }
}

public enum GameState
{
    Dialogs,
    Combat,
    Pause,
    Vicory,
    Lose
}
