using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawAttackCast : MonoBehaviour
{
    public LayerMask ignoreLayers;
    public Color color;

    public void UpdateDirection(Vector2 position, Vector2 direction, LineRenderer attackLine, float percentage = 1)
    {
        RaycastHit2D hit = Physics2D.Raycast(position, direction, Mathf.Infinity, ~ignoreLayers);

        Color aux = new Color(color.r, color.g, color.b, 255 / 255 * percentage);

        attackLine.endColor = aux;
        attackLine.startColor = aux;
        attackLine.SetPosition(0, position);
        attackLine.SetPosition(1, hit.point);
    }
}
