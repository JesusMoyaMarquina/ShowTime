using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class VictoryMenuScript : MonoBehaviour
{
    public GameObject ScoreScrollView;
    public GameObject scoreList;
    public GameObject scorePanel;
    public GameObject totalScoreText;

    public float timeBetweenScorePanel;
    public float totalScoreIncremental;

    public int countFPS = 30;
    public float duration = 0.5f;
    private float totalScore = 0;

    private Coroutine CountingCoroutine;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
    }

    private void UpdateText(float newTotalScore) 
    {
        if (CountingCoroutine != null)
        {
            StopCoroutine(CountingCoroutine);
        }
        CountingCoroutine = StartCoroutine(CountText(newTotalScore));
    }

    private IEnumerator CountText(float newTotalScore)
    {
        float previousTotalScore = totalScore;
        float stepAmount;

        stepAmount = (newTotalScore - previousTotalScore) / (countFPS + duration);

        if(previousTotalScore < newTotalScore)
        {
            while(previousTotalScore < newTotalScore)
            {
                previousTotalScore += stepAmount;
                if(previousTotalScore > newTotalScore)
                {
                    previousTotalScore = newTotalScore;
                }

                totalScoreText.GetComponent<TextMeshProUGUI>().text = $"Total Score: {string.Format("{0:0.00}", previousTotalScore)}";

                yield return new WaitForSeconds(1f / countFPS);
            }
        } 
        else
        {
            while (previousTotalScore > newTotalScore)
            {
                previousTotalScore += stepAmount;
                if (previousTotalScore < newTotalScore)
                {
                    previousTotalScore = newTotalScore;
                }

                totalScoreText.GetComponent<TextMeshProUGUI>().text = $"Total Score: {string.Format("{0:0.00}", previousTotalScore)}";

                yield return new WaitForSeconds(1f / countFPS);
            }
        }
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Vicory:
                if (!gameObject.activeSelf)
                {
                    gameObject.SetActive(true);
                }
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
        ScoreScrollView.GetComponent<ScrollRect>().vertical = false;
        float newTotalScore = 0;

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

            newTotalScore += score.GetTotalScore();

            //Resize container
            scoreList.GetComponent<RectTransform>().sizeDelta = new Vector2(scoreList.GetComponent<RectTransform>().sizeDelta.x, scorePanel.transform.GetComponent<RectTransform>().sizeDelta.y * scores.Count);
        
            yield return new WaitForSeconds(timeBetweenScorePanel);
        }

        ScoreScrollView.GetComponent<ScrollRect>().vertical = true;
        UpdateText(newTotalScore);
    }
}
