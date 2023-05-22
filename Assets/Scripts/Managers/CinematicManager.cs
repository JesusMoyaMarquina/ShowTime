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

    private bool dialogeStart;
    private int lineIndex;
    private int CinematicNumber;

    public GameObject playerCim, robotCim, bigEnemyCim, bossCim, cinematicCanvas;

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
        bigEnemyCim.SetActive(false);
        bossCim.SetActive(false);
        cinematicCanvas.SetActive(false);
        CinematicNumber = 0;
    }

    void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            cinematicCanvas.SetActive(true);
            switch (CinematicNumber)
            {
                case 1:
                    playerCim.SetActive(true);
                    robotCim.SetActive(true);

                    if (!dialogeStart)
                    {
                        StartDialoge(cinematic1Text);
                    }
                    else if (dialogeText.text == cinematic1Text[lineIndex])
                    {
                        NextDialogeLine(cinematic1Text);
                    }
                    else
                    {
                        StopCoroutine(dialogeText.text);
                        dialogeText.text = cinematic1Text[lineIndex];
                    }
                    break;

                case 2:
                    playerCim.SetActive(true);
                    if (lineIndex == 0)
                    {
                        bigEnemyCim.SetActive(true);
                    }

                    if (!dialogeStart)
                    {
                        StartDialoge(cinematic2Text);
                    }
                    else if (dialogeText.text == cinematic2Text[lineIndex])
                    {
                        NextDialogeLine(cinematic2Text);
                    }
                    else
                    {
                        StopCoroutine(dialogeText.text);
                        dialogeText.text = cinematic2Text[lineIndex];
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
                        NextDialogeLine(cinematic3Text);
                    }
                    else
                    {
                        StopCoroutine(dialogeText.text);
                        dialogeText.text = cinematic3Text[lineIndex];
                    }
                    break;
            }
        }
    }

    private void StartDialoge(string[] cinematicText)
    {
        dialogeStart = true;
        dialogePanel.SetActive(true);
        lineIndex = 0;
        StartCoroutine(ShowLine(cinematicText));
    }

    private void NextDialogeLine(string[] cinematicText)
    {
        lineIndex++;
        if (lineIndex < cinematicText.Length)
        {
            StartCoroutine(ShowLine(cinematicText));
        }
        else
        {
            dialogeStart = false;
            dialogePanel.SetActive(false);
            playerCim.SetActive(false);
            robotCim.SetActive(false);
            bigEnemyCim.SetActive(false);
            bossCim.SetActive(false);
            cinematicCanvas.SetActive(false);

            switch (CinematicNumber)
            {
                case 1:
                    GameManager.Instance.UpdateGameState(GameState.Combat);
                    break;
                case 2:
                    GameManager.Instance.UpdateGameState(GameState.Combat);
                    break;
                case 3:
                    GameManager.Instance.UpdateGameState(GameState.CombatFinished);
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
