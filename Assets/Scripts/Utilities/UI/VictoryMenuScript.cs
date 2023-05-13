using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenuScript : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        MenuManager.Instance.BackToMainMenu();
    }
}
