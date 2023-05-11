using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected bool isInCD;

    public void AttackB(Vector2 atkDist, Vector2 playerDir, float tiempoCD, float atkMng, float damageDeal)
    {
        if (isInCD == false)
        {
            StartCoroutine(Magnetismo(playerDir, tiempoCD, atkMng));

            if (playerDir.x < atkDist.x && playerDir.y < atkDist.y)
            {
                //enemyClose.GetComponent<EnemyMovement>().GetDamage(damageDeal);
            }
            StartCoroutine(TiempoCD(tiempoCD));
        }
    }

    IEnumerator Magnetismo(Vector2 playerDir, float tiempoCD, float atkMng)
    {
        Vector2.MoveTowards(transform.position, playerDir, atkMng);
        yield return new WaitForSeconds(tiempoCD);
    }

    IEnumerator TiempoCD(float tiempoCD)
    {
        isInCD = true;
        yield return new WaitForSeconds(tiempoCD);
        isInCD = false;
    }
}
