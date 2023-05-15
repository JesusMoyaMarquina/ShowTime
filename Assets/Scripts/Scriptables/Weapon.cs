using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    protected bool isInCD;

    public abstract Attack Hit(Vector2 playerDir, string attackName);

    public void Hit(Vector2 playerDir, float tiempoCD, float atkMng)
    {
        Magnetismo(playerDir, atkMng);
        StartCoroutine(TiempoCD(tiempoCD));
    }

    private void Magnetismo(Vector2 playerDir, float atkMng)
    {
        Vector2.MoveTowards(transform.position, playerDir, atkMng);
    }

    IEnumerator TiempoCD(float tiempoCD)
    {
        isInCD = true;
        yield return new WaitForSeconds(tiempoCD);
        isInCD = false;
    }
}
