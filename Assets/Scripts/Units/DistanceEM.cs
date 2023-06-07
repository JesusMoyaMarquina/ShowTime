using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceEM : EnemyMovement
{
    public AudioClip throwSound;

    public float maxDistance;
    private bool inMaxRange;
    private bool inMinRange;
    private GameObject arrow;

    public override void Tracking()
    {
        if (stunned)
        {
            rb.mass = 1;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        inMinRange = distance > minDistance;
        inMaxRange = distance <= maxDistance;

        if (!inMaxRange && (TrainManagerScript.Instance == null || TrainManagerScript.Instance.attackingTrain))
            inMovementRange = true;
        else if (!inMinRange && (TrainManagerScript.Instance == null || TrainManagerScript.Instance.attackingTrain))
            inMovementRange = false;
        else if (TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain)
        {
            inMovementRange = false;
        }

        Translation();
        SetAnimation();
    }

    public override void Attacking()
    {
        arrow = GetComponent<ObjectPool>().GetPooledObject();

        if (arrow != null)
        {
            arrow.transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            arrow.SetActive(true);
            arrow.GetComponent<DistanceAttack>().Launch(nearPlayer);
        }
    }

    public override void AddScore()
    {
        CombatManager.instance.AddKillScore(score, "Ranged");
    }

    public override void SetAttackingFalse()
    {
        lastAttack = Time.time;
        attacking = false;
        anim.SetBool("attacking", attacking);
    }

    public void PlayThrowFX()
    {
        audioSource.PlayOneShot(throwSound);
    }
}
