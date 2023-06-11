using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboListObjectScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI comboText;

    [SerializeField] private string comboID;

    public string GetComboID()
    {
        return comboID;
    }
}
