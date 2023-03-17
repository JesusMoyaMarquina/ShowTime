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
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerDirection();
    }

    private void PlayerMovement()
    {
        Vector2 playerVelocity = movement.normalized;

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

        anim.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("speedY", rb.velocity.y);
    }
}
