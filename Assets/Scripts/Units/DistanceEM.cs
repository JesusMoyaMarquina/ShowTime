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
        attack.GetComponent<MeleAttack>().Launch(nearPlayer);
        //if (direction.x > 0)
        //{
        //    attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
        //    attack.GetComponent<BasicAttack>().SetDirection("right");
        //}
        //else if (direction.x < 0)
        //{
        //    attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
        //    attack.GetComponent<BasicAttack>().SetDirection("left");
        //}
        //else if (direction.y > 0)
        //{
        //    attack.GetComponent<BasicAttack>().SetDirection("up");
        //}
    }
}
