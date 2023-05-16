using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class EnemyMovement : MonoBehaviour
{

    //General variables
    protected Rigidbody2D rb;
    private CircleCollider2D cc;
    protected Animator anim;
    private SpriteRenderer spriteRenderer;
    private CombatManager CombatMng;

    //Movement variables
    public float speed;
    protected Vector2 direction;
    private Vector2 movement;
    private bool isDash;
    private bool isDashing;
    private bool isDashInCooldown;

    private Vector2 position;
    public float minDistance;
    protected bool inMovementRange;

    private GameObject[] players;
    protected GameObject nearPlayer;
    protected float distance;
    private Vector3 enemyPos;

    //Stats variables
    public bool hitted;
    public float totalHealth;
    private float currentHealth;
    private bool alive;

    //Mocked basic attack variables
    protected bool attacking;
    protected float lastAttack;
    private float attackCooldown;

    public float distanceToPlayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        alive = true;
        attackCooldown = 3f;
        currentHealth = totalHealth;
    }

    void Update()
    {
        Movement();
    }

    public void Movement()
    {
        if (!alive) return;

        nearPlayer = FindNearPlayer();

        distance = Vector3.Distance(enemyPos, nearPlayer.transform.position);
        direction = new Vector2(nearPlayer.transform.position.x - enemyPos.x, nearPlayer.transform.position.y - enemyPos.y);

        Tracking();
    }

    public GameObject FindNearPlayer()
    {
        GameObject auxPlayer = players[0];
        enemyPos = transform.position;

        foreach (var player in players)
        {
            if (Vector3.Distance(enemyPos, player.transform.position) < Vector3.Distance(enemyPos, auxPlayer.transform.position))
                auxPlayer = player;
        }

        distanceToPlayer = Vector3.Distance(enemyPos, auxPlayer.transform.position);

        return auxPlayer;
    }



    public abstract void Tracking();
    public abstract void Attacking();

    public void Translation()
    {
        if (inMovementRange)
        {
            if (hitted || attacking) return;
            Vector2 direction = (nearPlayer.transform.position - transform.position).normalized;
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.identity;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
        else
        {
            position = rb.position;
            rb.velocity = Vector2.zero;
        }
    }

    public char SetDirection(bool withFourAngles = false)
    {
        char look;

        if (withFourAngles)
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                if (direction.x > 0)
                    look = 'L';
                else
                    look = 'R';
            else
                if (direction.y >= 0)
                    look = 'U';
                else
                    look = 'D';
        else
            if (direction.x > 0)
                look = 'L';
            else
                look = 'R';

        return look;
    }

    public void SetAnimation()
    {
        if ((attacking || hitted) && Vector2.Distance(rb.position, position) > 0.5f)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        switch (SetDirection())
        {
            case 'L':
                spriteRenderer.flipX = true;
                break;
            case 'R':
                spriteRenderer.flipX = false;
                break;
        }

        if (inMovementRange)
        {
            rb.mass = 1;
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        }
        else
        {
            rb.mass = 0.025f;

            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
            if (!hitted && (!attacking && Time.time > lastAttack + attackCooldown))
            {
                attacking = true;
                Attacking();
            }
        }

        anim.SetBool("attacking", attacking);
        anim.SetBool("isMoving", inMovementRange);
    }

    #region stats functions
    public void GetDamage(float damage)
    {
        if (isDashing)
        {
            return;
        }

        currentHealth -= damage;

        CheckDeadCondition();

        hitted = false;
        anim.SetBool("hitted", hitted);

        if (!attacking)
            hitted = true;
            anim.SetBool("hitted", hitted);
    }

    public bool isAlive()
    {
        return alive;
    }

    public void Die()
    {
        currentHealth = 0;
        CheckDeadCondition();
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void SetHittedFalse()
    {
        hitted = false;
        anim.SetBool("hitted", hitted);
    }

    public abstract void SetAttackingFalse();
    public abstract void AddScore();

    private void CheckDeadCondition()
    {

        if (currentHealth <= 0)
        {
            cc.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            currentHealth = 0;
            alive = false;
            anim.SetBool("alive", alive);
            AddScore();
        }
    }
    #endregion
}
