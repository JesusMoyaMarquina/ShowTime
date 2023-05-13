using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseMenuScript : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        MenuManager.Instance.BackToMainMenu();
    }
}
