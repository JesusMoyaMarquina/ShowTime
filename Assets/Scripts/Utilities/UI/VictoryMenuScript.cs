using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryMenuScript : MonoBehaviour
{
    public GameObject scoreList;
    public GameObject scorePanel;

    public void ReturnToMainMenu()
    {
        MenuManager.Instance.BackToMainMenu();
    }


    private void ScoreLoad()
    {
        string previousName = "";
        //Load score panels
        for (int i = 0; i < CombatManager.instance.GetScores().Count; i++)
        {
            //Set panel position
            float yPosition = scoreList.transform.localPosition.y - scorePanel.transform.GetComponent<RectTransform>().sizeDelta.y * i - scorePanel.transform.GetComponent<RectTransform>().sizeDelta.y / 2;
            GameObject actualScorePanel = Instantiate(scorePanel, new Vector3(0, 0, 0), Quaternion.identity, scoreList.transform);
            actualScorePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yPosition, 0);

            //Fill panel info
            Score score = CombatManager.instance.GetScores()[i];
            actualScorePanel.GetComponent<ScoreScript>().Inicialize(score.title, score.score, score.multiplier);
        }
    }
}
