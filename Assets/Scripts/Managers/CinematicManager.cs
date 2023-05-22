using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CinematicManager : MonoBehaviour
{
    [SerializeField, TextArea(4, 6)] private string[] cinematic1Text, cinematic2Text, cinematic3Text;
    [SerializeField] private GameObject dialogePanel;
    [SerializeField] private TMP_Text dialogeText;

    [SerializeField] private float dialogeSpeed;
    [SerializeField] private InstanceCinematic cinematicPlayer;

    private bool dialogeStart;
    private int lineIndex;
    private int CinematicNumber;
    private Coroutine lastShowLineCoroutine;

    public GameObject playerCim, robotCim, bossCim, cinematicCanvas;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        switch(state)
        {
            case GameState.Cinematics:
                CinematicNumber++;
                gameObject.SetActive(true);
                break;
            default:
                gameObject.SetActive(false);
                break;
        }
    }

    private void Start()
    {
        dialogePanel.SetActive(false);
        playerCim.SetActive(false);
        robotCim.SetActive(false);
        bossCim.SetActive(false);
        cinematicCanvas.SetActive(false);
        CinematicNumber = 0;
    }

    void Update()
    {
        cinematicCanvas.SetActive(true);
        switch (CinematicNumber)
        {
            case 1:
                playerCim.SetActive(true);
                robotCim.SetActive(true);

                if (!dialogeStart && cinematicPlayer.IsOnGoal())
                {
                    StartDialoge(cinematic1Text);
                }
                else if (dialogeText.text == cinematic1Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        NextDialogeLine(cinematic1Text);
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic1Text[lineIndex];
                    }
                }
                break;

            case 2:
                playerCim.SetActive(true);
                if (lineIndex == 1)
                {
                    bossCim.SetActive(true);
                }

                if (!dialogeStart)
                {
                    StartDialoge(cinematic2Text);
                }
                else if (dialogeText.text == cinematic2Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        NextDialogeLine(cinematic2Text);
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic2Text[lineIndex];
                    }
                }
                break;

            case 3:
                playerCim.SetActive(true);
                robotCim.SetActive(true);

                if (!dialogeStart)
                {
                    StartDialoge(cinematic3Text);
                }
                else if (dialogeText.text == cinematic3Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        NextDialogeLine(cinematic3Text);
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1"))
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic3Text[lineIndex];
                    }
                }
                break;
        }
    }

    private void StartDialoge(string[] cinematicText)
    {
        dialogeStart = true;
        dialogePanel.SetActive(true);
        lineIndex = 0;
        lastShowLineCoroutine = StartCoroutine(ShowLine(cinematicText));
    }

    private void NextDialogeLine(string[] cinematicText)
    {
        lineIndex++;
        if (lineIndex < cinematicText.Length)
        {
            lastShowLineCoroutine = StartCoroutine(ShowLine(cinematicText));
        }
        else
        {
            dialogeStart = false;
            dialogePanel.SetActive(false);
            playerCim.SetActive(false);
            robotCim.SetActive(false);
            bossCim.SetActive(false);
            cinematicCanvas.SetActive(false);

            switch (CinematicNumber)
            {
                case 1:
                    GameManager.Instance.UpdateGameState(GameState.Combat);
                    break;
                case 2:
                    GameManager.Instance.UpdateGameState(GameState.BossCombat);
                    break;
                case 3:
                    GameManager.Instance.UpdateGameState(GameState.Vicory);
                    break;

            }
        }
    }

    private IEnumerator ShowLine(string[] cinematicText)
    {
        dialogeText.text = string.Empty;

        foreach(char ch in cinematicText[lineIndex]) 
        { 
            dialogeText.text += ch;
            yield return new WaitForSecondsRealtime(dialogeSpeed);
        }
    }
}
