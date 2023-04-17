using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    protected Vector2[] atkDist = new Vector2[2];

    protected float[] tiempoCD = new float[2];
    protected float[] damageDeal = new float[2];

    protected GameObject enemyClose = null;

    protected float atkMng = 10;

    void Start()
    {
        atkDist[0] = new Vector2();
        atkDist[1] = new Vector2();

        tiempoCD[0] = 1;
        tiempoCD[1] = 1.5f;

        damageDeal[0] = 15;
        damageDeal[1] = 25;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            ShoftHit(atkDist[0], enemyClose, tiempoCD[0]);
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            ShoftHit(atkDist[1], enemyClose, tiempoCD[1]);
        }
    }

    public void ShoftHit(Vector2 atkDist, GameObject enmClose, float tiempoCD)
    {
        //Golpe flojo

        if (enemyClose != null)
            AttackB(atkDist, enemyClose, tiempoCD, atkMng, damageDeal[0]);
    }

    public void StrongHit(Vector2[] atkDist, GameObject enmClose, float fistCd)
    {
        //Putaso al suelo

        if (enemyClose != null)
            AttackB(atkDist[1], enemyClose, tiempoCD[1], atkMng, damageDeal[1]);
    }
}
