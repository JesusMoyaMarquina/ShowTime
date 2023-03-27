using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    public float fistMng;
    public float[] fistCd;
    public Vector2[] atkDist;

    private GameObject enmClose;
    private Animator Anim;

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void ShoftHit(Vector2 atkDist, GameObject enmClose, float fistCd)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //Putaso flojo
            AttackB(atkDist, enmClose, fistCd);
        }
    }

    public void StrongHit(Vector2 atkDist, GameObject enmClose, float fistCd)
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            //Putaso flojo
            AttackB(atkDist, enmClose, fistCd);
        }
    }
}
