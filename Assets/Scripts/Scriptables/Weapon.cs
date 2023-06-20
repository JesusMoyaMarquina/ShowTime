using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private float cdReducer;
    [SerializeField] private int lifeSteal;
    public float[] MngList;

    [SerializeField] protected List<Attack> attacks = new List<Attack>();

    protected float startCD, timeCD;
    protected WeaponType weaponType;
    protected bool isInNormalCD;
    protected bool isInCD;

    protected Attack executedAttack = null;

    private void Awake()
    {
        isInNormalCD = false;
        isInCD = false;
    }

    private void Update()
    {
        if (startCD - Time.time >= 0)
        {
            isInCD = true;
        } else
        {
            isInCD = false;
        }
    }

    public abstract Attack Hit(string attackName, int attackCount, Rigidbody2D playerRB, string attackType);

    public void Hit(float timeCD, Rigidbody2D playerRB, int attackCount, string attackType)
    {
        Magnetismo(MngList[attackCount], playerRB);
        if (attackCount >= 2)
        {
            if (timeCD > 0)
            {
                this.timeCD = timeCD;
                StartCD();
            }
        }
        if (attackType == "softHit")
        {
            StartCoroutine(TiempoNormalCD(attacks.Find(o => o.GetAttackName() == "SoftAttack").GetCD()));
        } else
        {
            StartCoroutine(TiempoNormalCD(attacks.Find(o => o.GetAttackName() == "StrongAttack").GetCD()));
        }
    }

    public void ReduceCD()
    {
        startCD -= cdReducer;
    }

    private void StartCD()
    {
        startCD = timeCD + Time.time;
    }

    public float GetLifeSteal()
    {
        return lifeSteal;
    }

    public bool IsInNormalCD()
    {
        return isInNormalCD;
    }

    public bool IsInCD()
    {
        return isInCD;
    }

    public float GetCDReducer()
    {
        return cdReducer;
    }

    private void Magnetismo(float atkMng, Rigidbody2D playerRB)
    {
        Vector2 direction = GetComponent<SpriteRenderer>().flipX ? Vector2.right : Vector2.left;
        playerRB.AddForce(direction * atkMng * playerRB.mass, ForceMode2D.Impulse);
    }

    IEnumerator TiempoNormalCD(float tiempoCD)
    {
        isInNormalCD = true;
        yield return new WaitForSeconds(tiempoCD);
        isInNormalCD = false;
    }
}

public enum WeaponType
{
    Fist = 0
}
