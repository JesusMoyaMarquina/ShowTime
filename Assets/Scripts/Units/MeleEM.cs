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
        if (!alive || hitted) return;

        if (distance > minDistance)
        {
            Vector2 direction = (nearPlayer.transform.position - transform.position).normalized;
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.identity;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
            SetAnimation(true);
        }
        else
        {
            rb.velocity = Vector2.zero;
            SetAnimation(false);
        }
    }
}
