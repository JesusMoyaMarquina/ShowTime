using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class EnemyMovement : MonoBehaviour
{

    //General variables
    protected Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    //Movement variables
    public float speed;
    private Vector2 direction;
    private Vector2 movement;
    private bool isDash;
    private bool isDashing;
    private bool isDashInCooldown;

    public float minDistance;

    private GameObject[] players;
    protected GameObject nearPlayer;
    protected float distance;
    private Vector3 enemyPos;

    //Stats variables
    public float health;
    protected bool hitted;
    protected bool alive;

    //Mocked basic attack variables
    public float damage;
    private bool attacking;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        alive = true;
    }

    void Update()
    {
        Movement();
    }

    public void Movement()
    {
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

    public void SetAnimation(bool isMoving)
    {
        bool isDown = false;
        bool isUp = false;
        bool isSide = false;

        anim.SetBool("isMoving", isMoving);

        if (direction.y >= 0)
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                isSide = true;
            }
            else
            {
                isUp = true;
            }
        }
        else
        {
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
            {
                isSide = true;
            }
            else
            {
                isDown = true;
            }
        }

        spriteRenderer.flipX = direction.x > 0;

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

        hitted = false;
        anim.SetBool("hitted", hitted);

        health -= damage;

        hitted = true;
        anim.SetBool("hitted", hitted);

        if (health <= 0)
        {
            health = 0;
            alive = false;
            anim.SetBool("alive", alive);
        }
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
    #endregion
}
