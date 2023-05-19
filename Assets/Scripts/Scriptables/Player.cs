using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    //General variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;

    //AudioVariables
    private AudioSource audioSource;
    public AudioClip stepFX, softAttackFX, strongAttackFX;

    //Color variables
    private Color baseColor;
    private Color hitColor;

    //Movement variables
    public float speed;
    private Vector2 direction;
    private Vector2 movement;
    private bool isDashing;
    private bool isDashInCooldown;
    private bool stunned;
    private float stunnedTime;

    //Stats variables
    public float maxHealth;
    public float knockbackForce;
    private Vector2 damageDirection;
    private bool hitted;
    private bool alive;
    private float currentHealth;
    private float inmortalityTime;
    private float startInmortalTime;
    private bool knockbacked;
    private bool isInmortal;

    private GameObject playerHealthBar;
    private GameObject otherPlayerHealthBar;

    //Atack Stats
    protected Vector2[] atkDist = new Vector2[2];

    protected float[] tiempoCD = new float[2];
    protected float[] damageDeal = new float[2];

    private bool attacking;
    private string executeAttackName;

    //Abilities
    private GameObject fistComboProgressBar, dashProgressBar;

    private float dashCooldownTime;
    private float executedAttackCD, totalExecutedAttackTime;

    //Combo system
    private Weapon currentWeapon;
    public Attack executedAttack;
    public Queue<string> inputQueue;

    private void Awake()
    {
        //Start player basics
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();

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

        //Abilities
        dashCooldownTime = 0;

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
        stunned = false;
        executedAttack = null;
        currentWeapon = GetComponent<Fist>();
    }

    private void Start()
    {
        //Abilities
        fistComboProgressBar = GameObject.Find("FistRadialDownProgressBarWithImage");
        dashProgressBar = GameObject.Find("DashRadialDownProgressBarWithImage");
        SetDashProgressBarToMaximum();
        SetFistProgressBarToMaximum(1);

        //Health
        playerHealthBar = GameObject.Find("PlayerHealthProgressBar");
        playerHealthBar.GetComponent<EntityProgressBar>().maximum = maxHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().previousCurrent = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }

    void Update()
    {
        if (!GameManager.Instance.isInCombat || stunned)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            anim.SetFloat("speed", 0);
            return;
        } else
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        PlayerMovement();
    }

    private void FixedUpdate()
    {
        if (!alive) return;

        CameraFollowUp();
    }

    public Vector2 GetDirection()
    {
        return direction;
    }

    private void PlayerMovement()
    {
        if (!alive) return;

        UpdateProgressBars();

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

        HandleAttack();
    }

    void QuitarAccion()
    {
        inputQueue.Clear();
    }

    #region Movement functions
    private void CameraFollowUp() => Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, -10);

    private void PlayerVelocity()
    {
        if (knockbacked || attacking) return;

        Vector2 pDirection = isDashing ? direction / 10 : movement.normalized;
        float pSpeed = isDashing ? speed * 1.2f : hitted ? speed / 2 : speed;

        rb.velocity = pDirection * pSpeed;
    }

    private void PlayerDirection()
    {
        if (isDashing || knockbacked || attacking) return;

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

    public void StopDashing()
    {
        isDashing = false;
        anim.SetBool("isDashing", isDashing);
        dashCooldownTime = 2 + Time.time - CombatManager.instance.beginBattleTime;
        StartCoroutine(DashCooldown());
    }

    IEnumerator DashCooldown()
    {
        isDashInCooldown = true;
        yield return new WaitForSeconds(2);
        isDashInCooldown = false;
    }
    #endregion

    #region Stats functions

    public void Heal(float heal)
    {
        if (currentHealth + heal < maxHealth)
        {
            currentHealth += heal;
        } else
        {
            currentHealth = maxHealth;
        }

        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }

    public void GetDamage(float damage, Vector2 damageDirection, float stunnedTime = 0, bool attackCancel = false, float force = 0)
    {
        if (isInmortal || isDashing || damage < 0) return;

        this.damageDirection = damageDirection;

        this.stunnedTime = stunnedTime;

        currentHealth -= damage;

        DamageAnimation(attackCancel, force);

        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();
    }

    private void CancelAttack()
    {
        SetAttackingFalse();
        DeactivateAttackCollider();
    }

    private void DamageAnimation(bool attackCancel, float force)
    {
        CancelAttack();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            alive = false;
            rb.velocity = Vector2.zero;
            anim.SetBool("alive", alive);
        }
        else
        {
            if (!attacking || !attackCancel)
            {
                if (force == 0)
                {
                    rb.AddForce(damageDirection * knockbackForce * rb.mass, ForceMode2D.Impulse);
                } else
                {
                    rb.AddForce(damageDirection * force * rb.mass, ForceMode2D.Impulse);
                }
                knockbacked = true;
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
        knockbacked = false;
        if (stunnedTime > 0)
        {
            StartCoroutine(StunCoroutine(stunnedTime));
        }
        anim.SetBool("hitted", hitted);
    }

    IEnumerator StunCoroutine(float time)
    {
        stunned = true;
        yield return new WaitForSeconds(time);
        stunned = false;
        stunnedTime = 0;
    }



    public bool isAlive()
    {
        return alive;
    }
    #endregion

    #region Attack functions
    private void HandleAttack()
    {
        if (inputQueue.Count >= 0 && inputQueue.Count <= 2 && !attacking && !hitted && !isDashing)
        {
            if (playerInput.actions["SoftHit"].triggered)
            {
                anim.SetInteger("finish", -1);
                attacking = true;

                inputQueue.Enqueue("softHit");
                CancelInvoke(nameof(QuitarAccion));
                Invoke(nameof(QuitarAccion), 2);

                if (inputQueue.Count < 3)
                {
                    Attack("SoftAttack");
                    PlaySoftAttackFX();
                }

                anim.SetBool("attacking", attacking);
                anim.SetTrigger("softAttack");
            }

            if (playerInput.actions["StrongHit"].triggered)
            {
                anim.SetInteger("finish", -1);
                attacking = true;

                inputQueue.Enqueue("strongHit");
                CancelInvoke(nameof(QuitarAccion));
                Invoke(nameof(QuitarAccion), 2);

                if (inputQueue.Count < 3)
                {
                    Attack("StrongAttack");
                    PlayStrongAttackFX();
                }

                anim.SetBool("attacking", attacking);
                anim.SetTrigger("strongAttack");
            }
        }

        if (currentWeapon.IsInCD())
        {
            QuitarAccion();
        }

        if (inputQueue.Count == 3)
        {
            List<string> actionList = inputQueue.ToList();

            switch ((actionList[0], actionList[1], actionList[2]))
            {
                case ("softHit", "softHit", "softHit"):

                    Attack("SoftSoftSoftCombo");
                    anim.SetInteger("finish", 0);

                    break;
                case ("softHit", "strongHit", "softHit"):

                    Attack("SoftStrongSoftCombo");
                    anim.SetInteger("finish", 1);

                    break;
                case ("strongHit", "softHit", "softHit"):

                    Attack("StrongSoftSoftCombo");
                    anim.SetInteger("finish", 2);

                    break;
                case ("strongHit", "strongHit", "softHit"):

                    Attack("StrongStrongSoftCombo");
                    anim.SetInteger("finish", 3);

                    break;
                case ("softHit", "softHit", "strongHit"):

                    Attack("SoftSoftStrongCombo");
                    anim.SetInteger("finish", 0);

                    break;
                case ("softHit", "strongHit", "strongHit"):

                    Attack("SoftStrongStrongCombo");
                    anim.SetInteger("finish", 1);

                    break;
                case ("strongHit", "softHit", "strongHit"):

                    Attack("StrongSoftStrongCombo");
                    anim.SetInteger("finish", 2);

                    break;
                case ("strongHit", "strongHit", "strongHit"):

                    Attack("StrongStrongStrongCombo");
                    anim.SetInteger("finish", 3);

                    break;
            }
            QuitarAccion();
        }

    }

    public void SetAttackingFalse()
    {
        attacking = false;
        anim.SetBool("attacking", false);
    }

    public void DeactivateAttackCollider()
    {
        if (executedAttack != null)
        {
            executedAttack.DeactivateCollider();
        }
    }

    public void StopMgn()
    {
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = Vector3.zero;
    }

    private void Attack(string attackName)
    {
        executedAttack = currentWeapon.Hit(attackName, inputQueue.Count - 1, rb);
        if (executedAttack.GetCD() > 0)
        {
            SetFistProgressBarToMaximum(executedAttack.GetCD());
            executedAttackCD = executedAttack.GetCD() + Time.time - CombatManager.instance.beginBattleTime;
            totalExecutedAttackTime = executedAttack.GetCD();
        }
    }

    public bool GetIsDashing()
    {
        return isDashing;
    }
    #endregion

    #region Abilities graphic management
    private void UpdateProgressBars()
    {
        float actualDashCooldownTime;
        if (dashCooldownTime > 0)
        {
            actualDashCooldownTime = 2 - (dashCooldownTime - (Time.time - CombatManager.instance.beginBattleTime));
            UpdateDashCooldownBar(actualDashCooldownTime);
        }
        else
        {
            actualDashCooldownTime = 0;
        }

        if (actualDashCooldownTime >= 2)
        {
            dashCooldownTime = 0;
            SetDashProgressBarToMaximum();
        }

        float actualExecutedAttackCD;
        if (executedAttackCD > 0)
        {
            actualExecutedAttackCD = totalExecutedAttackTime - (executedAttackCD - (Time.time - CombatManager.instance.beginBattleTime));
            UpdateFistCooldownBar(actualExecutedAttackCD);
        }
        else
        {
            actualExecutedAttackCD = 0;
        }

        if (actualExecutedAttackCD >= totalExecutedAttackTime && totalExecutedAttackTime > 0)
        {
            executedAttackCD = 0;
            SetFistProgressBarToMaximum(totalExecutedAttackTime);
        }
    }

    private void SetDashProgressBarToMaximum()
    {
        SetProgressBarToMaximum(dashProgressBar, 2);
    }

    private void UpdateDashCooldownBar(float current)
    {
        UpdateProgressBar(dashProgressBar, current);
    }
    public void SetFistProgressBarToMaximum(float maximum)
    {
        SetProgressBarToMaximum(fistComboProgressBar, maximum);
    }

    private void UpdateFistCooldownBar(float current)
    {
        UpdateProgressBar(fistComboProgressBar, current);
    }

    private void SetProgressBarToMaximum(GameObject progressBarGO, float maximum = 1)
    {
        ProgressBar progressBar = progressBarGO.GetComponent<ProgressBar>();
        progressBar.maximum = maximum;
        progressBar.current = 0;
        progressBar.GetCurrentFill();
    }

    private void UpdateProgressBar(GameObject progressBarGO, float current)
    {
        ProgressBar progressBar = progressBarGO.GetComponent<ProgressBar>();
        progressBar.current = current;
        progressBar.GetCurrentFill();
    }
    #endregion

    #region Sound management
    public void PlayStepFX()
    {
        audioSource.PlayOneShot(stepFX);
    }

    private void PlaySoftAttackFX()
    {
        audioSource.PlayOneShot(softAttackFX);
    }

    private void PlayStrongAttackFX()
    {
        audioSource.PlayOneShot(strongAttackFX);
    }


    #endregion
}
