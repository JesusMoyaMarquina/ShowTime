using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    private int actualWeapon;

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

    //Atack Stats
    protected Vector2[] atkDist = new Vector2[2];

    protected float[] tiempoCD = new float[2];
    protected float[] damageDeal = new float[2];

    protected float atkMng = 10;

    private bool attacking;

    //Combo system
    private Queue<string> inputQueue;

    private void Awake()
    {
        GameManager.OnGameStateChange += GameManagerOnGameStateChange;

        //Start player basics
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        actualWeapon = 0;

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

        //Atack Stats
        atkDist[0] = new Vector2();
        atkDist[1] = new Vector2();

        tiempoCD[0] = 1;
        tiempoCD[1] = 1.5f;

        damageDeal[0] = 15;
        damageDeal[1] = 25;

        //Combo Sistem
        inputQueue = new Queue<string>();

        //Attack
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
        }
    }

    void Update()
    {
        if (!(GameManager.Instance.state == GameState.Combat))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            anim.SetFloat("speed", 0);
            return;
        }

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

        if (isInmortal)
        {
            if (startInmortalTime + inmortalityTime < Time.time)
            {
                isInmortal = false;
                spriteRenderer.color = baseColor;
            }
        }

        if (inputQueue.Count >= 0 && inputQueue.Count <= 2 && !attacking)
        {
            if (playerInput.actions["SoftHit"].triggered)
            {
                anim.SetInteger("finish", -1);
                attacking = true;

                inputQueue.Enqueue("softHit");
                ShoftAttack();
                CancelInvoke(nameof(QuitarAccion));
                Invoke(nameof(QuitarAccion), 2);

                anim.SetBool("attacking", attacking);
                anim.SetTrigger("softAttack");
            }

            if (playerInput.actions["StrongHit"].triggered)
            {
                anim.SetInteger("finish", -1);
                attacking = true;

                inputQueue.Enqueue("strongHit");
                StrongAttack();
                CancelInvoke(nameof(QuitarAccion));
                Invoke(nameof(QuitarAccion), 2);

                anim.SetBool("attacking", attacking);
                anim.SetTrigger("strongAttack");
            }
        }

        if (inputQueue.Count == 3)
        {
            List<string> actionList = inputQueue.ToList();
            switch ((actionList[0], actionList[1], actionList[2]))
            {
                case ("softHit", "softHit", "softHit"):
                    anim.SetInteger("finish", 0);
                    break;
                case ("softHit", "strongHit", "softHit"):
                    anim.SetInteger("finish", 1);
                    break;
                case ("strongHit", "softHit", "softHit"):
                    anim.SetInteger("finish", 2);
                    break;
                case ("strongHit", "strongHit", "softHit"):
                    anim.SetInteger("finish", 3);
                    break;
                case ("softHit", "softHit", "strongHit"):
                    anim.SetInteger("finish", 0);
                    break;
                case ("softHit", "strongHit", "strongHit"):
                    anim.SetInteger("finish", 1);
                    break;
                case ("strongHit", "softHit", "strongHit"):
                    anim.SetInteger("finish", 2);
                    break;
                case ("strongHit", "strongHit", "strongHit"):
                    anim.SetInteger("finish", 3);
                    break;
            }
            QuitarAccion();
        }
    }

    void QuitarAccion()
    {
        inputQueue.Clear();
    }

    #region movement functions
    private void CameraFollowUp() => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

    private void PlayerVelocity()
    {
        Vector2 pDirection = isDashing ? direction / 10 : movement.normalized;
        float pSpeed = isDashing ? speed * 2 : attacking || hitted ? speed / 2 : speed;

        rb.velocity = pDirection * pSpeed;
    }

    private void PlayerDirection()
    {
        if (isDashing || attacking) return;

        if (rb.velocity.x != 0)
            direction = rb.velocity;

        spriteRenderer.flipX = direction.x > 0;
        
        anim.SetFloat("speed", rb.velocity.magnitude);
    }

    private void PlayerDash()
    {
        if (attacking || hitted) return;

        isDashing = true;
        anim.SetBool("isDashing", isDashing);
        anim.SetFloat("speed", rb.velocity.magnitude);
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

    #region atack
    public void SetAttackingFalse()
    {
        attacking = false;
        anim.SetBool("attacking", false);
    }

    private void ShoftAttack()
    {
        Weapon weapon;

        //Default = Fist
        switch (actualWeapon) 
        {
            default:
                weapon = GetComponent<Fist>();
                break;
        }

        weapon.SoftHit(direction, inputQueue.Count);
    }

    private void StrongAttack()
    {
        Weapon weapon;

        //Default = Fist
        switch (actualWeapon)
        {
            default:
                weapon = GetComponent<Fist>();
                break;
        }

        weapon.StrongHit(direction, inputQueue.Count);
    }
    #endregion
}
