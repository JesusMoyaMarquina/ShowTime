using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    protected Vector2[] atkDist = new Vector2[2];

    protected GameObject enemyClose = null;

    protected float[] tiempoCD = new float[2];
    protected float[] damageDeal = new float[2];

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
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //Golpe flojo

            if (enemyClose != null)
                AttackB(atkDist[0], enemyClose, tiempoCD[0], atkMng, damageDeal[0]);

        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //Putaso al suelo

            if (enemyClose != null)
                AttackB(atkDist[1], enemyClose, tiempoCD[1], atkMng, damageDeal[1]);

        }
    }
}
