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
        if (!alive || hitted)
        {
            return;
        }

        if (distance > minDistance)
        {
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            transform.position += speed * Time.deltaTime * transform.forward;
            transform.rotation = Quaternion.identity;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
            SetAnimation(true);
        }
        else
        {
            SetAnimation(false);
        }
    }
}
