using System;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class GameManager : MonoBehaviour, IPunObservable
{
    public static GameManager Instance;

    public GameState state;

    public bool multiplayerMode;

    public static event Action<GameState> OnGameStateChange;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.IsInRoom())
        {
            HandleMultiplayerMode();
        }
        else
        {
            HandleSinglePlayerMode();
        }

        UpdateGameState(GameState.Spawning);
    }

    public void UpdateGameState(GameState newState)
    {
        state = newState;

        switch (newState)
        {
            case GameState.Spawning:
                HandleSpawning();
                break;
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

    private void HandleSpawning()
    {
        Debug.Log("Handle Spawning");
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
        print(NetworkManager.Instance.IsInRoom());
        if (NetworkManager.Instance != null && !NetworkManager.Instance.IsInRoom())
        {
            Time.timeScale = 0f;
        }
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

    public void HandlePlayerLeftGame(Photon.Realtime.Player player)
    {
        print($"The player {player} disconnected");
    }

    private void HandleMultiplayerMode()
    {
        if (NetworkManager.Instance != null && NetworkManager.Instance.IsMaster())
        {
            print("Master");
            Instantiate(Resources.Load("Prefabs/Player"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("Players").transform);
        }
        else
        {
            print("Not master");
            Instantiate(Resources.Load("Prefabs/Players/SecondaryPlayer"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("Players").transform);
        }
        multiplayerMode = true;
    }

    private void HandleSinglePlayerMode()
    {
        multiplayerMode = false;
        Instantiate(Resources.Load("Prefabs/Players/Player"), new Vector3(0, 0, 0), Quaternion.identity, GameObject.Find("Players").transform);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        throw new NotImplementedException();
    }
}

public enum GameState
{
    Spawning,
    Dialogs,
    Combat,
    Pause,
    Vicory,
    Lose
}
