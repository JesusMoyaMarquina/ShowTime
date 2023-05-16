using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VictoryMenuScript : MonoBehaviour
{
    public GameObject scoreList;
    public GameObject scorePanel;
    public GameObject totalScoreText;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Vicory:
                StartCoroutine(ScoreLoad());
                break;
        }
    }

    public void ReturnToMainMenu()
    {
        MenuManager.Instance.BackToMainMenu();
    }


    IEnumerator ScoreLoad()
    {
        float totalScore = 0;

        List<Score> scores = CombatManager.instance.GetScores();

        //Load score panels
        for (int i = 0; i < scores.Count; i++)
        {
            //Set panel position
            float yPosition = scoreList.transform.localPosition.y - scorePanel.transform.GetComponent<RectTransform>().sizeDelta.y * i - scorePanel.transform.GetComponent<RectTransform>().sizeDelta.y / 2;
            GameObject actualScorePanel = Instantiate(scorePanel, new Vector3(0, 0, 0), Quaternion.identity, scoreList.transform);
            actualScorePanel.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yPosition, 0);

            //Fill panel info
            Score score = scores[i];
            actualScorePanel.GetComponent<ScoreScript>().Inicialize(score.title, score.multiplier.ToString(), score.score.ToString(), score.GetTotalScore().ToString(), score.numberOf.ToString());

            yield return new WaitForSeconds(0.5f);

            totalScore += score.GetTotalScore();
        }

        totalScoreText.GetComponent<TextMeshProUGUI>().text = $"Total Score: {totalScore}";
    }
}
