using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    public float maximum, current;
    public Image mask;
    public TextMeshProUGUI text;

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void GetCurrentFill()
    {
        mask.fillAmount = current / maximum;
    }
}
