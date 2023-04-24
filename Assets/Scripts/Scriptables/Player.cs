using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;

public class Player : MonoBehaviour
{

    //General variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private Fist fist;


    //Movement variables
    public float speed;
    private Vector2 direction;
    private Vector2 movement;
    private bool isDash;
    private bool isDashing;
    private bool isDashInCooldown;

    //Stats variables
    public float maxHealth;
    private bool hitted;
    private bool alive;
    private float currentHealth;

    private GameObject playerHealthBar;
    private GameObject otherPlayerHealthBar;

    //Atack Stats
    protected Vector2[] atkDist = new Vector2[2];

    protected float[] tiempoCD = new float[2];
    protected float[] damageDeal = new float[2];

    protected float atkMng = 10;

    private bool attacking;


    void Start()
    {
        //Start player basics
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        fist = GetComponent<Fist>();

        //Start direction
        direction = new Vector2(0, -1 * speed);

        //Start health
        alive = true;
        hitted = false;
        currentHealth = maxHealth;

        playerHealthBar = GameObject.Find("PlayerHealthProgressBar");
        playerHealthBar.GetComponent<EntityProgressBar>().maximum = maxHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().previousCurrent = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();

        otherPlayerHealthBar = GameObject.Find("OtherPlayerHealthBar");

        //Atack Stats
        atkDist[0] = new Vector2();
        atkDist[1] = new Vector2();

        tiempoCD[0] = 1;
        tiempoCD[1] = 1.5f;

        damageDeal[0] = 15;
        damageDeal[1] = 25;
    }

    void Update()
    {
        if (!alive || hitted || attacking)
        {
            return;
        }
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        isDash = playerInput.actions["Dash"].ReadValue<float>() == 1 ? true : false;
        if (playerInput.actions["Light Hit"].triggered)
        {

        }
    }

    private void FixedUpdate()
    {
        if (!alive || attacking)
        {
            return;
        }
        CameraFollowUp();
        if (isDash && !isDashInCooldown) PlayerDash();
        if (!isDashing)
        {
            PlayerMovement(speed);
            PlayerDirection();
        }
        else
        {
            PlayerMovement(speed + 10, true);
        }

        if (playerInput.actions["SoftHit"].triggered)
        {
            ShoftAttack();
        }

        if (playerInput.actions["StrongHit"].triggered)
        {
            StrongAttack();
        }
    }

    #region movement functions
    private void CameraFollowUp()
    {
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    private void PlayerMovement(float speed, bool isDashing = false)
    {
        Vector2 playerVelocity = isDashing ? direction / 10 : movement.normalized;

        rb.velocity = playerVelocity * speed;
    }

    private void PlayerDirection()
    {
        if (rb.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (rb.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = false;
        }
        if (rb.velocity != Vector2.zero)
        {
            direction = rb.velocity;
        }
        if (direction.x < 0.1f)
        {
            spriteRenderer.flipX = false;
        }

        anim.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("speedY", rb.velocity.y);
    }

    private void PlayerDash()
    {
        isDashing = true;
        anim.SetBool("isDashing", true);
        anim.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("speedY", rb.velocity.y);
    }

    private void StopDashing()
    {
        isDashing = false;
        anim.SetBool("isDashing", false);
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        isDashInCooldown = true;
        yield return new WaitForSeconds(2);
        isDashInCooldown = false;
    }
    #endregion

    #region stats functions
    public void GetDamage(float damage)
    {
        if (hitted || isDashing)
        {
            return;
        }
        currentHealth -= damage;
        hitted = true;
        anim.SetBool("hitted", hitted);

        if (currentHealth <= 0) 
        {
            currentHealth = 0;
            alive = false;
            rb.velocity = Vector2.zero;
            anim.SetBool("alive", alive);
        }

        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }

    public void SetActiveFalse()
    {
        gameObject.SetActive(false);
    }

    public void SetHittedFalse()
    {
        hitted = false;
        anim.SetBool("hitted", hitted);
    }

    public bool isAlive()
    {
        return alive;
    }
    #endregion

    #region atack
    public void SetAttackingFalse()
    {
        attacking = false;
        anim.SetBool("attacking", false);
    }

    private void ShoftAttack()
    {
        attacking = true;
        anim.SetBool("attacking", attacking);
        rb.velocity = Vector2.zero;

        fist.ShoftHit(atkDist[0], direction, tiempoCD[0], atkMng, damageDeal[0]);
    }

    private void StrongAttack()
    {
        attacking = true;
        anim.SetBool("attacking", attacking);
        rb.velocity = Vector2.zero;

        fist.StrongHit(atkDist[1], direction, tiempoCD[1], atkMng, damageDeal[1]);
    }
    #endregion
}
