using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EntityProgressBar : MonoBehaviour
{
    public float maximum, current, previousCurrent, progressSpeed;
    public Image realMask, progressMask;
    public TextMeshProUGUI text;

    private void FixedUpdate()
    {
        if (previousCurrent >= current) 
        {
            progressMask.fillAmount = previousCurrent / maximum;
            previousCurrent -= progressSpeed;
        }
    }

    public void SetText(string text)
    {
        this.text.text = text;
    }

    public void GetCurrentFill()
    {
        realMask.fillAmount = current / maximum;
    }
}
