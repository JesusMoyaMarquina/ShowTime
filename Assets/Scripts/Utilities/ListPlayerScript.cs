using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ListPlayerScript : MonoBehaviour
{
    public TextMeshProUGUI playerNameTMP;

    public void Inicialize(string name, bool master)
    {
        if (master)
        {
            playerNameTMP.text = $"{name} (Host)";
        }
        else
        {
            playerNameTMP.text = name;
        }
    }
}
