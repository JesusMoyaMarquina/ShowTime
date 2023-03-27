using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public float timerSpeed;

    public GameObject progressBar;
    public float combatTime;
    public float actualTime;
    public float previousTimes;
    public float previousTimesNonMultiplied;
    public float remainingTime;
    public float extraTime;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        progressBar.GetComponent<ProgressBar>().maximum = combatTime;
        timerSpeed = 1;
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
        }
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.state == GameState.Combat)
        {
            ManageBattleTime();
        }
    }

    public void MultiplyTimeSpeed(float multiplier)
    {
        CutPreviousTimer();
        timerSpeed = multiplier;
    }

    private void CutPreviousTimer()
    {
        previousTimes += actualTime;
        previousTimesNonMultiplied += actualTime / timerSpeed;
    }

    private void ManageBattleTime()
    {
        //Update battle time
        actualTime = (Time.time - previousTimesNonMultiplied) * timerSpeed;
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
