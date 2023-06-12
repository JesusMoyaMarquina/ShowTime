using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChargeBarScript : MonoBehaviour
{
    public float maximum, current;
    public Image mask;

    public void GetCurrentFill()
    {
        mask.fillAmount = current / maximum;
    }
}
