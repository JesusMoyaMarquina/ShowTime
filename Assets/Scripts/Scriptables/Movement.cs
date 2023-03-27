using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;
    private Vector2 direction;

    private Vector2 movement;
    private bool isDash;
    private bool isDashing;
    private bool isDashInCooldown;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
    }

    // Update is called once per frame
    void Update()
    {
        movement = playerInput.actions["Move"].ReadValue<Vector2>();
        isDash = playerInput.actions["Dash"].ReadValue<float>() == 1 ? true : false;
    }

    private void FixedUpdate()
    {
        if (isDash && !isDashInCooldown) PlayerDash();
        if (!isDashing)
        {
            PlayerMovement(speed);
            PlayerDirection();
        } else
        {
            PlayerMovement(speed + 10, true);
        }
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
        if(direction.x < 0.1f)
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
}
