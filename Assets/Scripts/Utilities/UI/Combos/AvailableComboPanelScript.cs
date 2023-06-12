using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class AvailableComboPanelScript : MonoBehaviour
{
    public GameObject comboListUI;

    [SerializeField] List<ComboListObjectScript> combos;
    List<ComboListObjectScript> availableCombos;

    private void Start()
    {
        availableCombos = new List<ComboListObjectScript>();
    }

    public void ComboListUpdate(Queue<string> comboList, bool isInCD = false)
    {
        foreach (ComboListObjectScript combo in availableCombos)
        {
            combo.gameObject.SetActive(false);
        }

        if (isInCD) return;

        availableCombos.Clear();

        string comboString = string.Join("-", comboList).ToLower();

        foreach (ComboListObjectScript combo in combos)
        {
            if (combo.GetComboID().StartsWith(comboString))
            {
                availableCombos.Add(combo);
                combo.gameObject.SetActive(true);

                float yPosition = comboListUI.transform.localPosition.y - combo.transform.GetComponent<RectTransform>().sizeDelta.y * (availableCombos.Count - 1) - combo.transform.GetComponent<RectTransform>().sizeDelta.y / 2;
                combo.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, yPosition, 0);
            }
        }
    }
}
