using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    public GameObject timerProgressBar, timerMultiplierProgressBar, hitsText, winZone;

    public UnitManager unitManager;

    public int startGenerateUnit, unitIncremental, secondsToGenerate;

    public float multiplierTime, combatTime;

    public int comboToMultiply;

    public float timeMultiplier;

    private int generateIteration;

    private bool multiplierActive, timerPause;

    private float actualTime, acumulatedMultiplierTime, previousTimes, previousTimesNonMultiplied, remainingTime, beginBattleTime, timerSpeed, comboMp = 1;

    protected int cSHelp = 0;

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
            case GameState.Pause:
                acumulatedMultiplierTime = actualTime;
                break;
            case GameState.Vicory:
                break;
        }
    }

    void Update()
    {
        if (GameManager.Instance.state == GameState.Combat)
        {
            HandleCombatInputs();
            if ((int)(Math.Ceiling(remainingTime)) == combatTime - secondsToGenerate * generateIteration || remainingTime == combatTime)
            {
                GenerateUnits();
            }
        }
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

    void KillAllEnemies()
    {
        foreach(EnemyMovement enemy in FindObjectsOfType<EnemyMovement>())
        {
            enemy.Die();
        }
    }

    void OpenWinZone()
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
        if (multiplierActive && multiplierTime - actualTime - acumulatedMultiplierTime >= 0)
        {
            timerMultiplierProgressBar.GetComponent<ProgressBar>().maximum = multiplierTime;
            timerMultiplierProgressBar.GetComponent<ProgressBar>().current = multiplierTime - actualTime - acumulatedMultiplierTime;
            timerMultiplierProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        }
        else if (multiplierActive && multiplierTime - actualTime - acumulatedMultiplierTime < 0)
        {
            ComboSistem(false);
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
        ComboSistem(false);
    }
    #endregion

    #region Sistema de combos
    public void ComboSistem(bool hit)
    {
        if (hit)
        {
            if ((cSHelp + 1) % comboToMultiply == 0)
            {
                cSHelp++;
                comboMp += timeMultiplier / 100;
                MultiplyTimeSpeed(comboMp);
            }
            else
            {
                cSHelp++;
            }
        }

        if (!hit)
        {
            timerMultiplierProgressBar.gameObject.SetActive(false);
            multiplierActive = false;
            cSHelp = 0;
            comboMp = 1f;
            hitsText.SetActive(false);
            MultiplyTimeSpeed(1);
        }
    }
    #endregion
}
