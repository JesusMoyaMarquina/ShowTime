using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossEM : EnemyMovement
{
    protected GameObject bossHealthBar;

    private GameObject sharp;
    protected DrawAttackCast dashCast;
    public AudioClip bossAttack;
    public LineRenderer attackLine;

    public float dashPreparationTime, dashCoolDown, minimumDashDistance, dashSpeed, dashStunTime, dashDamage, dashKnockbackForce, dashChargeTime;
    protected bool isDash;
    protected bool isDashing;
    protected bool isDashInCooldown;

    private void Start()
    {
        dashCast = GetComponent<DrawAttackCast>();
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        knockbacked = false;
        alive = true;
        dashCoolDown = 10f;
        attackCooldown = 3f;
        currentHealth = totalHealth;

        isDash = false;
        isDashing = false;
        isDashInCooldown = false;
        bossHealthBar = GameObject.Find("BossProgressBarHealth");
        bossHealthBar.GetComponent<EntityProgressBar>().maximum = totalHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().previousCurrent = currentHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }

    override
    public void GetDamage(float damage)
    {
        currentHealth -= damage;

        bossHealthBar.GetComponent<EntityProgressBar>().maximum = totalHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        bossHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();

        CheckDeadCondition();

        stunned = false;

        hitted = false;
        anim.SetBool("hitted", hitted);

        if (!attacking || isDashing)
            hitted = true;
        anim.SetBool("hitted", hitted);
    }

    public override void AddScore()
    {
        attackLine.enabled = false;
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

    public override void Movement()
    {
        if(!alive) return;

        if (isDashing || stunned)
        {
            Translation();
        }
        else
        {
            base.Movement();
        }
    }

    public override void Translation()
    {
        if(stunned)
        {
            return;
        }

        if (!isDash && !isDashing && !isDashInCooldown && distance <= minimumDashDistance)
        {
            StartCoroutine(DashPreparation());
        }

        if (isDash)
        {
            float percentage = (Time.time - dashChargeTime - dashPreparationTime) / dashPreparationTime + 1;
            dashCast.UpdateDirection(new Vector2(transform.position.x, transform.position.y), direction, attackLine, percentage);
            SetAttackingFalse();
        } else if (isDashing)
        {
            dashCast.UpdateDirection(new Vector2(transform.position.x, transform.position.y), direction, attackLine);
            rb.velocity = direction.normalized * dashSpeed;
            SetAttackingFalse();
        }
        else
        {
            base.Translation();
        }
    }

    IEnumerator DashPreparation()
    {
        dashChargeTime = Time.time;
        attackLine.enabled = true;
        isDash = true;
        yield return new WaitForSeconds(dashPreparationTime);
        isDash = false;
        BossDash();
    }

    private void BossDash()
    {
        isDashing = true;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        anim.SetBool("isDashing", isDashing);
    }

    private void StopDashing()
    {
        attackLine.enabled = false;
        isDashing = false;
        anim.SetBool("isDashing", isDashing);
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        isDashInCooldown = true;
        yield return new WaitForSeconds(dashCoolDown);
        isDashInCooldown = false;
    }

    public override void Tracking()
    {
        inMovementRange = distance > minDistance;
        Translation();
        SetAnimation();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!isDashing) return;

        rb.velocity = Vector2.zero;
        StopDashing();

        if (collision.collider.CompareTag("Map"))
        {
            StartCoroutine(StunCoroutine(dashStunTime));
        } else if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            player.StopDashing();
            player.GetDamage(dashDamage, direction, dashStunTime, true, dashKnockbackForce);
        }
    }

    IEnumerator StunCoroutine(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
    }

}
