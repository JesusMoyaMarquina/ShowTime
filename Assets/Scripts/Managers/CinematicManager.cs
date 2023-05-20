using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CinematicManager : MonoBehaviour
{
    [SerializeField, TextArea(4, 6)] private string[] cinematic1Text;
    [SerializeField] private GameObject dialogePanel;
    [SerializeField] private TMP_Text dialogeText;

    [SerializeField] private float dialogeSpeed;

    private bool dialogeStart;
    private int lineIndex;

    [HideInInspector]public bool inCinematic;

    public GameObject playerCim, robotCim, cinematicCanvas;

    private void Start()
    {
        dialogePanel.SetActive(false);
        playerCim.SetActive(false);
        robotCim.SetActive(false);
        cinematicCanvas.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.A))
            inCinematic = true;
        if (Input.GetKey(KeyCode.S))
            inCinematic = false;

        if (Input.GetButtonDown("Fire1") && inCinematic)
        {
            cinematicCanvas.SetActive(true);
            playerCim.SetActive(true);
            robotCim.SetActive(true);

            if (!dialogeStart)
            {
                StartDialoge();
            }
            else if (dialogeText.text == cinematic1Text[lineIndex])
            {
                NextDialogeLine();
            }
            else 
            { 
                StopCoroutine(dialogeText.text);
                dialogeText.text = cinematic1Text[lineIndex];
            }
        }
    }

    private void StartDialoge()
    {
        dialogeStart = true;
        dialogePanel.SetActive(true);
        lineIndex = 0;
        Time.timeScale = 0f;
        StartCoroutine(ShowLine());
    }

    private void NextDialogeLine()
    {
        lineIndex++;
        if (lineIndex < cinematic1Text.Length)
        {
            StartCoroutine(ShowLine());
        }
        else
        {
            dialogeStart = false;
            dialogePanel.SetActive(false);
            playerCim.SetActive(false);
            robotCim.SetActive(false);
            cinematicCanvas.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    private IEnumerator ShowLine()
    {
        dialogeText.text = string.Empty;

        foreach(char ch in cinematic1Text[lineIndex]) 
        { 
            dialogeText.text += ch;
            yield return new WaitForSecondsRealtime(dialogeSpeed);
        }
    }
}
