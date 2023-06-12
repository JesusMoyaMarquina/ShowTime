using UnityEngine;

public class PauseMenuScript : MonoBehaviour
{
    void Awake()
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
    }

    void Update()
    {
        HandlePauseMenuInputs();
    }

    public void OpenSettings()
    {
        MenuManager.Instance.principalSceneMenu = gameObject;
        MenuManager.Instance.OpenSettings();
    }

    private void HandlePauseMenuInputs()
    {
        if (gameObject != null && gameObject.activeSelf && Input.GetButtonDown("Cancel") && !MenuManager.Instance.closeDelay)
        {
            GameManager.Instance.UpdateGameState(GameManager.Instance.previousGameState);
        }
    }

    private void HandleGamePause(bool paused)
    {
        if (gameObject != null)
        {
            gameObject?.SetActive(paused);
            if (paused)
            {
                StartCoroutine(MenuManager.Instance.CloseMenuDelay());
            }
        }
    }

    public void ResumeGame()
    {
        GameManager.Instance.UpdateGameState(GameManager.Instance.previousGameState);
    }

    public void ReturnToMainMenu()
    {
        MenuManager.Instance.BackToMainMenu();
    }
}
