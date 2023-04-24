using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MeleEM : EnemyMovement
{
    public override void Tracking()
    {
        inMovementRange = distance > minDistance;
        Translation();
        SetAnimation();
    }

    public override void Attacking()
    {
        GameObject attack = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/AttackTriangle"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);

        if (direction.x > 0)
        {
            attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
            attack.GetComponent<BasicAttack>().SetDirection("right");
        }
        else if (direction.x < 0)
        {
            attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
            attack.GetComponent<BasicAttack>().SetDirection("left");
        }
        else if (direction.y > 0)
        {
            attack.GetComponent<BasicAttack>().SetDirection("up");
        }
    }
}
