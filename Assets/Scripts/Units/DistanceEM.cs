using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class DistanceEM : EnemyMovement
{
    public float maxDistance;
    private bool fixedPosition = false;
    
    public override void Tracking()
    {
        if (!alive || hitted)
        {
            return;
        }

        if (!fixedPosition)
        {
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
                fixedPosition = true;
                SetAnimation(false);
            }
        } else if (distance > maxDistance)
        {
            fixedPosition = false;
        }
    }
}
