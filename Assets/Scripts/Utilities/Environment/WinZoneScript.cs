using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinZoneScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameManager.Instance.UpdateGameState(GameState.Vicory);
    }
}
