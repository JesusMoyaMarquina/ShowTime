using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditScreensScript : MonoBehaviour
{
    public void Return()
    {
        MenuManager.Instance.principalSceneMenu?.SetActive(true);
        gameObject?.SetActive(false);
    }
}
