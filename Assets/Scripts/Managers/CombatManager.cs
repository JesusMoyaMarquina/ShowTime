using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    #region Unit variables
    public UnitManager unitManager;

    public int startGenerateUnit, unitIncremental, secondsToGenerate;

    private int generateIteration;
    #endregion

    #region Timer variables
    public GameObject timerProgressBar, timerMultiplierProgressBar;

    public float multiplierTime, combatTime, beginBattleTime;

    private bool multiplierActive, timerPause;

    private float actualTime, acumulatedMultiplierTime, previousTimes, previousTimesNonMultiplied, remainingTime, timerSpeed;
    #endregion

    #region Combo variables
    public GameObject hitsText;

    public float timerMultiplyPercentage, maximumMultiplier;

    public int comboToMultiply;

    private float comboMp = 1;

    protected int cSHelp = 0;
    #endregion

    #region Game state variables
    public GameObject winZone;
    #endregion

    #region Score variables
    public float baseScorePerCombo;

    private List<Score> scores = new List<Score>();
    #endregion

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        RestartGameTimer();
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        switch (state)
        {
            case GameState.Combat:
                CutPreviousTimer();
                break;
            case GameState.CombatFinished:
                FillScoreUI();
                break;
            case GameState.Pause:
                acumulatedMultiplierTime = actualTime;
                break;
            case GameState.Vicory:
                break;
        }
    }

    void Update()
    {
        if (GameManager.Instance.state == GameState.Combat || GameManager.Instance.state == GameState.CombatFinished)
        {
            HandleCombatInputs();
            print(remainingTime);
            if (Mathf.CeilToInt(remainingTime) == combatTime - secondsToGenerate * generateIteration || remainingTime == combatTime)
            {
                GenerateUnits();
            }
        }
    }

    public List<Score> GetScores()
    {
        return scores.OrderBy(o => o.multiplier).ToList(); ;
    }

    private void FillScoreUI()
    {
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.state == GameState.Combat)
        {
            if (!timerPause)
            {
                ManageBattleTime();
            }
            ManageLoseCondition();
            ManageWinCondition();
        }
    }

    private void GenerateUnits()
    {
        unitManager.GenerateUnits(startGenerateUnit + unitIncremental * generateIteration);
        generateIteration++;
    }

    private void HandleCombatInputs()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.Instance.UpdateGameState(GameState.Pause);
        }
    }

    private void KillAllEnemies()
    {
        foreach(EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            enemy.Die();
        }
    }

    private void OpenWinZone()
    {
        winZone.SetActive(true);
    }

    #region State conditions
    private void ManageLoseCondition()
    {
        Transform players = GameObject.Find("Players").transform;
        int totalOfPlayers = players.childCount;
        int deadPlayers = 0;

        for(int i = 0; i < totalOfPlayers; i++)
        {
            if (!players.GetChild(i).GetComponent<Player>().isAlive())
            {
                deadPlayers++;
            }
        }

        if(deadPlayers == totalOfPlayers) 
        {
            GameManager.Instance.UpdateGameState(GameState.Lose);
        }
    }

    private void ManageWinCondition()
    {
        if (remainingTime <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.CombatFinished);
            StopUI();
            KillAllEnemies();
            OpenWinZone();
        }
    }
    #endregion

    #region Timer management
    private void RestartGameTimer()
    {
        timerProgressBar.GetComponent<ProgressBar>().maximum = combatTime;
        timerMultiplierProgressBar.gameObject.SetActive(false);

        generateIteration = 0;
        acumulatedMultiplierTime = 0;
        multiplierActive = false;
        timerSpeed = 1;

        previousTimes = 0;
        previousTimesNonMultiplied = 0;

        beginBattleTime = Time.time;
    }

    public void MultiplyTimeSpeed(float multiplier)
    {
        CutPreviousTimer();
        timerSpeed = multiplier;
        acumulatedMultiplierTime = 0;
        if (multiplier > 1)
        {
            timerMultiplierProgressBar.GetComponent<ProgressBar>().SetText($"x{multiplier}");
        } else
        {
            timerMultiplierProgressBar.GetComponent<ProgressBar>().SetText($"");
        }
    }

    private void CutPreviousTimer()
    {
        previousTimes += actualTime;
        previousTimesNonMultiplied += actualTime / timerSpeed;
        actualTime = 0;
    }

    private void ManageBattleTime()
    {
        //Update battle time
        float realBattleTime = Time.time - beginBattleTime;
        actualTime = (realBattleTime - previousTimesNonMultiplied) * timerSpeed;
        remainingTime = combatTime - (actualTime + previousTimes);

        //Manage hits text
        if (cSHelp > 0)
        {
            timerMultiplierProgressBar.gameObject.SetActive(true);
            multiplierActive = true;
            hitsText.GetComponent<TextMeshProUGUI>().text = $"Hits {cSHelp}";
            hitsText.SetActive(true);
        }
        else
        {
            hitsText.SetActive(false);
        }

        //Manage multiplier time
        float multiplierTimerTime = multiplierTime - (actualTime) - acumulatedMultiplierTime;
        if (multiplierActive && multiplierTimerTime >= 0)
        {
            timerMultiplierProgressBar.GetComponent<ProgressBar>().maximum = multiplierTime;
            timerMultiplierProgressBar.GetComponent<ProgressBar>().current = multiplierTimerTime;
            timerMultiplierProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        }
        else if (multiplierActive && multiplierTimerTime < 0)
        {
            ComboSystem(false);
        }


        //Format time string
        TimeSpan t = TimeSpan.FromSeconds((int)Math.Ceiling(remainingTime));
        string sTime = t.ToString(@"mm\:ss");


        //Update graphic values
        timerProgressBar.GetComponent<ProgressBar>().current = combatTime - remainingTime;
        timerProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        timerProgressBar.GetComponent<ProgressBar>().SetText(sTime);
    }

    void StopUI()
    {
        timerPause = true;
        timerMultiplierProgressBar.GetComponent<ProgressBar>().current = 0;
        timerMultiplierProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        ComboSystem(false);
    }
    #endregion

    #region combo system
    public void ComboSystem(bool hit)
    {
        if (hit)
        {
            if ((cSHelp + 1) % comboToMultiply == 0 && comboMp < maximumMultiplier)
            {
                cSHelp++;
                comboMp += timerMultiplyPercentage / 100;
            }
            else
            {
                cSHelp++;
            }
            MultiplyTimeSpeed(comboMp);
        }

        if (!hit)
        {
            //Add score
            AddHitScore(cSHelp, comboMp);

            //Hide UI
            timerMultiplierProgressBar.gameObject.SetActive(false);
            multiplierActive = false;
            hitsText.SetActive(false);

            //ResetVatiables
            cSHelp = 0;
            comboMp = 1f;

            //Reset multiplier
            MultiplyTimeSpeed(1);
        }
    }
    #endregion

    #region score system
    public void AddHitScore(int hits, float multiplier = 1)
    {
        if (hits > 0)
        {
            AddScore(hits * baseScorePerCombo, "Hits", multiplier, hits);
        }
    }

    public void AddKillScore(float fscore = 0)
    {
        AddScore(fscore, "Kills", comboMp);
    }

    private void AddScore(float fscore = 0, string title = "Hits", float multiplier = 1, int numberOf = 1)
    {
        Score score = scores.Find(o => o.multiplier == multiplier && o.title == title);

        if (score == null)
        {
            scores.Add(score = new Score());
            score.multiplier = multiplier;
            score.title = title;
        }

        score.score += fscore;
        score.numberOf += numberOf;
    }
    #endregion
}
