using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEM : EnemyMovement
{
    public float maxDistance;
    private bool fixedPosition = false;

    public override void Tracking()
    {
        if (!alive || hitted) return;

        if (!fixedPosition)
        {
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
                fixedPosition = true; 
                rb.velocity = Vector2.zero;
                SetAnimation(false);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
            SetAnimation(false);
            if (distance > maxDistance)
            {
                fixedPosition = false;
            }
        }
    }
}
