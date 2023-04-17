using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    //General variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;


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

    //Mocked basic attack variables
    public float damage;
    private bool attacking;


    void Start()
    {
        //Start player basics
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();

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

        //Mocked basic attack
        attacking = false;
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
            executeBasicAttack();
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

    #region Basic player atack
    public void SetAttackingFalse()
    {
        attacking = false;
        anim.SetBool("attacking", false);
    }

    private void executeBasicAttack()
    {
        attacking = true;
        anim.SetBool("attacking", attacking);
        rb.velocity = Vector2.zero;
        if (direction.x > 0)
        {
            GameObject attack = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/AttackSquare"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);
            attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
            attack.GetComponent<BasicAttack>().SetDirection("right");
        } 
        else if (direction.x < 0)
        {
            GameObject attack = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/AttackSquare"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);
            attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
            attack.GetComponent<BasicAttack>().SetDirection("left");
        }
        else if (direction.y > 0)
        {
            GameObject attack = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/AttackSquare"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);
            attack.GetComponent<BasicAttack>().SetDirection("up");
        }
        else
        {
            GameObject attack = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/AttackSquare"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);
        }
    }
    #endregion

}
