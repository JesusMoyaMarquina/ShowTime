using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class CinematicManager : MonoBehaviour
{
    public static CinematicManager Instance;

    [SerializeField, TextArea(4, 6)] private string[] cinematic1Text, cinematic2Text, cinematic3Text, cinematic4Text;
    [SerializeField] private GameObject dialogePanel, fadeBlack, fadeInLight;
    [SerializeField] private TMP_Text dialogeText;

    [SerializeField] private float dialogeSpeed;
    [SerializeField] private InstanceCinematic cinematicPlayer;
    [SerializeField] private AudioClip AudioThunder, cinematicMusic;
    [SerializeField] private Animator cim1BgAnim, noahAnim, godAnim, scr3wAnim;

    private bool dialogeStart, dialogFinished, dialogDelay, playSound, pause, lastLine, fadedIn;
    private int lineIndex;
    private int CinematicNumber;
    private Coroutine lastShowLineCoroutine;

    public GameObject playerCim, robotCim, bossCim, godCim, bgPanel, cinematicCanvas;

    private AudioSource audioS;

    private void Awake()
    {
        Instance = this;
        audioS = GetComponent<AudioSource>();
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= GameManagerOnGameStateChange;
    }

    public void SetFadedInTrue()
    {
        fadedIn = true;
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
                    lastLine = false;
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
        lastLine = true;
        playSound = true;
        audioS.clip = cinematicMusic;
        audioS.Play();
    }

    void Update()
    {
        if (pause || lastLine) return;

        cinematicCanvas.SetActive(true);
        switch (CinematicNumber)
        {
            case 1:

                if (lineIndex == 1)
                {
                    if (playSound)
                    {
                        cim1BgAnim.SetTrigger("thunder");
                        audioS.PlayOneShot(AudioThunder);
                        playSound = false;
                    }
                    godCim.SetActive(true);
                }

                if (!dialogeStart && !dialogFinished)
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

                if (cinematic1Text[lineIndex].ToLower().Contains("noah:") && !(dialogeText.text == cinematic1Text[lineIndex]) && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic1Text[lineIndex].ToLower().Contains("noah:") && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", false);
                }
                else if (playerCim.activeSelf)
                {
                    noahAnim.SetBool("isSpeaking", false);
                    noahAnim.SetBool("notSpeaking", true);
                }

                if (cinematic1Text[lineIndex].ToLower().Contains("god:") && !(dialogeText.text == cinematic1Text[lineIndex]) && godCim.activeSelf)
                {
                    godAnim.SetBool("notSpeaking", false);
                    godAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic1Text[lineIndex].ToLower().Contains("god:") && godCim.activeSelf)
                {
                    godAnim.SetBool("notSpeaking", false);
                    godAnim.SetBool("isSpeaking", false);
                } else if (godCim.activeSelf)
                {
                    godAnim.SetBool("isSpeaking", false);
                    godAnim.SetBool("notSpeaking", true);
                }

                break;

            case 2:

                if (!dialogeStart && cinematicPlayer.IsOnGoal())
                {
                    playerCim.SetActive(true);
                    robotCim.SetActive(true);
                    StartDialoge(cinematic2Text);
                }
                else if (dialogeText.text == cinematic2Text[lineIndex] && cinematicPlayer.IsOnGoal())
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
                else if (cinematicPlayer.IsOnGoal())
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

                if (cinematic2Text[lineIndex].ToLower().Contains("noah:") && !(dialogeText.text == cinematic2Text[lineIndex]) && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic2Text[lineIndex].ToLower().Contains("noah:") && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", false);
                }
                else if (playerCim.activeSelf)
                {
                    noahAnim.SetBool("isSpeaking", false);
                    noahAnim.SetBool("notSpeaking", true);
                }

                if (cinematic2Text[lineIndex].ToLower().Contains("s.c.r-3w:") && !(dialogeText.text == cinematic2Text[lineIndex]) && robotCim.activeSelf)
                {
                    scr3wAnim.SetBool("notSpeaking", false);
                    scr3wAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic2Text[lineIndex].ToLower().Contains("s.c.r-3w:") && robotCim.activeSelf)
                {
                    scr3wAnim.SetBool("notSpeaking", false);
                    scr3wAnim.SetBool("isSpeaking", false);
                }
                else if (robotCim.activeSelf)
                {
                    scr3wAnim.SetBool("isSpeaking", false);
                    scr3wAnim.SetBool("notSpeaking", true);
                }

                if (fadedIn)
                {
                    cinematicPlayer.StartMoving();
                }
                else
                {
                    bgPanel.SetActive(false);
                    fadeInLight.SetActive(true);
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



                if (cinematic3Text[lineIndex].ToLower().Contains("noah:") && !(dialogeText.text == cinematic3Text[lineIndex]) && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic3Text[lineIndex].ToLower().Contains("noah:") && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", false);
                }
                else if (playerCim.activeSelf)
                {
                    noahAnim.SetBool("isSpeaking", false);
                    noahAnim.SetBool("notSpeaking", true);
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



                if (cinematic4Text[lineIndex].ToLower().Contains("noah:") && !(dialogeText.text == cinematic4Text[lineIndex]) && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic4Text[lineIndex].ToLower().Contains("noah:") && playerCim.activeSelf)
                {
                    noahAnim.SetBool("notSpeaking", false);
                    noahAnim.SetBool("isSpeaking", false);
                }
                else if (playerCim.activeSelf)
                {
                    noahAnim.SetBool("isSpeaking", false);
                    noahAnim.SetBool("notSpeaking", true);
                }

                if (cinematic4Text[lineIndex].ToLower().Contains("s.c.r-3w:") && !(dialogeText.text == cinematic4Text[lineIndex]) && robotCim.activeSelf)
                {
                    scr3wAnim.SetBool("notSpeaking", false);
                    scr3wAnim.SetBool("isSpeaking", true);
                }
                else if (cinematic4Text[lineIndex].ToLower().Contains("s.c.r-3w:") && robotCim.activeSelf)
                {
                    scr3wAnim.SetBool("notSpeaking", false);
                    scr3wAnim.SetBool("isSpeaking", false);
                }
                else if (robotCim.activeSelf)
                {
                    scr3wAnim.SetBool("isSpeaking", false);
                    scr3wAnim.SetBool("notSpeaking", true);
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
            lastLine = true;
            dialogePanel.SetActive(false);
            playerCim.SetActive(false);
            robotCim.SetActive(false);
            bossCim.SetActive(false);
            godCim.SetActive(false);
            lineIndex = 0;

            switch (CinematicNumber)
            {
                case 1:
                    dialogFinished = true;
                    fadeBlack.SetActive(true);
                    break;

                case 2:
                    CombatManager.instance.unitManager.GeneratePlayer();
                    CombatManager.instance.generatedPlayer = true;
                    audioS.Stop();
                    cinematicCanvas.SetActive(false);
                    GameManager.Instance.UpdateGameState(GameState.Combat);
                    break;

                case 3:
                    cinematicCanvas.SetActive(false);
                    GameManager.Instance.UpdateGameState(GameState.BossCombat);
                    break;

                case 4:
                    cinematicCanvas.SetActive(false);
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
