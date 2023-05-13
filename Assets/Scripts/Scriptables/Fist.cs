using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    public float[] softCD = new float[3];
    public float[] strongCD = new float[3];
    public float[] softDamages = new float[3];
    public float[] strongDamages = new float[3];
    public GameObject[] colliders;

    protected float atkMng = 10;

    private float actualDmg = 0;

    override
    public void SoftHit(Vector2 playerDir, int attackInCombo)
    {
        attackInCombo = attackInCombo - 1;

        if (!isInCD)
        {
            Hit(playerDir, softCD[attackInCombo], atkMng);
            actualDmg = softDamages[attackInCombo];
        }
    }

    override
    public void StrongHit(Vector2 playerDir, int attackInCombo)
    {
        attackInCombo = attackInCombo - 1;

        if (!isInCD)
        {
            Hit(playerDir, strongCD[attackInCombo], atkMng);
            actualDmg = strongDamages[attackInCombo];
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<EnemyMovement>().GetDamage(actualDmg);
        }
    }
}
