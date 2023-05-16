using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    public float[] MngList;

    private void Awake()
    {
        weaponType = WeaponType.Fist;
    }

    public List<Attack> attacks = new List<Attack>();

    Attack executedAttack = null;

    override
    public Attack Hit(string attackName, int attackCount)
    {
        executedAttack = attacks.Find(o => o.GetAttackName() == attackName);

        Hit(executedAttack.GetCD(), MngList[attackCount]);
        executedAttack.ActivateCollider();

        return executedAttack;
    }
}
