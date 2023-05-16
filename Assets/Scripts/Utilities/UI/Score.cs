using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    public string title = "Hits";
    public float multiplier = 1;
    public float score = 0;
    public int numberOf = 0;

    public float GetTotalScore()
    {
        return score * multiplier;
    }
}
