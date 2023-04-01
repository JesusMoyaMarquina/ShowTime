using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class EnemyMovement : MonoBehaviour
{

    //General variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    //Multiplayer variables
    private PhotonView photonView;

    //Movement variables
    public float speed;
    private Vector2 direction;
    private Vector2 movement;
    private bool isDash;
    private bool isDashing;
    private bool isDashInCooldown;

    public float minDistance;

    private GameObject[] players;
    private GameObject nearPlayer;
    private float distance;
    private Vector3 enemyPos;

    //Stats variables
    public float health;
    private bool hitted;
    private bool alive;

    //Mocked basic attack variables
    public float damage;
    private bool attacking;

    void Start()
    {
        gameObject.transform.parent = GameObject.Find("Units").transform;

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        alive = true;

        photonView = GetComponent<PhotonView>();
    }

    void Update()
    {
        if (GameManager.Instance.state == GameState.Combat || GameManager.Instance.state == GameState.Pause)
        {
            Movement();
        }
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

    public void Tracking()
    {
        if (!alive || hitted)
        {
            return;
        }

        if (distance > minDistance)
        {
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            transform.position += speed * Time.deltaTime * transform.forward;
            transform.rotation = Quaternion.identity;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
            SetAnimation(true);
        }
        else
        {
            SetAnimation(false);
        }
    }

    public void SetAnimation(bool isMoving)
    {
        bool isDown = false;
        bool isUp = false;
        bool isSide = false;

        anim.SetBool("isMoving", isMoving);
        if (!isMoving) return;

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
