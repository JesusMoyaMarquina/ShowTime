using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class MeleEM : EnemyMovement
{
    private GameObject sharp;

    public override void Tracking()
    {
        if (stunned)
        {
            rb.mass = 1;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            return;
        }

        if (TrainManagerScript.Instance == null || TrainManagerScript.Instance.attackingTrain)
        {
            inMovementRange = distance > minDistance;
        }
        else if (TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain)
        {
            inMovementRange = false;
        }
        Translation();
        SetAnimation();
    }

    public override void Attacking()
    {
        sharp = GetComponent<ObjectPool>().GetPooledObject();
        
        if (sharp != null)
        {
            sharp.transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            sharp.SetActive(true);
            sharp.GetComponent<MeleAttack>().Launch(nearPlayer, minDistance, SetDirection());
        }
    }

    public override void AddScore()
    {
        CombatManager.instance.AddKillScore(score, "Melee");
    }

    public override void SetAttackingFalse()
    {
        lastAttack = Time.time;
        attacking = false;
        if (sharp != null) sharp.GetComponent<MeleAttack>().DestroyObject();
        anim.SetBool("attacking", attacking);
    }
}
