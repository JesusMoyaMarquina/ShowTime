using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    protected Vector2[] atkDist = new Vector2[2];

    protected float[] tiempoCD = new float[2];
    protected float[] damageDeal = new float[2];

    protected float atkMng = 10;

    public void ShoftHit(Vector2 atkDist, Vector2 playerDir, float tiempoCD, float atkMng, float damageDeal)
    {
        AttackB(atkDist, playerDir, tiempoCD, atkMng, damageDeal);
    }

    public void StrongHit(Vector2 atkDist, Vector2 playerDir, float tiempoCD, float atkMng, float damageDeal)
    {
        AttackB(atkDist, playerDir, tiempoCD, atkMng, damageDeal);
    }
}
