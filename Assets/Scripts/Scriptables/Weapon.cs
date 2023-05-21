using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float[] MngList;
    public int lifeSteal;
    public float knockbackForce;

    public List<Attack> attacks = new List<Attack>();

    protected WeaponType weaponType;
    protected bool isInNormalCD;
    protected bool isInCD;

    protected Attack executedAttack = null;

    private void Awake()
    {
        isInNormalCD = false;
        isInCD = false;
    }

    public abstract Attack Hit(string attackName, int attackCount, Rigidbody2D playerRB, string attackType);

    public void Hit(float tiempoCD, Rigidbody2D playerRB, int attackCount, string attackType)
    {
        Magnetismo(MngList[attackCount], playerRB);
        if (attackCount >= 2)
        {
            if (tiempoCD > 0)
            {
                StartCoroutine(TiempoCD(tiempoCD));
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

    IEnumerator TiempoCD(float tiempoCD)
    {
        isInCD = true;
        yield return new WaitForSeconds(tiempoCD);
        isInCD = false;
    }
}

public enum WeaponType
{
    Fist = 0
}
