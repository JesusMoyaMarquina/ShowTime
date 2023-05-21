using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{

    private void Awake()
    {
        weaponType = WeaponType.Fist;
    }

    override
    public Attack Hit(string attackName, int attackCount, Rigidbody2D playerRB, string attackType)
    {
        executedAttack = attacks.Find(o => o.GetAttackName() == attackName);

        Hit(executedAttack.GetCD(), playerRB, attackCount, attackType);
        executedAttack.ActivateCollider();

        return executedAttack;
    }
}
