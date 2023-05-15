using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected WeaponType weaponType;
    protected bool isInCD;

    public abstract Attack Hit(string attackName, int attackCount);

    public void Hit(float tiempoCD, float atkMng)
    {
        Magnetismo(atkMng);
        StartCoroutine(TiempoCD(tiempoCD));
    }

    public bool IsInCD()
    {
        return isInCD;
    }

    private void Magnetismo(float atkMng)
    {
        int direction = GetComponent<SpriteRenderer>().flipX ? 1 : -1;
        GetComponent<Rigidbody2D>().AddForce(new Vector2(atkMng * direction, 0), ForceMode2D.Force);
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
