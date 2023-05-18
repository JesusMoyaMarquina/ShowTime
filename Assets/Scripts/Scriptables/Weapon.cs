using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public float[] MngList;
    public int lifeSteal;
    public float knockbackForce;

    protected WeaponType weaponType;
    protected bool isInCD;

    private void Awake()
    {
        isInCD = false;
    }

    public abstract Attack Hit(string attackName, int attackCount, Rigidbody2D playerRB);

    public void Hit(float tiempoCD, float atkMng, Rigidbody2D playerRB)
    {
        Magnetismo(atkMng, playerRB);
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

    private void Magnetismo(float atkMng, Rigidbody2D playerRB)
    {
        Vector2 direction = GetComponent<SpriteRenderer>().flipX ? Vector2.right : Vector2.left;
        playerRB.AddForce(direction * atkMng * playerRB.mass, ForceMode2D.Impulse);
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
