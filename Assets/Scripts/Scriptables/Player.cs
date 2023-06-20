using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;
using TMPro;
using Unity.Mathematics;

public class Player : MonoBehaviour
{

    //General variables
    private Rigidbody2D rb;
    private Animator anim;
    private SpriteRenderer spriteRenderer;
    private PlayerInput playerInput;

    //AudioVariables
    private AudioSource audioSource;
    public AudioClip stepFX, hurtFX, softAttackFX, strongAttackFX, finisherAttackFX;

    //Color variables
    private Color baseColor;
    private Color hitColor;

    //Movement variables
    [SerializeField] float speed, healCooldownTime, dashCooldownTime;
    private Vector2 direction;
    private Vector2 movement;
    private bool isDashing, isDashInCooldown, stunned, isHealInCooldown;
    private float stunnedTime, healCooldownStartTime, dashCooldownStartTime;

    //Stats variables
    public float maxHealth;
    public float knockbackForce;
    private Vector2 damageDirection;
    private bool hitted, inmortal;
    private bool alive;
    private float currentHealth;      // cambio para el guardado de partida
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
    private GameObject fistComboProgressBar, dashProgressBar, healProgressBar, screw;

    private float executedAttackCD, totalExecutedAttackTime;
    private float healCharge;

    //Combo system
    [SerializeField] private float cooldownReducer;
    private Weapon currentWeapon;
    public Attack executedAttack;
    public Queue<string> inputQueue;

    private AvailableComboPanelScript avialibleComboPanel;

    private void Awake()
    {
        //Start player basics
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerInput = GetComponent<PlayerInput>();
        audioSource = GetComponent<AudioSource>();

        //Start direction
        direction = new Vector2(speed, 0);

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
        dashCooldownStartTime = 0;

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
        inmortal = FindObjectOfType<TrainManagerScript>() != null;

        //Abilities
        fistComboProgressBar = GameObject.Find("FistUpDownProgressBar");
        dashProgressBar = GameObject.Find("DashUpDownProgressBar");
        healProgressBar = GameObject.Find("HealDownUpProgressBar");
        screw = FindObjectOfType<Screw>().gameObject;
        SetDashProgressBarToMaximum();
        SetFistProgressBarToMaximum(1);
        SetChargeBarScriptToMaximum(healProgressBar);

        //Health
        playerHealthBar = GameObject.Find("PlayerHealthProgressBar");
        playerHealthBar.GetComponent<EntityProgressBar>().maximum = maxHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().current = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().previousCurrent = currentHealth;
        playerHealthBar.GetComponent<EntityProgressBar>().GetCurrentFill();

        //Combo list
        avialibleComboPanel = FindObjectOfType<AvailableComboPanelScript>();
    }

    void Update()
    {
        if(TrainManagerScript.Instance != null)
        {
            if (playerInput.actions["SwapTrainingMode"].triggered)
            {
                TrainManagerScript.Instance.SwapTrainingMode();
            }
        }

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

        if (playerInput.actions["Heal"].triggered && !isHealInCooldown)
        {
            Heal(healCharge * maxHealth);
        }

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
        float pSpeed = isDashing ? speed : hitted ? speed / 2 : speed;

        rb.velocity = pDirection * pSpeed;
    }

    private void PlayerDirection()
    {
        if (isDashing || attacking) return;


        if(knockbacked && alive)
            direction = - damageDirection;
        else if (rb.velocity.x != 0 && alive)
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
        dashCooldownStartTime = dashCooldownTime + Time.time;
        StartCoroutine(DashCooldown());
    }

    IEnumerator HealCooldown()
    {
        isHealInCooldown = true;
        yield return new WaitForSeconds(healCooldownTime);
        isHealInCooldown = false;
    }

    IEnumerator DashCooldown()
    {
        isDashInCooldown = true;
        yield return new WaitForSeconds(dashCooldownTime);
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


        print(healCharge);

        if (healCharge >= 1)
        {
            ActivateRandomBoost();
        }

        healCharge = 0;

        healProgressBar.GetComponent<ChargeBarScript>().current = healCharge;
        healProgressBar.GetComponent<ChargeBarScript>().GetCurrentFill();

        if (screw != null)
        {
            screw.GetComponent<Screw>().ActivateHeal();
        }

        healCooldownStartTime = healCooldownTime + Time.time;
        StartCoroutine(HealCooldown());
    }

    private void ActivateRandomBoost()
    {
        //Speed, attack damage, recieved damage, cooldown reducer 
        int randNum = UnityEngine.Random.Range(0, 100);

        if (randNum < 25)
        {
            print("speedUp");
        }
        else if (randNum < 50)
        {
            print("attackUp");
        }
        else if (randNum < 75)
        {
            print("defeseUp");
        }
        else
        {
            print("cdReduction");
        }
    }

    public void ChargeHeal(float heal)
    {
        if (healCharge + heal / maxHealth < 1)
        {
            healCharge += heal / maxHealth;
        }
        else
        {
            healCharge = 1;
        }

        healProgressBar.GetComponent<ChargeBarScript>().current = healCharge;
        healProgressBar.GetComponent<ChargeBarScript>().GetCurrentFill();
    }

    public void GetDamage(float damage, Vector2 damageDirection, float stunnedTime = 0, bool attackCancel = false, float force = 0)
    {
        if (isInmortal || isDashing || damage < 0 || inputQueue.Count >= 3) return;

        this.damageDirection = damageDirection;

        this.stunnedTime = stunnedTime;

        if (!inmortal)
        {
            currentHealth -= damage;
        }

        if (healCharge > 0.75f)
        {
            healCharge = 0.75f;
            healProgressBar.GetComponent<ChargeBarScript>().current = healCharge;
            healProgressBar.GetComponent<ChargeBarScript>().GetCurrentFill();
        }
        else if (healCharge > 0.5f)
        {
            healCharge = 0.5f;
            healProgressBar.GetComponent<ChargeBarScript>().current = healCharge;
            healProgressBar.GetComponent<ChargeBarScript>().GetCurrentFill();
        }

        PlayHurtFX();

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
            direction = -damageDirection;
            PlayerDirection();
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
        if (inputQueue.Count >= 0 && inputQueue.Count <= 2 && !attacking && !hitted && !isDashing && !currentWeapon.IsInNormalCD())
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
            if (avialibleComboPanel != null)
            {
                avialibleComboPanel.ComboListUpdate(inputQueue, true);
            }
            QuitarAccion();
        } else
        {
            if (avialibleComboPanel != null)
            {
                avialibleComboPanel.ComboListUpdate(inputQueue);
            }
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
            PlayFinisherFX();
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
        executedAttack = currentWeapon.Hit(attackName, inputQueue.Count - 1, rb, inputQueue.Last());
        if (executedAttack.GetCD() > 0 && inputQueue.Count >= 3)
        {
            SetFistProgressBarToMaximum(executedAttack.GetCD());
            executedAttackCD = executedAttack.GetCD() + Time.time;
            totalExecutedAttackTime = executedAttack.GetCD();
        }
    }

    public bool GetIsDashing()
    {
        return isDashing;
    }

    public void ReduceCooldown()
    {
        currentWeapon.ReduceCD();
        executedAttackCD -= currentWeapon.GetCDReducer();
    }
    #endregion

    #region Abilities graphic management
    private void UpdateProgressBars()
    {
        float actualHealCooldownTime;
        if (healCooldownStartTime > 0)
        {
            actualHealCooldownTime = healCooldownStartTime - Time.time;
            UpdateHealCooldownBar(actualHealCooldownTime);
        }
        else
        {
            actualHealCooldownTime = 0;
        }

        if (actualHealCooldownTime <= 0)
        {
            healCooldownStartTime = 0;
            SetHealProgressBarToMaximum();
        }

        float actualDashCooldownTime;
        if (dashCooldownStartTime > 0)
        {
            actualDashCooldownTime = dashCooldownStartTime - Time.time;
            UpdateDashCooldownBar(actualDashCooldownTime);
        }
        else
        {
            actualDashCooldownTime = 0;
        }

        if (actualDashCooldownTime <= 0)
        {
            dashCooldownStartTime = 0;
            SetDashProgressBarToMaximum();
        }

        float actualExecutedAttackCD;
        if (executedAttackCD > 0)
        {
            actualExecutedAttackCD = executedAttackCD - Time.time;
            UpdateFistCooldownBar(actualExecutedAttackCD);
        }
        else
        {
            actualExecutedAttackCD = 0;
        }

        if (actualExecutedAttackCD <= 0)
        {
            executedAttackCD = 0;
            SetFistProgressBarToMaximum(totalExecutedAttackTime);
        }
    }

    private void SetHealProgressBarToMaximum()
    {
        SetProgressBarToMaximum(healProgressBar, healCooldownTime);
    }

    private void SetDashProgressBarToMaximum()
    {
        SetProgressBarToMaximum(dashProgressBar, dashCooldownTime);
    }

    private void UpdateDashCooldownBar(float current)
    {
        UpdateProgressBar(dashProgressBar, current);
    }

    private void UpdateHealCooldownBar(float current)
    {
        UpdateProgressBar(healProgressBar, current);
    }

    public void SetFistProgressBarToMaximum(float maximum)
    {
        SetProgressBarToMaximum(fistComboProgressBar, maximum);
    }

    private void UpdateFistCooldownBar(float current)
    {
        UpdateProgressBar(fistComboProgressBar, current);
    }

    private void SetChargeBarScriptToMaximum(GameObject progressBarGO, float maximum = 1)
    {
        ChargeBarScript progressBar = progressBarGO.GetComponent<ChargeBarScript>();
        progressBar.maximum = maximum;
        progressBar.current = 0;
        progressBar.GetCurrentFill();
    }

    private void SetProgressBarToMaximum(GameObject progressBarGO, float maximum = 1)
    {
        ProgressBar progressBar = progressBarGO.GetComponent<ProgressBar>();
        progressBarGO.GetComponentInChildren<TextMeshProUGUI>().text = "";
        progressBar.maximum = maximum;
        progressBar.current = 0;
        progressBar.GetCurrentFill();
    }

    private void UpdateProgressBar(GameObject progressBarGO, float current)
    {
        ProgressBar progressBar = progressBarGO.GetComponent<ProgressBar>();
        progressBarGO.GetComponentInChildren<TextMeshProUGUI>().text = string.Format("{0:F1}", current);
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

    private void PlayFinisherFX()
    {
        audioSource.PlayOneShot(finisherAttackFX);
    }

    private void PlayHurtFX()
    {
        audioSource.PlayOneShot(hurtFX);
    }


    #endregion
}
