using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fist : Weapon
{
    public List<Attack> attacks = new List<Attack>();

    protected float atkMng = 10;

    Attack executedAttack = null;

    override
    public Attack Hit(Vector2 playerDir, string attackName)
    {
        executedAttack = attacks.Find(o => o.GetAttackName() == attackName);

        if (!isInCD)
        {
            Hit(playerDir, executedAttack.GetCD(), atkMng);
            executedAttack.ActivateCollider();
        }

        return executedAttack;
    }
}
