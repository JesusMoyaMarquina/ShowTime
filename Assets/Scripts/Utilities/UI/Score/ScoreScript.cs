using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreScript : MonoBehaviour
{
    public TextMeshProUGUI titleTMP;
    public TextMeshProUGUI baseScoreTMP;
    public TextMeshProUGUI totalScoreTMP;

    public void Inicialize(string title, string multiplier, string baseScore, string totalScore, string numberOf)
    {
        titleTMP.text = $"{title} x{multiplier}";
        baseScoreTMP.text = $"{numberOf} = {baseScore}";
        totalScoreTMP.text = totalScore;
    }
}
