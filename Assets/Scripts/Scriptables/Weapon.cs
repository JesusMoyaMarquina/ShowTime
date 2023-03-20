using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected List<float> damage;
    public float atkMng;

    protected Animator Anim;   
    protected List<int> atkList;
    protected int atkCount;  
    protected float atkCD;
    protected bool isInCD;

    void AttackB(Vector2 atkDist, GameObject enemyClose, float tiempoCD)
    {
        if (isInCD == false)
        {

            StartCoroutine(Magnetismo(enemyClose.transform.position, tiempoCD));

            Vector2 hepl = enemyClose.transform.position - transform.position;

            if (hepl.x < atkDist.x && hepl.y < atkDist.y)
            {
                enemyClose.GetComponent<Enemy>().MakeDmg(damage[atkCount]);
            }

            StartCoroutine(TiempoCD(tiempoCD));
        }
    }

    IEnumerator Magnetismo(Vector2 enemCercano, float tiempoCD)
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
