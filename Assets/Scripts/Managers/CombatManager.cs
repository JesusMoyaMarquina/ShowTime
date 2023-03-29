using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{

    public GameObject progressBar;
    public TextMeshProUGUI timerSpeedText;

    public UnitManager unitManager;

    public int startGenerateUnit;
    public int unitIncremental;
    public int secondsToGenerate;

    public float combatTime;

    private int generateIteration;

    private float actualTime;
    private float previousTimes;
    private float previousTimesNonMultiplied;
    private float remainingTime;
    private float beginBattleTime;
    private float timerSpeed;

    private void Start()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        RestartGameTimer();
        generateIteration = 0;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        if(state == GameState.Combat)
        {
            CutPreviousTimer();
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
            Debug.Log(secondsToGenerate * generateIteration);
            Debug.Log(combatTime - secondsToGenerate * generateIteration);
        }
    }

    private void GenerateUnits()
    {
        unitManager.GenerateUnits(startGenerateUnit + unitIncremental * generateIteration);
        generateIteration++;
    }

    private void RestartGameTimer()
    {
        progressBar.GetComponent<ProgressBar>().maximum = combatTime;
        timerSpeed = 1;

        previousTimes = 0;
        previousTimesNonMultiplied = 0;

        beginBattleTime = Time.time;
    }

    public void MultiplyTimeSpeed(float multiplier)
    {
        CutPreviousTimer();
        timerSpeed = multiplier;
        if (multiplier > 1)
        {
            timerSpeedText.text = $"x{multiplier}";
        } else
        {
            timerSpeedText.text = "";
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


        //Format time string
        TimeSpan t = TimeSpan.FromSeconds((int)Math.Ceiling(remainingTime));
        string sTime = t.ToString(@"mm\:ss");


        //Update graphic values
        progressBar.GetComponent<ProgressBar>().current = actualTime + previousTimes;
        progressBar.GetComponent<ProgressBar>().GetCurrentFill();
        progressBar.GetComponent<ProgressBar>().SetText(sTime);

        //Check win condition
        if (remainingTime <= 0)
        {
            GameManager.Instance.UpdateGameState(GameState.Vicory);
        }
    }

    private void HandleCombatInputs()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager.Instance.UpdateGameState(GameState.Pause);
        }
        if (Input.GetKeyDown(KeyCode.L))
        {
            GameManager.Instance.UpdateGameState(GameState.Lose);
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
        if (Input.GetKeyDown(KeyCode.R))
        {
            MultiplyTimeSpeed(1);
        }
    }
}
