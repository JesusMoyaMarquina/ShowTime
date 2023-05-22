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
    public LayerMask ignoreLayers;
    protected bool isDash;
    protected bool isDashing;
    protected bool isDashInCooldown;
    private Vector2 dashAim;
    private Coroutine lastPreparateDashCoroutine;

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

        if (!attacking && !isDashing && !isDash)
        {
            hitted = true;
            anim.SetBool("hitted", hitted);
        }
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
            lastPreparateDashCoroutine = StartCoroutine(DashPreparation());
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
        Vector2 rLowerHitPosition = new Vector2(transform.position.x + (bc.size.x + bc.offset.x), 
                                                transform.position.y - (bc.size.y + bc.offset.y));
        Vector2 lLowerHitPosition = new Vector2(transform.position.x - (bc.size.x + bc.offset.x), 
                                                transform.position.y - (bc.size.y + bc.offset.y));
        Vector2 rHigherHitPosition = new Vector2(transform.position.x + (bc.size.x + bc.offset.x), 
                                                 transform.position.y + (bc.size.y + bc.offset.y));
        Vector2 lHigherHitPosition = new Vector2(transform.position.x - (bc.size.x + bc.offset.x), 
                                                 transform.position.y + (bc.size.y + bc.offset.y));

        BoxCollider2D pbc = nearPlayer.GetComponent<BoxCollider2D>();

        Vector2 rLowerPlayerHitPosition = new Vector2(nearPlayer.transform.position.x - 0.45f + (pbc.size.x + pbc.offset.x),
                                                      nearPlayer.transform.position.y + 0.45f - (pbc.size.y + pbc.offset.y));
        Vector2 lLowerPlayerHitPosition = new Vector2(nearPlayer.transform.position.x + 0.45f - (pbc.size.x + pbc.offset.x),
                                                      nearPlayer.transform.position.y + 0.45f - (pbc.size.y + pbc.offset.y));
        Vector2 rHigherPlayerHitPosition = new Vector2(nearPlayer.transform.position.x - 0.45f + (pbc.size.x + pbc.offset.x), 
                                                       nearPlayer.transform.position.y - 0.45f + (pbc.size.y + pbc.offset.y));
        Vector2 lHigherPlayerHitPosition = new Vector2(nearPlayer.transform.position.x + 0.45f - (pbc.size.x + pbc.offset.x),
                                                       nearPlayer.transform.position.y - 0.45f + (pbc.size.y + pbc.offset.y));

        Vector2 rHigherHitDirection = new Vector2(rHigherPlayerHitPosition.x - rHigherHitPosition.x,
                                                  rHigherPlayerHitPosition.y - rHigherHitPosition.y);
        Vector2 lHigherHitDirection = new Vector2(lHigherPlayerHitPosition.x - lHigherHitPosition.x,
                                                  lHigherPlayerHitPosition.y - lHigherHitPosition.y);
        Vector2 rLowerHitDirection = new Vector2(rLowerPlayerHitPosition.x - rLowerHitPosition.x,
                                                 rLowerPlayerHitPosition.y - rLowerHitPosition.y);
        Vector2 lLowerHitDirection = new Vector2(lLowerPlayerHitPosition.x - lLowerHitPosition.x,
                                                 lLowerPlayerHitPosition.y - lLowerHitPosition.y);

        RaycastHit2D rLowerHit = Physics2D.Raycast(rLowerHitPosition, rLowerHitDirection, Mathf.Infinity, ~ignoreLayers);
        RaycastHit2D lLowerHit = Physics2D.Raycast(lLowerHitPosition, lLowerHitDirection, Mathf.Infinity, ~ignoreLayers);
        RaycastHit2D rHigherHit = Physics2D.Raycast(rHigherHitPosition, rHigherHitDirection, Mathf.Infinity, ~ignoreLayers);
        RaycastHit2D lHigherHit = Physics2D.Raycast(lHigherHitPosition, lHigherHitDirection, Mathf.Infinity, ~ignoreLayers);

        print("a");
        Debug.DrawLine(rLowerHitPosition, rLowerHit.point);
        Debug.DrawLine(lLowerHitPosition, lLowerHit.point);
        Debug.DrawLine(rHigherHitPosition, rHigherHit.point);
        Debug.DrawLine(lHigherHitPosition, lHigherHit.point);

        if (!rLowerHit.collider.CompareTag("Player") || !lLowerHit.collider.CompareTag("Player") 
            || !rHigherHit.collider.CompareTag("Player") || !lHigherHit.collider.CompareTag("Player"))
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
        StopCoroutine(lastPreparateDashCoroutine);
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
