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
    public float health;
    private bool hitted;
    private bool alive;

    //Mocked basic attack variables
    public float damage;
    protected bool attacking;
    protected float lastAttack;
    private float attackCooldown;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        alive = true;
        attackCooldown = 3f;
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

    public char SetDirection()
    {
        char look;

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

        return look;
    }

    public void SetAnimation()
    {
        if ((attacking || hitted) && Vector2.Distance(rb.position, position) > 0.5f)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        bool isDown = false;
        bool isUp = false;
        bool isSide = false;

        switch (SetDirection())
        {
            case 'L':
                spriteRenderer.flipX = true;
                isSide = true;
                break;
            case 'R':
                spriteRenderer.flipX = false;
                isSide = true;
                break;
            case 'U':
                isUp = true;
                break;
            case 'D':
                isDown = true;
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

            if (isUp || isDown) 
                rb.constraints = RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
            else if (isSide)
                rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
            if (!hitted && (!attacking && Time.time > lastAttack + attackCooldown))
            {
                attacking = true;
                Attacking();
            }
        }

        anim.SetBool("attacking", attacking);
        anim.SetBool("isMoving", inMovementRange);
        anim.SetBool("isUp", isUp);
        anim.SetBool("isSide", isSide);
        anim.SetBool("isDown", isDown);
    }

    #region stats functions
    public void GetDamage(float damage)
    {
        if (isDashing)
        {
            return;
        }
        
        health -= damage;

        if (health <= 0)
        {
            cc.enabled = false;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            health = 0;
            alive = false;
            anim.SetBool("alive", alive);
        }
        
        hitted = false;
        anim.SetBool("hitted", hitted);

        if (!attacking)
            hitted = true;
            anim.SetBool("hitted", hitted);
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
    #endregion
}
