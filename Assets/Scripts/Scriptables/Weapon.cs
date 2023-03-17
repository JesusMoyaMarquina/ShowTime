using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public float damage;
    public float atkMng;

    protected Animator Anim;   
    protected int[] atkList;
    protected int atkCount;  
    protected float atkCD;
    protected bool atkInCD;
    protected Vector2 atkDist;

    void AttackB(float vidaEnem, GameObject enemyClose)
    {
        StartCoroutine(Magnetismo(enemyClose.transform.position));

        Vector2 hepl = enemyClose.transform.position - transform.position;

        if (hepl.x < atkDist.x && hepl.y < atkDist.y)
        {
            _ = vidaEnem - damage;
        }
    }

    IEnumerator Magnetismo(Vector2 enemCercano)
    {
        Vector2.MoveTowards(transform.position, enemCercano, atkMng);
        yield return null;
    }
}
