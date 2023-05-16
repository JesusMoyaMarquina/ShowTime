using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public int lifeSteal;
    public float knockbackForce;

    protected WeaponType weaponType;
    protected bool isInCD;

    private void Awake()
    {
        isInCD = false;
    }

    public abstract Attack Hit(string attackName, int attackCount);

    public void Hit(float tiempoCD, float atkMng)
    {
        Magnetismo(atkMng);
        if (tiempoCD > 0)
        {
            StartCoroutine(TiempoCD(tiempoCD));
        }
    }

    public float GetLifeSteal()
    {
        return lifeSteal;
    }

    public bool IsInCD()
    {
        return isInCD;
    }

    private void Magnetismo(float atkMng)
    {
        Vector2 direction = GetComponent<SpriteRenderer>().flipX ? Vector2.right : Vector2.left;
        GetComponent<Rigidbody2D>().AddForce(direction * atkMng, ForceMode2D.Force);
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
