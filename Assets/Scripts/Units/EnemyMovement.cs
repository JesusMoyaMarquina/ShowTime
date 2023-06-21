using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public abstract class EnemyMovement : MonoBehaviour
{
    //General variables
    protected Rigidbody2D rb;
    protected CircleCollider2D cc;
    protected Animator anim;
    protected SpriteRenderer spriteRenderer;
    [SerializeField] protected float easyMultiplier, hardMultiplier;

    //Movement variables
    public float speed;
    protected Vector2 direction;

    private Vector2 position;
    public float minDistance;
    protected bool inMovementRange;

    protected GameObject[] players;
    protected GameObject nearPlayer;
    protected float distance;
    protected Vector3 enemyPos;

    protected bool knockbacked;

    //Stats variables
    public bool hitted, stunned, inmortal;
    public float totalHealth;
    public float damage;
    protected float currentHealth;
    protected bool alive;

    //Mocked basic attack variables
    protected bool attacking;
    protected float lastAttack;
    protected float attackCooldown;

    public float distanceToPlayer;

    //Audio variables
    public AudioSource audioSource;
    public AudioClip stepFX, attackFX, hittedFX;

    //Score variables
    public float score;

    //Train variables
    private bool isCentered;

    void Start()
    {
        inmortal = FindObjectOfType<TrainManagerScript>() != null;

        if (SelectDifficultyScript.Instance != null)
        {
            switch (SelectDifficultyScript.Instance.GetDifficulty())
            {
                case 0:
                    totalHealth *= easyMultiplier;
                    damage *= easyMultiplier;
                    break;
                case 2:
                    totalHealth *= hardMultiplier;
                    damage *= hardMultiplier;
                    break;
                default:
                    break;
            }
        }

        rb = GetComponent<Rigidbody2D>();
        cc = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        players = GameObject.FindGameObjectsWithTag("Player");
        knockbacked = false;
        alive = true;
        isCentered = true;
        attackCooldown = 1.5f;
        currentHealth = totalHealth;
    }

    void Update()
    {
        Movement();
    }

    public virtual void Movement()
    {
        if (!alive || !GameManager.Instance.isInCombat) return;

        if (TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain)
        {
            isCentered = transform.position.x < 0.1f && transform.position.y < 0.1f && transform.position.x > -0.1f && transform.position.y > -0.1f ? true : false;
            direction = (-enemyPos).normalized;
            if (isCentered)
            {
                nearPlayer = FindNearPlayer();
                direction = new Vector2(nearPlayer.transform.position.x - enemyPos.x, nearPlayer.transform.position.y - enemyPos.y);
            }
        }
        else
        {
            nearPlayer = FindNearPlayer();

            distance = Vector3.Distance(enemyPos, nearPlayer.transform.position);
            direction = new Vector2(nearPlayer.transform.position.x - enemyPos.x, nearPlayer.transform.position.y - enemyPos.y);
        }

        Tracking();
    }

    public void Knockback(float knockbackForce, Vector2 direction)
    {
        if(GetType() == typeof(BossEM))
        {
            return;
        }

        attacking = false;
        knockbacked = true;
        rb.velocity = Vector2.zero;

        rb.mass = 1;
        rb.AddForce(this.direction.normalized * knockbackForce * rb.mass * -1, ForceMode2D.Impulse);
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

        distanceToPlayer = Vector3.Distance(enemyPos, auxPlayer.transform.position);

        return auxPlayer;
    }

    public abstract void Tracking();

    public abstract void Attacking();

    public virtual void Translation()
    {
        if (knockbacked)
        {
            rb.mass = 1;
            rb.constraints = RigidbodyConstraints2D.FreezeRotation;
            return; 
        }

        if (inMovementRange)
        {
            if (hitted || attacking) return;
            Vector2 direction = (nearPlayer.transform.position - transform.position).normalized;
            var targetPos = new Vector3(nearPlayer.transform.position.x, nearPlayer.transform.position.y, this.transform.position.z);
            transform.LookAt(targetPos);
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.identity;
            SetAttackingFalse();
        }
        else if (TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain && !isCentered)
        {
            if (hitted || attacking) return;
            Vector2 direction = (-transform.position).normalized;
            var targetPos = new Vector3(0, 0, this.transform.position.z);
            transform.LookAt(targetPos);
            rb.velocity = direction * speed;
            transform.rotation = Quaternion.identity;
            SetAttackingFalse();
        }
        else
        {
            position = rb.position;
            rb.velocity = Vector2.zero;
        }
    }

    public char SetDirection(bool withFourAngles = false)
    {
        char look;

        if (withFourAngles)
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                if (direction.x > 0)
                    look = 'L';
                else
                    look = 'R';
            else
                if (direction.y >= 0)
                    look = 'U';
                else
                    look = 'D';
        else
            if (direction.x > 0)
                look = 'L';
            else
                look = 'R';

        return look;
    }

    public char SetDirection(Vector2 direction, bool withFourAngles = false)
    {
        char look;

        if (withFourAngles)
            if (Mathf.Abs(direction.x) >= Mathf.Abs(direction.y))
                if (direction.x > 0)
                    look = 'L';
                else
                    look = 'R';
            else
                if (direction.y >= 0)
                look = 'U';
            else
                look = 'D';
        else
            if (direction.x > 0)
            look = 'L';
        else
            look = 'R';

        switch (SetDirection())
        {
            case 'L':
                spriteRenderer.flipX = true;
                break;
            case 'R':
                spriteRenderer.flipX = false;
                break;
        }

        return look;
    }

    public void StopMoving()
    {
        anim.SetBool("isMoving", false);
    }

    public void SetAnimation()
    {
        if(knockbacked) return;

        switch (SetDirection())
        {
            case 'L':
                spriteRenderer.flipX = true;
                break;
            case 'R':
                spriteRenderer.flipX = false;
                break;
        }

        if(TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain)
        {
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
            anim.SetBool("isMoving", rb.velocity.magnitude > 0.1f ? true : false);
            return;
        }

        if (attacking || hitted && Vector2.Distance(rb.position, position) > 0.5f)
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            return;
        }

        if (inMovementRange && GetType() != typeof(BossEM))
        {
            rb.mass = 1;
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        }
        else if (GetType() != typeof(BossEM))
        {
            rb.mass = 0.025f;

            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            
            if (!hitted && (!attacking && Time.time > lastAttack + attackCooldown) && (TrainManagerScript.Instance == null || TrainManagerScript.Instance.attackingTrain))
            {
                attacking = true;
            }
            else if (TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain)
            {
                SetAttackingFalse();
            }
        } else if (inMovementRange)
        {
            rb.constraints = RigidbodyConstraints2D.None | RigidbodyConstraints2D.FreezeRotation;
        } else
        {
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;

            if (!hitted && (!attacking && Time.time > lastAttack + attackCooldown) && (TrainManagerScript.Instance == null || TrainManagerScript.Instance.attackingTrain))
            {
                attacking = true;
            }
            else if (TrainManagerScript.Instance != null && !TrainManagerScript.Instance.attackingTrain)
            {
                SetAttackingFalse();
            }
        }

        anim.SetBool("attacking", attacking);
        anim.SetBool("isMoving", inMovementRange);
    }

    public IEnumerator SetStunned(float seconds)
    {
        stunned = true;
        anim.SetBool("stunned", true);
        yield return new WaitForSeconds(seconds);
        stunned = false;
        anim.SetBool("stunned", false);
    }

    #region stats functions
    public virtual void GetDamage(float damage)
    {
        if (!inmortal)
        {
            currentHealth -= damage;
        }
        SetAttackingFalse();
        PlayHittedFX();

        CheckDeadCondition();

        hitted = false;
        anim.SetBool("hitted", hitted);
        
        hitted = true;
        anim.SetBool("hitted", hitted);
    }

    public bool isAlive()
    {
        return alive;
    }

    public void Die()
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        currentHealth = 0;
        CheckDeadCondition();
    }

    public void DestroyThis()
    {
        Destroy(gameObject);
    }

    public void SetHittedFalse()
    {

        hitted = false;
        knockbacked = false;
        anim.SetBool("hitted", hitted);
        if (!(GetType() == typeof(BossEM)))
            SetAttackingFalse();
    }

    public abstract void SetAttackingFalse();
    public abstract void AddScore();

    protected void CheckDeadCondition()
    {
        if (currentHealth <= 0)
        {
            if (!(GetType() == typeof(BossEM)))
            {
                cc.enabled = false;
                rb.constraints = RigidbodyConstraints2D.FreezeAll;
            } else
            {
                CombatManager.instance.ReduceBossesToKill();
            }

            currentHealth = 0;
            alive = false;
            anim.SetBool("alive", alive);
        }
    }
    #endregion

    #region Audio management
    public void PlayAttackFX()
    {
        audioSource.PlayOneShot(attackFX);
    }

    public void PlayStepFX()
    {
        audioSource.PlayOneShot(stepFX);
    }

    public void PlayHittedFX()
    {
        audioSource.PlayOneShot(hittedFX);
    }
    #endregion
}
