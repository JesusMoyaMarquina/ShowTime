using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Attack
{
    [SerializeField] string attackName;
    [SerializeField] float damage;
    [SerializeField] float cd;
    [SerializeField] GameObject collider;

    public string GetAttackName()
    {
        return attackName;
    }
    public float GetDamage()
    {
        return damage;
    }
    public float GetCD()
    {
        return cd;
    }

    public void ActivateCollider()
    {
        if (!collider.activeSelf)
        {
            collider.SetActive(true);
            if (collider.GetComponent<BoxCollider2D>() != null)
            {
                collider.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (collider.GetComponent<CircleCollider2D>() != null)
            {
                collider.GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }

    public void DeactivateCollider()
    {
        if (collider.activeSelf)
        {
            if (collider.GetComponent<BoxCollider2D>() != null)
            {
                collider.GetComponent<BoxCollider2D>().enabled = true;
            }
            else if (collider.GetComponent<CircleCollider2D>() != null)
            {
                collider.GetComponent<CircleCollider2D>().enabled = true;
            }
            collider.SetActive(false);
        }
    }
}