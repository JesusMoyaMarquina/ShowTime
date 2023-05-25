using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeInLightScript : MonoBehaviour
{
    public void SetFadedInTrue()
    {
        CinematicManager.Instance.SetFadedInTrue();
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }
}
