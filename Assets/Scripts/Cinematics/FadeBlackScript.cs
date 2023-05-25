using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeBlackScript : MonoBehaviour
{
    public void UpdateGameState(GameState state)
    {
        GameManager.Instance.UpdateGameState(state);
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
