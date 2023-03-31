using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ServerScript : MonoBehaviour
{
    public TextMeshProUGUI roomNameTMP;
    public TextMeshProUGUI currenPlayersTMP;
    public Color selectedColor;

    public void Inicialize(string name, string playersMin, string playersMax)
    {
        roomNameTMP.text = name;
        currenPlayersTMP.text = $"{playersMin}/{playersMax}";
    }

    public void SetSelectedRoom()
    {
        GameObject.Find("MenuManager").GetComponent<MenuManager>().SetSelectedRoom(gameObject);
    }

    public void IsSelected(bool selected)
    {
        if(selected)
        {
            GetComponent<Image>().color = new Color(selectedColor.r, selectedColor.g, selectedColor.b);
        } 
        else
        {
            GetComponent<Image>().color = Color.white;
        }
    }
}
