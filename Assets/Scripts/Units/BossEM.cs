using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BossEM : EnemyMovement
{
    protected GameObject bossHealthBar;

    private BoxCollider2D bc;
    private GameObject sharp;
    protected DrawAttackCast dashCast;
    public AudioClip bossAttack;
    public LineRenderer attackLine;

    public float dashPreparationTime, dashCoolDown, minimumDashDistance, dashSpeed, dashStunTime, dashDamage, dashKnockbackForce, dashChargeTime;
    protected bool isDash;
    protected bool isDashing;
    protected bool isDashInCooldown;
    private Vector2 dashAim;

    private void Start()
    {
        dashCast = GetComponent<DrawAttackCast>();
        rb = GetComponent<Rigidbody2D>();
        bc = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        knockbacked = false;
        alive = true;
        attackCooldown = 1.5f;
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
        PlayHittedFX();

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

        bool isPossibleDash = true;

        if (isDash)
        {
            isPossibleDash = CheckPosibleDash();
        }

        if (!isDashing && !isDashInCooldown && distance <= minimumDashDistance && isPossibleDash && !isDash)
        {
            StartCoroutine(DashPreparation());
        }

        if (isDash)
        {
            float percentage = (Time.time - dashChargeTime - dashPreparationTime) / dashPreparationTime + 1;
            dashAim = dashCast.UpdateDirection(new Vector2(transform.position.x, transform.position.y), direction, attackLine, percentage);
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

    private bool CheckPosibleDash()
    {
        Vector2 lowerHitPosition = new Vector2(transform.position.x, transform.position.y);
        Vector2 higherHitPosition = new Vector2(transform.position.x, transform.position.y + bc.size.y * transform.localScale.y);
        Vector2 higherHitDirection = new Vector2(direction.x, direction.y - bc.size.y * transform.localScale.y + nearPlayer.GetComponent<BoxCollider2D>().size.y * nearPlayer.transform.localScale.y - 0.15f);
        RaycastHit2D lowerHit = Physics2D.Raycast(lowerHitPosition, direction, Mathf.Infinity, ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)));
        RaycastHit2D higherHit = Physics2D.Raycast(higherHitPosition, higherHitDirection, Mathf.Infinity, ~LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)));

        if(!lowerHit.collider.CompareTag("Player") && !higherHit.collider.CompareTag("Player"))
        {
            CancelDash();
            return false;
        }
        return true;
    }

    private void CancelDash()
    {
        isDash = false;
        attackLine.enabled = false;
        StopCoroutine(DashPreparation());
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
        float collisionOffset = Mathf.Sqrt(Mathf.Pow(bc.size.x * transform.localScale.x, 2) + Mathf.Pow(bc.size.y * transform.localScale.y, 2));
        if (!isDashing || (Vector2.Distance(transform.position, dashAim) > collisionOffset && !collision.collider.CompareTag("Player"))) return;

        rb.velocity = Vector2.zero;
        StopDashing();

        if (collision.collider.CompareTag("Map"))
        {
            StartCoroutine(SetStunned(dashStunTime));
        } else if (collision.collider.CompareTag("Player"))
        {
            Player player = collision.collider.GetComponent<Player>();
            player.StopDashing();
            player.GetDamage(dashDamage, direction, dashStunTime, true, dashKnockbackForce);
        }
    }

}
