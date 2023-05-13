using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonHoverScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    Animator animator;
    Button button;

    private void Start()
    {
        animator = GetComponent<Animator>();
        button = GetComponent<Button>();
        animator.updateMode = AnimatorUpdateMode.UnscaledTime;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
        {
            animator.SetBool("isHover", true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
        {
            animator.SetBool("isHover", false);
        }
    }
}
