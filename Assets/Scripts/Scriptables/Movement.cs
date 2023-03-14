using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float speed;
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private Vector2 direction;

    private float hMovement;
    private float vMovement;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        hMovement = Input.GetAxisRaw("Horizontal");
        vMovement = Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        PlayerMovement();
        PlayerDirection();
    }

    private void PlayerMovement()
    {
        Vector2 playerVelocity = new Vector2(hMovement, vMovement).normalized;

        rb.velocity = playerVelocity * speed;
    }

    private void PlayerDirection()
    {
        if (rb.velocity.x < -0.1f)
        {
            spriteRenderer.flipX = false;
        }
        else if (rb.velocity.x > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        if (rb.velocity != Vector2.zero)
        {
            direction = rb.velocity;
        }

        anim.SetFloat("speedX", Mathf.Abs(rb.velocity.x));
        anim.SetFloat("speedY", rb.velocity.y);
    }
}
