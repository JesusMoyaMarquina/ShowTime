using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CinematicManager : MonoBehaviour
{
    [SerializeField, TextArea(4, 6)] private string[] cinematic1Text, cinematic2Text, cinematic3Text, cinematic4Text;
    [SerializeField] private GameObject dialogePanel;
    [SerializeField] private TMP_Text dialogeText;

    [SerializeField] private float dialogeSpeed;
    [SerializeField] private InstanceCinematic cinematicPlayer;
    [SerializeField] private AudioClip AudioThunder, cinematicMusic;

    private bool dialogeStart, dialogDelay, playSound, pause;
    private int lineIndex;
    private int CinematicNumber;
    private Coroutine lastShowLineCoroutine;

    public GameObject playerCim, robotCim, bossCim, godCim, bgPanel, cinematicCanvas;

    private AudioSource audioS;

    private void Awake()
    {
        audioS = GetComponent<AudioSource>();
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
            case GameState.Pause:
                pause = true;
                audioS.Pause();
                break;
            case GameState.Cinematics:
                if (GameManager.Instance.previousGameState != GameState.Pause)  
                {
                    CinematicNumber++;
                }
                pause = false;
                audioS.Play();
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
        godCim.SetActive(false);
        bgPanel.SetActive(false);
        cinematicCanvas.SetActive(false);
        CinematicNumber = 0;
        playSound = true;
        audioS.clip = cinematicMusic;
        audioS.Play();
    }

    void Update()
    {
        if (pause) return;

        cinematicCanvas.SetActive(true);
        switch (CinematicNumber)
        {
            case 1:

                if (lineIndex == 1)
                {
                    if (playSound)
                    {
                        audioS.PlayOneShot(AudioThunder);
                        playSound = false;
                    }
                    godCim.SetActive(true);
                }

                if (!dialogeStart)
                {
                    bgPanel.SetActive(true);
                    playerCim.SetActive(true);
                    StartDialoge(cinematic1Text);
                }
                else if (dialogeText.text == cinematic1Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1") && !dialogDelay)
                    {
                        NextDialogeLine(cinematic1Text);
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1") && lastShowLineCoroutine != null && !dialogDelay)
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic1Text[lineIndex];
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                break;

            case 2:

                if (!dialogeStart && cinematicPlayer.IsOnGoal())
                {
                    playerCim.SetActive(true);
                    robotCim.SetActive(true);
                    StartDialoge(cinematic2Text);
                }
                else if (dialogeText.text == cinematic2Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1") && !dialogDelay)
                    {
                        NextDialogeLine(cinematic2Text);
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1") && lastShowLineCoroutine != null && !dialogDelay)
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic2Text[lineIndex];
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                break;

            case 3:

                playerCim.SetActive(true);
                if (lineIndex == 1)
                {
                    bossCim.SetActive(true);
                }

                if (!dialogeStart)
                {
                    StartDialoge(cinematic3Text);
                }
                else if (dialogeText.text == cinematic3Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1") && !dialogDelay)
                    {
                        NextDialogeLine(cinematic3Text);
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1") && lastShowLineCoroutine != null && !dialogDelay)
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic3Text[lineIndex];
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                break;

            case 4:

                playerCim.SetActive(true);
                robotCim.SetActive(true);

                if (!dialogeStart)
                {
                    StartDialoge(cinematic4Text);
                }
                else if (dialogeText.text == cinematic4Text[lineIndex])
                {
                    if (Input.GetButtonDown("Fire1") && !dialogDelay)
                    {
                        NextDialogeLine(cinematic4Text);
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
                    }
                }
                else
                {
                    if (Input.GetButtonDown("Fire1") && lastShowLineCoroutine != null && !dialogDelay)
                    {
                        StopCoroutine(lastShowLineCoroutine);
                        dialogeText.text = cinematic4Text[lineIndex];
                        if (gameObject.activeSelf)
                        {
                            StartCoroutine(DialogDelay());
                        }
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
            bgPanel.SetActive(false);
            playerCim.SetActive(false);
            robotCim.SetActive(false);
            bossCim.SetActive(false);
            godCim.SetActive(false);
            cinematicCanvas.SetActive(false);
            lineIndex = 0;

            switch (CinematicNumber)
            {
                case 1:
                    cinematicPlayer.Cinematic1Over();
                    GameManager.Instance.UpdateGameState(GameState.Cinematics);
                    break;

                case 2:
                    CombatManager.instance.unitManager.GeneratePlayer();
                    CombatManager.instance.generatedPlayer = true;
                    audioS.Stop();
                    GameManager.Instance.UpdateGameState(GameState.Combat);
                    break;

                case 3:
                    GameManager.Instance.UpdateGameState(GameState.BossCombat);
                    break;

                case 4:
                    GameManager.Instance.UpdateGameState(GameState.Vicory);
                    break;
            }
        }
    }

    private IEnumerator DialogDelay()
    {
        dialogDelay = true;
        yield return new WaitForEndOfFrame();
        dialogDelay = false;
    }

    private IEnumerator ShowLine(string[] cinematicText)
    {
        dialogeText.text = string.Empty;

        foreach(char ch in cinematicText[lineIndex]) 
        { 
            dialogeText.text += ch;
            yield return new WaitForSeconds(dialogeSpeed);
        }
    }
}
