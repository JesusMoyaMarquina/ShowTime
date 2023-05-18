using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEM : EnemyMovement
{
    protected GameObject bossHealthBar;

    private GameObject sharp;
    public AudioClip bossAttack;

    protected bool isDash;
    protected bool isDashing;
    protected bool isDashInCooldown;

    override
    public void GetDamage(float damage)
    {
        currentHealth -= damage;

        bossHealthBar.GetComponent<EntityProgressBar>().maximum = totalHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();

        CheckDeadCondition();

        hitted = false;
        anim.SetBool("hitted", hitted);

        if (!attacking || isDashing)
            hitted = true;
        anim.SetBool("hitted", hitted);
    }

    public override void AddScore()
    {
        CombatManager.instance.AddKillScore(score, "Boss");
    }

    public override void Attacking()
    {
        sharp = GetComponent<ObjectPool>().GetPooledObject();

        if (sharp != null)
        {
            sharp.transform.SetPositionAndRotation(new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity);
            sharp.SetActive(true);
            sharp.GetComponent<MeleAttack>().Launch(nearPlayer, minDistance, SetDirection(true));
        }
    }

    public override void SetAttackingFalse()
    {
        lastAttack = Time.time;
        attacking = false;
        if (sharp != null) sharp.GetComponent<MeleAttack>().DestroyObject();
        anim.SetBool("attacking", attacking);
    }

    public override void Translation()
    {
        if (isDashing || stunned)
        {
            return;
        }

        if(isDash)
        {
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            transform.rotation = Quaternion.identity;
            SetAttackingFalse();
            return;
        }

        base.Translation();
    }

    public override void Tracking()
    {
        if (stunned)
        {
            return;
        }

        inMovementRange = distance > minDistance;
        Translation();
        SetAnimation();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;

        if (collision.collider.CompareTag("Player"))
        {

        } else if (collision.collider)
        {

        }
    }
}
