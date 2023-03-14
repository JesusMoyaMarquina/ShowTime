using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject pauseMenu;
    public GameObject settingsMenu;

    // Start is called before the first frame update
    void Start()
    {
        settingsMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ReturnMainMenu()
    {
        mainMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }

    public void ReturnGame()
    {
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
    }
}
