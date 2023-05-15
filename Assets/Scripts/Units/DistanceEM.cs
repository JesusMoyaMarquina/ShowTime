using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEM : EnemyMovement
{
    public AudioSource audioSource;
    public AudioClip attackSound, throwSound;

    public float maxDistance;
    private bool inMaxRange;
    private bool inMinRange;
    private GameObject arrow;

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
    {}

    public void ThrowAttack()
    {
        arrow = GetComponent<ObjectPool>().GetPooledObject();

        if (arrow != null)
        {
            arrow.transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            arrow.SetActive(true);
            arrow.GetComponent<DistanceAttack>().Launch(nearPlayer);
        }
    }

    public override void SetAttackingFalse()
    {
        lastAttack = Time.time;
        attacking = false;
        anim.SetBool("attacking", attacking);
    }

    public void PlayAttackFX()
    {
        audioSource.PlayOneShot(attackSound);
    }

    public void PlayThrowFX()
    {
        audioSource.PlayOneShot(throwSound);
    }
}
