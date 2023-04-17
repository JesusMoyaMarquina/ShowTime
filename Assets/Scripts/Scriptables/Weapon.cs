using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected bool isInCD;

    public void AttackB(Vector2 atkDist, GameObject enemyClose, float tiempoCD, float atkMng, float damageDeal)
    {
        if (isInCD == false)
        {

            StartCoroutine(Magnetismo(enemyClose.transform.position, tiempoCD, atkMng));

            Vector2 hepl = enemyClose.transform.position - transform.position;

            if (hepl.x < atkDist.x && hepl.y < atkDist.y)
            {
                enemyClose.GetComponent<EnemyMovement>().GetDamage(damageDeal);
            }
            StartCoroutine(TiempoCD(tiempoCD));
        }
    }

    IEnumerator Magnetismo(Vector2 enemCercano, float tiempoCD, float atkMng)
    {
        Vector2.MoveTowards(transform.position, enemCercano, atkMng);
        yield return new WaitForSeconds(tiempoCD);
    }

    IEnumerator TiempoCD(float tiempoCD)
    {
        isInCD = true;
        yield return new WaitForSeconds(tiempoCD);
        isInCD = false;
    }
}
