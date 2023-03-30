using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{

    public GameObject timerProgressBar;
    public GameObject timerMultiplierProgressBar;

    public UnitManager unitManager;

    public int startGenerateUnit;
    public int unitIncremental;
    public int secondsToGenerate;
    public float multiplierTime;

    public float combatTime;

    private int generateIteration;
    private bool multiplierActive;

    private float actualTime;
    private float acumulatedMultiplierTime;
    private float previousTimes;
    private float previousTimesNonMultiplied;
    private float remainingTime;
    private float beginBattleTime;
    private float timerSpeed;

    private void Start()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        RestartGameTimer();
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        if(state == GameState.Combat)
        {
            CutPreviousTimer();
        } 
        else if (state == GameState.Pause) 
        {
            acumulatedMultiplierTime = actualTime;
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
            ManageBattleTime();
            ManageLoseCondition();
            ManageWinCondition();
        }
    }

    private void GenerateUnits()
    {
        unitManager.GenerateUnits(startGenerateUnit + unitIncremental * generateIteration);
        generateIteration++;
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
            GameManager.Instance.UpdateGameState(GameState.Vicory);
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
            timerMultiplierProgressBar.gameObject.SetActive(true);
            timerMultiplierProgressBar.GetComponent<ProgressBar>().SetText($"x{multiplier}");
            multiplierActive = true;
        } else
        {
            timerMultiplierProgressBar.gameObject.SetActive(false);
            multiplierActive = false;
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

        //Manage multiplier time
        if (multiplierActive && multiplierTime - actualTime - acumulatedMultiplierTime >= 0)
        {
            timerMultiplierProgressBar.GetComponent<ProgressBar>().maximum = multiplierTime;
            timerMultiplierProgressBar.GetComponent<ProgressBar>().current = multiplierTime - actualTime - acumulatedMultiplierTime;
            timerMultiplierProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        }
        else if (multiplierActive && multiplierTime - actualTime - acumulatedMultiplierTime < 0)
        {
            MultiplyTimeSpeed(1);
        }


        //Format time string
        TimeSpan t = TimeSpan.FromSeconds((int)Math.Ceiling(remainingTime));
        string sTime = t.ToString(@"mm\:ss");


        //Update graphic values
        timerProgressBar.GetComponent<ProgressBar>().current = combatTime - remainingTime;
        timerProgressBar.GetComponent<ProgressBar>().GetCurrentFill();
        timerProgressBar.GetComponent<ProgressBar>().SetText(sTime);
    }
    #endregion

    private void HandleCombatInputs()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.Instance.UpdateGameState(GameState.Pause);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = 0; i < GameObject.Find("Players").transform.childCount; i++)
            {
                GameObject.Find("Players").transform.GetChild(i).GetComponent<Player>().GetDamage(20);
            }
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MultiplyTimeSpeed(1.25f);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            MultiplyTimeSpeed(1.5f);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            MultiplyTimeSpeed(1.75f);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            MultiplyTimeSpeed(2);
        }
    }
}
