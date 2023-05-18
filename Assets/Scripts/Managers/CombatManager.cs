using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    private CombatState combatState;

    #region Unit variables
    public UnitManager unitManager;

    public int secondsToGenerate;

    private int generateIteration;
    #endregion

    #region Timer variables
    public GameObject timerCombatUI, timerProgressBar, hitsTimerCombatProgressBar, hitsTimerCombatText;

    public float multiplierTime, combatTime, beginBattleTime;

    private bool multiplierActive, timerPause;

    private float actualTime, acumulatedMultiplierTime, previousTimes, previousTimesNonMultiplied, remainingTime, timerSpeed;
    #endregion

    #region Combo variables

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

    #region Boss combat variables
    public GameObject boss;

    public GameObject bossCombatUI, enemyHealthProgressBar, hitsBossCombatProgressBar, hitsBossCombatText;

    public int bossesToKill;
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

                combatState = CombatState.timerCombat;
                CutPreviousTimer();

                break;
            case GameState.CombatFinished:

                combatState = CombatState.combatFinished;
                CutPreviousTimer();

                break;
            case GameState.BossCombat:

                combatState = CombatState.bossCombat;
                CutPreviousTimer();
                combatTime = 0;

                break;
            case GameState.Pause:

                combatState = CombatState.combatFinished;
                acumulatedMultiplierTime = actualTime;

                break;
            case GameState.Vicory:

                combatState = CombatState.pause;

                break;
            case GameState.Lose:

                combatState = CombatState.pause;

                break;
            case GameState.Cinematics:

                combatState = CombatState.pause;

                break;
        }
    }

    void Update()
    {
        if (GameManager.Instance.isInCombat)
        {
            HandleCombatInputs();
            if ((Mathf.CeilToInt(remainingTime) == combatTime - secondsToGenerate * generateIteration || remainingTime == combatTime) && remainingTime > 0)
            {
                GenerateUnits();
            }
        }
    }

    private void FixedUpdate()
    {
        if (!timerPause)
        {
            ManageLoseCondition();
            ManageBattleTime();
        }
        if (GameManager.Instance.isInCombat && combatState == CombatState.timerCombat)
        {
            ShowTimer();
            ShowBossHealth(false);
            ManageCombatChangeCondition();
        } else if (GameManager.Instance.isInCombat && combatState == CombatState.bossCombat)
        {
            if(boss == null)
            {
                ComboSystem(false);
                boss = unitManager.GenerateBoss();
            }
            ManageWinCondition();
            ShowTimer(false);
            ShowBossHealth();
        }
    }

    private void ShowTimer(bool show = true)
    {
        timerCombatUI.SetActive(show);
    }

    private void ShowBossHealth(bool show = true)
    {
        bossCombatUI.SetActive(show);
    }

    private void HandleCombatInputs()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.Instance.UpdateGameState(GameState.Pause);
        }
    }

    #region UI Management
    void StopUI()
    {
        if (timerCombatUI.activeSelf)
        {
            hitsTimerCombatProgressBar.GetComponent<ProgressBar>().current = 0;
            hitsTimerCombatProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        } else if (bossCombatUI.activeSelf)
        {
            hitsBossCombatProgressBar.GetComponent<ProgressBar>().current = 0;
            hitsBossCombatProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        }
        ComboSystem(false);
    }
    #endregion

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

    private void ManageCombatChangeCondition()
    {
        if (remainingTime <= 0)
        {
            StopUI();
            KillAllEnemies();
            GameManager.Instance.UpdateGameState(GameState.BossCombat);
        }
    }

    public void ReduceBossesToKill()
    {
        bossesToKill--;
    }

    private void ManageWinCondition()
    {
        if (bossesToKill <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.CombatFinished);
        }
    }

    private void GenerateUnits()
    {
        unitManager.GenerateUnits();
        unitManager.IncrementUntinsToGenerate();
        generateIteration++;
    }

    private void KillAllEnemies()
    {
        foreach (EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            enemy.Die();
        }
    }
    #endregion

    #region Timer management
    private void RestartGameTimer()
    {
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
        if (timerCombatUI.activeSelf)
        {
            if (multiplier > 1)
            {
                hitsTimerCombatProgressBar.GetComponent<ProgressBar>().SetText($"x{multiplier}");
            }
            else
            {
                hitsTimerCombatProgressBar.GetComponent<ProgressBar>().SetText($"");
            }
        } else if (bossCombatUI.activeSelf)
        {
            if (multiplier > 1)
            {
                hitsBossCombatProgressBar.GetComponent<ProgressBar>().SetText($"x{multiplier}");
            }
            else
            {
                hitsBossCombatProgressBar.GetComponent<ProgressBar>().SetText($"");
            }
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
        if (cSHelp > 0 && timerCombatUI.activeSelf)
        {
            hitsTimerCombatProgressBar.gameObject.SetActive(true);
            multiplierActive = true;
            hitsTimerCombatText.GetComponent<TextMeshProUGUI>().text = $"Hits {cSHelp}";
            hitsTimerCombatText.SetActive(true);
        }
        else if (cSHelp > 0 && bossCombatUI.activeSelf)
        {
            hitsBossCombatProgressBar.gameObject.SetActive(true);
            multiplierActive = true;
            hitsBossCombatText.GetComponent<TextMeshProUGUI>().text = $"Hits {cSHelp}";
            hitsBossCombatText.SetActive(true);
        }
        else if (timerCombatUI.activeSelf)
        {
            hitsTimerCombatProgressBar.SetActive(false);
            hitsTimerCombatText.SetActive(false);
        }
        else if (bossCombatUI.activeSelf)
        {
            hitsBossCombatProgressBar.SetActive(false);
            hitsBossCombatText.SetActive(false);
        }

        //Manage multiplier time
        float multiplierTimerTime = multiplierTime - (actualTime) - acumulatedMultiplierTime;
        if (multiplierActive && multiplierTimerTime >= 0 && timerCombatUI.activeSelf)
        {
            hitsTimerCombatProgressBar.GetComponent<ProgressBar>().maximum = multiplierTime;
            hitsTimerCombatProgressBar.GetComponent<ProgressBar>().current = multiplierTimerTime;
            hitsTimerCombatProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        }
        else if (multiplierActive && multiplierTimerTime >= 0 && bossCombatUI.activeSelf)
        {
            hitsBossCombatProgressBar.GetComponent<ProgressBar>().maximum = multiplierTime;
            hitsBossCombatProgressBar.GetComponent<ProgressBar>().current = multiplierTimerTime;
            hitsBossCombatProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        }
        else if (multiplierActive && multiplierTimerTime < 0)
        {
            ComboSystem(false);
        }


        //Format time string
        TimeSpan t = TimeSpan.FromSeconds((int)Math.Ceiling(remainingTime));
        string sTime = t.ToString(@"mm\:ss");

        //Update graphic values
        timerProgressBar.GetComponent<ProgressBar>().maximum = combatTime;
        timerProgressBar.GetComponent<ProgressBar>().current = combatTime - remainingTime;
        timerProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        timerProgressBar.GetComponent<ProgressBar>().SetText(sTime);
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
            if (timerCombatUI.activeSelf)
            {
                hitsTimerCombatProgressBar.gameObject.SetActive(false);
                hitsTimerCombatText.SetActive(false);
            }
            else if (bossCombatUI.activeSelf)
            {
                hitsBossCombatProgressBar.gameObject.SetActive(false);
                hitsBossCombatText.SetActive(false);
            }
            multiplierActive = false;

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

    public void AddKillScore(float fscore = 0, string type = "Melee")
    {
        AddScore(fscore, $"{type} Killed", comboMp);
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

    public List<Score> GetScores()
    {
        return scores.OrderBy(o => o.multiplier).ToList(); ;
    }
    #endregion

    enum CombatState
    {
        timerCombat,
        bossCombat,
        combatFinished,
        pause
    }
}
