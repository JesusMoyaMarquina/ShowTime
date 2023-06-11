using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class KeybindingMenuScript : MonoBehaviour
{
    public GameObject settingsMenu;
    public InputActionAsset inputActions;

    // Update is called once per frame
    void Update()
    {
        HandleKeybindingMenuInputs();
    }

    public void OpenKeybinds()
    {
        settingsMenu?.SetActive(false);
        gameObject?.SetActive(true);
    }

    public void KeybindsReturn()
    {
        settingsMenu?.SetActive(true);
        gameObject?.SetActive(false);
    }

    public void ResetAllBindings()
    {
        foreach (InputActionMap map in inputActions.actionMaps)
        {
            map.RemoveAllBindingOverrides();
        }
        PlayerPrefs.DeleteKey("rebinds");
    }

    private void HandleKeybindingMenuInputs()
    {
        if (gameObject != null && Input.GetButtonDown("Cancel") && gameObject.activeSelf)
        {
            KeybindsReturn();
        }
    }
}
