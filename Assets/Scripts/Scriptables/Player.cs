using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{

    //General variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;

    //Color variables
    private Color baseColor;
    private Color hitColor;

    //Movement variables
    public float speed;
    private Vector2 direction;
    private Vector2 movement;
    private bool isDashing;
    private bool isDashInCooldown;

    //Stats variables
    public float maxHealth;
    private bool hitted;
    private bool alive;
    private float currentHealth;
    private float inmortalityTime;
    private float startInmortalTime;
    private bool isInmortal;

    private GameObject playerHealthBar;
    private GameObject otherPlayerHealthBar;

    //Mocked basic attack variables
    public float damage;
    private bool attacking;

    void Start()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;
        //Start player basics
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();

        //Start direction
        direction = new Vector2(0, -1 * speed);

        //Start color
        baseColor = spriteRenderer.color;
        Color aux = baseColor;
        aux.a = 0.5f;
        hitColor = aux;

        //Start health
        alive = true;
        hitted = false;
        currentHealth = maxHealth;
        inmortalityTime = 1f;

        //Mocked basic attack
        attacking = false;
    }

    private void GameManagerOnGameStateChange(GameState state)
    {
        if (state == GameState.Combat)
        {
            playerHealthBar = GameObject.Find("PlayerHealthProgressBar");
            playerHealthBar.GetComponent<EntityProgressBar>().maximum = maxHealth;
            playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
            playerHealthBar.GetComponent<EntityProgressBar>().previousCurrent = currentHealth;
            playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();

            otherPlayerHealthBar = GameObject.Find("OtherPlayerHealthBar");
        }
    }

    void Update()
    {
        if (GameManager.Instance.state == GameState.Pause) return;

        PlayerMovement();
    }

    private void FixedUpdate()
    {
        if (!alive) return;

        CameraFollowUp();
    }

    private void PlayerMovement()
    {
        if (!alive) return;

        movement = playerInput.actions["Move"].ReadValue<Vector2>();

        PlayerVelocity();
        PlayerDirection();

        if (playerInput.actions["Dash"].ReadValue<float>() == 1 && !isDashInCooldown)
            PlayerDash();

        if (playerInput.actions["Light Hit"].triggered)
            ExecuteBasicAttack();

        if (isInmortal)
        {
            if (startInmortalTime + inmortalityTime < Time.time)
            {
                isInmortal = false;
                spriteRenderer.color = baseColor;
            }
        }
    }

    #region movement functions
    private void CameraFollowUp() => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

    private void PlayerVelocity()
    {
        Vector2 pDirection = isDashing ? direction / 10 : movement.normalized; ;
        float pSpeed = isDashing ? speed * 2 : attacking || hitted ? speed / 2 : speed;

        rb.velocity = pDirection * pSpeed;
    }

    private void PlayerDirection()
    {
        if (isDashing || attacking) return;

        if (rb.velocity != Vector2.zero)
            direction = rb.velocity;

        spriteRenderer.flipX = direction.x > 0.1f;
        
        anim.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("speedY", rb.velocity.y);
    }

    private void PlayerDash()
    {
        if (attacking || hitted) return;

        isDashing = true;
        anim.SetBool("isDashing", isDashing);
        anim.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("speedY", rb.velocity.y);
    }

    private void StopDashing()
    {
        isDashing = false;
        anim.SetBool("isDashing", isDashing);
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
        if (isInmortal || isDashing) return;

        currentHealth -= damage;

        DamageAnimation();

        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }

    private void DamageAnimation()
    {
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            alive = false;
            rb.velocity = Vector2.zero;
            anim.SetBool("alive", alive);
        }
        else
        {
            if (!attacking)
            {
                hitted = true;
                anim.SetBool("hitted", hitted);
            }
            startInmortalTime = Time.time;
            isInmortal = true;
            spriteRenderer.color = hitColor;
        }
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

    private void ExecuteBasicAttack()
    {
        if (isDashing || attacking || hitted) return;

        attacking = true;
        anim.SetBool("attacking", attacking);
        
        GameObject attack = (GameObject)Instantiate(Resources.Load("Prefabs/Attacks/AttackSquare"), new Vector3(transform.position.x, transform.position.y, 0), Quaternion.identity, transform);
        
        if (direction.x > 0)
        {
            attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
            attack.GetComponent<BasicAttack>().SetDirection("right");
        } 
        else if (direction.x < 0)
        {
            attack.transform.rotation = Quaternion.Euler(new Vector3(attack.transform.rotation.x, attack.transform.rotation.y, 90));
            attack.GetComponent<BasicAttack>().SetDirection("left");
        }
        else if (direction.y > 0)
        {
            attack.GetComponent<BasicAttack>().SetDirection("up");
        }
    }
    #endregion

}
