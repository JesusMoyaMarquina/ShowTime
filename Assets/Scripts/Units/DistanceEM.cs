using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class DistanceEM : EnemyMovement
{
    public float maxDistance;
    private bool inMaxRange;
    private bool inMinRange;

    public override void Tracking()
    {
        inMinRange = distance > minDistance;
        inMaxRange = distance <= maxDistance;

        if (!inMaxRange)
            inMovementRange = true;
        else if (!inMinRange)
            inMovementRange = false;

        Translation();
        SetAnimation();
    }

    public override void Attacking()
    {
        GameObject attack = Instantiate(attackObject, new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);
        attack.GetComponent<DistanceAttack>().Launch(nearPlayer);
    }
}
